using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Notifier.MailSender.Options;

namespace MyLab.Notifier.MailSender.Services
{
    class EmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly SmtpClient _client;

        public EmailSender(IOptions<SmtpOptions> options)
            :this(options.Value)
        {
            
        }

        public EmailSender(SmtpOptions options)
        {
            _options = options;
            _client = new SmtpClient(options.Host, options.Port)
            {
                Credentials = new NetworkCredential(options.Username, options.Username),
                EnableSsl = options.EnableSsl
            };
        }

        public async Task SendNotificationAsync(string[] contacts, EmailEnvelop envelop)
        {
            var from = _options.FromName != null
                ? new MailAddress(_options.FromEmail, _options.FromName)
                : new MailAddress(_options.FromEmail);

            var to = new MailAddress(contacts.First());
            
            var mailMsg = new MailMessage(from, to);

            if (contacts.Length > 1)
            {
                foreach (var contact in contacts.Skip(1))
                {
                    mailMsg.Bcc.Add(contact);
                }
            }

            await _client.SendMailAsync(mailMsg);
        }
    }
}