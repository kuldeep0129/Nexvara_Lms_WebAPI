using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Teacher.Attendance
{
    public class GetBatchClassSessionDto
    {
        public int BatchClassSessionId { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public int BatchScheduleId { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int? CourseModuleId { get; set; }
        public string? Status { get; set; }
        public bool IsReschedule { get; set; }
        public DateTime? OriginalSessionDate { get; set; }
        public TimeSpan? OriginalStartTime { get; set; }
        public TimeSpan? OriginalEndTime { get; set; }
        public string? RescheduleReason { get; set; }
        public bool IsActive { get; set; }
    }
}
