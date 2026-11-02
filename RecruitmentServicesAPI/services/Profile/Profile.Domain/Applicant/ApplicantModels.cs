using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Profile.Domain.Applicant;

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
    public string? AadhaarNo { get; set; }

}

public class photosign
{
    [Required]
    public IFormFile PhotoSign { get; set; }
    [Required]
    public string? documenttype { get; set; }
    public long? userid { get; set; }
    public string? FileName { get; set; }
    public byte[]? File { get; set; }
    public string? IPAddress { get; set; }
    public string? FileType { get; set; }
    public int? FileDetails { get; set; }
}

public class PhotoSignData
{
    public int FileDetails { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public byte[]? BinaryFiles { get; set; }
}

public class AddressRequest
{
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

    public int? CorrespondenceVillage { get; set; }
    [Required(ErrorMessage = "Correspondence PIN Code is required")]
    [RegularExpression(
@"^4[0-9]{5}$",
ErrorMessage = "Correspondence PIN Code must be 6 digits and start with 4."
)]
    public string? CorrespondencePinCode { get; set; }

    public int? CreatedBy { get; set; }

    public string? IPAddress { get; set; }
}
