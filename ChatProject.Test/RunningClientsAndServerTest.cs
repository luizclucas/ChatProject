using ChatProject.ClientSide;
using ChatProject.Domain.Services;
using ChatProject.Server;
using System.Threading.Tasks;
using Xunit;

namespace ChatProject.Test
{
    public class RunningClientsAndServerTest
    {
        private RunningServer _runningServer = new RunningServer(new ConnectionDomainService());
        private RunningClient _runningClient = new RunningClient(new ConnectionDomainService(), new MessageHandleDomainService());

        public RunningClientsAndServerTest()
        {
        }

        [Fact]
        public async Task Connect_Client()
        {
            Task.Run(() => _runningServer.StartListening());
            await Task.Delay(2000);
            var client = await _runningClient.ConnectAsync();
            Assert.True(client.Socket.Connected);
        }

    }
}
