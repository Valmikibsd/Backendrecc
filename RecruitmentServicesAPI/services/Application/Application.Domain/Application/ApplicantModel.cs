using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Model
{
    public class ApplicantModel
    {


    }


    public class AppliedPostModel
    {
        public string? AppNo { get; set; }
        public string? DepartmentName { get; set; }
        public string? AdvertisementName { get; set; }
        public string? PostName { get; set; }
        public string? CandidateName { get; set; }
        public DateTime AppliedDate { get; set; }
        public string? PaymentStatus { get; set; }

        public string? PaymentTransactionId { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public decimal Fees { get; set; }

        public string? EmailId { get; set; }

        public string? MobileNo { get; set; }

        public string? PayServiceId { get; set; }

        public bool? IsPhysicalHallticketPublished { get; set; }
        public bool? IsWrittenHallticketPublished { get; set; }

        public DateTime? PaymentStartDate { get; set; }
        public DateTime? PaymentEndDate { get; set; }
        public DateTime? CancellationEndDate { get; set; }
    }

    public class CancelledApplicationModel
    {
        public string? AppNo { get; set; }
        public string? DepartmentName { get; set; }
        public string? AdvertisementName { get; set; }
        public string? PostName { get; set; }
        public string? CandidateName { get; set; }
        public DateTime AppliedDate { get; set; }
        public DateTime CancelledDate { get; set; }
        public string? PaymentStatus { get; set; }
        public int DepartmentId { get; set; }
        public int AdvertisementId { get; set; }
        public int PostId { get; set; }
        public decimal Fees { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
    }

    public class AuthOtpExpiryModel
    {
        public int OtpExpiryMinutes { get; set; }
    }

    public class CandidateAppliedPostRequest
    {
        public long UserId { get; set; }
    }

    public class SendCancellationOtpRequest
    {
        [Required(ErrorMessage = "CAPTCHA verification token is required.")]
        public string? CaptchaToken { get; set; }

        [Required(ErrorMessage = "Application number is required.")]
        [StringLength(50, ErrorMessage = "Application number cannot exceed 50 characters.")]
        public string? AppNo { get; set; }
    }

    public class CancelApplicationRequest
    {
        public long UserId { get; set; }
        public string? CancelledIpAddress { get; set; }

        [Required(ErrorMessage = "Application number is required.")]
        [StringLength(50, ErrorMessage = "Application number cannot exceed 50 characters.")]
        public string? AppNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId must be greater than zero.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than zero.")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits.")]
        public string? Otp { get; set; }

    }

    public class EligibilityCriteriaRequest { 
        public long? UserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int? DepartmentId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId must be greater than zero.")]
        public int? AdvertisementId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than zero.")]
        public int? PostId { get; set; } 
        public int? GenderId { get; set; } 
    }
    public class ApplicationEligibilityCriteriaRequest { 
        public long? UserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int? DepartmentId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId must be greater than zero.")]
        public int? AdvertisementId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than zero.")]
        public int? PostId { get; set; } 
        public int? GenderId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ParallelReservationId must be greater than zero.")]
        [Required(ErrorMessage = "ParallelReservation is required")]
        public int? ParallelReservationId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than zero.")]
        [Required(ErrorMessage = "CategoryId is required")]
        public int? CategoryId { get; set; } 
        public string? Unit { get; set; } 
    }
    public class EligibilityCriteriaResponse
    {
        public bool IsEligible { get; set; }
        public string? Message { get; set; }
        public string? EligibilityStatus { get; set; }
        public string? Type { get; set; }
    }

    public class PostApplicationRequest
    {
        public string? UnitId { get; set; }
        [Required(ErrorMessage = "ParelledReservation is required")]
        public int ParelledReservationId { get; set; }
        [Required(ErrorMessage = "Postid is required")]
        public int Postid { get; set; }
        [Required(ErrorMessage = "DepartmentId is required")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "AdvertisementId is required")]
        public int AdvertisementId { get; set; }
        public bool IsUnit { get; set; }

        public string? Photo { get; set; }



        public string? PhotofileName { get; set; }

        public string? PhotofileType { get; set; }

        public string? Sign { get; set; }



        public string? SignfileName { get; set; }

        public string? SignfileType { get; set; }
        public PersonalDetailsRequest PersonalInformation { get; set; }

        public OtherDetailsRequest OtherDetails { get; set; }

        public AddressRequest AddressDetails { get; set; }

        public List<EducationRequest> EducationDetails { get; set; }
            = new();
    }

    public class AddressRequest
    {

        public string? UnitId { get; set; }

        public int? Postid { get; set; }

        public int? DepartmentId { get; set; }

        public int? AdvertisementId { get; set; }

        public string? ApplicationNo { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Permanent Address is required")]
        [StringLength(300, ErrorMessage = "Permanent Address cannot exceed 300 characters.")]
        [RegularExpression(
    @"^[a-zA-Z0-9\s(),./\[\]_\-&]{1,300}$",
    ErrorMessage = "Only letters, numbers, spaces and (),./[]-_& are allowed in Permanent Address."
    )]
        public string? PermanentAddress1 { get; set; }
        [Required]
        public int PermanentStateId { get; set; }
        [Required]
        public int PermanentDistrictId { get; set; }
        [Required]
        public int PermanentTalukaId { get; set; }
        //[Required(ErrorMessage = "Permanent Village is required")]
        //    [RegularExpression(
        //@"^[a-zA-Z\u0900-\u097F]+(?:[\s][a-zA-Z\u0900-\u097F]+)*$",
        //ErrorMessage = "Only alphabets and spaces are allowed."
        //)]
        public int? PermanentVillage { get; set; }
        [Required(ErrorMessage = "Permanent PIN Code is required")]
        [RegularExpression(
    @"^4[0-9]{5}$",
    ErrorMessage = "Permanent PIN Code must be 6 digits and start with 4."
    )]
        public string? PermanentPinCode { get; set; }

        public int? IsPermanentSameasCorrespondence { get; set; }

        // --------------  CorrespondenceAddress---

        [Required(ErrorMessage = "Correspondence Address is required")]
        [StringLength(300, ErrorMessage = "Correspondence Address cannot exceed 300 characters.")]
        [RegularExpression(
    @"^[a-zA-Z0-9\s(),./\[\]_\-&]{1,300}$",
    ErrorMessage = "Only letters, numbers, spaces and (),./[]-_& are allowed in Correspondence Address."
    )]
        public string? CorrespondenceAddress1 { get; set; }
        [Required]
        public int CorrespondenceStateId { get; set; }
        [Required]
        public int CorrespondenceDistrictId { get; set; }
        [Required]
        public int CorrespondenceTalukaId { get; set; }

        //[Required(ErrorMessage = "Correspondence Village is required")]
        //    [RegularExpression(
        //@"^[a-zA-Z\u0900-\u097F]+(?:[\s][a-zA-Z\u0900-\u097F]+)*$",
        //ErrorMessage = "Only alphabets and spaces are allowed."
        //)]
        public int? CorrespondenceVillage { get; set; }
        [Required(ErrorMessage = "Correspondence PIN Code is required")]
        [RegularExpression(
    @"^4[0-9]{5}$",
    ErrorMessage = "Correspondence PIN Code must be 6 digits and start with 4."
    )]

        public string? CorrespondencePinCode { get; set; }

        public long? CreatedBy { get; set; }

        public string? IPAddress { get; set; }
    }

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
        public DateTime? PassingDate { get; set; }

        [Required]
        [ValidPercentageAttribute]
        public decimal Percentage { get; set; }
        [Required]
        public int QualificationModeId { get; set; }


        public int? CreatedBy { get; set; }

        public string? IPAddress { get; set; }


    }


    public class PersonalDetailsRequest
    {
        public long UserId { get; set; }

        public string? AadhaarNo { get; set; }

        public string? AadhaarName { get; set; }

        public DateTime? AadhaarDOB { get; set; }

        public string? UnitId { get; set; }

        public int? Postid { get; set; }
        public int? DepartmentId { get; set; }
        public int? ParelledReservationId { get; set; }
        public int? AdvertisementId { get; set; }
        public int? Category { get; set; }
        public int? Gender { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "First Name can contain only English letters and spaces.")]
        public string? FirstName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "Father Name can contain only English letters and spaces.")]
        public string? FatherName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "Mother Name can contain only English letters and spaces.")]
        public string? MotherName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "SurNam can contain only English letters and spaces.")]
        public string? SurName { get; set; }

        [Required]
        [RegularExpression(
        @"^[A-Za-z]+(?:\s[A-Za-z]+)*$",
        ErrorMessage = "FullNameEnglish can contain only English letters and spaces.")]
        public string? FullNameEnglish { get; set; }

        [Required]
        [RegularExpression(
    @"^[\u0900-\u097F]+(?:\s[\u0900-\u097F]+)*$",
    ErrorMessage = "Full Name in Marathi can contain only Marathi letters and spaces.")]
        public string? FullNameMarathi { get; set; }

        [Required]
        [RegularExpression(
    @"^[6-9][0-9]{9}$",
    ErrorMessage = "Mobile number must be a valid 10-digit number.")]
        public string? MobileNo { get; set; }

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



        public string? IPAddress { get; set; }
    }

    public class OtherDetailsRequest
    {
        public string? UnitId { get; set; }

        public int? Postid { get; set; }
        public int? DepartmentId { get; set; }

        public int? AdvertisementId { get; set; }
        public string? ApplicationNo { get; set; }
        public long UserId { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? ChestDeflated { get; set; }
        public decimal? ChestInflated { get; set; }
        public int? CasteCategory { get; set; }
        public string? CasteCertificateNo { get; set; }
        public int? CasteIssuingDistrict { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? CasteIssuingDate { get; set; }
        public bool? IsCasteValidityCertificate { get; set; }
        public string? CasteValidityCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? CasteValidityIssuingDate { get; set; }
        public bool? IsNonCreamyLayer { get; set; }
        public string? NonCreamyLayerCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? NonCreamyLayerIssuingDate { get; set; }
        public bool? IsDomicile { get; set; }
        public int? DomicileState { get; set; }
        public string? DomicileCertificateNo { get; set; }
        public int? DomicileIssuingDistrict { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? DomicileIssuingDate { get; set; }
        public bool? IsPersonWithDisability { get; set; }
        public int? DisabilityCategory { get; set; }
        public int? DisabilitySubCategory { get; set; }
        public decimal? DisabilityPercentage { get; set; }
        public int? DisabilityCertificateType { get; set; }
        public string? DisabilityCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? DisabilityIssuingDate { get; set; }
        public bool? IsUDIDCard { get; set; }
        public string? UDIDNo { get; set; }
        public bool? IsSportsPerson { get; set; }
        public string? SportsCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? SportsIssuingDate { get; set; }
        public bool? IsExServiceman { get; set; }
        public string? ExServicemanCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? ExServicemanIssuingDate { get; set; }
        public bool? IsExServicemanDependent { get; set; }
        public string? ExServicemanDependentCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? ExServicemanDependentIssuingDate { get; set; }
        public bool? IsMaharashtraEmployee { get; set; }
        public string? SevaarthID { get; set; }
        public bool? IsAdditionalReservation { get; set; }
        public bool? IsProjectAffected { get; set; }
        public string? ProjectAffectedCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? ProjectAffectedIssuingDate { get; set; }
        public bool? IsEarthquakeAffected { get; set; }
        public string? EarthquakeAffectedCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? EarthquakeAffectedIssuingDate { get; set; }
        public bool? IsAnshakalin { get; set; }
        public string? AnshakalinCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? AnshakalinIssuingDate { get; set; }
        public bool? IsPoliceChild { get; set; }
        public string? PoliceChildCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? PoliceChildIssuingDate { get; set; }
        public bool? IsHomeGuard { get; set; }
        public string? HomeGuardCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? HomeGuardIssuingDate { get; set; }
        public bool? IsWomanReservation { get; set; }
        public string? WomanReservationCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? WomanReservationIssuingDate { get; set; }
        public bool? IsOrphanReservation { get; set; }
        public int? OrphanType { get; set; }
        public string? OrphanCertificateNo { get; set; }
        [ValidPassingDateAttribute]
        public DateTime? OrphanCertificateIssuingDate { get; set; }

        public string? IPAddress { get; set; }
       // public int? Gender { get; set; }
    }

    public class ApplicationProfileResponse
    {
        public string? AppNO { get; set; }

        public string? Message { get; set; }

        public bool Status { get; set; }
    }

    public class ApplicationExists
    {
        public int IsApplicationExists { get; set; }
    }


    public class ApplicantViewApplicationResponse
    {
        public object? PersonalDetails { get; set; }

        public object? EducationDetails { get; set; }
        public object? AddressDetails { get; set; }
        public object? OtherDetails { get; set; }
    }

    public class ViewApplicantionRequest
    {

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }
        public int? UnitId { get; set; }
        public long? UserId { get; set; }

        public string ApplicationNo { get; set; }
    }
    public class CheckApplicantionRequest
    {

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public long? UserId { get; set; }


    }
    public class ApplicantCreation
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameMarathi { get; set; }
        //public string Image { get; set; }
        public byte[] Image { get; set; }
        public string? DOB { get; set; }
        public string? Gender { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }


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
