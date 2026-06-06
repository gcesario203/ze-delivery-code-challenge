using PartnerService.Api.Partner.Presentation;
using PartnerService.Api.Shared.Middlewares;
using PartnerService.Application.Shared;
using PartnerService.Infra.Shared;
using PartnerService.Infra.Shared.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfraShared(builder.Configuration, useInMemoryDatabase: true);
builder.Host.AddWolwerine();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<GlobalExceptionsMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPartnerEndpoints();

app.Run();

public partial class Program { }