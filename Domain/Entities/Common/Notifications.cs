using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class Notifications : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser user { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        
    }
}
