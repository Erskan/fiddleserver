using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiddleServer.Actors;
using Newtonsoft.Json;

namespace FiddleServer.Server
{
    static class GameState
    {
        static List<Actors.Player> players = new List<Actors.Player>();
        private static readonly object _playersLock = new object();
        static List<PlayerModel> models = new List<PlayerModel>();
        private static readonly object _modelsLock = new object();
        static Actors.Target currentTarget = new Actors.Target();
        private static readonly object _targetLock = new object();
        public static int sessionBest = 0;
        private static readonly object _sessionBestLock = new object();
        public static List<SocketServer.SocketServer> socketList = new List<SocketServer.SocketServer>();
        private static readonly object _listLock = new object();

        /// <summary>
        /// Initialize the static class before we need so it has a target ready
        /// </summary>
        static GameState()
        {
            currentTarget.GenerateTargetValues();
            Console.WriteLine("GAMESTATE: Initialized GameState...");
        }

        /// <summary>
        /// Adds player to list
        /// </summary>
        /// <param name="newPlayer">The player to add</param>
        static public void ConnectPlayer(Actors.Player newPlayer)
        {
            lock (_playersLock)
            {
                players.Add(newPlayer);
                Console.WriteLine("GAMESTATE: Player with id: " + newPlayer.id + " is now being tracked...");
            }
        }

        /// <summary>
        /// Adds the socket to the list of sockets handled by the server
        /// </summary>
        /// <param name="srv">New socket to add</param>
        static public void AddSocket(SocketServer.SocketServer srv)
        {
            lock(_listLock)
            {
                socketList.Add(srv);
            }
        }

        /// <summary>
        /// Removes the disconnected socket from the server
        /// </summary>
        /// <param name="srv">Socket to remove</param>
        static public void RemoveSocket(SocketServer.SocketServer srv)
        {
            lock (_listLock)
            {
                socketList.Remove(srv);
            }
        }

        /// <summary>
        /// Sends a given message to every client in the socket collection
        /// </summary>
        /// <param name="msg">The message to broadcast</param>
        public static void BroadCastMessage(Message msg)
        {
            Console.WriteLine("GAMESTATE: Broadcasting message to all clients.");
            foreach (var socket in socketList)
            {
                socket.SendMessage(JsonConvert.SerializeObject(msg));
            }
        }

        /// <summary>
        /// Updates or creates a player depending on if they can be found in the list.
        /// </summary>
        /// <param name="p">Decoded player object</param>
        static public void HandleIncomingPlayer(Actors.Player p)
        {
            Actors.Player pExist = players.Find(x => x.id == p.id);
            if (pExist == null)
            {
                ConnectPlayer(p);
                return;
            }
            int idx = players.IndexOf(pExist);
            lock (_playersLock)
            {
                players[idx] = p;
            }
        }

        /// <summary>
        /// Removes player by Id if player exists in list
        /// </summary>
        /// <param name="iid">Id of player to remove. Used to identify</param>
        static public void DisconnectPlayer(string iid)
        {
            Actors.Player exitingPlayer = players.Find(x => x.id == iid);
            Console.WriteLine("GAMESTATE: Player with id: " + exitingPlayer.id + " is exiting the session with a score of " + exitingPlayer.points.ToString());
            lock (_sessionBestLock)
            {
                if (exitingPlayer.points > sessionBest)
                    sessionBest = exitingPlayer.points;
            }
            lock (_playersLock)
            {
                players.Remove(exitingPlayer);
            }
        }

        /// <summary>
        /// Gets the list of active players
        /// </summary>
        /// <returns>List of players</returns>
        internal static List<Player> GetPlayers()
        {
            return players;
        }

        /// <summary>
        /// Returns this sessions highest recorded score.
        /// </summary>
        /// <returns>The score as an integer.</returns>
        static public int GetSessionBest()
        {
            return sessionBest;
        }

        /// <summary>
        /// Returns the current active target
        /// </summary>
        /// <returns>The active target</returns>
        static public Actors.Target GetTarget()
        {
            return currentTarget;
        }

        /// <summary>
        /// Generate a message for the clients to read as string
        /// </summary>
        /// <returns>The message as a JSON string</returns>
        static public string GetTargetMessageString()
        {
            Message clientTargetMessage = new Message();
            clientTargetMessage.message = "newtarget";
            clientTargetMessage.target = currentTarget;
            return JsonConvert.SerializeObject(clientTargetMessage);
        }

        /// <summary>
        /// Generate a message for the clients to read
        /// </summary>
        /// <returns>The message object</returns>
        static public Message GetTargetMessage()
        {
            Message clientTargetMessage = new Message();
            clientTargetMessage.message = "newtarget";
            clientTargetMessage.target = currentTarget;
            return clientTargetMessage;
        }

        /// <summary>
        /// Dumps all current info to the starting client
        /// </summary>
        /// <returns>Message ready to be serialized.</returns>
        static public Message GetGameStartMessage()
        {
            Message clientStartMessage = new Message();
            clientStartMessage.message = "start";
            lock (_playersLock)
            {
                clientStartMessage.players = GameState.players;
            }
            lock (_modelsLock)
            {
                clientStartMessage.models = GameState.models;
            }
            clientStartMessage.target = GetTarget();
            return clientStartMessage;
        }

        /// <summary>
        /// Registers the scoring of a point from a client
        /// </summary>
        /// <param name="player">The player that first registered the point</param>
        internal static void RegisterPoint(Actors.Player player)
        {
            int idx = players.IndexOf(player);
            lock (_playersLock)
            {
                players[idx].points++;
            }
            lock (_targetLock)
            {
                currentTarget = new Actors.Target();
                currentTarget.GenerateTargetValues();
            }
            // Notify all clients that a new target is generated
            //BroadCastMessage(GetTargetMessage());
        }
    }
}
