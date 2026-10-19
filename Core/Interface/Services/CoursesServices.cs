using AutoMapper;
using CloudinaryDotNet;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.UserInterface;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public class CoursesServices : ICoursesServices
    {
        private readonly ICoursesRepository _reposiotry;
        private readonly IFileUploadServices _uploadServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CoursesServices(IMapper mapper, IUnitOfWork unitOfWork, ICoursesRepository reposiotry, IFileUploadServices fileservices)
        {
            _reposiotry = reposiotry;
            _uploadServices = fileservices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<object>> CreateBatchAsync(CreateBatchDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var data = new Batch
            {
                CourseId = dto.CourseId,
                Name = dto.Name,
                BatchCode = dto.BatchCode,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Capacity = dto.Capacity,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };

            await _reposiotry.CreateBatchAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateCoursesAsync(CreateCourses course, string userId)
        {
            var response = new ResponseDto<object>();
            if (course == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var data = _mapper.Map<Course>(course);
                data.IsActive = true;
                data.CreateAt = DateTime.UtcNow;
                data.CreateBy = userId;
                await _reposiotry.CreateCourseAsync(data);
                await _unitOfWork.SaveAsync();
                var settings = _mapper.Map<CourseSettings>(course);
                settings.CourseId = data.Id;
                settings.IsActive = true;
                settings.CreateAt = DateTime.Now;
                settings.CreateBy = userId;
                await _reposiotry.CreateCourseSettingAsync(settings);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.OK;
                response.Success = true;
                response.Message = SystemMessage.RecordCreate;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }


        }

        public async Task<ResponseDto<object>> CreateCoursesModuleAsync(CreateCourseModuleDto course, string userId)
        {

            var response = new ResponseDto<object>();
            if (course == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var data = new CourseModule
            {
                CourseId = course.CourseId,
                Name = course.Name,
                Description = course.Description,
                Sortorder = course.Sortorder,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };

            await _reposiotry.CreateCourseModuleAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdBatchAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdBatchAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdCoursesAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdCoursesAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdCoursesModuleAsync(int Id) 
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdCoursesModuleAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListBatchAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListBatchAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListCoursesAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListCourseAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListCoursesModuleAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListCourseModuleAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateBatchAsync(UpdateCourseBatchDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var resCourse = await _reposiotry.GetByIdBatchUpdateAsync(dto.Id);
            resCourse.CourseId = dto.Id;
            resCourse.Name = dto.Name;
            resCourse.BatchCode = dto.BatchCode;
            resCourse.StartDate = dto.StartDate;
            resCourse.EndDate = dto.EndDate;
            resCourse.Capacity = dto.Capacity;
            resCourse.UpdateAt = DateTime.Now;
            resCourse.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateCoursesAsync(UpdateCourses course, string userId)
        {
            var response = new ResponseDto<object>();
            if (course == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var resCourse = await _reposiotry.GetByIdCoursesUpdateAsync(course.Id);
                if (resCourse == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    response.ResponseCode = (int)APIResponseCodes.NotFound;
                    response.Success = false;
                    response.Message = SystemMessage.RecordNotFound;
                    return response;
                }
                _mapper.Map(course, resCourse);
                resCourse.UpdateAt = DateTime.Now;
                resCourse.UpdateBy = userId;

                var settings = await _reposiotry.GetByIdCoursesSettingAsync(resCourse.Id);
                if (settings == null)
                {
                    settings = _mapper.Map<CourseSettings>(course);
                    settings.CourseId = resCourse.Id;
                    settings.CreateAt = DateTime.Now;
                    settings.CreateBy = userId;

                    await _reposiotry.CreateCourseSettingAsync(settings);
                }
                else
                {
                    _mapper.Map(course, settings);
                    settings.UpdateAt = DateTime.UtcNow;
                    settings.UpdateBy = userId;
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.OK;
                response.Success = true;
                response.Message = SystemMessage.RecordUpdate;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseDto<object>> UpdateCoursesModuleAsync(UpdateCourseModule course, string userId)
        {
            var response = new ResponseDto<object>();
            if (course == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var resCourse = await _reposiotry.GetByIdCoursesModuleUpdateAsync(course.Id);
            resCourse.CourseId = course.Id;
            resCourse.Name = course.Name;
            resCourse.Description = course.Description;
            resCourse.Sortorder = course.Sortorder;
            resCourse.UpdateAt = DateTime.Now;
            resCourse.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusBatchAsync(int Id, string userId)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {

                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var resCourse = await _reposiotry.UpdateBatchAsync(Id);
            resCourse.IsActive = !resCourse.IsActive;
            resCourse.UpdateAt = DateTime.Now;
            resCourse.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusCoursesAsync(int id, string userId)
        {
            var response = new ResponseDto<object>();

            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }

            var resCourse = await _reposiotry.UpdateCourseAsync(id);
            if (resCourse == null)
            {
                await _unitOfWork.RollbackTransactionAsync();

                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            resCourse.IsActive = !resCourse.IsActive;
            resCourse.UpdateAt = DateTime.Now;
            resCourse.UpdateBy = userId;

            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordStatusChange;
            return response;

        }

        public async Task<ResponseDto<object>> UpdateStatusCoursesModuleAsync(int Id, string userId)
        {
            var response = new ResponseDto<object>();
            if(Id <= 0)
            {

                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var resCourse = await _reposiotry.UpdateCourseModuleAsync(Id);
            resCourse.IsActive = !resCourse.IsActive;
            resCourse.UpdateAt = DateTime.Now;
            resCourse.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }
    }
}
