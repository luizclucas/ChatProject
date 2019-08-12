using ChatProject.Domain;
using ChatProject.Domain.Configuration;
using ChatProject.Domain.Entities;
using ChatProject.Domain.Services;
using ChatProject.Helper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ChatProject.Domain.Exceptions;
using ChatProject.Domain.Enum;

namespace ChatProject.Server
{
    public class RunningServer
    {
        #region [ PRIVATE PROPERTIES ]
        private ConnectionDomainService _connectionService;
        private Socket _server = null;
        private ManualResetEvent _connected = new ManualResetEvent(false);
        private List<Room> _rooms;
        private ILogger _log = Log.ForContext<RunningServer>();
        #endregion


        public RunningServer(ConnectionDomainService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task StartListening()
        {
            try
            {
                _log.Information("Server is starting");
                _server = _connectionService.CreateListener();
                _log.Information("Server started");

                while (true)
                {
                    _connected.Reset();
                    // Start an asynchronous socket to listen for connections
                    _server.BeginAccept(new AsyncCallback(AcceptCallback), _server);

                    // Wait until a connection is made before continuing
                    _connected.WaitOne();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error on starting listener");
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue accepting new connections
            _connected.Set();

            // Accept new client socket connection
            Socket socket = _server.EndAccept(ar);

            Client client = new Client(socket);

            // Store all clients
            if (_rooms == null)
            {
                _rooms = new List<Room>()
                {
                    new Room("Default", new List<Client>())
                };
            }

            _rooms.FirstOrDefault(p => p.RoomName == "Default").AddClient(client);

            // Begin receiving messages from new connection
            try
            {
                MessageVO msg = new MessageVO();
                client.Message = msg;
                client.Socket.BeginReceive(client.Buffer, 0, MessageParameters.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
                SendToClient(client, "Welcome to our chat server. Please provide a nickname by Using /Name | /Help to list all commands.");
            }
            catch (SocketException)
            {
                // Client was forcebly closed on the client side
                CloseClient(client);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error on beginning to receive messages");
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead;

            // Check for null values
            if (!TryGetClient(ar, out string error, out Client client))
            {
                _log.Information(error);
                return;
            }

            // Reading message from the client socket
            try
            {
                bytesRead = client.Socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                _log.Information("Client closed on the client side");
                CloseClient(client);
                return;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error on reading client socket");
                return;
            }

            if (bytesRead > 0)
            {
                client.BuildIncomingMessage(bytesRead);

                if (client.MessageReceived())
                {
                    client.Message.RemoveEndIncomingMessage();
                    var message = client.Message.IncomingMessage.FromJsonTo<Message>();

                    if (string.IsNullOrWhiteSpace(client.NickName) && string.IsNullOrWhiteSpace(message.ClientNickname))
                    {
                        SendToClient(client, "Please provide a nickname by Using /Name Command");
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(message.ClientNickname))
                    {
                        try
                        {
                            SetNickName(client, message.ClientNickname);
                            SendToClient(client, "You were registered Succesfully!");
                        }
                        catch (DuplicateNickNameException)
                        {
                            SendToClient(client, $"Sorry, the nickname {message.ClientNickname} is already taken, Please choose a differente one");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(message.CreateRoomName))
                    {
                        RemoveClientFromRooms(client);
                        _rooms.Add(new Room(message.CreateRoomName, new List<Client>() { client }));
                        client.SetRoomName(message.CreateRoomName);
                    }
                    else if (!string.IsNullOrWhiteSpace(message.ChangeRoomName))
                    {
                        try
                        {
                            CheckIfRoomExistsAndChange(client, message.ChangeRoomName);
                        }
                        catch (RoomDoesntExistsException)
                        {
                            SendToClient(client, "The room you tried to change doesn't exists");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(message.ClientNameToSendMessage))
                    {
                        try
                        {
                            SendMessageToAnUser(client, message);
                        }
                        catch (ClientDoesntExistsException)
                        {
                            SendToClient(client, "The client you wanted to talk doesnt exists or isn't in the same room as you.");
                        }
                    }
                    else if(message.ListRooms)
                    {
                        string roomsName = "";
                        foreach (var room in _rooms)
                        {
                            roomsName += room.RoomName + "\n";
                        }
                        SendToClient(client, "Open rooms: \n" + roomsName);
                    }
                    else if(message.ListUsers)
                    {
                        string usersOfYourRoom = "";
                        foreach (var clientInRoom in _rooms.FirstOrDefault(p => p.RoomName == client.RoomName).Clients)
                        {
                            if(!string.IsNullOrWhiteSpace(clientInRoom.NickName))
                            {
                                usersOfYourRoom += clientInRoom.NickName + "\n";
                            }
                        }
                        SendToClient(client, "Users in you room: \n" + usersOfYourRoom);
                    }
                    else
                    {
                        Room room = _rooms.FirstOrDefault(p => p.RoomName == client.RoomName);
                        SendToAllClientRoom(room, client.NickName + " says", message.MessageToBeSend);
                    }

                    client.Message.ClearIncomingMessage();

                }
            }

            // Listen for more incoming messages
            try
            {
                MessageVO msg = new MessageVO();
                client.Message = msg;
                client.Socket.BeginReceive(client.Buffer, 0, MessageParameters.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            }
            catch (SocketException)
            {
                // Client was forcebly closed on the client side
                CloseClient(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool TryGetClient(IAsyncResult ar, out string error, out Client client)
        {
            client = null;
            error = "";

            if (ar == null)
            {
                error = "Async result null";
                return false;
            }

            // Check client
            client = (Client)ar.AsyncState;
            if (client == null)
            {
                error = "Client null";
                return false;
            }

            return true;
        }

        private void CloseClient(Client client)
        {
            client.Close();
            RemoveClientFromRooms(client);
        }

        private void SetNickName(Client client, string nickName)
        {
            if (_rooms.Any(p => p.Clients.Any(c => c.NickName == nickName)))
            {
                throw new DuplicateNickNameException();
            }

            client.SetNickName(nickName);
        }

        private void RemoveClientFromRooms(Client client)
        {
            foreach (var room in _rooms)
            {
                if (room.Clients.Contains(client))
                {
                    room.Clients.Remove(client);
                }
            }

            _log.Information("Client: {0} has been removed from rooms", client.NickName);
        }
        private void CheckIfRoomExistsAndChange(Client client, string roomName)
        {
            var roomToChange = _rooms.FirstOrDefault(p => p.RoomName == roomName);

            if (roomToChange == null)
            {
                _log.Information("Room doesn't exists");
                throw new RoomDoesntExistsException();
            }

            RemoveClientFromRooms(client);
            client.SetRoomName(roomName);
            roomToChange.AddClient(client);
            _log.Information("Client: {0} changed to room: {1}", client.NickName, roomToChange.RoomName);
        }

        private void SendMessageToAnUser(Client client, Message message)
        {
            if (ClientExists(message.ClientNameToSendMessage, client.RoomName))
            {
                if (message.MessageType == MessageType.Public)
                {
                    string prefix = client.NickName + " says to " + message.ClientNameToSendMessage;

                    Room room = _rooms.FirstOrDefault(p => p.RoomName == client.RoomName);
                    SendToAllClientRoom(room, prefix, message.MessageToBeSend);
                }
                else if(message.MessageType == MessageType.Private)
                {
                    Client userToSendMessage = _rooms.FirstOrDefault(p => p.RoomName == client.RoomName).Clients.FirstOrDefault(p => p.NickName == message.ClientNameToSendMessage);
                    SendToClient(userToSendMessage, client.NickName + " says privately to " + message.ClientNameToSendMessage + ": " + message.MessageToBeSend);
                }
            }
            else
            {
                throw new ClientDoesntExistsException();
            }
        }

        private bool ClientExists(string nickName, string roomName)
        {
            return _rooms.Any(p => p.Clients.Any(c => c.NickName == nickName && c.RoomName == roomName));
        }

        private void SendToAllClientRoom(Room room, string prefix, string message)
        {
            foreach (var client in room.Clients)
            {
                SendToClient(client, prefix + ": " + message);
            }
        }

        private void SendToClient(Client client, string message)
        {
            if (client == null)
            {
                _log.Information("Not possible, client is null");
                return;
            }

            _log.Information("Sending reply to client: {0}", client.NickName);

            MessageVO msg = new MessageVO();
            msg.CreateMessageToBeSendByServer(message);

            // Create reply
            var byteReply = msg.ConvertMessageToBeSendToBytes();

            // Listen for more incoming messages
            try
            {
                client.Socket.BeginSend(byteReply, 0, byteReply.Length, SocketFlags.None, new AsyncCallback(SendReplyCallback), client);
            }
            catch (SocketException)
            {
                // Client was forcebly closed on the client side
                CloseClient(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendReplyCallback(IAsyncResult ar)
        {
        }
    }
}
