using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateAdmissionApplicationDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int ProgramId { get; set; }

        [MaxLength(150)]
        public string? Qualification { get; set; }

        [MaxLength(2000)]
        public string? Message { get; set; }
    }
}
