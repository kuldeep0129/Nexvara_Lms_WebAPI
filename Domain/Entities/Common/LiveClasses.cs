using Domain.Entities.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class LiveClasses : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }

        public int? ModuleId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public CourseModule? module { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledStartAt { get; set; }
        public DateTime? ScheduledEndAt { get; set; }
        public string? MeetingProvider { get; set; }
        public string? MeetingUrl { get; set; }
        public string? RecordingUrl { get; set; }
        public string? RecordingUrlPublicId { get; set; }
    }
}
