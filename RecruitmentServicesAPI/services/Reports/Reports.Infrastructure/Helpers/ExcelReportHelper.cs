using ClosedXML.Excel;
using System.IO;

namespace Reports.Infrastructure.Helpers;

public static class ExcelReportHelper
{
    private static readonly XLColor HeaderBackgroundColor = XLColor.FromHtml("#1E3A8A");
    private static readonly XLColor HeaderTextColor = XLColor.White;
    private static readonly XLColor BorderColor = XLColor.FromHtml("#CCCCCC");

    /// <summary>
    /// Applies standardized header styling to row 1 of a worksheet.
    /// </summary>
    public static void ApplyHeaderStyle(IXLWorksheet worksheet, string[] headers)
    {
        for (int col = 0; col < headers.Length; col++)
        {
            var cell = worksheet.Cell(1, col + 1);
            cell.Value = headers[col];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = HeaderBackgroundColor;
            cell.Style.Font.FontColor = HeaderTextColor;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = BorderColor;
        }
    }

    /// <summary>
    /// Applies standard light thin border to a specific row's cells up to specified column count.
    /// </summary>
    public static void ApplyRowBorders(IXLWorksheet worksheet, int rowIndex, int totalColumns)
    {
        for (int col = 1; col <= totalColumns; col++)
        {
            var cell = worksheet.Cell(rowIndex, col);
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = BorderColor;
        }
    }

    /// <summary>
    /// Saves workbook content to a byte array.
    /// </summary>
    public static byte[] ToByteArray(IXLWorkbook workbook)
    {
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
