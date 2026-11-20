using Domain.Entities.Common;
using Domain.Entities.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Repository
{
    public interface IPaymentRepository
    {
        Task<Payments?> GetPaymentByIdAsync(int paymentId);
        Task<Payments?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<Enrollment?> GetEnrollmentByIdAsync(int enrollmentId);
        Task UpdatePaymentAsync(Payments payment);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task SaveAsync();
    }
}
