using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class UpdateLiveClassesDto
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public int? ModuleId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledStartAt { get; set; }
        public DateTime? ScheduledEndAt { get; set; }
        public string? MeetingProvider { get; set; }
        public string? MeetingUrl { get; set; }
        public IFormFile? Recording { get; set; }
    }
}
