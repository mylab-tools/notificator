using System;
using LinqToDB;
using LinqToDB.DataProvider.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Db;
using MyLab.Notifier.ChannelAdapter;
using MyLab.Notifier.EmailSender.Options;
using MyLab.Notifier.EmailSender.Services;

namespace MyLab.Notifier.EmailSender
{
    public static class MailSenderApp
    {
        public const string SmtpSectionName = "Smtp";
        
        public static IServiceCollection AddMailSenderLogic(this IServiceCollection srv)
        {
            return srv.AddNotifierChannelLogic<NotifierEmailChannelLogic>()
                .AddSingleton<IEmailSender, Services.EmailSender>()
                .AddDbTools(new MySqlDataProvider(ProviderName.MySql));
        }

        public static IServiceCollection ConfigureMailSender(this IServiceCollection srv, IConfiguration configuration)
        {
            srv.ConfigureNotifierChannelLogic(configuration);

            srv.AddOptions<SmtpOptions>()
                .Bind(configuration.GetSection(SmtpSectionName))
                .ValidateDataAnnotations();

            srv.AddOptions<EmailChannelOptions>()
                .Bind(configuration.GetSection(ChannelAdapter.AppIntegration.SectionName))
                .ValidateDataAnnotations();

            return srv;
        }

        public static IServiceCollection ConfigureMailSender(this IServiceCollection srv,
            Action<EmailChannelOptions> configMailChannel,
            Action<SmtpOptions> configSmtp = null)
        {
            srv.Configure(configMailChannel);

            if (configSmtp != null)
                srv.Configure(configSmtp);
    
            return srv;
        }
    }
}
