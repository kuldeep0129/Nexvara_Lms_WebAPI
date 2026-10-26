using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IBatchServices
    {
        Task<ResponseDto<object>> CreateBatchAsync(CreateBatchDto dto, string userId);
        Task<ResponseDto<object>> GetBatchListAsync( PaginationRequestDto pagination,  BatchFilterDto filter);

        Task<ResponseDto<object>> GetBatchByIdAsync(int id);
        Task<ResponseDto<object>> UpdateBatchAsync( int id,  UpdateBatchDto dto,  string userId);
        Task<ResponseDto<object>> UpdateBatchStatusAsync( int id,UpdateBatchStatusDto dto, string userId);
        Task<ResponseDto<object>> UpdateBatchPublishAsync( int id, UpdateBatchPublishDto dto, string userId);
        Task<ResponseDto<object>> ArchiveBatchAsync(int id, string userId);

        //Batch Sechdule
        Task<ResponseDto<object>> CreateScheduleAsync( int batchId,  CreateBatchScheduleDto dto, string userId);
        Task<ResponseDto<object>> GetByIdScheduleAsync( int scheduleId);
        Task<ResponseDto<object>> GetListBatchSchedulesAsync(int batchId);
        Task<ResponseDto<object>> UpdateBatchSchedulesStatusAsync(int scheduleId,int batchId,string userId);
        Task<ResponseDto<object>> UpdateScheduleAsync( int batchId, int scheduleId, UpdateBatchScheduleDto dto, string userId);
        Task<ResponseDto<object>> GetBatchScheduleListAsync(PaginationRequestDto pagination,BatchScheduleFilterDto filter);
    }
}
