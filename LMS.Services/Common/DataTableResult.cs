using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Common
{
    public class DataTableResult<T>
    {
        public int TotalRecords { get; set; }
        public List<T> Data { get; set; } = [];

    }
}
