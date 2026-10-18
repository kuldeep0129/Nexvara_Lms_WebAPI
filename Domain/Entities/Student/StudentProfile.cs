using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class StudentProfile : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? user { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? ProfileImagePublicId { get; set; }
        public DateTime? DateOfBirth { get; set; }
       public string? Gender { get; set; }
       public string? Qualification { get; set; }
       public string? CollegeName { get; set; }
       public int? PassingYear { get; set; }
       public string? Address { get; set; }



    }
}
