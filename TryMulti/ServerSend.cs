using System;

namespace TryMulti
{
	class ServerSend
	{
		private static void SendTCPData(int _toClient, Packet _packet)
        {
			_packet.WriteLength();
			Server.clients[_toClient].tcp.SendData(_packet);
        }

		private static void SendTCPDataToALL( Packet _packet)
		{
			_packet.WriteLength();
			for(int i = 1; i <= Server.MaxPlayers; i++)
            {
				Server.clients[i].tcp.SendData(_packet);
			}
			
		}

		public static void Welcome(int _toClient, string _msg)
        {
			using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
				_packet.Write(_msg);
				_packet.Write(_toClient);

				SendTCPData(_toClient, _packet);
            };
        }

		//public static void Sender(string _msg)
  //      {
		//	using (Packet _packet = new Packet((int)ServerPackets.welcome))
		//	{
		//		_packet.Write(_msg);


		//		SendTCPDataToALL( _packet);
		//	};
		//}
	}
}

