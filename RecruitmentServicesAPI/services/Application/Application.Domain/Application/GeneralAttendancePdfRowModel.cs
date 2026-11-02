namespace Application.Domain.Application
{
    // Row returned by SP_GetGeneralWrittenAttendancePDF (no Unit).
    public class GeneralAttendancePdfRowModel
    {
        public int Serial_Numbers { get; set; }

        public int TotalCount { get; set; }

        public string? FullName { get; set; }

        public string? Appno { get; set; }

        public string? TransactionID { get; set; }

        public string? PostName { get; set; }

        public string? ScheduledCenterName { get; set; }

        public string? ScheduledDate { get; set; }

        public string? ScheduledTime { get; set; }

        public string? HeadingLine1 { get; set; }

        public string? HeadingLine2 { get; set; }

        // Dummy default until the procedure returns it.
        public string? HeadingLine3 { get; set; } = "Attendance Muster - Sea Test";

        public byte[]? photo { get; set; }

        public byte[]? sign { get; set; }
    }
}
