using Application.Application.Interfaces;
using Application.Domain.Application;
using CrudOperations;
using Shared.Kernel;
using System.Threading.Tasks;

namespace Application.Infrastructure.Application
{
    public class WrittenMusterRepository(ICrudOperationService crudOperationService) : IWrittenMusterRepository
    {
        private readonly ICrudOperationService _crudOperationService = crudOperationService;

        public async Task<ResponseList<WrittenMusterModel>> GetWrittenScheduleDetails(
            string unitId,
            int page,
            int pageSize,
            string search)
        {
            return await _crudOperationService.GetList<WrittenMusterModel>(
                "[dbo].[Usp_GetWrittenScheduleDetails]",
                new
                {
                    UnitID = unitId,
                    Page = page,
                    PageSize = pageSize,
                    Search = search
                },
                ConnectionNames.Applications);
        }

        public async Task<ResponseList<GeneralWrittenMusterModel>> GetGeneralWrittenScheduleDetails(
            int departmentId,
            GeneralWrittenMusterPaginationRequest request,
            string search)
        {
            return await _crudOperationService.GetList<GeneralWrittenMusterModel>(
                "[dbo].[Usp_GetGeneralWrittenScheduleDetails]",
                new
                {
                    DepartmentId = departmentId,
                    request.AdvertisementId,
                    PostID = request.PostId,
                    request.Page,
                    request.PageSize,
                    Search = search
                },
                ConnectionNames.Applications);
        }

        public async Task<ResponseList<GeneralAttendancePdfRowModel>> GetGeneralWrittenAttendancePdfData(
            int departmentId,
            int advertisementId,
            int postId,
            int filterId)
        {
            return await _crudOperationService.GetList<GeneralAttendancePdfRowModel>(
                "dbo.SP_GetGeneralWrittenAttendancePDF",
                new
                {
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId,
                    PostID = postId,
                    Filter_ID = filterId
                },
                ConnectionNames.Applications);
        }

        public async Task<ResponseList<AttendancePdfRowModel>> GetWrittenAttendancePdfData(int filterId)
        {
            return await _crudOperationService.GetList<AttendancePdfRowModel>(
                "SP_GeWrittenAttendancePDF",
                new
                {
                    Filter_ID = filterId
                },
                ConnectionNames.Applications);
        }
    }
}
