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
        public const string AlreadyUseEmail = "The email address {Email} is already registered.";
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
        public const string InvalidCourse = "Invalid course.";
        public const string BatchNameIsRequired = "Batch name is required.";
        public const string PasswordChangedSuccess = "Password changed successfully";
        public const string AllPasswordFieldRequired = "All password fields are required.";
        public const string DifferntCurrentPassword = "New password must be different from current password.";
        public const string DoNotMatchConfirmPassword = "New password and confirm password do not match.";
        public const string RecordStatusChange = "Record Status  Changed Successfull.";
        public const string InvalidEndDate = "End date cannot be earlier than start date.";
        public const string InvalidCapacity = "Capacity must be greater than zero.";
        public const string BatchNameAlreadyExits = "Batch with this name already exists for this course.";
        public const string BatchCodeAlreadyExits = "Batch code already exists.";
        public const string BatchNotFound="Batch not found.";
        public const string CourseNotFound="Course not found.";
        public const string InvalidBatchStatus= "Invalid batch status.";
        public const string CannotBeChangeCompleteStatus= "Completed batch status cannot be changed.";
        public const string CannotBeChangeCancelStatus= "Cancelled batch status cannot be changed.";
        public const string CannotBePublishCancelBatch= "Cancelled batch cannot be published.";
        public const string CannotBePublishCompleteBatch= "Completed batch cannot be published.";
        public const string BatchSucessfullPublish= "Batch published successfully.";
        public const string BatchUnSucessfullPublish= "Batch unpublished successfully";
        public const string BatchAlreadyyyArchived = "Batch is already archived.";
        public const string ActiveBatchCannotBeArchived = "Active batch cannot be archived. Complete or cancel the batch first.";
        public const string InvalidScheduledata = "Invalid schedule data.";
        public const string InvalidEndTime = "End time must be greater than start time.";
        public const string ScheduleAlreadyExistsBranch = "This schedule already exists for this batch.";
        public const string ScheduleNotFoundThisBatch = "Schedule not found for this batch.";
        public const string ScheduleSameDayAlreadyExits = "A schedule with the same day and time already exists";
        public const string TeacherIsRequired = "Teacher is required.";
        public const string TeacherAlreadyHasPrimary = "This batch already has a primary teacher.";
        public const string TeacherAssignedNotFound = "Teacher assignment not found.";
        public const string ThisBatchAlreadyHoldPrimaryTeacher = "This batch already has a primary teacher.";
        public const string TeacherIsAlreadyAssignedThisBatch = "Teacher is already assigned to this batch.";

    }
}
