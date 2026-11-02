using Application.Application.Interfaces;
using Application.Domain.Application;
using CrudOperations;
using Shared.Kernel;
using System.Threading.Tasks;

namespace Application.Infrastructure.Application
{
    public class AttendanceMusterRepository(ICrudOperationService crudOperationService) : IAttendanceMusterRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;

        public async Task<ResponseList<AttendanceMusterModel>> GetPhysicalScheduleDetails(
            string unitId,
            int page,
            int pageSize,
            string search)
        { 
            return await _crudOperationService.GetList<AttendanceMusterModel>(
                "[dbo].[Usp_GetPhysicalScheduleDetails]",
                new
                {
                    UnitID = unitId,
                    Page = page,
                    PageSize = pageSize,
                    Search = search
                },
                ConnectionNames.Applications);
        }

        public async Task<ResponseList<AttendancePdfRowModel>> GetAttendancePdfData(int filterId)
        {
            return await _crudOperationService.GetList<AttendancePdfRowModel>(
                "SP_GetPhysicalAttendancePDF",
                new
                {
                    Filter_ID = filterId
                },
                ConnectionNames.Applications);
        }
    }
}
