using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Notifier.Share;

namespace MyLab.Notifier.ChannelAdapter
{
    /// <summary>
    /// Contains tools for integrations into application
    /// </summary>
    public static class AppIntegration
    {
        public const string SectionName = "NotifierChannel";

        /// <summary>
        /// Adds notifier channel logic
        /// </summary>
        /// <typeparam name="TChannel">logic type</typeparam>
        public static IServiceCollection AddNotifierChannelLogic<TChannel>(this IServiceCollection srv)
            where TChannel : class, INotifierChannel
        {
            return srv.AddRabbit()
                .AddRabbitConsumer<NotifierChannelAdapterOptions, NotifierEnvelopConsumer>(opt => opt.MqQueue)
                .AddSingleton<INotifierChannel, TChannel>();
        }

        /// <summary>
        /// Adds notifier channel logic
        /// </summary>
        public static IServiceCollection AddNotifierChannelLogic(this IServiceCollection srv, NotifierEnvelopConsumer consumer, IConfiguration configuration)
        {
            return srv.AddRabbit()
                .AddRabbitConsumer<NotifierChannelAdapterOptions, NotifierEnvelopConsumer>(consumer, opt => opt.MqQueue);
        }

        public static IServiceCollection ConfigureNotifierChannelLogic(this IServiceCollection srv,
            IConfiguration configuration, string sectionName = SectionName)
        {
            srv.ConfigureRabbit(configuration)
                .AddOptions<NotifierChannelAdapterOptions>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();

            return srv;
        }
    }
}
