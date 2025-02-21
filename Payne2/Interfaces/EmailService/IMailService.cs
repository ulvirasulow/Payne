using Payne.Helpers.Email;

namespace Payne.Interfaces.EmailService;

public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}