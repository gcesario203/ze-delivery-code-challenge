
using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Partner.Events;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Tests.UnitTests.Domain.Partner.Entities;


public class Partner
{
    [Fact]
    public void CreatePartner_ValidData_ShouldCreatePartner()
    {
        // Arrange
        var tradingName = "Test Trading Name";
        var ownerName = "Test Owner Name";
        var document = new CnpjVO("12345678000195");

        // Act
        var partner = new PartnerEntity(tradingName, ownerName, document);

        // Assert
        Assert.NotNull(partner);
        Assert.Equal(tradingName, partner.TradingName);
        Assert.Equal(ownerName, partner.OwnerName);
        Assert.Equal(document, partner.Document);
        Assert.NotEqual(Guid.Empty, partner.Id);
    }

    [Fact]
    public void CreatePartner_ValidDataWithId_ShouldCreatePartner()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tradingName = "Test Trading Name";
        var ownerName = "Test Owner Name";
        var document = new CnpjVO("12345678000195");

        // Act
        var partner = new PartnerEntity(id, tradingName, ownerName, document);

        // Assert
        Assert.NotNull(partner);
        Assert.Equal(id, partner.Id);
        Assert.Equal(tradingName, partner.TradingName);
        Assert.Equal(ownerName, partner.OwnerName);
        Assert.Equal(document, partner.Document);
    }

    [Fact]
    public void CreatePartner_InvalidId_ShouldThrowException()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var tradingName = "Test Trading Name";
        var ownerName = "Test Owner Name";
        var document = new CnpjVO("12345678000195");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PartnerEntity(invalidId, tradingName, ownerName, document));
    }

    [Fact]
    public void CreatePartner_MissingTradingName_ShouldThrowException()
    {
        // Arrange
        var ownerName = "Test Owner Name";
        var document = new CnpjVO("12345678000195");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PartnerEntity(string.Empty, ownerName, document));
    }

    [Fact]
    public void CreatePartner_MissingOwnerName_ShouldThrowException()
    {
        // Arrange
        var tradingName = "Test Trading Name";
        var document = new CnpjVO("12345678000195");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PartnerEntity(tradingName, string.Empty, document));
    }

    [Fact]
    public void CreatePartner_MissingDocument_ShouldThrowException()
    {
        // Arrange
        var tradingName = "Test Trading Name";
        var ownerName = "Test Owner Name";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PartnerEntity(tradingName, ownerName, null));
    }

    [Fact]
    public void UpdateTradingName_ValidData_ShouldUpdateTradingName()
    {
        // Arrange
        var partner = new PartnerEntity("Old Trading Name", "Owner Name", new CnpjVO("12345678000195"));
        var newTradingName = "New Trading Name";

        // Act
        partner.UpdateTradingName(newTradingName);

        // Assert
        Assert.Equal(newTradingName, partner.TradingName);
    }

    [Fact]
    public void UpdateTradingName_MissingTradingName_ShouldThrowException()
    {
        // Arrange
        var partner = new PartnerEntity("Old Trading Name", "Owner Name", new CnpjVO("12345678000195"));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => partner.UpdateTradingName(string.Empty));
    }

    [Fact]
    public void UpdateOwnerName_ValidData_ShouldUpdateOwnerName()
    {
        // Arrange
        var partner = new PartnerEntity("Trading Name", "Old Owner Name", new CnpjVO("12345678000195"));
        var newOwnerName = "New Owner Name";

        // Act
        partner.UpdateOwnerName(newOwnerName);

        // Assert
        Assert.Equal(newOwnerName, partner.OwnerName);
    }

    [Fact]
    public void UpdateOwnerName_MissingOwnerName_ShouldThrowException()
    {
        // Arrange
        var partner = new PartnerEntity("Trading Name", "Old Owner Name", new CnpjVO("12345678000195"));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => partner.UpdateOwnerName(string.Empty));
    }

    [Fact]
    public void UpdateDocument_ValidData_ShouldUpdateDocument()
    {
        // Arrange
        var partner = new PartnerEntity("Trading Name", "Owner Name", new CnpjVO("12345678000195"));
        var newDocument = new CnpjVO("53665844000118");

        // Act
        partner.UpdateDocument(newDocument);

        // Assert
        Assert.Equal(newDocument, partner.Document);
    }

    [Fact]
    public void UpdateDocument_MissingDocument_ShouldThrowException()
    {
        // Arrange
        var partner = new PartnerEntity("Trading Name", "Owner Name", new CnpjVO("12345678000195"));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => partner.UpdateDocument(null));
    }

    [Fact]
    public void CreatePartner_ValidData_ShouldCreateADomainEvent()
    {
        // Arrange
        var tradingName = "Test Trading Name";
        var ownerName = "Test Owner Name";
        var document = new CnpjVO("12345678000195");

        // Act
        var partner = new PartnerEntity(tradingName, ownerName, document);

        // Assert
        Assert.NotEmpty(partner.DomainEvents);
        Assert.IsType<PartnerCreatedEvent>(partner.DomainEvents.First());
    }
}