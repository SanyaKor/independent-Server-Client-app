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

				receiveBuffer = new byte[dataBufferSize];

				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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

					stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch(Exception ex)
                {
					Console.WriteLine($"Error receiving TCP data: {ex}");
                }
            }
        }
	}
}

