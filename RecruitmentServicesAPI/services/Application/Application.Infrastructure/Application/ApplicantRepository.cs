using Application.Application.Interfaces;
using Application.Domain.Application;
using Application.Domain.Model;
using Azure.Core;
using ClientsideEncryption;
using CrudOperations;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Shared.Kernel.Interfaces;
using Shared.Kernel.Helpers;

namespace Application.Infrastructure.Application
{
    public class ApplicantRepository(
        ICrudOperationService crudOperationService,
        IConfiguration configuration,
        ISmsService smsService) : IApplicantRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;
        private readonly IConfiguration _configuration = configuration;
        private readonly ISmsService _smsService = smsService;

        #region recipt
        public async Task<ResponseList<AppliedPostModel>> GetCandidateAppliedPosts(long userId)
        {
            return await _crudOperationService.GetList<AppliedPostModel>(
                "[dbo].[USP_GetCandidateAppliedPosts]",
                new
                {
                    UserId = userId
                },
                ConnectionNames.Applications);
        }

        public async Task<ResponseList<CancelledApplicationModel>> GetCancelledApplications(long userId)
        {
            return await _crudOperationService.GetList<CancelledApplicationModel>(
                "[dbo].[USP_GetCancelledApplications]",
                new
                {
                    UserId = userId
                },
                ConnectionNames.Applications);
        }

        public async Task<Response<ReceiptResponse>> GetReceiptDetails(ReceiptRequest request)
        {
            return await _crudOperationService.GetSingleRecord<ReceiptResponse>(
                "[dbo].[usp_GetReceiptDetails]",
                new
                {
                    UserId = request.UserId,
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    PostId = request.PostId,
                    ApplicationID = request.ApplicationID
                },
                ConnectionNames.Applications);
        }


        public async Task<Response<HallTicketModel>> GetPhysicalHallTicket(HallTicketRequest request)
        {
            return await _crudOperationService.GetSingleRecord<HallTicketModel>(
                "[dbo].[USP_GetHallTicket]",
                new
                {
                    UserId = request.UserId,
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    PostId = request.PostId,
                    ApplicationID = request.ApplicationId
                },
                ConnectionNames.Applications);
        }

        public async Task<Response<WrittenHallTicket>> GetWrittenHallTicket(WrittenHallTickRequest request)
        {
            return await _crudOperationService.GetSingleRecord<WrittenHallTicket>(
                "[dbo].[USP_GetWrittenHallTicket]",
                new
                {
                    UserId = request.UserId,
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    PostId = request.PostId,
                    ApplicationID = request.ApplicationId
                },
                ConnectionNames.Applications);

        }

        public async Task<Response> SendCancellationOtp(long userId, string? appNo = null)
        {
            var user = await _crudOperationService.GetSingleRecord<CandidateUserMobileInfo>(
                "[dbo].[GetAuthUserById]",
                new { UserId = userId },
                ConnectionNames.Registration);

            var mobileNo = user?.Data?.MobileNo;
            if (user?.Data == null || !user.Status || string.IsNullOrWhiteSpace(mobileNo))
            {
                return new Response { Status = false, Message = "Registered mobile number is not available." };
            }

            var otp = Random.Shared.Next(100000, 999999).ToString();

            var otpResponse = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[ManageAuthOTP]",
                new
                {
                    Value = mobileNo,
                    OTP = otp,
                    Type = "mobile",
                    Purpose = "cancelapplication"
                },
                ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to save OTP." };

            if (!otpResponse.Status)
            {
                return otpResponse;
            }

            var expiryMinutesResult = await _crudOperationService.GetSingleRecord<AuthOtpExpiryModel>(
                "[dbo].[GetAuthOtpExpiry]",
                new { },
                ConnectionNames.Registration);

            var otpMinutes = (expiryMinutesResult.Status && expiryMinutesResult.Data != null && expiryMinutesResult.Data.OtpExpiryMinutes > 0)
                ? expiryMinutesResult.Data.OtpExpiryMinutes
                : 3;

            // REPLACE: Use ApplicationCancellationOtp template when separate template is configured in DLT
            // var smsText = SMSMessageHelper.ApplicationCancellationOtp
            //     .Replace("{#ApplicationNo#}", appNo ?? "")
            //     .Replace("{#MobileOTP#}", otp)
            //     .Replace("{#var#}", otpMinutes.ToString());
            // var sms = await _smsService.SendAsync(mobileNo, smsText, SMSTemplateIDHelper.ApplicationCancellationOtpTempID);

            var smsText = SMSMessageHelper.CommonOtp
                .Replace("{#MobileOTP#}", otp)
                .Replace("{#var#}", otpMinutes.ToString());

            var sms = await _smsService.SendAsync(mobileNo, smsText, SMSTemplateIDHelper.CommonOtpTempID);

            return sms.Status
                ? new Response { Status = true, Message = MaskedOtpMessage(mobileNo) }
                : sms;
        }

        public async Task<Response> CancelApplication(CancelApplicationRequest request)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>(
                "[dbo].[CancelApplication]",
                new
                {
                    UserId = request.UserId,
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    PostId = request.PostId,
                    ApplicationNo = request.AppNo,
                    OTP = request.Otp,
                    CancelledIpAddress = request.CancelledIpAddress
                },
                ConnectionNames.Applications) ?? new Response
                {
                    Status = false,
                    Message = "Application cancellation failed."
                };

            if (response.Status)
            {
                // After successful cancellation, send confirmation SMS
                try
                {
                    var user = await _crudOperationService.GetSingleRecord<CandidateUserMobileInfo>(
                        "[dbo].[GetAuthUserById]",
                        new { UserId = request.UserId },
                        ConnectionNames.Registration);

                    var mobileNo = user?.Data?.MobileNo;
                    if (!string.IsNullOrWhiteSpace(mobileNo))
                    {
                        // REPLACE: Use ApplicationCancellation template when separate template is configured in DLT
                        // var confirmationSms = SMSMessageHelper.ApplicationCancellation
                        //     .Replace("{#ApplicationNo#}", request.AppNo ?? "");
                        // await _smsService.SendAsync(mobileNo, confirmationSms, SMSTemplateIDHelper.ApplicationCancellationTempID);
                    }
                }
                catch
                {
                    throw;
                    // Ignore SMS failures so cancellation result itself is not impacted
                }
            }

            return response;
        }

        private static string MaskedOtpMessage(string? mobile)
        {
            var value = mobile ?? "";
            if (value.Length < 4)
            {
                return "OTP sent successfully.";
            }

            return $"OTP sent to mobile ending {value[^4..]}.";
        }

        public async Task<ResponseList<EligibilityCriteriaResponse>> CheckEligibilityCriteria(
            EligibilityCriteriaRequest obj)
        {
            var res = await _crudOperationService
                    .GetList<EligibilityCriteriaResponse>(
                        "[dbo].[Usp_CheckEligibilityCriteria]",
                        new
                        {
                            obj.UserId,
                            obj.PostId,
                            obj.DepartmentId,
                            obj.AdvertisementId,
                            obj.GenderId
                        },
                        ConnectionNames.Applications);

            return res;
        }
        public async Task<ResponseList<EligibilityCriteriaResponse>> CheckApplicationEligibilityCriteria(
            ApplicationEligibilityCriteriaRequest obj)
        {
            var res = await _crudOperationService
                    .GetList<EligibilityCriteriaResponse>(
                        "[dbo].[Usp_CheckApplicationEligibilityCriteria]",
                        new
                        {
                            obj.UserId,
                            obj.PostId,
                            obj.DepartmentId,
                            obj.AdvertisementId,
                            obj.GenderId,
                            obj.CategoryId,
                            obj.Unit,
                            obj.ParallelReservationId
                        },
                        ConnectionNames.Applications);

            return res;
        }

        public async Task<Response<ApplicationProfileResponse>> SaveApplication(
            PostApplicationRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (request.PersonalInformation is null) throw new ArgumentNullException(nameof(request.PersonalInformation));
            if (request.EducationDetails is null) throw new ArgumentNullException(nameof(request.EducationDetails));
            if (request.OtherDetails is null) throw new ArgumentNullException(nameof(request.OtherDetails));
            if (request.AddressDetails is null) throw new ArgumentNullException(nameof(request.AddressDetails));

            byte[]? photoBytes = null;
            byte[]? signBytes = null;
            if (!string.IsNullOrWhiteSpace(request.Photo))
            {
                try
                {
                    photoBytes = Convert.FromBase64String(request.Photo);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Invalid photo format.", nameof(request.Photo));
                }
                if (photoBytes.Length == 0)
                {
                    throw new ArgumentException("Photo cannot be empty.", nameof(request.Photo));
                }
            }
            if (!string.IsNullOrWhiteSpace(request.Sign))
            {
                try { 
                    signBytes = Convert.FromBase64String(request.Sign);
                }
                catch (FormatException) {
                    throw new ArgumentException("Invalid signature format.", nameof(request.Sign)); 
                }
                if (signBytes.Length == 0) {
                    throw new ArgumentException("Signature cannot be empty.", nameof(request.Sign)); 
                }
            }

            using var connection =
                _crudOperationService.CreateConnection(ConnectionNames.Applications);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction =
                _crudOperationService.BeginTransaction(connection);

            try
            {
                // =====================================================
                // 1. PERSONAL / APPLICATION PROFILE
                // =====================================================

                var personal =
                    await _crudOperationService
                        .InsertWithTransaction<ApplicationProfileResponse>(
                            "[dbo].[Usp_InsertApplicationProfileDetails]",
                            request.PersonalInformation,
                            connection,
                            transaction);

                if (personal == null)
                {
                    throw new Exception(
                        "Application profile details were not inserted.");
                }

                if (!personal.Status)
                {
                    throw new Exception(
                        personal.Message ??
                        "Application profile insertion failed.");
                }


                // =====================================================
                // IMPORTANT:
                // personal.AppNO is now available
                // =====================================================

                if (string.IsNullOrEmpty(personal.AppNO))
                {
                    throw new Exception(
                        "Application number was not generated.");
                }


                var applicationNo = personal.AppNO;
                request.OtherDetails.ApplicationNo = applicationNo;
                request.OtherDetails.DepartmentId = request.DepartmentId;
                request.OtherDetails.Postid = request.Postid;
                request.OtherDetails.AdvertisementId = request.AdvertisementId;
                request.OtherDetails.UnitId = request.UnitId;
                request.OtherDetails.UserId = request.PersonalInformation.UserId;
                request.OtherDetails.IPAddress = request.PersonalInformation.IPAddress;

                // =====================================================
                // 2. OTHER DETAILS
                // =====================================================



                var other =
                    await _crudOperationService
                        .InsertWithTransaction<Response>(
                            "[dbo].[Usp_InsertApplicationOtherDetails]",
                            request.OtherDetails,
                            connection,
                            transaction);

                if (other == null || !other.Status)
                {
                    throw new Exception(
                        other?.Message ??
                        "Other details insertion failed.");
                }


                // =====================================================
                // 3. ADDRESS DETAILS
                // =====================================================
                request.AddressDetails.ApplicationNo = applicationNo;
                request.AddressDetails.DepartmentId = request.DepartmentId;
                request.AddressDetails.Postid = request.Postid;
                request.AddressDetails.AdvertisementId = request.AdvertisementId;
                request.AddressDetails.UnitId = request.UnitId;
                request.AddressDetails.UserId = request.PersonalInformation.UserId;
                request.AddressDetails.CreatedBy = request.PersonalInformation.UserId;
                request.AddressDetails.IPAddress = request.PersonalInformation.IPAddress;

                var address =
                          await _crudOperationService
                          .InsertWithTransaction<Response>(
                              "[dbo].[Usp_InsertApplicationAddressDetails]",
                               request.AddressDetails,
                                connection,
                               transaction);

                if (address == null || !address.Status)
                {
                    throw new Exception(
                        address?.Message ??
                        "Address details insertion failed.");
                }


                // =====================================================
                // 4. EDUCATION DETAILS
                // =====================================================


                //var educationTable =
                //        CreateEducationDataTable(request.EducationDetails);
                //var parameters = new DynamicParameters();

                //parameters.Add("UserId", request.PersonalInformation.UserId);
                //parameters.Add("ApplicationNo", applicationNo);
                //parameters.Add("DepartmentId", request.DepartmentId);
                //parameters.Add("AdvertisementId", request.AdvertisementId);
                //parameters.Add("PostId", request.Postid);
                //parameters.Add("UnitId", request.UnitId);
                //parameters.Add("IPAddress", request.PersonalInformation.IPAddress);

                //parameters.Add(
                //    "Education",
                //    educationTable.AsTableValuedParameter("dbo.EducationDetail")
                //);

                var educationTable = CreateEducationDataTable(request.EducationDetails);

                var parameters = new
                {
                    request.PersonalInformation.UserId,
                    ApplicationNo = applicationNo,
                    request.DepartmentId,
                    request.AdvertisementId,
                    PostId = request.Postid,
                    request.UnitId,
                    request.PersonalInformation.IPAddress,
                    Education = educationTable.AsTableValuedParameter("dbo.EducationDetail")
                };

                var education =
                        await _crudOperationService
                           .InsertWithTransaction<Response>(
                              "[dbo].[Usp_InsertApplicationEducationDetails]",
                               parameters,
                               connection,
                               transaction);

                if (education == null || !education.Status)
                {
                    throw new Exception(
                      education?.Message ??
                     "Education details insertion failed.");
                }





                // =====================================================
                // 5.photo Sign
                // =====================================================

               

                var photoSignRequest = new
                {
                    request.PersonalInformation.UserId,
                    Photo = photoBytes,
                    Sign = signBytes,
                    ApplicationNo = applicationNo,
                    request.PhotofileName,
                    request.PhotofileType,
                    request.SignfileName,
                    request.SignfileType,
                    request.Postid,
                    request.DepartmentId,
                    request.AdvertisementId,
                    request.UnitId,
                    request.PersonalInformation.IPAddress
                };

                var photoSign =
                    await _crudOperationService.InsertWithTransaction<Response>(
                        "[MahaRec_Documents].[dbo].[Usp_InsertApplicationPhotoSignDetails]",
                        photoSignRequest,
                        connection,
                        transaction);

                if (photoSign == null || !photoSign.Status)
                    throw new Exception(
                        photoSign?.Message ?? "Photo/Sign saving failed."
                    );


                // =====================================================
                // ALL SUCCESSFUL
                // =====================================================

                transaction.Commit();

                return new Response<ApplicationProfileResponse>
                {
                    Data = personal,

                    Status = true,

                    Message =
                        "Application submitted successfully."
                };
            }
            catch (Exception ex)
            {
                // =====================================================
                // ANY SP FAILS
                // =====================================================

                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    // Optional logging
                    throw;
                }
                throw;

            }
        }

        public async Task<Response<ApplicationExists>> CheckApplicationExists(CheckApplicantionRequest obj)
        {
            var res = await _crudOperationService
                    .GetSingleRecord<ApplicationExists>(
                        "[dbo].[Usp_CheckApplicationExists]",
                        new
                        {
                            obj.UserId,
                            obj.PostId,
                            obj.DepartmentId,
                            obj.AdvertisementId
                        },
                        ConnectionNames.Applications);

            return res;
        }

        public async Task<Response<ApplicantCreation>> GetAadhaarDetails(long id)
        {
            var response = await _crudOperationService
                .GetSingleRecord<ApplicantCreation>(
                    "[dbo].[Usp_GetAadhaarDetails]",
                    new
                    {
                        userid = id
                    }, ConnectionNames.Registration);
            if (response != null && response.Data != null)
            {
                var applicant = response.Data as ApplicantCreation;
                // string base64Image = applicant.Image;

                applicant.Name = AESEncrytDecry.DecryptStringAES(applicant.Name);
                applicant.DOB = AESEncrytDecry.DecryptStringAES(applicant.DOB);
                applicant.Mobile = applicant.Mobile;
                applicant.Gender = AESEncrytDecry.DecryptStringAES(applicant.Gender);
                applicant.NameMarathi = AESEncrytDecry.DecryptStringAES(applicant.NameMarathi);
                //applicant.AadhaarNo = MaskAadhaar(AESEncrytDecry.DecryptStringAES(applicant.AadhaarNo));
                applicant.Email = applicant.Email;

            }
            return response;
        }

        private DataTable CreateEducationDataTable(List<EducationRequest> educationList)
        {
            var table = new DataTable();

            table.Columns.Add("DegreeID", typeof(int));
            table.Columns.Add("CourseId", typeof(int));
            table.Columns.Add("SubjectId", typeof(int));
            table.Columns.Add("StateId", typeof(int));
            table.Columns.Add("UniversityId", typeof(int));
            table.Columns.Add("ResultId", typeof(int));
            table.Columns.Add("PassingDate", typeof(DateTime));
            table.Columns.Add("Percentage", typeof(decimal));
            table.Columns.Add("QualificationModeId", typeof(int));

            foreach (var item in educationList)
            {
                table.Rows.Add(
                    item.DegreeID,
                    item.CourseId,
                    item.SubjectId,
                    item.StateId,
                    item.UniversityId,
                    item.ResultId,
                    item.PassingDate,
                    item.Percentage,
                    item.QualificationModeId
                );
            }

            return table;
        }

        public async Task<Response<ApplicantViewApplicationResponse>> ViewApplication(ViewApplicantionRequest obj)
        {
            try
            {
                var res = await _crudOperationService
                    .BindDynamicLists(
                            "[dbo].[Usp_GetApplicationDetails]",
                            obj,
                            new List<string>
                            {
                            "PersonalDetails",
                            "EducationDetails",
                            "Address",
                            "OtherDetails",
                            "Status"
                            },
                ConnectionNames.Applications);

                var response = new ApplicantViewApplicationResponse
                {
                    PersonalDetails =
                                    res.NamedDataSets.GetValueOrDefault("PersonalDetails"),

                    EducationDetails =
                                    res.NamedDataSets.GetValueOrDefault("EducationDetails"),

                    AddressDetails =
                                    res.NamedDataSets.GetValueOrDefault("Address"),
                    OtherDetails =
                                    res.NamedDataSets.GetValueOrDefault("OtherDetails")
                };

                return new Response<ApplicantViewApplicationResponse>
                {
                    Data = response,
                    Status = Convert.ToBoolean(res.Status),
                    Message = res.Message
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response<ApplicantViewApplicationResponse>> ViewCancelledApplicationDetails(ViewApplicantionRequest obj)
        {
            try
            {
                var res = await _crudOperationService
                    .BindDynamicLists(
                            "[dbo].[Usp_GetCancelledApplicationDetails]",
                            obj,
                            new List<string>
                            {
                            "PersonalDetails",
                            "EducationDetails",
                            "Address",
                            "OtherDetails",
                            "Status"
                            },
                ConnectionNames.Applications);

                var response = new ApplicantViewApplicationResponse
                {
                    PersonalDetails =
                                    res.NamedDataSets.GetValueOrDefault("PersonalDetails"),

                    EducationDetails =
                                    res.NamedDataSets.GetValueOrDefault("EducationDetails"),

                    AddressDetails =
                                    res.NamedDataSets.GetValueOrDefault("Address"),
                    OtherDetails =
                                    res.NamedDataSets.GetValueOrDefault("OtherDetails")
                };

                return new Response<ApplicantViewApplicationResponse>
                {
                    Data = response,
                    Status = Convert.ToBoolean(res.Status),
                    Message = res.Message
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion


 

        public async Task<Response> SaveApplicationNo( DtoApplicationNo request)
        {
            var response = await _crudOperationService.InsertUpdateDelete<Response>
                    (
                       "[dbo].[ManageApplicationNo]",
                        new
                        {
                            Type = 1,

                            Year = request.Year,
                            DepartmentId = request.DepartmentId,
                            AdvertisementId = request.AdvertisementId,
                            PostId = request.PostId,
                            AppNo = request.AppNo

                        },
                        ConnectionNames.Applications
                    );


            return response ?? new Response
            {
                Status = false,
                Message = "Failed to save Application Number."
            };
        }


   
        public async Task<ResponseList<ApplicationNoModel>> GetApplicationNoList()
        {
            var response = await _crudOperationService.GetList<ApplicationNoModel>
                    (
                        "[dbo].[ManageApplicationNo]",
                        new
                        {
                            Type = 2
                        },
                        ConnectionNames.Applications
                    );


            return response;
        }



    }

    public class CandidateUserMobileInfo
    {
        public string? MobileNo { get; set; }
    }





}
