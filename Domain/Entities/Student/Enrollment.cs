using Domain.Entities.Common;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class Enrollment : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }
        public string? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? course { get; set; }
        public string? BatchId { get; set; }
        [ForeignKey(nameof(BatchId))]
        public Batch? batch { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public decimal? CompletionPercentage { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
