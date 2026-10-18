using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateTestimonialDto
    {
        [Required]
        [MaxLength(2000)]
        public string ReviewText { get; set; } = string.Empty;

        [Range(1, 5)]
        public int? Rating { get; set; }

    }
}
