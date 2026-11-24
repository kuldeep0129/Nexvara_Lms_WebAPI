using AutoMapper;
using Core.Dtos.Batch;
using Core.Dtos.Common;
using Core.Dtos.Courses;
using Core.Dtos.Teacher.Attendance;
using Core.Dtos.Teacher.Batch;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Teacher;
using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TeacherServices : ITeacherServices
    {
        private readonly ITeacherRepository _reposiotry;
        private readonly IFileUploadServices _uploadServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public TeacherServices(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ITeacherRepository reposiotry, IFileUploadServices fileservices, UserManager<ApplicationUser> userManager)
        {
            _reposiotry = reposiotry;
            _uploadServices = fileservices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ResponseDto<object>> AssignTeacherAsync(int batchId, CreateBatchTeacherDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (batchId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            if (dto == null || string.IsNullOrWhiteSpace(dto.TeacherId))
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

            var batchExists = await _reposiotry.BatchExistsAsync(batchId);
            if (!batchExists)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }
            var teacherExists = await _userManager.FindByIdAsync(dto.TeacherId);
            if (teacherExists == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.TeacherNotFound;
                return response;
            }
            var alreadyAssigned = await _reposiotry.TeacherAlreadyAssignedAsync(batchId, dto.TeacherId);
            if (alreadyAssigned)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.TeacherIsAlreadyAssignedThisBatch;
                return response;
            }
            if (dto.IsPrimary)
            {
                var primaryExists = await _reposiotry.PrimaryTeacherExistsAsync(batchId);

                if (primaryExists)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.ThisBatchAlreadyHoldPrimaryTeacher;
                    return response;
                }
            }
            var batchTeacher = new BatchTeacher
            {
                BatchId = batchId,
                TeacherId = dto.TeacherId,
                IsPrimary = dto.IsPrimary,
                IsActive = true,
                AssignedAt = DateTime.Now,
                AssignedBy = userId,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };

            await _reposiotry.CreateAsync(batchTeacher);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateBatchTeacherAsync(CreateTeacherBatchDto dto, string userId)
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

            var data = new BatchTeacher
            {
                BatchId = dto.BatchId,
                TeacherId = dto.TeacherId,
                IsPrimary = dto.IsPrimary,
                AssignedAt = dto.AssignedAt,
                IsActive = true,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };

            await _reposiotry.CreateBatchTeacherAsync(data);
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordCreate;
            return response;
        }

        public async Task<ResponseDto<object>> CreateStudentAttendanceAsync(CreateStudentAttendanceDto dto)
        {
            var respone = new ResponseDto<object>();
            try
            {
                // =========================================
                // 1. Validate request
                // =========================================

                if (dto == null || dto.AttendanceRecords == null || !dto.AttendanceRecords.Any())
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.RequiredAttendanceRecord;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 2. Get logged-in teacher
                // =========================================

                var user = _httpContextAccessor.HttpContext?.User;

                var teacherId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(teacherId))
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.TeacherAuthenticationRequired;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 3. Validate student IDs
                // =========================================

                if (dto.AttendanceRecords.Any(x => string.IsNullOrWhiteSpace(x.StudentId)))
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.InvalidStudentId;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 4. Validate session IDs
                // =========================================

                if (dto.AttendanceRecords.Any(x => x.BatchClassSessionId <= 0))
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.InvalidbatchClassSessionId;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 5. Validate enum status
                // =========================================

                if (dto.AttendanceRecords.Any(x => !Enum.IsDefined(typeof(AttendanceStatus), x.Status)))
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.InvalidAttendanceStatus;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 6. Prevent duplicate request records
                // =========================================

                var duplicateRecords = dto.AttendanceRecords.GroupBy(x => new
                {
                    x.StudentId,
                    x.BatchClassSessionId
                }).Any(g => g.Count() > 1);

                if (duplicateRecords)
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.DuplicateStudentAttendance;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                var sessionIds = dto.AttendanceRecords.Select(x => x.BatchClassSessionId).Distinct().ToList();

                var studentIds = dto.AttendanceRecords.Select(x => x.StudentId).Distinct().ToList();

                // =========================================
                // 7. Get sessions
                // =========================================

                var sessions = await _reposiotry.GetSessionsByIdsAsync(sessionIds);

                if (sessions.Count != sessionIds.Count)
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.OneOrMoreSessionInvalid;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 8. Ensure all sessions belong to same batch
                // =========================================

                var batchIds = sessions.Select(x => x.BatchId).Distinct().ToList();

                if (batchIds.Count != 1)
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.AttendanceSessionbelongSameBatch;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                var batchId = batchIds[0];

                // =========================================
                // 9. Verify teacher assignment
                // =========================================

                var isAssigned = await _reposiotry.IsTeacherAssignedToBatchAsync(batchId.Value, teacherId);

                if (!isAssigned)
                {

                    respone.Success = false;
                    respone.Message = SystemMessage.YouAreNotAssignedThisBatch;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;
                }

                // =========================================
                // 10. Verify student enrollment
                // =========================================

                var enrolledStudentIds = await _reposiotry.GetEnrolledStudentIdsAsync(batchId.Value, studentIds);

                var enrolledSet = enrolledStudentIds.ToHashSet();

                var invalidStudents = studentIds.Where(x => !enrolledSet.Contains(x)).ToList();

                if (invalidStudents.Any())
                {
                    respone.Success = false;
                    respone.Message = SystemMessage.SomeStudnetAreNotEnrolled;
                    respone.ResponseCode = (int)APIResponseCodes.BadRequest;
                    return respone;

                }

                // =========================================
                // 11. Fetch existing attendance
                // =========================================

                var existingRecords = await _reposiotry.GetExistingAttendanceAsync(sessionIds, studentIds);

                var existingDictionary = existingRecords.ToDictionary(x => (x.StudentId, x.BatchClassSessionId));

                var newRecords = new List<BatchSessionAttendance>();

                var currentTime = DateTime.Now;

                // =========================================
                // 12. Insert or update
                // =========================================

                foreach (var item in dto.AttendanceRecords)
                {
                    var key = (item.StudentId, item.BatchClassSessionId);

                    if (existingDictionary.TryGetValue(key, out var existing))
                    {
                        // Update existing attendance
                        existing.Status = item.Status.ToString();
                        existing.Remarks = item.Remarks?.Trim();
                        existing.MarkedAt = currentTime;
                        existing.MarkedBy = teacherId;
                        existing.UpdateAt = currentTime;
                        existing.UpdateBy = teacherId;
                    }
                    else
                    {
                        // Create new attendance
                        newRecords.Add(new BatchSessionAttendance
                        {
                            StudentId = item.StudentId,
                            BatchClassSessionId = item.BatchClassSessionId,
                            Status = item.Status.ToString(),
                            Remarks = item.Remarks?.Trim(),
                            MarkedAt = currentTime,
                            MarkedBy = teacherId,
                            CreateAt = currentTime,
                            CreateBy = teacherId,
                        });
                    }
                }

                // =========================================
                // 13. Add new records
                // =========================================

                await _reposiotry.AddAttendanceRangeAsync(newRecords);

                // =========================================
                // 14. Save all changes
                // =========================================

                await _unitOfWork.SaveAsync();
                respone.Success = true;
                respone.Message = SystemMessage.RecordCreate;
                respone.ResponseCode = (int)APIResponseCodes.OK;
                return respone;
            }
            catch (Exception ex)
            {
                // Log the exception using ILogger in production.
                respone.Success = false;
                respone.Message = ex.Message;
                respone.ResponseCode = (int)APIResponseCodes.InternalServerError;
                return respone;
            }
        }

        public async Task<ResponseDto<object>> GetBatchTeachersAsync(int batchId)
        {
            var response = new ResponseDto<object>();
            if (batchId <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var IsBatch = await _reposiotry.BatchExistsAsync(batchId);
            if (!IsBatch)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.BatchNotFound;
                return response;
            }
            var res = await _reposiotry.GetByBatchIdAsync(batchId);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdBatchTeacherAsync(int Id)
        {
            var response = new ResponseDto<object>();
            if (Id <= 0)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var res = await _reposiotry.GetByIdBatchTeacherAsync(Id);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListAdminTeacherAssignAsync(AdminTeacherRequestVm vm)
        {
            var response = new ResponseDto<object>();
            if (vm == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListAdminTeacherAssignAsync(vm.PageNumber, vm.PageSize, vm.Status);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.Data = res;
            return response;
        }

        public async Task<ResponseDto<object>> GetListBatchTeacherAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }
            var res = await _reposiotry.GetListBatchTeacherAsync(dto.PageNumber, dto.PageSize);
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            return response;
        }

        public async Task<ResponseDto<object>> GetListTeacherIdNameAsync()
        {
            var response = new ResponseDto<object>();
            var res = await _reposiotry.GetListTeacherIdNameAsync();
            response.Success = true;
            response.Data = res;
            response.Message = SystemMessage.RecordFetch;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;

        }
        public async Task<ResponseDto<object>> GetMyBatchesAsync(
            PaginationRequestDto pagination,
            TeacherBatchFilterDto filter)
        {
            // =========================================================
            // 1. Get current authenticated user
            // =========================================================

            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null ||
                user.Identity?.IsAuthenticated != true)
            {
                return new ResponseDto<object>
                {
                    ResponseCode = (int)APIResponseCodes.UnAuthrized,
                    Success = false,
                    Message = SystemMessage.UnAuthorized,
                    Data = null
                };
            }


            // =========================================================
            // 2. Role validation
            // =========================================================

            if (!user.IsInRole(RoleData.Trainer.ToString()))
            {
                return new ResponseDto<object>
                {
                    ResponseCode = (int)APIResponseCodes.UnAuthrized,
                    Success = false,
                    Message = SystemMessage.UnAuthorized,
                    Data = null
                };
            }


            // =========================================================
            // 3. Get current teacher ID from JWT
            // =========================================================

            var teacherId = user.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(teacherId))
            {
                return new ResponseDto<object>
                {
                    ResponseCode = (int)APIResponseCodes.UnAuthrized,
                    Success = false,
                    Message = "Teacher identity could not be determined.",
                    Data = null
                };
            }


            // =========================================================
            // 4. Validate pagination
            // =========================================================

            pagination ??= new PaginationRequestDto();

            if (pagination.PageNumber <= 0)
            {
                pagination.PageNumber = 1;
            }

            if (pagination.PageSize <= 0)
            {
                pagination.PageSize = 10;
            }

            if (pagination.PageSize > 100)
            {
                pagination.PageSize = 100;
            }


            // =========================================================
            // 5. Validate filter
            // =========================================================

            filter ??= new TeacherBatchFilterDto();

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                filter.Status = filter.Status.Trim();
            }


            // =========================================================
            // 6. Repository call
            // =========================================================

            var result = await _reposiotry.GetMyBatchesAsync(
                teacherId,
                pagination,
                filter);


            // =========================================================
            // 7. Response message based on filter
            // =========================================================

            var message = result.TotalCount > 0
                ? "Batches retrieved successfully."
                : "No batches found.";


            // =========================================================
            // 8. Final response
            // =========================================================

            return new ResponseDto<object>
            {
                ResponseCode = (int)APIResponseCodes.OK,
                Success = true,
                Message = message,
                Data = result
            };
        }

        public async Task<ResponseDto<object>> GetParticularStudnetAttendanceSummeryAsync(int batchId, string studentId)
        {
            var response = new ResponseDto<object>();
            if (batchId <= 0 || string.IsNullOrWhiteSpace(studentId)) 
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var res = await _reposiotry.GetParticularStudnetAttendanceSummeryAsync(batchId,studentId);
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = res;
            return response;
        }

        public async Task<ResponseDto<object>> GetStudnetAttendanceSummeryAsync(int batchId)
        {
           var response = new ResponseDto<object>();
            if(batchId <= 0)
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var res = await _reposiotry.GetStudnetAttendanceSummeryAsync(batchId);
            response.Success = true;
            response.Message = SystemMessage.RecordFetch;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = res;
            return response;
        }

        public async Task<ResponseDto<PageResultDto<GetBatchClassSessionDto>>> GetTeacherBatchClassSessionsAsync(int? batchId, int pageNumber, int pageSize)
        {
            // 1. Get logged-in teacher ID
            var teacherId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(teacherId))
            {
                return new ResponseDto<PageResultDto<GetBatchClassSessionDto>>
                {
                    Success = false,
                    Message = "Teacher authentication required."
                };
            }

            // 2. Validate pagination
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            pageSize = Math.Min(pageSize, 100);

            // 3. Validate batch ID
            if (batchId.HasValue && batchId.Value <= 0)
            {
                return new ResponseDto<PageResultDto<GetBatchClassSessionDto>>
                {
                    Success = false,
                    Message = "Invalid batch ID."
                };
            }

            // 4. Get sessions
            var result = await _reposiotry.GetTeacherBatchClassSessionsAsync(  teacherId, batchId, pageNumber, pageSize);

            // 5. Success response
            return new ResponseDto<PageResultDto<GetBatchClassSessionDto>>
            {
                Success = true,
                Message = "Batch class sessions fetched successfully.",
                Data = result
            };
        }

        public async Task<ResponseDto<object>> GetTeacherBatchOverViewAsync(int batchId)
        {

            if (batchId <= 0)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    ResponseCode = (int)APIResponseCodes.BadRequest,
                    Message = "Invalid batch id."
                };
            }


            // =====================================
            // 2. Get Current Logged-in User
            // =====================================

            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity!.IsAuthenticated)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    ResponseCode = (int)APIResponseCodes.UnAuthrized,
                    Message = "User is not authenticated."
                };
            }


            // =====================================
            // 3. Validate Teacher Role
            // =====================================

            if (!user.IsInRole("Trainer"))
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    ResponseCode = (int)APIResponseCodes.Forbidden,
                    Message = "You are not authorized to access teacher batch details."
                };
            }


            // =====================================
            // 4. Get Teacher Id
            // =====================================

            var teacherId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(teacherId))
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    ResponseCode = (int)APIResponseCodes.UnAuthrized,
                    Message = "Unable to identify logged-in teacher."
                };
            }


            // =====================================
            // 5. Get Batch Overview
            // =====================================

            var result = await _reposiotry.GetTeacherBatchOverViewAsync(batchId, teacherId);

            if (result == null)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    ResponseCode = (int)APIResponseCodes.NotFound,
                    Message = "Batch not found or you are not assigned to this batch."
                };
            }


            // =====================================
            // 6. Success Response
            // =====================================

            return new ResponseDto<object>
            {
                Success = true,
                ResponseCode = (int)APIResponseCodes.OK,
                Message = "Batch overview fetched successfully.",
                Data = result
            };
        }

        public async Task<ResponseDto<object>> UpdateBatchTeacherAsync(UpdateTeacherBatchDto dto, string userId)
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
            var res = await _reposiotry.GetByIdBatchTeacherUpdateAsync(dto.Id);
            res.BatchId = dto.BatchId;
            res.TeacherId = dto.TeacherId;
            res.IsPrimary = dto.IsPrimary;
            res.AssignedAt = dto.AssignedAt;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusBatchTeacherAsync(int Id, string userId)
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
            var res = await _reposiotry.UpdateBatchTeacherAsync(Id);
            res.IsActive = !res.IsActive;
            res.UpdateAt = DateTime.Now;
            res.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateTeacherAssigmentPrimaryAsync(int batchId, string teacherId, UpdateBatchTeacherAssignStatusDto dto, string userId)
        {
            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            if (dto == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                return response;
            }

            if (batchId <= 0 || string.IsNullOrWhiteSpace(teacherId))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batchTeacher = await _reposiotry.GetByBatchAndTeacherIdAsync(batchId, teacherId);
            if (batchTeacher == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.TeacherAssignedNotFound;
                return response;
            }
            if (batchTeacher.IsPrimary == dto.Status)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = dto.Status
                ? "Teacher is already the primary teacher."
                : "Teacher is already not the primary teacher.";
                return response;
            }
            if (dto.Status)
            {
                var primaryExists = await _reposiotry.PrimaryTeacherExistsAsync(batchId, teacherId);
                if (primaryExists)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.ThisBatchAlreadyHoldPrimaryTeacher;
                    return response;
                }
            }
            batchTeacher.IsPrimary = dto.Status;
            batchTeacher.UpdateAt = DateTime.Now;
            batchTeacher.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordStatusChange;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateTeacherAssigmentStatusAsync(int batchId, string teacherId, string userId)
        {
            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            if (batchId <= 0 || string.IsNullOrWhiteSpace(teacherId))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batchTeacher = await _reposiotry.GetByBatchAndTeacherIdAsync(batchId, teacherId);
            if (batchTeacher == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.TeacherAssignedNotFound;
                return response;
            }
            batchTeacher.IsActive = !batchTeacher.IsActive;
            batchTeacher.UpdateAt = DateTime.UtcNow;
            batchTeacher.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordStatusChange;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateTeacherAssignmentAsync(int batchId, string teacherId, UpdateBatchTeacherDto dto, string userId)
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

            if (batchId <= 0 || string.IsNullOrWhiteSpace(teacherId))
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                return response;
            }
            var batchTeacher = await _reposiotry.GetByBatchAndTeacherIdAsync(batchId, teacherId);
            if (batchTeacher == null)
            {
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Success = false;
                response.Message = SystemMessage.TeacherAssignedNotFound;
                return response;
            }
            if (dto.IsPrimary && !batchTeacher.IsPrimary)
            {
                var primaryExists = await _reposiotry.PrimaryTeacherExistsAsync(batchId, teacherId);
                if (primaryExists)
                {
                    response.ResponseCode = (int)APIResponseCodes.BadRequest;
                    response.Success = false;
                    response.Message = SystemMessage.TeacherAlreadyHasPrimary;
                    return response;
                }
            }
            batchTeacher.IsPrimary = dto.IsPrimary;
            batchTeacher.UpdateAt = DateTime.UtcNow;
            batchTeacher.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Success = true;
            response.Message = SystemMessage.RecordUpdate;
            return response;
        }
    }
}
