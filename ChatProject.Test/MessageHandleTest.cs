using ChatProject.Domain.Enum;
using ChatProject.Domain.Exceptions;
using ChatProject.Domain.Services;
using Xunit;

namespace ChatProject.Test
{
    public class MessageHandleTest
    {
        
        private MessageHandleDomainService _messageHandleDomainService = new MessageHandleDomainService();

        [Fact]
        public void Invalid_Command_With_More_Than_One_Bar()
        {
            Assert.Throws<InvalidMessageException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("//CR");
            });
        }

        [Fact]
        public void Change_Room_Without_Name()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/CR");
            });
        }

        [Fact]
        public void Create_Room_Without_Name()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/CTR");
            });
        }

        [Fact]
        public void Set_Nickname_Empty()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/NAME");
            });
        }

        [Fact]
        public void Send_Message_User_Public_Empty()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/P");
            });
        }

        [Fact]
        public void Send_Message_User_Public_Empty_Message()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/P Luiz");
            });
        }

        [Fact]
        public void Send_Message_User_Private_Empty()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/PR");
            });
        }

        [Fact]
        public void Send_Message_User_Private_Empty_Message()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/PR Luiz");
            });
        }

        [Fact]
        public void Command_Not_Exists()
        {
            Assert.Throws<InvalidCommandException>(() =>
            {
                _messageHandleDomainService.CreatingMessageToBeSend("/PR2");
            });
        }

        [Fact]
        public void Change_Room()
        {
            string command = "/CR Default";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.Equal("Default", message.ChangeRoomName);
        }

        [Fact]
        public void Create_Room()
        {
            string command = "/CTR Default";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.Equal("Default", message.CreateRoomName);
        }

        [Fact]
        public void Set_NickName()
        {
            string command = "/Name luiz";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.Equal("luiz", message.ClientNickname);
        }

        [Fact]
        public void List_Users()
        {
            string command = "/LU";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.True(message.ListUsers);
        }

        [Fact]
        public void List_Rooms()
        {
            string command = "/LR";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.True(message.ListRooms);
        }

        [Fact]
        public void Send_Message_Public_To_An_User()
        {
            string command = "/P Luiz Bora entrar na take :-)";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.Equal("Luiz", message.ClientNameToSendMessage);
            Assert.Equal("Bora entrar na take :-)", message.MessageToBeSend);
            Assert.Equal(MessageType.Public, message.MessageType);
        }

        [Fact]
        public void Send_Message_Private_To_An_User()
        {
            string command = "/PR Luiz Bora entrar na take :-)";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.Equal("Luiz", message.ClientNameToSendMessage);
            Assert.Equal("Bora entrar na take :-)", message.MessageToBeSend);
            Assert.Equal(MessageType.Private, message.MessageType);
        }


        [Fact]
        public void Asking_Help()
        {
            string command = "/Help";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.True(message.AskedHelp);
        }

        [Fact]
        public void Leaving_Chat()
        {
            string command = "/Exit";
            var message = _messageHandleDomainService.CreatingMessageToBeSend(command);

            Assert.True(message.Exit);
        }

    }
}
