
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Partner.Events;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Shared;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;
using PartnerService.Application.Shared;

namespace PartnerService.Tests.Integration.Shared.Events;

[Collection("db")]
public class DomainEventsDispatcherIntegrationTests
{
    private readonly InMemoryFixture _fixture;

    public DomainEventsDispatcherIntegrationTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Dispatch_ShouldLogWhenPartnerCreatedEventIsHandled()
    {
        // Arrange
        var logger = Substitute.For<ILogger<PartnerCreatedEventHandler>>();

        // Cria um scope com o logger substituído
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfraShared(new ConfigurationBuilder().Build(), useInMemoryDatabase: true);
        services.AddApplication();

        services.AddSingleton(logger);

        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var commandHandler = new CreatePartnerCommandHandler(
            scope.ServiceProvider.GetRequiredService<IPartnerCommandRepository>(),
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>(),
            new CreatePartnerCommandValidator(),
            scope.ServiceProvider.GetRequiredService<IPartnerQueryRepository>(),
            scope.ServiceProvider.GetRequiredService<ILogger<CreatePartnerCommandHandler>>()
        );

        var cmd = new CreatePartnerCommand
        {
            TradingName = "Ze delivery",
            OwnerName = "Gabriel cesario",
            Document = "06369660000120"
        };

        // Act
        await commandHandler.Handle(cmd);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v =>
                v.ToString()!.Contains("Ze delivery") &&
                v.ToString()!.Contains("Gabriel cesario") &&
                v.ToString()!.Contains("06369660000120")
            ),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>()
        );

        await db.Database.EnsureDeletedAsync();
    }
}