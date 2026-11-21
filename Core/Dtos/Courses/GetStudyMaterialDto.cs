using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class GetStudyMaterialdashboardDto
    {
        public int Total { get; set; }
        public List<GetStudyMaterialDto> material { get; set; }
    }
    public class GetStudyMaterialDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int BatchId { get;set; }
        public string BatchName { get; set; }   
        public int ModuleId { get;set; }
        public string ModuleName { get; set; }   
        public string Title { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public bool IsActive { get; set; }  

    }
}
