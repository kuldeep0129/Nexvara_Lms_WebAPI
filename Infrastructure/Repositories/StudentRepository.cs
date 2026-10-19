using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
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

        public async Task<BatchStudent?> UpdateBatchStudentAsync(int Id)
        {
            return await _context.BatchStudents.FirstOrDefaultAsync(x => x.Id == Id);

        }
    }
}
