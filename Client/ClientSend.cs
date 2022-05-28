using System;
namespace Client
{
	public class ClientSend
	{
		private static void SendTCPData(Packet _packet)
        {
			_packet.WriteLength();
			Client.Instance.tcp.SendData(_packet);
        }

		public static void WelcomeReceived()
        {
			using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
				_packet.Write(Client.Instance.myId);
				_packet.Write("random name");

				SendTCPData(_packet);
            }
        }
	}
}

