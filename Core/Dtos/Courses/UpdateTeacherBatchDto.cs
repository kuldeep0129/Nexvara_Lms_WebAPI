using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class UpdateTeacherBatchDto
    {
        public int Id { get; set; }

        public int? BatchId { get; set; }
        public string? TeacherId { get; set; }
        public bool IsPrimary { get; set; }
        public DateOnly AssignedAt { get; set; }
    }
}
