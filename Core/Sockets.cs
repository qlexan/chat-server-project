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


