using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class InterviewQuestions : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public InterviewCategories? Category { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? Explanation { get; set; }
        public int? Difficulty { get; set; }


    }
}
