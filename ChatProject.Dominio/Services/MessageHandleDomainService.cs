using ChatProject.Domain.Configuration;
using ChatProject.Domain.Entities;
using ChatProject.Domain.Enum;
using ChatProject.Domain.Exceptions;
using System.Linq;

namespace ChatProject.Domain.Services
{

    public class MessageHandleDomainService
    {

        public Message CreatingMessageToBeSend(string message)
        {
            Message messageSending = new Message();

            if (message.Count(p => p == '/') > 1)
            {
                throw new InvalidMessageException("The message contains more than 1 '/'");
            }

            if (message.StartsWith('/'))
            {
                //0 is the command with /
                //1 is what command does
                var commandSplit = message.Split(' ');
                string command = commandSplit[0].Remove(0, 1);
                string commandText = commandSplit.Count() > 1 ? commandSplit[1] : "";
                string messageToBeSend = null;

                // all the rest is message to be send.
                if (commandSplit.Count() > 2)
                {
                    for (int i = 2; i < commandSplit.Count(); i++)
                    {
                        messageToBeSend += commandSplit[i] + " ";
                    }
                    messageToBeSend = messageToBeSend.TrimEnd();
                }


                if (ReservedWords.IsReservedWord(command.ToUpper()))
                {
                    switch (command.ToUpper())
                    {
                        case ReservedWords.ChangeRoom:
                            if (string.IsNullOrWhiteSpace(commandText))
                            {
                                throw new InvalidCommandException("Please provide a room name");
                            }
                            return new Message() { ChangeRoomName = commandText };
                        case ReservedWords.CreateRoom:
                            if (string.IsNullOrWhiteSpace(commandText))
                            {
                                throw new InvalidCommandException("Please provide a room name");
                            }
                            return new Message() { CreateRoomName = commandText };
                        case ReservedWords.Nickname:
                            if (string.IsNullOrWhiteSpace(commandText))
                            {
                                throw new InvalidCommandException("Please provide a nickname");
                            }
                            return new Message() { ClientNickname = commandText };
                        case ReservedWords.ListUsers:
                            return new Message() { ListUsers = true };
                        case ReservedWords.ListRoom:
                            return new Message() { ListRooms = true };
                        case ReservedWords.Userpublic:
                            if (string.IsNullOrWhiteSpace(commandText))
                            {
                                throw new InvalidCommandException("Please provide a user nickname");
                            }
                            if (string.IsNullOrWhiteSpace(messageToBeSend))
                            {
                                throw new InvalidCommandException("Please provide a message to be send");
                            }
                            return new Message() { ClientNameToSendMessage = commandText, MessageToBeSend = messageToBeSend, MessageType = MessageType.Public };
                        case ReservedWords.Userprivate:
                            if (string.IsNullOrWhiteSpace(commandText))
                            {
                                throw new InvalidCommandException("Please provide a user nickname");
                            }
                            if (string.IsNullOrWhiteSpace(messageToBeSend))
                            {
                                throw new InvalidCommandException("Please provide a message to be send");
                            }
                            return new Message() { ClientNameToSendMessage = commandText, MessageToBeSend = messageToBeSend, MessageType = MessageType.Private };
                        case ReservedWords.Help:
                            return new Message() { AskedHelp = true };
                        case ReservedWords.Exit:
                            return new Message() { Exit = true };
                        default:
                            throw new InvalidCommandException("Invalid command, please use command /help to show you all commands that you can use");
                    }
                }
                else
                {
                    throw new InvalidCommandException("Invalid command, please use command /help to show you all commands that you can use");
                }
            }

            //Sending to everyone in that room, since it's not a command.
            messageSending.MessageToBeSend = message;
            return messageSending;
        }
    }
}
