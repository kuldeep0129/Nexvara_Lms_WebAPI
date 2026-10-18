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
    public class CourseLastProject : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
