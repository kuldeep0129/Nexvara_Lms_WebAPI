using Core.Dtos.Account;
using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
using Domain.Entities.Teacher;
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
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;
        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
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
                  query=  query.Where(x => x.IsActive && x.StartDate <= today && x.EndDate >= today);
                    break;

                case AdminTeacherAssingStatus.Draft:
                   query = query.Where(x => !x.IsActive);
                    break;

                case AdminTeacherAssingStatus.Completed:
                    query = query.Where(x =>  x.EndDate < today );
                    break;

                case AdminTeacherAssingStatus.Upcomming:
                    query= query.Where(x =>  x.StartDate > today);
                    break;
                default :
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
            var data = await (from user in _context.Users join userRole in _context.UserRoles  on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                join teacher in _context.TeacherProfiles
                    on user.Id equals teacher.UserId
                where role.Name == RoleData.Trainer.ToString()
                      && teacher.IsActive
                select new  GetListTeacherIdNameDto
                {
                    Id = user.Id,
                    Name = user.FullName
                }).Distinct().AsNoTracking().ToListAsync();
            return data;
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
