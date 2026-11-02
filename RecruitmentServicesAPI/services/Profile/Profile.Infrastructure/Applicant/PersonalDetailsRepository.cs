using CrudOperations;
using ClientsideEncryption;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Infrastructure.Applicant
{
    public class PersonalDetailsRepository(ICrudOperationService crudOperationService) : IPersonalDetailsRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;
        public async Task<Response> SavePersonalDetails(
        PersonalDetailsRequest request)
        {
            var result = await _crudOperationService
                .InsertUpdateDelete<Response>(
                    "[dbo].[Usp_InsertUpdatePersonalDetails]",
                    new
                    {
                        request.UserId,
                        request.AadhaarNo,
                        request.AadhaarName,
                        request.AadhaarDOB,
                        request.Gender,
                        request.FirstName,
                        request.FatherName,
                        request.MotherName,
                        request.SurName,
                        request.FullNameEnglish,
                        request.FullNameMarathi,
                        request.MobileNo,
                        request.EmailId,
                        request.MaritalStatusId,
                        request.NationalityId,
                        request.MotherToungueId,
                        request.OtherMotherTongue,
                        request.NameChange,
                        request.FullChangedName,
                        request.CreatedBy,
                        request.IPAddress,
                        request.isDraft
                    }, ConnectionNames.Profile);

            return result;
        }
        public async Task<Response> SavePersonalDraftDetails(
        PersonalDetailsDraftRequest request)
        {
            var result = await _crudOperationService
                .InsertUpdateDelete<Response>(
                    "[dbo].[Usp_InsertUpdatePersonalDetails]",
                    new
                    {
                        request.UserId,
                        request.AadhaarNo,
                        request.AadhaarName,
                        request.AadhaarDOB,
                        request.Gender,
                        request.FirstName,
                        request.FatherName,
                        request.MotherName,
                        request.SurName,
                        request.FullNameEnglish,
                        request.FullNameMarathi,
                        request.MobileNo,
                        request.EmailId,
                        request.MaritalStatusId,
                        request.NationalityId,
                        request.MotherToungueId,
                        request.OtherMotherTongue,
                        request.NameChange,
                        request.FullChangedName,
                        request.CreatedBy,
                        request.IPAddress,
                        request.isDraft
                    }, ConnectionNames.Profile);

            return result;
        }

        public async Task<Response<PersonalDetailsRequest>> GetPersonalDetails(long userId)
        {
            var res = await _crudOperationService
                .GetSingleRecord<PersonalDetailsRequest>(
                    "[dbo].[Usp_GetPersonalDetails]",
                    new
                    {
                        UserId = userId
                    },
                    ConnectionNames.Profile);

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
                applicant.AadhaarNo = MaskAadhaar(AESEncrytDecry.DecryptStringAES(applicant.AadhaarNo));
                applicant.Email = applicant.Email;

            }
            return response;
        }

        private string MaskAadhaar(string aadhaarNo)
        {
            if (string.IsNullOrEmpty(aadhaarNo) || aadhaarNo.Length < 4)
                return aadhaarNo;

            return "XXXX XXXX " + aadhaarNo.Substring(aadhaarNo.Length - 4);
        }
    }
}
