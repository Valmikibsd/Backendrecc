using CrudOperations;
using Reports.Application.Interfaces;
using Reports.Infrastructure.Report;
using Shared.Auth;
using Shared.Kernel;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Reports);

_ = builder.Configuration.GetConnectionString(ConnectionNames.Master)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Master}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Applications)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Applications}' is not configured.");

builder.Services.AddScoped<ICrudOperationService>(_ =>
    new CrudOperationDataAccess(builder.Configuration));
builder.Services.AddScoped<IApplicationReportRepository, ApplicationReportRepository>();

builder.Services.AddControllers();
builder.Services.AddSharedApiBehavior();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddExceptionHandling();
builder.Services.AddTokenValidation();
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
app.MapSharedHealth(ServiceNames.Reports);
app.MapControllers();
app.Run();
