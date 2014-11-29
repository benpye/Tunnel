using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace Tunnel
{
    class Server
    {
        private HttpServer _httpServer;
        private TcpClient _endpoint;
        private NetworkStream _endpointStream;

        private string _endpointHost;
        private int _endpointPort;

        public Server(int httpPort, string endpoint, int endpointPort)
        {
            _httpServer = new HttpServer(httpPort, HandleRequest);

            _endpointHost = endpoint;
            _endpointPort = endpointPort;
        }

        public Task RunAsync()
        {
            return _httpServer.RunAsync();
        }

        public void Connect()
        {
            try
            {
                _endpoint = new TcpClient(_endpointHost, _endpointPort);
                _endpointStream = _endpoint.GetStream();
            }
            catch (SocketException) { }
        }

        public void HandleRequest(HttpListenerContext ctx)
        {
            if(ctx.Request.RawUrl.Contains("new") || _endpoint == null || !_endpoint.Connected)
            {
                Console.WriteLine("Server: Creating new stream");
                Connect();
            }

            if(!_endpoint.Connected)
            {
                Console.WriteLine("Server: Request dropped: endpoint disconnected");
                return;
            }

            ctx.Response.SendChunked = false;

            // If POST we have incoming data
            if(ctx.Request.HttpMethod == "POST")
            {
                var bodyStream = ctx.Request.InputStream;
                var reader = new StreamReader(bodyStream);

                var dataBytes = Convert.FromBase64String(reader.ReadToEnd());
                _endpointStream.Write(dataBytes, 0, dataBytes.Length);
            }

            if (_endpointStream.DataAvailable)
            {
                // Replace 2048 by arg
                var tempBuffer = new byte[2048];
                int bytesRead = _endpointStream.Read(tempBuffer, 0, tempBuffer.Length);

                var b64 = Encoding.ASCII.GetBytes(Convert.ToBase64String(tempBuffer, 0, bytesRead));

                ctx.Response.ContentLength64 = b64.LongLength;

                ctx.Response.OutputStream.Write(b64, 0, b64.Length);
            }

            ctx.Response.OutputStream.Close();
        }
    }
}
