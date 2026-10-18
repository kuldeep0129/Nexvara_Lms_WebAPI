using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    public class CourseSettings : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public string? Icon { get; set; } 
        public string? ColorCode { get; set; } 
        public string? SoftColorCode { get; set; } 
    }
}
