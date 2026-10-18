using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateOrganizationDto
    {

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? WhatsApp { get; set; }

        [MaxLength(500)]
        public string? LinkedInURL { get; set; }

        [MaxLength(500)]
        public string? FacebookURL { get; set; }

        [MaxLength(500)]
        public string? YouTubeURL { get; set; }
        public string? InstagramURL { get; set; }

        [MaxLength(500)]
        public string? TwitterURL { get; set; }

        [MaxLength(200)]
        public string? Timing { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? PinCode { get; set; }
        public IFormFile? Sortlogo { get; set; }
        public IFormFile? Fulllogo { get; set; }
        public string? MapLocation { get; set; }

    }
}
