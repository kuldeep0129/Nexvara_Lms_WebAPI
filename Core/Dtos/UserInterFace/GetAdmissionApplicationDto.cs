using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace
{
    public class GetAdmissionApplicationDto
    {
        public int Id { get; set; }

        public string ApplicationNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int ProgramId { get; set; }

        public string ProgramName { get; set; } = string.Empty;

        public string? Qualification { get; set; }

        public string? Message { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
