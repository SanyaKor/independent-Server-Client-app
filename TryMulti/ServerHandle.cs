using System;
namespace TryMulti
{
	class ServerHandle
	{
		public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
			int _clientIdCheck = _packet.ReadInt();
			string _username = _packet.ReadString();

			Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
			if(_fromClient != _clientIdCheck)
            {
				Console.WriteLine("mm hueta");
            }
        }
	}
}

