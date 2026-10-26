using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class CreateBatchTeacherDto
    {
        [Required]
        public string TeacherId { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;
    }
}
