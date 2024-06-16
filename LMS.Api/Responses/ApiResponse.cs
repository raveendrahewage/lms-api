using LMS.Data.Models;
using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Responses
{
    public class ApiResponse
    {
        public ApiResponse(
            bool success,
            object? data
        )
        {
            this.Success = success;
            this.Data = data;
        }
        public ApiResponse(
            bool success,
            string message
        )
        {
            this.Success = success;
            this.Message = message;
        }
        public ApiResponse(
            bool success,
            string message,
            object? data
        ) {
            this.Success = success;
            this.Message = message;
            this.Data = data;
        }
        public bool Success { get; set; }
        public object? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
