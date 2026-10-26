using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class BatchScheduleFilterDto
    {

        public string? Search { get; set; }

        public int? BatchId { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public bool? IsActive { get; set; }

        public string? SortBy { get; set; }

        public bool SortDescending { get; set; } = true;
    }
}
