using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateStudentAchievementDto
    {
        [Required]
        [MaxLength(150)]
        public string StudentDisplayName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string AchievementTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

    
        public IFormFile? ImageUrl { get; set; }
    }
}
