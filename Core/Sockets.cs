using System.Net.Sockets;
using System.Net;
using System.Text;
using SocketLib;

namespace SocketLib
{
    public interface ISocketHandler
    {
        Socket Socket { get; set; }

        void SendMessage(string message, ISocketHandler? handler = null);
        string ReceiveMessage(int buffersize = 1024, Socket? socket = null);
        void Connect(IPEndPoint ipendpoint, Socket? socket = null);
        void Close();
    }
    public class SocketHelper
    {
        public void SendMessage(ISocketHandler handler, string message)
        {
            if (handler == null)
            {
                System.Console.WriteLine("Target is null");
            }
            else
            {
                byte[] messageSent = Encoding.ASCII.GetBytes(message);
                handler.Socket.Send(messageSent);
            }

        }

        public string ReceiveMessage(ISocketHandler handler, int buffersize = 1024)
        {

            byte[] buffer = new byte[buffersize];
            int byteRecv = handler.Socket.Receive(buffer);
            return Encoding.ASCII.GetString(buffer, 0, byteRecv);
        }
        public void Close(ISocketHandler handler)
        {
            handler.Socket.Shutdown(SocketShutdown.Both);
            handler.Socket.Close();
        }
    }
    public class Client : ISocketHandler
    {
        public Socket Socket { get; set; }
        private readonly SocketHelper _helper;
        public Client(SocketHelper helper)
        {
            _helper = helper;
        }
        public void Connect(IPEndPoint endPoint, Socket? socket = null)
        {
            var sock = socket ?? Socket;
            sock.Connect(endPoint);
        }

        public void SendMessage(string message, ISocketHandler? handler = null)
        {
            _helper.SendMessage(handler ?? this, message);
        }
        public string ReceiveMessage(int buffersize = 1024, Socket? socket = null) => _helper.ReceiveMessage(this, buffersize);
        public void Close() => _helper.Close(this);

    }
    public class Server : ISocketHandler
    {
        private List<Client> _connectedClients = [];
        public Socket Socket { get; set; }
        private readonly SocketHelper _helper;

        public Server(SocketHelper helper)
        {
            _helper = helper;
        }

        public void Connect(IPEndPoint endpoint, Socket? socket = null)
        {
            var sock = socket ?? Socket;
            sock.Bind(endpoint);
            sock.Listen(10);
            Console.WriteLine("[SERVER] Successfully binded");
        }
        public List<Client> GetConnectedClients()
        {
            return _connectedClients;
        }
        public Client Accept()
        {
            Socket acceptedSocket = Socket.Accept();
            Client client = new Client(_helper) { Socket = acceptedSocket };
            _connectedClients.Add(client); // store it internally
            return client;
        }

        public void SendMessage(string message, ISocketHandler? handler = null) => _helper.SendMessage(handler, message);
        public string ReceiveMessage(int buffersize = 1024, Socket? socket = null) => _helper.ReceiveMessage(this, buffersize);
        public void Close() => _helper.Close(this);



    }
}
namespace FacadeLib
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
        public ISocketHandler ServerAccept() => _server.Accept();
        public void ServerSend(ISocketHandler handler, string message) => _server.SendMessage(message, handler);
        public string ServerReceive() => _server.ReceiveMessage();
        public void ServerClose() => _server.Close();
    }
}
