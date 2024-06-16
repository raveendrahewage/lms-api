using LMS.Services.Helpers.Interfaces;
using LMS.Services.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace LMS.Api.Middleware
{
    public class ApiExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionHandlerMiddleware> _logger;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ApiExceptionHandlerMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlerMiddleware> logger, IApiResponseHelper apiResponseHelper)
        {
            _next = next;
            _logger = logger;
            _apiResponseHelper = apiResponseHelper;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _apiResponseHelper.GenerateApiResponse(false, exception.Message);

            var jsonResponse = JsonConvert.SerializeObject(response, _jsonSerializerSettings);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
