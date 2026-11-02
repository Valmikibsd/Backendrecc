using CrudOperations;
using Exam.Application;
using Exam.Application.Interfaces;
using Exam.Infrastructure.Exam;
using Shared.Auth;
using Shared.Kernel;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Exam);

builder.Services.AddControllers();
builder.Services.AddSharedApiBehavior();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddExceptionHandling();
builder.Services.AddTokenValidation();
builder.Services.AddActivityLogging();
builder.Services.AddSharedCors(builder.Configuration);

_ = builder.Configuration.GetConnectionString(ConnectionNames.Applications)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Applications}' is not configured.");

builder.Services.AddScoped<ICrudOperationService,CrudOperationDataAccess>();
builder.Services.AddScoped<IPhysicalScheduleRepository, PhysicalScheduleRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsExtensions.PolicyName);
app.UseExceptionHandling();
app.UseAuthentication();
app.UseTokenValidation();
app.UseAuthorization();
app.UseActivityLogging();
app.MapSharedHealth(ServiceNames.Exam);
app.MapControllers();
app.Run();
