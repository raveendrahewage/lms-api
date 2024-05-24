using LMS.Services.Helpers.Interfaces;
using LMS.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Helpers
{
    public class ApiResponseHelper : IApiResponseHelper
    {
        public ApiResponse<T> GenerateApiResponse<T>(bool success, T? data)
            => new ApiResponse<T>(success, data);
        public ApiResponse<T> GenerateApiResponse<T>(bool success, string message)
            => new ApiResponse<T>(success, message);
        public ApiResponse<T> GenerateApiResponse<T>(bool success, string message, T? data)
            => new ApiResponse<T>(success, message, data);
    }
}
