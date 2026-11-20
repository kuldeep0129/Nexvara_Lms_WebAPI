using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Student.Batch;
using Core.Dtos.Student.Sechdule;
using Core.Interface.Repository;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {

        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task CreateBatchStudentAsync(BatchStudent entity)
        {
            await _context.BatchStudents.AddAsync(entity);
        }

        public async Task CreatePaymentAsync(Payments entity)
        {
            await _context.Payments.AddAsync(entity);
        }

        public async Task CreateStudentEnrollmentAsync(Enrollment entity)
        {
            await _context.Enrollments.AddAsync(entity);
        }

        public async Task<Batch?> GetBatchForEnrollmentAsync(int batchId)
        {
            return await _context.Batches.Include(x => x.course).FirstOrDefaultAsync(x => x.Id == batchId && x.IsActive);
        }

        public async Task<GetBatchStudentDto?> GetByIdBatchStudentAsync(int Id)
        {
            return await _context.BatchStudents.AsNoTracking().Include(x => x.user).Include(x => x.batch).Where(x => x.Id == Id && x.IsActive)
              .Select(x => new GetBatchStudentDto
              {
                  Id = x.Id,
                  BatchId = x.BatchId,
                  BatchName = x.batch.Name,
                  StudentId = x.StudentId,
                  StudentName = x.user.FullName,
                  EnrolledAt = x.EnrolledAt,
                  CompleteAt = x.CompleteAt,
                  IsActive = x.IsActive,
              }).FirstOrDefaultAsync();
        }

        public async Task<BatchStudent?> GetByIdBatchStudentUpdateAsync(int Id)
        {
            return await _context.BatchStudents.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);

        }

        public async Task<ExploreCourseResponseDto<ExploreBatchDto>> GetExploreCoursesAsync(ExploreCourseRequestDto request)
        {
            var query = _context.Batches.AsNoTracking().Include(x => x.course).Where(x => x.IsPublished && x.IsActive && x.course.IsActive && x.course.IsPublished);
            // ============================
            // SEARCH
            // ============================
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x => x.Name.Contains(search) || x.Description.Contains(search) || x.course.Technologies.Contains(search));
            }

            // ============================
            // LEVEL FILTER
            // ============================

            if (!string.IsNullOrWhiteSpace(request.Level) && request.Level != "All")
            {
                query = query.Where(x => x.course.Level == request.Level);
            }

            // ============================
            // MODE FILTER
            // ============================

            if (!string.IsNullOrWhiteSpace(request.Mode) && request.Mode != "All")
            {
                query = query.Where(x => x.course.Mode == request.Mode);
            }

            // ============================
            // PRICE FILTER
            // ============================

            if (!string.IsNullOrWhiteSpace(request.PriceType))
            {
                if (request.PriceType.Equals("Free", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(x => x.course.Price == 0);
                }

                else if (request.PriceType.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(x => x.course.Price > 0);
                }
            }

            // ============================
            // SORT
            // ============================

            query = request.SortBy?.ToLower() switch
            {
                "popular" =>
                    query.OrderByDescending(x => x.course.Star),

                "newest" =>
                    query.OrderByDescending(x => x.Id),

                "pricelowtohigh" =>
                    query.OrderBy(x => x.course.Price),

                "pricehightolow" =>
                    query.OrderByDescending(x => x.course.Price),

                _ =>
                    query.OrderByDescending(x => x.course.Star)
            };

            // ============================
            // TOTAL COUNT
            // ============================

            var totalCount = await query.CountAsync();

            // ============================
            // PAGINATION
            // ============================
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 12 : Math.Min(request.PageSize, 100);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ExploreBatchDto
                {
                    BatchId = x.Id,
                    BatchName = x.Name,
                    CourseId = x.CourseId ?? 0,
                    ThumbnailUrl = x.course.ImageUrl,
                    CourseName = x.course.Title,
                    Description = x.Description,
                    CourseTags = x.course.Technologies,
                    Duration = x.course.Duration,
                    StartDate = x.StartDate,
                    Level = x.course.Level,
                    Mode = x.course.Mode,
                    Price = x.course.Price,
                    Rating = x.course.Star,
                    IsActive = x.IsActive,
                    AvailableSeats = (x.Capacity ?? 0) - _context.Enrollments.Count(e => e.BatchId == x.Id && e.IsActive && (e.PaymentStatus == PaymentStatus.Free.ToString() || e.PaymentStatus == PaymentStatus.Success.ToString())),
                    IsEnrolled = _context.Enrollments.Any(g => g.BatchId == x.Id && (g.PaymentStatus == PaymentStatus.Free.ToString() || g.PaymentStatus == PaymentStatus.Success.ToString()) && g.IsActive == true)
                }).ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new ExploreCourseResponseDto<ExploreBatchDto>
            {
                Items = items,

                PageNumber = pageNumber,

                PageSize = pageSize,

                TotalCount = totalCount,

                TotalPages = totalPages,

                HasNextPage = pageNumber < totalPages,

                HasPreviousPage = pageNumber > 1
            };
        }

        public async Task<PageResultDto<GetBatchStudentDto>?> GetListBatchStudentAsync(int pageNumber, int pageSize)
        {
            var query = _context.BatchStudents.AsNoTracking().Include(x => x.user).Include(x => x.batch)
               .Select(x => new GetBatchStudentDto
               {
                   Id = x.Id,
                   BatchId = x.BatchId,
                   BatchName = x.batch.Name,
                   StudentId = x.StudentId,
                   StudentName = x.user.FullName,
                   EnrolledAt = x.EnrolledAt,
                   CompleteAt = x.CompleteAt,
                   IsActive = x.IsActive,
               });
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetBatchStudentDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<int> GetStudentBatchCountAsync(string studentId, string? status)
        {
            var query = _context.Enrollments.AsNoTracking().Where(x => x.StudentId == studentId && x.BatchId != null);
            query = ApplyStatusFilter(query, status);
            return await query.CountAsync();
        }

        public async Task<List<GetStudentEnrolledBatchDto>> GetStudentEnrolledBatchesAsync(string studentId, StudentEnrolledBatchFilterDto filter)
        {
            var today = DateTime.Today;
            var query = _context.Enrollments.AsNoTracking().Where(x => x.StudentId == studentId && x.BatchId != null && x.batch != null);

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(x => x.batch!.Name.Contains(search) || x.batch.BatchCode.Contains(search) || x.course.Title.Contains(search));
            }

            // Status filter
            query = ApplyStatusFilter(query, filter.Status);

            // Sorting
            query = (filter.SortBy ?? "Progress").ToLower() switch
            {
                "name" => query.OrderBy(x => x.batch!.Name),

                "newest" => query.OrderByDescending(x => x.EnrollmentDate),

                "progress" => query.OrderByDescending(x => x.CompletionPercentage),

                _ => query.OrderByDescending(x => x.EnrollmentDate)
            };

            // Pagination
            return await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                .Select(x => new GetStudentEnrolledBatchDto
                {
                    EnrollmentId = x.Id,

                    BatchId = x.BatchId!.Value,
                    BatchCode = x.batch!.BatchCode,
                    BatchName = x.batch.Name,

                    CourseId = x.CourseId ?? 0,
                    CourseName = x.course.Title,

                    StartDate = x.batch.StartDate,
                    EndDate = x.batch.EndDate,

                    ProgressPercentage = x.CompletionPercentage ?? 0,

                    Status = x.CompletedAt != null || x.batch.Status == BatchStatus.Completed.ToString() || x.batch.EndDate < today ? BatchStatus.Completed.ToString() : x.batch.StartDate > today ? BatchStatus.Upcoming.ToString() : BatchStatus.Active.ToString(),

                    Action = x.CompletedAt != null || x.batch.Status == BatchStatus.Completed.ToString() || x.batch.EndDate < today ? "Rewatch" : "Enter"
                })
                .ToListAsync();
        }

        // Common status condition
        private static IQueryable<Enrollment> ApplyStatusFilter(IQueryable<Enrollment> query, string? status)
        {
            var today = DateTime.Today;

            if (string.IsNullOrWhiteSpace(status) ||
                status.Equals("All", StringComparison.OrdinalIgnoreCase))
                return query;

            switch (status.ToLower())
            {
                case "upcoming":
                    query = query.Where(x => x.batch != null && x.batch.StartDate > today);
                    break;

                case "completed":
                    query = query.Where(x => x.CompletedAt != null || (x.batch != null && (x.batch.Status == BatchStatus.Completed.ToString() || x.batch.EndDate < today)));
                    break;

                case "active":
                    query = query.Where(x => x.CompletedAt == null && x.batch != null && x.batch.StartDate <= today && x.batch.EndDate >= today && x.batch.Status != BatchStatus.Cancelled.ToString() && x.batch.Status != BatchStatus.Completed.ToString());
                    break;
            }

            return query;
        }


        public async Task<List<GetStudentSechduleDto>> GetStudentSechduleAsync(DateTime? dateTime, string studentId)
        {
            var fromDate = dateTime?.Date ?? DateTime.Today;
            return await _context.BatchClassSessions.AsNoTracking().Include(x => x.batch).Include(x => x.courseModule).Where(x => x.SessionDate >= fromDate && _context.Enrollments.Any(g => g.BatchId == x.BatchId && g.StudentId == studentId && g.IsActive)).Select(x => new GetStudentSechduleDto
            {
                Id = x.Id,
                BatchName = x.batch.Name,
                ModuleName = x.courseModule.Title,
                Date = x.SessionDate,
                Time = x.StartTime
            }).ToListAsync();
        }

        public async Task<bool> IsCheckAlreadyStudentEnrollAsync(string userId, int courseId, int batchId)
        {
            return await _context.Enrollments.AnyAsync(x => x.StudentId == userId && x.IsActive && (x.PaymentStatus == PaymentStatus.Free.ToString() || x.PaymentStatus == PaymentStatus.Success.ToString()) && x.BatchId == batchId && x.CourseId == courseId);
        }

        public Task<bool> IsCheckBatchFreeOrPaidAsync(int courseId, int batchId)
        {
            throw new NotImplementedException();
        }

        public async Task<BatchStudent?> UpdateBatchStudentAsync(int Id)
        {
            return await _context.BatchStudents.FirstOrDefaultAsync(x => x.Id == Id);

        }

        public async Task<List<GetContinueLearningDto>> GetContinueLearningAsync(string studentId)
        {
            var batches = await _context.Enrollments.AsNoTracking().Where(x => x.StudentId == studentId && x.IsActive && (x.PaymentStatus == PaymentStatus.Success.ToString() || x.PaymentStatus == PaymentStatus.Free.ToString()))
                .Select(g => new GetContinueLearningDto
                {
                    BatchId = g.BatchId ?? 0,
                    BatchCode = g.batch.BatchCode,
                    BatchName = g.batch.Name,
                    CourseId = g.CourseId ?? 0,
                    CourseName = g.course.Title,
                    ProgressClass = _context.BatchClassSessions.Count(s => s.BatchId == g.BatchId && s.Status == "Completed"),
                    TotalClass = _context.BatchClassSessions.Count(s => s.BatchId == g.BatchId && s.Status != "Cancelled"),
                    ModuleList = new List<GetContinueModuledto>()
                }).ToListAsync();
            if (!batches.Any())
                return batches;
            var batchIds = batches.Select(x => x.BatchId).Distinct().ToList();
            var courseIds = batches.Select(x => x.CourseId).Distinct().ToList();
            var videos = await _context.CourseVideos.AsNoTracking().Where(v => v.IsPublished && v.CourseId != null && batchIds.Contains(v.BatchId.Value) && courseIds.Contains(v.CourseId.Value))
                .Select(v => new
                {
                    v.Id,
                    BatchId = v.BatchId,
                    ModuleId = v.ModuleId,
                    ModuleName = v.module.Title,
                    Duration = v.DurationInSeconds ?? 0
                }).ToListAsync();
            if (!videos.Any())
                return batches;
            var videoIds = videos.Select(v => v.Id).ToList();
            var progressRecords = await _context.VideoProgresses.AsNoTracking().Where(p => p.StudentId == studentId && p.VideoId != null && videoIds.Contains(p.VideoId.Value))
                .Select(p => new
                {
                    VideoId = p.VideoId.Value,
                    WatchedSeconds = p.WatchedSeconds ?? 0,
                    CompletionPercentage = p.CompletionPercentage ?? 0,
                    p.IsCompleted,
                    p.LastWatchedAt
                }).ToListAsync();
            foreach (var batch in batches)
            {
                var batchVideos = videos.Where(v => v.BatchId == batch.BatchId).ToList();

                var moduleGroups = batchVideos.Where(v => v.ModuleId != null)
                    .GroupBy(v => new
                    {
                        ModuleId = v.ModuleId.Value,
                        v.ModuleName
                    });

                foreach (var module in moduleGroups)
                {
                    var moduleVideos = module.ToList();

                    var moduleProgress = progressRecords.Where(p => moduleVideos.Any(v => v.Id == p.VideoId)).ToList();

                    var totalDuration = moduleVideos.Sum(v => v.Duration);

                    var watchedDuration = moduleProgress
                        .Sum(p => p.IsCompleted
                            ? moduleVideos.First(v => v.Id == p.VideoId).Duration
                            : Math.Min(
                                p.WatchedSeconds,
                                moduleVideos.First(v => v.Id == p.VideoId).Duration));

                    var remainingDuration = Math.Max(  0, totalDuration - watchedDuration);

                    var percentage = totalDuration == 0
                        ? 0
                        : Math.Round(
                            (decimal)watchedDuration / totalDuration * 100,
                            2);

                    batch.ModuleList.Add(new GetContinueModuledto
                    {
                        ModuleId = module.Key.ModuleId,
                        ModuleName = module.Key.ModuleName,

                        CurrentLeftCompleteDuration = FormatDuration(remainingDuration),

                        completePercentage = $"{percentage:0.##}%",

                        LastWatchAt = moduleProgress.Any()
                            ? moduleProgress.Max(p => p.LastWatchedAt)
                            : null
                    });
                }
            }

            return batches;

        }
        private static string FormatDuration(int seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            return time.TotalHours >= 1 ? time.ToString(@"hh\:mm\:ss") : time.ToString(@"mm\:ss");
        }
    }
}
