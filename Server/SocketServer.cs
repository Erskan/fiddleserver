using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace FiddleServer.SocketServer
{
    class SocketServer
    {
        /// <summary>
        /// Starts the Websocket server by listening for requests
        /// </summary>
        public async void Start(string listenerPrefix)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(listenerPrefix);
            httpListener.Start();
            Console.WriteLine("Listening for incoming HTTP calls...");

            while (true)
            {
                HttpListenerContext httpContext = await httpListener.GetContextAsync();
                if(httpContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                    httpContext.Response.Close();
                }
            }
        }

        /// <summary>
        /// Handle incoming requests
        /// </summary>
        /// <param name="request">WebSocket request made to the server</param>
        public async void ProcessRequest(HttpListenerContext request)
        {
            WebSocketContext socketContext = null;
            string clientIP = request.Request.RemoteEndPoint.Address.ToString();
            try
            {
                socketContext = await request.AcceptWebSocketAsync(subProtocol: null);
                Console.WriteLine("{0} connected to the server.", clientIP);
            }
            catch (Exception e)
            {
                request.Response.StatusCode = 500;
                request.Response.Close();
                Console.WriteLine(e.ToString());
                return;
            }

            WebSocket socket = socketContext.WebSocket;
            try
            {
                byte[] receiveBuffer = new byte[1024];
                while(socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult socketResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if(socketResult.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        Console.WriteLine("{0} disconnected from the server.", clientIP);
                    }
                    else
                    {
                        await socket.SendAsync(new ArraySegment<byte>(receiveBuffer, 0, socketResult.Count), WebSocketMessageType.Binary, socketResult.EndOfMessage, CancellationToken.None);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (socket != null)
                    socket.Dispose();
            }
        }
    }
}
