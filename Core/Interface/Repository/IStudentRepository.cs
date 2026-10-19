using Core.Dtos.Common;
using Core.Dtos.Courses;
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
    }
}
