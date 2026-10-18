using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace.GetUserInterface
{
    public class TestimonialResponseDto
    {
        public int Id { get; set; }
        public string StudentDisplayName { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public int? Rating { get; set; }
    }
}
