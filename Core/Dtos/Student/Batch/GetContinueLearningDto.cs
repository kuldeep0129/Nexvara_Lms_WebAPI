using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student.Batch
{
    public class GetContinueLearningDto
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
        public string BatchName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int ProgressClass { get; set; }
        public int TotalClass { get; set; }
        public List<GetContinueModuledto> ModuleList { get; set; } = new();
    }

    public class GetContinueModuledto
    {

        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string CurrentLeftCompleteDuration { get; set; }
        public string completePercentage { get; set; }
        public DateTime? LastWatchAt { get; set; }
    }
}
