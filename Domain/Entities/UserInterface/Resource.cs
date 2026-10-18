using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Resource : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(220)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string ResourceType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FileUrl { get; set; }

        public DateTime? PublishedAt { get; set; }

        public bool IsPublished { get; set; } = false;

    }
}
