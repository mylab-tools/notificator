namespace MyLab.Notifier.Api.Options
{
    public class NotifierOptions
    {
        public ChannelOptions[] Channels { get; set; }
    }

    public class ChannelOptions
    {
        public string Id { get; set; }
    }
}
