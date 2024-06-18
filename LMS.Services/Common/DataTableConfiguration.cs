using LMS.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Common
{
    public class DataTableConfiguration
    {
        public int page {  get; set; }
        public int pageSize { get; set; }
        public string sortBy { get; set; } = "Id";
        public SortMode sortMode { get; set; } = SortMode.ASC;
        public string search { get; set; } = string.Empty;
    }
}
