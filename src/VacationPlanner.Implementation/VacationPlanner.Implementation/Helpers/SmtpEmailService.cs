using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.Helpers
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(NotificationMessage notifficationMessage)
        {
            var smtp = _config.GetSection("Smtp");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
            {
                EnableSsl = false,
                UseDefaultCredentials = false
            };

            foreach (var recipient in notifficationMessage.RecipientMails)
            {
                var message = new MailMessage(
                    smtp["From"],
                    recipient,
                    notifficationMessage.Subject,
                    notifficationMessage.Body
                );

                await client.SendMailAsync(message);
            }
        }
    }
}
