
using ImTools;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Partner.Mappers;
using PartnerService.Infra.Shared.Models;
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
        var dbModel = partner.ToDbModel();

        var alreadyTracked = _context.ChangeTracker
            .Entries<PartnerDbModel>()
            .Any(e => e.Entity.Id == dbModel.Id);

        if (!alreadyTracked)
            await _context.Partners.AddAsync(dbModel);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PartnerEntity partner)
    {
        var dbModel = partner.ToDbModel();

        var entry = _context.ChangeTracker
            .Entries<PartnerDbModel>()
            .FirstOrDefault(e => e.Entity.Id == dbModel.Id);

        if (entry != null)
            entry.CurrentValues.SetValues(dbModel);
        else
            _context.Partners.Update(dbModel);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PartnerEntity partner)
    {
        var dbModel = partner.ToDbModel();

        var entry = _context.ChangeTracker
            .Entries<PartnerDbModel>()
            .FirstOrDefault(e => e.Entity.Id == dbModel.Id);

        if (entry != null)
            _context.Partners.Remove(entry.Entity);
        else
            _context.Partners.Remove(dbModel);

        await _context.SaveChangesAsync();
    }
}