using ClosedXML.Excel;
using ClientsideEncryption;
using CrudOperations;
using Master.Application.Interfaces;
using Master.Domain.Master;
using Master.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Master.Infrastructure.Repositories
{
    public class MasterRepository(ICrudOperationService crudOperationService, IConfiguration configuration) : IMasterRepository
    {

        private readonly ICrudOperationService _crudOperationService = crudOperationService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ResponseList<DepartmentDetailsModel>> GetDepartmentDetails()
        {
            var response= await _crudOperationService.GetList<DepartmentDetailsModel>("[dbo].[Usp_GetDepartmentDetails]", new {});
            return response;
        }

        public async Task<ResponseList<DepartmentAdvertisementModel>> GetDepartmentAdvertisements(int departmentId)
        {
            return await _crudOperationService.GetList<DepartmentAdvertisementModel>(
                "[dbo].[Usp_GetDepartmentAdvertisementsDetails]",
                new
                {
                    DepartmentId = departmentId
                });
        }

        public async Task<ResponseList<DepartmentPostModel>> GetDepartmentPosts(int departmentId, int advertisementId)
        {
            return await _crudOperationService.GetList<DepartmentPostModel>(
                "[dbo].[Usp_GetDepartmentPostDetails]",
                new
                {
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId
                });
        }

        public async Task<Response> SaveAdvertisementPost(DtoAdvertisementPost dtoAdvertisementPost)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[ManageAdvertisementPost]",
                new
                {
                    Type = 1,
                    AdvertisementPostId = 0,
                    DepartmentId = dtoAdvertisementPost.DepartmentId,
                    AdvertisementId = dtoAdvertisementPost.AdvertisementId,
                    PostId = dtoAdvertisementPost.PostId,
                    RecruitmentStartDate = dtoAdvertisementPost.RecruitmentStartDate,
                    RecruitmentEndDate = dtoAdvertisementPost.RecruitmentEndDate,
                    PaymentStartDate = dtoAdvertisementPost.PaymentStartDate,
                    PaymentEndDate = dtoAdvertisementPost.PaymentEndDate,
                    CancellationEndDate = dtoAdvertisementPost.CancellationEndDate,
                    AgeOnDate = dtoAdvertisementPost.AgeOnDate,
                    Vacancies = dtoAdvertisementPost.Vacancies,
                    Qualification = dtoAdvertisementPost.Qualification,
                    PayServiceId = dtoAdvertisementPost.PayServiceId,
                    IsActive = dtoAdvertisementPost.IsActive,
                    CreatedBy = dtoAdvertisementPost.CreatedBy,
                    UpdatedBy = dtoAdvertisementPost.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save advertisement post." };
            }

            return response;
        }

        public async Task<Response> UpdateAdvertisementPost(DtoAdvertisementPost dtoAdvertisementPost)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[ManageAdvertisementPost]",
                new
                {
                    Type = 2,
                    AdvertisementPostId = dtoAdvertisementPost.AdvertisementPostId,
                    DepartmentId = dtoAdvertisementPost.DepartmentId,
                    AdvertisementId = dtoAdvertisementPost.AdvertisementId,
                    PostId = dtoAdvertisementPost.PostId,
                    RecruitmentStartDate = dtoAdvertisementPost.RecruitmentStartDate,
                    RecruitmentEndDate = dtoAdvertisementPost.RecruitmentEndDate,
                    PaymentStartDate = dtoAdvertisementPost.PaymentStartDate,
                    PaymentEndDate = dtoAdvertisementPost.PaymentEndDate,
                    CancellationEndDate = dtoAdvertisementPost.CancellationEndDate,
                    AgeOnDate = dtoAdvertisementPost.AgeOnDate,
                    Vacancies = dtoAdvertisementPost.Vacancies,
                    Qualification = dtoAdvertisementPost.Qualification,
                    PayServiceId = dtoAdvertisementPost.PayServiceId,
                    IsActive = dtoAdvertisementPost.IsActive,
                    CreatedBy = dtoAdvertisementPost.CreatedBy,
                    UpdatedBy = dtoAdvertisementPost.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to update advertisement post." };
            }

            return response;
        }

        public async Task<ResponseList<AdvertisementPostModel>> GetAdvertisementPostList(DtoAdvertisementPostList request)
        {
            return await _crudOperationService.GetList<AdvertisementPostModel>(
                "[dbo].[Usp_GetAdvertisementPostList]",
                new
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Search = request.Search
                });
        }


      
        public async Task<ResponseList<PostMasterModel>> GetDepartmentPosts()
        {
            return await _crudOperationService.GetList<PostMasterModel>(
                "[dbo].[Usp_GetDepartmentPosts]",
                new { });
        }

  

        public async Task<Response<AdvertisementPostModel>> GetAdvertisementPostById(int advertisementPostId)
        {
            var response = await _crudOperationService
                .GetSingleRecord<AdvertisementPostModel>(
                    "[dbo].[Usp_GetAdvertisementPostById]",
                    new
                    {
                        AdvertisementPostId = advertisementPostId
                    });

            return response;
        }


        public async Task<ResponseList<DtoMenuItem>> GetMenuByRoleId(int roleId)
        {
            try
            {
                return await _crudOperationService.GetList<DtoMenuItem>("[UspGetMenuByRoleId]", new { RoleId = roleId });
            }
            catch (Exception)
            {
                return new ResponseList<DtoMenuItem>
                {
                    Status = false,
                    Message = "Failed to get menu due to an internal error.",
                    Data = new List<DtoMenuItem>()
                };
            }
        }

        public async Task<ResponseList<DtoRoleItem>> GetRoleList()
        {
            try
            {
                return await _crudOperationService.GetList<DtoRoleItem>("[UspGetRoleList]", new { });
            }
            catch (Exception)
            {
                return new ResponseList<DtoRoleItem>
                {
                    Status = false,
                    Message = "Failed to get role list due to an internal error.",
                    Data = new List<DtoRoleItem>()
                };
            }
        }


        public async Task<ResponseList<DepartmentCandidatePostModel>> GetDepartmentCandidatePostDetails(long userId,int departmentId, int advertisementId)
        {
            return await _crudOperationService.GetList<DepartmentCandidatePostModel>(
                "[dbo].[Usp_GetDepartmentCandidatePostDetails]",
                new
                {
                    UserId = userId,
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId
                });
        }

        public async Task<Response> SaveDepartment(DtoDepartment dtoDepartment)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdateDepartment]",
                new
                {
                    DepartmentId = 0,
                    DepartmentCode = dtoDepartment.DepartmentCode,
                    DepartmentName = dtoDepartment.DepartmentName,
                    DepartmentName_Mr = dtoDepartment.DepartmentName_Mr,
                    DepartmentType = dtoDepartment.DepartmentType,
                    Description = dtoDepartment.Description,
                    Logo = dtoDepartment.Logo,
                    IsActive = dtoDepartment.IsActive,
                    CreatedBy = dtoDepartment.CreatedBy,
                    UpdatedBy = dtoDepartment.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save department." };
            }

            return response;
        }

        public async Task<Response> UpdateDepartment(DtoDepartment dtoDepartment)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdateDepartment]",
                new
                {
                    DepartmentId = dtoDepartment.DepartmentId,
                    DepartmentCode = dtoDepartment.DepartmentCode,
                    DepartmentName = dtoDepartment.DepartmentName,
                    DepartmentName_Mr = dtoDepartment.DepartmentName_Mr,
                    DepartmentType = dtoDepartment.DepartmentType,
                    Description = dtoDepartment.Description,
                    Logo = dtoDepartment.Logo,
                    IsActive = dtoDepartment.IsActive,
                    CreatedBy = dtoDepartment.CreatedBy,
                    UpdatedBy = dtoDepartment.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to update department." };
            }

            return response;
        }

        public async Task<Response<DepartmentModel>> GetDepartmentById(int departmentId)
        {
            return await _crudOperationService.GetSingleRecord<DepartmentModel>(
                "[dbo].[Usp_GetDepartmentById]",
                new
                {
                    DepartmentId = departmentId
                });
        }

        public async Task<ResponseList<DepartmentModel>> GetDepartmentList(DtoDepartmentList request)
        {
            return await _crudOperationService.GetList<DepartmentModel>(
                "[dbo].[Usp_GetDepartmentList]",
                new
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Search = request.Search
                });
        }

        public async Task<ResponseList<DepartmentModel>> GetActiveDepartmentList()
        {
            return await _crudOperationService.GetList<DepartmentModel>(
                "[dbo].[Usp_GetActiveDepartmentList]",
                new { });
        }

        public async Task<Response> SavePost(DtoPost dtoPost)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdatePost]",
                new
                {
                    PostId = 0,
                    PostCode = string.IsNullOrWhiteSpace(dtoPost.PostCode) ? null : dtoPost.PostCode.Trim(),
                    PostName = dtoPost.PostName,
                    PostNameMarathi = dtoPost.PostNameMarathi,
                    IsActive = dtoPost.IsActive,
                    CreatedBy = dtoPost.CreatedBy,
                    UpdatedBy = dtoPost.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save post." };
            }

            return response;
        }

        public async Task<Response> UpdatePost(DtoPost dtoPost)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdatePost]",
                new
                {
                    PostId = dtoPost.PostId,
                    PostCode = string.IsNullOrWhiteSpace(dtoPost.PostCode) ? null : dtoPost.PostCode.Trim(),
                    PostName = dtoPost.PostName,
                    PostNameMarathi = dtoPost.PostNameMarathi,
                    IsActive = dtoPost.IsActive,
                    CreatedBy = dtoPost.CreatedBy,
                    UpdatedBy = dtoPost.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to update post." };
            }

            return response;
        }

        public async Task<Response<DtoPost>> GetPostById(int postId)
        {
            return await _crudOperationService.GetSingleRecord<DtoPost>(
                "[dbo].[Usp_GetPostById]",
                new
                {
                    PostId = postId
                });
        }

        public async Task<ResponseList<DtoPost>> GetPostList(DtoPostList request)
        {
            return await _crudOperationService.GetList<DtoPost>(
                "[dbo].[Usp_GetPostList]",
                new
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Search = request.Search
                });
        }

        public async Task<Response> SaveHelpDetails(DtoHelpDetails dtoHelpDetails)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[InsertUpdateHelpDetails]",
                new
                {
                    dtoHelpDetails.HelpDetailsId,
                    dtoHelpDetails.DepartmentId,
                    dtoHelpDetails.DepartmentContactNo,
                    dtoHelpDetails.DepartmentEmailId,
                    dtoHelpDetails.MahaITContactNo,
                    dtoHelpDetails.MahaITEmailId,
                    dtoHelpDetails.HelpDescription,
                    dtoHelpDetails.IsActive,
                    dtoHelpDetails.UserId
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save help details." };
            }

            return response;
        }

        public async Task<Response<DtoHelpDetails>> GetHelpDetailsById(int helpDetailsId)
        {
            return await _crudOperationService.GetSingleRecord<DtoHelpDetails>(
                "[dbo].[GetHelpDetailsById]",
                new
                {
                    HelpDetailsId = helpDetailsId
                });
        }

        public async Task<ResponseList<DtoHelpDetails>> GetHelpDetailsList()
        {
            return await _crudOperationService.GetList<DtoHelpDetails>(
                "[dbo].[GetHelpDetailsList]",
                new { });
        }

        public async Task<ResponseList<DtoHelpDetails>> GetHelpDetailsByDepartment(int departmentId)
        {
            return await _crudOperationService.GetList<DtoHelpDetails>(
                "[dbo].[GetHelpDetailsByDepartment]",
                new
                {
                    DepartmentId = departmentId
                });
        }

        public async Task<Response> SaveNotification(DtoNotification dtoNotification)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdateNotification]",
                new
                {
                    SrNo = dtoNotification.SrNo,
                    DepartmentId = dtoNotification.DepartmentId,
                    NotificationDetails = dtoNotification.NotificationDetails,
                    StartDate = dtoNotification.StartDate,
                    EndDate = dtoNotification.EndDate,
                    NotificationPdf = dtoNotification.NotificationPdf,
                    NotificationFileName = dtoNotification.NotificationFileName,
                    IsActive = dtoNotification.IsActive,
                    CreatedBy = dtoNotification.CreatedBy,
                    UpdatedBy = dtoNotification.UpdatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save notification." };
            }

            return response;
        }

        public async Task<ResponseList<NotificationModel>> GetNotificationList(DtoNotificationList request)
        {
            return await _crudOperationService.GetList<NotificationModel>(
                "[dbo].[Usp_GetNotificationList]",
                new
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Search = request.Search
                });
        }

        public async Task<Response<NotificationModel>> GetNotificationDetails(int srNo,int departmentId)
        {
            return await _crudOperationService.GetSingleRecord<NotificationModel>(
                "[dbo].[Usp_GetNotificationDetails]",
                new
                {
                    SrNo = srNo,
                    DepartmentId = departmentId
                });
        }
        public async Task<ResponseList<NotificationModel>> GetNotification(int departmentId)
        {
            return await _crudOperationService.GetList<NotificationModel>(
                "[dbo].[Usp_GetNotification]",
                new
                {
                    DepartmentId = departmentId
                });
        }

        public async Task<ResponseList<LatestNewsModel>> GetLatestNews()
        {
            return await _crudOperationService.GetList<LatestNewsModel>(
                "[dbo].[GetLatestNews]",
                new { });
        }

        public async Task<Response<HomeCountDetailsModel>> GetHomeCountDetails()
        {
            return await _crudOperationService.GetSingleRecord<HomeCountDetailsModel>(
                "[dbo].[Usp_GetCountDetails]",
                new { });
        }

        // Added By K.p on 08.08.2026
        public async Task<ResponseList<CategoryMasterModel>> GetCategory(int? categoryId)
        {

            return await _crudOperationService.GetList<CategoryMasterModel>(
                    "[dbo].[Usp_GetCategory]",
                    new
                    {
                        categoryId
                    });
        }

        public async Task<Response> SaveFeesDetails(List<DtoFeesMasterDetailsPost> dtoFeesMasterDetailsPost)
        {
            var first = dtoFeesMasterDetailsPost[0];

            var dt = new DataTable();
            dt.Columns.Add("CategoryId", typeof(int));
            dt.Columns.Add("Amount", typeof(decimal));

            foreach (var item in dtoFeesMasterDetailsPost)
            {
                dt.Rows.Add(item.CategoryId, item.Amount);
            }

            var response = await _crudOperationService
                .InsertUpdateDelete<Response>("[dbo].[InsertFeesDetails]",
                    new
                    {
                        DepartmentId = first.DepartmentId,
                        AdvertisementId = first.AdvertisementId,
                        PostId = first.PostId,
                        FeesDetails = dt,
                        CreatedBy = first.CreatedBy
                    })
                .ConfigureAwait(false);

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save fees details." };
            }

            return response;
        }

        public async Task<ResponseList<FeesMasterDEtailsModel>> GetFeesMasterDetailsList(DtoFeesMasterList request)
        {
            return await _crudOperationService.GetList<FeesMasterDEtailsModel>(
                "[dbo].[usp_GetFeesMasterDetailList]",
                new
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Search = request.Search
                });
        }

        public async Task<Response> UpdateFeesMasterDetails(DtoFeesMasterDetailsPost dtoFeesMasterDetailsPost)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[usp_UpdateFeesDetails]",
                new
                {
                    dtoFeesMasterDetailsPost.FeesCriteriaId,
                    dtoFeesMasterDetailsPost.Amount,
                    dtoFeesMasterDetailsPost.IsActive,
                    dtoFeesMasterDetailsPost.CreatedBy
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to update fees details." };
            }

            return response;
        }
        #region AgeCriteria
        public async Task<Response> SaveAgeCriteria(DtoAgeCriteria dtoAgeCriteria)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertAgeCriteria]",
                new
                {
                    dtoAgeCriteria.AgeCriteriaId,
                    dtoAgeCriteria.DepartmentId,
                    dtoAgeCriteria.AdvertisementId,
                    dtoAgeCriteria.PostId,
                    dtoAgeCriteria.GenderId,
                    dtoAgeCriteria.CategoryId,
                    dtoAgeCriteria.ParallelReservationId,
                    dtoAgeCriteria.MinimumAge,
                    dtoAgeCriteria.MaximumAge,
                    dtoAgeCriteria.IsActive
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save age criteria." };
            }

            return response;
        }

        public async Task<Response> UpdateAgeCriteria(DtoAgeCriteria dtoAgeCriteria)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertAgeCriteria]",
                new
                {
                    dtoAgeCriteria.AgeCriteriaId,
                    dtoAgeCriteria.DepartmentId,
                    dtoAgeCriteria.AdvertisementId,
                    dtoAgeCriteria.PostId,
                    dtoAgeCriteria.GenderId,
                    dtoAgeCriteria.CategoryId,
                    dtoAgeCriteria.ParallelReservationId,
                    dtoAgeCriteria.MinimumAge,
                    dtoAgeCriteria.MaximumAge,
                    dtoAgeCriteria.IsActive
                });

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to update age criteria." };
            }

            return response;
        }

        public async Task<Response<AgeCriteriaModel>> GetAgeCriteriaById(long ageCriteriaId)
        {
            return await _crudOperationService.GetSingleRecord<AgeCriteriaModel>(
                "[dbo].[Usp_GetAgeCriteriaById]",
                new
                {
                    AgeCriteriaId = ageCriteriaId
                });
        }

        public async Task<ResponseList<AgeCriteriaModel>> GetAgeCriteriaList(DtoAgeCriteriaList request)
        {
            return await _crudOperationService.GetList<AgeCriteriaModel>(
                "[dbo].[Usp_GetAgeCriteriaList]",
                new
                {
                    request.Page,
                    request.PageSize,
                    request.Search,
                    request.DepartmentId,
                    request.AdvertisementId,
                    request.PostId,
                    request.GenderId,
                    request.CategoryId
                });
        }

        public async Task<ResponseList<AgeCriteriaImportErrorModel>> ImportAgeCriteria(DataTable rows, long createdBy)
        {
            return await _crudOperationService.GetList<AgeCriteriaImportErrorModel>(
                "[dbo].[Usp_BulkInsertAgeCriteria]",
                new
                {
                    AgeCriteriaRows = rows,
                    CreatedBy = createdBy
                });
        }
        #endregion

        #region AdvertisementPdf
        public async Task<Response> SaveAdvertisementPdf(DtoAdvertisementPdf dtoAdvertisementPdf)
        {
            // FileData is null on update when no new file is sent. It must be typed as binary,
            // otherwise a null is sent as nvarchar and SQL Server rejects it for VARBINARY(MAX).
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("AdvertisementPdfId", dtoAdvertisementPdf.AdvertisementPdfId ?? 0);
            parameters.Add("DepartmentId", dtoAdvertisementPdf.DepartmentId);
            parameters.Add("AdvertisementId", dtoAdvertisementPdf.AdvertisementId);
            parameters.Add("PostId", dtoAdvertisementPdf.PostId);
            parameters.Add("UnitId", dtoAdvertisementPdf.UnitId);
            parameters.Add("FileName", dtoAdvertisementPdf.FileName);
            parameters.Add("FileType", dtoAdvertisementPdf.FileType);
            parameters.Add("FileData", dtoAdvertisementPdf.FileData, DbType.Binary, size: -1);
            parameters.Add("Description", dtoAdvertisementPdf.Description);
            parameters.Add("IsActive", dtoAdvertisementPdf.IsActive);
            parameters.Add("UserId", dtoAdvertisementPdf.UserId);

            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[SaveAdvertisementPdf]",
                parameters);

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save advertisement PDF." };
            }

            return response;
        }

        public async Task<Response<AdvertisementPdfModel>> GetAdvertisementPdfById(long advertisementPdfId)
        {
            return await _crudOperationService.GetSingleRecord<AdvertisementPdfModel>(
                "[dbo].[GetAdvertisementPdfById]",
                new
                {
                    AdvertisementPdfId = advertisementPdfId
                });
        }

        public async Task<ResponseList<AdvertisementPdfModel>> GetAdvertisementPdfList(DtoAdvertisementPdfList request)
        {
            return await _crudOperationService.GetList<AdvertisementPdfModel>(
                "[dbo].[GetAdvertisementPdfList]",
                new
                {
                    request.Page,
                    request.PageSize,
                    request.Search
                });
        }

        public async Task<Response<AdvertisementPdfFileModel>> GetAdvertisementPdfFile(long advertisementPdfId)
        {
            return await _crudOperationService.GetSingleRecord<AdvertisementPdfFileModel>(
                "[dbo].[GetAdvertisementPdfFile]",
                new
                {
                    AdvertisementPdfId = advertisementPdfId
                });
        }

        public async Task<ResponseList<AdvertisementPdfModel>> GetPublicAdvertisementPdfList(DtoPublicAdvertisementPdfList request)
        {
            return await _crudOperationService.GetList<AdvertisementPdfModel>(
                "[dbo].[GetPublicAdvertisementPdfList]",
                new
                {
                    request.DepartmentId,
                    request.AdvertisementId,
                    request.PostId
                });
        }

        public async Task<Response<AdvertisementPdfFileModel>> GetPublicAdvertisementPdfFile(long advertisementPdfId)
        {
            return await _crudOperationService.GetSingleRecord<AdvertisementPdfFileModel>(
                "[dbo].[GetPublicAdvertisementPdfFile]",
                new
                {
                    AdvertisementPdfId = advertisementPdfId
                });
        }
        #endregion

        #region RulePdf
        public async Task<Response> SaveRulePdf(DtoRulePdf dtoRulePdf)
        {
            // FileData is null on update when no new file is sent. It must be typed as binary,
            // otherwise a null is sent as nvarchar and SQL Server rejects it for VARBINARY(MAX).
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("RuleId", dtoRulePdf.RuleId ?? 0);
            parameters.Add("DepartmentId", dtoRulePdf.DepartmentId);
            parameters.Add("AdvertisementId", dtoRulePdf.AdvertisementId);
            parameters.Add("PostId", dtoRulePdf.PostId);
            parameters.Add("FileName", dtoRulePdf.FileName);
            parameters.Add("FileType", dtoRulePdf.FileType);
            parameters.Add("FileData", dtoRulePdf.FileData, DbType.Binary, size: -1);
            parameters.Add("Description", dtoRulePdf.Description);
            parameters.Add("IsActive", dtoRulePdf.IsActive);
            parameters.Add("UserId", dtoRulePdf.UserId);

            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[SaveRulePdf]",
                parameters);

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save rule PDF." };
            }

            return response;
        }

        public async Task<Response<RulePdfModel>> GetRulePdfById(long ruleId)
        {
            return await _crudOperationService.GetSingleRecord<RulePdfModel>(
                "[dbo].[GetRulePdfById]",
                new
                {
                    RuleId = ruleId
                });
        }

        public async Task<ResponseList<RulePdfModel>> GetRulePdfList(DtoRulePdfList request)
        {
            return await _crudOperationService.GetList<RulePdfModel>(
                "[dbo].[GetRulePdfList]",
                new
                {
                    request.Page,
                    request.PageSize,
                    request.Search
                });
        }

        public async Task<Response<RulePdfFileModel>> GetRulePdfFile(long ruleId)
        {
            return await _crudOperationService.GetSingleRecord<RulePdfFileModel>(
                "[dbo].[GetRulePdfFile]",
                new
                {
                    RuleId = ruleId
                });
        }

        public async Task<ResponseList<RulePdfModel>> GetPublicRulePdfList(DtoPublicRulePdfList request)
        {
            return await _crudOperationService.GetList<RulePdfModel>(
                "[dbo].[GetPublicRulePdfList]",
                new
                {
                    request.DepartmentId,
                    request.AdvertisementId,
                    request.PostId
                });
        }

        public async Task<Response<RulePdfFileModel>> GetPublicRulePdfFile(long ruleId)
        {
            return await _crudOperationService.GetSingleRecord<RulePdfFileModel>(
                "[dbo].[GetPublicRulePdfFile]",
                new
                {
                    RuleId = ruleId
                });
        }
        #endregion

        
        public async Task<byte[]> GetAgeCriteriaTemplate()
        {
            var departmentsResponse = await GetActiveDepartmentList();

            var advertisementsResponse =
                await GetAdvertisementList(new DtoAdvertisementList
                {
                    Page = 1,
                    PageSize = 5000,
                    Search = null
                });

            var postsResponse = await GetDepartmentPosts();
            var gendersResponse = await GetGender();
            var categoriesResponse = await GetCategory(null);
            var parallelReservationsResponse = await GetParallelReservation();

            // ============================================================
            // GET DROPDOWN VALUES
            // ============================================================

            var departmentNames =
                (departmentsResponse.Data ?? new List<DepartmentModel>())
                .Select(x => x.DepartmentName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var advertisementNames =
                (advertisementsResponse.Data ?? new List<AdvertisementMasterModel>())
                .Where(x => x.IsActive)
                .Select(x => x.AdvertisementName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var postNames =
                (postsResponse.Data ?? new List<PostMasterModel>())
                .Select(x => x.PostName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var genderNames =
                (gendersResponse.Data ?? new List<DDlList>())
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var categoryNames =
                (categoriesResponse.Data ?? new List<CategoryMasterModel>())
                .Select(x => x.Category)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var parallelReservationNames =
                (parallelReservationsResponse.Data ?? new List<DDlList>())
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // ============================================================
            // CREATE WORKBOOK
            // ============================================================

            using var workbook = new XLWorkbook();

            // Main sheet
            var sheet = workbook.Worksheets.Add("AgeCriteria");

            // Hidden list sheet
            var listSheet = workbook.Worksheets.Add("Lists");

            // IMPORTANT:
            // Keep this Hidden while testing.
            // Once confirmed working, change to VeryHidden.
            listSheet.Visibility = XLWorksheetVisibility.VeryHidden;

            // ============================================================
            // HEADERS
            // ============================================================

            string[] headers =
            {
        "DepartmentName",
        "AdvertisementName",
        "PostName",
        "GenderName",
        "CategoryName",
        "ParallelReservationName",
        "MinimumAge",
        "MaximumAge",
        "IsActive"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cell(1, i + 1);

                cell.Value = headers[i];

                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor =
                    XLColor.FromHtml("#1E3A8A");

                cell.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                cell.Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;
            }

            const int templateRowCount = 500;

            // ============================================================
            // DROPDOWN LISTS
            // ============================================================

            AddDropdownValidation(
                workbook,
                sheet,
                1,
                templateRowCount,
                listSheet,
                1,
                departmentNames,
                "DepartmentList");

            AddDropdownValidation(
                workbook,
                sheet,
                2,
                templateRowCount,
                listSheet,
                2,
                advertisementNames,
                "AdvertisementList");

            AddDropdownValidation(
                workbook,
                sheet,
                3,
                templateRowCount,
                listSheet,
                3,
                postNames,
                "PostList");

            AddDropdownValidation(
                workbook,
                sheet,
                4,
                templateRowCount,
                listSheet,
                4,
                genderNames,
                "GenderList");

            AddDropdownValidation(
                workbook,
                sheet,
                5,
                templateRowCount,
                listSheet,
                5,
                categoryNames,
                "CategoryList");

            AddDropdownValidation(
                workbook,
                sheet,
                6,
                templateRowCount,
                listSheet,
                6,
                parallelReservationNames,
                "ParallelReservationList");

            // ============================================================
            // IS ACTIVE LIST
            // ============================================================

            listSheet.Cell("G1").Value = "TRUE";
            listSheet.Cell("G2").Value = "FALSE";

            var activeListRange = listSheet.Range("G1:G2");

            workbook.DefinedNames.Add(
                "IsActiveList",
                activeListRange);

            var activeValidation =
                sheet.Range(2, 9, templateRowCount, 9)
                     .CreateDataValidation();

            activeValidation.List("=IsActiveList");
            activeValidation.IgnoreBlanks = true;
            activeValidation.InCellDropdown = true;

            // ============================================================
            // AGE VALIDATION
            // ============================================================

            var ageValidation =
                sheet.Range(2, 7, templateRowCount, 8)
                     .CreateDataValidation();

            ageValidation.WholeNumber.Between(1, 100);
            ageValidation.IgnoreBlanks = true;

            // ============================================================
            // FORMATTING
            // ============================================================

            sheet.Row(1).Height = 25;

            sheet.Column(1).Width = 25;
            sheet.Column(2).Width = 25;
            sheet.Column(3).Width = 25;
            sheet.Column(4).Width = 18;
            sheet.Column(5).Width = 20;
            sheet.Column(6).Width = 25;
            sheet.Column(7).Width = 15;
            sheet.Column(8).Width = 15;
            sheet.Column(9).Width = 12;

            // Freeze header
            sheet.SheetView.FreezeRows(1);

            // Filter
            sheet.Range(
                1,
                1,
                templateRowCount,
                headers.Length)
                .SetAutoFilter();

            // ============================================================
            // SAVE
            // ============================================================

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private static void AddDropdownValidation(
     XLWorkbook workbook,
     IXLWorksheet sheet,
     int column,
     int lastRow,
     IXLWorksheet listSheet,
     int listColumn,
     List<string> values,
     string rangeName)
        {
            // ============================================================
            // CLEAN VALUES
            // ============================================================

            values = (values ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            // If no data exists, put a placeholder
            if (values.Count == 0)
            {
                values.Add("N/A");
            }

            // ============================================================
            // WRITE VALUES TO LIST SHEET
            // ============================================================

            for (int i = 0; i < values.Count; i++)
            {
                listSheet.Cell(i + 1, listColumn).Value = values[i];
            }

            // ============================================================
            // CREATE NAMED RANGE
            // ============================================================

            var listRange = listSheet.Range(
                1,
                listColumn,
                values.Count,
                listColumn);

            // Delete existing name if present
            var existingName = workbook.DefinedNames
                .FirstOrDefault(x => x.Name.Equals(
                    rangeName,
                    StringComparison.OrdinalIgnoreCase));

            if (existingName != null)
            {
                workbook.DefinedNames.Delete(rangeName);
            }

            // Add workbook-level named range
            workbook.DefinedNames.Add(
                rangeName,
                listRange);

            // ============================================================
            // APPLY DROPDOWN
            // ============================================================

            var validationRange = sheet.Range(
                2,
                column,
                lastRow,
                column);

            var validation =
                validationRange.CreateDataValidation();

            // IMPORTANT:
            // Use named range instead of Lists!A1:A100
            validation.List($"={rangeName}");

            validation.IgnoreBlanks = true;

            validation.InCellDropdown = true;
        }

        public async Task<Response> SaveQualificationCriteria(DtoQualificationCriteriaPost dtoQualificationCriteriaPost)
        {
            var dt = new DataTable();
            dt.Columns.Add("RequiredQualification", typeof(int));

            foreach (var qualificationId in dtoQualificationCriteriaPost.QualificationIds.Distinct())
            {
                if (qualificationId > 0)
                {
                    dt.Rows.Add(qualificationId);
                }
            }

            var response = await _crudOperationService
                .InsertUpdateDelete<Response>("[dbo].[InsertQualificationCriteria]",
                    new
                    {
                        DepartmentId = dtoQualificationCriteriaPost.DepartmentId,
                        AdvertisementId = dtoQualificationCriteriaPost.AdvertisementId,
                        PostId = dtoQualificationCriteriaPost.PostId,
                        CreatedBy = dtoQualificationCriteriaPost.CreatedBy,
                        QualificationDetails = dt
                    })
                .ConfigureAwait(false);

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to save qualification criteria details." };
            }

            return response;
        }

        public async Task<ResponseList<QualificationCriteriaModel>> GetQualificationCriteriaList(DtoQualificationCriteriaList request)
        {
            return await _crudOperationService.GetList<QualificationCriteriaModel>(
                "[dbo].[usp_GetQualificationCriteriaList]",
                new
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Search = request.Search
                }).ConfigureAwait(false);
        }

        public async Task<Response> UpdateQualificationCriteria(DtoQualificationCriteriaPost dtoQualificationCriteriaPost)
        {
            var dt = new DataTable();
            dt.Columns.Add("RequiredQualification", typeof(int));

            foreach (var qualificationId in dtoQualificationCriteriaPost.QualificationIds.Distinct())
            {
                if (qualificationId > 0)
                {
                    dt.Rows.Add(qualificationId);
                }
            }

            var response = await _crudOperationService
                .InsertUpdateDelete<Response>("[dbo].[UpdateQualificationCriteria]",
                    new
                    {
                        DepartmentId = dtoQualificationCriteriaPost.DepartmentId,
                        AdvertisementId = dtoQualificationCriteriaPost.AdvertisementId,
                        PostId = dtoQualificationCriteriaPost.PostId,
                        UpdatedBy = dtoQualificationCriteriaPost.UpdatedBy ?? dtoQualificationCriteriaPost.CreatedBy,
                        QualificationDetails = dt
                    })
                .ConfigureAwait(false);

            if (response == null)
            {
                return new Response { Status = false, Message = "Failed to update qualification criteria details." };
            }

            return response;
        }

        public async Task<ResponseList<QualificationCriteriaDetailModel>> GetQualificationCriteriaByPost(int departmentId, int advertisementId, int postId)
        {
            return await _crudOperationService.GetList<QualificationCriteriaDetailModel>(
                "[dbo].[usp_GetQualificationCriteriaByPost]",
                new
                {
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId,
                    PostId = postId
                }).ConfigureAwait(false);
        }

        // added by valmiki 13082026 for state, district, taluka
        #region Dropdown Valmiki
        public async Task<ResponseList<DDlList>> GetState()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetState", new { });
        }
        public async Task<ResponseList<DDlList>> GetDomicileState()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.GetDomicileState", new { });
        }
        public async Task<ResponseList<DDlList>> GetParallelReservation()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetParallelReservation", new { });
        }
        public async Task<ResponseList<DDlList>> GetUserParallelReservation(long userid)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetUserParallelReservation", new { userid });
        }
        public async Task<ResponseList<DDlList>> GetDistrict(int stateid)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetDistict", new
                {
                    stateid
                });
        }
        public async Task<ResponseList<DDlList>> GetDomicileDistrict(int stateid)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.GetDomicileDistrict", new
                {
                    StateId = stateid
                });
        }
        public async Task<ResponseList<DDlList>> GetTaluka(int district)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetTaluka", new
                {
                    districtid = district
                });
        }
        public async Task<ResponseList<DDlList>> GetVillage(int taluka)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetVillage", new
                {
                    SubDistrictcode= taluka
                });
        }
       

        public async Task<ResponseList<DDlList>> GetDegree()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetQualification", new { });
        }
       
        public async Task<ResponseList<DDlList>> GetResult()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetResult", new { });
        }
        public async Task<ResponseList<DDlList>> GetCourse(int degreeId)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetCourse", new { QualificationId = degreeId });
        }
        public async Task<ResponseList<DDlList>> GetSubject(int courseId)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetSubject", new {courseId });
        }
        public async Task<ResponseList<DDlList>> GetBoardUniversity(int stateId)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetBoardUniversity", new {stateId });
        }

        public async Task<ResponseList<DDlList>> GetQualificationMode()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetQualificationMode", new { });
        }
        public async Task<ResponseList<GeneralWrittenExamCenterModel>> GetGeneralWrittenExamCenters(GeneralWrittenExamCenterRequest request)
        {
            return await _crudOperationService.GetList<GeneralWrittenExamCenterModel>(
                "[dbo].[Usp_GetWrittenExamCenterGeneral]",
                new
                {
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    PostId = request.PostId
                },
                ConnectionNames.Master);
        }

        public async Task<ResponseList<DDlList>> GetGender()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetGender", new { });
        }
        public async Task<ResponseList<DDlList>> GetMaritalStatus()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetMaritalStatus", new { });
        }
        public async Task<ResponseList<DDlList>> GetNationality()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetNationality", new { });
        }
        public async Task<ResponseList<DDlList>> GetMotherTongue()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.Usp_GetMotherTongue", new { });
        }

        public async Task<ResponseList<DDlList>> GetDisabilityType()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.GetDisabilityType", new { });
        }

        public async Task<ResponseList<DDlList>> GetDisabilitySubType(int disabilityTypeId)
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.GetDisabilitySubType", new { DisabilityTypeId = disabilityTypeId });
        }

        public async Task<ResponseList<DDlList>> GetDisabilityCertificateType()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.GetDisabilityCertificateType", new { });
        }

        public async Task<ResponseList<DDlList>> GetAnathType()
        {
            return await _crudOperationService.GetList<DDlList>(
                "dbo.GetAnathType", new { });
        }

        public async Task<ResponseList<UnitDDlList>> GetUnits(int postid)
        {
            return await _crudOperationService.GetList<UnitDDlList>(
                "dbo.Usp_GetUnitDetails", new
                {
                    postid
                });
        }
        #endregion




        // ================= Advertisement Master =================
        public async Task<Response> SaveAdvertisement(DtoAdvertisement dtoAdvertisement)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>
                (
                    "[dbo].[ManageAdvertisement]",
                    new
                    {
                        Type = 1,

                        AdvertisementId = 0,

                        DepartmentId = dtoAdvertisement.DepartmentId,
                        ExamYear = dtoAdvertisement.ExamYear,
                        AdvertisementNo = dtoAdvertisement.AdvertisementNo,
                        AdvertisementName = dtoAdvertisement.AdvertisementName,
                        AdvertisementNameMarathi = dtoAdvertisement.AdvertisementNameMarathi,
                        AdvertisementDesc = dtoAdvertisement.AdvertisementDesc,
                        AdvertisementStartDate = dtoAdvertisement.AdvertisementStartDate,
                        AdvertisementEndDate = dtoAdvertisement.AdvertisementEndDate,
                        AdvertisementImage = dtoAdvertisement.AdvertisementImage,
                        IsActive = dtoAdvertisement.IsActive,
                        IsUnitApplicable = dtoAdvertisement.IsUnitApplicable,
                        CreatedBy = dtoAdvertisement.CreatedBy,
                        UpdatedBy = dtoAdvertisement.UpdatedBy
                    });

            if (response == null)
            {
                return new Response
                {
                    Status = false,
                    Message = "Failed to save advertisement."
                };
            }

            return response;
        }


        public async Task<Response> UpdateAdvertisement( DtoAdvertisement dtoAdvertisement)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>
                (
                    "[dbo].[ManageAdvertisement]",
                    new
                    {
                        Type = 2,

                        AdvertisementId = dtoAdvertisement.AdvertisementId,
                        DepartmentId = dtoAdvertisement.DepartmentId,
                        ExamYear = dtoAdvertisement.ExamYear,
                        AdvertisementNo = dtoAdvertisement.AdvertisementNo,
                        AdvertisementName = dtoAdvertisement.AdvertisementName,
                        AdvertisementNameMarathi = dtoAdvertisement.AdvertisementNameMarathi,
                        AdvertisementDesc = dtoAdvertisement.AdvertisementDesc,
                        AdvertisementStartDate = dtoAdvertisement.AdvertisementStartDate,
                        AdvertisementEndDate = dtoAdvertisement.AdvertisementEndDate,
                        AdvertisementImage = dtoAdvertisement.AdvertisementImage,
                        IsActive = dtoAdvertisement.IsActive,
                        IsUnitApplicable = dtoAdvertisement.IsUnitApplicable,
                        CreatedBy = dtoAdvertisement.CreatedBy,
                        UpdatedBy = dtoAdvertisement.UpdatedBy
                    });

            if (response == null)
            {
                return new Response
                {
                    Status = false,
                    Message = "Failed to update advertisement."
                };
            }

            return response;
        }

        public async Task<ResponseList<AdvertisementMasterModel>> GetAdvertisementList(DtoAdvertisementList request)
        {
            return await _crudOperationService.GetList<AdvertisementMasterModel>
                (
                    "[dbo].[ManageAdvertisement]",
                    new
                    {
                        Type = 3,

                        Page = request.Page,
                        PageSize = request.PageSize,
                        Search = request.Search
                    });
        }


        public async Task<Response<AdvertisementMasterModel>> GetAdvertisementById(int advertisementId)
        {
            return await _crudOperationService.GetSingleRecord<AdvertisementMasterModel>
                (
                    "[dbo].[ManageAdvertisement]",
                    new
                    {
                        Type = 4,

                        AdvertisementId = advertisementId
                    });
        }

        // ================= Advertisement Tables Master =================
        public async Task<Response> CloneAdvertisementTables(DtoCloneAdvertisementTables request, long createdBy)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_CloneAndCreateAdvertisementTables]",
                new
                {
                    SourceSrNo = request.SourceSrNo,
                    TargetDepartmentId = request.DepartmentId,
                    TargetAdvertisementId = request.AdvertisementId,
                    TargetPostId = request.PostId,
                    CreatedBy = createdBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to clone advertisement tables." };
        }

        public async Task<ResponseList<AdvertisementTablesModel>> GetAdvertisementTablesList(DtoAdvertisementTablesList request)
        {
            return await _crudOperationService.GetList<AdvertisementTablesModel>(
                "[dbo].[Usp_GetAdvertisementTablesList]",
                new
                {
                    SearchText = request.SearchText,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                },
                ConnectionNames.Master);
        }

        public async Task<ResponseList<DtoSourceAdvertisementTableDropdown>> GetSourceAdvertisementTables()
        {
            return await _crudOperationService.GetList<DtoSourceAdvertisementTableDropdown>(
                "[dbo].[Usp_GetSourceAdvertisementTablesDropdown]",
                new { },
                ConnectionNames.Master);
        }

        public async Task<Response> ToggleAdvertisementTableStatus(DtoToggleAdvertisementTableStatus request)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_ToggleAdvertisementTableStatus]",
                new
                {
                    SrNo = request.SrNo,
                    IsActive = request.IsActive
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to update status." };
        }

        public async Task<ResponseList<UnitModel>> GetActiveUnitList()
        {
            return await _crudOperationService.GetList<UnitModel>(
                "[dbo].[GetActiveUnitList]",
                new { },
                ConnectionNames.Master);
        }

        public async Task<Response> SaveDepartmentUser(DtoDepartmentUserSave request)
        {
            var encryptedPassword = string.IsNullOrWhiteSpace(request.Password)
                ? null
                : AESEncrytDecry.EncryptStringAES(request.Password.Trim());

            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[ManageDepartmentUser]",
                new
                {
                    Type = request.UserId > 0 ? 2 : 1,
                    UserId = request.UserId,
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    UnitID = string.IsNullOrWhiteSpace(request.UnitID) ? null : request.UnitID.Trim(),
                    Username = request.Username.Trim(),
                    Password = encryptedPassword,
                    EmailId = string.IsNullOrWhiteSpace(request.EmailId) ? null : request.EmailId.Trim(),
                    MobileNo = string.IsNullOrWhiteSpace(request.MobileNo) ? null : request.MobileNo.Trim(),
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy
                },
                ConnectionNames.Registration);

            return response ?? new Response { Status = false, Message = "Failed to save department user." };
        }

        public async Task<ResponseList<DepartmentUserModel>> GetDepartmentUserList(DtoDepartmentUserList request)
        {
            return await _crudOperationService.GetList<DepartmentUserModel>(
                "[dbo].[GetDepartmentUserList]",
                new
                {
                    DepartmentId = request.DepartmentId ?? 0,
                    AdvertisementId = request.AdvertisementId ?? 0,
                    UnitID = request.UnitID,
                    Search = request.SearchText
                },
                ConnectionNames.Registration);
        }

        public async Task<Response<DepartmentUserModel>> GetDepartmentUserById(long userId)
        {
            return await _crudOperationService.GetSingleRecord<DepartmentUserModel>(
                "[dbo].[GetDepartmentUserById]",
                new { UserId = userId },
                ConnectionNames.Registration);
        }

        public async Task<Response> ToggleDepartmentUserStatus(long userId, bool isActive, long updatedBy)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[ManageDepartmentUser]",
                new
                {
                    Type = 3,
                    UserId = userId,
                    DepartmentId = (int?)null,
                    AdvertisementId = (int?)null,
                    UnitID = (string?)null,
                    Username = (string?)null,
                    Password = (string?)null,
                    EmailId = (string?)null,
                    MobileNo = (string?)null,
                    IsActive = isActive,
                    CreatedBy = updatedBy
                },
                ConnectionNames.Registration);

            return response ?? new Response { Status = false, Message = "Failed to update department user status." };
        }

        #region Education Master (Qualification, Course, Subject)
        public async Task<Response> SaveQualification(DtoQualification dtoQualification)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdateQualification]",
                new
                {
                    QualificationId = dtoQualification.QualificationId,
                    Qualification = dtoQualification.Qualification?.Trim(),
                    IsActive = dtoQualification.IsActive,
                    CreatedBy = dtoQualification.CreatedBy,
                    UpdatedBy = dtoQualification.UpdatedBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to save qualification type." };
        }

        public async Task<ResponseList<QualificationMasterModel>> GetQualificationMasterList(DtoQualificationList request)
        {
            return await _crudOperationService.GetList<QualificationMasterModel>(
                "[dbo].[Usp_GetQualificationMasterList]",
                new
                {
                    Search = request.Search?.Trim()
                },
                ConnectionNames.Master);
        }

        public async Task<Response<QualificationMasterModel>> GetQualificationById(int qualificationId)
        {
            return await _crudOperationService.GetSingleRecord<QualificationMasterModel>(
                "[dbo].[Usp_GetQualificationById]",
                new
                {
                    QualificationId = qualificationId
                },
                ConnectionNames.Master);
        }

        public async Task<Response> ToggleQualificationStatus(DtoQualificationStatusToggle request)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_ToggleQualificationStatus]",
                new
                {
                    QualificationId = request.QualificationId,
                    IsActive = request.IsActive,
                    UpdatedBy = request.UpdatedBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to update qualification status." };
        }

        public async Task<Response> SaveCourse(DtoCourse dtoCourse)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdateCourse]",
                new
                {
                    CourseId = dtoCourse.CourseId,
                    QualificationId = dtoCourse.QualificationId,
                    CourseName = dtoCourse.CourseName?.Trim(),
                    IsActive = dtoCourse.IsActive,
                    CreatedBy = dtoCourse.CreatedBy,
                    UpdatedBy = dtoCourse.UpdatedBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to save qualification name." };
        }

        public async Task<ResponseList<CourseMasterModel>> GetCourseMasterList(DtoCourseList request)
        {
            return await _crudOperationService.GetList<CourseMasterModel>(
                "[dbo].[Usp_GetCourseMasterList]",
                new
                {
                    QualificationId = request.QualificationId,
                    Search = request.Search?.Trim()
                },
                ConnectionNames.Master);
        }

        public async Task<Response<CourseMasterModel>> GetCourseById(int courseId)
        {
            return await _crudOperationService.GetSingleRecord<CourseMasterModel>(
                "[dbo].[Usp_GetCourseById]",
                new
                {
                    CourseId = courseId
                },
                ConnectionNames.Master);
        }

        public async Task<Response> ToggleCourseStatus(DtoCourseStatusToggle request)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_ToggleCourseStatus]",
                new
                {
                    CourseId = request.CourseId,
                    IsActive = request.IsActive,
                    UpdatedBy = request.UpdatedBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to update course status." };
        }

        public async Task<Response> SaveSubject(DtoSubject dtoSubject)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_InsertUpdateSubject]",
                new
                {
                    SubjectId = dtoSubject.SubjectId,
                    CourseId = dtoSubject.CourseId,
                    Subject = dtoSubject.Subject?.Trim(),
                    IsActive = dtoSubject.IsActive,
                    CreatedBy = dtoSubject.CreatedBy,
                    UpdatedBy = dtoSubject.UpdatedBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to save subject / stream / branch." };
        }

        public async Task<ResponseList<SubjectMasterModel>> GetSubjectMasterList(DtoSubjectList request)
        {
            return await _crudOperationService.GetList<SubjectMasterModel>(
                "[dbo].[Usp_GetSubjectMasterList]",
                new
                {
                    QualificationId = request.QualificationId,
                    CourseId = request.CourseId,
                    Search = request.Search?.Trim()
                },
                ConnectionNames.Master);
        }

        public async Task<Response<SubjectMasterModel>> GetSubjectById(int subjectId)
        {
            return await _crudOperationService.GetSingleRecord<SubjectMasterModel>(
                "[dbo].[Usp_GetSubjectById]",
                new
                {
                    SubjectId = subjectId
                },
                ConnectionNames.Master);
        }

        public async Task<Response> ToggleSubjectStatus(DtoSubjectStatusToggle request)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[Usp_ToggleSubjectStatus]",
                new
                {
                    SubjectId = request.SubjectId,
                    IsActive = request.IsActive,
                    UpdatedBy = request.UpdatedBy
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to update subject status." };
        }
        #endregion

        #region Role & Menu Management
        public async Task<ResponseList<DtoRoleMasterItem>> GetRoleMasterList()
        {
            return await _crudOperationService.GetList<DtoRoleMasterItem>(
                "[dbo].[UspGetRoleMasterList]",
                new { },
                ConnectionNames.Master);
        }

        public async Task<Response> SaveRoleMaster(DtoSaveRoleRequest request, long userId)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[UspSaveRoleMaster]",
                new
                {
                    request.RoleId,
                    request.RoleName,
                    request.RoutePrefix,
                    request.IsActive,
                    UserId = userId
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to save role." };
        }

        public async Task<ResponseList<DtoMenuListItem>> GetMenuList(string? routePrefix = null)
        {
            return await _crudOperationService.GetList<DtoMenuListItem>(
                "[dbo].[UspGetMenuList]",
                new { RoutePrefix = routePrefix },
                ConnectionNames.Master);
        }

        public async Task<ResponseList<DtoParentMenuDropdownItem>> GetParentMenuDropdown(string? routePrefix = null)
        {
            return await _crudOperationService.GetList<DtoParentMenuDropdownItem>(
                "[dbo].[UspGetParentMenuDropdown]",
                new { RoutePrefix = routePrefix },
                ConnectionNames.Master);
        }

        public async Task<Response> SaveMenu(DtoSaveMenuRequest request, long userId)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[UspSaveMenu]",
                new
                {
                    request.MenuId,
                    request.MenuName,
                    request.MenuNameMarathi,
                    request.RouteUrl,
                    request.ParentMenuId,
                    request.Icon,
                    request.SortOrder,
                    request.IsVisibleInMenu,
                    request.IsActive,
                    request.AssignToRoutePrefix,
                    UserId = userId
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to save menu." };
        }

        public async Task<ResponseList<DtoRoleMenuMappingItem>> GetRoleMenuMappingByRoutePrefix(string routePrefix)
        {
            return await _crudOperationService.GetList<DtoRoleMenuMappingItem>(
                "[dbo].[UspGetRoleMenuMappingByRoutePrefix]",
                new { RoutePrefix = routePrefix },
                ConnectionNames.Master);
        }

        public async Task<Response> SaveRoleMenuMapping(DtoSaveRoleMenuMappingRequest request, long userId)
        {
            var menuIdsCsv = request.MenuIds != null && request.MenuIds.Count > 0
                ? string.Join(",", request.MenuIds)
                : string.Empty;

            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[UspSaveRoleMenuMapping]",
                new
                {
                    RoutePrefix = request.RoutePrefix,
                    MenuIds = menuIdsCsv,
                    UserId = userId
                },
                ConnectionNames.Master);

            return response ?? new Response { Status = false, Message = "Failed to save role menu mappings." };
        }
        #endregion




        // ===============================================
        // Written Hall Ticket Instruction Master
        // ===============================================

        // 1. SAVE
        public async Task<Response> SaveWrittenHallTicketInstruction( WrittenHallTicketInstructionModel model)
        {
            return await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[USP_ManageWrittenHallTicketInstructions]",
                new
                {
                    Action = "INSERT",
                    model.SrNo,
                    model.DepartmentId,
                    model.AdvertisementId,
                    model.PostId,
                    model.Instruction
                },
                ConnectionNames.Master
            ) ?? new Response
            {
                Status = false,
                Message = "Unable to save instructions."
            };
        }


        // 2. UPDATE
        public async Task<Response> UpdateWrittenHallTicketInstruction( WrittenHallTicketInstructionModel model)
        {
            return await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[USP_ManageWrittenHallTicketInstructions]",
                new
                {
                    Action = "UPDATE",
                    model.SrNo,
                    model.DepartmentId,
                    model.AdvertisementId,
                    model.PostId,
                    model.Instruction
                },
                ConnectionNames.Master
            ) ?? new Response
            {
                Status = false,
                Message = "Unable to update instructions."
            };
        }


        // 3. GET ALL
        public async Task<ResponseList<WrittenHallTicketInstructionModel>> GetAllWrittenHallTicketInstructions()
        {
            return await _crudOperationService
                .GetList<WrittenHallTicketInstructionModel>(
                    "[dbo].[USP_ManageWrittenHallTicketInstructions]",
                    new
                    {
                        Action = "GETALL"
                    },
                    ConnectionNames.Master
                );
        }


       
        // 4. GET BY ID
        public async Task<Response<WrittenHallTicketInstructionModel>> GetWrittenHallTicketInstructionById(int srNo)
        {
            return await _crudOperationService
                .GetSingleRecord<WrittenHallTicketInstructionModel>(
                    "[dbo].[USP_ManageWrittenHallTicketInstructions]",
                    new
                    {
                        Action = "GETBYID",
                        SrNo = srNo
                    },
                    ConnectionNames.Master
                );
        }


        // 5. DELETE
        public async Task<Response> DeleteWrittenHallTicketInstruction(int srNo)
        {
            return await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[USP_ManageWrittenHallTicketInstructions]",
                new
                {
                    Action = "DELETE",
                    SrNo = srNo
                },
                ConnectionNames.Master
            ) ?? new Response
            {
                Status = false,
                Message = "Unable to delete instructions."
            };
        }
    }
}
