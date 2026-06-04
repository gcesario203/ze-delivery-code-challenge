namespace PartnerService.Domain.Shared.Entities;

public interface IEntity<TId>
{
    public TId Id { get; }
}