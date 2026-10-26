using AutoMapper;
using Azure;
using CloudinaryDotNet.Actions;
using Core.Dtos.Account;
using Core.Dtos.Common;
using Core.Dtos.Profile;
using Core.Interface.Repository;
using Core.Interface.Services;
using Domain.common;
using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailServices _emailServices;
        private readonly IFileUploadServices _uploadServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;


        public AccountServices(UserManager<ApplicationUser> userManager, IEmailServices emailServices, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IFileUploadServices uploadServices, IMapper mapper, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext contex, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = contex;
            _mapper = mapper;
            _uploadServices = uploadServices;
            _roleManager = roleManager;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _emailServices = emailServices;
        }


        public async Task<ResponseDto<object>> LoginAsync(LoginDto dto)
        {
            var response = new ResponseDto<object>();
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var IsPassword = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!IsPassword.Succeeded)
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Message = SystemMessage.InvalidPassword;
                return response;
            }

            if (!user.EmailConfirmed)
            {
                var token1 = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedToken = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token1)
                );

                var confirmationUrl =
                    $"{_configuration["Frontend:BaseUrl"]}/confirm-email" +
                    $"?userId={Uri.EscapeDataString(user.Id)}" +
                    $"&token={Uri.EscapeDataString(encodedToken)}";

                await _emailServices.SendEmailConfirmationAsync(
                    user.Email!,
                    user.FullName ?? user.Email!,
                    confirmationUrl,
                    default
                );
              

                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                response.Message =
                    "Please verify your email. A new verification link has been sent to your email address.";

                return response;
            }

            //check user Active 
            var userRole = await _userManager.GetRolesAsync(user);

            if (userRole == null || userRole.Count == 0)
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Message = "User role is not assigned.";
                return response;
            }

            if (userRole.Contains(RoleData.Student.ToString()))
            {
                var res = await _accountRepository.GetByIdStudentProfileAsync(user.Id);
                if (res.IsActive == false)
                {
                    response.Success = false;
                    response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                    response.Message = "Please Contact Adminstration.";
                    return response;
                }
            }
            else if (userRole.Contains(RoleData.Trainer.ToString()))
            {
                var res = await _accountRepository.GetByIdTeacherProfileAsync(user.Id);
                if (res.IsActive == false)
                {
                    response.Success = false;
                    response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                    response.Message = "Please Contact Adminstration.";
                    return response;
                }
            }

            //refereh token
            var rawRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = GetRefreshTokenExpiry();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = HashRefreshToken(rawRefreshToken),
                CreatedAt = DateTime.Now,
                ExpiresAt = refreshTokenExpiry
            };
            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();
            //jwt token genrate
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Email,user.Email ?? ""),
                new Claim(ClaimTypes.Name , user.FullName ?? user.Email ?? "")
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key is missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");
            var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var configuredMinutes) ? configuredMinutes : 30;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims, notBefore: DateTime.UtcNow, expires: expiresAt, signingCredentials: credentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var data = new LoginResponseDto
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresAtUtc = expiresAt,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList(),
                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiry
            };
            response.Success = true;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Message = SystemMessage.LoginSuccess;
            response.Data = data;
            return response;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(randomBytes);
        }
        private static string HashRefreshToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
        private DateTime GetRefreshTokenExpiry()
        {
            var days = int.TryParse(_configuration["Jwt:RefreshTokenExpiryDays"], out var configuredDays) ? configuredDays : 7;
            if (days <= 0)
            {
                throw new InvalidOperationException("Refresh token expiry must be greater than zero.");
            }
            return DateTime.UtcNow.AddDays(days);
        }

        public async Task<ResponseDto<object>> RefreshAsync(string dto)
        {
            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(dto))
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }
            var tokenHash = HashRefreshToken(dto);
            // Use a transaction so rotation is handled together.
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var storedToken = await _context.RefreshTokens.Include(x => x.User).SingleOrDefaultAsync(x => x.TokenHash == tokenHash);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            var user = storedToken.User;

            // Optional account security checks.
            if (await _userManager.IsLockedOutAsync(user))
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                response.Message = SystemMessage.UnAuthorized;
                return response;
            }

            // Revoke old refresh token.
            storedToken.RevokedAt = DateTime.UtcNow;

            // Generate replacement refresh token.
            var newRawRefreshToken = GenerateRefreshToken();
            var newRefreshTokenHash = HashRefreshToken(newRawRefreshToken);
            var newRefreshTokenExpiry = GetRefreshTokenExpiry();
            storedToken.ReplacedByTokenHash = newRefreshTokenHash;
            var newTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = newRefreshTokenExpiry
            };
            _context.RefreshTokens.Add(newTokenEntity);
            // Get current roles, not stale role claims.
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>  {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(ClaimTypes.NameIdentifier, user.Id),
              new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
              new Claim( ClaimTypes.Name, user.FullName ?? user.Email ?? string.Empty)};
            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                claims.Add(new Claim("FullName", user.FullName));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key is missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");
            var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var configuredMinutes) ? configuredMinutes : 30;
            if (expiryMinutes <= 0)
            {
                throw new InvalidOperationException("JWT expiry must be greater than zero.");
            }

            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(expiryMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Success = true;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Message = SystemMessage.LoginSuccess;
            response.Data = new LoginResponseDto
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresAtUtc = expiresAt,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList(),
                RefreshToken = newRawRefreshToken,
                RefreshTokenExpiresAt = newRefreshTokenExpiry
            };

            return response;
        }

        public async Task<ResponseDto<object>> LogoutAsync(LogoutDto dto, string userId, string? ipAddress)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            if (userId == null)
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }
            var tokenHash = HashRefreshToken(dto.RefreshToken);
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.UserId == userId);
            if (storedToken == null)
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidRefershToken;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            if (storedToken.IsRevoked)
            {
                response.Success = false;
                response.Message = SystemMessage.UserAlreadyLogout;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            await _context.SaveChangesAsync();
            response.Success = true;
            response.Message = SystemMessage.LogoutSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> RegisterAsync(RegistrationDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            var data = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.MobileNo,
            };
            var res = await _userManager.CreateAsync(data, dto.Password);
            if (!res.Succeeded)
            {
                response.Success = false;
                response.Message = SystemMessage.AlreadyUseEmail.Replace("{Email}", data.Email);
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            if (!await _roleManager.RoleExistsAsync(dto.Role.ToString()))
            {
                response.Success = false;
                response.Message = $"Role '{dto.Role.ToString()}' does not exist.";
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var roleResult = await _userManager.AddToRoleAsync(data, dto.Role.ToString());
            if (!roleResult.Succeeded)
            {
                response.Success = false;
                response.Message = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(data);
            //token url convert base 64
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationUrl = $"{_configuration["Frontend:BaseUrl"]}/confirm-email" +
                $"?userId={data.Id}&token={encodedToken}";
            await _emailServices.SendEmailConfirmationAsync(data.Email!, data.FullName ?? data.Email!, confirmationUrl, default);
            switch (dto.Role.ToString())
            {
                case "Student":
                    {
                        var studentProfile = new StudentProfile
                        {
                            UserId = data.Id,
                            IsActive = true,
                            CreateAt = DateTime.UtcNow
                        };

                        await _accountRepository.StudentProfileAsync(studentProfile);
                        await _unitOfWork.SaveAsync();
                        break;
                    }

                case "Trainer":
                    {
                        var teacherProfile = new TeacherProfiles
                        {
                            UserId = data.Id,
                            IsActive = true,
                            CreateAt = DateTime.UtcNow
                        };

                        await _accountRepository.TeacherProfileAsync(teacherProfile);
                        await _unitOfWork.SaveAsync();
                        break;
                    }

                default:
                    break;
            }
            response.Success = true;
            response.Message = SystemMessage.UserRegisterSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;

        }


        public async Task<ResponseDto<object>> StudentProfileAsync([FromForm] CreateStudentProfile profile, string userId)
        {
            var response = new ResponseDto<object>();
            if (profile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            if (userId == null)
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }
            var studentProfile = await _accountRepository.GetByIdStudentProfileAsync(profile.Id);
            if (studentProfile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            if (profile.ProfileImage != null && profile.ProfileImage.Length > 0)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(profile.ProfileImage, "students/profile", default);
                studentProfile.ProfileImageUrl = uploadResult.Url;
                studentProfile.ProfileImagePublicId = uploadResult.PublicId;
            }
            studentProfile.DateOfBirth = profile.DateOfBirth;
            studentProfile.Gender = profile.Gender.ToString();
            studentProfile.Address = profile.Address;
            studentProfile.Qualification = profile.Qualification;
            studentProfile.CollegeName = profile.collegename;
            studentProfile.PassingYear = profile.Passingyear;
            studentProfile.UpdateAt = DateTime.Now;
            studentProfile.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = SystemMessage.UpdateStudentProfileSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStudentProfileAsync([FromForm] CreateStudentProfile profile, string userId)
        {
            var response = new ResponseDto<object>();

            // Request validation
            if (profile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            // User validation
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }

            // Get existing profile
            var studentProfile = await _accountRepository.GetByIdStudentProfileAsync(userId ?? profile.Id);

            if (studentProfile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.StudentNotFound;
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                return response;
            }

            // Map normal fields
            _mapper.Map(profile, studentProfile);

            // New profile image
            if (profile.ProfileImage != null && profile.ProfileImage.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(studentProfile.ProfileImagePublicId))
                {
                    await _uploadServices.DeleteCloubinaryAsync(studentProfile.ProfileImagePublicId);
                }

                // Upload new image
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(profile.ProfileImage, "students/profile", default);
                studentProfile.ProfileImageUrl = uploadResult.Url;

                studentProfile.ProfileImagePublicId = uploadResult.PublicId;
            }
            // Audit fields
            studentProfile.UpdateAt = DateTime.UtcNow;
            studentProfile.UpdateBy = userId;
            // Save
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = SystemMessage.UpdateStudentProfileSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> GetByIdStudentProfileAsync(string userId)
        {
            var response = new ResponseDto<object>();
            if (userId == null)
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var data = await _accountRepository.GetByIdStudentProfileAsync(userId);
            var map = _mapper.Map<StudentProfileResponseDto>(data);

            response.Success = true;
            response.Message = SystemMessage.StudentFetchSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = map;
            return response;
        }

        public async Task<ResponseDto<object>> GetListStudentProfileAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var pagedStudents = await _accountRepository.GetListStudentProfileAsync(dto.PageNumber, dto.PageSize);
            var students = _mapper.Map<List<StudentProfileResponseDto>>(pagedStudents.Items);

            var result = new PageResultDto<StudentProfileResponseDto>
            {
                Items = students,
                PageNumber = pagedStudents.PageNumber,
                PageSize = pagedStudents.PageSize,
                TotalCount = pagedStudents.TotalCount
            };

            response.Success = true;
            response.Message = SystemMessage.StudentFetchSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = result;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusStudentProfileAsync(string userId)
        {
            var response = new ResponseDto<object>();
            // User validation
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }
            // Get existing profile
            var studentProfile = await _accountRepository.UpdateStudentProfileAsync(userId);

            if (studentProfile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.StudentNotFound;
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                return response;
            }
            studentProfile.IsActive = !studentProfile.IsActive;
            studentProfile.UpdateAt = DateTime.UtcNow;
            studentProfile.UpdateBy = userId;
            // Save
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = SystemMessage.UpdateStudentProfileSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> TeacherProfileAsync(CreateTeacherProfile profile, string userId)
        {
            var response = new ResponseDto<object>();
            if (profile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            if (userId == null)
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }
            var teacherProfile = await _accountRepository.GetByIdTeacherProfileAsync(profile.Id);
            if (teacherProfile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            if (profile.ProfileImage != null && profile.ProfileImage.Length > 0)
            {
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(profile.ProfileImage, "Teacher/profile", default);
                teacherProfile.ProfileImageUrl = uploadResult.Url;
                teacherProfile.ProfileImagePublicId = uploadResult.PublicId;
            }
            teacherProfile.Qualification = profile.Qualification;
            teacherProfile.ExperienceYears = profile.Experienceyear;
            teacherProfile.Bio = profile.bio;
            teacherProfile.PassingYear = profile.PassingYear;
            teacherProfile.Specialization = profile.specialization;
            teacherProfile.UpdateAt = DateTime.Now;
            teacherProfile.UpdateBy = userId;
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = SystemMessage.UpdateProfileSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateTeacherProfileAsync(CreateTeacherProfile profile, string userId)
        {
            var response = new ResponseDto<object>();

            // Request validation
            if (profile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            // User validation
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }

            // Get existing profile
            var teacherProfile = await _accountRepository.GetByIdTeacherProfileAsync(userId);

            if (teacherProfile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.TeacherNotFound;
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                return response;
            }

            // Map normal fields
            _mapper.Map(profile, teacherProfile);

            // New profile image
            if (profile.ProfileImage != null && profile.ProfileImage.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(teacherProfile.ProfileImagePublicId))
                {
                    await _uploadServices.DeleteCloubinaryAsync(teacherProfile.ProfileImagePublicId);
                }

                // Upload new image
                var uploadResult = await _uploadServices.UploadCloubinaryAsync(profile.ProfileImage, "Teacher/profile", default);
                teacherProfile.ProfileImageUrl = uploadResult.Url;

                teacherProfile.ProfileImagePublicId = uploadResult.PublicId;
            }
            // Audit fields
            teacherProfile.UpdateAt = DateTime.UtcNow;
            teacherProfile.UpdateBy = userId;
            // Save
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = SystemMessage.UpdateProfileSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> UpdateStatusTeacherProfileAsync(string userId)
        {
            var response = new ResponseDto<object>();
            // User validation
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }
            // Get existing profile
            var teacherProfile = await _accountRepository.UpdateTeacherProfileAsync(userId);

            if (teacherProfile == null)
            {
                response.Success = false;
                response.Message = SystemMessage.StudentNotFound;
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                return response;
            }
            teacherProfile.IsActive = !teacherProfile.IsActive;
            teacherProfile.UpdateAt = DateTime.UtcNow;
            teacherProfile.UpdateBy = userId;
            // Save
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = SystemMessage.UpdateProfileSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;

        }

        public async Task<ResponseDto<object>> GetByIdTeacherProfileAsync(string userId)
        {
            var response = new ResponseDto<object>();
            if (userId == null)
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidId;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var data = await _accountRepository.GetByIdTeacherProfileAsync(userId);
            var map = _mapper.Map<TeacherProfileResponseDto>(data);

            response.Success = true;
            response.Message = SystemMessage.TeacherFetchSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = map;
            return response;
        }

        public async Task<ResponseDto<object>> GetListTeacherProfileAsync(PaginationRequestDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var pagedTeacher = await _accountRepository.GetListTeacherProfileAsync(dto.PageNumber, dto.PageSize);
            var teacher = _mapper.Map<List<TeacherProfileResponseDto>>(pagedTeacher.Items);

            var result = new PageResultDto<TeacherProfileResponseDto>
            {
                Items = teacher,
                PageNumber = pagedTeacher.PageNumber,
                PageSize = pagedTeacher.PageSize,
                TotalCount = pagedTeacher.TotalCount
            };

            response.Success = true;
            response.Message = SystemMessage.TeacherFetchSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Data = result;
            return response;
        }

        public async Task<ResponseDto<object>> ConfirmEmailAsync(string userId, string token)
        {
            var response = new ResponseDto<object>();
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Message = SystemMessage.InvalidId;
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var res = await _userManager.FindByIdAsync(userId);
            if (res == null)
            {
                response.Message = SystemMessage.RecordNotFound;
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            if (res.EmailConfirmed)
            {
                response.Success = true;
                response.Message = SystemMessage.EmailAlreadyConfirmed;
                response.ResponseCode = (int)APIResponseCodes.OK;
                return response;
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(res, decodedToken);

            if (!result.Succeeded)
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidORExpiredEmailLink;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            response.Success = true;
            response.Message = SystemMessage.EmailConfirmedSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;

            return response;

        }

        public async Task<ResponseDto<object>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var response = new ResponseDto<object>();
            if (dto == null)
            {
                response.Message = SystemMessage.RequestBodyNull;
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var user = await _userManager.FindByEmailAsync(dto.Email.Trim());
            if (user == null)
            {
                response.Message = SystemMessage.RecordNotFound;
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"];

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new InvalidOperationException("Frontend:BaseUrl is not configured.");
            }
            var resetUrl = $"{frontendBaseUrl}/reset-password" +
                           $"?userId={Uri.EscapeDataString(user.Id)}" +
                           $"&token={Uri.EscapeDataString(encodedToken)}";
            await _emailServices.SendPasswordResetAsync(user.Email!, user.UserName ?? user.Email!, resetUrl);

            response.Success = true;
            response.Message = SystemMessage.ResetPasswordSentEmail;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var response = new ResponseDto<object>();

            if (dto == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.Token))
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidORExpiredEmailLink;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                response.Success = false;
                response.Message = SystemMessage.InvalidORExpiredEmailLink;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            response.Success = true;
            response.Message = SystemMessage.PasswordResetSuccess;
            response.ResponseCode = (int)APIResponseCodes.OK;
            return response;
        }

        public async Task<ResponseDto<object>> GetCommonProfileDto(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var Role = roles.FirstOrDefault() ?? "";

            switch (Role)
            {
                case "Student":
                    {
                        var result = await GetByIdStudentProfileAsync(userId);
                        return result;
                    }

                case "Trainer":
                    {
                        var result = await GetByIdTeacherProfileAsync(userId);
                        return result;
                    }

                default:
                    throw new Exception("Invalid role.");
            }
        }

        public async Task<ResponseDto<object>> ChangePasswordAsync(ChangePasswordDto dto, string userId)
        {
            var response = new ResponseDto<object>();

            if (dto == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RequestBodyNull;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Success = false;
                response.Message = SystemMessage.UnAuthorized;
                response.ResponseCode = (int)APIResponseCodes.UnAuthrized;
                return response;
            }

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword) ||  string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                response.Success = false;
                response.Message = SystemMessage.AllPasswordFieldRequired;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                response.Success = false;
                response.Message = SystemMessage.DoNotMatchConfirmPassword;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            if (dto.CurrentPassword == dto.NewPassword)
            {
                response.Success = false;
                response.Message = SystemMessage.DifferntCurrentPassword;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = SystemMessage.RecordNotFound;
                response.ResponseCode = (int)APIResponseCodes.NotFound;
                return response;
            }

            var result = await _userManager.ChangePasswordAsync(  user,  dto.CurrentPassword,  dto.NewPassword
            );

            if (!result.Succeeded)
            {
                response.Success = false;
                response.ResponseCode = (int)APIResponseCodes.BadRequest;

                response.Message = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)
                );

                return response;
            }

            response.Success = true;
            response.ResponseCode = (int)APIResponseCodes.OK;
            response.Message = SystemMessage.PasswordChangedSuccess;

            return response;
        }
    }
}
