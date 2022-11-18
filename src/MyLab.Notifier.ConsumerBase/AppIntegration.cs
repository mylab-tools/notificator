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
        /// <summary>
        /// Adds notifier channel logic
        /// </summary>
        /// <typeparam name="TChannel">logic type</typeparam>
        public static IServiceCollection AddNotifierChannelLogic<TChannel>(this IServiceCollection services, IConfiguration configuration)
            where TChannel : INotifierChannel
        {
            return services.AddRabbit()
                .ConfigureRabbit(configuration)
                .Configure<NotifierChannelAdapterOptions>(configuration.GetSection("ChannelAdapter"))
                .AddRabbitConsumer<NotifierChannelAdapterOptions, NotifierEnvelopConsumer<TChannel>>(opt => opt.MqQueue); 
        }

        /// <summary>
        /// Adds notifier channel logic
        /// </summary>
        public static IServiceCollection AddNotifierChannelLogic(this IServiceCollection services, NotifierEnvelopConsumer<INotifierChannel> consumer, IConfiguration configuration)
        {
            return services.AddRabbit()
                .ConfigureRabbit(configuration)
                .Configure<NotifierChannelAdapterOptions>(configuration.GetSection("ChannelAdapter"))
                .AddRabbitConsumer<NotifierChannelAdapterOptions, NotifierEnvelopConsumer<INotifierChannel>>(consumer, opt => opt.MqQueue);
        }
    }
}
