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
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public LiveClassServices(IMapper mapper, IUnitOfWork unitOfWork, ILiveClassRepository reposiotry, IFileUploadServices fileservices, IConfiguration configuration, HttpClient httpClient)
        {
            _reposiotry = reposiotry;
            _uploadServices = fileservices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClient;
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

            bool hasCloudinaryVideo = dto.VideoUrl != null && dto.VideoUrl.Length > 0;
            bool hasYoutubeVideo = !string.IsNullOrWhiteSpace(dto.VideoYoutubeUrl);
            if (hasCloudinaryVideo == hasYoutubeVideo)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Please provide exactly one video source.";
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
                IsPublished = dto.IsPublished,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
            if (hasCloudinaryVideo)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(dto.VideoUrl, "Video/Recorded", default);
                data.VideoProvider = "Cloudinary";
                data.VideoUrl = uploadResult.Url;
                data.VideoUrlPublicId = uploadResult.PublicId;
                data.DurationInSeconds = uploadResult.Duration.HasValue ? (int)Math.Ceiling(uploadResult.Duration.Value) : null;
            }
            else
            {
                var videoId = ExtractYouTubeVideoId(dto.VideoYoutubeUrl!);
                if (string.IsNullOrWhiteSpace(videoId))
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = "Invalid YouTube video URL.";
                    return response;
                }
                data.VideoProvider = "YouTube";
                data.VideoId = videoId;
                data.DurationInSeconds = await GetYouTubeDurationAsync(videoId);
            }
            await _reposiotry.CreateCourseVideosAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }
        private string? ExtractYouTubeVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return null;

            if (uri.Host.Equals("youtu.be",
                StringComparison.OrdinalIgnoreCase))
            {
                return uri.AbsolutePath.Trim('/');
            }

            if (uri.Host.EndsWith("youtube.com",
                StringComparison.OrdinalIgnoreCase))
            {
                if (uri.AbsolutePath == "/watch")
                    return System.Web.HttpUtility.ParseQueryString(
                        uri.Query).Get("v");

                if (uri.AbsolutePath.StartsWith("/embed/"))
                    return uri.AbsolutePath.Split('/').Last();

                if (uri.AbsolutePath.StartsWith("/shorts/"))
                    return uri.AbsolutePath.Split('/').Last();
            }

            return null;
        }
        private async Task<int?> GetYouTubeDurationAsync(string videoId)
        {
            var apiKey = _configuration["YouTube:ApiKey"];

            var url = $"https://www.googleapis.com/youtube/v3/videos" +
                      $"?part=contentDetails&id={videoId}&key={apiKey}";

            var result = await _httpClient.GetFromJsonAsync<YouTubeVideoResponse>(url);

            var duration = result?.Items?.FirstOrDefault()?.ContentDetails?.Duration;

            if (string.IsNullOrWhiteSpace(duration))
                return null;

            return (int)System.Xml.XmlConvert.ToTimeSpan(duration).TotalSeconds;
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

        public async Task<ResponseDto<object>> GetListCourseVideoAsync(PaginationRequestDto dto, int batchId)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListCourseVideosAsync(dto.PageNumber, dto.PageSize, batchId);
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
            // Both sources cannot be provided together.
            bool hasCloudinaryVideo = dto.VideoUrl != null && dto.VideoUrl.Length > 0;
            bool hasYoutubeVideo = !string.IsNullOrWhiteSpace(dto.VideoYoutubeUrl);

            if (hasCloudinaryVideo && hasYoutubeVideo)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = "Please provide only one video source.";
                return response;
            }

            var res = await _reposiotry.GetByIdCourseVideosUpdateAsync(dto.Id);
            string? youtubeVideoId = null;

            if (hasYoutubeVideo)
            {
                youtubeVideoId = ExtractYouTubeVideoId(dto.VideoYoutubeUrl!);

                if (string.IsNullOrWhiteSpace(youtubeVideoId))
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = "Invalid YouTube video URL.";
                    return response;
                }
            }

            // Update basic details.
            res.CourseId = dto.CourseId;
            res.ModuleId = dto.ModuleId;
            res.BatchId = dto.BatchId;
            res.Title = dto.Title.Trim();
            res.Description = dto.Description;
            res.DisplayOrder = dto.DisplayOrder;
            res.IsPublished = dto.IsPublished;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;

            // Replace video only when a new source is provided.
            if (hasCloudinaryVideo)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(
                    dto.VideoUrl!,
                    "Video/Recorded",
                    default);

                res.VideoProvider = "Cloudinary";
                res.VideoUrl = uploadResult.Url;
                res.VideoUrlPublicId = uploadResult.PublicId;
                res.VideoId = null;

                res.DurationInSeconds = uploadResult.Duration.HasValue
                    ? (int)Math.Ceiling(uploadResult.Duration.Value)
                    : null;
            }
            else if (hasYoutubeVideo)
            {
                var duration = await GetYouTubeDurationAsync(youtubeVideoId!);

                res.VideoProvider = "YouTube";
                res.VideoId = youtubeVideoId;
                res.VideoUrl = null;
                res.VideoUrlPublicId = null;
                res.DurationInSeconds = duration;
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

        public async Task<ResponseDto<object>> GetStudentCourseVideosAsync(int batchId, string studentId)
        {
            var response = new ResponseDto<object>();
           
            if (string.IsNullOrWhiteSpace(studentId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            if (batchId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetStudentCourseVideosAsync(batchId,studentId);        
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<Guid?> StartSessionAsync(string studentId, int videoId)
        {
            if (videoId <= 0)
                throw new ArgumentException("Invalid video ID.");

            var session = await _reposiotry.StartSessionAsync(studentId, videoId);

            return session.SessionId; 
        }

        public async Task<bool> SaveProgressAsync(string studentId, SaveVideoProgressDto request)
        {
            if (request.EventId == Guid.Empty ||
                  request.SessionId == Guid.Empty ||
                  request.VideoId <= 0 ||
                  request.PositionSeconds < 0)
            {
                throw new ArgumentException(
                    "Invalid video progress request.");
            }

            var allowedEvents = new[]
            {
                "Checkpoint",
                "Paused",
                "PageHidden",
                "Ended",
                "Resumed"
            };

            if (!allowedEvents.Contains(request.EventType))
                throw new ArgumentException("Invalid event type.");

            return await _reposiotry.SaveProgressAsync(
                studentId, request);
        }

        public async Task<VideoProgressResponseDto?> GetProgressAsync(string studentId, int videoId)
        {
            return await _reposiotry.GetProgressAsync(studentId, videoId);
        }

        public async Task<VideoProgressResponseDto?> GetLastWatchedAsync(string studentId)
        {
            return await _reposiotry.GetLastWatchedAsync(studentId);

        }

        public async Task<bool> EndSessionAsync(string studentId, Guid sessionId, string reason)
        {
            return await _reposiotry.EndSessionAsync( studentId, sessionId, reason);
        }
    }
}
