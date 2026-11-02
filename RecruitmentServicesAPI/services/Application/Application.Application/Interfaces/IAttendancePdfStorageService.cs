namespace Application.Application.Interfaces
{
    public interface IAttendancePdfStorageService
    {
        string SavePdf(byte[] pdfBytes, string pdfPath);
    }
}
