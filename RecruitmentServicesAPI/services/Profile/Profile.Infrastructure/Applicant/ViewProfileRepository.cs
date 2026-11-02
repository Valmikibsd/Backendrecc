using CrudOperations;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Infrastructure.Applicant
{
    public class ViewProfileRepository(ICrudOperationService crudOperationService) : IViewProfileRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;


        public async Task<Response<ApplicantViewProfileResponse>> ViewProfile(
       long UserId)
        {
            try
            {

                var res = await _crudOperationService
                    .BindDynamicLists(
                            "[dbo].[Usp_GetPersonalDetailsView]",
                            new
                            {
                                UserId
                            },
                            new List<string>
                            {
                            "PersonalDetails",
                            "EducationDetails",
                            "Address",
                            "OtherDetails",
                            "Status"
                            },
                            ConnectionNames.Profile);

                var response = new ApplicantViewProfileResponse
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

                return new Response<ApplicantViewProfileResponse>
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

    }
}
