using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class CreateClassAttendanceDto
    {
        public int? LiveClassId { get; set; }
        public string? StudentId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime LeftAt { get; set; }
        public DateTime DurationInSeconds { get; set; }
    }
}
