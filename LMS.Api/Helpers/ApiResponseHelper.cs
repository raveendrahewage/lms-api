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
        public ApiResponse GenerateApiResponse(bool success, string message)
            => new ApiResponse(success, message);
        public ApiResponse GenerateApiResponse(bool success, object? data)
            => new ApiResponse(success, data);
        public ApiResponse GenerateApiResponse(bool success, string message, object? data)
            => new ApiResponse(success, message, data);
    }
}
