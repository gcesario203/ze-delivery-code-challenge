
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Shared.ValueObjects;
using PartnerService.Infra.Partner.Repositories.Commands;
using PartnerService.Infra.Partner.Repositories.Queries;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Infra.Shared.Persistence;

[Collection("db")]
public class UnitOfWorkUnitTests
{
    private readonly InMemoryFixture _fixture;

    public UnitOfWorkUnitTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_ShouldCommitTransaction()
    {
        Guid partnerId;

        using (var scope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            await unitOfWork.BeginTransactionAsync();

            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("54017426000187"));

            await commandRepository.AddAsync(partner);

            await unitOfWork.CommitTransactionAsync();

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
            Assert.Equal("54017426000187", retrievedPartner.Document.Value);
        }
    }

    [Fact]
    public async Task RollbackAsync_ShouldRollbackTransaction()
    {
        using (var scope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            await unitOfWork.BeginTransactionAsync();

            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("40092311000142"));

            await commandRepository.AddAsync(partner);

            await unitOfWork.RollbackTransactionAsync();
        }

        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByCnpjAsync("40092311000142");

            Assert.Null(retrievedPartner);
        }
    }

    [Fact]
    public async Task CommitAsync_ShouldHandleMultipleOperationsInTransaction()
    {
        Guid partnerId;

        using (var scope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            await unitOfWork.BeginTransactionAsync();

            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("13632699000154"));
            await commandRepository.AddAsync(partner);

            partner.UpdateTradingName("Ze delivery updated");
            await commandRepository.UpdateAsync(partner);

            await unitOfWork.CommitTransactionAsync();

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
            Assert.Equal("Ze delivery updated", retrievedPartner.TradingName);
            Assert.Equal("Gabriel cesario", retrievedPartner.OwnerName);
            Assert.Equal("13632699000154", retrievedPartner.Document.Value);
        }
    }

    [Fact]
    public async Task RollbackAsync_ShouldHandleMultipleOperationsInTransaction()
    {
        using (var scope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            await unitOfWork.BeginTransactionAsync();

            var commandRepository = new PartnerCommandRepository(appDbContext);

            var partner = new PartnerEntity("Ze delivery", "Gabriel cesario", new CnpjVO("40177245000103"));
            await commandRepository.AddAsync(partner);

            partner.UpdateTradingName("Ze delivery updated");
            await commandRepository.UpdateAsync(partner);

            await unitOfWork.RollbackTransactionAsync();
        }

        using (var queryScope = _fixture.ServiceProvider.CreateScope())
        {
            var appDbContext = queryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = new UnitOfWork(appDbContext);
            var queryRepository = new PartnerQueryRepository(unitOfWork);
            var retrievedPartner = await queryRepository.GetByCnpjAsync("40177245000103");

            Assert.Null(retrievedPartner);
        }
    }
}