using LMS.Data.Common;
using LMS.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMS.Data.Models
{
    public class File: DataRecord
    {
        public string Name {  get; set; } = string.Empty;
        public long Size {  get; set; }
        public FileCategory Category {  get; set; }
        public string Description {  get; set; } = string.Empty;
        public string Url {  get; set; }
        public FileStatus FileStatus {  get; set; }
        [ForeignKey(nameof(UploadedBy))]
        public int UploadedById { get; set; }
        public virtual SystemUser UploadedBy {  get; set; }
    }
}
