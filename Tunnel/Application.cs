using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;

namespace Tunnel
{
    class Application
    {
        [Verb(Description = "Run the HTTP server side of the tunnel")]
        public static void Server(
            [DefaultValue(80)]
            int httpPort,
            string endpointHost,
            int endpointPort,
            [DefaultValue(2048)]
            int bufferSize)
        {
            var s = new Server(httpPort, endpointHost, endpointPort, bufferSize);
            s.RunAsync().Wait();
        }

        [Verb(Description = "Run the HTTP client side of the tunnel")]
        public static void Client(
            string httpHost,
            [DefaultValue(80)]
            int httpPort,
            int endpointPort,
            [DefaultValue(2048)]
            int bufferSize)
        {
            var c = new Client(httpHost, httpPort, endpointPort, bufferSize);
            c.AcceptClient();
            while (true)
                c.SendRequestAsync().Wait();
        }
    }
}
