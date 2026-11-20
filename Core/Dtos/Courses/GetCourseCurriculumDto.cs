using System;
using System.Collections.Generic;

namespace Core.Dtos.Courses
{
    public class GetCourseCurriculumGroupDto
    {
        public int Id { get; set; }

        public string CourseName { get; set; }
        public string CourseShortDescription { get; set; }
        public string CourseDescription { get; set; }
        public string CoursePoint { get; set; }
        public string Technologies { get; set; }
        public string CousrseImage { get; set;  }

        public string CourseLevel { get; set; }
        public string? CourseDuration { get; set; }

        public int ActiveStudent { get; set; }
        public string? Rating { get; set; }

        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }

        //// Course Curriculum
        public List<GetCourseSyllabusCurriculumDto> Syllabus { get; set; }  = new();

        //public List<GetCourseModuleCurriculumDto> Modules { get; set; }  = new();

        //// Course Batches
        //public List<GetBatchCurriculumDto> Batches { get; set; } = new();
    }


    public class GetCourseSyllabusCurriculumDto
    {
        public int SyllabusId { get; set; }
        public string SyllabusTitle { get; set; }
        public string SyllabusDescription { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }


    public class GetCourseModuleCurriculumDto
    {
        public int ModuleId { get; set; }
        public string ModuleTitle { get; set; }
        public string ModuleDescription { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        // Module Content
        public List<GetCourseMaterialCurriculumDto> Materials { get; set; }  = new();

        public List<GetCourseVideoCurriculumDto> Videos { get; set; }  = new();
    }


    public class GetCourseMaterialCurriculumDto
    {
        public int MaterialId { get; set; }
        public string MaterialTitle { get; set; }

        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }

        public string UploadedById { get; set; }
        public string UploadedByName { get; set; }

        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
    }


    public class GetCourseVideoCurriculumDto
    {
        public int VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDescription { get; set; }

        public string VideoUrl { get; set; }
        public string VideoUrlPublicId { get; set; }

        public int DurationInSeconds { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
    }


    public class GetBatchCurriculumDto
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string BatchDescription { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Capacity { get; set; }
        public int EnrolledStudent { get; set; }

        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
    }
}