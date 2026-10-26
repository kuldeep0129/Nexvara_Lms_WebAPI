using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Interface.Repository;
using Domain.Entities;
using Domain.Entities.Student;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BatchRepository : IBatchRepository
    {
        private readonly ApplicationDbContext _context;
        public BatchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BatchCodeExistsAsync(string code)
        {
            return await _context.Batches.AsNoTracking().AnyAsync(x => x.BatchCode != null && x.BatchCode.ToLower() == code.ToLower());
        }

        public async Task<bool> BatchExistsAsync(int batchId)
        {
            return await _context.Batches.AsNoTracking().AnyAsync(x => x.Id == batchId);
        }

        public async Task<bool> BatchNameExistsAsync(int courseId, string name)
        {
            return await _context.Batches.AsNoTracking().AnyAsync(x => x.CourseId == courseId && x.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses.AsNoTracking().AnyAsync(x => x.Id == courseId);
        }

        public async Task CreateBatchAsync(Batch entity)
        {
            await _context.Batches.AddAsync(entity);
        }

        public async Task CreateScheduleAsync(BatchSchedule schedule)
        {
            await _context.BatchSchedules.AddAsync(schedule);
        }

        public async Task<GetBatchDetailsDto?> GetBatchByIdAsync(int id)
        {
            return await _context.Batches.AsNoTracking().Include(x => x.course).Where(x => x.Id == id)
              .Select(x => new GetBatchDetailsDto
              {
                  Id = x.Id,
                  CourseId = x.CourseId ?? 0,
                  CourseName = x.course.Title,
                  Name = x.Name,
                  Code = x.BatchCode,
                  Description = x.Description,
                  StartDate = x.StartDate,
                  EndDate = x.EndDate,
                  Capacity = x.Capacity,
                  TotalStudents = _context.Enrollments.Count(e => e.BatchId == x.Id && e.IsActive),
                  TotalTeachers = _context.BatchTeachers.Count(t => t.BatchId == x.Id && t.IsActive),
                  Status = x.Status,
                  IsPublished = x.IsPublished,
                  IsActive = x.IsActive,
                  CreateAt = x.CreateAt,
                  CreateBy = x.CreateBy,
                  UpdateAt = x.UpdateAt,
                  UpdateBy = x.UpdateBy
              }).FirstOrDefaultAsync();
        }

        public async Task<Batch?> GetBatchEntityByIdAsync(int id)
        {
            return await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PageResultDto<GetBatchListDto>> GetBatchListAsync(PaginationRequestDto pagination, BatchFilterDto filter)
        {
            var query = _context.Batches.AsNoTracking().Include(x => x.course).AsQueryable();
            #region Search

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();

                query = query.Where(x => x.Name.ToLower().Contains(search) || (x.BatchCode != null && x.BatchCode.ToLower().Contains(search)) || (x.Description != null && x.Description.ToLower().Contains(search)));
            }

            #endregion


            #region Course Filter

            if (filter.CourseId.HasValue)
            {
                query = query.Where(x => x.CourseId == filter.CourseId.Value);
            }

            #endregion


            #region Status Filter

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.ToString());
            }

            #endregion


            #region Published Filter

            if (filter.IsPublished.HasValue)
            {
                query = query.Where(x => x.IsPublished == filter.IsPublished.Value);
            }

            #endregion


            #region Active Filter

            if (filter.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == filter.IsActive.Value);
            }

            #endregion


            #region Total Count

            var totalCount = await query.CountAsync();

            #endregion


            #region Sorting

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.Trim().ToLower())
                {
                    case "name":

                        query = filter.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                        break;

                    case "startdate":

                        query = filter.SortDescending ? query.OrderByDescending(x => x.StartDate) : query.OrderBy(x => x.StartDate);
                        break;

                    case "enddate":

                        query = filter.SortDescending ? query.OrderByDescending(x => x.EndDate) : query.OrderBy(x => x.EndDate);
                        break;
                    case "createdat":

                        query = filter.SortDescending ? query.OrderByDescending(x => x.CreateAt) : query.OrderBy(x => x.CreateAt);
                        break;
                    default:

                        query = query.OrderByDescending(x => x.CreateAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.CreateAt);
            }

            #endregion


            #region Projection + Pagination

            var items = await query
                .Select(x => new GetBatchListDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId ?? 0,
                    CourseName = x.course.Title,
                    Name = x.Name,
                    Code = x.BatchCode,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Capacity = x.Capacity,
                    TotalStudents = _context.Enrollments.Count(e => e.BatchId == x.Id && e.IsActive),
                    TotalTeachers = _context.BatchTeachers.Count(t => t.BatchId == x.Id && t.IsActive),
                    Status = x.Status ?? "",
                    IsPublished = x.IsPublished,
                    IsActive = x.IsActive
                })
                .Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

            #endregion


            return new PageResultDto<GetBatchListDto>
            {
                Items = items,

                PageNumber = pagination.PageNumber,

                PageSize = pagination.PageSize,

                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetBatchScheduleDto>> GetBatchScheduleListAsync(PaginationRequestDto pagination, BatchScheduleFilterDto filter)
        {
            var query = _context.BatchSchedules.AsNoTracking().Include(x => x.batch).ThenInclude(x => x.course).AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>x.batch.Name.Contains(search) ||(x.batch.BatchCode != null && x.batch.BatchCode.Contains(search)) || x.batch.course.Title.Contains(search));
            }

            // Batch filter
            if (filter.BatchId.HasValue)
            {
                query = query.Where(x => x.BatchId == filter.BatchId.Value);
            }

            // Day filter
            if (filter.DayOfWeek.HasValue)
            {
                query = query.Where(x => x.DayOfWeek == filter.DayOfWeek.ToString());
            }
            // Active filter
            if (filter.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == filter.IsActive.Value);
            }

            var totalCount = await query.CountAsync();

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "batchname" => filter.SortDescending ? query.OrderByDescending(x => x.batch.Name) : query.OrderBy(x => x.batch.Name),

                "startdate" => filter.SortDescending? query.OrderByDescending(x => x.StartDate)  : query.OrderBy(x => x.StartDate),

                "starttime" => filter.SortDescending ? query.OrderByDescending(x => x.StartTime) : query.OrderBy(x => x.StartTime),

                "dayofweek" => filter.SortDescending  ? query.OrderByDescending(x => x.DayOfWeek)  : query.OrderBy(x => x.DayOfWeek),

                _ => filter.SortDescending  ? query.OrderByDescending(x => x.Id)  : query.OrderBy(x => x.Id)
            };

            var items = await query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).Select(x => new GetBatchScheduleDto
                {
                    Id = x.Id,
                    BatchId = x.BatchId ?? 0,
                    BatchName = x.batch.Name,

                    CourseId = x.batch.CourseId ?? 0,
                    CourseName = x.batch.course.Title,

                    DayOfWeek = x.DayOfWeek,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    ClassType = x.ClassType,
                    MeetingLink = x.MettingLink,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return new PageResultDto<GetBatchScheduleDto>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<BatchSchedule?> GetByIdBatchScheduleAsync(int scheduleId)
        {
           return await _context.BatchSchedules.Include(x=> x.batch).FirstOrDefaultAsync(x=> x.Id == scheduleId);
        }

        public async Task<BatchSchedule?> GetByIdBatchScheduleAsync(int scheduleId, int batchId)
        {
            return await _context.BatchSchedules.Include(x => x.batch).FirstOrDefaultAsync(x => x.Id == scheduleId && x.BatchId == batchId);

        }

        public async Task<List<GetBatchScheduleDto>> GetListBatchSchedulesAsync(int batchId)
        {

            return await _context.BatchSchedules.AsNoTracking().Where(x => x.BatchId == batchId).OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime)
                .Select(x => new GetBatchScheduleDto
                {
                    Id = x.Id,
                    BatchId = x.BatchId ?? 0,
                    BatchName = x.batch.Name,
                    DayOfWeek = x.DayOfWeek,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    ClassType = x.ClassType,
                    MeetingLink = x.MettingLink,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> ScheduleExistsAsync(int batchId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {

            return await _context.BatchSchedules.AsNoTracking().AnyAsync(x =>  x.BatchId == batchId &&  x.DayOfWeek == dayOfWeek.ToString() && x.StartTime == startTime && x.EndTime == endTime && x.IsActive);
        }

        public async Task<bool> ScheduleExistsAsync(int batchId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int scheduleId)
        {
            return await _context.BatchSchedules.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.Id != scheduleId &&x.DayOfWeek == dayOfWeek.ToString() && x.StartTime == startTime &&  x.EndTime == endTime && x.IsActive);
        }

        public async Task<bool> UpdateBatchAsync(Batch batch)
        {
            _context.Batches.Update(batch);
            return true;
        }

      
    }
}

