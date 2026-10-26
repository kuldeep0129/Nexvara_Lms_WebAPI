using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace.GetUserInterface
{
    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public string? Point { get; set; }
        public string? Duration { get; set; }
        public string? Level { get; set; }
        public string? ImageUrl { get; set; }
        public string? Icon { get; set;  }
        public string? color { get; set;  }
        public string? SoftColor { get; set;  }
        public string? AccessType { get; set; }

        public decimal? Price { get; set; }
        public string? Demand { get; set;  }
        public string LastProjectIcon {  get; set; }
        public string LastProjectName{  get; set; }
        public string LastProjectDescriptions{  get; set; }
    }
}
