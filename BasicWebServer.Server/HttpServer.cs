using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;

        private readonly int port;

        private readonly TcpListener serverListener;

        private readonly RoutingTable routingTable;

        public const string NewLine = "\r\n";

        public HttpServer(
            string ipAddress,
            int port, 
            Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);

            this.port = port;

            this.serverListener = new TcpListener(this.ipAddress, this.port);

            routingTableConfiguration(this.routingTable = new RoutingTable());
        }

        public HttpServer(int port,Action<IRoutingTable> routingTable)
            : this("127.0.0.1", port, routingTable)
        {
        }
        public HttpServer(Action<IRoutingTable> routingTable)
           : this(8080, routingTable)
        {
        }

        public async Task Start()
        {
            this.serverListener.Start();

            Console.WriteLine($"Surver start on port {port}");
            Console.WriteLine("Listenig for requests ......");

            while (true)
            {
                var conection = await serverListener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                 {
                     using (var networStream = conection.GetStream())
                     {
                         var requestText = await this.ReadRequest(networStream);

                         Console.WriteLine(requestText);

                         var request = Request.Parse(requestText);

                         var response = this.routingTable.MatchRequest(request);

                         AddSession(request, response);

                         await WriteResponse(networStream, response);
                     }
                 });
            }
        }

        private static void AddSession(Request request, Response response)
        {
            var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            if (!sessionExists)
            {
                request.Session[Session.SessionCurrentDateKey] = DateTime.UtcNow.ToString();
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);
            }
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024;

            var buffer = new byte[bufferLength];

            var requestBuilder = new StringBuilder();

            var totalBytes = 0;

            do
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength);

                totalBytes += bytesRead;

                if (totalBytes > 10*1024)
                {
                    throw new InvalidOperationException("Request is too large");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            } while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        public async Task WriteResponse(NetworkStream networkStream, Response response)
        {
            var responseByte = Encoding.UTF8.GetBytes(response.ToString());

            if (response.FileContent != null)
            {
                responseByte = responseByte
                    .Concat(response.FileContent)
                    .ToArray();
            }

            await networkStream.WriteAsync(responseByte);
        }
    }
}
