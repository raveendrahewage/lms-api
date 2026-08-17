using LMS.Services.Models;

namespace LMS.Services.Azure.Interfaces
{
    public interface IAzureStorageService
    {
        string GenerateUploadSasUrl(string fileName, out string blobName);
        string GenerateDownloadSasUrl(string blobName, string fileName);
        Task EnqueuePdfJobAsync(PdfJobMessage jobMessage);
    }
}
