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
    public class ClassAttendance : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? LiveClassId { get; set; }
        [ForeignKey(nameof(LiveClassId))]
        public LiveClasses? liveClasses { get; set; }

        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }

        public DateTime JoinedAt { get; set; }
        public DateTime LeftAt { get; set; }
        public DateTime DurationInSeconds { get; set; }
    }
}
