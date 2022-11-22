using System.ComponentModel.DataAnnotations;
using MyLab.Notifier.ChannelAdapter;

namespace MyLab.Notifier.MailSender.Options
{
    /// <summary>
    /// Contains Mail channel options
    /// </summary>
    class MailChannelOptions : NotifierChannelAdapterOptions
    {
        /// <summary>
        /// Defines maximum hidden copies number (BCC: blind carbon copy)
        /// </summary>
        /// <remarks>Available range 1...2000. 100 - by default</remarks>
        [Range(1, 2000)]
        public int BccLimit { get; set; } = 100;
    }
}