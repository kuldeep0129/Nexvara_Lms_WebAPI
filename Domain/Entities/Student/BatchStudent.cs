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
    public class BatchStudent : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }

        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }
        public DateTime? EnrolledAt { get; set; }
        public DateTime? CompleteAt { get; set; }
    }
}
