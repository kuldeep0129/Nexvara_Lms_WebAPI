using Domain.Entities.Student;
using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public class Payments : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? user { get; set; }
        public int? EnrollmentId { get; set; }
        [ForeignKey(nameof(EnrollmentId))]
        public Enrollment? enrollment { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentGateway { get; set; }
        public string? PaymentStatus { get; set; }
        public string? ProviderEventId { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }


    }
}
