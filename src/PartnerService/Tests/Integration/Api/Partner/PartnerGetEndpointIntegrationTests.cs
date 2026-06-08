

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PartnerService.Api.Shared.DataObjects;
using PartnerService.Application.Partner.Queries.GetById;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.Integration.Api.Partner;

[Collection("api")]
public class PartnerGetEndpointIntegrationTests
{
    private readonly ApiFixture _fixture;

    public PartnerGetEndpointIntegrationTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPartner_ShouldReturnPartner_WhenPartnerExists()
    {
        var client = _fixture.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/partners", PartnerTestData.CreateValidCommand("09744933000168"));
        createResponse.EnsureSuccessStatusCode();

        var content = await createResponse.Content.ReadFromJsonAsync<ApiResponse<PartnerViewModel>>();
        var partner = content!.Data!;

        var response = await client.GetAsync($"/partners/{partner.Id}");
        response.EnsureSuccessStatusCode();

        var getContent = await response.Content.ReadFromJsonAsync<ApiResponse<PartnerViewModel>>();
        getContent.Should().NotBeNull();
        getContent!.Success.Should().BeTrue();
        getContent.Data.Should().NotBeNull();
        getContent.Data!.Id.Should().Be(partner.Id);
        getContent.Data.TradingName.Should().Be(partner.TradingName);
        getContent.Data.OwnerName.Should().Be(partner.OwnerName);
        getContent.Data.Document.Should().Be(partner.Document);
    }

    [Fact]
    public async Task GetPartner_ShouldReturnNotFound_WhenPartnerDoesNotExist()
    {
        var client = _fixture.CreateClient();

        var response = await client.GetAsync($"/partners/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
