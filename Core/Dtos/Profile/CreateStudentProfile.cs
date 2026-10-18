using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Profile
{
    public class CreateStudentProfile
    {
        [Required]
        public string Id { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public GenderEnum? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Qualification {  get; set; }
        public string? collegename {  get; set; }
        public string? Address {  get; set; }
        public int? Passingyear  {  get; set; }

    }
}
