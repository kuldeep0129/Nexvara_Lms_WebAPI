using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class GetBatchTeacherDto
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public string TeacherId { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public string? TeacherEmail { get; set; }
        public string? ProfileImageUrl { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsActive { get; set; }

        public DateTime? AssignedAt { get; set; }
    }
}
