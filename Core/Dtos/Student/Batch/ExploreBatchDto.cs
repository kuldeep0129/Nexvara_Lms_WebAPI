using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student.Batch
{
    public class ExploreBatchDto
    {
        public int BatchId { get; set; }

        public int CourseId { get; set; }

        public string BatchName { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public string Description { get; set; } = string.Empty;

        public string CourseTags { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }

        public string Duration { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string Mode { get; set; } = string.Empty;

        public int AvailableSeats { get; set; }

        public decimal? Price { get; set; }

        public string? Rating { get; set; }
        public bool IsEnrolled { get; set; }

        public bool IsActive { get; set; }

    }
}
