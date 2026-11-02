using CrudOperations;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Infrastructure.Applicant
{
    public class EducationRepository(ICrudOperationService crudOperationService) : IEducationRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;
        public async Task<Response> SaveEducation(
            EducationRequest obj)
        {
            try
            {
                var res = await _crudOperationService
                    .InsertUpdateDelete<Response>(
                        "[dbo].[Usp_InsertUpdateEducationDetails]",
                        new
                        {
                            obj.UserId,
                            obj.DegreeID,
                            obj.CourseId,
                            obj.UniversityId,
                            obj.ResultId,
                            obj.StateId,
                            obj.SubjectId,
                            obj.QualificationModeId,
                            obj.PassingDate,
                            obj.Percentage,
                            obj.Eid,
                            obj.CreatedBy,
                            obj.IPAddress
                            
                        },
                        ConnectionNames.Profile);

                return res;
            }
            catch (Exception ex)
            {
                throw;
                
            }
        }

        public async Task<ResponseList<EducationDetailsResponse>> GetEducationDetails(
        long UserId)
        {

            var res = await _crudOperationService
                .GetList<EducationDetailsResponse>(
                    "[dbo].[Usp_GetEducationDetails]",
                    new
                    {
                        UserId
                    },
                    ConnectionNames.Profile);

            return res;

        }

        public async Task<Response<CheckDegreeCourseResponse>> CheckDegreeCourse(
    long UserId,
    int DegreeId,
    int CourseId,
    long? Eid)
        {
           
                var res = await _crudOperationService
                    .GetSingleRecord<CheckDegreeCourseResponse>(
                        "[dbo].[Usp_checkDegreeCourse]",
                        new
                        {
                            userid = UserId,
                            degreeid = DegreeId,
                            courseid = CourseId,
                            Eid = Eid
                        },
                        ConnectionNames.Profile);

                return res;


        }

        public async Task<Response> DeleteEducation(
            long SrNo,
            long? DeletedBy,
            string? IPAddress)
        {
            try
            {
                var res = await _crudOperationService
                    .InsertUpdateDelete<Response>(
                        "[dbo].[Usp_DeleteEducationDetails]",
                        new
                        {
                            SrNo,
                            DeletedBy,
                            IPAddress
                        },
                        ConnectionNames.Profile);

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
