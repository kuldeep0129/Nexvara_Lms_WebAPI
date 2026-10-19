using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class GetLiveClassDto
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public string BatchName { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledStartAt { get; set; }
        public DateTime? ScheduledEndAt { get; set; }
        public string? MeetingProvider { get; set; }
        public string? MeetingUrl { get; set; }
        public string? RecordingUrl { get; set; }
        public string? RecordingUrlPublicId { get; set; }
        public bool IsActive { get; set; }
    }
}
