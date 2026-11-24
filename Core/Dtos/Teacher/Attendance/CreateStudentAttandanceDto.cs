using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Teacher.Attendance
{
    public class CreateStudentAttendanceDto
    {
        public int BatchId { get; set; }
        public List<CreateStudentAttendanceItemDto> AttendanceRecords { get; set; } = new();
    }

    public class CreateStudentAttendanceItemDto
    {
        public string StudentId { get; set; } = string.Empty;

        public int BatchClassSessionId { get; set; }

        public AttendanceStatus Status { get; set; }

        public string? Remarks { get; set; }
    }

    public class GetStudentAttendanceSummeryDto
    {
        public int BatchId { get; set; }
        public int TotalStudent { get; set; }
        public Decimal BatchAverageAttendance { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public List<GetListStudnetAttendanceDto> ParticularAttendnace { get; set; } = new();
    }
    public class GetListStudnetAttendanceDto
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string? StudnetProfileImage { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public decimal AverageAttendance { get; set; }
    }


    public class GetListParticularStudentSummeryDto
    {
        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string? StudentProfilePic { get; set; }

        public int TotalClasses { get; set; }

        public int Present { get; set; }

        public int Absent { get; set; }

        public int NotMarked { get; set; }

        public decimal AverageAttendance { get; set; }

        public List<GetParticularStudentAttendanceHistoryDto> Summary { get; set; }
            = new();
    }

    public class GetParticularStudentAttendanceHistoryDto
    {
        public int AttendanceId { get; set; }

        public int SessionId { get; set; }

        public DateTime SessionDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string Status { get; set; } = "Not Marked";

        public string? Remarks { get; set; }

        public DateTime? MarkedAt { get; set; }
    }

}
