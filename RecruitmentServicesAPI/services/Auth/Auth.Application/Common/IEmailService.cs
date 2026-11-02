using CrudOperations;

namespace Auth.Application.Interfaces;

public interface IEmailService
{
    Task<Response> SendMail(string recipient, string subject, string body, bool isHtml = true);
}
