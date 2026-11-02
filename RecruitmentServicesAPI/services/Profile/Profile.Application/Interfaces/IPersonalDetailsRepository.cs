using Profile.Domain.Applicant;

using CrudOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Application.Interfaces
{
    public interface IPersonalDetailsRepository
    {
        Task<Response> SavePersonalDetails(PersonalDetailsRequest obj);
        Task<Response> SavePersonalDraftDetails(PersonalDetailsDraftRequest obj);

        Task<Response<PersonalDetailsRequest>> GetPersonalDetails(long id);

        Task<Response<ApplicantCreation>> GetAadhaarDetails(long id);
    }
}
