using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class StartVideoSessionDto
    {
        public int VideoId { get; set; }

    }
    public class SaveVideoProgressDto
    {
        public Guid EventId { get; set; }

        public Guid SessionId { get; set; }

        public int VideoId { get; set; }

        public int PositionSeconds { get; set; }

        // Checkpoint, Paused, PageHidden, Ended
        public string EventType { get; set; } = "Checkpoint";

        public bool IsPlaying { get; set; }

        public DateTime OccurredAt { get; set; }

        public string? StopReason { get; set; }
    }
    public class VideoProgressResponseDto
    {
        public int VideoId { get; set; }

        public int LastPositionSeconds { get; set; }

        public int WatchedSeconds { get; set; }

        public decimal CompletionPercentage { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? LastWatchedAt { get; set; }
    }
}
