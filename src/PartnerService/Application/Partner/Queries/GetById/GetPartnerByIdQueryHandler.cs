

using PartnerService.Domain.Partner.Repositories;

namespace PartnerService.Application.Partner.Queries.GetById;

public class GetPartnerByIdQueryHandler
{
    private readonly IPartnerQueryRepository _partnerQueryRepository;

    public GetPartnerByIdQueryHandler(IPartnerQueryRepository partnerQueryRepository)
    {
        _partnerQueryRepository = partnerQueryRepository;
    }
    
    public async Task<PartnerViewModel> Handle(GetPartnerByIdQuery query)
    {
        var partner = await _partnerQueryRepository.GetByIdAsync(query.Id);

        if (partner == null)
            return null;

        return partner.ToViewModel();
    }
}