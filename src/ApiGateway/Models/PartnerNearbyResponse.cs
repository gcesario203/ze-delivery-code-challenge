namespace ApiGateway.Models;

public sealed class PartnerNearbyResponse
{
    public string Id { get; set; }

    public string TradingName { get; set; }

    public string OwnerName { get; set; }

    public string Document { get; set; }

    public AddressResponse Address { get; set; }

    public CoverageAreaResponse CoverageArea { get; set; }
}

public sealed class AddressResponse
{
    public CoordinateResponse Cordinates { get; set; }
}

public sealed class CoordinateResponse
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

public sealed class CoverageAreaResponse
{
    public List<List<List<CoordinateResponse>>> Cordinates { get; set; }
}

public sealed class ApiResponse<T>
{
    public ApiResponse(T data, string message = null, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }

    public T Data { get; set; }

    public string Message { get; set; }

    public bool Success { get; set; }
}
