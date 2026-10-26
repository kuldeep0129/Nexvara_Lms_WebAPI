using Domain.Entities.Common;
using Domain.Entities.Teacher;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Batch : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? course { get; set; }
        public string? Name { get; set; }

        public string? BatchCode { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Capacity { get; set; }
        public string? Status { get; set; }
        public bool IsPublished { get; set; }
        public ICollection<BatchTeacher> BatchTeachers { get; set; }   = new List<BatchTeacher>();
    }
}
