namespace Application.Domain.Application
{
    public class WrittenMusterModel
    {
        public string? PostName { get; set; }

        public string? UnitName { get; set; }

        public string? CenterName { get; set; }

        public string? ScheduledDate { get; set; }

        public string? ScheduledTime { get; set; }

        public string? PDFName { get; set; }

        public string? PDFPath { get; set; }

        public string? StartAppNo { get; set; }

        public string? EndAppNo { get; set; }

        public int FilterId { get; set; }

        public int Count { get; set; }
    }
}
