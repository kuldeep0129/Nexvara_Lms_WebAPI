using AutoMapper;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public class CoursesServices : ICoursesServices
    {
        private readonly ICoursesServices _Services;
        private readonly IFileUploadServices _uploadServices;
        private readonly IMapper _mapper;
        public CoursesServices(IMapper mapper, ICoursesServices services,IFileUploadServices fileservices)
        {
            _Services = services;
            _uploadServices = fileservices;
            _mapper = mapper;
        }

        public Task<ResponseDto<object>> CreateCoursesAsync(CreateCourses course, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto<object>> GetByIdCoursesAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto<object>> GetListCoursesAsync(PaginationRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto<object>> UpdateCoursesAsync(UpdateCourses course, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto<object>> UpdateStatusCoursesAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
