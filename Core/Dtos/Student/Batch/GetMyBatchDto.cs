using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student.Batch
{
    public class GetStudentEnrolledBatchDto
    {
        public int EnrollmentId { get; set; }
        public int BatchId { get; set; }

        public string? BatchCode { get; set; }
        public string? BatchName { get; set; }

        public int CourseId { get; set; }
        public string? CourseName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal ProgressPercentage { get; set; }

        public string? Status { get; set; }

        public string Action { get; set; } = "Enter";
    }

    public class StudentEnrolledBatchFilterDto
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


    public class GetStudentEnrolledBatchListDto
    {
        public int TotalBatches { get; set; }
        public int ActiveBatches { get; set; }
        public int CompletedBatches { get; set; }
        public int UpcomingBatches { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public List<GetStudentEnrolledBatchDto> Batches { get; set; }   = new();
    }
}
