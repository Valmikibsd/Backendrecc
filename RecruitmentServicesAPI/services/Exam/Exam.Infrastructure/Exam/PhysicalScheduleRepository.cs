using CrudOperations;
using Dapper;
using Exam.Application.Interfaces;
using Exam.Domain.Exam;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Infrastructure.Exam
{
    public class PhysicalScheduleRepository: IPhysicalScheduleRepository
    {
        private readonly ICrudOperationService _crudOperationService;
        private readonly IConfiguration _configuration;

        public PhysicalScheduleRepository( ICrudOperationService crudOperationService, IConfiguration configuration)
        {
            _crudOperationService = crudOperationService;
            _configuration = configuration;
        }

        

        public async Task<ResponseList<PhysicalScheduleMasterResponse>>GetPhysicalScheduleMaster( PhysicalScheduleMasterRequest request)
        {
            var parameters = new
            {
                Action = request.Action,
                DepartmentId = request.DepartmentId,
                AdvertisementId = request.AdvertisementId,
                PostId = request.PostId,
                UnitId = request.UnitId
            };

            var response = await _crudOperationService.GetList<PhysicalScheduleMasterResponse>
                (
                        "[dbo].[USP_GetPhysicalScheduleMasters]",
                        parameters,
                        ConnectionNames.Applications
                );

            return response;
        }

        public async Task<PhysicalScheduleSearchResponse> SearchPhysicalScheduleApplications( PhysicalScheduleSearchRequest request)
        {
            var connectionString = _configuration.GetConnectionString("Applications");

            using var connection = new SqlConnection(connectionString);

            var parameters = new DynamicParameters();

            parameters.Add( "@DepartmentId", request.DepartmentId);

            parameters.Add( "@AdvertisementId", request.AdvertisementId);

            parameters.Add( "@PostID", request.PostId);

            parameters.Add(  "@UnitID", request.UnitId);

            parameters.Add( "@Gender", request.Gender);

            parameters.Add( "@Category",  request.Category);

            parameters.Add( "@ExService", request.ExService);

            parameters.Add( "@SportMan", request.SportMan);

            parameters.Add( "@Capacity", request.Capacity);

            parameters.Add( "@ScheduledDate",  request.ScheduledDate.ToString("dd/MM/yyyy"));

            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync
                (
                    "[dbo].[Usp_GetPhysicalScheduleCount]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

            // Result Set 1
            var applications = (await multi.ReadAsync<PhysicalScheduleApplication>()).ToList();

            // Result Set 2
            var counts = await multi.ReadFirstOrDefaultAsync<PhysicalScheduleCount>();

            var availableApplications = applications.FirstOrDefault()?.TotalCount ?? 0;

            return new PhysicalScheduleSearchResponse
            {
                TotalApplications = counts?.TotalApplications ?? 0,

                AvailableApplications =  availableApplications,

                ScheduledApplications = counts?.ScheduledApplications ?? 0,

                PendingApplications = counts?.PendingApplications ?? 0,

                Applications = applications
            };
        }



        public async Task<GeneratePhysicalScheduleResponse> GeneratePhysicalSchedule(GeneratePhysicalScheduleRequest request)
        {
            var connectionString = _configuration.GetConnectionString("Applications");

            using var connection = new SqlConnection(connectionString);

            /*
             Example:
             PhysicalAttendance_11-09-2026_05-30-PM.pdf
            */

            string formattedTime;

            if (DateTime.TryParse(
                request.ScheduledTime,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedTime))
            {
                formattedTime =
                    parsedTime.ToString(
                        "hh-mm-tt",
                        CultureInfo.InvariantCulture);
            }
            else
            {
                // Fallback if time comes like 10:30 AM
                formattedTime = request.ScheduledTime
                    .Replace(":", "-")
                    .Replace(" ", "-");
            }

            string fileName =
                "PhysicalAttendance_" +
                request.ScheduledDate.ToString("dd-MM-yyyy") +
                "_" +
                formattedTime +
                ".pdf";


            var parameters = new DynamicParameters();

            parameters.Add(
                "@DepartmentId",
                request.DepartmentId,
                DbType.Int32);

            parameters.Add(
                "@AdvertisementId",
                request.AdvertisementId,
                DbType.Int32);

            parameters.Add(
                "@PostID",
                request.PostId,
                DbType.String);

            parameters.Add(
                "@UnitID",
                request.UnitId,
                DbType.String);

            parameters.Add(
                "@Gender",
                request.Gender,
                DbType.Int32);

            parameters.Add(
                "@Category",
                request.Category,
                DbType.String);

            parameters.Add(
                "@ExService",
                request.ExService,
                DbType.Int32);

            parameters.Add(
                "@SportMan",
                request.SportMan,
                DbType.Int32);

            parameters.Add(
                "@Capacity",
                request.Capacity.ToString(),
                DbType.String);

            parameters.Add(
                "@CenterName",
                request.CenterName,
                DbType.String);

            parameters.Add(
                "@ScheduledDate",
                request.ScheduledDate.ToString("dd/MM/yyyy"),
                DbType.String);

            parameters.Add(
                "@ScheduledTime",
                request.ScheduledTime,
                DbType.String);

            parameters.Add(
                "@FilePath",
                fileName,
                DbType.String);


            await connection.OpenAsync();


            var result = await connection.QueryFirstOrDefaultAsync<dynamic>
                (
                    "[dbo].[Usp_GeneratePhysicalAttendance]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );


            if (result == null)
            {
                return new GeneratePhysicalScheduleResponse
                {
                    Success = false,
                    GeneratedCount = 0,
                    FileName = fileName,
                    Message = "Physical schedule generation failed."
                };
            }


            string strCount =  Convert.ToString(result.strCount) ?? "";


            if (strCount.Equals(
                "No_Record",
                StringComparison.OrdinalIgnoreCase))
            {
                return new GeneratePhysicalScheduleResponse
                {
                    Success = false,
                    GeneratedCount = 0,
                    FileName = fileName,
                    Message = "No eligible applications found."
                };
            }


            int generatedCount = 0;

            int.TryParse(
                strCount,
                out generatedCount);


            return new GeneratePhysicalScheduleResponse
            {
                Success = generatedCount > 0,

                GeneratedCount = generatedCount,

                FileName = fileName,

                Message = generatedCount > 0
                    ? $"{generatedCount} application(s) scheduled successfully."
                    : "No eligible applications found."
            };
        }

        public async Task<WrittenScheduleSearchResponse>SearchWrittenScheduleApplications( WrittenScheduleSearchRequest request)
        {
            var connectionString = _configuration.GetConnectionString("Applications");

            using var connection = new SqlConnection(connectionString);

            var parameters = new DynamicParameters();

            parameters.Add(
                "@DepartmentId",
                request.DepartmentId,
                DbType.Int32);

            parameters.Add(
                "@AdvertisementId",
                request.AdvertisementId,
                DbType.Int32);

            parameters.Add(
                "@PostID",
                request.PostId,
                DbType.Int32);

            parameters.Add(
                "@UnitID",
                request.UnitId,
                DbType.String);

            parameters.Add(
                "@Gender",
                request.Gender,
                DbType.Int32);


            await connection.OpenAsync();


            var result = await connection.QueryFirstOrDefaultAsync <WrittenScheduleSearchResponse>
                (
                    "[dbo].[Usp_GetWrittenCandidatesCount]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );


            return result ?? new WrittenScheduleSearchResponse
            {
                TotalCount = 0
            };
        }

        public async Task<GenerateWrittenScheduleResponse> GenerateWrittenSchedule( GenerateWrittenScheduleRequest request)
        {
            var connectionString = _configuration.GetConnectionString("Applications");

            using var connection =  new SqlConnection(connectionString);

            // Example:
            // WrittenAttendance_16-09-2026_05-00-PM.pdf
            string formattedTime = request.ScheduledTime
                .Replace(":", "-")
                .Replace(" ", "-");

            string fileName =
                "WrittenAttendance_" +
                request.ScheduledDate.ToString("dd-MM-yyyy") +
                "_" +
                formattedTime +
                ".pdf";

            var parameters = new DynamicParameters();

            parameters.Add("@DepartmentId",
                request.DepartmentId,
                DbType.Int32);

            parameters.Add("@AdvertisementId",
                request.AdvertisementId,
                DbType.Int32);

            parameters.Add("@UnitID",
                request.UnitId,
                DbType.String);

            parameters.Add("@Gender",
                request.Gender,
                DbType.Int32);

            parameters.Add("@Capacity",
                request.Capacity,
                DbType.Int32);

            parameters.Add("@CenterName",
                request.CenterName,
                DbType.String);

            parameters.Add("@ScheduledDate",
                request.ScheduledDate.Date,
                DbType.Date);

            parameters.Add("@ScheduledTime",
                request.ScheduledTime,
                DbType.String);

            parameters.Add("@PostID",
                request.PostId,
                DbType.Int32);

            parameters.Add("@FilePath",
                fileName,
                DbType.String);

            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>
                (
                    "[dbo].[Usp_GenerateWrittenAttendance]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

            if (result == null)
            {
                return new GenerateWrittenScheduleResponse
                {
                    Success = false,
                    GeneratedCount = 0,
                    FileName = fileName,
                    Message = "Written schedule generation failed."
                };
            }

            string strCount = Convert.ToString(result.strCount) ?? "";

            if (strCount.Equals(
                "No_Record",
                StringComparison.OrdinalIgnoreCase))
            {
                return new GenerateWrittenScheduleResponse
                {
                    Success = false,
                    GeneratedCount = 0,
                    FileName = fileName,
                    Message = "No eligible written applications found."
                };
            }

            int generatedCount = 0;
            int.TryParse(strCount, out generatedCount);

            return new GenerateWrittenScheduleResponse
            {
                Success = generatedCount > 0,
                GeneratedCount = generatedCount,
                FileName = fileName,

                Message = generatedCount > 0
                    ? $"{generatedCount} application(s) scheduled successfully."
                    : "No eligible written applications found."
            };
        }

        public async Task<WrittenScheduleSearchResponse> SearchGeneralWrittenScheduleApplications(GeneralWrittenScheduleSearchRequest request)
        {
            var result = await _crudOperationService.InsertUpdateDelete<WrittenScheduleSearchResponse>(
                "[dbo].[Usp_GetWrittenCandidatesCountGeneral]",
                new
                {
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    PostID = request.PostId,
                    Gender = request.Gender
                },
                ConnectionNames.Applications);

            return result ?? new WrittenScheduleSearchResponse
            {
                TotalCount = 0
            };
        }

        public async Task<GenerateWrittenScheduleResponse> GenerateGeneralWrittenSchedule(GenerateGeneralWrittenScheduleRequest request)
        {
            DateTime scheduledDate = request.ScheduledDate!.Value.Date;

            // Example: GeneralWrittenHallticket_16-09-2026_05-00-PM.pdf
            string formattedTime = request.ScheduledTime
                .Replace(":", "-")
                .Replace(" ", "-");

            string fileName =
                "GeneralWrittenHallticket_" +
                scheduledDate.ToString("dd-MM-yyyy") +
                "_" +
                formattedTime +
                ".pdf";

            var result = await _crudOperationService.InsertUpdateDelete<GeneralWrittenScheduleGenerateResult>(
                "[dbo].[Usp_GenerateWrittenHallticketGeneral]",
                new
                {
                    DepartmentId = request.DepartmentId,
                    AdvertisementId = request.AdvertisementId,
                    Gender = request.Gender,
                    Capacity = request.Capacity,
                    CenterName = request.CenterName,
                    ScheduledDate = scheduledDate,
                    ScheduledTime = request.ScheduledTime,
                    PostID = request.PostId,
                    FilePath = fileName
                },
                ConnectionNames.Applications);

            if (result == null)
            {
                return new GenerateWrittenScheduleResponse
                {
                    Success = false,
                    GeneratedCount = 0,
                    FileName = fileName,
                    Message = "General written schedule generation failed."
                };
            }

            string strCount = result.StrCount ?? "";

            if (strCount.Equals("No_Record", StringComparison.OrdinalIgnoreCase))
            {
                return new GenerateWrittenScheduleResponse
                {
                    Success = false,
                    GeneratedCount = 0,
                    FileName = fileName,
                    Message = "No eligible written applications found."
                };
            }

            int generatedCount = 0;
            int.TryParse(strCount, out generatedCount);

            return new GenerateWrittenScheduleResponse
            {
                Success = generatedCount > 0,
                GeneratedCount = generatedCount,
                FileName = fileName,

                Message = generatedCount > 0
                    ? $"{generatedCount} application(s) scheduled successfully."
                    : "No eligible written applications found."
            };
        }
    }
}
