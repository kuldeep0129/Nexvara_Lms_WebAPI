using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateSiteContentDto
    {
        [Required]
        [MaxLength(200)]
        public string SectionKey { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Subtitle { get; set; }

        public string? Body { get; set; }

  
        public FormFile? ImageUrl { get; set; }

        [MaxLength(150)]
        public string? ButtonText { get; set; }

        [MaxLength(500)]
        [Url]
        public string? ButtonUrl { get; set; }
    }
}
