using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.common
{
    public static class SystemMessage
    {
        public const string SaveSuccess = "Data Saved Successfull..";
        public const string LoginSuccess = "Login Successfull..";
        public const string UnAuthorized = "User is UnAuthrized.";
        public const string UserRegisterSuccess = "User registered and role assigned successfully.";
        public const string InvalidPassword = "Password is Invalid.";
        public const string UpdateProfileSuccess = "Profile Updated Successfull.";
        public const string InvalidId = "Id is Invalid.";
        public const string StudentNotFound = "Student profile not found..";
        public const string TeacherNotFound = "Teacher profile not found..";
        public const string UserAlreadyLogout = "User already logout.";
        public const string UpdateStudentProfileSuccess = "Student Profile Success.";
        public const string StudentFetchSuccess = "Students fetched successfully.";
        public const string TeacherFetchSuccess = "Teacher fetched successfully.";
        public const string LogoutSuccess = "Logout is Successfully.";
        public const string InvalidRefershToken = "Refersh Token is Invalid.";
        public const string RequestBodyNull = "Request Body is Null..";
        public const string ContentAlreadyExists = "Content for this section already exists.";
        public const string ResourcesAlreadyExists = "A resource with this slug already exists.";
        public const string ProgramAlreadyExists = "A program with this slug already exists..";
        public const string CourseAlreadyExists = "A course with this slug already exists.";
        public const string ProgramNotExists = "Program is not Exist or not Publish..";
        public const string InvalidStartDate = "End date must be greater than or equal to start date.";
        public const string SectionKeyRequired = "Section key is required.";
        public const string SiteContentRetrive = "Site content retrieved successfully.";
        public const string NotFoundSiteContent = "Site content not found.";
        public const string ProgramRetrieve = "Programs retrieved successfully.";
        public const string CourseRetrieve = "Course retrieved successfully.";
        public const string StudentachievementsRetrieve = "Student achievements retrieved successfully.";
        public const string StudentProjectRetrieve = "Student Project retrieved successfully.";
        public const string TestimonialsRetrieve = "Testimonials retrieved successfully.";
        public const string ActiveannouncementsRetrieve = "Active announcements retrieved successfully.";
        public const string RecordNotFound = "Record not Found.";
        public const string PasswordResetSuccess = "Password Reset Success.";
        public const string ResetPasswordSentEmail = "PasswordResetEmailSent";
        public const string EmailAlreadyConfirmed = "Email is already confirmed.";
        public const string EmailConfirmedSuccess = "Email confirmed successfully.";
        public const string InvalidORExpiredEmailLink = "Invalid or expired email confirmation link.";
        public const string RecordCreate = "Record Create Successfull.";
        public const string RecordUpdate = "Record Update Successfull.";
        public const string RecordFetch = "Record Fetch Successfull.";
        public const string RecordStatusChange = "Record Status  Changed Successfull.";
    }
}
