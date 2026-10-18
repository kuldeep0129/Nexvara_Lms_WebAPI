using Core.Dtos.Common;
using Core.Interface.Repository;
using Domain.Entities.Student;
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
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StudentProfile?> GetByIdStudentProfileAsync(string Id)
        {
          return  await _context.StudentProfiles.Include(x=> x.user).FirstOrDefaultAsync(x=> x.UserId == Id && x.IsActive );
        }

        public async Task<TeacherProfiles?> GetByIdTeacherProfileAsync(string Id)
        {
            return await _context.TeacherProfiles.Include(x=> x.user).FirstOrDefaultAsync(x => x.UserId == Id && x.IsActive);
        }

        public async Task<PageResultDto<StudentProfile>?> GetListStudentProfileAsync(int pageNumber, int pageSize)
        {
           var query = _context.StudentProfiles.AsNoTracking().Include(x=> x.user).OrderByDescending(x=>x.Id).AsQueryable();
            var totalCount = await query.CountAsync();
            var entity = await query.Skip((pageNumber -1)*pageSize).Take(pageSize).ToListAsync();
            return new PageResultDto<StudentProfile>
            {
                Items = entity,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PageResultDto<TeacherProfiles>?> GetListTeacherProfileAsync(int pageNumber, int pageSize)
        {
            var query = _context.TeacherProfiles.AsNoTracking().Include(x => x.user).OrderByDescending(x => x.Id).AsQueryable();
            var totalCount = await query.CountAsync();
            var entity = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageResultDto<TeacherProfiles>
            {
                Items = entity,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task StudentProfileAsync(StudentProfile profile)
        {
            await _context.StudentProfiles.AddAsync(profile);
        }

        public async Task TeacherProfileAsync(TeacherProfiles profile)
        {
            await _context.TeacherProfiles.AddAsync(profile);
        }

        public async Task<StudentProfile?> UpdateStudentProfileAsync(string Id)
        {
            return await _context.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == Id);

        }

        public async Task<TeacherProfiles?> UpdateTeacherProfileAsync(string Id)
        {
            return await _context.TeacherProfiles.FirstOrDefaultAsync(x => x.UserId == Id);

        }
    }
}
