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
    public class CreateTeacherProfile
    {
        [Required]
        public string Id { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Qualification { get; set; }
        public decimal? Experienceyear { get; set; }
        public string? bio { get; set; }
        public string? specialization { get; set; }
        public string? PassingYear { get; set; }
        public int? Passingyear { get; set; }
    }
}
