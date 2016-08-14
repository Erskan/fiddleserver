using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace FiddleServer.SocketServer
{
    class SocketServer
    {
        //  TODO: Handle multiple connections at once
        private WebSocket socket;

        /// <summary>
        /// Starts the Websocket server by listening for requests
        /// </summary>
        public async void Start(string listenerPrefix)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(listenerPrefix);
            httpListener.Start();
            Console.WriteLine("Listening for incoming WebSocket calls...");

            while (true)
            {
                HttpListenerContext httpContext = await httpListener.GetContextAsync();
                if(httpContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(httpContext);
                }
                else
                {
                    // Upgrade required
                    httpContext.Response.StatusCode = 426;
                    httpContext.Response.Close();
                }
            }
        }

        /// <summary>
        /// Handle incoming requests
        /// </summary>
        /// <param name="request">WebSocket request made to the server</param>
        private async void ProcessRequest(HttpListenerContext request)
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

            socket = socketContext.WebSocket;
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
                        HandleMessage(socketResult, receiveBuffer);
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

        /// <summary>
        /// Handles incoming messages
        /// </summary>
        /// <param name="message">WebSocketReceiveResult to determine things about the message</param>
        /// <param name="messageData">The message data in byte format</param>
        private void HandleMessage(WebSocketReceiveResult message, byte[] messageData)
        {
            string decodedMessage = Encoding.UTF8.GetString(messageData).Substring(0, message.Count);
            Console.WriteLine(decodedMessage);
            var jsonMessage = JsonConvert.DeserializeObject(decodedMessage);
            // TODO: Game logic
            SendMessage("Message handled.");
        }

        /// <summary>
        /// Sends a text message to the client
        /// </summary>
        /// <param name="message">The message in string format</param>
        private void SendMessage(string message)
        {
            SendMessage(Encoding.UTF8.GetByteCount(message), Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Sends a binary message to the client
        /// </summary>
        /// <param name="messageLength">Number of bytes to send</param>
        /// <param name="messageData">The byte array containing the message</param>
        private async void SendMessage(int messageLength, byte[] messageData)
        {
            await socket.SendAsync(new ArraySegment<byte>(messageData, 0, messageLength), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
