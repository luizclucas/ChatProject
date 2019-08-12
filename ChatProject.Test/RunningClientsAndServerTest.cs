using ChatProject.ClientSide;
using ChatProject.Domain.Entities;
using ChatProject.Domain.Services;
using ChatProject.Server;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;


namespace ChatProject.Test
{
    [DefaultPriority(0)]
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class RunningClientsAndServerTest
    {
        private static RunningServer _runningServer = new RunningServer(new ConnectionDomainService());
        private static RunningClient _firstClientSide;
        private static RunningClient _secondClientSide;
        private static RunningClient _thirdClientSide;
        private static Client _firstClient; //Luiz
        private static Client _secondClient; //Gabriel
        private static Client _thirdCLient; //Carol

        public RunningClientsAndServerTest()
        {
            Task.Run(() => _runningServer.StartListening());
        }

        [Fact, Priority(0)]
        public async Task Connect_Client()
        {
            _firstClientSide = new RunningClient(new ConnectionDomainService(), new MessageHandleDomainService());
            _firstClient = await _firstClientSide.ConnectAsync();
            Assert.True(_firstClient.Socket.Connected);
        }

        [Fact, Priority(1)]
        public async Task Send_Name_Client()
        {
            var response = await _firstClientSide.SendTestAsync(_firstClient, "/Name Luiz");
            Assert.Equal("You were registered Succesfully!", response);
        }

        [Fact, Priority(2)]
        public async Task Send_SameName_Client()
        {
            var clientSide = new RunningClient(new ConnectionDomainService(), new MessageHandleDomainService());
            Client client = await clientSide.ConnectAsync();
            var response = await clientSide.SendTestAsync(client, "/Name Luiz");            
            Assert.Equal("Sorry, the nickname Luiz is already taken, Please choose a differente one", response);
        }

        [Fact, Priority(3)]
        public async Task Create_Room_And_List()
        {
            await _firstClientSide.SendTestAsync(_firstClient, "/CTR Room2");
            var response = await _firstClientSide.SendTestAsync(_firstClient, "/LR");

            Assert.Contains("Room2", response);
        }

        [Fact, Priority(4)]
        public async Task Create_New_Client_And_Change_To_Room2()
        {
            _secondClientSide = new RunningClient(new ConnectionDomainService(), new MessageHandleDomainService());
            _secondClient = await _secondClientSide.ConnectAsync();
            await _secondClientSide.SendTestAsync(_secondClient, "/Name Gabriel");
            await _secondClientSide.SendTestAsync(_secondClient, "/CR Room2");
            var response = await _secondClientSide.SendTestAsync(_secondClient, "/LU");
            Assert.Contains("Luiz", response);
        }

        [Fact, Priority(5)]
        public async Task Luiz_Send_Message_ToRoom()
        {
            var response = await _firstClientSide.SendTestAsync(_firstClient, "Ola Galera!");
            Assert.Equal("Luiz says: Ola Galera!", response);
        }

        [Fact, Priority(6)]
        public async Task Gabriel_Send_Public_Message_To_Luiz()
        {            
            var response = await _secondClientSide.SendTestAsync(_secondClient, "/P Luiz e ae Luiz");
            Assert.Equal("Gabriel says to Luiz: e ae Luiz", response);
        }

        [Fact, Priority(7)]
        public async Task Send_Message_To_Inexistent_User()
        {
            var response = await _secondClientSide.SendTestAsync(_secondClient, "/P Maria e ae Luiz");
            Assert.Equal("The client you wanted to talk doesnt exists or isn't in the same room as you.", response);
        }

        [Fact, Priority(8)]
        public async Task Create_New_Client_And_Try_Send_Message_From_Other_Room()
        {
            _thirdClientSide = new RunningClient(new ConnectionDomainService(), new MessageHandleDomainService());
            _thirdCLient = await _thirdClientSide.ConnectAsync();
            await _thirdClientSide.SendTestAsync(_thirdCLient, "/Name Carol");
            var response = await _secondClientSide.SendTestAsync(_secondClient, "/P Carol oi Carol");
            Assert.Equal("The client you wanted to talk doesnt exists or isn't in the same room as you.", response);
        }

    }
}
