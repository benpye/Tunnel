using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;

namespace Tunnel
{
    class Client
    {
        private HttpClient _httpClient;
        private TcpListener _endpoint;
        private TcpClient _endpointClient;
        private NetworkStream _endpointStream;
        private bool _newStream;
        private byte[] _buffer;

        public Client(string httpHost, int httpPort, int endpointPort, int bufferSize)
        {
            _endpoint = new TcpListener(IPAddress.Loopback, endpointPort);
            _endpoint.Start();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"http://{httpHost}:{httpPort}/");
            _buffer = new byte[bufferSize];
        }

       public void AcceptClient()
        {
            var t = _endpoint.AcceptTcpClientAsync();
            t.ContinueWith((task) =>
            {
                // We only support 1 connection
                _endpointClient?.Close();
                _endpointClient = task.Result;
                _endpointStream = _endpointClient.GetStream();

                _newStream = true;

                AcceptClient();
            });
        }

        public async Task SendRequestAsync()
        {
            // Nothing is connected to the tunnel
            if (_endpointStream == null)
                return;

            HttpResponseMessage msg;

            if (_endpointStream.DataAvailable)
            {
                int bytesRead = _endpointStream.Read(_buffer, 0, _buffer.Length);

                StringContent content = new StringContent(Convert.ToBase64String(_buffer, 0, bytesRead));

                msg = await _httpClient.PostAsync(_newStream ? "/new" : "", content);
            }
            else
                msg = await _httpClient.GetAsync(_newStream ? "/new" : "");

            var str = await msg.Content.ReadAsStringAsync();

            if (str.Length > 0)
            {
                byte[] buf = Convert.FromBase64String(str);
                // If we are connected to something forward the data, else drop it
                _endpointStream.Write(buf, 0, buf.Length);
            }

            if(_newStream)
                _newStream = false;
        }

    }
}
