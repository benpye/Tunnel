using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Tunnel
{
    public class HttpServer
    {
        private HttpListener _listener;
        private Action<HttpListenerContext> _callback;

        public HttpServer(int port, Action<HttpListenerContext> method)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://+:\{port}/");

            _callback = method;

            _listener.Start();
        }

        public async Task RunAsync()
        {
            while (_listener.IsListening)
            {
                _callback(await _listener.GetContextAsync());
            }
        }
    }
}
