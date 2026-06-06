
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Api.Shared.DataObjects;
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Partner.Queries.GetById;
using PartnerService.Application.Shared.ValueObjects;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.Integration.Api.Partner;

[Collection("api")]
public class PartnerCreateEndpointIntegrationTests
{
    private readonly ApiFixture _fixture;

    public PartnerCreateEndpointIntegrationTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreatePartner_ShouldReturnCreated()
    {
        var client = _fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/partners", new CreatePartnerCommand
        {
            TradingName = "Ze delivery",
            OwnerName = "Gabriel cesario",
            Document = "06369660000120"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<PartnerViewModel>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.TradingName.Should().Be("Ze delivery");
        content.Data.OwnerName.Should().Be("Gabriel cesario");
        content.Data.Document.Should().Be("06369660000120");
    }

    [Fact]
    public async Task CreatePartner_ShouldReturnBadRequest_WhenInvalidData()
    {
        var client = _fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/partners", new CreatePartnerCommand
        {
            TradingName = "",
            OwnerName = "",
            Document = "invalid"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ValidationFailure>>>();

        content.Should().NotBeNull();
        content!.Success.Should().BeFalse();
        content.Data.Should().Contain(e => e.PropertyName == nameof(CreatePartnerCommand.TradingName));
        content.Data.Should().Contain(e => e.PropertyName == nameof(CreatePartnerCommand.OwnerName));
        content.Data.Should().Contain(e => e.PropertyName == nameof(CreatePartnerCommand.Document));
    }

    [Fact]
    public async Task CreatePartner_ShouldReturnConflict_WhenDuplicateDocument()
    {
        var client = _fixture.CreateClient();

        var command = new CreatePartnerCommand
        {
            TradingName = "Ze delivery",
            OwnerName = "Gabriel cesario",
            Document = "06369660000120"
        };

        await client.PostAsJsonAsync("/partners", command);
        var response = await client.PostAsJsonAsync("/partners", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ValidationFailure>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeFalse();
        content.Data.Should().Contain(e => e.PropertyName == nameof(CreatePartnerCommand.Document) &&
                                           e.ErrorMessage.Contains("already exists", StringComparison.OrdinalIgnoreCase));
    }
}