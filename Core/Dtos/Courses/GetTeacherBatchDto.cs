using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class GetTeacherBatchDto
    {
        public int Id { get; set; }

        public int? BatchId { get; set; }
        public string BatchName { get; set; }
        public string? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public bool IsPrimary { get; set; }
        public DateOnly AssignedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
