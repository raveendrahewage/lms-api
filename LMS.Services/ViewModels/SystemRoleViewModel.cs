using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class SystemRoleViewModel: DataRecordViewModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
