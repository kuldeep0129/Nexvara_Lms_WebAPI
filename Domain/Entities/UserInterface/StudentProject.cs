using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    public class StudentProject : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Example: React, ASP.NET Core, SQL Server
        [MaxLength(500)]
        public string? TechnologyTags { get; set; }

        [MaxLength(500)]
        public string? RepositoryUrl { get; set; }

        [MaxLength(500)]
        public string? DemoUrl { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

    }
}
