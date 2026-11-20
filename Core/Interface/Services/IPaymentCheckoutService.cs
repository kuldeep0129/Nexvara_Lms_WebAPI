using Core.Dtos.Common;
using Core.Dtos.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IPaymentCheckoutService
    {
        Task<PaymentCheckoutResult> CreateCheckoutAsync(string orderReference, decimal amount, string studentId, string courseName);
        Task<ResponseDto<object>> VerifyPaymentAsync(VerifyPaymentDto dto);

    }
}
