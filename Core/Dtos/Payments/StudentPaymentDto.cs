using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Payments
{
    public class StudentPaymentDto
    {
        public int Id { get; set; }
        public int? EnrollmentId { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentGateway { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
