using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Dtos.Batch
{
    public class GetBatchScheduleDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CourseId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CourseName { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string ClassType { get; set; }

        public string? MeetingLink { get; set; }
        public bool IsActive { get; set; }
    }
}
