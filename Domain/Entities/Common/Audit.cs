using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class Audit : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser user { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }

    }
}
