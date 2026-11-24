using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Teacher.Attendance;
using Core.Dtos.Teacher.Batch;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface ITeacherServices
    {
        //Common
        Task<ResponseDto<object>> GetListTeacherIdNameAsync();


        //BatchTeacher
        Task<ResponseDto<object>> CreateBatchTeacherAsync(CreateTeacherBatchDto dto, string userId);
        Task<ResponseDto<object>> UpdateBatchTeacherAsync(UpdateTeacherBatchDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusBatchTeacherAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdBatchTeacherAsync(int Id);
        Task<ResponseDto<object>> GetListBatchTeacherAsync(PaginationRequestDto dto);


        //BatchTeacherManagement
        Task<ResponseDto<object>> AssignTeacherAsync( int batchId,   CreateBatchTeacherDto dto,  string userId);
        Task<ResponseDto<object>> GetBatchTeachersAsync(  int batchId);
        Task<ResponseDto<object>> UpdateTeacherAssignmentAsync(int batchId, string teacherId, UpdateBatchTeacherDto dto, string userId);
        Task<ResponseDto<object>> UpdateTeacherAssigmentStatusAsync( int batchId, string teacherId, string userId);
        Task<ResponseDto<object>> UpdateTeacherAssigmentPrimaryAsync( int batchId, string teacherId,UpdateBatchTeacherAssignStatusDto dto, string userId);
        Task<ResponseDto<object>> GetListAdminTeacherAssignAsync(AdminTeacherRequestVm vm);

        Task<ResponseDto<object>> GetMyBatchesAsync( PaginationRequestDto pagination, TeacherBatchFilterDto filter);

        Task<ResponseDto<object>> GetTeacherBatchOverViewAsync( int batchId);

        //For teacher Manual Attendance 
        Task<ResponseDto<object>> CreateStudentAttendanceAsync(CreateStudentAttendanceDto dto);
        Task<ResponseDto<object>> GetStudnetAttendanceSummeryAsync(int batchId);
        Task<ResponseDto<object>> GetParticularStudnetAttendanceSummeryAsync(int batchId, string studentId);
        Task<ResponseDto<PageResultDto<GetBatchClassSessionDto>>> GetTeacherBatchClassSessionsAsync( int? batchId,int pageNumber, int pageSize);
    }
}
