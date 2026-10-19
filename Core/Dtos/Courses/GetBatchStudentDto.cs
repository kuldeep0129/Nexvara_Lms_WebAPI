using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class GetBatchStudentDto
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public string BatchName { get; set; }
        public string? StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime? EnrolledAt { get; set; }
        public DateTime? CompleteAt { get; set; }
        public bool IsActive { get; set; }
    }
}
