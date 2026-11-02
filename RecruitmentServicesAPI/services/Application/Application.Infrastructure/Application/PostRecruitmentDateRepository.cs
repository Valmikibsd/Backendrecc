using Application.Application.Interfaces;
using Application.Domain.Application;
using CrudOperations;
using Shared.Kernel;
using System.Threading.Tasks;

namespace Application.Infrastructure.Application
{
    public class PostRecruitmentDateRepository(ICrudOperationService crudOperationService) : IPostRecruitmentDateRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;

        public async Task<ResponseList<PostRecruitmentDateCheckModel>> CheckPostRecruitmentDates(int departmentId, int advertisementId, int postId)
        {
            return await _crudOperationService.GetList<PostRecruitmentDateCheckModel>(
                "[dbo].[Usp_CheckPostRecruitmentDates]",
                new
                {
                    Departmentid = departmentId,
                    AdvertisementId = advertisementId,
                    PostId = postId
                },
                ConnectionNames.Master);
        }
    }
}
