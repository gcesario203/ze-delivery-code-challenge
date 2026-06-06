
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PartnerService.Api.Shared.DataObjects;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Api.Shared;

[Collection("api")]
public class GlobalExceptionMiddlewareUnitTests
{
    private readonly ApiFixture _fixture;

    public GlobalExceptionMiddlewareUnitTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnProblem_WhenUnhandledException()
    {
        var client = _fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/partners", new
        {
        });

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

        content.Should().NotBeNull();
        content!.Message.Should().Be("An unexpected error occurred. Please try again later.");
        content.Success.Should().BeFalse();
    }
}