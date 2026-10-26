using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class BatchFilterDto
    {
        public string? Search { get; set; }

        public int? CourseId { get; set; }

        public BatchStatus? Status { get; set; }

        public bool? IsPublished { get; set; }

        public bool? IsActive { get; set; }

        public string? SortBy { get; set; }

        public bool SortDescending { get; set; } = true;
    }
}
