using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class GetStudentCourseVideoDto
    {
        public int CurrentVideoId { get; set; }
        public int NextVideoId { get; set; }
        public List<GetClassRoomCourseVideoDto> VideoDetails { get; set; }
    }
    public class GetClassRoomCourseVideoDto
    {
        public int VideoId { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDescription { get; set; }
        public int Sequence { get; set; }
        public string? Videourl { get; set; }
        public int durationSeconds { get; set; }
        public int? watchedSeconds { get; set; }
        public decimal? progressPercentage { get; set; }
    }
}
