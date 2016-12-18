using System;
using System.Net;
using System.Threading;

namespace FiddleServer.Server
{
    class HTTPUpgrader
    {
        private TickLoop _ticker;
        private Thread _tickerThread;

        public HTTPUpgrader()
        {
            _ticker = new TickLoop();
            _tickerThread = new Thread(new ThreadStart(_ticker.StartAsync));
            _tickerThread.Start();
        }

        public async void StartAsync(string listenerPrefix)
        {
            HttpListener httpListener = new HttpListener();
            try
            {
                httpListener.Prefixes.Add(listenerPrefix);
                httpListener.Start();
                Console.WriteLine("HTTP: Listening for incoming WebSocket calls...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP: Error starting the httplistener.");
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            

            while (true)
            {
                HttpListenerContext httpContext = await httpListener.GetContextAsync();
                if (httpContext.Request.IsWebSocketRequest)
                {
                    Console.WriteLine("HTTP: Incoming connection. Starting socket thread...");
                    SocketServer.SocketServer oSocket = new SocketServer.SocketServer(httpContext);
                    GameState.AddSocket(oSocket);
                    Thread newSocket = new Thread(new ThreadStart(oSocket.ProcessRequestAsync));
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
