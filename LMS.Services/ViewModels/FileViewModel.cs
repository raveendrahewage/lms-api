using LMS.Data.Enum;
using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMS.Services.ViewModels
{
    public class FileViewModel:DataRecordViewModel
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public FileCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; }
        public FileStatus FileStatus { get; set; }
        public int UploadedById { get; set; }
        public virtual SystemUserViewModel? UploadedBy { get; set; }
    }
}
