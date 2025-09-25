using System.Net.Sockets;
using System.Net;
using System.Text;


namespace SocketLib
{
    public interface INetworkHandler 
    {
        Task SendMessage(string message, INetworkHandler? handler = null);
        Task<string> ReceiveMessage(int buffersize = 1024, Socket? socket = null);
        void Close();
    }

    public interface IClientHandler : INetworkHandler 
    {
      TcpClient Connection {get;} 

      Task Connect(IPEndPoint ipendpoint, Socket? socket = null);
    }
    public interface IServerHandler : INetworkHandler {
      Task<TcpClient> AcceptClientAsync();

    }
    public class NetworkHelper
    {
        public async Task SendMessage(INetworkHandler handler, string message)
        {
            if (handler == null)
            {
                Console.WriteLine("Target is null");
            }
            else
            {
                byte[] messageSent = Encoding.ASCII.GetBytes(message);
                await handler.Socket.SendAsync(messageSent);
            }

        }

        public async Task<string> ReceiveMessage(INetworkHandler handler, int buffersize = 1024)
        {

            byte[] buffer = new byte[buffersize];
            int byteRecv = await handler.Socket.ReceiveAsync(buffer);
            return Encoding.ASCII.GetString(buffer, 0, byteRecv);
        }
        public void Close(INetworkHandler handler)
        {
            handler.Socket.Shutdown(SocketShutdown.Both);
            handler.Socket.Close();
        }
    }
    public class Client : INetworkHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkHelper _helper;
        public Client(NetworkHelper helper)
        {
            _helper = helper;
            _client = new TcpClient();
        }
        public async Task Connect(IPEndPoint endPoint, Socket? socket = null)
        {
          await _client.ConnectAsync(endPoint);
        }

        public async Task SendMessage(string message, INetworkHandler? handler = null)
        {
            await _helper.SendMessage(handler ?? this, message);
        }
        public Task<string> ReceiveMessage(int buffersize = 1024, Socket? socket = null) => _helper.ReceiveMessage(this, buffersize);
        public void Close() => _helper.Close(this);
        
        public TcpClient Connection  => _client;

    }
    public class Server : INetworkHandler
    {
        private List<Client> _connectedClients = [];
        public Socket Socket { get; set; } = null!;
        private readonly NetworkHelper _helper;

        public Server(NetworkHelper helper)
        {
          _helper = helper;
        }

        public async Task Connect(IPEndPoint endpoint, Socket? socket = null)
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
        public void Accept()
        {
            Socket acceptedSocket = Socket.Accept();
            Client client = new Client(_helper) { Socket = acceptedSocket };
            _connectedClients.Add(client); // store it internally
        }

        public Task SendMessage(string message, INetworkHandler? handler = null) => _helper.SendMessage(handler, message);
        public Task<string> ReceiveMessage(int buffersize = 1024, Socket? socket = null) => _helper.ReceiveMessage(this, buffersize);
        public void Close() => _helper.Close(this);



    }
}

