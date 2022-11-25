using System.Threading.Tasks;

namespace MyLab.Notifier.EmailSender.Services
{
    interface IEmailSender
    {
        Task SendNotificationAsync(string[] contacts, EmailEnvelop envelop);
    }

    class EmailEnvelop
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}