using Microsoft.AspNetCore.Http;

namespace Core.Dtos.LiveClasses
{
    public class UpdateCourseVideosDto
    {
        public int Id { get; set; }

        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? BatchId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Cloudinary video
        public IFormFile? VideoUrl { get; set; }

        // YouTube video
        public string? VideoYoutubeUrl { get; set; }

        public int? DurationInSeconds { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }
    }
}