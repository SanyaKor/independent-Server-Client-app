using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
	class Client
	{

		private static readonly Lazy<Client> instance =
			new Lazy<Client>(() => new Client());

		public static Client Instance { get { return instance.Value; } }


		public static int dataBufferSize = 4096;
		public string ip = "127.0.0.1";
		public int port = 5555;
		public int myId;
		public TCP tcp;

		private delegate void PacketHandler(Packet _packet);
		private static Dictionary<int, PacketHandler> packetHandlers;

		private Client()
        {
			Start();
		}

		private void Start()
        {
			tcp = new TCP();
        }

		public void ConnectToServer()
        {
			InitializeClientData();
			
			tcp.Connect();
        }

		public class TCP
        {
			public TcpClient socket;

			private NetworkStream stream;
			private Packet receivedData;
			private byte[] receiveBuffer;

			public void Connect()
            {
				socket = new TcpClient
				{
					ReceiveBufferSize = dataBufferSize,
					SendBufferSize = dataBufferSize
				};


				receiveBuffer = new byte[dataBufferSize];

                socket.BeginConnect(Instance.ip, Instance.port ,ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult ar)
            {
				socket.EndConnect(ar);

                if (!socket.Connected)
                {
					return;
                }

				stream = socket.GetStream();

				receivedData = new Packet();

				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback,null); 
			}

            private void ReceiveCallback(IAsyncResult ar)
            {
				try
				{
					int _byteLength = stream.EndRead(ar);
					if (_byteLength <= 0)
					{
						//TODO disconnect
						return;
					}

					byte[] _data = new byte[_byteLength];
					Array.Copy(receiveBuffer, _data, _byteLength);

					receivedData.Reset(HandleData(_data));
					//Console.WriteLine(Encoding.ASCII.GetString(_data, 0, _data.Length));
					stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error receiving TCP data: {ex}");
				}
			}

			private bool HandleData(byte[] _data)
            {
				int _packetLength = 0;

				receivedData.SetBytes(_data);

				if(receivedData.UnreadLength() >= 4)
                {
					_packetLength = receivedData.ReadInt();
					if(_packetLength <= 0)
                    {
						return true;
                    }
                }

				while(_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
					byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
					ThreadManager.ExecuteOnMainThread(() =>
                    {
						using (Packet _packet = new Packet(_packetBytes))
                        {
							int _packetId = _packet.ReadInt();
							packetHandlers[_packetId](_packet);
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

		private void InitializeClientData()
        {
			packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)ServerPackets.welcome, ClientHandle.Welcome }
			};

			Console.WriteLine("Initialized packets.");
        }
	}
}

