using CrudOperations;
using Profile.Application.Interfaces;
using Profile.Infrastructure.Applicant;
using Shared.Auth;
using Shared.Kernel;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Profile);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

_ = builder.Configuration.GetConnectionString(ConnectionNames.Registration)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Registration}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Documents)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Documents}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Profile)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Profile}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Master)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Master}' is not configured.");

builder.Services.AddScoped<ICrudOperationService>(_ =>
    new CrudOperationDataAccess(builder.Configuration));
builder.Services.AddScoped<IApplicantRepository, ApplicantRepository>();
builder.Services.AddScoped<IEducationRepository, EducationRepository>();
builder.Services.AddScoped<IViewProfileRepository, ViewProfileRepository>();
builder.Services.AddScoped<IPersonalDetailsRepository, PersonalDetailsRepository>();
builder.Services.AddScoped<IOtherDetailsRepository, OtherDetailsRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

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
app.UseSession();
app.UseAuthentication();
app.UseTokenValidation();
app.UseAuthorization();
app.UseActivityLogging();
app.MapSharedHealth(ServiceNames.Profile);
app.MapControllers();
app.Run();
