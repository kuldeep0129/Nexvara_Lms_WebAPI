using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.LiveClasses
{
    public class GetClassAttendanc
    {
        public int Id { get; set; }
        public int? LiveClassId { get; set; }
        public string LiveClassTitle { get; set; }
        public string? StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime LeftAt { get; set; }
        public DateTime DurationInSeconds { get; set; }
        public bool IsActive { get; set; }
    }
}
