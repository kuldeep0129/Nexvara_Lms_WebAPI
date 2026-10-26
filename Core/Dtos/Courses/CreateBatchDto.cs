using Domain.Entities.UserInterface;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class CreateBatchDto
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Capacity { get; set; }

        public BatchStatus Status { get; set; } = BatchStatus.Draft;

        public bool IsPublished { get; set; } = false;
    }
}
