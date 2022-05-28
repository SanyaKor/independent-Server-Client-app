using System;
namespace Client
{
	public class ClientHandle
	{
		public static void Welcome(Packet _packet)
		{
			string _msg = _packet.ReadString();
			int _myId = _packet.ReadInt();

			Console.WriteLine($"Message from server: {_msg}");
			Client.Instance.myId = _myId;
			ClientSend.WelcomeReceived();

			// TODO : SEND WELCOME 
		}

	}
}

