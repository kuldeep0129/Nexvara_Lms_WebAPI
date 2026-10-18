using Core.Dtos.Common;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface IAccountRepository
    {
        Task StudentProfileAsync(StudentProfile profile);
        Task<StudentProfile?> GetByIdStudentProfileAsync(string Id);
        Task<PageResultDto<StudentProfile>?> GetListStudentProfileAsync(int pageNumber,int pageSize);
        Task<StudentProfile?> UpdateStudentProfileAsync(string Id);


        //Teacher Profile
        Task TeacherProfileAsync(TeacherProfiles profile);
        Task<TeacherProfiles?> GetByIdTeacherProfileAsync(string Id);
        Task<PageResultDto<TeacherProfiles>?> GetListTeacherProfileAsync(int pageNumber, int pageSize);
        Task<TeacherProfiles?> UpdateTeacherProfileAsync(string Id);
    }
}
