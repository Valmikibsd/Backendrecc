using Application.Domain.Application;
using CrudOperations;
using System.Threading.Tasks;

namespace Application.Application.Interfaces
{
    public interface IAttendanceMusterRepository
    {
        Task<ResponseList<AttendanceMusterModel>> GetPhysicalScheduleDetails(
            string unitId,
            int page,
            int pageSize,
            string search);

        Task<ResponseList<AttendancePdfRowModel>> GetAttendancePdfData(int filterId);
    }
}
