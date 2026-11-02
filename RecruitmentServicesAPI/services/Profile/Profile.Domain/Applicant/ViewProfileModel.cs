using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Domain.Applicant
{
    public class ViewProfileModel
    {
        public string AadhaarNo { get; set; }
        public string AadhaarName { get; set; }
        public string AadhaarDOB { get; set; }
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SurName { get; set; }
        public string FullNameEnglish { get; set; }
        public string FullNameMarathi { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }

        public string MaritalStatus { get; set; }
        public string Nationality { get; set; }
        public string MotherTongue { get; set; }
        public string NameChange { get; set; }
        public string FullChangedName { get; set; }
        public string OtherMotherTongue { get; set; }
        public byte[]? Photo { get; set; }
        public byte[]? Sign { get; set; }

        //public string FirstNameMarathi { get; set; }
        //public string FatherNameMarathi { get; set; }
        //public string MotherNameMarathi { get; set; }
        //public string SurNameMarathi { get; set; }

        public string Gender { get; set; }
        public DateTime? DOB { get; set; }

        public string Religion { get; set; }
        public string Category { get; set; }
        public string Caste { get; set; }

        public string CasteCertificateNo { get; set; }
        public DateTime? CasteCertificateDate { get; set; }

        public string ParallelReservation { get; set; }
        public string ParallelReservationNo { get; set; }
        public DateTime? ParallelReservationDate { get; set; }

        public bool? ExService15YearsCompleted { get; set; }
        public DateTime? ExServiceManServiceJoinDate { get; set; }

        public bool? IsNCLCertificate { get; set; }
        public string NCLCertificateNo { get; set; }
        public DateTime? NCLCertificateDate { get; set; }

        public bool? IsMaharshtraDomicile { get; set; }
        public string MaharshtraDomicileCertificateNo { get; set; }
        public DateTime? MaharshtraDomicileCertificateDate { get; set; }

        public bool? IsKarnatakaDomicile { get; set; }
        public string KarnatakaDomicileCertificateNo { get; set; }
        public DateTime? KarnatakaDomicileCertificateDate { get; set; }

        public bool? IsAnnath { get; set; }
        public string AnathType { get; set; }
        public DateTime? AnnathCertificateDate { get; set; }

        public bool? IsNCC { get; set; }
        public string NCCCertificateNo { get; set; }
        public DateTime? NCCCertificateDate { get; set; }

        public bool? IsFarmerSucide { get; set; }
        public string FarmerSucideCertificateNo { get; set; }
        public DateTime? FarmerSucideCertificateDate { get; set; }

        public bool? IsPhysicallyFit { get; set; }
        public bool? IsMedicallyFit { get; set; }
        public bool? IsReadyToWorkonContract { get; set; }
        public bool? IsSmallVehicleLicence { get; set; }

        public bool? IsGovernmentEmployee { get; set; }
        public string GovernmentOfficeName { get; set; }
        public string GovernmentOfficeDistrict { get; set; }

        public bool? IsComputerKnowledge
        {
            get; set;

        }
    }

    public class ApplicantViewProfileResponse
    {
        public object? PersonalDetails { get; set; }

        public object? EducationDetails { get; set; }
        public object? AddressDetails { get; set; }
        public object? OtherDetails { get; set; }
    }
}
