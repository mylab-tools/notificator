using System.ComponentModel.DataAnnotations;

namespace MyLab.Notifier.MailSender.Options
{
    /// <summary>
    /// Contains SMTP connection options
    /// </summary>
    class SmtpOptions
    {
        /// <summary>
        /// Server host name
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        /// <remarks>587 by default</remarks>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Specify whether a Secure Sockets Layer (SSL) should be using to encrypt the connection
        /// </summary>
        /// <remarks>false - by default</remarks>
        public bool EnableSsl { get; set; } = false;

        /// <summary>
        /// Sender email
        /// </summary>
        [Required]
        public string FromEmail { get; set; }

        /// <summary>
        /// Sender Name
        /// </summary>
        /// <remarks>Empty by default</remarks>
        public string FromName { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}