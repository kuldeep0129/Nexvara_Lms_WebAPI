using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    public class SiteContent : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string SectionKey { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Subtitle { get; set; }

        public string? Body { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(150)]
        public string? ButtonText { get; set; }

        [MaxLength(500)]
        public string? ButtonUrl { get; set; }

        public bool IsPublished { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }
}
