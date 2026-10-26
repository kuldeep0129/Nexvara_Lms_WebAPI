using Core.Dtos.Account;
using Core.Dtos.Common;
using Core.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IAccountServices
    {
        Task<ResponseDto<object>> LoginAsync(LoginDto dto);
        Task<ResponseDto<object>> RefreshAsync(string dto);
        Task<ResponseDto<object>> LogoutAsync( LogoutDto dto,   string userId, string? ipAddress);
        Task<ResponseDto<object>> ConfirmEmailAsync(string userId, string token);
        Task<ResponseDto<object>> RegisterAsync(RegistrationDto dto);

        //Forget Password 
        Task<ResponseDto<object>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ResponseDto<object>> ResetPasswordAsync(ResetPasswordDto dto);

        //Student
        Task<ResponseDto<object>> StudentProfileAsync(CreateStudentProfile profile, string userId);
        Task<ResponseDto<object>> UpdateStudentProfileAsync(CreateStudentProfile profile, string userId);
        Task<ResponseDto<object>> UpdateStatusStudentProfileAsync(string userId);
        Task<ResponseDto<object>> GetByIdStudentProfileAsync(string userId);
        Task<ResponseDto<object>> GetListStudentProfileAsync(PaginationRequestDto dto);


        //Teacher 
        Task<ResponseDto<object>> TeacherProfileAsync(CreateTeacherProfile profile, string userId);
        Task<ResponseDto<object>> UpdateTeacherProfileAsync(CreateTeacherProfile profile, string userId);
        Task<ResponseDto<object>> UpdateStatusTeacherProfileAsync(string userId);
        Task<ResponseDto<object>> GetByIdTeacherProfileAsync(string userId);
        Task<ResponseDto<object>> GetListTeacherProfileAsync(PaginationRequestDto dto);

        //common profile
        Task<ResponseDto<object>> GetCommonProfileDto(string userId );
        Task<ResponseDto<object>> ChangePasswordAsync(ChangePasswordDto dto, string userId
);

    }
}
