using Core.Dtos.Common;
using Core.Dtos.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IStudentServices
    {
        //BatchStudent
        Task<ResponseDto<object>> CreateBatchStudentAsync(CreateBatchStudentDto dto, string userId);
        Task<ResponseDto<object>> UpdateBatchStudentAsync(UpdateBatchStudentDto dto, string userId);
        Task<ResponseDto<object>> UpdateStatusBatchStudentAsync(int Id, string userId);
        Task<ResponseDto<object>> GetByIdBatchStudentAsync(int Id);
        Task<ResponseDto<object>> GetListBatchStudentAsync(PaginationRequestDto dto);
    }
}
