using Core.Dtos.Account;
using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Teacher.Attendance;
using Core.Dtos.Teacher.Batch;
using Core.Interface.Repository;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;
        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAttendanceRangeAsync(List<BatchSessionAttendance> attendance)
        {
            if (attendance.Count == 0)
                return;

            await _context.BatchSessionAttendances.AddRangeAsync(attendance);
        }

        public async Task<bool> BatchExistsAsync(int batchId)
        {
            return await _context.Batches.AsNoTracking().AnyAsync(x => x.Id == batchId);
        }

        public async Task CreateAsync(BatchTeacher batchTeacher)
        {
            await _context.BatchTeachers.AddAsync(batchTeacher);
        }

        public async Task CreateBatchTeacherAsync(BatchTeacher entity)
        {
            await _context.BatchTeachers.AddAsync(entity);
        }

        public async Task<BatchTeacher?> GetByBatchAndTeacherIdAsync(int batchId, string teacherId)
        {
            return await _context.BatchTeachers.FirstOrDefaultAsync(x => x.BatchId == batchId && x.TeacherId == teacherId && x.IsActive);
        }

        public async Task<List<GetBatchTeacherDto>> GetByBatchIdAsync(int batchId)
        {
            return await _context.BatchTeachers.AsNoTracking().Where(x => x.BatchId == batchId && x.IsActive)
                .Select(x => new GetBatchTeacherDto
                {
                    Id = x.Id,
                    BatchId = x.BatchId ?? 0,
                    TeacherId = x.TeacherId,
                    TeacherName = x.user.UserName ?? string.Empty,
                    TeacherEmail = x.user.Email,
                    ProfileImageUrl = _context.TeacherProfiles.Where(g => g.UserId == x.TeacherId).Select(g => g.ProfileImagePublicId).FirstOrDefault(),
                    IsPrimary = x.IsPrimary,
                    IsActive = x.IsActive,
                    AssignedAt = x.AssignedAt
                })
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.TeacherName)
                .ToListAsync();
        }

        public async Task<GetBatchTeacherDto?> GetByIdAsync(int id)
        {
            return await _context.BatchTeachers.AsNoTracking().Where(x => x.Id == id)
                .Select(x => new GetBatchTeacherDto
                {
                    Id = x.Id,
                    BatchId = x.BatchId ?? 0,
                    TeacherId = x.TeacherId,
                    TeacherName = x.user.UserName ?? string.Empty,
                    TeacherEmail = x.user.Email,
                    IsPrimary = x.IsPrimary,
                    IsActive = x.IsActive,
                    AssignedAt = x.AssignedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<GetTeacherBatchDto?> GetByIdBatchTeacherAsync(int Id)
        {
            return await _context.BatchTeachers.AsNoTracking().Include(x => x.user).Include(x => x.batch).Where(x => x.Id == Id && x.IsActive)
              .Select(x => new GetTeacherBatchDto
              {
                  Id = x.Id,
                  BatchId = x.BatchId,
                  BatchName = x.batch.Name,
                  TeacherId = x.TeacherId,
                  TeacherName = x.user.FullName,
                  IsPrimary = x.IsPrimary,
                  IsActive = x.IsActive,
              }).FirstOrDefaultAsync();
        }

        public async Task<BatchTeacher?> GetByIdBatchTeacherUpdateAsync(int Id)
        {
            return await _context.BatchTeachers.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<List<string>> GetEnrolledStudentIdsAsync(int batchId, List<string> studentIds)
        {
            return await _context.Enrollments.AsNoTracking().Where(x => studentIds.Contains(x.StudentId) && x.BatchId == batchId && x.IsActive && (x.PaymentStatus == PaymentStatus.Free.ToString() || x.PaymentStatus == PaymentStatus.Success.ToString())).Select(x => x.StudentId).Distinct().ToListAsync();
        }

        public async Task<List<BatchSessionAttendance>> GetExistingAttendanceAsync(List<int> sessionIds, List<string> studentIds)
        {
            return await _context.BatchSessionAttendances.Where(x => sessionIds.Contains(x.BatchClassSessionId.Value) && studentIds.Contains(x.StudentId)).ToListAsync();
        }

        public async Task<PageResultDto<GetAdminTeacherAssignDto>> GetListAdminTeacherAssignAsync(int pageNumber, int pageSize, AdminTeacherAssingStatus status)
        {
            var today = DateTime.Today;
            var query = _context.Batches.AsNoTracking().Include(x => x.course)
              .Select(x => new GetAdminTeacherAssignDto
              {
                  BatchId = x.Id,
                  BatchName = x.Name,
                  CourseId = x.CourseId ?? 0,
                  CourseName = x.course.Title,
                  StartDate = x.StartDate,
                  EndDate = x.EndDate,
                  WeekOfDay = string.Join(",", _context.BatchSchedules.Where(g => g.BatchId == x.Id).Select(g => g.DayOfWeek)),
                  TeacherId = _context.BatchTeachers.Where(g => g.BatchId == x.Id && g.IsPrimary && g.IsActive).Select(g => g.TeacherId.ToString()).FirstOrDefault(),
                  TeacherName = _context.BatchTeachers.Where(g => g.BatchId == x.Id && g.IsPrimary && g.IsActive).Select(g => g.user.FullName).FirstOrDefault(),
                  IsActive = x.IsActive
              });
            switch (status)
            {
                case AdminTeacherAssingStatus.All:
                    break;

                case AdminTeacherAssingStatus.Active:
                    query = query.Where(x => x.IsActive && x.StartDate <= today && x.EndDate >= today);
                    break;

                case AdminTeacherAssingStatus.Draft:
                    query = query.Where(x => !x.IsActive);
                    break;

                case AdminTeacherAssingStatus.Completed:
                    query = query.Where(x => x.EndDate < today);
                    break;

                case AdminTeacherAssingStatus.Upcomming:
                    query = query.Where(x => x.StartDate > today);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status));

            }
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetAdminTeacherAssignDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetTeacherBatchDto>?> GetListBatchTeacherAsync(int pageNumber, int pageSize)
        {
            var query = _context.BatchTeachers.AsNoTracking().Include(x => x.user).Include(x => x.batch)
               .Select(x => new GetTeacherBatchDto
               {
                   Id = x.Id,
                   BatchId = x.BatchId,
                   BatchName = x.batch.Name,
                   TeacherId = x.TeacherId,
                   TeacherName = x.user.FullName,
                   IsPrimary = x.IsPrimary,
                   AssignedAt = x.AssignedAt,
                   IsActive = x.IsActive,
               });
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetTeacherBatchDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<GetListTeacherIdNameDto>> GetListTeacherIdNameAsync()
        {
            var data = await (from user in _context.Users
                              join userRole in _context.UserRoles on user.Id equals userRole.UserId
                              join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                              join teacher in _context.TeacherProfiles
                                  on user.Id equals teacher.UserId
                              where role.Name == RoleData.Trainer.ToString()
                                    && teacher.IsActive
                              select new GetListTeacherIdNameDto
                              {
                                  Id = user.Id,
                                  Name = user.FullName
                              }).Distinct().AsNoTracking().ToListAsync();
            return data;
        }

        public async Task<PageResultDto<GetActiveTeacherBatchDto>> GetMyBatchesAsync(string teacherId, PaginationRequestDto pagination, TeacherBatchFilterDto filter)
        {
            // =========================================================
            // 1. Normalize pagination
            // =========================================================

            var pageNumber = pagination?.PageNumber <= 0
                ? 1
                : pagination?.PageNumber ?? 1;

            var pageSize = pagination?.PageSize <= 0
                ? 10
                : Math.Min(pagination?.PageSize ?? 10, 100);


            // =========================================================
            // 2. Get batches assigned to current teacher
            // =========================================================

            var teacherBatchIds = _context.BatchTeachers
                .AsNoTracking()
                .Where(x =>
                    x.TeacherId == teacherId &&
                    x.IsActive)
                .Select(x => x.BatchId)
                .Distinct();


            // =========================================================
            // 3. Base Batch Query
            // =========================================================

            var query = _context.Batches
                .AsNoTracking()
                .Where(x =>
                    teacherBatchIds.Contains(x.Id) &&
                    x.IsPublished &&
                    x.IsActive);


            // =========================================================
            // 4. Status Filter
            //
            // Active
            // Upcoming
            // Completed
            // Cancelled
            // =========================================================

            if (!string.IsNullOrWhiteSpace(filter?.Status))
            {
                var status = filter.Status.Trim();

                query = query.Where(x =>
                    x.Status == status);
            }


            // =========================================================
            // 5. Total Count Before Pagination
            // =========================================================

            var totalCount = await query.CountAsync();


            // =========================================================
            // 6. Pagination + Basic Batch Information
            // =========================================================

            var batches = await query
                .OrderBy(x => x.StartDate)
                .ThenBy(x => x.Name)

                .Skip((pageNumber - 1) * pageSize)

                .Take(pageSize)

                .Select(batch => new
                {
                    batch.Id,

                    batch.Name,

                    CourseId = batch.CourseId ?? 0,

                    CourseName = batch.course != null
                        ? batch.course.Title
                        : string.Empty,

                    batch.BatchCode,

                    Capacity = batch.Capacity ?? 0,

                    StartDate = batch.StartDate ?? default,

                    EndDate = batch.EndDate ?? default,

                    batch.Status
                })

                .ToListAsync();


            // =========================================================
            // 7. No Data
            // =========================================================

            if (batches.Count == 0)
            {
                return new PageResultDto<GetActiveTeacherBatchDto>
                {
                    Items = new List<GetActiveTeacherBatchDto>(),

                    PageNumber = pageNumber,

                    PageSize = pageSize,

                    TotalCount = totalCount
                };
            }


            // =========================================================
            // 8. Current Page Batch IDs
            // =========================================================

            var batchIds = batches
                .Select(x => x.Id)
                .ToList();


            // =========================================================
            // 9. Enrollment Summary
            // =========================================================

            var enrollmentSummary = await _context.Enrollments

                .AsNoTracking()

                .Where(x =>
                    x.BatchId.HasValue &&
                    batchIds.Contains(x.BatchId.Value) &&
                    x.IsActive)

                .GroupBy(x => x.BatchId.Value)

                .Select(g => new
                {
                    BatchId = g.Key,

                    StudentCount = g.Count(),

                    CourseProgress = g
                        .Select(x => (decimal?)x.CompletionPercentage)
                        .Average() ?? 0
                })

                .ToDictionaryAsync(x => x.BatchId);


            // =========================================================
            // 10. Next Class
            // =========================================================

            var today = DateTime.Now.Date;

            var nextClasses = await _context.BatchClassSessions

                .AsNoTracking()

                .Where(session =>
                    session.BatchId.HasValue &&
                    batchIds.Contains(session.BatchId.Value) &&
                    session.SessionDate >= today &&
                    session.Status == "Scheduled")

                .OrderBy(session => session.SessionDate)

                .ThenBy(session => session.StartTime)

                .Select(session => new
                {
                    BatchId = session.BatchId.Value,

                    SessionId = session.Id,

                    session.SessionDate,

                    session.StartTime,

                    EndTime = session.EndTime ?? default,

                    // =====================================================
                    // Class Type
                    // =====================================================

                    ClassType = _context.BatchSchedules

                        .Where(schedule =>
                            schedule.Id == session.BatchScheduleId)

                        .Select(schedule =>
                            schedule.ClassType)

                        .FirstOrDefault(),

                    // =====================================================
                    // Meeting Link
                    // =====================================================

                    MeetingLink = _context.BatchSchedules

                        .Where(schedule =>
                            schedule.Id == session.BatchScheduleId)

                        .Select(schedule =>
                            schedule.MettingLink)

                        .FirstOrDefault(),

                    // =====================================================
                    // Module Name
                    // =====================================================

                    ModuleName = session.CourseModuleId.HasValue

                        ? _context.CourseModules

                            .Where(module =>
                                module.Id == session.CourseModuleId.Value)

                            .Select(module =>
                                module.Name)

                            .FirstOrDefault()

                        : null
                })

                .ToListAsync();


            // =========================================================
            // 11. Keep Only Next Class Per Batch
            // =========================================================

            var nextClassDictionary = nextClasses

                .GroupBy(x => x.BatchId)

                .ToDictionary(
                    g => g.Key,
                    g => g.First());


            // =========================================================
            // 12. Final DTO Mapping
            // =========================================================

            var items = batches

                .Select(batch =>
                {
                    enrollmentSummary.TryGetValue(
                        batch.Id,
                        out var enrollment);


                    nextClassDictionary.TryGetValue(
                        batch.Id,
                        out var nextClass);


                    return new GetActiveTeacherBatchDto
                    {
                        // =================================================
                        // Batch Information
                        // =================================================

                        Id = batch.Id,

                        Name = batch.Name,

                        CourseId = batch.CourseId,

                        CourseName = batch.CourseName,

                        BatchCode = batch.BatchCode,


                        // =================================================
                        // Student Information
                        // =================================================

                        StudentCount =
                            enrollment?.StudentCount ?? 0,

                        Capacity =
                            batch.Capacity,


                        // =================================================
                        // Progress
                        // =================================================

                        CourseProgress = Math.Round(
                            enrollment?.CourseProgress ?? 0,
                            2),


                        // =================================================
                        // Attendance
                        // =================================================
                        // Actual attendance aggregate can be added once
                        // ClassAttendance relationship is finalized.
                        // =================================================

                        AttendancePercentage = 0,


                        // =================================================
                        // Batch Dates
                        // =================================================

                        StartDate = batch.StartDate,

                        EndDate = batch.EndDate,


                        // =================================================
                        // Status
                        // =================================================

                        Status = batch.Status,


                        // =================================================
                        // Next Class
                        // =================================================

                        NextClass = nextClass == null

                            ? null

                            : new GetTeacherNextClassDto
                            {
                                SessionId =
                                    nextClass.SessionId,

                                SessionDate =
                                    nextClass.SessionDate,

                                StartTime =
                                    nextClass.StartTime,

                                EndTime =
                                    nextClass.EndTime,

                                ClassType =
                                    nextClass.ClassType
                                    ?? string.Empty,

                                MeetingLink =
                                    nextClass.MeetingLink,

                                ModuleName =
                                    nextClass.ModuleName
                            },


                        // =================================================
                        // Assignment
                        // =================================================

                        PendingAssignmentCount = 0,


                        // =================================================
                        // Assessment
                        // =================================================

                        PendingAssessmentCount = 0
                    };
                })

                .ToList();


            // =========================================================
            // 13. Final Paginated Response
            // =========================================================

            return new PageResultDto<GetActiveTeacherBatchDto>
            {
                Items = items,

                PageNumber = pageNumber,

                PageSize = pageSize,

                TotalCount = totalCount
            };
        }

        public async Task<GetListParticularStudentSummeryDto> GetParticularStudnetAttendanceSummeryAsync(int batchId, string studentId)
        {
            // 1. Validate student enrollment
            var isEnrolled = await _context.Enrollments.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.StudentId == studentId && x.IsActive && (x.PaymentStatus == PaymentStatus.Free.ToString() || x.PaymentStatus == PaymentStatus.Success.ToString()));

            if (!isEnrolled)
                throw new KeyNotFoundException("Student is not enrolled in this batch.");

            // 2. Get student profile
            var student = await _context.StudentProfiles.AsNoTracking().Where(x => x.UserId == studentId && x.IsActive)
                .Select(x => new
                {
                    StudentId = x.UserId,
                    StudentName = x.user.FullName,
                    ProfileImage = x.ProfileImageUrl
                }).FirstOrDefaultAsync();

            if (student == null)
                throw new KeyNotFoundException("Student profile not found.");

            // 3. Get all active batch sessions
            var sessions = await _context.BatchClassSessions.AsNoTracking().Where(x =>
                    x.BatchId == batchId &&
                    x.IsActive).OrderByDescending(x => x.SessionDate).ThenByDescending(x => x.StartTime)
                .Select(x => new
                {
                    x.Id,
                    x.SessionDate,
                    x.StartTime,
                    x.EndTime
                })
                .ToListAsync();

            var sessionIds = sessions.Select(x => x.Id).ToList();

            // 4. Get student's attendance records
            var attendanceRecords = await _context.BatchSessionAttendances.AsNoTracking().Where(x => x.StudentId == studentId && x.IsActive && x.BatchClassSessionId.HasValue && sessionIds.Contains(x.BatchClassSessionId.Value))
                .Select(x => new
                {
                    x.Id,
                    SessionId = x.BatchClassSessionId!.Value,
                    x.Status,
                    x.Remarks,
                    x.MarkedAt
                }).ToListAsync();

            // 5. Handle multiple records per session
            // Ideally database should enforce one active record per student/session.
            var attendanceDictionary = attendanceRecords.GroupBy(x => x.SessionId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.MarkedAt).ThenByDescending(x => x.Id).First());

            // 6. Map all sessions with attendance
            var sessionHistory = sessions
                .Select(session =>
                {
                    attendanceDictionary.TryGetValue(
                        session.Id,
                        out var attendance);

                    return new GetParticularStudentAttendanceHistoryDto
                    {
                        AttendanceId = attendance?.Id ?? 0,
                        SessionId = session.Id,
                        SessionDate = session.SessionDate,
                        StartTime = session.StartTime,
                        EndTime = session.EndTime,

                        Status = attendance?.Status ?? "Not Marked",

                        Remarks = attendance?.Remarks,

                        MarkedAt = attendance?.MarkedAt
                    };
                })
                .ToList();

            // 7. Calculate summary
            var totalClasses = sessionHistory.Count;

            var present = sessionHistory.Count(x => x.Status == AttendanceStatus.Present.ToString());

            var absent = sessionHistory.Count(x => x.Status == AttendanceStatus.Absent.ToString());

            var notMarked = sessionHistory.Count(x => x.Status == "Not Marked");

            var averageAttendance = totalClasses > 0 ? Math.Round((decimal)present / totalClasses * 100, 2) : 0;

            // 8. Return final response
            return new GetListParticularStudentSummeryDto
            {
                StudentId = student.StudentId!,
                StudentName = student.StudentName,
                StudentProfilePic = student.ProfileImage,

                TotalClasses = totalClasses,
                Present = present,
                Absent = absent,
                NotMarked = notMarked,

                AverageAttendance = averageAttendance,

                Summary = sessionHistory
            };
        }

        public async Task<List<BatchClassSession>> GetSessionsByIdsAsync(List<int> sessionIds)
        {
            return await _context.BatchClassSessions.AsNoTracking().Where(x => sessionIds.Contains(x.Id) && x.IsActive).ToListAsync();
        }

        public async Task<GetStudentAttendanceSummeryDto> GetStudnetAttendanceSummeryAsync(int batchId)
        {
            var totalStudent = await _context.Enrollments.CountAsync(x => x.BatchId == batchId && x.IsActive && (x.PaymentStatus == PaymentStatus.Free.ToString() || x.PaymentStatus == PaymentStatus.Success.ToString()));

            var totalClasses = await _context.BatchClassSessions.CountAsync(x => x.BatchId == batchId && x.IsActive);

            var totalPresent = await _context.BatchSessionAttendances.CountAsync(x => x.Status == AttendanceStatus.Present.ToString() && x.BatchClassSession.BatchId == batchId && x.IsActive);

            var today = DateTime.Today;

            var tomorrow = today.AddDays(1);

            var presentToday = await _context.BatchSessionAttendances.CountAsync(x => x.MarkedAt >= today && x.MarkedAt < tomorrow && x.Status == AttendanceStatus.Present.ToString() && x.BatchClassSession.BatchId == batchId && x.IsActive);

            var absentToday = await _context.BatchSessionAttendances.CountAsync(x => x.MarkedAt >= today && x.MarkedAt < tomorrow && x.Status == AttendanceStatus.Absent.ToString() && x.BatchClassSession.BatchId == batchId && x.IsActive);

            decimal totalPossibleAttendance = (decimal)totalStudent * totalClasses;

            decimal batchAverageAttendance = totalPossibleAttendance > 0 ? Math.Round((decimal)totalPresent / totalPossibleAttendance * 100, 2) : 0;

            var studentAttendance = await _context.Enrollments.Where(x => x.BatchId == batchId && x.IsActive && (x.PaymentStatus == PaymentStatus.Free.ToString() || x.PaymentStatus == PaymentStatus.Success.ToString()))
      .Select(student => new GetListStudnetAttendanceDto
      {

          StudentId = student.StudentId,

          StudentName = student.user.FullName,

          StudnetProfileImage = _context.StudentProfiles.Where(p => p.UserId == student.StudentId && p.IsActive).Select(p => p.ProfileImageUrl).FirstOrDefault(),

          Present = _context.BatchSessionAttendances.Count(a => a.StudentId == student.StudentId && a.IsActive && a.Status == AttendanceStatus.Present.ToString() && a.BatchClassSession.BatchId == batchId),

          Absent = _context.BatchSessionAttendances.Count(a => a.StudentId == student.StudentId && a.IsActive && a.Status == AttendanceStatus.Absent.ToString() && a.BatchClassSession.BatchId == batchId),

          AverageAttendance = totalClasses > 0 ? Math.Round((decimal)_context.BatchSessionAttendances.Count(a => a.StudentId == student.StudentId && a.IsActive && a.Status == AttendanceStatus.Present.ToString() && a.BatchClassSession.BatchId == batchId) / totalClasses * 100, 2) : 0
      }).ToListAsync();
            // 8. Return summary
            return new GetStudentAttendanceSummeryDto
            {
                BatchId = batchId,
                TotalStudent = totalStudent,
                BatchAverageAttendance = batchAverageAttendance,
                PresentToday = presentToday,
                AbsentToday = absentToday,
                ParticularAttendnace = studentAttendance
            };
        }

        public async Task<PageResultDto<GetBatchClassSessionDto>> GetTeacherBatchClassSessionsAsync(string teacherId, int? batchId, int pageNumber, int pageSize)
        {
            // 1. Base query
            var query = _context.BatchClassSessions
                .AsNoTracking()
                .Where(x =>
                    x.TeacherId == teacherId &&
                    x.IsActive);

            // 2. Optional batch filter
            if (batchId.HasValue && batchId.Value > 0)
            {
                query = query.Where(x => x.BatchId == batchId.Value);
            }

            // 3. Total records
            var totalRecords = await query.CountAsync();

            // 4. Pagination + DTO projection
            var items = await query
                .OrderByDescending(x => x.SessionDate)
                .ThenByDescending(x => x.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetBatchClassSessionDto
                {
                    BatchClassSessionId = x.Id,

                    BatchId = x.BatchId.Value,
                    BatchName = x.batch.Name,

                    BatchScheduleId = x.BatchScheduleId.Value,

                    SessionDate = x.SessionDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,

                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher != null
                        ? x.Teacher.FullName
                        : null,

                    CourseModuleId = x.CourseModuleId,

                    Status = x.Status,

                    IsReschedule = x.IsReschedule,
                    OriginalSessionDate = x.OriginalSessionDate,
                    OriginalStartTime = x.OriginalStartTime,
                    OriginalEndTime = x.OriginalEndTime,
                    RescheduleReason = x.RescheduleReason,

                    IsActive = x.IsActive
                })
                .ToListAsync();

            // 5. Response
            return new PageResultDto<GetBatchClassSessionDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
            };
        }

        public async Task<GetTeacherBatchOverviewDto> GetTeacherBatchOverViewAsync(int batchId, string teacherId)
        {
            // =========================================
            // 1. Validate Batch and Teacher Assignment
            // =========================================

            if (batchId <= 0 || string.IsNullOrWhiteSpace(teacherId))
                return null;

            var batch = await _context.Batches
                .AsNoTracking()
                .Where(b =>
                    b.Id == batchId &&
                    b.IsPublished &&
                    b.IsActive &&
                    _context.BatchTeachers.Any(bt =>
                        bt.BatchId == b.Id &&
                        bt.TeacherId == teacherId &&
                        bt.IsActive))
                .Select(b => new TeacherBatchBasicInfoDto
                {
                    BatchId = b.Id,
                    BatchName = b.Name,
                    BatchCode = b.BatchCode,
                    CourseId = b.CourseId ?? 0,
                    CourseName = b.course!.Title,
                    StartDate = b.StartDate ?? default,
                    EndDate = b.EndDate ?? default,
                    BatchStatus = b.Status,
                    Capacity = b.Capacity ?? 0
                })
                .FirstOrDefaultAsync();

            if (batch == null)
                return null;



            // =========================================
            // 2. Enrollment and Student Count
            // =========================================

            var enrollmentQuery = _context.Enrollments
                .AsNoTracking()
                .Where(e =>
                    e.BatchId == batchId &&
                    e.StudentId != null);

            var totalStudents = await enrollmentQuery
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();


            // =========================================
            // 3. Average Course Progress
            // =========================================

            var courseProgress = await _context.Enrollments
      .AsNoTracking()
      .Where(e =>
          e.BatchId == batchId &&
          e.StudentId != null &&
          e.CompletionPercentage.HasValue)
      .Select(e => (decimal?)e.CompletionPercentage)
      .AverageAsync() ?? 0m;


            // =========================================
            // 4. Next Live Class
            // =========================================

            var now = DateTime.Now;

            var nextClass = await _context.LiveClasses
                .AsNoTracking()
                .Where(x =>
                    x.BatchId == batchId &&
                    x.ScheduledStartAt >= now)
                .OrderBy(x => x.ScheduledStartAt)
                .Select(x => new TeacherNextLiveClassDto
                {
                    SessionId = x.Id,

                    Topic = x.Title,

                    ModuleName = x.module != null
                        ? x.module.Name
                        : null,

                    SessionDate = x.ScheduledStartAt.Date,

                    StartTime = x.ScheduledStartAt.TimeOfDay,

                    EndTime = x.ScheduledEndAt.HasValue
                        ? x.ScheduledEndAt.Value.TimeOfDay
                        : default,

                    ClassType = x.MeetingProvider ?? string.Empty,

                    MeetingLink = x.MeetingUrl
                })
                .FirstOrDefaultAsync();


            // =========================================
            // 5. Get Batch Live Classes
            // =========================================

            var liveClasses = await _context.LiveClasses
                .AsNoTracking()
                .Where(x => x.BatchId == batchId)
                .OrderBy(x => x.ScheduledStartAt)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.ScheduledStartAt,
                    x.ScheduledEndAt
                })
                .ToListAsync();

            var liveClassIds = liveClasses
                .Select(x => x.Id)
                .ToList();


            // =========================================
            // 6. Get Attendance Records
            // =========================================

            var studentIdsQuery = enrollmentQuery
                .Select(e => e.StudentId)
                .Distinct();

            var attendanceRecords = await _context.ClassAttendances
                .AsNoTracking()
                .Where(x =>
                    x.StudentId != null &&
                    studentIdsQuery.Contains(x.StudentId) &&
                    x.LiveClassId.HasValue &&
                    liveClassIds.Contains(x.LiveClassId.Value))
                .Select(x => new
                {
                    x.StudentId,
                    LiveClassId = x.LiveClassId!.Value,
                    x.JoinedAt,
                    x.LeftAt,
                    x.DurationInSeconds,
                    x.Status
                })
                .ToListAsync();

            var courseSyllabus = await _context.CourseSyllabus.AsNoTracking().Where(x => x.CourseId == batch.CourseId).Select(x => new GetCourseSyllabusDto
            {
                Id = x.Id,
                Title = x.Title,
                CouseId = x.CourseId ?? 0,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder ?? 0
            }).ToListAsync();



            // =========================================
            // 7. Aggregate Multiple Join Records
            // =========================================

            var attendanceGroups = attendanceRecords
                .GroupBy(x => new
                {
                    x.StudentId,
                    x.LiveClassId
                })
                .Select(g => new
                {
                    g.Key.StudentId,
                    g.Key.LiveClassId,

                    TotalSeconds = g.Sum(x => x.DurationInSeconds),

                    FirstJoinTime = g.Min(x => x.JoinedAt),

                    LastLeaveTime = g
                        .Where(x => x.LeftAt.HasValue)
                        .Select(x => x.LeftAt)
                        .Max(),

                    Status = g.Any(x =>
                        string.Equals(
                            x.Status,
                            "Present",
                            StringComparison.OrdinalIgnoreCase))
                                ? "Present"
                                : g.Any(x =>
                                    string.Equals(
                                        x.Status,
                                        "Late",
                                        StringComparison.OrdinalIgnoreCase))
                                    ? "Late"
                                    : g.Select(x => x.Status)
                                        .FirstOrDefault() ?? "Absent"
                })
                .ToList();


            // =========================================
            // 8. Prepare Student Attendance Summary
            // =========================================

            // Only classes whose scheduled end time has passed
            // are considered eligible for attendance.

            var eligibleClasses = liveClasses
                .Where(x =>
                    x.ScheduledEndAt.HasValue &&
                    x.ScheduledEndAt.Value <= now)
                .ToList();

            var eligibleClassIds = eligibleClasses
                .Select(x => x.Id)
                .ToHashSet();

            var students = await enrollmentQuery
                .Where(e => e.StudentId != null)
                .Select(e => new
                {
                    e.StudentId,
                    StudentName = e.user != null
                        ? e.user.UserName
                        : "Unknown Student"
                })
                .Distinct()
                .ToListAsync();

            var attendanceList = students
                .Select(student =>
                {
                    var studentRecords = attendanceGroups
                        .Where(a => a.StudentId == student.StudentId)
                        .ToList();

                    var presentCount = studentRecords
                        .Where(a =>
                            eligibleClassIds.Contains(a.LiveClassId) &&
                            (a.Status == "Present" || a.Status == "Late"))
                        .Select(a => a.LiveClassId)
                        .Distinct()
                        .Count();

                    var lateCount = studentRecords
                        .Where(a =>
                            eligibleClassIds.Contains(a.LiveClassId) &&
                            a.Status == "Late")
                        .Select(a => a.LiveClassId)
                        .Distinct()
                        .Count();

                    var absentCount = Math.Max(
                        eligibleClasses.Count - presentCount,
                        0);

                    var percentage = eligibleClasses.Count > 0
                        ? Math.Round(
                            (decimal)presentCount /
                            eligibleClasses.Count * 100m,
                            2)
                        : 0m;

                    var sessions = eligibleClasses
                        .Select(liveClass =>
                        {
                            var attendance = studentRecords
                                .FirstOrDefault(a =>
                                    a.LiveClassId == liveClass.Id);

                            return new TeacherStudentAttendanceSessionDto
                            {
                                SessionId = liveClass.Id,

                                ScheduleDate = liveClass.ScheduledStartAt,

                                AttendanceStatus = attendance?.Status
                                    ?? "Absent",

                                TotalMinutes = attendance != null
                                    ? (int)Math.Round(
                                        attendance.TotalSeconds / 60m)
                                    : 0,

                                FirstJoinTime = attendance?.FirstJoinTime,

                                LastLeaveTime = attendance?.LastLeaveTime
                            };
                        })
                        .ToList();

                    return new TeacherStudentAttendanceDto
                    {
                        StudentId = student.StudentId!,

                        StudentName = student.StudentName,

                        TotalClasses = eligibleClasses.Count,

                        Present = presentCount,

                        Absent = absentCount,

                        Late = lateCount,

                        AttendancePercentage = percentage,

                        Sessions = sessions
                    };
                })
                .ToList();


            // =========================================
            // 9. Overall Attendance Percentage
            // =========================================

            var totalEligibleAttendance =
                students.Count * eligibleClasses.Count;

            var totalPresentAttendance = attendanceList
                .Sum(x => x.Present);

            var overallAttendance = totalEligibleAttendance > 0
                ? Math.Round(
                    (decimal)totalPresentAttendance /
                    totalEligibleAttendance * 100m,
                    2)
                : 0m;


            // =========================================
            // 10. Get Published Batch Materials
            // =========================================

            var materials = await _context.CourseMaterials
                .AsNoTracking()
                .Where(m =>
                    m.BatchId == batchId &&
                    m.IsPublished)
                .OrderByDescending(m => m.CreateAt)
                .Select(m => new TeacherBatchMaterialDto
                {
                    Id = m.Id,

                    Name = m.Title,

                    ModuleName = m.module != null
                        ? m.module.Title
                        : null,

                    FileSize = m.FileSize ?? 0,

                    FileType = m.FileType ?? string.Empty,

                    UploadedAt = m.CreateAt,

                    FileUrl = m.FileUrl ?? string.Empty,
                    IsActive = m.IsActive
                })
                .ToListAsync();


            // =========================================
            // 11. Final Overview Response
            // =========================================

            var result = new GetTeacherBatchOverviewDto
            {
                Summary = new TeacherBatchSummaryDto
                {
                    TotalStudents = totalStudents,

                    CourseProgress = Math.Round(courseProgress, 2),

                    AttendancePercentage = overallAttendance,

                    NextClassDate = nextClass?.SessionDate
                },

                BasicInformation = batch,

                NextLiveClass = nextClass,

                Attendance = attendanceList,

                Materials = materials,
                SyllabusInformation = courseSyllabus
            };

            return result;
        }

        public async Task<bool> IsTeacherAssignedToBatchAsync(int batchId, string teacherId)
        {
            return await _context.BatchTeachers.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.TeacherId == teacherId && x.IsActive);
        }

        public async Task<bool> PrimaryTeacherExistsAsync(int batchId)
        {
            return await _context.BatchTeachers.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.IsPrimary && x.IsActive);
        }

        public async Task<bool> PrimaryTeacherExistsAsync(int batchId, string teacherId)
        {
            return await _context.BatchTeachers.AnyAsync(x => x.BatchId == batchId && x.TeacherId == teacherId && x.IsPrimary && x.IsActive);
        }

        public async Task<bool> TeacherAlreadyAssignedAsync(int batchId, string teacherId)
        {
            return await _context.BatchTeachers.AsNoTracking().AnyAsync(x => x.BatchId == batchId && x.TeacherId == teacherId && x.IsActive);
        }



        public async Task<BatchTeacher?> UpdateBatchTeacherAsync(int Id)
        {
            return await _context.BatchTeachers.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
