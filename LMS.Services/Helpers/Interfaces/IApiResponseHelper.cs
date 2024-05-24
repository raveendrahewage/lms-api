using LMS.Services.Responses;

namespace LMS.Services.Helpers.Interfaces
{
    public interface IApiResponseHelper
    {
        ApiResponse<T> GenerateApiResponse<T>(bool success, T? data);
        ApiResponse<T> GenerateApiResponse<T>(bool success, string message);
        ApiResponse<T> GenerateApiResponse<T>(bool success, string message, T? data);
    }
}