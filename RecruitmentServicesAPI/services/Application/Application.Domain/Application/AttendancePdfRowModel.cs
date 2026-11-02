namespace Application.Domain.Application
{
    public class AttendancePdfRowModel
    {
        public int Serial_Numbers { get; set; }

        public int TotalCount { get; set; }

        public string? FullName { get; set; }

        public string? Appno { get; set; }

        public string? TransactionID { get; set; }

        public string? PostName { get; set; }

        public string? UnitName { get; set; }

        public string? ScheduledCenterName { get; set; }

        public string? ScheduledDate { get; set; }

        public string? ScheduledTime { get; set; }

        public string HeadingLine1 { get; set; } = "Government of Maharashtra - Home Department";

        public string HeadingLine2 { get; set; } = "Maharashtra Police Recruitment 2026";
        public string HeadingLine3 { get; set; } = "Attendance Muster - Sea Test";

        public byte[]? photo { get; set; }

        public byte[]? sign { get; set; }
    }
}
