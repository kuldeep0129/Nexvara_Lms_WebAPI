using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateStudentProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Example: React, ASP.NET Core, SQL Server
        [MaxLength(500)]
        public string? TechnologyTags { get; set; }

        [MaxLength(500)]
        [Url]
        public string? RepositoryUrl { get; set; }

        [MaxLength(500)]
        [Url]
        public string? DemoUrl { get; set; }

       
        public FormFile? ThumbnailUrl { get; set; }
    }
}
