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
    public class AssessmentQuestions : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? AssessmentId { get; set; }
        [ForeignKey(nameof(AssessmentId))]
        public Assessments? assessments { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public decimal Marks { get; set; }
        public int DisplayOrder { get; set; }
    }
}
