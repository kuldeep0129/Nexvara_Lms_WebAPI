using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class BaseDbModel
    {
        public bool IsActive { get; set; }
        public string? CreateBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string? UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; } 
    }
}
