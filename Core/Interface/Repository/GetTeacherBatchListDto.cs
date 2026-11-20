using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public class GetTeacherBatchListDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
        public string BatchName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public int EnrollStudentSheetCount { get; set; }
        public int TotalStudentSheetCount { get; set; }
        public int VideoProgress { get; set; }
        public DateTime NextClassDate { get; set; }
        public DateTime ClassTime { get; set; }
    }
}
