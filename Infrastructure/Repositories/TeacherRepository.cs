using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
using Domain.Entities.Teacher;
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

        public async Task CreateBatchTeacherAsync(BatchTeacher entity)
        {
            await  _context.BatchTeachers.AddAsync(entity);
        }

        public async Task<GetTeacherBatchDto?> GetByIdBatchTeacherAsync(int Id)
        {
            return await _context.BatchTeachers.AsNoTracking().Include(x => x.user).Include(x=> x.batch).Where(x => x.Id == Id && x.IsActive)
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
              }).FirstOrDefaultAsync();
        }

        public async Task<BatchTeacher?> GetByIdBatchTeacherUpdateAsync(int Id)
        {
            return await _context.BatchTeachers.FirstOrDefaultAsync(x=> x.Id == Id && x.IsActive);
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

        public async Task<BatchTeacher?> UpdateBatchTeacherAsync(int Id)
        {
            return await _context.BatchTeachers.FirstOrDefaultAsync(x => x.Id == Id );
        }
    }
}
