using Shared.Kernel.Interfaces;
using Auth.Application.Interfaces;
using CrudOperations;
using RecruitmentPortal.Core.Aadhaar;
using RecruitmentPortal.Core.Email;
using RecruitmentPortal.InfraHelper;
using RecruitmentPortal.Infrastructure.Aadhaar;
using RecruitmentPortal.Infrastructure.Auth;
using Shared.Kernel.Services;
using RecruitmentPortal.Infrastructure.Registration;
using RecruitmentPortal.Infrastructure.Common;
using Shared.Auth;
using Shared.Kernel;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Auth);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

_ = builder.Configuration.GetConnectionString(ConnectionNames.Registration)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Registration}' is not configured.");

_ = builder.Configuration.GetConnectionString(ConnectionNames.Master)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Master}' is not configured.");

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new EmailConfig
    {
        Host = config["EMAIL:smtpServer"],
        Port = int.TryParse(config["EMAIL:port"], out var port) ? port : 587,
        FromEmail = config["EMAIL:fromAddress"],
        Username = config["EMAIL:accessKey"],
        Password = config["EMAIL:secretKey"],
        EnableSSL = true,
        Timeout = 100000
    };
});

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<Shared.Kernel.Interfaces.ISmsService, Shared.Kernel.Services.SmsService>();
builder.Services.AddScoped<ICrudOperationService>(_ =>
    new CrudOperationDataAccess(builder.Configuration));
builder.Services.AddScoped<IAuthLoginRepository, AuthLoginRepository>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IUIDService, UIDService>();
builder.Services.AddSingleton<Shared.Kernel.Interfaces.ICaptchaService, Shared.Kernel.Services.CaptchaService>();
builder.Services.Configure<AadhaarSettings>(builder.Configuration.GetSection("AadhaarSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<WebHelper>();
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
app.MapSharedHealth(ServiceNames.Auth);
app.MapControllers();
app.Run();
