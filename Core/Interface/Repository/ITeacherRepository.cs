using Core.Dtos.Common;
using Core.Dtos.Courses;
using Domain.Entities;
using Domain.Entities.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface ITeacherRepository
    {

        //Batch Teacher
        Task CreateBatchTeacherAsync(BatchTeacher entity);
        Task<BatchTeacher?> GetByIdBatchTeacherUpdateAsync(int Id);
        Task<GetTeacherBatchDto?> GetByIdBatchTeacherAsync(int Id);
        Task<PageResultDto<GetTeacherBatchDto>?> GetListBatchTeacherAsync(int pageNumber, int pageSize);
        Task<BatchTeacher?> UpdateBatchTeacherAsync(int Id);
    }
}
