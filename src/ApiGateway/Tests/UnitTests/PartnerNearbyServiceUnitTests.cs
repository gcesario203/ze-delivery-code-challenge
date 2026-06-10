using ApiGateway.Models;
using ApiGateway.Services;
using FluentAssertions;
using NSubstitute;

namespace ApiGateway.Tests.UnitTests;

public class PartnerNearbyServiceUnitTests
{
    [Fact]
    public async Task GetNearestPartnerAsync_ShouldReturnNull_WhenGeolocationServiceReturnsNotFound()
    {
        var service = Substitute.For<IPartnerNearbyService>();
        service.GetNearestPartnerAsync(-23.551, -46.634, Arg.Any<CancellationToken>())
            .Returns((PartnerNearbyResponse)null);

        var result = await service.GetNearestPartnerAsync(-23.551, -46.634, CancellationToken.None);

        result.Should().BeNull();
    }
}
