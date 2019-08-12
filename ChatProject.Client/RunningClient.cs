using ChatProject.Domain.Configuration;
using ChatProject.Domain.Entities;
using ChatProject.Domain.Exceptions;
using ChatProject.Domain.Services;
using Serilog;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatProject.ClientSide
{
    public class RunningClient
    {
        #region [ PRIVATE PROPERTIES ]
        public ConnectionDomainService _connectionService;
        private ILogger _log = Log.ForContext<RunningClient>();
        private MessageHandleDomainService _messageHandleDomainService;
        private string MessageReceived { get; set; }
        #endregion

        #region [ CONSTRUCTOR ]
        public RunningClient(ConnectionDomainService connectionService, MessageHandleDomainService messageHandle)
        {
            _connectionService = connectionService;
            _messageHandleDomainService = messageHandle;
        }
        #endregion
        /// <summary>
        /// Attempts to connect to a server
        /// </summary>
        public async Task<Client> ConnectAsync()
        {
            Socket socket = _connectionService.CreateSocket();
            Client client = new Client(socket);

            int attempts = 0;

            // Loop until we connect (server could be down)
            while (!client.Socket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    await client.Socket.ConnectAsync(_connectionService.EndPoint);
                    await Task.Delay(1500);
                }
                catch (SocketException e)
                {
                    Console.Clear();
                }

                if (attempts > 100)
                {
                    throw new ConnectionTImeoutException();
                }
            }

            Thread receiveThread = new Thread(async () => await ReceiveAsync(client));
            receiveThread.Start();

            return client;
        }

        public async Task RunningAsync(Client client)
        {      
            while (true)
            {
                string sentence = Console.ReadLine();

                try
                {
                    var messageToBeSend = _messageHandleDomainService.CreatingMessageToBeSend(sentence);

                    if (messageToBeSend.Exit)
                    {
                        _log.Information("Disconected, Bye!");
                        break;
                    }
                    if (messageToBeSend.AskedHelp)
                    {
                        _log.Information(messageToBeSend.Help);
                        continue;
                    }
                    //Used to test.
                    MessageReceived = null;
                    await SendAsync(client, messageToBeSend);
                }
                catch (InvalidCommandException ex)
                {
                    _log.Information(ex.Message);
                    throw;
                }
                catch (Exception e)
                {
                    _log.Error(e, "Unhandled Error");
                }
            }
        }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <param name="client"></param>
        private async Task SendAsync(Client client, Message message)
        {
            client.Message.CreateMessageToBeSend(message);
            byte[] data = client.Message.ConvertMessageToBeSendToBytes();

            await Task.Delay(200);
            try
            {
                client.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
            }
            catch (SocketException)
            {
                Console.WriteLine("Server Closed");
                client.Close();
                Thread.CurrentThread.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.CurrentThread.Abort();
            }
        }

        public async Task<string> SendTestAsync(Client client, string sentence)
        {
            var messageToBeSend = _messageHandleDomainService.CreatingMessageToBeSend(sentence);
            await SendAsync(client, messageToBeSend);
            await Task.Delay(2000);
            return MessageReceived;
        }

        /// <summary>
        /// Message sent handler
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            _log.Information("sent");
        }

        private async Task ReceiveAsync(Client client)
        {
            int bytesRead = 0;

            while (true)
            {
                // Read message from the server
                try
                {
                    bytesRead = await client.Socket.ReceiveAsync(client.Buffer, SocketFlags.None);
                }
                catch (SocketException)
                {
                    client.Close();
                    Thread.CurrentThread.Abort();
                }
                catch (Exception)
                {
                    Thread.CurrentThread.Abort();
                }

                // Check message
                if (bytesRead > 0)
                {
                    client.BuildIncomingMessage(bytesRead);

                    // Check if i received the full message
                    if (client.MessageReceived())
                    {
                        string messageReceived = client.Message.IncomingMessage.Replace(MessageParameters.MessageEnd, "");
                        _log.Information(messageReceived);
                        client.Message.ClearIncomingMessage();
                        MessageReceived = messageReceived;
                    }
                }
            }
        }
    }
}
