using GeolocalizationService.Api.Grpc.Services;
using GeolocalizationService.Api.Messaging;
using GeolocalizationService.Application.Shared.Configuration;
using GeolocalizationService.Infra.Shared.Configuration;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(endpointOptions =>
    {
        endpointOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddGrpc();
builder.Services.AddApplication();
builder.Services.AddInfraShared(builder.Configuration);
builder.Services.AddHostedService<PartnerCreatedConsumerBackgroundService>();

var app = builder.Build();

app.MapGrpcService<PartnerGeolocationGrpcService>();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

public partial class Program;
