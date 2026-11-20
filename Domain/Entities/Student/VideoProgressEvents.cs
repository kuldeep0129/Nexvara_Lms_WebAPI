using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class VideoProgressEvents
    {
        [Key]
        public int Id { get; set; }
        public Guid? EventId { get; set; }
        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? User { get; set; }
        public int? VideoId { get; set; }
        [ForeignKey(nameof(VideoId))]
        public CourseVideos? video { get; set; }
        public Guid? SessionId { get; set; }
        public string? EventType { get; set; }
        public int? PositionSeconds { get; set; }
        public int? WatchedSecondsDelta { get; set; }
        public string? NetworkStatus { get; set; }
        public DateTime? OccurredAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string? StopReason { get; set; }
    }
}
