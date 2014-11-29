using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CLAP;

namespace Tunnel
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.RunConsole<Application>(args);
        }
    }
}
