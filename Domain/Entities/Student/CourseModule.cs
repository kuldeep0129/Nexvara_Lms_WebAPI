using Domain.Entities.Common;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Student
{
    public class CourseModule : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course course { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Sortorder { get; set; }
    }
}
