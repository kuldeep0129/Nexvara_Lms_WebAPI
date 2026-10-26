using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Teacher
{
    public class TeacherProfiles : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? user { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? ProfileImagePublicId { get; set; }
        public string? Qualification { get; set; }

        public decimal? ExperienceYears { get; set; }
        public string? PassingYear { get; set; }
        public string? Bio { get; set; }
        public string? Specialization { get; set; }
    }
}
