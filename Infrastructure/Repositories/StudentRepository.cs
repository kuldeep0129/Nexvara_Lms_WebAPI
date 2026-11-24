using Core.Dtos.Certificate;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Dashboard;
using Core.Dtos.Payments;
using Core.Dtos.Student;
using Core.Dtos.Student.Attendance;
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

        public async Task<ExploreCourseResponseDto<ExploreBatchDto>> GetExploreCoursesAsync(ExploreCourseRequestDto request,string userId)
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
                    IsEnrolled = _context.Enrollments.Any(g => g.StudentId== userId && g.BatchId == x.Id && (g.PaymentStatus == PaymentStatus.Free.ToString() || g.PaymentStatus == PaymentStatus.Success.ToString()) && g.IsActive == true)
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

        public async Task<PageResultDto<GetStudentClassAttendanceDto>> GetStudentClassAttendanceAsync(string studentId, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 50);
            var query = _context.ClassAttendances.AsNoTracking().Where(x =>  x.StudentId == studentId &&  x.IsActive);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.JoinedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetStudentClassAttendanceDto
                {
                    Id = x.Id,
                    LiveClassId = x.LiveClassId,
                    LiveClassTitle = x.liveClasses != null  ? x.liveClasses.Title  : null,
                    BatchName = x.liveClasses != null &&  x.liveClasses.batch != null  ? x.liveClasses.batch.Name : null,
                    ModuleName = x.liveClasses != null &&  x.liveClasses.module != null  ? x.liveClasses.module.Name  : null,
                    JoinedAt = x.JoinedAt,
                    LeftAt = x.LeftAt,
                    Status = x.Status,
                    DurationInSeconds = x.DurationInSeconds
                }).ToListAsync();

            return new PageResultDto<GetStudentClassAttendanceDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<StudentCertificateDto>> GetMyCertificatesAsync(string studentId)
        {
            return await _context.Certificates.AsNoTracking()
               .Where(x => x.StudentId == studentId && x.IsActive)
               .OrderByDescending(x => x.IssueDate)
               .Select(x => new StudentCertificateDto
               {
                   Id = x.Id,
                   CourseId = x.CourseId ?? 0,
                   CourseName = x.course != null ? x.course.Title : string.Empty,
                   CertificateNumber = x.CertificateNumber,
                   VerificationCode = x.VerificationCode,
                   IssuedAt = x.IssueDate ?? default,
                   CertificateUrl = x.CertificateUrl,
                   IsActive = x.IsActive
               }).ToListAsync();
        }

        public  Task<bool> IsCertificateIssuedAsync(string studentId, int courseId)
        {
            return _context.Certificates.AnyAsync(x => x.StudentId == studentId && x.CourseId == courseId && x.IsActive);

        }

        public Task<bool> IsStudentEligibleForCertificateAsync(string studentId, int courseId)
        {
            return _context.Enrollments.AsNoTracking().AnyAsync(x =>
                          x.StudentId == studentId && x.CourseId == courseId && x.IsActive &&
                          (x.PaymentStatus == PaymentStatus.Success.ToString() || x.PaymentStatus == PaymentStatus.Free.ToString()) &&
                          (x.CompletedAt != null || x.CompletionPercentage >= 60));
        }

        public Task<Certificates?> GetCertificateByVerificationCodeAsync(string verificationCode)
        {
            return _context.Certificates.AsNoTracking()
                          .Include(x => x.user).Include(x => x.course)
                          .FirstOrDefaultAsync(x => x.VerificationCode == verificationCode && x.IsActive);
        }

        public async Task CreateStudentCertificateAsync(Certificates certificate)
        {
            await _context.Certificates.AddAsync(certificate);
        }

        public async Task<List<StudentPaymentDto>> GetStudentPaymentsAsync(string studentId)
        {
            return await _context.Set<Payments>()
                .AsNoTracking()
                .Where(p => p.StudentId == studentId )
                .OrderByDescending(p => p.PaidAt)
                .Select(p => new StudentPaymentDto
                {
                    Id = p.Id,
                    EnrollmentId = p.EnrollmentId,
                    Amount = p.Amount,
                    PaymentGateway = p.PaymentGateway,
                    PaymentStatus = p.PaymentStatus,
                    TransactionId = p.TransactionId,
                    PaidAt = p.PaidAt
                })
                .ToListAsync();
        }

        public async Task<GetStudentDashboardDto> GetStudentDashboardAsync(string studentId)
        {
            var today = DateTime.Today;
            var currentTime = DateTime.Now.TimeOfDay;

            // =====================================================
            // 1. Student Information
            // =====================================================

            var studentName = await _context.Users.AsNoTracking().Where(x => x.Id == studentId).Select(x => x.FullName).FirstOrDefaultAsync();

            // =====================================================
            // 2. Valid Enrollments
            // =====================================================

            var enrollmentQuery = _context.Enrollments
                .AsNoTracking().Where(x =>
                    x.StudentId == studentId &&
                    x.IsActive &&
                    x.BatchId != null &&
                    (x.PaymentStatus == PaymentStatus.Free.ToString() ||
                     x.PaymentStatus == PaymentStatus.Success.ToString()));

            var enrolledCourses = await enrollmentQuery
                .Where(x => x.CourseId != null)
                .Select(x => x.CourseId)
                .Distinct()
                .CountAsync();

            // =====================================================
            // 3. Upcoming Live Classes
            // =====================================================

            var enrolledBatchIds = enrollmentQuery.Select(x => x.BatchId);

            var liveClasses = await _context.BatchClassSessions
                .AsNoTracking()
                .Where(x =>
                    x.BatchId != null &&
                    enrolledBatchIds.Contains(x.BatchId) &&
                    x.IsActive &&
                    x.SessionDate >= today &&
                    x.Status == BatchStatus.Active.ToString() &&
                    (x.SessionDate > today || x.StartTime >= currentTime))
                .CountAsync();

            // =====================================================
            // 4. Student Certificates
            // =====================================================

            var certificates = await _context.Certificates
                .AsNoTracking()
                .CountAsync(x =>
                    x.StudentId == studentId &&
                    x.IsActive);

            // Assignment module is not yet implemented.
            // =====================================================

            // =====================================================
            // 5. My Enrolled Courses
            // =====================================================

            var courses = await enrollmentQuery
                .Where(x => x.CourseId != null && x.BatchId != null)
                .Select(x => new StudentDashboardCourseDto
                {
                    BatchId = x.BatchId.Value,

                    CourseId = x.CourseId.Value,

                    CourseName = x.course != null
                        ? x.course.Title
                        : string.Empty,

                    CourseImage = x.course != null
                        ? x.course.ImageUrl
                        : null,

                    BatchName = x.batch != null
                        ? x.batch.Name
                        : string.Empty,

                    BatchCode = x.batch != null
                        ? x.batch.BatchCode
                        : string.Empty,

                    Progress = x.CompletionPercentage ?? 0,

                    NextClassDate = _context.BatchClassSessions
                        .Where(s =>
                            s.BatchId == x.BatchId &&
                            s.IsActive &&
                            s.Status == BatchStatus.Active.ToString() &&
                            (s.SessionDate > today ||
                             (s.SessionDate == today &&
                              s.StartTime >= currentTime)))
                        .OrderBy(s => s.SessionDate)
                        .ThenBy(s => s.StartTime)
                        .Select(s => (DateTime?)s.SessionDate)
                        .FirstOrDefault(),

                    NextClassTime = _context.BatchClassSessions
                        .Where(s =>
                            s.BatchId == x.BatchId &&
                            s.IsActive &&
                            s.Status == BatchStatus.Active.ToString() &&
                            (s.SessionDate > today ||
                             (s.SessionDate == today &&
                              s.StartTime >= currentTime)))
                        .OrderBy(s => s.SessionDate)
                        .ThenBy(s => s.StartTime)
                        .Select(s => (TimeSpan?)s.StartTime)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.Progress)
                .ToListAsync();

            // =====================================================
            // 6. Recent Grades
            // =====================================================

            // AssessmentAttempts is not registered in the current
            // ApplicationDbContext, so no fake grades are returned.

            var recentGrades = new List<StudentDashboardRecentGradeDto>();

            // =====================================================
            // 7. Final Dashboard Response
            // =====================================================

            return new GetStudentDashboardDto
            {
                StudentName = studentName ?? string.Empty,

                Stats = new StudentDashboardStatsDto
                {
                    EnrolledCourses = enrolledCourses,
                    LiveClasses = liveClasses,
                    Certificates = certificates,
                    AssignmentsDue = 0
                },

                Courses = courses,

                RecentGrades = recentGrades
            };
        }

        public async Task<List<StudentAssessmentListDto>> GetMyAssessmentsAsync(string studentId)
        {
            var now = DateTime.Now;

            var enrolledBatchIds = _context.Enrollments
                .AsNoTracking()
                .Where(x =>
                    x.StudentId == studentId &&
                    x.IsActive &&
                    x.BatchId != null &&
                    (x.PaymentStatus == PaymentStatus.Free.ToString() ||
                     x.PaymentStatus == PaymentStatus.Success.ToString()))
                .Select(x => x.BatchId.Value);

            var assessments = await _context.Assessments
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.BatchId != null &&
                    enrolledBatchIds.Contains(x.BatchId.Value))
                .Select(x => new StudentAssessmentListDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,

                    CourseId = x.CourseId,
                    CourseName = x.course != null
                        ? x.course.Title
                        : null,

                    BatchId = x.BatchId,
                    BatchName = x.batch != null
                        ? x.batch.Name
                        : null,

                    TotalMarks = x.TotalMarks ?? 0,
                    PassingMarks = x.PassingMarks ?? 0,
                    DurationInMinutes = x.DurationInMinutes ?? 0,

                    TotalQuestions = _context.AssessmentQuestions
                        .Count(q => q.AssessmentId == x.Id && q.IsActive),

                    StartAt = x.StartAt,
                    EndAt = x.EndAt,

                    Status = x.EndAt.HasValue && x.EndAt.Value < now
                        ? AssessmentStatus.Close.ToString()
                        : x.StartAt > now
                            ? AssessmentStatus.Upcomming.ToString()
                            : AssessmentStatus.Available.ToString(),

                    HasAttempt = _context.AssessmentAttempts.Any(a =>
                        a.AssessmentId == x.Id &&
                        a.StudentId == studentId),

                    AttemptId = _context.AssessmentAttempts
                        .Where(a =>
                            a.AssessmentId == x.Id &&
                            a.StudentId == studentId &&
                            a.IsActive)
                        .OrderByDescending(a => a.StartedAt)
                        .Select(a => (int?)a.Id)
                        .FirstOrDefault(),

                    IsSubmitted = _context.AssessmentAttempts.Any(a =>
                        a.AssessmentId == x.Id &&
                        a.StudentId == studentId &&
                        a.SubmittedAt != null &&
                        a.IsActive)
                })
                .OrderBy(x => x.StartAt)
                .ToListAsync();

            return assessments;
        }

        public async Task<StudentAssessmentDetailsDto?> GetAssessmentDetailsAsync(int assessmentId, string studentId)
        {
            var enrolledBatchIds = _context.Enrollments
                   .AsNoTracking()
                   .Where(x =>
                       x.StudentId == studentId &&
                       x.IsActive &&
                       x.BatchId != null &&
                       (x.PaymentStatus == PaymentStatus.Free.ToString() ||
                        x.PaymentStatus == PaymentStatus.Success.ToString()))
                   .Select(x => x.BatchId.Value);

            var assessment = await _context.Assessments
                .AsNoTracking()
                .Where(x =>
                    x.Id == assessmentId &&
                    x.IsActive &&
                    x.BatchId != null &&
                    enrolledBatchIds.Contains(x.BatchId.Value))
                .Select(x => new StudentAssessmentDetailsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,

                    TotalMarks = x.TotalMarks ?? 0,
                    PassingMarks = x.PassingMarks ?? 0,
                    DurationInMinutes = x.DurationInMinutes ?? 0,

                    StartAt = x.StartAt,
                    EndAt = x.EndAt,

                    TotalQuestions = _context.AssessmentQuestions
                        .Count(q => q.AssessmentId == x.Id && q.IsActive),

                    Questions = _context.AssessmentQuestions
                        .Where(q =>
                            q.AssessmentId == x.Id &&
                            q.IsActive)
                        .OrderBy(q => q.DisplayOrder)
                        .Select(q => new StudentAssessmentQuestionDto
                        {
                            Id = q.Id,
                            QuestionText = q.QuestionText,
                            QuestionType = q.QuestionType,
                            Marks = q.Marks,
                            DisplayOrder = q.DisplayOrder,

                            Options = _context.QuestionOptions
                                .Where(o =>
                                    o.QuestionId == q.Id &&
                                    o.IsActive)
                                .OrderBy(o => o.Id)
                                .Select(o => new StudentQuestionOptionDto
                                {
                                    Id = o.Id,
                                    OptionText = o.OptionText
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return assessment;
        }

        public async Task<Assessments?> GetAssessmentByIdAsync(int assessmentId)
        {

            return await _context.Assessments.FirstOrDefaultAsync(x =>  x.Id == assessmentId && x.IsActive);
        }

        public async Task<AssessmentAttempts?> GetInProgressAssessmentAttemptAsync(int assessmentId, string studentId)
        {
            return await _context.AssessmentAttempts.FirstOrDefaultAsync(x => x.AssessmentId == assessmentId && x.StudentId == studentId &&  x.SubmittedAt == null &&  x.IsActive);
        }

        public async Task<AssessmentAttempts?> GetAssessmentAttemptForSubmitAsync(int attemptId, string studentId)
        {
            return await _context.AssessmentAttempts.FirstOrDefaultAsync(x =>  x.Id == attemptId && x.StudentId == studentId && x.IsActive);
        }

        public async Task CreateAssessmentAttemptAsync(AssessmentAttempts attempt)
        {
            await _context.AssessmentAttempts.AddAsync(attempt);
        }

        public async Task<List<AssessmentQuestions>> GetAssessmentQuestionsAsync(int assessmentId)
        {
            return await _context.AssessmentQuestions.AsNoTracking().Where(x => x.AssessmentId == assessmentId &&  x.IsActive).ToListAsync();
        }

        public async Task CreateAssessmentAnswersAsync(List<AssessmentAnswers> answers)
        {
            await _context.AssessmentAnswers.AddRangeAsync(answers);
        }

        public async Task<List<StudentAssessmentResultDto>> GetMyAssessmentResultsAsync(string studentId)
        {
            return await _context.AssessmentAttempts.AsNoTracking().Where(x => x.StudentId == studentId && x.IsActive &&  x.SubmittedAt != null)
                  .OrderByDescending(x => x.SubmittedAt)
                  .Select(x => new StudentAssessmentResultDto
                  {
                      AttemptId = x.Id,
                      AssessmentId = x.AssessmentId ?? 0,

                      AssessmentTitle = x.assessmentId != null
                          ? x.assessmentId.Title
                          : string.Empty,

                      TotalMarks = x.assessmentId != null
                          ? x.assessmentId.TotalMarks ?? 0
                          : 0,

                      ObtainedMarks = x.ObtainedMarks,
                      Percentage = x.Percentage,

                      PassingMarks = x.assessmentId != null
                          ? x.assessmentId.PassingMarks ?? 0
                          : 0,

                      IsPassed = x.IsPassed,

                      StartedAt = x.StartedAt,
                      SubmittedAt = x.SubmittedAt
                  })
                  .ToListAsync();
        }

        public async Task<List<QuestionOptions>> GetQuestionOptionsAsync(int questionId)
        {
            return await _context.QuestionOptions.AsNoTracking().Where(x => x.QuestionId == questionId && x.IsActive).ToListAsync();
        }
    }
}

