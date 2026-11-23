using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student
{

    public class StudentAssessmentListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int? CourseId { get; set; }
        public string? CourseName { get; set; }

        public int? BatchId { get; set; }
        public string? BatchName { get; set; }

        public decimal TotalMarks { get; set; }
        public decimal PassingMarks { get; set; }
        public decimal DurationInMinutes { get; set; }

        public int TotalQuestions { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public string Status { get; set; } = string.Empty;
        public bool HasAttempt { get; set; }
        public int? AttemptId { get; set; }
        public bool IsSubmitted { get; set; }
    }
    public class StudentAssessmentDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal TotalMarks { get; set; }
        public decimal PassingMarks { get; set; }
        public decimal DurationInMinutes { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public int TotalQuestions { get; set; }

        public List<StudentAssessmentQuestionDto> Questions { get; set; } = new();
    }

    public class StudentAssessmentQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int QuestionType { get; set; }
        public decimal Marks { get; set; }
        public int DisplayOrder { get; set; }

        public List<StudentQuestionOptionDto> Options { get; set; } = new();
    }

    public class StudentQuestionOptionDto
    {
        public int Id { get; set; }
        public string? OptionText { get; set; }
    }

    public class SubmitAssessmentDto
    {
        public List<SubmitAssessmentAnswerDto> Answers { get; set; } = new();
    }

    public class SubmitAssessmentAnswerDto
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
    }
    public class StudentAssessmentResultDto
    {
        public int AttemptId { get; set; }
        public int AssessmentId { get; set; }

        public string AssessmentTitle { get; set; } = string.Empty;

        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }

        public decimal PassingMarks { get; set; }
        public bool IsPassed { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
    public class StartAssessmentDto
    {
        public int AttemptId { get; set; }
        public int AssessmentId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
