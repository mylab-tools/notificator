using LinqToDB;
using LinqToDB.DataProvider.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Db;
using MyLab.Notifier.ChannelAdapter;
using MyLab.Notifier.MailSender.Options;
using MyLab.Notifier.MailSender.Services;

namespace MyLab.Notifier.MailSender
{
    public static class MailSenderApp
    {
        public const string SmtpSectionName = "Smtp";
        
        public static IServiceCollection AddMailSender(this IServiceCollection srv)
        {
            return srv.AddNotifierChannelLogic<NotifierEmailChannelLogic>()
                .AddSingleton<IEmailSender, EmailSender>()
                .AddDbTools(new MySqlDataProvider(ProviderName.MySql));
        }

        public static IServiceCollection ConfigureMailSender(this IServiceCollection srv, IConfiguration configuration)
        {
            srv.ConfigureNotifierChannelLogic(configuration)
                .ConfigureDbTools(configuration);

            srv.AddOptions<SmtpOptions>()
                .Bind(configuration.GetSection(SmtpSectionName))
                .ValidateDataAnnotations();

            srv.AddOptions<MailChannelOptions>()
                .Bind(configuration.GetSection(ChannelAdapter.AppIntegration.SectionName))
                .ValidateDataAnnotations();

            return srv;
        }
    }
}
