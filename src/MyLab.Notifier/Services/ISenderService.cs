using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Db;
using MyLab.Notifier.Models;
using MyLab.Notifier.Options;

namespace MyLab.Notifier.Services
{
    public interface ISenderService
    {
        Task SendNotificationToSubjectAsync(string subjectId, NotificationDto notification);
        Task SendNotificationToTopicAsync(string subjectId, NotificationDto notification);
    }

    class SenderService : ISenderService
    {
        private readonly IDbManager _dbManager;
        private readonly NotifierOptions _options;

        public SenderService(IDbManager dbManager, IOptions<NotifierOptions> options)
            :this(dbManager, options.Value)
        {
            
        }

        public SenderService(IDbManager dbManager, NotifierOptions options)
        {
            _dbManager = dbManager;
            _options = options;
        }

        public Task SendNotificationToSubjectAsync(string subjectId, NotificationDto notification)
        {
            throw new System.NotImplementedException();
        }

        public Task SendNotificationToTopicAsync(string subjectId, NotificationDto notification)
        {
            throw new System.NotImplementedException();
        }
    }
}
