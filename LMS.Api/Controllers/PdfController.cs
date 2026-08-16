using LMS.Api.Helpers;
using LMS.Api.Helpers.Interfaces;
using LMS.Service.Services;
using LMS.Services.Azure.Interfaces;
using LMS.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/pdf")]
[Authorize]
public class PdfController(IAzureStorageService azureStorageService, IApiResponseHelper apiResponseHelper) : ControllerBase
{
    private readonly IAzureStorageService _azureStorageService = azureStorageService;
    private readonly IApiResponseHelper _apiResponseHelper = apiResponseHelper;

    [HttpGet("generate-upload-url")]
    public IActionResult GenerateUploadUrl([FromQuery] string fileName)
    {
        var uploadUrl = _azureStorageService.GenerateUploadSasUrl(fileName, out var blobName);
        return Ok(_apiResponseHelper.GenerateApiResponse(true, new UploadResponse(uploadUrl, blobName)));
    }

    [HttpPost("submit-job")]
    public async Task<IActionResult> SubmitJob([FromBody] SubmitJobRequest request)
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int parsedUserId) ? parsedUserId : 0;

        var jobId = Guid.NewGuid().ToString();
        var jobPayload = new PdfJobMessage(jobId, userId, request.BlobName, request.OriginalFileName);

        await _azureStorageService.EnqueuePdfJobAsync(jobPayload);

        return Ok(_apiResponseHelper.GenerateApiResponse(true, "Processing queued successfully.", jobId));
    }
}