using ChatProject.Domain.Configuration;
using ChatProject.Domain.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;

namespace ChatProject.Domain.Entities
{
    public class Client
    {
        #region [ PUBLIC PROPERTIES ]
        public Socket Socket { get; protected set; }
        public string NickName { get; protected set; }
        public byte[] Buffer { get; protected set; }
        public string RoomName { get; protected set; } = "Default";

        public MessageVO Message { get; set; } = new MessageVO();
        #endregion

        #region [ CONSTRUCTOR ]
        public Client(Socket socket)
        {
            Socket = socket;
            Buffer = new byte[MessageParameters.BufferSize];
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void SetNickName(string nickName)
        {
            NickName = nickName;
        }
        public void SetRoomName(string roomName)
        {
            RoomName = roomName;
        }
        public void Close()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch (Exception e)
            {
                throw new CloseSocketException("Connection was already closed", e);
            }
        }

        public void BuildIncomingMessage(int bytesRead)
        {
            Message.IncomingMessage = Encoding.ASCII.GetString(Buffer, 0, bytesRead);
        }
        public bool MessageReceived()
        {
            return Message.IncomingMessage.IndexOf(MessageParameters.MessageEnd) > -1;
        }

        public void CheckIncomingMessage()
        {
            if (string.IsNullOrWhiteSpace(NickName) && Message.IncomingMessage.Contains(@"\Name "))
            {
                string nicknameAtIncommingMessage = (Message.IncomingMessage.Split(' '))[1];
                NickName = nicknameAtIncommingMessage;
            }
        }
        #endregion
    }
}
