using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class GetCoursesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Slug { get; set; } 
        public string? Summary { get; set; }
        public string? Points { get; set; }
        public string? Technologies { get; set; }
        public string? Description { get; set; }
        public string? Duration { get; set; }
        public string? Demand { get; set; }
        public string? Star { get; set; }
        public string? Level { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; } 
        public int SortOrder { get; set; } 
        public bool IsFeatured { get; set; } 
        public bool IsActive { get; set; }
    }
}
