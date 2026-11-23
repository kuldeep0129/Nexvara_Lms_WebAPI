using Domain.Entities.Common;
using Domain.Entities.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Teacher
{
    public class BatchSessionAttendance :  BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        public int? BatchClassSessionId { get; set; }

        [ForeignKey(nameof(BatchClassSessionId))]
        public BatchClassSession? BatchClassSession { get; set; }

        public string StudentId { get; set; } = string.Empty;

        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? Student { get; set; }

        // Present, Absent, Late, Leave
        public string Status { get; set; } = "Absent";

        public string? Remarks { get; set; }

        public DateTime MarkedAt { get; set; }

        public string? MarkedBy { get; set; }
    }
}
