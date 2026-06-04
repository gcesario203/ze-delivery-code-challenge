
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
        await _context.Partners.AddAsync(partner);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(PartnerEntity entity)
    {
        _context.Partners.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PartnerEntity partner)
    {
        _context.Partners.Update(partner);
        await _context.SaveChangesAsync();
    }
}