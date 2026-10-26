using CloudinaryDotNet.Actions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class CreateCourses
    {

        [Required, MaxLength(180)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Summary { get; set; }

        public string? Points { get; set; }
        public string? Technologies { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Duration { get; set; }
        public string? Demand { get; set; }
        [Range(1,5)]
        public string? Star { get; set; }

        [MaxLength(50)]
        public string? Level { get; set; }

        public IFormFile? ImageUrl { get; set; }

        public bool IsPublished { get; set; } = false;
        public CourseAccessType? AccessType { get; set; }

        public decimal? Price { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;

        public string? Icon { get; set; }
        public string? ColorCode { get; set; }
        public string? SoftColorCode { get; set; }
    }
}
