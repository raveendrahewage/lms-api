using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Azure.Models
{
    public record PdfJobMessage(
        int FileId,
        string JobId,
        int UserId,
        string BlobName,
        string OriginalFileName
    );
}
