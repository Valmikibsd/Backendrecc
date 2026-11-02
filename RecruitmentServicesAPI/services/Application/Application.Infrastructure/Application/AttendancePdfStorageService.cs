using Application.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Application.Infrastructure.Application
{
    public class AttendancePdfStorageService(IConfiguration configuration) : IAttendancePdfStorageService
    {
        private const string AttendanceMasterMarker = "AttendanceMaster/";

        private readonly IConfiguration _configuration = configuration;

        public string SavePdf(byte[] pdfBytes, string pdfPath)
        {
            var root = _configuration["FileStorage:AttendanceMasterRoot"]
                ?? throw new InvalidOperationException("FileStorage:AttendanceMasterRoot is not configured.");

            var fullPath = ResolvePhysicalPath(root, pdfPath);

            var folder = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllBytes(fullPath, pdfBytes);

            return fullPath;
        }

        private static string ResolvePhysicalPath(string root, string pdfPath)
        {
            var normalized = pdfPath.Replace('\\', '/');

            var markerIndex = normalized.IndexOf(AttendanceMasterMarker, StringComparison.OrdinalIgnoreCase);
            var relative = markerIndex >= 0
                ? normalized[(markerIndex + AttendanceMasterMarker.Length)..]
                : normalized.TrimStart('.', '/');

            var relativePath = relative.Replace('/', Path.DirectorySeparatorChar);

            return Path.Combine(root, relativePath);
        }
    }
}
