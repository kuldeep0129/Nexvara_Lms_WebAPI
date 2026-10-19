using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class UpdateVideoProgressDto
    {
        public int Id { get; set; }
        public int? VideoId { get; set; }
        public string? StudentId { get; set; }
        public int? WatchedSeconds { get; set; }
        public decimal? CompletionPercentage { get; set; }
        public int? LastPositionSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastWatchedAt { get; set; }
    }
}
