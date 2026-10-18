using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace.GetUserInterface
{
    public class ProgramResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? C { get; set; }
        public string? CS { get; set; }

        public string? Summary { get; set; }
        public string? Duration { get; set; }
        public string? Mode { get; set; }
        public string? Eligibility { get; set; }
        public string? Fee { get; set; }
        public string? Description { get; set; }

        public string Points { get; set; } 
    }
}
