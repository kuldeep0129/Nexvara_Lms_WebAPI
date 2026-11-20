using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class GetBatchScheduleGroupDto
    {
        public int BatchId { get; set; }
        public string? BatchName { get; set; }

        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int? ScheduleId { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public bool IsActive { get; set; }
        public int ActiveStudent { get; set; }
        public int TotalStudent { get; set; }


        public List<GetBatchScheduleDto> Schedules { get; set; } = new();
    }
}
