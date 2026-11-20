using Core.Interface.Repository;
using Domain.Entities.Common;
using Domain.Entities.Student;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetEnrollmentByIdAsync(int enrollmentId)
        {
            return await _context.Enrollments.Include(e => e.batch).ThenInclude(b => b.course).FirstOrDefaultAsync(e => e.Id == enrollmentId);
        }

        public async Task<Payments?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<Payments?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();

        }

        public async Task UpdateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await Task.CompletedTask;
        }

        public async Task UpdatePaymentAsync(Payments payment)
        {
            _context.Payments.Update(payment);
            await Task.CompletedTask;
        }
    }
}
