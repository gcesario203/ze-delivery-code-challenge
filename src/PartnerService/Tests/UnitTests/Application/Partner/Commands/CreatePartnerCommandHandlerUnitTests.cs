
using Microsoft.Extensions.Logging;
using NSubstitute;
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.Exceptions;
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Domain.Shared.ValueObjects;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Application.Partner.Commands;

public class CreatePartnerCommandHandlerUnitTests
{
    [Fact]
    public async Task Handle_ShouldCreatePartner_WhenCommandIsValid()
    {
        // Arrange
        var command = PartnerTestData.CreateValidCommand("74310629000174");

        var mockRepo = Substitute.For<IPartnerCommandRepository>();
        var mockLogger = Substitute.For<ILogger<CreatePartnerCommandHandler>>();
        var mockUnitOfWork = Substitute.For<IUnitOfWork>();
        var validator = new CreatePartnerCommandValidator();
        var mockQueryRepo = Substitute.For<IPartnerQueryRepository>();

        var handler = new CreatePartnerCommandHandler(mockRepo,
                                                      mockUnitOfWork,
                                                      validator,
                                                      mockQueryRepo,
                                                      mockLogger);

        // Act
        await handler.Handle(command);

        // Assert
        await mockRepo.Received(1).AddAsync(Arg.Is<PartnerEntity>(p =>
            p.TradingName == command.TradingName &&
            p.OwnerName == command.OwnerName &&
            p.Document.Value == command.Document));
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreatePartnerCommand
        {
            TradingName = "",
            OwnerName = "",
            Document = "invalid-document"
        };

        var mockRepo = Substitute.For<IPartnerCommandRepository>();
        var mockLogger = Substitute.For<ILogger<CreatePartnerCommandHandler>>();
        var mockUnitOfWork = Substitute.For<IUnitOfWork>();
        var validator = new CreatePartnerCommandValidator();
        var mockQueryRepo = Substitute.For<IPartnerQueryRepository>();

        var handler = new CreatePartnerCommandHandler(mockRepo,
                                                      mockUnitOfWork,
                                                      validator,
                                                      mockQueryRepo,
                                                      mockLogger);

        // Act & Assert
        await Assert.ThrowsAsync<CommandValidationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenDocumentAlreadyExists()
    {
        // Arrange
        var command = PartnerTestData.CreateValidCommand("74310629000174");

        var existingPartner = new PartnerEntity("Existing Partner", "Existing Owner", new CnpjVO(command.Document));

        var mockRepo = Substitute.For<IPartnerCommandRepository>();
        var mockLogger = Substitute.For<ILogger<CreatePartnerCommandHandler>>();
        var mockUnitOfWork = Substitute.For<IUnitOfWork>();
        var validator = new CreatePartnerCommandValidator();
        var mockQueryRepo = Substitute.For<IPartnerQueryRepository>();
        mockQueryRepo.GetByCnpjAsync(command.Document).Returns(existingPartner);

        var handler = new CreatePartnerCommandHandler(mockRepo,
                                                      mockUnitOfWork,
                                                      validator,
                                                      mockQueryRepo,
                                                      mockLogger);

        // Act & Assert
        await Assert.ThrowsAsync<CommandValidationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_ShouldCommitUnitOfWork_WhenCommandIsValid()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IPartnerCommandRepository>();
        var validator = new CreatePartnerCommandValidator();
        var queryRepo = Substitute.For<IPartnerQueryRepository>();
        var logger = Substitute.For<ILogger<CreatePartnerCommandHandler>>();

        queryRepo.GetByCnpjAsync(Arg.Any<string>())
            .Returns((PartnerEntity)null);

        var handler = new CreatePartnerCommandHandler(
            repo, uow, validator, queryRepo, logger);

        var command = PartnerTestData.CreateValidCommand("22117401000169");

        await handler.Handle(command);

        await uow.Received(1).CommitAsync();
    }
}
