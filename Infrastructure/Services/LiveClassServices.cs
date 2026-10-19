using AutoMapper;
using CloudinaryDotNet.Actions;
using Core.Dtos.Common;
using Core.Dtos.LiveClasses;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Domain.Entities.UserInterface;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Services
{
    public class LiveClassServices : ILiveClassServices
    {
        private readonly ILiveClassRepository _reposiotry;
        private readonly IFileUploadServices _uploadServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LiveClassServices(IMapper mapper, IUnitOfWork unitOfWork, ILiveClassRepository reposiotry, IFileUploadServices fileservices)
        {
            _reposiotry = reposiotry;
            _uploadServices = fileservices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<object>> CreateClassAttendanceAsync(CreateClassAttendanceDto dto, string userId)
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

            var data = new ClassAttendance
            {
                LiveClassId = dto.LiveClassId,
                StudentId = dto.StudentId,
                JoinedAt = dto.JoinedAt,
                LeftAt = dto.LeftAt,
                DurationInSeconds = dto.DurationInSeconds,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            await _reposiotry.CreateClassAttendanceAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateCourseVideoAsync(CreateCourseVideoDto dto, string userId)
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

            var data = new CourseVideos
            {
                CourseId = dto.CourseId,
                ModuleId = dto.ModuleId,
                BatchId = dto.BatchId,
                Title = dto.Title,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                DurationInSeconds = dto.DurationInSeconds,
                IsPublished = dto.IsPublished,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            if (dto.VideoUrl != null && dto.VideoUrl.Length > 0)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(dto.VideoUrl, "Video/Recorded", default);
                data.VideoUrl = uploadResult.Url;
                data.VideoUrlPublicId = uploadResult.PublicId;
            }
            await _reposiotry.CreateCourseVideosAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateLiveClassAsync(CreateLiveClassDto dto, string userId)
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

            var data = new LiveClasses
            {
                BatchId = dto.BatchId,
                ModuleId = dto.ModuleId,
                Title = dto.Title,
                Description = dto.Description,
                ScheduledStartAt = dto.ScheduledStartAt,
                ScheduledEndAt = dto.ScheduledEndAt,
                MeetingProvider = dto.MeetingProvider,
                MeetingUrl = dto.MeetingUrl,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            if (dto.Recording != null && dto.Recording.Length > 0)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(dto.Recording, "Recording", default);
                data.RecordingUrl = uploadResult.Url;
                data.RecordingUrlPublicId = uploadResult.PublicId;
            }
            await _reposiotry.CreateLiveClassAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateVideoProgressAsync(CreateVideoProgressDto dto, string userId)
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

            var data = new VideoProgress
            {
                VideoId = dto.VideoId,
                StudentId = dto.StudentId,
                WatchedSeconds = dto.WatchedSeconds,
                CompletionPercentage = dto.CompletionPercentage,
                LastPositionSeconds = dto.LastPositionSeconds,
                IsCompleted = dto.IsCompleted,
                LastWatchedAt = dto.LastWatchedAt,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };

            await _reposiotry.CreateVideoProgressAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdClassAttendanceAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdClassAttendanceAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdCourseVideoAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdCourseVideosAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdLiveClassAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdLiveClassAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdVideoProgressAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdVideoProgressAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListClassAttendanceAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListClassAttendanceAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListCourseVideoAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListCourseVideosAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListLiveClassAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListLiveClassAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListVideoProgressAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListVideoProgressAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateClassAttendanceAsync(UpdateClassAttendance dto, string userId)
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
            var res = await _reposiotry.GetByIdClassAttendanceUpdateAsync(dto.Id);
            res.LiveClassId = dto.LiveClassId;
            res.StudentId = dto.StudentId;
            res.JoinedAt = dto.JoinedAt;
            res.LeftAt = dto.LeftAt;
            res.DurationInSeconds = dto.DurationInSeconds;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateCourseVideoAsync(UpdateCourseVideosDto dto, string userId)
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
            var res = await _reposiotry.GetByIdCourseVideosUpdateAsync(dto.Id);
            res.CourseId = dto.CourseId;
            res.ModuleId = dto.ModuleId;
            res.BatchId = dto.BatchId;
            res.Title = dto.Title;
            res.Description = dto.Description;
            res.DisplayOrder = dto.DisplayOrder;
            res.DurationInSeconds = dto.DurationInSeconds;
            res.IsPublished = dto.IsPublished;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            if (dto.VideoUrl != null && dto.VideoUrl.Length > 0)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(dto.VideoUrl, "Video/Recorded", default);
                res.VideoUrl = uploadResult.Url;
                res.VideoUrlPublicId = uploadResult.PublicId;
            }
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateLiveClassAsync(UpdateLiveClassesDto dto, string userId)
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
            var res = await _reposiotry.GetByIdLiveClassUpdateAsync(dto.Id);
            res.BatchId = dto.BatchId;
            res.ModuleId = dto.ModuleId;
            res.Title = dto.Title;
            res.Description = dto.Description;
            res.ScheduledStartAt = dto.ScheduledStartAt;
            res.ScheduledEndAt = dto.ScheduledEndAt;
            res.MeetingProvider = dto.MeetingProvider;
            res.MeetingUrl = dto.MeetingUrl;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusClassAttendanceAsync(int Id, string userId)
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
            var res = await _reposiotry.UpdateClassAttendanceAsync(Id);
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusCourseVideoAsync(int Id, string userId)
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
            var res = await _reposiotry.UpdateCourseVideosAsync(Id);
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusLiveClassAsync(int Id, string userId)
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
            var res = await _reposiotry.UpdateLiveClassAsync(Id);
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusVideoProgressAsync(int Id, string userId)
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
            var res = await _reposiotry.UpdateVideoProgressAsync(Id);
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateVideoProgressAsync(UpdateVideoProgressDto dto, string userId)
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
            var res = await _reposiotry.GetByIdVideoProgressUpdateAsync(dto.Id);

            res.VideoId = dto.VideoId;
            res.StudentId = dto.StudentId;
            res.WatchedSeconds = dto.WatchedSeconds;
            res.CompletionPercentage = dto.CompletionPercentage;
            res.LastPositionSeconds = dto.LastPositionSeconds;
            res.IsCompleted = dto.IsCompleted;
            res.LastWatchedAt = dto.LastWatchedAt;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }
    }
}
