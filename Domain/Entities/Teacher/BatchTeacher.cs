using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Teacher
{
    public class BatchTeacher : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        public int? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }

        public string? TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public ApplicationUser? user { get; set; }

        public bool IsPrimary { get; set; }
        public DateTime? AssignedAt {  get; set; }
        public string? AssignedBy {  get; set; }
        public DateTime? RemovedAt {  get; set; }
        public string? RemovedBy {  get; set; }


    }
}
