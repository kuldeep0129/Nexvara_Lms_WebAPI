using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class VideoWatchSession
    {
        [Key]
        public int Id { get; set; }
        public Guid? SessionId { get; set; }
        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? User { get; set; }
        public int? VideoId { get; set; }
        [ForeignKey(nameof(VideoId))]
        public CourseVideos? video { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? LastHeartbeatAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? LastPositionSeconds { get; set; }
        public bool IsPlaying { get; set; }
        public string Status { get; set; } = "Active";
        public string? StopReason { get; set; }

    }
}
