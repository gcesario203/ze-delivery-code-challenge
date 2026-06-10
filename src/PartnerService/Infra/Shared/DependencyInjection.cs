

using System.Data;
using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using PartnerService.Application.Partner.Events;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.Events;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Infra.GeoLocalization;
using PartnerService.Infra.Messaging;
using PartnerService.Infra.Partner.Repositories.Commands;
using PartnerService.Infra.Partner.Repositories.Queries;
using PartnerService.Infra.Shared.EventBus;
using PartnerService.Infra.Shared.Outbox;
using PartnerService.Infra.Shared.Persistence;
using Wolverine;

namespace PartnerService.Infra.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraShared(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useInMemoryDatabase = false,
        bool useInMemoryGeolocalization = false)
    {
        if (useInMemoryDatabase)
        {
            var keepAliveConnection = new SqliteConnection("Data Source=:memory:");
            keepAliveConnection.Open();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(keepAliveConnection));

            services.AddSingleton<IDbConnection>(keepAliveConnection);
        }
        else
        {
            var connString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connString));

            services.AddScoped<IDbConnection>(_ =>
            {
                var conn = new NpgsqlConnection(connString);
                conn.Open();
                return conn;
            });
        }
        services.AddScoped<IHandlerDispatcher, WolverineHandlerDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPartnerCommandRepository, PartnerCommandRepository>();
        services.AddScoped<IPartnerQueryRepository, PartnerQueryRepository>();
        services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        if (useInMemoryGeolocalization || configuration.GetValue("Geolocalization:UseInMemory", false))
        {
            services.AddSingleton<IGeolocalizationClient, InMemoryGeolocalizationClient>();
        }
        else
        {
            services.AddSingleton<IPartnerGeolocationPublisher, RabbitMqPartnerGeolocationPublisher>();
            services.AddSingleton<IGeolocalizationClient, GrpcGeolocalizationClient>();
        }

        services.AddHostedService<OutboxProcessorBackgroundService>();

        services.Scan(scan => scan
            .FromAssemblyOf<PartnerCreatedEventHandler>()
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IHostBuilder AddWolwerine(this IHostBuilder host)
    {
        return host.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(
                typeof(Application.Shared.DependencyInjection).Assembly);

            opts.UseRuntimeCompilation();

            opts.CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>();
            opts.CodeGeneration.AlwaysUseServiceLocationFor<IUnitOfWork>();
        });
    }
}