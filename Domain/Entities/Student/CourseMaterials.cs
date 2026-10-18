using Domain.Entities.Common;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class CourseMaterials : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? course { get; set; }

        public int? ModuleId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public CourseModule? module { get; set; }
        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }
        public string Title { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public bool IsPublished { get; set; }
        public string UploadedBy { get; set; }

    }

}
