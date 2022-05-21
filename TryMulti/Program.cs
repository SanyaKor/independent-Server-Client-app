using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TryMulti
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";

            Server.Start(50, 5555);
            Console.ReadKey();
        }
    }
}
