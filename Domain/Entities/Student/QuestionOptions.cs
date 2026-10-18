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
    public class QuestionOptions : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public AssessmentQuestions? AssessmentQuestions { get; set; }
        public string? OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
