using System;
using System.Net;
using System.Net.Sockets;

namespace TryMulti
{
	class Server
	{
		public static int MaxPlayers { get; private set; }

		public static int Port { get; private set; }

		public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

		private static TcpListener tcpListener;

		public static void Start(int _maxPlayers, int _port){
			MaxPlayers = _maxPlayers;
			Port = _port;

			Console.WriteLine("Starting server...");
			tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			InitializeServerData();

			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

			Console.WriteLine($"Server started on {Port}");

		}

		private static void TCPConnectCallback(IAsyncResult asyncResult)
        {
			TcpClient _client = tcpListener.EndAcceptTcpClient(asyncResult);
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
			Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}");


			// ищем среди словаря соединений свободный сокет
			for (int i = 1; i <= MaxPlayers; i++)
			{
				if(clients[i] .tcp.socket == null)
                {
					clients[i].tcp.Connect(_client);
					Console.WriteLine($"Player {_client.Client.RemoteEndPoint} connected successfully");
					return;
                }
			}

			Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect : Server is full");
		}

		private static void InitializeServerData()
        {
			for(int i = 1; i <= MaxPlayers; i++)
            {
				clients.Add(i, new Client(i));
            }
        }
		
	}
}

