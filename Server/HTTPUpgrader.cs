using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace FiddleServer.Server
{
    class HTTPUpgrader
    {
        public async void Start(string listenerPrefix)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(listenerPrefix);
            httpListener.Start();
            Console.WriteLine("HTTP: Listening for incoming WebSocket calls...");

            while (true)
            {
                HttpListenerContext httpContext = await httpListener.GetContextAsync();
                if (httpContext.Request.IsWebSocketRequest)
                {
                    Console.WriteLine("HTTP: Incoming connection. Starting socket thread...");
                    SocketServer.SocketServer oSocket = new SocketServer.SocketServer(httpContext);
                    GameState.AddSocket(oSocket);
                    Thread newSocket = new Thread(new ThreadStart(oSocket.ProcessRequest));
                    newSocket.Start();
                }
                else
                {
                    // Upgrade required
                    httpContext.Response.StatusCode = 426;
                    httpContext.Response.Close();
                }
            }
        }
    }
}
