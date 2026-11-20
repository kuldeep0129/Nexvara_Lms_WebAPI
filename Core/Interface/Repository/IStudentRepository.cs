using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Student.Batch;
using Core.Dtos.Student.Sechdule;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface IStudentRepository
    {
        //Batch Student
        Task CreateBatchStudentAsync(BatchStudent entity);
        Task<BatchStudent?> GetByIdBatchStudentUpdateAsync(int Id);
        Task<GetBatchStudentDto?> GetByIdBatchStudentAsync(int Id);
        Task<PageResultDto<GetBatchStudentDto>?> GetListBatchStudentAsync(int pageNumber, int pageSize);
        Task<BatchStudent?> UpdateBatchStudentAsync(int Id);

        //New Batch Code 
        Task<ExploreCourseResponseDto<ExploreBatchDto>> GetExploreCoursesAsync(ExploreCourseRequestDto request);

        //Enrollment
        Task CreateStudentEnrollmentAsync(Enrollment entity);
        Task<bool> IsCheckAlreadyStudentEnrollAsync(string userId, int courseId, int batchId);
        Task<bool> IsCheckBatchFreeOrPaidAsync(int courseId,int batchId);
        Task CreatePaymentAsync(Payments entity);
        Task<Batch?> GetBatchForEnrollmentAsync(int batchId);

        //Schedule 
        Task<List<GetStudentSechduleDto>> GetStudentSechduleAsync(DateTime? dateTime, string studentId);

        //My Batch
        Task<int> GetStudentBatchCountAsync( string studentId,string? status);

        Task<List<GetStudentEnrolledBatchDto>> GetStudentEnrolledBatchesAsync(string studentId, StudentEnrolledBatchFilterDto filter);
        Task<List<GetContinueLearningDto>> GetContinueLearningAsync(string studentId);

    }
}
