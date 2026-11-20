using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Payments
{
    public class PaymentCheckoutResult
    {
        public bool Success { get; set; }

        public string? OrderId { get; set; }

        public string? KeyId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";

        public string? ErrorMessage { get; set; }
    }
}
