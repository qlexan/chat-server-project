using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SocketLib
{
    public interface ISocketHandler
    {
        Socket Socket { get; set; }

        void SendMessage(string message, Socket socket = null)
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
        public void Connect(IPEndPoint endPoint)
        {
            Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(endPoint);
        }

    }
    public class Server : ISocketHandler
    {
        public Socket Socket { get; set; }

        public void BindAndListen(IPEndPoint endpoint)
        {
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(10);
            Console.WriteLine("[SERVER] Successfully binded");
        }

        public Socket Accept()
        {
            return Socket.Accept();
        }


    }
}
