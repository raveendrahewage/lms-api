using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Azure.Models
{
    public record PdfJobMessage(
        string JobId,
        string UserId,
        string BlobName,
        string OriginalFileName
    );
}
