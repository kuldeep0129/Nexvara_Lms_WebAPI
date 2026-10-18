using Core.Dtos.UserInterFace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IEmailServices
    {
        Task SendEnquiryNotificationAsync(CreateEnquiryDto enquiry,  CancellationToken cancellationToken = default);

        Task SendEmailConfirmationAsync(   string email, string userName,string confirmationUrl, CancellationToken cancellationToken = default);
        Task SendPasswordResetAsync( string email,string userName, string resetUrl,CancellationToken cancellationToken = default);

    }
}

