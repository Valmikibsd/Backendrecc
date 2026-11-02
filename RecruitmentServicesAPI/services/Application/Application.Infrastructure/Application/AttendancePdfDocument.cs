using Application.Domain.Application;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Application.Infrastructure.Application
{
    public class AttendancePdfDocument(List<AttendancePdfRowModel> rows) : IDocument
    {
        private const string LogoResourceName = "Application.Infrastructure.Assets.logo.png";
        private const float WatermarkOpacity = 0.08f;

        private readonly List<AttendancePdfRowModel> _rows = rows;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        private static byte[]? LoadLogo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(LogoResourceName);
            if (stream == null)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray(); 
        }

        private static byte[] FadeImage(byte[] imageBytes, float opacity)
        {
            using var sourceStream = new MemoryStream(imageBytes);
            using var source = new Bitmap(sourceStream);
            using var faded = new Bitmap(source.Width, source.Height);
            using var graphics = Graphics.FromImage(faded);

            var colorMatrix = new ColorMatrix
            {
                Matrix33 = opacity
            };

            using var attributes = new ImageAttributes();

            attributes.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);

            graphics.DrawImage(
                source,
                new Rectangle(0, 0, source.Width, source.Height),
                0, 0,
                source.Width,
                source.Height,
                GraphicsUnit.Pixel,
                attributes);

            using var outputStream = new MemoryStream();
            faded.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);

            return outputStream.ToArray();
        }

        public void Compose(IDocumentContainer container)
        {
            var first = _rows.First();
            var logo = LoadLogo();
             
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(9));
                if (logo != null)
                {
                    var watermark = FadeImage(logo, WatermarkOpacity);

                    page.Background()
                        .AlignCenter()
                        .AlignMiddle()
                        .Width(300)
                        .Image(watermark)
                        .FitWidth();
                }
                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.ConstantItem(60).Height(60).Element(container =>
                        {
                            if (logo != null)
                            {
                                container.Image(logo).FitArea();
                            }
                        });

                        row.RelativeItem().AlignCenter().AlignMiddle().Column(heading =>
                        {
                            heading.Item().AlignCenter().Text(first.HeadingLine1).FontSize(14).Bold();
                            heading.Item().AlignCenter().Text(first.HeadingLine2).FontSize(12).Bold();
                            heading.Item().AlignCenter().Text(first.HeadingLine3).FontSize(10).Bold();
                        });

                        row.ConstantItem(60).Height(60).Element(container =>
                        {
                            if (logo != null)
                            {
                                container.Image(logo).FitArea();
                            }
                        });
                    });

                    header.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn(2);
                        });

                        table.Cell().Border(1).Padding(3).Text("Unit").Bold();
                        table.Cell().Border(1).Padding(3).Text(first.UnitName);
                        table.Cell().Border(1).Padding(3).Text("Post").Bold();
                        table.Cell().Border(1).Padding(3).Text(first.PostName);
                        table.Cell().Border(1).Padding(3).Text("Date").Bold();
                        table.Cell().Border(1).Padding(3).Text(first.ScheduledDate);
                        table.Cell().Border(1).Padding(3).Text("Time").Bold();
                        table.Cell().Border(1).Padding(3).Text(first.ScheduledTime);

                        table.Cell().Border(1).Padding(3).Text("Address").Bold();
                        table.Cell().ColumnSpan(7).Border(1).Padding(3).AlignCenter().Text(first.ScheduledCenterName);
                    });
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(5);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(4);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Border(1).Padding(3).AlignCenter().Text("Sr.No").Bold();
                        header.Cell().Border(1).Padding(3).AlignCenter().Text("Candidate Name\nApplication No.\nAadhaar No.").Bold();
                        header.Cell().Border(1).Padding(3).AlignCenter().Text("Candidate Photo\nSignature").Bold();
                        header.Cell().Border(1).Padding(3).AlignCenter().Text("Spot Photo").Bold();
                        header.Cell().Border(1).Padding(3).AlignCenter().Text("Candidate Signature").Bold();
                    });

                    foreach (var row in _rows)
                    {
                        // Sr. No.
                        table.Cell().Border(1).Padding(3).AlignCenter().AlignMiddle().Text(row.Serial_Numbers.ToString());
                        // Name + Application No.
                        table.Cell().Border(1).Padding(3).AlignCenter().AlignMiddle()
                            .Column(cell =>
                            {
                                cell.Item().AlignCenter().Text(row.FullName).Bold();

                                cell.Item().AlignCenter().Text(row.Appno);
                            });

                        // Photo + Signature
                        table.Cell().Border(1).Padding(3).AlignCenter().AlignMiddle()
                            .Column(cell =>
                            {
                                if (row.photo is { Length: > 0 })
                                {
                                    cell.Item().AlignCenter().Height(80).Image(row.photo).FitArea();
                                }

                                if (row.sign is { Length: > 0 })
                                {
                                    cell.Item().AlignCenter().Height(30).Image(row.sign).FitArea();
                                }
                            });

                        // Empty cells
                        table.Cell().Border(1).Padding(3).AlignCenter().AlignMiddle().Text(string.Empty);

                        table.Cell().Border(1).Padding(3).AlignCenter().AlignMiddle().Text(string.Empty);
                    }
                });

                page.Footer().AlignCenter()
                .Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                });
            });
        }
    }
}
