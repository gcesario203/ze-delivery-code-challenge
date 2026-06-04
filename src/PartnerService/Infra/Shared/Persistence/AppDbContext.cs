

using Microsoft.EntityFrameworkCore;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Infra.Shared.Models;

namespace PartnerService.Infra.Shared.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<PartnerDbModel> Partners => Set<PartnerDbModel>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}