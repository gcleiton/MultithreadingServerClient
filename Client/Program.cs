using System.Net.Sockets;
using Client;

var client = new TcpClient();
ClientHandler
    .With(client)
    .Initialize();