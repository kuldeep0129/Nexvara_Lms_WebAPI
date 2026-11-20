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
    public class CourseVideos : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CourseId {  get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? course { get; set; }
        public int? ModuleId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public CourseSyllabus? module { get; set; }
        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? VideoId { get; set; }
        public string? VideoUrl { get; set; }
        public string VideoProvider { get; set; } = "YouTube";
        public string? VideoUrlPublicId { get; set; }
        public int? DurationInSeconds { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsPublished { get; set; }

    }
}
