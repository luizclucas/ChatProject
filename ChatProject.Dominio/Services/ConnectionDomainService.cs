using System;
using System.Net;
using System.Net.Sockets;
using ChatProject.Domain.Exceptions;
using Serilog;

namespace ChatProject.Domain.Services
{
    public class ConnectionDomainService
    {
        #region [ PUBLIC PROPERTIES ]
        public EndPoint EndPoint { get { return new IPEndPoint(LocalIPAddress, Port); } }
        public IPAddress LocalIPAddress { get { return IPAddress.Loopback; } }
        public int Port { get { return 18521; } }
        #endregion

        #region [ PRIVATE PROPERTIES ]
        private ILogger _log = Log.ForContext<ConnectionDomainService>();
        #endregion

        #region [ PUBLIC METHODS ]
        public Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Socket CreateListener()
        {
            Socket socket = null;
            try
            {
                socket = CreateSocket();
                socket.Bind(EndPoint);
                socket.Listen(15);
            }
            catch (Exception e)
            {
                //Sometimes there's problem that type of exception isn't logged, so i'm logging it in a context to get even in elasticsearch or seq.
                _log
                    .ForContext("ExceptionType", e.GetType().FullName)
                    .Error(e, "Exception on creating listener");
                throw new CreateListenerException();
            }

            return socket;
        }   
        #endregion



    }
}
