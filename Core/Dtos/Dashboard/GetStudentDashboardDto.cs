using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Dashboard
{
    public class GetStudentDashboardDto
    {
        public string StudentName { get; set; } = string.Empty;

        public StudentDashboardStatsDto Stats { get; set; } = new();

        public List<StudentDashboardCourseDto> Courses { get; set; } = new();

        public List<StudentDashboardRecentGradeDto> RecentGrades { get; set; } = new();
    }

    public class StudentDashboardStatsDto
    {
        public int EnrolledCourses { get; set; }
        public int LiveClasses { get; set; }
        public int Certificates { get; set; }
        public int AssignmentsDue { get; set; }
    }

    public class StudentDashboardCourseDto
    {
        public int BatchId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseImage { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public string BatchCode { get; set; } = string.Empty;
        public decimal Progress { get; set; }
        public DateTime? NextClassDate { get; set; }
        public TimeSpan? NextClassTime { get; set; }
    }

    public class StudentDashboardRecentGradeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal Total { get; set; }
        public decimal Percentage { get; set; }
        public bool IsPassed { get; set; }
    }
}
