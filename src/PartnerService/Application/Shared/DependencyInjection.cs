// Application/DependencyInjection.cs
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PartnerService.Application.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos os validators do assembly automaticamente
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}