using System.Collections.Generic;

namespace ChatProject.Domain.Entities
{
    public class Room
    {
        #region [ PUBLIC PROPERTIES ]
        public string RoomName { get; protected set; }
        public IList<Client> Clients { get; protected set; }
        #endregion

        #region [ CONSTRUCTOR ]
        public Room(string roomName, IList<Client> clients)
        {
            RoomName = roomName;
            Clients = clients;
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void AddClient(Client client)
        {
            Clients.Add(client);
        }
        #endregion

    }
}
