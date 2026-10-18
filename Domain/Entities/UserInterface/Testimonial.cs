using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    public class Testimonial : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser applicationUser { get; set; }


        [Required, MaxLength(2000)]
        public string ReviewText { get; set; } = string.Empty;

        [Range(1, 5)]
        public int? Rating { get; set; }
        public bool IsPublished { get; set; } = false;
    }
}
