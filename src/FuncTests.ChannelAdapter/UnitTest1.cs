
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using MyLab.Notifier.ChannelAdapter;
using MyLab.Notifier.Share;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests.ChannelAdapter
{
    public class ChannelAdapterBehavior
    {
        private readonly ITestOutputHelper _output;

        public ChannelAdapterBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ShouldSendNotificationToSubject()
        {
            //Arrange
            var channelMock = new Mock<INotifierChannel>();

            channelMock.Setup(ch => ch.)

            var serviceCollection = new ServiceCollection()
                .AddLogging(l => l.AddFilter(l => true).AddXUnit(_output))
                .AddNotifierChannelLogic<>();

            //Act


            //Assert

        }
    }
}
