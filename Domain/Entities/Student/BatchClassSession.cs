using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class BatchClassSession : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }

        public int? BatchScheduleId { get; set; }
        [ForeignKey(nameof(BatchScheduleId))]
        public BatchSchedule? BatchSchedule { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public ApplicationUser? Teacher { get; set; }
        public int? CourseModuleId { get; set; }
        [ForeignKey(nameof(CourseModuleId))]
        public CourseSyllabus? courseModule { get; set; }
        public string? Status { get; set; }
        public bool IsReschedule { get; set; }
        public DateTime? OriginalSessionDate { get; set; }
        public TimeSpan? OriginalStartTime { get; set; }
        public TimeSpan? OriginalEndTime { get; set; }
        public string? RescheduleReason { get; set; }

    }
}
