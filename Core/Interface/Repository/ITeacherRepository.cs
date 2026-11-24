using Core.Dtos.Account;
using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Teacher.Attendance;
using Core.Dtos.Teacher.Batch;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface ITeacherRepository
    {
        #region Common
        //common
        Task<List<GetListTeacherIdNameDto>> GetListTeacherIdNameAsync();

        //Batch Teacher
        Task CreateBatchTeacherAsync(BatchTeacher entity);
        Task<BatchTeacher?> GetByIdBatchTeacherUpdateAsync(int Id);
        Task<GetTeacherBatchDto?> GetByIdBatchTeacherAsync(int Id);
        Task<PageResultDto<GetTeacherBatchDto>?> GetListBatchTeacherAsync(int pageNumber, int pageSize);
        Task<BatchTeacher?> UpdateBatchTeacherAsync(int Id);

        //Assign Batch Teacher
        Task<bool> BatchExistsAsync(int batchId);


        Task<bool> TeacherAlreadyAssignedAsync( int batchId, string teacherId);

        Task<bool> PrimaryTeacherExistsAsync(int batchId);

        Task CreateAsync(BatchTeacher batchTeacher);

        Task<GetBatchTeacherDto?> GetByIdAsync(int id);

        Task<List<GetBatchTeacherDto>> GetByBatchIdAsync( int batchId);
        Task<BatchTeacher?> GetByBatchAndTeacherIdAsync(int batchId, string teacherId);
        Task<bool> PrimaryTeacherExistsAsync( int batchId, string teacherId);
        Task<PageResultDto<GetAdminTeacherAssignDto>> GetListAdminTeacherAssignAsync(int pageNumber,int pageSize, AdminTeacherAssingStatus status );
        #endregion

        Task<PageResultDto<GetActiveTeacherBatchDto>> GetMyBatchesAsync( string teacherId,  PaginationRequestDto pagination, TeacherBatchFilterDto filter);
        Task<GetTeacherBatchOverviewDto> GetTeacherBatchOverViewAsync(int batchId ,string teacherId);

        //teacher Manual Attandnce for BatchSession Attendance
        Task<List<BatchClassSession>> GetSessionsByIdsAsync(List<int> sessionIds);

        Task<List<string>> GetEnrolledStudentIdsAsync(  int batchId, List<string> studentIds);

        Task<List<BatchSessionAttendance>> GetExistingAttendanceAsync( List<int> sessionIds, List<string> studentIds);

        Task AddAttendanceRangeAsync(List<BatchSessionAttendance> attendance);
        Task<bool> IsTeacherAssignedToBatchAsync( int batchId,  string teacherId);
        Task<GetStudentAttendanceSummeryDto> GetStudnetAttendanceSummeryAsync(int batchId);
        Task<GetListParticularStudentSummeryDto> GetParticularStudnetAttendanceSummeryAsync(int batchId,string studentId);

        Task<PageResultDto<GetBatchClassSessionDto>> GetTeacherBatchClassSessionsAsync( string teacherId,  int? batchId,  int pageNumber,  int pageSize);

    }
}
