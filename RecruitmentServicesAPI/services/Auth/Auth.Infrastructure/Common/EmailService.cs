using Auth.Application.Interfaces;
using CrudOperations;
using RecruitmentPortal.Core.Email;
using System.Net;
using System.Net.Mail;

namespace RecruitmentPortal.Infrastructure.Common;

public class EmailService(EmailConfig emailConfig) : IEmailService
{
    public async Task<Response> SendMail(string recipient, string subject, string body, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(emailConfig.Host)
            || string.IsNullOrWhiteSpace(emailConfig.FromEmail)
            || string.IsNullOrWhiteSpace(recipient))
        {
            return new Response { Status = false, Message = "Email configuration or recipient is missing." };
        }

        try
        {
            using var client = new SmtpClient(emailConfig.Host, emailConfig.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailConfig.Username, emailConfig.Password),
                EnableSsl = emailConfig.EnableSSL,
                Timeout = emailConfig.Timeout > 0 ? emailConfig.Timeout : 100000
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(emailConfig.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mail.To.Add(recipient);

            await client.SendMailAsync(mail).ConfigureAwait(false);

            return new Response { Status = true, Message = "Email sent successfully." };
        }
        catch (Exception ex)
        {
            return new Response { Status = false, Message = ex.Message };
        }
    }
}
