using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.LiveClasses;
using Core.Interface.Repository;
using Domain.Entities.Common;
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
    public class LiveClassRepository : ILiveClassRepository
    {
        private readonly ApplicationDbContext _context;
        public LiveClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateClassAttendanceAsync(ClassAttendance entity)
        {
            await _context.ClassAttendances.AddAsync(entity);
        }

        public async Task CreateCourseVideosAsync(CourseVideos entity)
        {
            await _context.CourseVideos.AddAsync(entity);
        }

        public async Task CreateLiveClassAsync(LiveClasses entity)
        {
            await _context.LiveClasses.AddAsync(entity);
        }

        public async Task CreateVideoProgressAsync(VideoProgress entity)
        {
            await _context.VideoProgresses.AddAsync(entity);
        }

        public async Task<bool> EndSessionAsync(string studentId, Guid sessionId, string reason)
        {
            var session = await _context.Set<VideoWatchSession>().FirstOrDefaultAsync(x => x.SessionId == sessionId && x.StudentId == studentId && x.Status == "Active");
            if (session == null)
                return false;
            session.Status = "Closed";
            session.StopReason = reason;
            session.EndedAt = DateTime.UtcNow;
            session.IsPlaying = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GetClassAttendanc?> GetByIdClassAttendanceAsync(int Id)
        {
            return await _context.ClassAttendances.AsNoTracking().Include(x => x.user).Include(x => x.liveClasses).Where(x => x.Id == Id && x.IsActive)
              .Select(x => new GetClassAttendanc
              {
                  Id = x.Id,
                  LiveClassId = x.LiveClassId,
                  LiveClassTitle = x.liveClasses.Title,
                  StudentId = x.StudentId,
                  StudentName = x.user.FullName,
                  JoinedAt = x.JoinedAt,
                  LeftAt = x.LeftAt ?? default,
                  DurationInSeconds = x.DurationInSeconds,
                  IsActive = x.IsActive,
              }).FirstOrDefaultAsync();
        }

        public async Task<ClassAttendance?> GetByIdClassAttendanceUpdateAsync(int Id)
        {
            return await _context.ClassAttendances.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<GetCourseVideoDto?> GetByIdCourseVideosAsync(int Id)
        {
            return await _context.CourseVideos.AsNoTracking().Include(x => x.course).Include(x => x.module).Include(x => x.batch).Where(x => x.Id == Id && x.IsActive)
            .Select(x => new GetCourseVideoDto
            {
                Id = x.Id,
                CourseId = x.CourseId,
                CourseName = x.course.Title,
                ModuleId = x.ModuleId,
                ModuleName = x.module.Title,
                BatchId = x.BatchId,
                BatchName = x.batch.Name,
                Title = x.Title,
                Description = x.Description,
                VideoUrl = x.VideoUrl,
                VideoUrlPublicId = x.VideoUrlPublicId,
                DurationInSeconds = x.DurationInSeconds,
                IsPublished = x.IsPublished,
                IsActive = x.IsActive,
            }).FirstOrDefaultAsync();
        }

        public async Task<CourseVideos?> GetByIdCourseVideosUpdateAsync(int Id)
        {
            return await _context.CourseVideos.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<GetLiveClassDto?> GetByIdLiveClassAsync(int Id)
        {
            return await _context.LiveClasses.AsNoTracking().Include(x => x.batch).Include(x => x.module).Where(x => x.Id == Id && x.IsActive)
             .Select(x => new GetLiveClassDto
             {
                 Id = x.Id,
                 BatchId = x.BatchId,
                 BatchName = x.batch.Name,
                 ModuleId = x.ModuleId,
                 ModuleName = x.module.Name,
                 Title = x.Title,
                 Description = x.Description,
                 ScheduledStartAt = x.ScheduledStartAt,
                 ScheduledEndAt = x.ScheduledEndAt,
                 MeetingProvider = x.MeetingProvider,
                 MeetingUrl = x.MeetingUrl,
                 RecordingUrl = x.RecordingUrl,
                 RecordingUrlPublicId = x.RecordingUrlPublicId,
                 IsActive = x.IsActive,
             }).FirstOrDefaultAsync();
        }

        public async Task<LiveClasses?> GetByIdLiveClassUpdateAsync(int Id)
        {
            return await _context.LiveClasses.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);

        }

        public async Task<GetVideoProgressDto?> GetByIdVideoProgressAsync(int Id)
        {

            return await _context.VideoProgresses.AsNoTracking().Include(x => x.user).Include(x => x.courseVideos).Where(x => x.Id == Id && x.IsActive)
              .Select(x => new GetVideoProgressDto
              {
                  Id = x.Id,
                  VideoId = x.VideoId,
                  VideoName = x.courseVideos.Title,
                  StudentId = x.StudentId,
                  StudentName = x.user.FullName,
                  WatchedSeconds = x.WatchedSeconds,
                  CompletionPercentage = x.CompletionPercentage,
                  LastPositionSeconds = x.LastPositionSeconds,
                  IsCompleted = x.IsCompleted,
                  LastWatchedAt = x.LastWatchedAt ?? default,
                  IsActive = x.IsActive,
              }).FirstOrDefaultAsync();
        }

        public async Task<VideoProgress?> GetByIdVideoProgressUpdateAsync(int Id)
        {
            return await _context.VideoProgresses.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
        }

        public async Task<VideoProgressResponseDto?> GetLastWatchedAsync(string studentId)
        {
            return await _context.Set<VideoProgress>().AsNoTracking().Where(x => x.StudentId == studentId).OrderByDescending(x => x.LastWatchedAt)
                .Select(x => new VideoProgressResponseDto
                {
                    VideoId = x.VideoId ?? 0,
                    LastPositionSeconds = x.LastPositionSeconds ?? 0,
                    WatchedSeconds = x.WatchedSeconds ?? 0,
                    CompletionPercentage = x.CompletionPercentage ?? 0,
                    IsCompleted = x.IsCompleted,
                    LastWatchedAt = x.LastWatchedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<PageResultDto<GetClassAttendanc>?> GetListClassAttendanceAsync(int pageNumber, int pageSize)
        {

            var query = _context.ClassAttendances.AsNoTracking().Include(x => x.user).Include(x => x.liveClasses)
               .Select(x => new GetClassAttendanc
               {
                   Id = x.Id,
                   LiveClassId = x.LiveClassId,
                   LiveClassTitle = x.liveClasses.Title,
                   StudentId = x.StudentId,
                   StudentName = x.user.FullName,
                   JoinedAt = x.JoinedAt,
                   LeftAt = x.LeftAt ?? default,
                   DurationInSeconds = x.DurationInSeconds,
                   IsActive = x.IsActive,
               });
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetClassAttendanc>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetCourseVideoDto>?> GetListCourseVideosAsync(int pageNumber, int pageSize, int? batchId)
        {

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            var query = _context.CourseVideos.Include(x => x.course).Include(x => x.batch).Include(x => x.module).AsNoTracking().Where(x => x.IsActive && x.BatchId == batchId);
            var totalRecords = await query.CountAsync();
            var data = await query.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.CreateAt).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(x => new GetCourseVideoDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    CourseName = x.course != null ? x.course.Title : null,
                    ModuleId = x.ModuleId,
                    ModuleName = x.module != null ? x.module.Title : null,
                    BatchId = x.BatchId,
                    BatchName = x.batch != null ? x.batch.Name : null,
                    Title = x.Title,
                    Description = x.Description,
                    VideoProvider = x.VideoProvider,
                    VideoUrl = x.VideoUrl,
                    VideoUrlPublicId = x.VideoUrlPublicId,
                    VideoId = x.VideoId,
                    DurationInSeconds = x.DurationInSeconds,
                    DisplayOrder = x.DisplayOrder ?? 0,
                    IsPublished = x.IsPublished,
                    IsActive = x.IsActive,
                    CreateAt = x.CreateAt
                })
                .ToListAsync();

            return new PageResultDto<GetCourseVideoDto>
            {
                Items = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
            };
        }

        public async Task<PageResultDto<GetLiveClassDto>?> GetListLiveClassAsync(int pageNumber, int pageSize)
        {

            var query = _context.LiveClasses.AsNoTracking().Include(x => x.batch).Include(x => x.module)
               .Select(x => new GetLiveClassDto
               {
                   Id = x.Id,
                   BatchId = x.BatchId,
                   BatchName = x.batch.Name,
                   ModuleId = x.ModuleId,
                   ModuleName = x.module.Name,
                   Title = x.Title,
                   Description = x.Description,
                   ScheduledStartAt = x.ScheduledStartAt,
                   ScheduledEndAt = x.ScheduledEndAt,
                   MeetingProvider = x.MeetingProvider,
                   MeetingUrl = x.MeetingUrl,
                   RecordingUrl = x.RecordingUrl,
                   RecordingUrlPublicId = x.RecordingUrlPublicId,
                   IsActive = x.IsActive,
               });
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetLiveClassDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<GetVideoProgressDto>?> GetListVideoProgressAsync(int pageNumber, int pageSize)
        {
            var query = _context.VideoProgresses.AsNoTracking().Include(x => x.user).Include(x => x.courseVideos)
             .Select(x => new GetVideoProgressDto
             {
                 Id = x.Id,
                 VideoId = x.VideoId,
                 VideoName = x.courseVideos.Title,
                 StudentId = x.StudentId,
                 StudentName = x.user.FullName,
                 WatchedSeconds = x.WatchedSeconds,
                 CompletionPercentage = x.CompletionPercentage,
                 LastPositionSeconds = x.LastPositionSeconds,
                 IsCompleted = x.IsCompleted,
                 LastWatchedAt = x.LastWatchedAt ?? default,
                 IsActive = x.IsActive,
             });
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetVideoProgressDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<VideoProgressResponseDto?> GetProgressAsync(string studentId, int videoId)
        {
            return await _context.Set<VideoProgress>().AsNoTracking().Where(x => x.StudentId == studentId && x.VideoId == videoId)
                          .Select(x => new VideoProgressResponseDto
                          {
                              VideoId = x.VideoId ?? 0,
                              LastPositionSeconds = x.LastPositionSeconds ?? 0,
                              WatchedSeconds = x.WatchedSeconds ?? 0,
                              CompletionPercentage = x.CompletionPercentage ?? 0,
                              IsCompleted = x.IsCompleted,
                              LastWatchedAt = x.LastWatchedAt
                          }).FirstOrDefaultAsync();
        }

        public async Task<GetStudentCourseVideoDto> GetStudentCourseVideosAsync(int batchId, string studentId)
        {
            var videos = await _context.CourseVideos.Include(x => x.batch).Include(x => x.course).AsNoTracking().Where(x => x.BatchId == batchId && x.IsPublished && x.IsActive).OrderBy(x => x.DisplayOrder)
       .Select(x => new GetClassRoomCourseVideoDto
       {
           VideoId = x.Id,
           BatchId = x.BatchId ?? 0,
           BatchName = x.batch != null ? x.batch.Name : string.Empty,
           CourseId = x.CourseId ?? 0,
           CourseName = x.course != null ? x.course.Title : string.Empty,
           VideoTitle = x.Title,
           VideoDescription = x.Description ?? string.Empty,
           Sequence = x.DisplayOrder ?? 0,
           Videourl = x.VideoId,
           durationSeconds = x.DurationInSeconds ?? 0,
           watchedSeconds = _context.VideoProgresses.Where(p => p.VideoId == x.Id && p.StudentId == studentId).Select(p => p.WatchedSeconds).FirstOrDefault(),
           progressPercentage = _context.VideoProgresses.Where(p => p.VideoId == x.Id && p.StudentId == studentId)
               .Select(p => p.CompletionPercentage).FirstOrDefault()
       }).ToListAsync();

            if (videos == null || !videos.Any())
            {
                return new GetStudentCourseVideoDto
                {
                    CurrentVideoId = 0,
                    NextVideoId = 0,
                    VideoDetails = new List<GetClassRoomCourseVideoDto>()
                };
            }

            // First incomplete video
            var currentVideo = videos.FirstOrDefault(x => (x.progressPercentage ?? 0) < 100);
            // If all videos are completed, select the last video
            currentVideo ??= videos.Last();
            var currentIndex = videos.FindIndex(x => x.VideoId == currentVideo.VideoId);
            var nextVideoId = currentIndex < videos.Count - 1 ? videos[currentIndex + 1].VideoId : 0;
            return new GetStudentCourseVideoDto
            {
                CurrentVideoId = currentVideo.VideoId,
                NextVideoId = nextVideoId,
                VideoDetails = videos
            };
        }

        public async Task<PageResultDto<GetStudentUpcomingLiveClassDto>> GetStudentUpcomingLiveClassesAsync(string studentId, DateTime currentDateTime, int pageNumber, int pageSize)
        {
            var query = _context.LiveClasses.AsNoTracking().Where(x => x.IsActive && x.BatchId != null && x.ScheduledStartAt >= currentDateTime &&

                       _context.Enrollments.Any(e => e.BatchId == x.BatchId && e.StudentId == studentId &&
                           e.IsActive &&
                           (
                               e.PaymentStatus == PaymentStatus.Success.ToString() || e.PaymentStatus == PaymentStatus.Free.ToString()
                           )
                       )
                   )
                   .Select(x => new GetStudentUpcomingLiveClassDto
                   {
                       Id = x.Id,

                       BatchId = x.BatchId,
                       BatchName = x.batch != null
                           ? x.batch.Name
                           : null,

                       ModuleId = x.ModuleId,
                       ModuleName = x.module != null
                           ? x.module.Name
                           : null,

                       Title = x.Title,
                       Description = x.Description,

                       ScheduledStartAt = x.ScheduledStartAt,
                       ScheduledEndAt = x.ScheduledEndAt,

                       MeetingProvider = x.MeetingProvider,
                       MeetingUrl = x.MeetingUrl
                   });

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.ScheduledStartAt)
                .ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResultDto<GetStudentUpcomingLiveClassDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> SaveProgressAsync(string studentId, SaveVideoProgressDto request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var duplicate = await _context.VideoProgressEvents.AnyAsync(x => x.EventId == request.EventId);
                if (duplicate)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                var session = await _context.VideoWatchSessions.FirstOrDefaultAsync(x => x.SessionId == request.SessionId && x.StudentId == studentId && x.VideoId == request.VideoId);
                if (session == null || session.Status != "Active")
                    return false;
                var video = await _context.CourseVideos.FirstOrDefaultAsync(x => x.Id == request.VideoId && x.IsPublished);
                if (video == null)
                    return false;
                if (request.PositionSeconds < 0 ||
                    request.PositionSeconds > video.DurationInSeconds)
                    return false;
                var now = DateTime.UtcNow;
                // Calculate watch time using server timestamps.
                // Only count a short, plausible playing interval.
                var elapsed = session.LastHeartbeatAt.HasValue ? (now - session.LastHeartbeatAt.Value).TotalSeconds : 0;
                var watchedDelta = session.IsPlaying && elapsed > 0 && elapsed <= 30 ? (int)Math.Floor(elapsed) : 0;
                var progress = await _context.VideoProgresses.FirstOrDefaultAsync(x => x.StudentId == studentId && x.VideoId == request.VideoId);
                if (progress == null)
                {
                    progress = new VideoProgress
                    {
                        StudentId = studentId,
                        VideoId = request.VideoId,
                        WatchedSeconds = 0,
                        LastPositionSeconds = 0,
                        CompletionPercentage = 0,
                        IsCompleted = false,
                        IsActive = true
                    };
                    _context.VideoProgresses.Add(progress);
                }

                progress.WatchedSeconds = (progress.WatchedSeconds ?? 0) + watchedDelta;
                progress.LastPositionSeconds = request.PositionSeconds;
                progress.LastWatchedAt = now;
                progress.LastSavedAt = now;
                progress.LastSessionId = request.SessionId;
                progress.LastEventId = request.EventId;
                progress.UpdateAt = now;
                var duration = video.DurationInSeconds;
                if (duration > 0)
                {
                    if (duration.HasValue && duration.Value > 0 &&
                        progress.WatchedSeconds.HasValue)
                    {
                        progress.CompletionPercentage = Math.Min(
                            100m,
                            Math.Round(
                                progress.WatchedSeconds.Value / duration.Value * 100m,
                                2
                            )
                        );
                    }
                    else
                    {
                        progress.CompletionPercentage = 0m;
                    }
                }
                // Example completion policy: 90%
                progress.IsCompleted = progress.CompletionPercentage >= 90;
                // Save event history
                var progressEvent = new VideoProgressEvents
                {
                    EventId = request.EventId,
                    SessionId = request.SessionId,
                    StudentId = studentId,
                    VideoId = request.VideoId,
                    EventType = request.EventType,
                    PositionSeconds = request.PositionSeconds,
                    WatchedSecondsDelta = watchedDelta,
                    OccurredAt = request.OccurredAt,
                    ReceivedAt = now,
                    StopReason = request.StopReason
                };
                _context.VideoProgressEvents.Add(progressEvent);
                // Update session heartbeat
                session.LastHeartbeatAt = now;
                session.LastPositionSeconds = request.PositionSeconds;
                session.IsPlaying = request.IsPlaying;
                if (request.EventType == "Ended")
                {
                    session.Status = "Completed";
                    session.EndedAt = now;
                    session.IsPlaying = false;
                    session.StopReason = "VideoEnded";
                }
                else if (request.EventType == "Paused")
                {
                    session.IsPlaying = false;
                    session.StopReason = request.StopReason ?? "UserPaused";
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VideoWatchSession> StartSessionAsync(string studentId, int videoId)
        {
            var video = await _context.CourseVideos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == videoId && x.IsPublished && x.IsActive);
            var now = DateTime.Now;
            var session = new VideoWatchSession
            {
                SessionId = Guid.NewGuid(),
                StudentId = studentId,
                VideoId = videoId,
                StartedAt = now,
                LastHeartbeatAt = now,
                LastPositionSeconds = 0,
                IsPlaying = false,
                Status = "Active"
            };
            _context.VideoWatchSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;

        }

        public async Task<ClassAttendance?> UpdateClassAttendanceAsync(int Id)
        {
            return await _context.ClassAttendances.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<CourseVideos?> UpdateCourseVideosAsync(int Id)
        {
            return await _context.CourseVideos.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<LiveClasses?> UpdateLiveClassAsync(int Id)
        {
            return await _context.LiveClasses.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<VideoProgress?> UpdateVideoProgressAsync(int Id)
        {
            return await _context.VideoProgresses.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
