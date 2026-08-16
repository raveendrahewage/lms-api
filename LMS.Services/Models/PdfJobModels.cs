using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services.Models
{
    public record PdfJobMessage(
        string JobId, 
        string UserId, 
        string BlobName, 
        string OriginalFileName
    );

    public record SubmitJobRequest(
        string BlobName, 
        string OriginalFileName
    );

    public record UploadResponse(
        string UploadUrl,
        string BlobName
    );
}
