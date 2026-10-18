using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.UserInterface
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Program : BaseDbModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(180)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Summary { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ProgramTypeId { get; set; }
        [ForeignKey(nameof(ProgramTypeId))]
        public ProgramType? ProgramType { get; set; }

        public int? Durationvalue { get; set; }
        public string? DurationUnit { get; set; }

        [MaxLength(50)]
        public string? Mode { get; set; }

        [MaxLength(500)]
        public string? Eligibility { get; set; }

        [Precision(12, 2)]
        public decimal? Fee { get; set; }

        public string? WithoutFee { get; set; }
        public string? Point { get; set; }

        public bool IsPublished { get; set; } = false;

        public int? SortOrder { get; set; } = 0;


    }
}
