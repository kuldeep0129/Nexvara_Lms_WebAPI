using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Organization : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? AspNetUserId { get; set; }
        [ForeignKey(nameof(AspNetUserId))]
        public ApplicationUser? applicationUser { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? WhatsApp { get; set; }

        [MaxLength(500)]
        public string? LinkedInURL { get; set; }

        [MaxLength(500)]
        public string? FacebookURL { get; set; }

        [MaxLength(500)]
        public string? YouTubeURL { get; set; }
        public string? InstagramURL { get; set; }

        [MaxLength(500)]
        public string? TwitterURL { get; set; }

        [MaxLength(200)]
        public string? Timing { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        public string? SortLog { get; set; }
        public string? FullLogo { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? PinCode { get; set; }
        public string? MapLocation { get; set; }
    }
}
