using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class GetAdminTeacherAssignDto
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string BatchCode { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string WeekOfDay { get; set; }
    
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}
