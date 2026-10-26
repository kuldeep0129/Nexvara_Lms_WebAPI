using Core.Dtos.Account;
using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Domain.Entities;
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



    }
}
