using CrudOperations;
using Payment.Application;
using Payment.Application.Interfaces;
using Payment.Infrastructure.Payment;
using Shared.Auth;
using Shared.Kernel;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Payment);

_ = builder.Configuration.GetConnectionString(ConnectionNames.Payment)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Payment}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Applications)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Applications}' is not configured.");
_ = builder.Configuration.GetConnectionString(ConnectionNames.Master)
    ?? throw new InvalidOperationException($"Connection string '{ConnectionNames.Master}' is not configured.");

builder.Services.AddHttpClient<IPaymentClient, PaymentClient>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPaymentChecksumService, PaymentChecksumService>();
builder.Services.AddScoped<IPaymentCallbackService, PaymentCallbackService>();
builder.Services.AddScoped<IPaymentRequestLogRepository, PaymentRequestLogRepository>();
builder.Services.AddScoped<IPaymentCallbackRepository, PaymentCallbackRepository>();
builder.Services.AddScoped<IPaymentApplicationStatusRepository, PaymentApplicationStatusRepository>();
builder.Services.AddScoped<IAdvertisementPayServiceRepository, AdvertisementPayServiceRepository>();

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
app.MapSharedHealth(ServiceNames.Payment);
app.MapControllers();
app.Run();
