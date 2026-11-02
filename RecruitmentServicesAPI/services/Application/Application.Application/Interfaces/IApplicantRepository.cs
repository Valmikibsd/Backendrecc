using Application.Domain.Application;
using Application.Domain.Model;
using CrudOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Application.Interfaces
{
    public interface IApplicantRepository
    {

        Task<ResponseList<AppliedPostModel>> GetCandidateAppliedPosts(long userId);
        Task<ResponseList<CancelledApplicationModel>> GetCancelledApplications(long userId);

        Task<Response<ReceiptResponse>> GetReceiptDetails( ReceiptRequest request);

        Task<Response<HallTicketModel>> GetPhysicalHallTicket(HallTicketRequest request);

        Task<Response<WrittenHallTicket>> GetWrittenHallTicket(WrittenHallTickRequest request);

        Task<Response> SendCancellationOtp(long userId, string? appNo = null);
        Task<Response> CancelApplication(CancelApplicationRequest request);
        Task<ResponseList<EligibilityCriteriaResponse>> CheckEligibilityCriteria(EligibilityCriteriaRequest obj);
        Task<ResponseList<EligibilityCriteriaResponse>> CheckApplicationEligibilityCriteria(ApplicationEligibilityCriteriaRequest obj);
        

        Task<Response<ApplicationProfileResponse>> SaveApplication(PostApplicationRequest request);
        Task<Response<ApplicationExists>> CheckApplicationExists(CheckApplicantionRequest obj);
        Task<Response<ApplicantViewApplicationResponse>> ViewApplication(ViewApplicantionRequest obj);
        Task<Response<ApplicantViewApplicationResponse>> ViewCancelledApplicationDetails(ViewApplicantionRequest obj);
        Task<Response<ApplicantCreation>> GetAadhaarDetails(long id);



        Task<Response> SaveApplicationNo(DtoApplicationNo request);

        Task<ResponseList<ApplicationNoModel>> GetApplicationNoList();


    }
}
