using Core.Dtos.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Teacher.Batch
{
    public class GetTeacherBatchOverviewDto
    {
        public TeacherBatchSummaryDto Summary { get; set; } = new();

        public TeacherBatchBasicInfoDto BasicInformation { get; set; } = new();
        public List<GetCourseSyllabusDto?> SyllabusInformation { get; set; } = new();

        public TeacherNextLiveClassDto? NextLiveClass { get; set; }

        public List<TeacherStudentAttendanceDto> Attendance { get; set; } = new();

        public List<TeacherBatchMaterialDto> Materials { get; set; } = new();
    }
    public class TeacherBatchSummaryDto
    {
        public int TotalStudents { get; set; }

        public decimal? AttendancePercentage { get; set; }

        public decimal CourseProgress { get; set; }

        public DateTime? NextClassDate { get; set; }
    }
    public class TeacherBatchBasicInfoDto
    {
        public int BatchId { get; set; }

        public string BatchName { get; set; } = string.Empty;

        public string BatchCode { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string BatchStatus { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }

    public class TeacherNextLiveClassDto
    {
        public int SessionId { get; set; }

        public string Topic { get; set; } = string.Empty;

        public string? ModuleName { get; set; }

        public DateTime SessionDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string ClassType { get; set; } = string.Empty;

        public string? MeetingLink { get; set; }
    }

    public class TeacherStudentAttendanceDto
    {
        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public int TotalClasses { get; set; }

        public int Present { get; set; }

        public int Absent { get; set; }

        public int Late { get; set; }

        public decimal AttendancePercentage { get; set; }

        public List<TeacherStudentAttendanceSessionDto> Sessions { get; set; } = new();
    }
    public class TeacherStudentAttendanceSessionDto
    {
        public int SessionId { get; set; }

        public DateTime ScheduleDate { get; set; }

        public string AttendanceStatus { get; set; } = string.Empty;

        public int TotalMinutes { get; set; }

        public DateTime? FirstJoinTime { get; set; }

        public DateTime? LastLeaveTime { get; set; }
    }
    public class TeacherBatchMaterialDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ModuleName { get; set; }

        public long FileSize { get; set; }

        public string FileType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public string FileUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
