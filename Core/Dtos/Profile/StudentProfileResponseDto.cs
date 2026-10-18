using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Profile
{
    public class StudentProfileResponseDto
    {
        public int Id { get; set; }

        public string? UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? ProfileImagePublicId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Qualification { get; set; }
        public string? CollegeName { get; set; }
        public int? PassingYear { get; set; }
        public string? Address { get; set; }
        public string IsActive { get; set; }
    }
}
