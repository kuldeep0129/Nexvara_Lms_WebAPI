using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateCourseDto
    {
        [Required]
        [MaxLength(180)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Summary { get; set; }
        public string? Points { get; set; }
        public string? Technologies { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Duration { get; set; }

        [MaxLength(50)]
        public string? Level { get; set; }
        public string? Demand { get; set; }
        public string? Star { get; set; }

        public IFormFile? ImageUrl { get; set; }
        public CreateCourseSettings setting { get; set; }
        public CreateCourseLastProject lastProject { get; set; }
    }
    public class CreateCourseSettings
    {
        public string? Icon { get; set; }
        public string? ColorCode { get; set; }
        public string? SoftColorCode { get; set; }
    }
    public class CreateCourseLastProject
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? Icon { get; set; }
    }

}
