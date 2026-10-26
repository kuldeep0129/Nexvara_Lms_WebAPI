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
    public class BatchSchedule : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }
        public string? DayOfWeek { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ClassType { get; set; }
        public string? MettingLink { get; set; }
    }
}
