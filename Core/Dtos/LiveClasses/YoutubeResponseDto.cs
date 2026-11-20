using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class YouTubeVideoResponse
    {
        public List<YouTubeVideoItem> Items { get; set; } = new();
    }

    public class YouTubeVideoItem
    {
        public YouTubeContentDetails ContentDetails { get; set; } = new();
    }

    public class YouTubeContentDetails
    {
        public string Duration { get; set; } = string.Empty;
    }
    public class CloudinaryUploadResult
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
        public double? Duration { get; set; }
    }
}
