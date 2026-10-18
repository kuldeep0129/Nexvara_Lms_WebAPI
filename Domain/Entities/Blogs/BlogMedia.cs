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
    public class BlogMedia : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BlogPostId { get; set; }

        [ForeignKey(nameof(BlogPostId))]
        public BlogPost BlogPost { get; set; } = null!;

        [Required, MaxLength(500)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AltText { get; set; }
    }
}
