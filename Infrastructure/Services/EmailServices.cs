using Core.Common;
using Core.Dtos.UserInterFace;
using Core.Interface.Services;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly EmailSetting _settings;

        public EmailServices(IOptions<EmailSetting> options)
        {
            _settings = options.Value;
        }

        public async Task SendEnquiryNotificationAsync(CreateEnquiryDto enquiry, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.FromName, _settings.Username));

            message.To.Add(MailboxAddress.Parse(_settings.ToEmail));

            message.ReplyTo.Add(MailboxAddress.Parse(enquiry.Email));

            message.Subject = $"New Website Enquiry - {enquiry.Name} | Nexvara Technologies";

            message.Body = new BodyBuilder
            {
                HtmlBody = BuildHtml(enquiry),
                TextBody = $"""
                New website enquiry - Nexvara Technologies

                Name: {enquiry.Name}
                Email: {enquiry.Email}
                Phone: {enquiry.Phone}
                Qualification: {enquiry.Qualification}
                Program: {enquiry.Program}
                Calling Time: {enquiry.CallingTime}

                Message:
                {enquiry.Message}
                """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

            await smtp.SendAsync(message, cancellationToken);

            await smtp.DisconnectAsync(true, cancellationToken);
        }
        private static string E(string? value) => WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(value) ? "Not provided" : value);

        private static string BuildHtml(CreateEnquiryDto e)
        {
            return $$"""
        <!DOCTYPE html>
        <html>
        <body style="margin:0;padding:0;background:#f1f5f9;
                     font-family:Arial,sans-serif;color:#172b4d;">
          <table role="presentation" width="100%" cellpadding="0"
                 cellspacing="0" style="background:#f1f5f9;padding:24px 8px;">
            <tr><td align="center">
              <table role="presentation" width="600" cellpadding="0"
                     cellspacing="0"
                     style="max-width:600px;width:100%;background:#ffffff;
                            border-radius:12px;overflow:hidden;">
                <tr>
                  <td style="background:#0a2647;padding:28px 30px;color:#ffffff;">
                    <div style="font-size:12px;letter-spacing:2px;
                                color:#b9d7f5;font-weight:bold;">
                      NEXVARA TECHNOLOGIES
                    </div>
                    <h1 style="font-size:25px;margin:18px 0 8px;">
                      New Website Enquiry
                    </h1>
                    <p style="font-size:14px;line-height:1.6;margin:0;color:#dce8f5;">
                      A potential customer has contacted your business.
                    </p>
                  </td>
                </tr>
                <tr>
                  <td style="padding:28px 30px;">
                    <span style="display:inline-block;background:#eaf2fb;
                                 color:#205295;border-radius:20px;
                                 padding:7px 12px;font-size:11px;
                                 font-weight:bold;">
                      NEW LEAD
                    </span>

                    <h2 style="font-size:18px;margin:24px 0 16px;">
                      Customer Information
                    </h2>

                    <table role="presentation" width="100%" cellpadding="0"
                           cellspacing="0" style="font-size:14px;">
                      <tr>
                        <td style="padding:12px 8px 12px 0;border-bottom:1px solid #e8edf3;">
                          <span style="color:#718096;font-size:11px;">FULL NAME</span>
                          <div style="font-weight:bold;margin-top:6px;">{{E(e.Name)}}</div>
                        </td>
                        <td style="padding:12px 0 12px 8px;border-bottom:1px solid #e8edf3;">
                          <span style="color:#718096;font-size:11px;">PROGRAM</span>
                          <div style="font-weight:bold;margin-top:6px;">{{E(e.Program)}}</div>
                        </td>
                      </tr>
                      <tr>
                        <td style="padding:12px 8px 12px 0;border-bottom:1px solid #e8edf3;">
                          <span style="color:#718096;font-size:11px;">EMAIL ADDRESS</span>
                          <div style="margin-top:6px;">{{E(e.Email)}}</div>
                        </td>
                        <td style="padding:12px 0 12px 8px;border-bottom:1px solid #e8edf3;">
                          <span style="color:#718096;font-size:11px;">PHONE</span>
                          <div style="margin-top:6px;">{{E(e.Phone)}}</div>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="2" style="padding:12px 0;border-bottom:1px solid #e8edf3;">
                          <span style="color:#718096;font-size:11px;">QUALIFICATION</span>
                          <div style="margin-top:6px;">{{E(e.Qualification)}}</div>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="2" style="padding:12px 0;border-bottom:1px solid #e8edf3;">
                          <span style="color:#718096;font-size:11px;">PREFERRED CALLING TIME</span>
                          <div style="margin-top:6px;">{{E(e.CallingTime)}}</div>
                        </td>
                      </tr>
                    </table>

                    <h2 style="font-size:17px;margin:24px 0 12px;">Customer Message</h2>
                    <div style="background:#f5f7fa;border-left:4px solid #1e6bff;
                                padding:16px;border-radius:4px;
                                font-size:14px;line-height:1.8;
                                overflow-wrap:anywhere;">{{E(e.Message)}}</div>

                    <p style="font-size:14px;line-height:1.7;margin-top:24px;">
                      Please follow up with this enquiry at your earliest convenience.
                    </p>

                    <table role="presentation" cellpadding="0" cellspacing="0">
                      <tr>
                        <td style="background:#0a2647;border-radius:6px;">
                          <a href="mailto:{{Uri.EscapeDataString(e.Email)}}"
                             style="display:inline-block;padding:13px 20px;
                                    color:#ffffff;text-decoration:none;
                                    font-size:14px;font-weight:bold;">
                            Reply to Enquirer &#8599;
                          </a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style="background:#f8fafc;padding:20px 30px;
                             border-top:1px solid #e8edf3;">
                    <p style="font-size:12px;line-height:1.7;color:#718096;margin:0;">
                      This is an automated notification from the Nexvara Technologies
                      website. Please do not reply directly to this notification;
                      use the Reply to Enquirer link above.
                    </p>
                  </td>
                </tr>
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;
        }

        public async Task SendEmailConfirmationAsync(string email, string userName, string confirmationUrl, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add( new MailboxAddress(  _settings.FromName, _settings.Username));

            message.To.Add(   MailboxAddress.Parse(email));

            message.Subject = "Verify Your Email - Nexvara Technologies";

            message.Body = new BodyBuilder
            {
                HtmlBody = BuildEmailConfirmationHtml( userName,  confirmationUrl),

                TextBody = $"""
        Hello {userName},

        Thank you for creating your Nexvara Technologies account.

        Please verify your email address by opening the link below:

        {confirmationUrl}

        If you did not create this account, you can safely ignore this email.

        Regards,
        Nexvara Technologies
        """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(  _settings.Host, _settings.Port, SecureSocketOptions.StartTls,  cancellationToken);

            await smtp.AuthenticateAsync(  _settings.Username,  _settings.Password, cancellationToken);

            await smtp.SendAsync(  message, cancellationToken);

            await smtp.DisconnectAsync(   true,  cancellationToken);
        }
        private static string BuildEmailConfirmationHtml(
    string userName,
    string confirmationUrl)
        {
            var safeUserName = E(userName);
            var safeConfirmationUrl = WebUtility.HtmlEncode(confirmationUrl);

            return $$"""
    <!DOCTYPE html>
    <html>
    <body style="margin:0;padding:0;background:#f1f5f9;
                 font-family:Arial,sans-serif;color:#172b4d;">

      <table role="presentation"
             width="100%"
             cellpadding="0"
             cellspacing="0"
             style="background:#f1f5f9;padding:24px 8px;">

        <tr>
          <td align="center">

            <table role="presentation"
                   width="600"
                   cellpadding="0"
                   cellspacing="0"
                   style="max-width:600px;width:100%;
                          background:#ffffff;
                          border-radius:12px;
                          overflow:hidden;">

              <!-- HEADER -->
              <tr>
                <td style="background:#0a2647;
                           padding:30px;
                           color:#ffffff;">

                  <div style="font-size:12px;
                              letter-spacing:2px;
                              color:#b9d7f5;
                              font-weight:bold;">
                    NEXVARA TECHNOLOGIES
                  </div>

                  <h1 style="font-size:25px;
                             margin:18px 0 8px;">
                    Verify Your Email
                  </h1>

                  <p style="font-size:14px;
                            line-height:1.6;
                            margin:0;
                            color:#dce8f5;">
                    One quick step to activate your account.
                  </p>

                </td>
              </tr>

              <!-- CONTENT -->
              <tr>
                <td style="padding:32px 30px;">

                  <p style="font-size:15px;
                            line-height:1.7;
                            margin:0 0 15px;">
                    Hello <strong>{{safeUserName}}</strong>,
                  </p>

                  <p style="font-size:14px;
                            line-height:1.8;
                            color:#4a5568;">

                    Thank you for creating your account with
                    <strong>Nexvara Technologies</strong>.
                    Please verify your email address to complete
                    your registration.
                  </p>

                  <!-- BUTTON -->
                  <table role="presentation"
                         cellpadding="0"
                         cellspacing="0"
                         style="margin:28px 0;">

                    <tr>
                      <td style="background:#0a2647;
                                 border-radius:7px;">

                        <a href="{{safeConfirmationUrl}}"
                           style="display:inline-block;
                                  padding:14px 24px;
                                  color:#ffffff;
                                  text-decoration:none;
                                  font-size:14px;
                                  font-weight:bold;">

                          Verify Email Address &#8594;

                        </a>

                      </td>
                    </tr>

                  </table>

                  <p style="font-size:13px;
                            line-height:1.7;
                            color:#718096;">

                    If the button above does not work, copy and paste
                    the following link into your browser:

                  </p>

                  <div style="background:#f5f7fa;
                              padding:12px;
                              border-radius:6px;
                              font-size:12px;
                              line-height:1.6;
                              word-break:break-all;">

                    {{safeConfirmationUrl}}

                  </div>

                  <p style="font-size:13px;
                            line-height:1.7;
                            color:#718096;
                            margin-top:24px;">

                    If you did not create this account, you can safely
                    ignore this email.

                  </p>

                </td>
              </tr>

              <!-- FOOTER -->
              <tr>
                <td style="background:#f8fafc;
                           padding:20px 30px;
                           border-top:1px solid #e8edf3;">

                  <p style="font-size:12px;
                            line-height:1.7;
                            color:#718096;
                            margin:0;">

                    This is an automated email from Nexvara Technologies.
                    Please do not reply to this email.

                  </p>

                  <p style="font-size:12px;
                            color:#a0aec0;
                            margin:10px 0 0;">

                    © 2026 Nexvara Technologies. All rights reserved.

                  </p>

                </td>
              </tr>

            </table>

          </td>
        </tr>

      </table>

    </body>
    </html>
    """;
        }

        public async Task SendPasswordResetAsync(string email, string userName, string resetUrl, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _settings.FromName,
                    _settings.Username));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject =
                "Reset Your Password - Nexvara Technologies";

            message.Body = new BodyBuilder
            {
                HtmlBody = BuildPasswordResetHtml(
                    userName,
                    resetUrl),

                TextBody = $"""
        Hello {userName},

        We received a request to reset your Nexvara Technologies account password.

        Reset your password using this link:

        {resetUrl}

        If you did not request a password reset, you can safely ignore this email.

        Regards,
        Nexvara Technologies
        """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync( _settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync( _settings.Username, _settings.Password,cancellationToken);
            await smtp.SendAsync( message, cancellationToken);
            await smtp.DisconnectAsync(  true, cancellationToken);
        }

        private static string BuildPasswordResetHtml(
    string userName,
    string resetUrl)
        {
            var safeUserName = E(userName);
            var safeResetUrl = WebUtility.HtmlEncode(resetUrl);

            return $$"""
    <!DOCTYPE html>
    <html>
    <body style="margin:0;padding:0;background:#f1f5f9;
                 font-family:Arial,sans-serif;color:#172b4d;">

      <table role="presentation"
             width="100%"
             cellpadding="0"
             cellspacing="0"
             style="background:#f1f5f9;padding:24px 8px;">

        <tr>
          <td align="center">

            <table role="presentation"
                   width="600"
                   cellpadding="0"
                   cellspacing="0"
                   style="max-width:600px;width:100%;
                          background:#ffffff;
                          border-radius:12px;
                          overflow:hidden;">

              <tr>
                <td style="background:#0a2647;
                           padding:30px;
                           color:#ffffff;">

                  <div style="font-size:12px;
                              letter-spacing:2px;
                              color:#b9d7f5;
                              font-weight:bold;">
                    NEXVARA TECHNOLOGIES
                  </div>

                  <h1 style="font-size:25px;
                             margin:18px 0 8px;">
                    Reset Your Password
                  </h1>

                  <p style="font-size:14px;
                            line-height:1.6;
                            margin:0;
                            color:#dce8f5;">
                    Secure your account with a new password.
                  </p>

                </td>
              </tr>

              <tr>
                <td style="padding:32px 30px;">

                  <p style="font-size:15px;
                            line-height:1.7;">
                    Hello <strong>{{safeUserName}}</strong>,
                  </p>

                  <p style="font-size:14px;
                            line-height:1.8;
                            color:#4a5568;">

                    We received a request to reset the password
                    for your Nexvara Technologies account.

                  </p>

                  <p style="font-size:14px;
                            line-height:1.8;
                            color:#4a5568;">

                    Click the button below to create a new password.

                  </p>

                  <table role="presentation"
                         cellpadding="0"
                         cellspacing="0"
                         style="margin:28px 0;">

                    <tr>
                      <td style="background:#0a2647;
                                 border-radius:7px;">

                        <a href="{{safeResetUrl}}"
                           style="display:inline-block;
                                  padding:14px 24px;
                                  color:#ffffff;
                                  text-decoration:none;
                                  font-size:14px;
                                  font-weight:bold;">

                          Reset Password &#8594;

                        </a>

                      </td>
                    </tr>

                  </table>

                  <p style="font-size:13px;
                            line-height:1.7;
                            color:#718096;">

                    If the button does not work, copy and paste
                    the following link into your browser:

                  </p>

                  <div style="background:#f5f7fa;
                              padding:12px;
                              border-radius:6px;
                              font-size:12px;
                              line-height:1.6;
                              word-break:break-all;">

                    {{safeResetUrl}}

                  </div>

                  <div style="margin-top:24px;
                              background:#fff8e6;
                              border-left:4px solid #d69e2e;
                              padding:14px;
                              font-size:13px;
                              line-height:1.7;
                              color:#744210;">

                    If you did not request a password reset,
                    you can safely ignore this email.

                  </div>

                </td>
              </tr>

              <tr>
                <td style="background:#f8fafc;
                           padding:20px 30px;
                           border-top:1px solid #e8edf3;">

                  <p style="font-size:12px;
                            line-height:1.7;
                            color:#718096;
                            margin:0;">

                    This is an automated email from Nexvara Technologies.
                    Please do not reply to this email.

                  </p>

                  <p style="font-size:12px;
                            color:#a0aec0;
                            margin:10px 0 0;">

                    © 2026 Nexvara Technologies.
                    All rights reserved.

                  </p>

                </td>
              </tr>

            </table>

          </td>
        </tr>

      </table>

    </body>
    </html>
    """;
        }
    }
}
