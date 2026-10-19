using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.LiveClasses;
using Core.Interface.Repository;
using Domain.Entities.Common;
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
                  LeftAt = x.LeftAt,
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
              return await _context.CourseVideos.AsNoTracking().Include(x => x.course).Include(x => x.module).Include(x=> x.batch).Where(x => x.Id == Id && x.IsActive)
              .Select(x => new GetCourseVideoDto
              {
                  Id = x.Id,
                  CourseId = x.CourseId,
                  CourseName = x.course.Title,
                  ModuleId = x.ModuleId,
                  ModuleName = x.module.Name,
                  BatchId = x.BatchId,
                  BatchName = x.batch.Name,
                  Title = x.Title,
                  Description = x.Description,
                  VideoUrl = x.VideoUrl,
                  VideoUrlPublicId = x.VideoUrlPublicId,
                  DisplayOrder = x.DisplayOrder,
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
                  LastWatchedAt = x.LastWatchedAt,
                  IsActive = x.IsActive,
              }).FirstOrDefaultAsync();
        }

        public async Task<VideoProgress?> GetByIdVideoProgressUpdateAsync(int Id)
        {
            return await _context.VideoProgresses.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive);
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
                   LeftAt = x.LeftAt,
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

        public async Task<PageResultDto<GetCourseVideoDto>?> GetListCourseVideosAsync(int pageNumber, int pageSize)
        {

            var query = _context.CourseVideos.AsNoTracking().Include(x => x.course).Include(x => x.module).Include(x => x.batch)
               .Select(x => new GetCourseVideoDto
               {
                   Id = x.Id,
                   CourseId = x.CourseId,
                   CourseName = x.course.Title,
                   ModuleId = x.ModuleId,
                   ModuleName = x.module.Name,
                   BatchId = x.BatchId,
                   BatchName = x.batch.Name,
                   Title = x.Title,
                   Description = x.Description,
                   VideoUrl = x.VideoUrl,
                   VideoUrlPublicId = x.VideoUrlPublicId,
                   DisplayOrder = x.DisplayOrder,
                   DurationInSeconds = x.DurationInSeconds,
                   IsPublished = x.IsPublished,
                   IsActive = x.IsActive,
               });
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PageResultDto<GetCourseVideoDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
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
                 LastWatchedAt = x.LastWatchedAt,
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
