
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Shared.Repositories;

namespace PartnerService.Domain.Partner.Repositories;

public interface IPartnerQueryRepository : IQueryRepository<PartnerEntity>
{
    Task<PartnerEntity> GetByCnpjAsync(string cnpj);
}