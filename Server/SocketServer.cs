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

using FiddleServer.Server;

namespace FiddleServer.SocketServer
{
    class SocketServer
    {
        private WebSocket socket;
        private HttpListenerContext request;
        private string gId;
        private ulong tickCount;

        public SocketServer(HttpListenerContext hlc)
        {
            Console.WriteLine("SOCKET: Constructing socket.");
            tickCount = 0;
            request = hlc;
            Thread.Sleep(1);
        }

        /// <summary>
        /// Handle incoming requests to the socket
        /// </summary>
        public async void ProcessRequest()
        {
            Console.WriteLine("SOCKET: Processing socket request.");
            WebSocketContext socketContext = null;
            string clientIP = request.Request.RemoteEndPoint.Address.ToString();
            try
            {
                socketContext = await request.AcceptWebSocketAsync(subProtocol: null);
                Console.WriteLine("SOCKET: {0} connected to the server.", clientIP);
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
                        GameState.RemoveSocket(this);
                        Console.WriteLine("SOCKET: {0} disconnected from the server.", clientIP);
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
                //GameState.DisconnectPlayer(clientIP); /* Dont use clientIP for id. Should find something better. */
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
            //Console.WriteLine(decodedMessage);
            
            Message msg = JsonConvert.DeserializeObject<Message>(decodedMessage);

            switch (msg.message)
            {
                // Send entire state to client
                case "start":
                    Console.WriteLine("SOCKET: start message from player {0}.", msg.players[0].id);
                    gId = msg.players[0].id;
                    GameState.HandleIncomingPlayer(msg.players[0]);
                    SendMessage(JsonConvert.SerializeObject(GameState.GetGameStartMessage()));
                    break;
                // Regular tick
                case "tick":
                    //Console.WriteLine("SOCKET: player message.");
                    if (!GameState.GetTarget().Id.Equals(msg.target.Id)) // If there is a target mismatch we need to update the client target.
                    {
                        SendMessage(JsonConvert.SerializeObject(new Message
                        {
                            message = "newtarget",
                            players = null, /* send scoring player id to update scoreboard? */
                            target = GameState.GetTarget(),
                            alertmessage = null
                        }));
                    }
                    GameState.HandleIncomingPlayer(msg.players[0]);
                    break;
                // Client disconnection
                case "endgame":
                    Console.WriteLine("SOCKET: endgame message.");
                    string endOfGameMessage = (msg.players[0].points > GameState.sessionBest) ? "You set a new record with: " + msg.players[0].points.ToString() + " points!" : "The score to beat this session is: " + GameState.sessionBest.ToString() + " points!";
                    //Console.WriteLine(endOfGameMessage);
                    SendMessage(endOfGameMessage);
                    GameState.DisconnectPlayer(msg.players[0].id);
                    break;
                // Client scored
                case "registerpoint":
                    Console.WriteLine("SOCKET: registerpoint message.");
                    GameState.RegisterPoint(msg.players[0]);
                    SendMessage(GameState.GetTargetMessageString());
                    break;
                // Client needs current target
                case "targetrequest":
                    Console.WriteLine("SOCKET: targetrequest message.");
                    SendMessage(GameState.GetTargetMessageString());
                    break;


                default:
                    Console.WriteLine("SOCKET: Unrecognized message type {0}. Ignoring...", msg.message);
                    break;
            }
            // TODO: Game logic
            //SendMessage("Message handled.");
        }

        /// <summary>
        /// Sends a text message to the client
        /// </summary>
        /// <param name="message">The message in string format</param>
        public void SendMessage(string message)
        {
            // Don't spam the console...
            //Console.WriteLine("SOCKET: Sending message: " + message);
            // TODO: Check if we should be sending Server.Message type back to client as well.
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
