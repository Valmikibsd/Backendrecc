using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Domain.Applicant
{
    public class PersonalDetailsRequest
    {
        public long UserId { get; set; }

        public string? AadhaarNo { get; set; }

        public string? AadhaarName { get; set; }

        public DateTime? AadhaarDOB { get; set; }

        public int? Gender { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "First Name can contain only English letters and spaces.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "Father Name can contain only English letters and spaces.")]
        public string FatherName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "Mother Name can contain only English letters and spaces.")]
        public string MotherName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "SurNam can contain only English letters and spaces.")]
        public string SurName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "FullNameEnglish can contain only English letters and spaces.")]
        public string FullNameEnglish { get; set; }

        [Required]
        [RegularExpression(
    @"^[\u0900-\u097F]+(?:\s[\u0900-\u097F]+)*$",
    ErrorMessage = "Full Name in Marathi can contain only Marathi letters and spaces.")]
        public string FullNameMarathi { get; set; }

        [Required]
        [RegularExpression(
    @"^[6-9][0-9]{9}$",
    ErrorMessage = "Mobile number must be a valid 10-digit number.")]
        public string MobileNo { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailId { get; set; }

        [Required]
        public int MaritalStatusId { get; set; }

        [Required]
        public int NationalityId { get; set; }

        [Required]
        public int MotherToungueId { get; set; }
       
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "OtherMotherTongue can contain only English letters and spaces.")]
        public string? OtherMotherTongue { get; set; }

        public int NameChange { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "FullChangedName can contain only English letters and spaces.")]
        public string? FullChangedName { get; set; }

        public int? isDraft { get; set; }
        public int IsProfileComplete { get; set; }
        public int CreatedBy { get; set; }

        public string? IPAddress { get; set; }
    }

    public class PersonalDetailsDraftRequest
    {
        public long UserId { get; set; }

        public string? AadhaarNo { get; set; }

        public string? AadhaarName { get; set; }

        [ValidPassingDateAttribute]
        public DateTime? AadhaarDOB { get; set; }

        public int? Gender { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "First Name can contain only English letters and spaces.")]
        public string? FirstName { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "Father Name can contain only English letters and spaces.")]
        public string? FatherName { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "Mother Name can contain only English letters and spaces.")]
        public string? MotherName { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "SurName Name can contain only English letters and spaces.")]
        public string? SurName { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "FullName English can contain only English letters and spaces.")]
        public string? FullNameEnglish { get; set; }

        [RegularExpression(
    @"^[\u0900-\u097F]+(?:\s[\u0900-\u097F]+)*$",
    ErrorMessage = "Full Name in Marathi can contain only Marathi letters and spaces.")]
        public string? FullNameMarathi { get; set; }

        [RegularExpression(
    @"^[6-9][0-9]{9}$",
    ErrorMessage = "Mobile number must be a valid 10-digit number.")]
        public string? MobileNo { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? EmailId { get; set; }

 
        public int? NationalityId { get; set; }

        public int? MaritalStatusId { get; set; }


        public int? MotherToungueId { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "OtherMotherTongue can contain only English letters and spaces.")]
        public string? OtherMotherTongue { get; set; }
        public int? NameChange { get; set; }

        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "FullChangedName can contain only English letters and spaces.")]
        public string? FullChangedName { get; set; }

        public int? isDraft { get; set; }
        public int CreatedBy { get; set; }

        public string? IPAddress { get; set; }
    }
}
