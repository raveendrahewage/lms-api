using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services.Models
{
    public record PdfJobMessage(
        int FileId,
        string JobId, 
        int UserId, 
        string BlobName, 
        string OriginalFileName
    );

    public record SubmitJobRequest(
        string BlobName, 
        string OriginalFileName,
        FileViewModel File
    );

    public record UploadResponse(
        string UploadUrl,
        string BlobName
    );
}
