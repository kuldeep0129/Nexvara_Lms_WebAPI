using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class CreateCertificateDto
    {
        [Required]
        [MaxLength(150)]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string ProgramName { get; set; } = string.Empty;

        [Required]
        public DateTime IssueDate { get; set; }
    }
}
