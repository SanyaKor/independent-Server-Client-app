using System;
using System.Net;
using System.Net.Sockets;

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
			tcp.Connect();
        }

		public class TCP
        {
			public TcpClient socket;

			private NetworkStream stream;
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

					//TODO handle data

					stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error receiving TCP data: {ex}");
				}
			}
        }
	}
}

