using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using VacationPlanner.Interfaces;

namespace VacationPlanner.Implementation
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = _config.GetSection("Smtp");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
            {
                EnableSsl = false,
                UseDefaultCredentials = false
            };

            var message = new MailMessage(
                smtp["From"],
                to,
                subject,
                body
            );

            await client.SendMailAsync(message);
        }
    }
}
