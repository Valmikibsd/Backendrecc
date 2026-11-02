using CrudOperations;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;

namespace Profile.Infrastructure.Applicant
{
    public class OtherDetailsRepository(ICrudOperationService crudOperationService) : IOtherDetailsRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;

        public async Task<Response> SaveOtherDetails(OtherDetailsRequest request)
        {
            var result = await _crudOperationService
                .InsertUpdateDelete<Response>(
                    "[dbo].[InsertUpdateOtherDetails]",
                    new
                    {
                        request.UserId,
                        request.Height,
                        request.Weight,
                        request.ChestDeflated,
                        request.ChestInflated,
                        request.CasteCategory,
                        request.CasteCertificateNo,
                        request.CasteIssuingDistrict,
                        request.CasteIssuingDate,
                        request.IsCasteValidityCertificate,
                        request.CasteValidityCertificateNo,
                        request.CasteValidityIssuingDate,
                        request.IsNonCreamyLayer,
                        request.NonCreamyLayerCertificateNo,
                        request.NonCreamyLayerIssuingDate,
                        request.IsDomicile,
                        request.DomicileState,
                        request.DomicileCertificateNo,
                        request.DomicileIssuingDistrict,
                        request.DomicileIssuingDate,
                        request.IsPersonWithDisability,
                        request.DisabilityCategory,
                        request.DisabilitySubCategory,
                        request.DisabilityPercentage,
                        request.DisabilityCertificateType,
                        request.DisabilityCertificateNo,
                        request.DisabilityIssuingDate,
                        request.IsUDIDCard,
                        request.UDIDNo,
                        request.IsSportsPerson,
                        request.SportsCertificateNo,
                        request.SportsIssuingDate,
                        request.IsExServiceman,
                        request.ExServicemanCertificateNo,
                        request.ExServicemanIssuingDate,
                        request.IsExServicemanDependent,
                        request.ExServicemanDependentCertificateNo,
                        request.ExServicemanDependentIssuingDate,
                        request.IsMaharashtraEmployee,
                        request.SevaarthID,
                        request.IsAdditionalReservation,
                        request.IsProjectAffected,
                        request.ProjectAffectedCertificateNo,
                        request.ProjectAffectedIssuingDate,
                        request.IsEarthquakeAffected,
                        request.EarthquakeAffectedCertificateNo,
                        request.EarthquakeAffectedIssuingDate,
                        request.IsAnshakalin,
                        request.AnshakalinCertificateNo,
                        request.AnshakalinIssuingDate,
                        request.IsPoliceChild,
                        request.PoliceChildCertificateNo,
                        request.PoliceChildIssuingDate,
                        request.IsHomeGuard,
                        request.HomeGuardCertificateNo,
                        request.HomeGuardIssuingDate,
                        request.IsWomanReservation,
                        request.WomanReservationCertificateNo,
                        request.WomanReservationIssuingDate,
                        request.IsOrphanReservation,
                        request.OrphanType,
                        request.OrphanCertificateNo,
                        request.OrphanCertificateIssuingDate,
                        request.IsDraft,
                        request.CreatedBy,
                        request.IPAddress
                    },
                    ConnectionNames.Profile);

            return result;
        }

        public async Task<Response<OtherDetailsRequest>> GetOtherDetails(long userId)
        {
            return await _crudOperationService
                .GetSingleRecord<OtherDetailsRequest>(
                    "[dbo].[GetOtherDetails]",
                    new { UserId = userId },
                    ConnectionNames.Profile);
        }
    }
}
