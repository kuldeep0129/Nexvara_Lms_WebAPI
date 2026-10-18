using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class CollegeCourses : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public int? CollegeId { get; set; }
        [ForeignKey(nameof(CollegeId))]
        public Colleges? Colleges { get; set; }
        public string? CourseName { get; set; }
        public string? BranchName { get; set; }
        public decimal? CollegeFee { get; set; }
        public decimal? ActualFee { get; set; }
        public decimal? DurationYears { get; set; }
        public string? Eligibility { get; set; }
    }
}
