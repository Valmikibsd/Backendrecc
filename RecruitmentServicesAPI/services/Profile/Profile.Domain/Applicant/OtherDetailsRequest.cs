using System;


namespace Profile.Domain.Applicant
{
    public class OtherDetailsRequest
    {
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
        public int? IsDraft { get; set; }
        public long CreatedBy { get; set; }
        public string? IPAddress { get; set; }
        public int? Gender { get; set; }
    }

}
