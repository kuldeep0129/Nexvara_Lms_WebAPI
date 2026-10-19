using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class CreateBatchStudentDto
    {
        public int? BatchId { get; set; }
        public string? StudentId { get; set; }
        public DateTime? EnrolledAt { get; set; }
        public DateTime? CompleteAt { get; set; }
    }
}
