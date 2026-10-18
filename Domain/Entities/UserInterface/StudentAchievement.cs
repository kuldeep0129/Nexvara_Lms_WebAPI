using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    public class StudentAchievement : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string StudentDisplayName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string AchievementTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
    }
}
