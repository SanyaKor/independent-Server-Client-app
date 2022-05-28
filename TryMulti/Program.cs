using System;
using System.Threading;

namespace TryMulti
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            isRunning = true;

            
            
            Thread mainThread = new Thread(new ThreadStart(MainThread));

            mainThread.Start();

            Server.Start(50, 5555);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per seconds.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                GameLogic.Update();

                _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);
            }
        }
    }
}
