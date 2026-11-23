using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student.Attendance
{
    public class GetStudentClassAttendanceDto
    {
        public int Id { get; set; }
        public int? LiveClassId { get; set; }
        public string? LiveClassTitle { get; set; }
        public string? BatchName { get; set; }
        public string? ModuleName { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public string? Status { get; set; }
        public int DurationInSeconds { get; set; }
    }
}
