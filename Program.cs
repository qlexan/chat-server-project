using SocketLib;
using System.Net.Sockets;
using System.Net;

var endPoint = new IPEndPoint(IPAddress.Loopback, 12345);

Server server = new();
ISocketHandler serverHandler = server;

server.BindAndListen(endPoint);

System.Console.WriteLine("[SERVER] Waiting for connection...");
Socket clientAccepted = server.Accept();
System.Console.WriteLine("[SERVER] Client connected.");

Client client = new();
ISocketHandler clientHandler = client;
client.Connect(endPoint);
System.Console.WriteLine("[CLIENT] Connected to server.");


clientHandler.SendMessage("Hello from client");
string serverReceived = serverHandler.ReceiveMessage(socket:clientAccepted);
System.Console.WriteLine("[SERVER] Received: "+ serverReceived);

serverHandler.SendMessage("Hello Client", clientAccepted);
string clientReceived = clientHandler.ReceiveMessage();
System.Console.WriteLine("[CLIENT] Received: " + clientReceived);
