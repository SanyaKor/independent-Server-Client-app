using System;
using System.Net;
using System.Net.Sockets;

namespace TryMulti
{
	public class Client
	{
		public static int dataBufferSize = 4096;
		public int id;
		public TCP tcp;

		public Client(int _clientId)
        {
			id = _clientId;
			tcp = new TCP(id);
        }

		public class TCP
		{
			public TcpClient socket;
			private NetworkStream stream;
			private byte[] receiveBuffer;
			private Packet receivedData;
			
			private readonly int id;

			public TCP(int _id)
            {
				id = _id;
            }

			public void Connect(TcpClient _socket)
            {
				socket = _socket;
				socket.ReceiveBufferSize = dataBufferSize;
				socket.SendBufferSize = dataBufferSize;

				stream = socket.GetStream();

				receivedData = new Packet();

				receiveBuffer = new byte[dataBufferSize];

				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

				ServerSend.Welcome(id, "Welcome to the server!");


				//ServerSend.Welcome(id, "Welcome again!");

			}

			public void SendData(Packet _packet)
            {
                try
                {
					if(socket != null)
                    {
						//Console.WriteLine("Sending package...");
						stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch(Exception ex)
                {
					Console.WriteLine($"Error sending data to player {id} via TCP: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
					int _byteLength = stream.EndRead(ar);
					if(_byteLength <= 0)
                    {
						//TODO disconnect
						return;
                    }

					byte[] _data = new byte[_byteLength];
					Array.Copy(receiveBuffer, _data, _byteLength);

					//TODO handle data
					receivedData.Reset(HandleData(_data));
					stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch(Exception ex)
                {
					Console.WriteLine($"Error receiving TCP data: {ex}");
                }
            }

			private bool HandleData(byte[] _data)
			{
				int _packetLength = 0;

				receivedData.SetBytes(_data);

				if (receivedData.UnreadLength() >= 4)
				{
					_packetLength = receivedData.ReadInt();
					if (_packetLength <= 0)
					{
						return true;
					}
				}

				while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
				{
					byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
					ThreadManager.ExecuteOnMainThread(() =>
					{
						using (Packet _packet = new Packet(_packetBytes))
						{
							int _packetId = _packet.ReadInt();
							Server.packetHandlers[_packetId](id, _packet);
						}
					});


					_packetLength = 0;

					if (receivedData.UnreadLength() >= 4)
					{
						_packetLength = receivedData.ReadInt();
						if (_packetLength <= 0)
						{
							return true;
						}
					}
				}

				//Console.WriteLine();
				if (_packetLength <= 1)
				{
					return true;
				}


				return false;
			}
		}
	}
}

