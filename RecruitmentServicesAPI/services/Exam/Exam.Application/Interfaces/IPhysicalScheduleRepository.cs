using CrudOperations;
using Exam.Domain.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Application.Interfaces
{
    public interface IPhysicalScheduleRepository
    {
        Task<ResponseList<PhysicalScheduleMasterResponse>> GetPhysicalScheduleMaster( PhysicalScheduleMasterRequest request);

        Task<PhysicalScheduleSearchResponse> SearchPhysicalScheduleApplications( PhysicalScheduleSearchRequest request);

        Task<GeneratePhysicalScheduleResponse> GeneratePhysicalSchedule( GeneratePhysicalScheduleRequest request);

        Task<WrittenScheduleSearchResponse>SearchWrittenScheduleApplications( WrittenScheduleSearchRequest request);

        Task<GenerateWrittenScheduleResponse> GenerateWrittenSchedule( GenerateWrittenScheduleRequest request);

        Task<WrittenScheduleSearchResponse> SearchGeneralWrittenScheduleApplications(GeneralWrittenScheduleSearchRequest request);

        Task<GenerateWrittenScheduleResponse> GenerateGeneralWrittenSchedule(GenerateGeneralWrittenScheduleRequest request);

    }
}
