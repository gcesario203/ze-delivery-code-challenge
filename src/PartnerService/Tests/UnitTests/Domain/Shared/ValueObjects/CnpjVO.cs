
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Tests.UnitTests.Domain.Shared.ValueObjects;

public class CnpjVOUnitTests
{
    [Fact]
    public void CreateCnpjVO_ValidCnpj_ShouldCreateCnpjVO()
    {
        // Arrange
        var cnpj = "12345678000195";

        // Act
        var cnpjVO = new CnpjVO(cnpj);

        // Assert
        Assert.NotNull(cnpjVO);
        Assert.Equal(cnpj, cnpjVO.Value);
    }

    [Fact]
    public void CreateCnpjVO_InvalidCnpj_ShouldThrowException()
    {
        // Arrange
        var invalidCnpj = "invalid_cnpj";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CnpjVO(invalidCnpj));
    }

    [Fact]
    public void CreateCnpjVO_NullCnpj_ShouldThrowException()
    {
        // Arrange
        string nullCnpj = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CnpjVO(nullCnpj));
    }

    [Fact]
    public void CreateCnpjVO_EmptyCnpj_ShouldThrowException()
    {
        // Arrange
        var emptyCnpj = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CnpjVO(emptyCnpj));
    }

    [Fact]
    public void CreateCnpjVO_CnpjWithAllSameDigits_ShouldThrowException()
    {
        // Arrange
        var cnpjWithSameDigits = "11111111111111";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CnpjVO(cnpjWithSameDigits));
    }

    [Fact]
    public void CreateCnpjVO_CnpjWithInvalidDigits_ShouldThrowException()
    {
        // Arrange
        var cnpjWithInvalidDigits = "12345678000196"; // Last two digits are invalid

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CnpjVO(cnpjWithInvalidDigits));
    }
}