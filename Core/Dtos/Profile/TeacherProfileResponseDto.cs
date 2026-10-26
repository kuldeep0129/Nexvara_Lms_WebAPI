using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Profile
{
    public class TeacherProfileResponseDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? ProfileImagePublicId { get; set; }
        public string? Qualification { get; set; }
        public string ProfileUrl { get; set; }

        public decimal? ExperienceYears { get; set; }
        public string? PassingYear { get; set; }
        public string? Bio { get; set; }
        public string? Specialization { get; set; }
        public bool IsActive { get; set; }
    }
}
