using LMS.Api.Helpers.Interfaces;
using LMS.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Api.Helpers
{
    public class ApiResponseHelper : IApiResponseHelper
    {
        public ApiResponse GenerateApiResponse(bool success, string message)
            => new(success, message);
        public ApiResponse GenerateApiResponse(bool success, object? data)
            => new(success, data);
        public ApiResponse GenerateApiResponse(bool success, string message, object? data)
            => new(success, message, data);
    }
}
