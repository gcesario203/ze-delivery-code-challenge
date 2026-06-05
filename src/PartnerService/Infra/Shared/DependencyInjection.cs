

using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PartnerService.Application.Partner.Events;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.Events;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Partner.Repositories.Commands;
using PartnerService.Infra.Partner.Repositories.Queries;
using PartnerService.Infra.Shared.Persistence;

namespace PartnerService.Infra.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraShared(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useInMemoryDatabase = false)
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
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPartnerCommandRepository, PartnerCommandRepository>();
        services.AddScoped<IPartnerQueryRepository, PartnerQueryRepository>();

        services.Scan(scan => scan
            .FromAssemblyOf<PartnerCreatedEventHandler>()
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}