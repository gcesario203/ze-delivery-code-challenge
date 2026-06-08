
namespace GeolocalizationService.Domain.Shared.Entities;

public abstract class BaseEntity : IEntity<Guid>
{
    public Guid Id { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    protected BaseEntity(Guid id)
    {
        if(id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));
            
        Id = id;
    }
}