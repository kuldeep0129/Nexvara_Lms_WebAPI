using Core.Dtos.Batch;
using Core.Dtos.Common;
using Domain.Entities;
using Domain.Entities.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface IBatchRepository
    {
        Task CreateBatchAsync(Batch entity);
        Task<bool> CourseExistsAsync(int courseId);

        Task<bool> BatchNameExistsAsync( int courseId, string name);

        Task<bool> BatchCodeExistsAsync( string code);
        Task<PageResultDto<GetBatchListDto>> GetBatchListAsync( PaginationRequestDto pagination, BatchFilterDto filter);

        Task<GetBatchDetailsDto?> GetBatchByIdAsync(int id);

        Task<Batch?> GetBatchEntityByIdAsync(int id);

        Task<bool> UpdateBatchAsync(Batch batch);

        //Batch Sechdule
        Task<bool> BatchExistsAsync(int batchId);

        Task<bool> ScheduleExistsAsync( int batchId,  DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);

        Task CreateScheduleAsync(BatchSchedule schedule);
        Task<BatchSchedule?> GetByIdBatchScheduleAsync(int scheduleId);
        Task<BatchSchedule?> GetByIdBatchScheduleAsync(int scheduleId,int batchId);
        Task<List<GetBatchScheduleDto>> GetListBatchSchedulesAsync(int batchId);
        Task<bool> ScheduleExistsAsync(int batchId, DayOfWeek dayOfWeek,TimeSpan startTime, TimeSpan endTime, int scheduleId);
        Task<PageResultDto<GetBatchScheduleDto>> GetBatchScheduleListAsync( PaginationRequestDto pagination,BatchScheduleFilterDto filter);
    }
}
