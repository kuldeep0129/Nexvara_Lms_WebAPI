using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Interface.Repository;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.UserInterface;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Interface.Services
{
    public class CoursesServices : ICoursesServices
    {
        private readonly ICoursesRepository _reposiotry;
        private readonly IFileUploadServices _uploadServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public CoursesServices(IMapper mapper, IUnitOfWork unitOfWork, ICoursesRepository reposiotry, IFileUploadServices fileservices, IHttpContextAccessor contextAccessor)
        {
            _reposiotry = reposiotry;
            _uploadServices = fileservices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
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
                if (course.ImageUrl != null && course.ImageUrl.Length > 0)
                {
                    var uploadResult = await _uploadServices.UploadCloubinaryAsync(course.ImageUrl, "Admin/Course", default);
                    data.ImageUrl = uploadResult.Url;
                }
                data.Mode = course.Mode.ToString();
                data.AccessType = course.AccessType.ToString();
                data.Price = course.Price;
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

        public async Task<ResponseDto<object>> CreateCourseSyllabusAsync(CreateCourseSyllabusDto dto, string userId)
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
            var res = await _reposiotry.IsDisplayOrderExistsCourseSyllabusAsync(dto.CourseId, dto.DisplayOrder);
            if (res)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.AlreadyPlaceinTable;
                return response;
            }
            var data = new CourseSyllabus
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            await _reposiotry.CreateCourseSyllabusAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
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

        public async Task<ResponseDto<GetCourseCurriculumGroupDto>> GetCourseCurriculumAsync(int courseId)
        {
            var response = new ResponseDto<GetCourseCurriculumGroupDto>();

            // -----------------------------------------
            // 1. VALIDATE ID
            // -----------------------------------------

            if (courseId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }

            // -----------------------------------------
            // 2. GET CURRICULUM
            // -----------------------------------------

            var curriculum = await _reposiotry.GetCourseCurriculumAsync(courseId);

            // -----------------------------------------
            // 3. COURSE NOT FOUND
            // -----------------------------------------

            if (curriculum == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.CourseNotFound;
                return response;
            }

            // -----------------------------------------
            // 4. SUCCESS
            // -----------------------------------------
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = curriculum;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdCourseSyllabusAsync(int id)
        {
            var response = new ResponseDto<object>();
            if (id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetCourseSyllabusAsync(id);
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

        public async Task<ResponseDto<object>> GetListCoursesFeaturesAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListCourseFeatureAsync(dto.PageNumber, dto.PageSize);
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

        public async Task<ResponseDto<object>> GetListCoursesPublishAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListCoursePublishAsync(dto.PageNumber, dto.PageSize);
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
                var image = resCourse.ImageUrl;
                if (resCourse == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    response.ResponseCode = (int)APIResponseCodes.NotFound;
                    response.Success = false;
                    response.Message = SystemMessage.RecordNotFound;
                    return response;
                }
                _mapper.Map(course, resCourse);
                // New Course 
                if (course.ImageUrl != null && course.ImageUrl.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(resCourse.ImageUrl))
                    {
                        await _uploadServices.DeleteCloubinaryAsync(resCourse.ImageUrl);
                    }

                    // Upload new image
                    var uploadResult = await _uploadServices.UploadCloubinaryAsync(course.ImageUrl, "Admin/Course", default);
                    resCourse.ImageUrl = uploadResult.Url;
                }
                else
                {
                    resCourse.ImageUrl = image;
                }
                resCourse.AccessType = course.AccessType.ToString();
                resCourse.Mode = course.Mode.ToString();
                resCourse.Price = course.Price;
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

        public async Task<ResponseDto<object>> UpdateCourseSyllabusAsync(UpdateCourseSyllabusDto dto, string userId)
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
            var syllabus = await _reposiotry.GetByIdCourseSyllabusAsync(dto.Id);

            if (syllabus.IsActive == false)
            {
                var user = _contextAccessor.HttpContext?.User;
                var role = user?.FindFirstValue(ClaimTypes.Role);
                if (role == RoleData.SuperAdmin.ToString() || role == RoleData.Admin.ToString())
                {
                    syllabus.Title = dto.Title;
                    syllabus.Description = dto.Description;
                    syllabus.DisplayOrder = dto.DisplayOrder;
                    syllabus.UpdateAt = DateTime.Now;
                    syllabus.UpdateBy = userId;
                    await _unitOfWork.SaveAsync();
                    response.ResponseCode = (int)APIResponseCodes.OK;
                    response.Success = true;
                    response.Message = SystemMessage.RecordUpdate;
                    return response;
                }
                else
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.SyllabusInActive;
                    return response;
                }
            }
            syllabus.Title = dto.Title;
            syllabus.Description = dto.Description;
            syllabus.DisplayOrder = dto.DisplayOrder;
            syllabus.UpdateAt = DateTime.Now;
            syllabus.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateCourseSyllabusStatusAsync(int Id, string userId)
        {
            var response = new ResponseDto<object>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var syllabus = await _reposiotry.GetByIdCourseSyllabusAsync(Id);

            syllabus.IsActive = !syllabus.IsActive;
            syllabus.UpdateAt = DateTime.Now;
            syllabus.UpdateBy = userId;
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

        public async Task<ResponseDto<object>> CreateCourseMaterialAsync(CreateCourseMaterialsDto dto, string userId)
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
            var fileSize = dto.FileUrl.Length;
            var extension = Path.GetExtension(dto.FileUrl.FileName).ToLowerInvariant();
            var fileType = dto.FileUrl.ContentType;
            var data = new CourseMaterials
            {
                CourseId = dto.CourseId,
                ModuleId = dto.ModuleId,
                BatchId = dto.BatchId,
                FileType = fileType,
                FileSize = fileSize,
                Title = dto.Title,
                IsPublished = true,
                UploadedBy = userId,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            if (dto.FileUrl != null && dto.FileUrl.Length > 0)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(dto.FileUrl, "Course/Material", default);
                data.FileUrl = uploadResult.Url;
            }
            await _reposiotry.CreateCourseMaterialAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateCourseMaterialAsync(UpdateCourseMaterialDto dto, string userId)
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
            var res = await _reposiotry.GetByIdCourseMaterialAsync(dto.Id);
            if (res == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }


            res.CourseId = dto.CourseId;
            res.ModuleId = dto.ModuleId;
            res.BatchId = dto.BatchId;
            res.Title = dto.Title;
            res.UploadedBy = userId;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;

            if (dto.FileUrl != null && dto.FileUrl.Length > 0)
            {

                var fileSize = dto.FileUrl.Length;
                var extension = Path.GetExtension(dto.FileUrl.FileName).ToLowerInvariant();
                var fileType = dto.FileUrl.ContentType;
                if (!string.IsNullOrWhiteSpace(res.FileUrl))
                {
                    await _uploadServices.DeleteCloubinaryAsync(res.FileUrl);
                }

                // Upload new image
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(dto.FileUrl, "Course/Material", default);
                res.FileUrl = uploadResult.Url;
                res.FileType = fileType;
                res.FileSize = fileSize;
            }
          
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateCourseMaterialStatusAsync(int Id, string userId)
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
            var res = await _reposiotry.GetByIdCourseMaterialAsync(Id);
            if (res == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
          
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateCourseMaterialPublishAsync(int Id, string userId)
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
            var res = await _reposiotry.GetByIdCourseMaterialAsync(Id);
            if (res == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            res.IsPublished = !res.IsPublished;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;

            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.SaveSuccess;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdCourseMaterialAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdCourseMaterialAsync(Id);
            if (res == null)
            {
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                return response;
            }
            var data = new GetCourseMaterialDto
            {
                Id = res.Id,
                CourseId = res.CourseId ?? 0,
                CourseName = res.course.Title,
                ModuleId = res.ModuleId ?? 0,
                ModuleName = res.module.Title,
                BatchId = res. BatchId ?? 0,
                BatchName = res.batch.Name,
                Title = res.Title,
                FileUrl = res.FileUrl,
                FileType = res.FileType,
                FileSize = res.FileSize,
                IsPublished = res.IsPublished,
                UploadedBy = res.user.FullName,
                IsActive = res.IsActive,
            };
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = data;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListCourseSyllabusAsync(PaginationRequestDto dto,int courseId)
        {
            var response = new ResponseDto<object>();
          
            var res = await _reposiotry.GetListCourseSyllabusAsync(dto.PageNumber,dto.PageSize, courseId);
          
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }
    }
}
