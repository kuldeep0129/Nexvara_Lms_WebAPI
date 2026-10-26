using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class UpdateBatchStatusDto
    {
        [Required]
        public BatchStatus Status { get; set; }
    }
}
