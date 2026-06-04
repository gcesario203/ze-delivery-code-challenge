
using Microsoft.Extensions.Logging;
using NSubstitute;
using PartnerService.Application.Partner.Queries.GetById;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Tests.UnitTests.Application.Partner.Queries;

public class GetByIdHandlerUnitTests
{
    [Fact]
    public async Task Handle_ShouldReturnPartnerViewModel_WhenPartnerExists()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partnerEntity = new PartnerEntity(partnerId, "Ze delivery", "Gabriel cesario", new CnpjVO("33557708000105"));

        var mockRepo = Substitute.For<IPartnerQueryRepository>();
        var mockLogger = Substitute.For<ILogger<GetPartnerByIdQueryHandler>>();
        mockRepo.GetByIdAsync(partnerId).Returns(partnerEntity);

        var handler = new GetPartnerByIdQueryHandler(mockRepo, mockLogger);
        var query = new GetPartnerByIdQuery { Id = partnerId };

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(partnerEntity.Id.ToString(), result.Id);
        Assert.Equal(partnerEntity.TradingName, result.TradingName);
        Assert.Equal(partnerEntity.OwnerName, result.OwnerName);
        Assert.Equal(partnerEntity.Document.Value, result.Document);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPartnerDoesNotExist()
    {
        // Arrange
        var partnerId = Guid.NewGuid();

        var mockRepo = Substitute.For<IPartnerQueryRepository>();
        var mockLogger = Substitute.For<ILogger<GetPartnerByIdQueryHandler>>();
        mockRepo.GetByIdAsync(partnerId).Returns((PartnerEntity)null);

        var handler = new GetPartnerByIdQueryHandler(mockRepo, mockLogger);
        var query = new GetPartnerByIdQuery { Id = partnerId };

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }
}