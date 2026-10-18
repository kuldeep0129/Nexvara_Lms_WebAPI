using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Account
{
    public class RegistrationDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public RoleStatus Role { get; set; }
    }
}
