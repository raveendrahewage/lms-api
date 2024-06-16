using LMS.Services.Responses;

namespace LMS.Services.Helpers.Interfaces
{
    public interface IApiResponseHelper
    {
        ApiResponse GenerateApiResponse(bool success, string message);
        ApiResponse GenerateApiResponse(bool success, object? data);
        ApiResponse GenerateApiResponse(bool success, string message, object? data);
    }
}