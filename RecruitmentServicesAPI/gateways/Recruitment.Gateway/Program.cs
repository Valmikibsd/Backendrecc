using Shared.Kernel;
using Shared.Kernel.Encryption;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedSerilog(ServiceNames.Gateway);
builder.Services.AddRequestResponseEncryption(builder.Configuration);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddExceptionHandling();
builder.Services.AddSharedCors(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsExtensions.PolicyName);
app.UseRequestResponseEncryption();
app.UseExceptionHandling();
app.MapSharedHealth(ServiceNames.Gateway);

app.MapReverseProxy();
app.Run();