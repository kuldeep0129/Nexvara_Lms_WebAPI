using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Interface.Repository;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Enums;
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

        public async Task<Batch?> BatchExistsAsync(int batchId)
        {
            return await _context.Batches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == batchId && x.IsActive );
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

        public async Task<PageResultDto<GetBatchScheduleGroupDto>> GetBatchScheduleListAsync( PaginationRequestDto pagination, BatchScheduleFilterDto filter)
        {
            // =========================================================
            // 1. Normalize Pagination
            // =========================================================

            var pageNumber = pagination.PageNumber <= 0
                ? 1
                : pagination.PageNumber;

            var pageSize = pagination.PageSize <= 0
                ? 10
                : Math.Min(pagination.PageSize, 100);


            // =========================================================
            // 2. Base Query
            // =========================================================

            var query = _context.BatchSchedules
                .AsNoTracking()
                .AsQueryable();


            // =========================================================
            // 3. Search
            // =========================================================

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    x.batch.Name.Contains(search) ||
                    (x.batch.BatchCode != null &&
                     x.batch.BatchCode.Contains(search)) ||
                    x.batch.course.Title.Contains(search));
            }


            // =========================================================
            // 4. Batch Filter
            // =========================================================

            if (filter.BatchId.HasValue)
            {
                query = query.Where(x =>
                    x.BatchId == filter.BatchId.Value);
            }


            
            if (filter.DayOfWeek.HasValue)
            {
                var dayOfWeek =  filter.DayOfWeek.Value.ToString();

                query = query.Where(x => x.DayOfWeek == dayOfWeek);
            }


            // =========================================================
            // 6. Active / Inactive Filter
            // =========================================================

            if (filter.IsActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == filter.IsActive.Value);
            }


            // =========================================================
            // 7. GROUP BY BATCH
            // =========================================================

            var groupedQuery = query
                .GroupBy(x => new
                {
                    BatchId = x.BatchId,
                    BatchName = x.batch.Name,
                    CourseId = x.batch.CourseId,
                    CourseName = x.batch.course.Title
                })
                .Select(g => new
                {
                    BatchId = g.Key.BatchId,
                    BatchName = g.Key.BatchName,
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.CourseName
                });


            // =========================================================
            // 8. Total Batch Count
            // =========================================================

            var totalCount = await groupedQuery.CountAsync();


            // =========================================================
            // 9. Sorting
            // =========================================================

            groupedQuery = filter.SortBy?.ToLowerInvariant() switch
            {
                "batchname" => filter.SortDescending
                    ? groupedQuery.OrderByDescending(x => x.BatchName)
                    : groupedQuery.OrderBy(x => x.BatchName),

                "coursename" => filter.SortDescending
                    ? groupedQuery.OrderByDescending(x => x.CourseName)
                    : groupedQuery.OrderBy(x => x.CourseName),

                "batchid" => filter.SortDescending
                    ? groupedQuery.OrderByDescending(x => x.BatchId)
                    : groupedQuery.OrderBy(x => x.BatchId),

                _ => filter.SortDescending
                    ? groupedQuery.OrderByDescending(x => x.BatchId)
                    : groupedQuery.OrderBy(x => x.BatchId)
            };


            // =========================================================
            // 10. Batch-Level Pagination
            // =========================================================

            var pagedBatches = await groupedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            // =========================================================
            // 11. Empty Result
            // =========================================================

            if (!pagedBatches.Any())
            {
                return new PageResultDto<GetBatchScheduleGroupDto>
                {
                    Items = new List<GetBatchScheduleGroupDto>(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }


            // =========================================================
            // 12. Get Batch IDs
            // =========================================================

            var batchIds = pagedBatches
                .Where(x => x.BatchId.HasValue)
                .Select(x => x.BatchId!.Value)
                .ToList();


            // =========================================================
            // 13. Get Primary Teachers
            // =========================================================

            var primaryTeachers = await _context.BatchTeachers
                .AsNoTracking()
                .Where(x =>
                    x.IsPrimary &&
                    x.BatchId.HasValue &&
                    batchIds.Contains(x.BatchId.Value))
                .Select(x => new
                {
                    BatchId = x.BatchId!.Value,
                    TeacherId = x.TeacherId,
                    TeacherName = x.user.FullName
                })
                .ToListAsync();


            // =========================================================
            // 14. Get Enrollment Counts
            // =========================================================

            var enrollmentCounts = await _context.Enrollments
                .AsNoTracking()
                .Where(x =>
                    x.BatchId.HasValue &&
                    batchIds.Contains(x.BatchId.Value) &&
                    x.IsActive)
                .GroupBy(x => new
                {
                    x.BatchId,
                    x.CourseId
                })
                .Select(g => new
                {
                    BatchId = g.Key.BatchId!.Value,
                    CourseId = g.Key.CourseId!.Value,
                    Count = g.Count()
                })
                .ToListAsync();


            // =========================================================
            // 15. Get Schedules
            // =========================================================

            var schedules = await _context.BatchSchedules.Include(x=> x.batch).ThenInclude(x=> x.course)
                .AsNoTracking()
                .Where(x =>
                    x.BatchId.HasValue &&
                    batchIds.Contains(x.BatchId.Value))
                .Select(x => new
                {
                    Id = x.Id,

                    BatchId = x.BatchId!.Value,

                    BatchName = x.batch.Name,

                    CourseId = x.batch.CourseId!.Value,

                    CourseName = x.batch.course.Title,
                    Thumbnail = x.batch.course.ImageUrl,

                    Capacity = x.batch.Capacity,

                    DayOfWeek = x.DayOfWeek,

                    StartDate = x.StartDate,

                    EndDate = x.EndDate,

                    StartTime = x.StartTime,

                    EndTime = x.EndTime,

                    ClassType = x.ClassType,

                    MeetingLink = x.MettingLink,

                    IsActive = x.IsActive
                })
                .OrderBy(x => x.BatchId)
                .ThenBy(x => x.StartDate)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.Id)
                .ToListAsync();


            // =========================================================
            // 16. Build Final Response
            // =========================================================

            var items = pagedBatches
                .Select(batch =>
                {
                    var batchId = batch.BatchId ?? 0;
                    var courseId = batch.CourseId ?? 0;

                    // Primary teacher
                    var teacher = primaryTeachers
                        .FirstOrDefault(x => x.BatchId == batchId);

                    // Enrollment
                    var enrollment = enrollmentCounts
                        .FirstOrDefault(x =>
                            x.BatchId == batchId &&
                            x.CourseId == courseId);

                    // Batch schedules
                    var batchSchedules = schedules
                        .Where(x => x.BatchId == batchId)
                        .Select(x => new GetBatchScheduleDto
                        {
                            Id = x.Id,

                            BatchId = x.BatchId,
                            BatchName = x.BatchName,

                            CourseId = x.CourseId,
                            CourseName = x.CourseName,

                            TeacherId = teacher?.TeacherId,
                            TeacherName = teacher?.TeacherName,
                            ImageUrl = x.Thumbnail,
                            EnrolledStudent = enrollment?.Count ?? 0,

                            TotalStuentSheet = x.Capacity,

                            DayOfWeek = x.DayOfWeek,

                            StartDate = x.StartDate,
                            EndDate = x.EndDate,

                            StartTime = x.StartTime,
                            EndTime = x.EndTime,

                            ClassType = x.ClassType,

                            MeetingLink = x.MeetingLink,

                            IsActive = x.IsActive
                        })
                        .ToList();

                    return new GetBatchScheduleGroupDto
                    {
                        BatchId = batchId,
                        BatchName = batch.BatchName,

                        CourseId = courseId,
                        CourseName = batch.CourseName,

                        Schedules = batchSchedules
                    };
                })
                .ToList();


            // =========================================================
            // 17. Final Response
            // =========================================================

            return new PageResultDto<GetBatchScheduleGroupDto>
            {
                Items = items,

                PageNumber = pageNumber,

                PageSize = pageSize,

                TotalCount = totalCount
            };
        }
        public async Task<BatchSchedule?> GetByIdBatchScheduleAsync(int scheduleId)
        {
            return await _context.BatchSchedules.Include(x => x.batch).FirstOrDefaultAsync(x => x.Id == scheduleId);
        }

        public async Task<BatchSchedule?> GetByIdBatchScheduleAsync(int scheduleId, int batchId)
        {
            return await _context.BatchSchedules.Include(x => x.batch).FirstOrDefaultAsync(x => x.Id == scheduleId && x.BatchId == batchId);

        }

        //public async Task<PageResultDto<GetTeacherBatchListDto>?> GetListBatchActiveForTeacherAsync(int pageNumber, int pageSize)
        //{
        //    var today = DateTime.Today;
        //    var query = _context.BatchTeachers.AsNoTracking().Include(x => x.batch).ThenInclude(x => x.course).Include(x => x.user).Where(x => x.IsActive == true && (x.batch.StartDate >= today && x.batch.EndDate <= today)).OrderByDescending(x => x.CreateAt).AsQueryable();
        //    var totalCount = await query.CountAsync();
        //    #region Projection + Pagination

        //    var items = await query
        //        .Select(x => new GetTeacherBatchListDto
        //        {
        //            Id = x.Id,
        //            BatchId = x.BatchId ?? 0,
        //            BatchCode = x.batch.BatchCode,
        //            BatchName = x.batch.Name,
        //            CourseId = x.batch.CourseId ?? 0,
        //            CourseName = x.batch.course.Title,
        //            IsPrimary = x.IsPrimary,
        //            EnrollStudentSheetCount = _context.Enrollments.Count(g => g.BatchId == x.BatchId && g.IsActive == true && (g.PaymentStatus == PaymentStatus.Success.ToString() || g.PaymentStatus == PaymentStatus.Free.ToString())),
        //            TotalStudentSheetCount = x.batch.Capacity ?? 0,
        //            VideoProgress = 0,
        //            NextClassDate = _context.BatchSchedules.Where(x => x.BatchId == x.BatchId && x.IsActive)
        //            IsActive = x.IsActive
        //        })
        //        .Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

        //    #endregion


        //    return new PageResultDto<GetBatchListDto>
        //    {
        //        Items = items,

        //        PageNumber = pagination.PageNumber,

        //        PageSize = pagination.PageSize,

        //        TotalCount = totalCount
        //    };
        //}

        private static DateTime? CalculateNextClassDate(BatchSchedule schedule, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(schedule.DayOfWeek))
                return null;
            if (!schedule.StartDate.HasValue || !schedule.EndDate.HasValue)
                return null;
            if (!Enum.TryParse<DayOfWeek>(schedule.DayOfWeek, true, out var targetDay))
            {
                return null;
            }
            var startDate = schedule.StartDate.Value.Date;
            var endDate = schedule.EndDate.Value.Date;
            var fromDate = now.Date > startDate ? now.Date : startDate;
            if (fromDate > endDate)
                return null;
            var daysToAdd = ((int)targetDay - (int)fromDate.DayOfWeek + 7) % 7;
            var nextDate = fromDate.AddDays(daysToAdd);
            if (nextDate > endDate)
                return null;
            var classTime = schedule.StartTime ?? TimeSpan.Zero;
            var nextClassDateTime = nextDate.Add(classTime);
            if (nextClassDateTime <= now)
            {
                nextClassDateTime = nextClassDateTime.AddDays(7);
                if (nextClassDateTime.Date > endDate)
                    return null;
            }
            return nextClassDateTime;
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

            return await _context.BatchSchedules.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.DayOfWeek == dayOfWeek.ToString() && x.StartTime == startTime && x.EndTime == endTime && x.IsActive);
        }

        public async Task<bool> ScheduleExistsAsync(int batchId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int scheduleId)
        {
            return await _context.BatchSchedules.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.Id != scheduleId && x.DayOfWeek == dayOfWeek.ToString() && x.StartTime == startTime && x.EndTime == endTime && x.IsActive);
        }

        public async Task<bool> UpdateBatchAsync(Batch batch)
        {
            _context.Batches.Update(batch);
            return true;
        }

        public async Task CreateBatchClassSessionAsync(List<BatchClassSession> session)
        {
            await _context.BatchClassSessions.AddRangeAsync(session);
        }

        public async Task<List<BatchClassSession?>> GetListByIdBatchClassSessionAsync(int scheduleId, int batchId)
        {
            return await _context.BatchClassSessions.Where(x=> x.BatchId == batchId && x.BatchScheduleId == scheduleId).ToListAsync();
        }
    }
}

