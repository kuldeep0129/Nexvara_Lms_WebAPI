using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateProgramDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(180)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Summary { get; set; }
        [Required]
        public int DurationValue { get; set; }

        [MaxLength(100)]
        [Required]
        public string? DurationUnit { get; set; }

        public string? Point { get; set; }

        [Required]
        public int ProgramTypeId { get; set; }


        [MaxLength(50)]
        public string? Mode { get; set; }

        [MaxLength(500)]
        public string? Eligibility { get; set; }

        [Range(0, 9999999999.99)]
        public decimal? Fee { get; set; }

        public string? WithoutFee { get; set; }

        public string? Description { get; set; }
        public CreateProgramSettingDto  setting {get;  set;} = new();
    }
    public class CreateProgramSettingDto
    {
        public string? Icon { get; set; }
        public string? ColorCode { get; set; }
        public string? SoftColorCode { get; set; }

    }
}
