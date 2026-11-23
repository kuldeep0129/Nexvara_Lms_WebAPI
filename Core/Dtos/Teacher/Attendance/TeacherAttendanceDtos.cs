using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Teacher.Attendance
{
    public class AttendanceSessionDto
    {
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ModuleName { get; set; }
        public string? SessionStatus { get; set; }
        public int TotalStudents { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public bool AttendanceMarked { get; set; }
    }

    public class SessionStudentAttendanceDto
    {
        public int? AttendanceId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string Status { get; set; } = "NotMarked";
        public string? Remarks { get; set; }
        public DateTime? MarkedAt { get; set; }
    }

    public class MarkSessionAttendanceRequestDto
    {
        public List<MarkStudentAttendanceDto> Students { get; set; } = new();
    }

    public class MarkStudentAttendanceDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class UpdateStudentAttendanceDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class StudentAttendanceHistoryDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int TotalClasses { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public decimal AttendancePercentage { get; set; }
        public List<StudentAttendanceHistoryItemDto> Sessions { get; set; } = new();
    }

    public class StudentAttendanceHistoryItemDto
    {
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ModuleName { get; set; }
        public string Status { get; set; } = "Absent";
        public string? Remarks { get; set; }
        public DateTime? MarkedAt { get; set; }
    }

    public class BatchAttendanceSummaryDto
    {
        public int BatchId { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSessions { get; set; }
        public int TotalAttendanceEntries { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public decimal AverageAttendance { get; set; }
    }

    // Internal repository projection.
    public class AttendanceStudentLookupDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }
}
