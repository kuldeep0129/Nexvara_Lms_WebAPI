using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Payments
{
    public class VerifyPaymentDto
    {
        [Required]
        public int EnrollmentId { get; set; }

        [Required]
        public int PaymentId { get; set; }

        [Required]
        public string RazorpayOrderId { get; set; }

        [Required]
        public string RazorpayPaymentId { get; set; }

        [Required]
        public string RazorpaySignature { get; set; }
    }
}
