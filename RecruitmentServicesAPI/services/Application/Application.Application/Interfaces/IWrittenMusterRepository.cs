using Application.Domain.Application;
using CrudOperations;
using System.Threading.Tasks;

namespace Application.Application.Interfaces
{
    public interface IWrittenMusterRepository
    {
        Task<ResponseList<WrittenMusterModel>> GetWrittenScheduleDetails(
            string unitId,
            int page,
            int pageSize,
            string search);

        Task<ResponseList<GeneralWrittenMusterModel>> GetGeneralWrittenScheduleDetails(
            int departmentId,
            GeneralWrittenMusterPaginationRequest request,
            string search);

        Task<ResponseList<GeneralAttendancePdfRowModel>> GetGeneralWrittenAttendancePdfData(
            int departmentId,
            int advertisementId,
            int postId,
            int filterId);

        Task<ResponseList<AttendancePdfRowModel>> GetWrittenAttendancePdfData(int filterId);
    }
}
