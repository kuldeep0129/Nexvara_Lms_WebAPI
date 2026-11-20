using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student.Batch
{
    public class ExploreCourseRequestDto
    {
        public string? Search { get; set; }

        public string? PriceType { get; set; }
        // All / Free / Paid

        public string? EnrollmentStatus { get; set; }

        public string? Level { get; set; }

        public string? Mode { get; set; }

        public string? SortBy { get; set; }
        // Popular / Newest / PriceLowToHigh / PriceHighToLow

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 12;
    }
}
