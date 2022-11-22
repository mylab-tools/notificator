using System.ComponentModel.DataAnnotations;

namespace MyLab.Notifier.ChannelAdapter
{
    /// <summary>
    /// Contains channel adapter options
    /// </summary>
    public class NotifierChannelAdapterOptions
    {
        

        [Required] 
        public string MqQueue { get; set; }
    }
}