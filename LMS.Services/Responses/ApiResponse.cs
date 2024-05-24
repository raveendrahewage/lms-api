using LMS.Data.Models;
using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Responses
{
    public class ApiResponse<T>
    {
        public ApiResponse(
            bool success,
            T? data
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
            T? data
        ) {
            this.Success = success;
            this.Message = message;
            this.Data = data;
        }
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
