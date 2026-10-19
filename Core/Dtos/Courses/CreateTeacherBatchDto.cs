using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class CreateTeacherBatchDto
    {

        public int? BatchId { get; set; }
        public string? TeacherId { get; set; }
        public bool IsPrimary { get; set; }
        public DateOnly AssignedAt { get; set; }
    }
}
