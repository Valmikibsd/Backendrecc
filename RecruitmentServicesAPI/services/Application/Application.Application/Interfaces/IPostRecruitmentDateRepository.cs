using Application.Domain.Application;
using CrudOperations;
using System.Threading.Tasks;

namespace Application.Application.Interfaces
{
    public interface IPostRecruitmentDateRepository
    {
        Task<ResponseList<PostRecruitmentDateCheckModel>> CheckPostRecruitmentDates(int departmentId, int advertisementId, int postId);
    }
}
