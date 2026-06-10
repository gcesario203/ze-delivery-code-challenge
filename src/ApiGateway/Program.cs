using ApiGateway.Models;
using ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("partner-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:PartnerService"] ?? "http://localhost:5114");
});

builder.Services.AddSingleton<IPartnerNearbyService, PartnerNearbyService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/partners/nearby", async (
    double latitude,
    double longitude,
    IPartnerNearbyService partnerNearbyService,
    CancellationToken cancellationToken) =>
{
    var partner = await partnerNearbyService.GetNearestPartnerAsync(latitude, longitude, cancellationToken);

    if (partner is null)
        return Results.NotFound(new ApiResponse<string>(null, "No partner found for the given location.", false));

    return Results.Ok(new ApiResponse<PartnerNearbyResponse>(partner));
})
.WithName("GetNearestPartnerByGeolocation")
.WithTags("Partners");

app.MapReverseProxy();

app.Run();

public partial class Program;
