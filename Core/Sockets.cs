using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server.Sockets
{
    public interface ISocketHandler
    {
        Socket Socket { get; set; }

        void SendMessage(string message)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(message);
            Socket.Send(messageSent);
        }

        string ReceiveMessage(int buffersize = 1024)
        {
            byte[] buffer = new byte[buffersize];
            int byteRecv = Socket.Receive(buffer);
            return Encoding.ASCII.GetString(buffer, 0, byteRecv);
        }

        void Close()
        {
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
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
}
