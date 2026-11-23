using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Certificate
{
    public class IssueCertificateDto
    {
        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        [MaxLength(1000)]
        public string? CertificateUrl { get; set; }
    }

    public class StudentCertificateDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public string? CertificateUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class CertificateVerificationDto
    {
        public bool IsValid { get; set; }
        public string CertificateNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
    }
}
