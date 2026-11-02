using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Master.Api.Models;

public class AdvertisementPdfRequest : IValidatableObject
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
    private const string PdfExtension = ".pdf";
    private const string PdfContentType = "application/pdf";
    private static readonly byte[] PdfSignature = { 0x25, 0x50, 0x44, 0x46 }; // "%PDF"

    public long? AdvertisementPdfId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
    public int DepartmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
    public int AdvertisementId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
    public int PostId { get; set; }

    //[Required(ErrorMessage = "Unit is required.")]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Unit must be a 2 digit value such as 01.")]
    public string? UnitId { get; set; }

    [StringLength(500, ErrorMessage = "Maximum 500 characters allowed.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public IFormFile? File { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var isNewRecord = !AdvertisementPdfId.HasValue || AdvertisementPdfId.Value <= 0;

        if (File == null || File.Length == 0)
        {
            if (isNewRecord)
            {
                yield return new ValidationResult("PDF file is required.", new[] { nameof(File) });
            }
            yield break;
        }

        if (!string.Equals(Path.GetExtension(File.FileName), PdfExtension, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(File.ContentType, PdfContentType, StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult("Only PDF files are allowed.", new[] { nameof(File) });
            yield break;
        }

        if (File.Length > MaxFileSizeInBytes)
        {
            yield return new ValidationResult("File size cannot exceed 5 MB.", new[] { nameof(File) });
            yield break;
        }

        if (!HasPdfSignature(File))
        {
            yield return new ValidationResult("Invalid PDF file.", new[] { nameof(File) });
        }
    }

    private static bool HasPdfSignature(IFormFile file)
    {
        var header = new byte[PdfSignature.Length];
        using var stream = file.OpenReadStream();
        var bytesRead = stream.Read(header, 0, header.Length);
        return bytesRead == header.Length && header.SequenceEqual(PdfSignature);
    }
}
