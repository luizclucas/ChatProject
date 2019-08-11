using ChatProject.Domain.Enum;

namespace ChatProject.Domain.Entities
{
    public class Message
    {
        public bool ListRooms { get; set; } = false;
        public bool ListUsers { get; set; } = false;
        public string ClientNameToSendMessage { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Public;
        public string MessageToBeSend { get; set; }
        public string ClientNickname { get; set; }
        public string CreateRoomName { get; set; }
        public string ChangeRoomName { get; set; }
        public bool Exit { get; set; } = false;
        public bool AskedHelp { get; set; } = false;
        public string Help
        {
            get
            {
                return "\n/LU => List the users that are in your room. \n" +
 "/NAME => To set your nickname to let you be able to talk with others.\n" +
 "/PR => Let you send private message to a person.\n" +
 "/P => Let you send public message to a person.\n" +
 "/CTR => Creating a room\n" +
 "/CR => Changing room\n" +
 "/LR => List rooms.\n" +
 "/HELP => Show all commands\n" +
 "/EXIT => Leave chat.\n";
            }
        }


    }
}
