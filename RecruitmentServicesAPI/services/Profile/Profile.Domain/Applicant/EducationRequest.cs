using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Domain.Applicant
{
    public class EducationRequest
    {
        public long? UserId { get; set; }
        public long? Eid { get; set; }

        [Required]
        public int DegreeID { get; set; }

        [Required]
        public int CourseId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public int StateId { get; set; }

        [Required]
        public int UniversityId { get; set; }

        [Required]
        public int ResultId { get; set; }

        [Required]
        [ValidPassingDateAttribute]
        public DateTime PassingDate { get; set; }

        [Required]
        [ValidPercentageAttribute]
        public decimal Percentage { get; set; }
        [Required]
        public int QualificationModeId { get; set; }

        
        public int? CreatedBy { get; set; }

        public string? IPAddress { get; set; }

       
    }

    public class EducationDetailsResponse
    {
        public long UserId { get; set; }

        public long Eid { get; set; }
        public int DegreeID { get; set; }
        public string? Degree { get; set; }

        public int CourseId { get; set; }
        public string? Course { get; set; }
        public int SubjectId { get; set; }
        public string? Subject { get; set; }
        public int StateId { get; set; }
        public string? State { get; set; }

        public string? University { get; set; }
        public int UniversityID { get; set; }

        public int ResultId { get; set; }
        public string? ResultName { get; set; }

        public int QualificationModeId { get; set; }
        public string? QualificationMode { get; set; }

        public decimal Percentage { get; set; }

        public string? PassingDate { get; set; }
    }

    public class CheckDegreeCourseResponse
    {
        public bool available { get; set; }
        
        
    }

    public class ValidPassingDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (value is not DateTime passingDate)
                return false;

            DateTime minDate = new DateTime(1950, 1, 1);
            DateTime maxDate = DateTime.Today;

            return passingDate.Date >= minDate &&
                   passingDate.Date <= maxDate;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between 01/01/1950 and {DateTime.Today:dd/MM/yyyy}.";
        }
    }

    public class ValidPercentageAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (!decimal.TryParse(value.ToString(), out decimal percentage))
                return false;

            if (percentage < 0 || percentage > 100)
                return false;

            return decimal.Round(percentage, 2) == percentage;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between 0 and 100 with maximum 2 decimal places.";
        }
    }
}
