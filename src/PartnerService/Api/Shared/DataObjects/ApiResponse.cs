
namespace PartnerService.Api.Shared.DataObjects;

public sealed class ApiResponse<T>
{
    public T Data { get; set; }

    public string Message { get; set; }

    public bool Success { get; set; }

    public ApiResponse(T data, string message = null, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }
}