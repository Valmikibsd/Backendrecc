using Application.Application;
using Application.Application.Interfaces;
using Application.Infrastructure.Application;
using CrudOperations;
using QuestPDF.Infrastructure;
using Shared.Auth;
using Shared.Kernel;
using Shared.Logging;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Application);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


_ = builder.Configuration.GetConnectionString(ConnectionNames.Applications)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Applications}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Registration)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Registration}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Master)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Master}' is not configured.");

builder.Services.AddScoped<ICrudOperationService>(_ =>
    new CrudOperationDataAccess(builder.Configuration));
builder.Services.AddScoped<IApplicantRepository, ApplicantRepository>();
builder.Services.AddScoped<IAttendanceMusterRepository, AttendanceMusterRepository>();
builder.Services.AddScoped<IAttendancePdfStorageService, AttendancePdfStorageService>();
builder.Services.AddScoped<IWrittenMusterRepository, WrittenMusterRepository>();
builder.Services.AddScoped<IPostRecruitmentDateRepository, PostRecruitmentDateRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();


builder.Services.AddControllers();
builder.Services.AddSharedApiBehavior();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddExceptionHandling();
builder.Services.AddTokenValidation();
builder.Services.AddScoped<Shared.Kernel.Interfaces.ISmsService, Shared.Kernel.Services.SmsService>();
builder.Services.AddSingleton<Shared.Kernel.Interfaces.ICaptchaService, Shared.Kernel.Services.CaptchaService>();
builder.Services.AddActivityLogging();
builder.Services.AddSharedCors(builder.Configuration);

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
app.MapSharedHealth(ServiceNames.Application);
app.MapControllers();
app.Run();
