using Domain.Entities;
using Domain.Entities.Student;
using Domain.Entities.Teacher;
using Domain.Entities.UserInterface;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AdmissionApplication> AdmissionApplications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        //public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<SiteContent> SiteContents { get; set; }
        public DbSet<StudentAchievement> StudentAchievements { get; set; }
        public DbSet<StudentProject> StudentProjects { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<ApplicationLogs> ApplicationLogs { get; set; }
        public DbSet<ProgramType> ProgramTypes { get; set; }
        public DbSet<ProgramSetting> ProgramSettings { get; set; }
        public DbSet<CourseLastProject> CourseLastProjects { get; set; }
        public DbSet<CourseSettings> CourseSettings { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<TeacherProfiles> TeacherProfiles { get; set; }

    }
}
