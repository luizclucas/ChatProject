using ChatProject.Domain.Configuration;
using ChatProject.Domain.Entities;
using ChatProject.Helper;
using System.Text;

namespace ChatProject.Domain
{
    public class MessageVO
    {
        #region [ PROPERTIES ]
        public string IncomingMessage { get; set; }
        public string MessageToBeSend { get; set; }
        #endregion

        #region [ CONSTRUCTOR ]
        public MessageVO()
        {
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void CreateMessageToBeSend(Message msg)
        {
            MessageToBeSend = msg.ToJson() + MessageParameters.MessageEnd;
        }

        public void CreateMessageToBeSendByServer(string msg)
        {
            MessageToBeSend = msg + MessageParameters.MessageEnd;
        }

        public byte[] ConvertMessageToBeSendToBytes()
        {
            if (!MessageToBeSend.EndsWith(MessageParameters.MessageEnd))
            {
                MessageToBeSend += MessageParameters.MessageEnd;
            }
            return Encoding.ASCII.GetBytes(MessageToBeSend);
        }

        public void ClearIncomingMessage()
        {
            IncomingMessage = null;
        }

        public void RemoveEndIncomingMessage()
        {
           IncomingMessage = IncomingMessage.Replace(MessageParameters.MessageEnd, "");
        }

        #endregion
    }
}
