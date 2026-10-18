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
    public class AssessmentAnswers : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? AttemptId { get; set; }
        [ForeignKey(nameof(AttemptId))]
        public AssessmentAttempts? attempts { get; set; }
        public int? QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public AssessmentQuestions? questions { get; set; }
        public int? SelectedOptionId { get; set; }
        [ForeignKey(nameof(SelectedOptionId))]
        public QuestionOptions? options { get; set; }
        public string? AnswerText { get; set; }
        public decimal? MarksObtained { get; set; }
    }
}
