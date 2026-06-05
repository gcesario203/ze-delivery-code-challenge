using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Shared.Persistence;

namespace PartnerService.Infra.Partner.Repositories.Commands;

public class PartnerCommandRepository : IPartnerCommandRepository
{
    private readonly AppDbContext _context;

    public PartnerCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PartnerEntity partner)
    {

        var alreadyTracked = _context.ChangeTracker
            .Entries<PartnerEntity>()
            .Any(e => e.Entity.Id == partner.Id);

        if (!alreadyTracked)
            await _context.Partners.AddAsync(partner);

    }

    public async Task UpdateAsync(PartnerEntity partner)
    {

        var entry = _context.ChangeTracker
            .Entries<PartnerEntity>()
            .FirstOrDefault(e => e.Entity.Id == partner.Id);

        if (entry != null)
            entry.CurrentValues.SetValues(entry.Entity);
        else
            _context.Partners.Update(partner);

    }

    public async Task DeleteAsync(PartnerEntity partner)
    {

        var entry = _context.ChangeTracker
            .Entries<PartnerEntity>()
            .FirstOrDefault(e => e.Entity.Id == partner.Id);

        if (entry != null)
            _context.Partners.Remove(entry.Entity);
        else
            _context.Partners.Remove(partner);

    }
}