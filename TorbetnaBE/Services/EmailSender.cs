using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using TorbetnaBE.Dtos;

namespace TorbetnaBE.Services
{
    public class EmailSender
    {
        private readonly string _apiKey;

        public EmailSender(IOptions<SendGridSettings> options)
        {
            _apiKey = options.Value.ApiKey;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("202111210@students.asu.edu.jo", "Torbetna ASU");
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
