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
    public class BlogPostCategory : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BlogPostId { get; set; }

        [ForeignKey(nameof(BlogPostId))]
        public BlogPost BlogPost { get; set; } = null!;

        [Required]
        public int BlogCategoryId { get; set; }

        [ForeignKey(nameof(BlogCategoryId))]
        public BlogCategory BlogCategory { get; set; } = null!;
    }
}
