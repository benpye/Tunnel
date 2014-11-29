using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Tunnel
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Server(80, "curlybracket.co.uk", 22);
            s.RunAsync();
            Console.WriteLine("Server running");
            var c = new Client("localhost", 80, 900);
            c.AcceptClient();
            Console.WriteLine("Client running");
            while (true)
            {
                c.SendRequestAsync().Wait();
                Thread.Sleep(10);
            }
        }
    }
}
