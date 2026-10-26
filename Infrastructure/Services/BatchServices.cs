using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BatchServices : IBatchServices
    {
        private readonly IBatchRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BatchServices(IBatchRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<object>> ArchiveBatchAsync(int id, string userId)
        {
            var response = new ResponseDto<object>();
            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batch = await _repository.GetBatchEntityByIdAsync(id);
            if (batch == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }
            if (!batch.IsActive)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.BatchAlreadyyyArchived;
                return response;
            }
            // Do not archive active batch directly
            if (batch.Status == BatchStatus.Active.ToString())
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.ActiveBatchCannotBeArchived;
                return response;
            }

            batch.IsActive = false;
            batch.IsPublished = false;
            batch.UpdateAt = DateTime.UtcNow;
            batch.UpdateBy = userId;
            await _repository.UpdateBatchAsync(batch);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateBatchAsync(CreateBatchDto dto, string userId)
        {
            var response = new ResponseDto<object>();

            #region Validation

            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            if (dto.CourseId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidCourse;
                return response;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.BatchNameIsRequired;
                return response;
            }

            if (dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidEndDate;
                return response;
            }

            if (dto.Capacity.HasValue && dto.Capacity.Value <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidCapacity;
                return response;
            }

            #endregion

            #region Course Validation

            var courseExists = await _repository.CourseExistsAsync(dto.CourseId);

            if (!courseExists)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }

            #endregion

            #region Duplicate Batch Name

            var batchNameExists = await _repository.BatchNameExistsAsync(dto.CourseId, dto.Name.Trim());
            if (batchNameExists)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.BatchNameAlreadyExits;
                return response;
            }

            #endregion

            #region Batch Code

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                var batchCodeExists = await _repository.BatchCodeExistsAsync(dto.Code.Trim());

                if (batchCodeExists)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;

                    response.Success = false;
                    response.Message = SystemMessage.BatchCodeAlreadyExits;
                    return response;
                }
            }

            #endregion

            #region Create Batch

            var batch = new Batch
            {
                CourseId = dto.CourseId,

                Name = dto.Name.Trim(),

                BatchCode = string.IsNullOrWhiteSpace(dto.Code)
                    ? null
                    : dto.Code.Trim(),

                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),

                StartDate = dto.StartDate,

                EndDate = dto.EndDate,

                Capacity = dto.Capacity,

                Status = dto.Status.ToString(),

                IsPublished = dto.IsPublished,

                IsActive = true,

                CreateAt = DateTime.UtcNow,

                CreateBy = userId
            };

            await _repository.CreateBatchAsync(batch);

            await _unitOfWork.SaveAsync();

            #endregion

            #region Response
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;

            #endregion
        }

        public async Task<ResponseDto<object>> CreateScheduleAsync(int batchId, CreateBatchScheduleDto dto, string userId)
        {
            var response = new ResponseDto<object>();

            if (batchId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }

            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidScheduledata;
                return response;
            }

            // Check Batch
            var batchExists = await _repository.BatchExistsAsync(batchId);

            if (!batchExists)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }

            // Validate Time
            if (dto.EndTime <= dto.StartTime)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidEndTime;
                return response;
            }

            // Validate Date
            if (dto.EndDate.HasValue &&
                dto.EndDate.Value.Date < dto.StartDate.Date)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidEndDate;
                return response;
            }

            // Check Duplicate Schedule
            var scheduleExists = await _repository.ScheduleExistsAsync(batchId, dto.DayOfWeek, dto.StartTime, dto.EndTime);

            if (scheduleExists)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.ScheduleAlreadyExistsBranch;
                return response;
            }

            var schedule = new BatchSchedule
            {
                BatchId = batchId,

                DayOfWeek = dto.DayOfWeek.ToString(),

                StartDate = dto.StartDate,

                EndDate = dto.EndDate,

                StartTime = dto.StartTime,

                EndTime = dto.EndTime,

                ClassType = dto.ClassType.ToString(),

                MettingLink = string.IsNullOrWhiteSpace(dto.MeetingLink)
                    ? null
                    : dto.MeetingLink.Trim(),

                IsActive = true,

                CreateAt = DateTime.UtcNow,
                CreateBy = userId
            };

            await _repository.CreateScheduleAsync(schedule);

            await _unitOfWork.SaveAsync();

            response.ResponseCode = (int)APIResponseCodes.Created;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> GetBatchByIdAsync(int id)
        {
            var response = new ResponseDto<object>();
            #region Validation
            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            #endregion
            #region Get Data
            var result = await _repository.GetBatchByIdAsync(id);
            #endregion
            #region Not Found
            if (result == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            #endregion
            #region Success
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = result;
            response.Message = SystemMessage.RecordFetch;
            return response;
            #endregion
        }

        public async Task<ResponseDto<object>> GetBatchListAsync(PaginationRequestDto pagination, BatchFilterDto filter)
        {
            var response = new ResponseDto<object>();
            #region Validation
            if (pagination == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            #endregion
            #region Pagination Validation
            if (pagination.PageNumber <= 0)
            {
                pagination.PageNumber = 1;
            }
            if (pagination.PageSize <= 0)
            {
                pagination.PageSize = 10;
            }
            if (pagination.PageSize > 100)
            {
                pagination.PageSize = 100;
            }
            #endregion
            #region Filter
            filter ??= new BatchFilterDto();
            #endregion
            var result = await _repository.GetBatchListAsync(pagination, filter);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = result;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateBatchSchedulesStatusAsync(int scheduleId, int batchId, string userId)
        {
            var response = new ResponseDto<object>();

            if (batchId <= 0 || scheduleId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _repository.GetByIdBatchScheduleAsync(scheduleId,batchId);
            if (res == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordStatusChange;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdScheduleAsync(int scheduleId)
        {
            var response = new ResponseDto<object>();
            if (scheduleId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _repository.GetByIdBatchScheduleAsync(scheduleId);
            if (res == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            var data = new GetBatchScheduleDto
            {
                Id = res.Id,
                BatchId = res.BatchId ?? 0,
                BatchName = res.batch.Name,
                DayOfWeek = res.DayOfWeek,
                StartDate = res.StartDate,
                EndDate = res.EndDate,
                StartTime = res.StartTime,
                EndTime = res.EndTime,
                ClassType = res.ClassType,
                MeetingLink = res.MettingLink,
                IsActive = res.IsActive
            };
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = data;
            return response;
        }

        public async Task<ResponseDto<object>> GetListBatchSchedulesAsync(int batchId)
        {
            var response = new ResponseDto<object>();
            if (batchId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batchExists = await _repository.BatchExistsAsync(batchId);
            if (!batchExists)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }
            var schedules = await _repository.GetListBatchSchedulesAsync(batchId);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = schedules;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateBatchAsync(int id, UpdateBatchDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }

            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            var batch = await _repository.GetBatchEntityByIdAsync(id);

            if (batch == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }

            // Course validation
            var courseExists = await _repository.CourseExistsAsync(dto.CourseId);

            if (!courseExists)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.CourseNotFound;
                return response;
            }

            // Date validation
            if (dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidEndDate;
                return response;
            }

            // Capacity validation
            if (dto.Capacity.HasValue && dto.Capacity.Value <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidCapacity;
                return response;
            }

            // Update fields
            batch.CourseId = dto.CourseId;
            batch.Name = dto.Name.Trim();
            batch.BatchCode = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code.Trim();
            batch.Description = dto.Description?.Trim();
            batch.StartDate = dto.StartDate;
            batch.EndDate = dto.EndDate;
            batch.Capacity = dto.Capacity;
            batch.IsPublished = dto.IsPublished;
            // Audit fields
            batch.UpdateAt = DateTime.UtcNow;
            batch.UpdateBy = userId;
            await _repository.UpdateBatchAsync(batch);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateBatchPublishAsync(int id, UpdateBatchPublishDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batch = await _repository.GetBatchEntityByIdAsync(id);
            if (batch == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }

            // Cancelled batch should not be published
            if (batch.Status == BatchStatus.Cancelled.ToString() && dto.IsPublished)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.CannotBePublishCancelBatch;
                return response;
            }
            // Completed batch should normally not be published
            if (batch.Status == BatchStatus.Completed.ToString() && dto.IsPublished)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.CannotBePublishCompleteBatch;
                return response;
            }
            batch.IsPublished = dto.IsPublished;
            batch.UpdateAt = DateTime.UtcNow;
            batch.UpdateBy = userId;
            await _repository.UpdateBatchAsync(batch);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = dto.IsPublished ? SystemMessage.BatchSucessfullPublish : SystemMessage.BatchUnSucessfullPublish;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateBatchStatusAsync(int id, UpdateBatchStatusDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batch = await _repository.GetBatchEntityByIdAsync(id);
            if (batch == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }

            if (!Enum.IsDefined(typeof(BatchStatus), dto.Status))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidBatchStatus;
                return response;
            }
            // Completed batch cannot be moved back
            if (batch.Status == BatchStatus.Completed.ToString() && dto.Status != BatchStatus.Completed)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.CannotBeChangeCompleteStatus;
                return response;
            }
            // Cancelled batch cannot be activated again
            if (batch.Status == BatchStatus.Cancelled.ToString() && dto.Status != BatchStatus.Cancelled)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.CannotBeChangeCancelStatus;
                return response;
            }
            batch.Status = dto.Status.ToString();
            batch.UpdateAt = DateTime.UtcNow;
            batch.UpdateBy = userId;

            await _repository.UpdateBatchAsync(batch);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateScheduleAsync(int batchId, int scheduleId, UpdateBatchScheduleDto dto, string userId)
        {

            var response = new ResponseDto<object>();
            if (batchId <= 0 || scheduleId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var batchExists = await _repository.BatchExistsAsync(batchId);

            if (!batchExists)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }
            var schedule = await _repository.GetByIdBatchScheduleAsync(scheduleId);

            if (schedule == null || schedule.BatchId != batchId)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.ScheduleNotFoundThisBatch;
                return response;             
            }
            if (dto.EndTime <= dto.StartTime)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.InvalidEndTime;
                return response;
            }

            if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.StartDate.Date)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.InvalidEndDate;
                return response;
            }
            var duplicateSchedule = await _repository.ScheduleExistsAsync( batchId, dto.DayOfWeek,   dto.StartTime, dto.EndTime, scheduleId);
            if (duplicateSchedule)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.ScheduleSameDayAlreadyExits;
                return response;
            }
            schedule.DayOfWeek = dto.DayOfWeek.ToString();
            schedule.StartDate = dto.StartDate;
            schedule.EndDate = dto.EndDate;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.ClassType = dto.ClassType.ToString();
            schedule.MettingLink = string.IsNullOrWhiteSpace(dto.MeetingLink)
                ? null
                : dto.MeetingLink.Trim();

            schedule.UpdateAt = DateTime.UtcNow;
            schedule.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async  Task<ResponseDto<object>> GetBatchScheduleListAsync(PaginationRequestDto pagination, BatchScheduleFilterDto filter)
        {
            var response = new ResponseDto<object>();
            if (pagination.PageNumber <= 0)
                pagination.PageNumber = 1;

            if (pagination.PageSize <= 0)
                pagination.PageSize = 10;

            if (pagination.PageSize > 100)
                pagination.PageSize = 100;

            var result = await _repository.GetBatchScheduleListAsync( pagination, filter);
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = result;
            return response;

        }
    }
}
