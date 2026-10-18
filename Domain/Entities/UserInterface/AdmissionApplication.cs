using Domain.Entities.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    [Index(nameof(ApplicationNumber), IsUnique = true)]
    public class AdmissionApplication : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string ApplicationNumber { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public int ProgramId { get; set; }

        [MaxLength(150)]
        public string? Qualification { get; set; }

        [MaxLength(2000)]
        public string? Message { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = EntityStatus.Submitted.ToString();

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ProgramId))]
        public Program Program { get; set; } = null!;
    }
}
