using SocketLib;
using System.Net;
namespace ManagerLib
{
    class SocketManager
    {
        protected Client _client;
        protected Server _server;

        public SocketManager(Client client, Server server)
        {
            this._client = client;
            this._server = server;
        }
        // Client functions
        public void ClientConnect(IPEndPoint endpoint) => _client.Connect(endpoint);
        public void ClientSend(string message) => _client.SendMessage(message);
        public string ClientReceive() => _client.ReceiveMessage();
        public void ClientClose() => _client.Close();

        // Server functions

        public void ServerConnect(IPEndPoint endpoint) => _server.Connect(endpoint);

        ///<summary><c>ServerAccept</c> accepts a client socket and adds it to the internal list.</summary>
        public void ServerAccept() => _server.Accept();

        public void ServerSend(Sockets handler, string message) {
          foreach (var client in _server.GetConnectedClients()) {
            _server.SendMessage(message, client);
          }
        }
        public string ServerReceive() => _server.ReceiveMessage();
        public void ServerClose() => _server.Close();
    }
}
