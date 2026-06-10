namespace ZeDelivery.Contracts.Messaging;

public sealed class PartnerCreatedIntegrationMessage
{
    public Guid PartnerId { get; set; }

    public AddressMessage Address { get; set; }

    public CoverageAreaMessage CoverageArea { get; set; }
}

public sealed class AddressMessage
{
    public CoordinateMessage Cordinates { get; set; }
}

public sealed class CoordinateMessage
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

public sealed class CoverageAreaMessage
{
    public List<List<List<CoordinateMessage>>> Cordinates { get; set; }
}
