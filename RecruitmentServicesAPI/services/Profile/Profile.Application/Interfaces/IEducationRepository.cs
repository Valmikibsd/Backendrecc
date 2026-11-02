using CrudOperations;
using Profile.Domain.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Application.Interfaces
{
    public interface IEducationRepository
    {
        Task<Response> SaveEducation(EducationRequest obj);

        Task<ResponseList<EducationDetailsResponse>> GetEducationDetails(
            long UserId);

        Task<Response<CheckDegreeCourseResponse>> CheckDegreeCourse(
       long UserId,
       int DegreeId,
       int CourseId,
       long? Eid);

        Task<Response> DeleteEducation(
            long SrNo,
            long? DeletedBy,
            string? IPAddress);
    }
}
