using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.UserInterFace.GetUserInterface
{
    public class AnnouncementResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? SortDescription { get; set; }
        public string? Description { get; set; }
        public string AnouncementType { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public DateTime PublishAt { get; set; }
        public DateTime ExpireAt { get; set; }
        public bool IsPinned { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}
