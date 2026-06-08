

using Microsoft.Extensions.Logging;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Domain.Partner.Repositories;
using Wolverine.Attributes;

namespace PartnerService.Application.Partner.Queries.GetById;

public class GetPartnerByIdQueryHandler
{
    private readonly IPartnerQueryRepository _partnerQueryRepository;

    private readonly ILogger<GetPartnerByIdQueryHandler> _logger;

    private readonly IGeolocalizationClient _geolocalizationClient;

    public GetPartnerByIdQueryHandler(IPartnerQueryRepository partnerQueryRepository,
                                      IGeolocalizationClient geolocalizationClient,
                                      ILogger<GetPartnerByIdQueryHandler> logger)
    {
        _partnerQueryRepository = partnerQueryRepository;
        _geolocalizationClient = geolocalizationClient;
        _logger = logger;
    }

    [WolverineHandler]
    public async Task<PartnerViewModel> Handle(GetPartnerByIdQuery query)
    {
        _logger.LogInformation("Handling GetPartnerByIdQuery for Id: {Id}", query.Id);
        var partner = await _partnerQueryRepository.GetByIdAsync(query.Id);

        if (partner == null)
        {
            _logger.LogWarning("No partner found with Id: {Id}", query.Id);
            return null;
        }

        _logger.LogInformation("Successfully retrieved partner with Id: {Id}", query.Id);

        var geolocalization = await _geolocalizationClient.GetPartnerGeolocalizationAsync(query.Id);

        return partner.ToViewModel(geolocalization);
    }
}