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
    public class VideoProgress : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? VideoId { get; set; }
        [ForeignKey(nameof(VideoId))]
        public CourseVideos? courseVideos { get; set; }

        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }
        public int? WatchedSeconds { get; set; }
        public decimal? CompletionPercentage { get; set; }
        public int? LastPositionSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastWatchedAt { get; set; }


    }
}
