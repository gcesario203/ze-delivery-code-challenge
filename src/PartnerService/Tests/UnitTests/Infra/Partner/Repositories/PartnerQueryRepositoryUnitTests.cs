
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Shared.ValueObjects;
using PartnerService.Infra.Partner.Repositories.Commands;
using PartnerService.Infra.Partner.Repositories.Queries;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Infra;

[Collection("db")]
public class PartnerQueryRepositoryUnitTests
{
    private readonly InMemoryFixture _fixture;

    public PartnerQueryRepositoryUnitTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByCnpjAsync_ShouldReturnPartner_WhenPartnerExists()
    {
        Guid partnerId;

        using (var commandScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = commandScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("04090644000179"));

            await commandRepository.AddAsync(partner);

            Assert.NotEqual(Guid.Empty, partner.Id);

            partnerId = partner.Id;
        }

        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByCnpjAsync("04090644000179");

            Assert.NotNull(retrievedPartner);
            Assert.Equal(partnerId, retrievedPartner.Id);
            Assert.Equal("Ze delivery", retrievedPartner.TradingName);
            Assert.Equal("Gabriel cesario", retrievedPartner.OwnerName);
            Assert.Equal("04090644000179", retrievedPartner.Document.Value);
        }
    }

    [Fact]
    public async Task GetByCnpjAsync_ShouldReturnNull_WhenPartnerDoesNotExist()
    {
        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByCnpjAsync("00000000000000");

            Assert.Null(retrievedPartner);
        }
    }
}