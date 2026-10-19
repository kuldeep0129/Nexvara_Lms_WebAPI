using Core.Dtos.Common;
using Core.Dtos.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface ITeacherServices
    {
        //BatchTeacher
        Task<ResponseDto<object>> CreateBatchTeacherAsync(CreateTeacherBatchDto dto, string userId);
        Task<ResponseDto<object>> UpdateBatchTeacherAsync(UpdateTeacherBatchDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusBatchTeacherAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdBatchTeacherAsync(int Id);
        Task<ResponseDto<object>> GetListBatchTeacherAsync(PaginationRequestDto dto);
    }
}
