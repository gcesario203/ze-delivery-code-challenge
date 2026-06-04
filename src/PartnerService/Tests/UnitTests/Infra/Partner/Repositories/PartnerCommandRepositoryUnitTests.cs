
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Shared.ValueObjects;
using PartnerService.Infra.Partner.Repositories.Commands;
using PartnerService.Infra.Partner.Repositories.Queries;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Infra;

[Collection("db")]
public class PartnerCommandRepositoryUnitTests
{
    private readonly InMemoryFixture _fixture;

    public PartnerCommandRepositoryUnitTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ShouldAddPartner()
    {
        Guid partnerId;

        using (var commandScope = _fixture.ServiceProvider.CreateScope())
        {

            var appDbContext = commandScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("33557708000105"));

            await commandRepository.AddAsync(partner);

            Assert.NotEqual(Guid.Empty, partner.Id);

            partnerId = partner.Id;
        }

        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByIdAsync(partnerId);

            Assert.NotNull(retrievedPartner);
            Assert.Equal(partnerId, retrievedPartner.Id);
            Assert.Equal("Ze delivery", retrievedPartner.TradingName);
            Assert.Equal("Gabriel cesario", retrievedPartner.OwnerName);
            Assert.Equal("33557708000105", retrievedPartner.Document.Value);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePartner()
    {
        Guid partnerId;

        using (var commandScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = commandScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("65409496000105"));
            await commandRepository.AddAsync(partner);
            partnerId = partner.Id;

            // Update
            partner.UpdateTradingName("Ze delivery updated");
            await commandRepository.UpdateAsync(partner);
        }

        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByIdAsync(partnerId);

            Assert.NotNull(retrievedPartner);
            Assert.Equal("Ze delivery updated", retrievedPartner.TradingName);
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePartner()
    {
        Guid partnerId;

        using (var commandScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = commandScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("06311678000171"));
            await commandRepository.AddAsync(partner);
            partnerId = partner.Id;

            // Delete
            await commandRepository.DeleteAsync(partner);
        }

        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByIdAsync(partnerId);

            Assert.Null(retrievedPartner);
        }
    }
}