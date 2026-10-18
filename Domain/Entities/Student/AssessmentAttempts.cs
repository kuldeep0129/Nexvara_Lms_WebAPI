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
    public class AssessmentAttempts : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? AssessmentId { get; set; }
        [ForeignKey(nameof(AssessmentId))]
        public Assessments? assessmentId { get; set; }

        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime SubmittedAt { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public bool IsPassed { get; set; }
    }
}
