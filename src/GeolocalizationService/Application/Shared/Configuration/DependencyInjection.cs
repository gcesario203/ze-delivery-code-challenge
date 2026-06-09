

using FluentValidation;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace GeolocalizationService.Application.Shared.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICreatePartnerGeolocation, CreatePartnerGeolocation>();
        services.AddScoped<IGetPartnerGeolocationByPartnerId, GetPartnerGeolocationByPartnerId>();
        services.AddScoped<IUpdatePartnerGeolocation, UpdatePartnerGeolocation>();
        services.AddScoped<IGetNearestPartnerGeolocation, GetNearestPartnerGeolocation>();

        return services;
    }
}
