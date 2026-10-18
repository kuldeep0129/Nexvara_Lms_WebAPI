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
    public class GuidanceCategories : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public GuidanceCategories? categories { get; set; }
        public int? AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        public ApplicationUser? uesr { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public DateTime PublishedAt { get; set; }

    }
}
