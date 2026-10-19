using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class UpdateCourseVideosDto
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? BatchId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? VideoUrl { get; set; }
        public string? DurationInSeconds { get; set; }
        public string? DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
    }
}
