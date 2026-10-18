using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class Certificates : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }
        public string? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? course { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? VerificationCode { get; set; }
    }
}
