using System;

namespace Application.Domain.Application
{
    public class WrittenHallTicket
    {
        public string? RecruitmentTitle { get; set; }
        public string? OfficeName { get; set; }
        public string? Title { get; set; }

        public string? ExamDate { get; set; }
        public string? ExamTime { get; set; }
        public string? ExamPlace { get; set; }

        public string? CandidateNameMarathi { get; set; }
        public string? CandidateName { get; set; }

        public string? PostName { get; set; }
        public string? PostNameMarathi { get; set; }

        public string? ApplicationNo { get; set; }

        public string? Address { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }

        public string? SeatNumber { get; set; }
        public string? CenterCode { get; set; }

        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }

        public string? Category { get; set; }
        public string? NonCreamyLayer { get; set; }
        public string? ParallelReservation { get; set; }

        public byte[]? Photo { get; set; }
        public string? PhotoFileType { get; set; }

        public byte[]? Signature { get; set; }
        public string? SignatureFileType { get; set; }

        public string? Instruction { get; set; }
    }
}