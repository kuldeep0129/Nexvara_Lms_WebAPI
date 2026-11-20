using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Teacher.Batch
{
    public class GetActiveTeacherBatchDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string BatchCode { get; set; } = string.Empty;

        public int StudentCount { get; set; }

        public int Capacity { get; set; }

        public decimal CourseProgress { get; set; }

        public decimal AttendancePercentage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public GetTeacherNextClassDto? NextClass { get; set; }

        public int PendingAssignmentCount { get; set; }

        public int PendingAssessmentCount { get; set; }

        public string Status { get; set; } = string.Empty;
    }
    public class GetTeacherNextClassDto
    {
        public int SessionId { get; set; }

        public DateTime SessionDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string ClassType { get; set; } = string.Empty;

        public string? MeetingLink { get; set; }

        public string? ModuleName { get; set; }
    }
    public class TeacherBatchFilterDto
    {
        public string? Status { get; set; }
    }
}
