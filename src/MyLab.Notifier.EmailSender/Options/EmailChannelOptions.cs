using System.ComponentModel.DataAnnotations;

namespace MyLab.Notifier.EmailSender.Options
{
    /// <summary>
    /// Contains Email channel options
    /// </summary>
    public class EmailChannelOptions
    {
        /// <summary>
        /// Defines maximum hidden copies number (BCC: blind carbon copy)
        /// </summary>
        /// <remarks>Available range 1...2000. 100 - by default</remarks>
        [Range(1, 2000)]
        public int BccLimit { get; set; } = 100;
    }
}