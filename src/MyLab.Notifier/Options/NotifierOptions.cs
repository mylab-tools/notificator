namespace MyLab.Notifier.Options
{
    public class NotifierOptions
    {
        public ChannelOptions Channels { get; set; }
    }

    public class ChannelOptions
    {
        public string Id { get; set; }

        public string Queue { get; set; }
    }
}
