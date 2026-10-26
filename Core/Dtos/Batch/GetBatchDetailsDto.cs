using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class GetBatchDetailsDto
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Capacity { get; set; }

        public int TotalStudents { get; set; }

        public int TotalTeachers { get; set; }

        public string Status { get; set; }

        public bool IsPublished { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateAt { get; set; }

        public string? CreateBy { get; set; }

        public DateTime? UpdateAt { get; set; }

        public string? UpdateBy { get; set; }
    }
}
