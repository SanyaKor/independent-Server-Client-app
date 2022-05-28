using System;

using System.Threading;

namespace Client
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "client";
            var singleton = Client.Instance;

            isRunning = true;

            singleton.ConnectToServer();
            Thread mainThread = new Thread(new ThreadStart(MainThread));

            mainThread.Start();

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
