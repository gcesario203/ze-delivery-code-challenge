

using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Shared.ValueObjects;
using PartnerService.Infra.Shared.Models;

namespace PartnerService.Infra.Partner.Mappers;

public static class PartnerMapper
{
    public static PartnerDbModel ToDbModel(this PartnerEntity entity)
    {
        return new PartnerDbModel
        {
            Id = entity.Id.ToString(),
            TradingName = entity.TradingName,
            OwnerName = entity.OwnerName,
            Document = entity.Document.Value
        };
    }

    public static PartnerEntity ToEntity(this PartnerDbModel dbModel)
    {
        if (dbModel == null) return null;

        return new PartnerEntity(
            id: Guid.Parse(dbModel.Id),
            tradingName: dbModel.TradingName,
            ownerName: dbModel.OwnerName,
            document: new CnpjVO(dbModel.Document)
        );
    }
}