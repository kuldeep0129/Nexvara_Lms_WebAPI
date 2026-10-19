using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class GetCourseBatchDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public int? CourseId { get; set; }
        public string? Name { get; set; }
        public string? BatchCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Capacity { get; set; }
        public bool IsActive {  get; set; }
    }
}
