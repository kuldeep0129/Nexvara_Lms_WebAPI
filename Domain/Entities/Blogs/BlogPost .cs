using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Blogs
{
    public class BlogPost : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Excerpt { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? AuthorUserId { get; set; }

        [ForeignKey(nameof(AuthorUserId))]
        public ApplicationUser? Author { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Draft";

        public DateTime? PublishedAt { get; set; }

        public DateTime? ScheduledAt { get; set; }

        public int ReadingTimeMinutes { get; set; }

        [MaxLength(250)]
        public string? MetaTitle { get; set; }

        [MaxLength(500)]
        public string? MetaDescription { get; set; }
    }
}
