using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            var singleton = Client.Instance;
            
            singleton.ConnectToServer();
            Console.ReadKey();
        }
    }
}
