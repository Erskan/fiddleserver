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
        static volatile List<Actors.Player> players = new List<Actors.Player>();
        static volatile Actors.Target currentTarget = new Actors.Target();
        public static volatile int sessionBest = 0;

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
            players.Add(newPlayer);
            Console.WriteLine("GAMESTATE: Player with id: " + newPlayer.id + " is now being tracked...");
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
            players[idx] = p;
        }

        /// <summary>
        /// Removes player by IP if player exists in list
        /// </summary>
        /// <param name="iip">IP of player to remove. Used to identify</param>
        static public void DisconnectPlayer(string iid)
        {
            Actors.Player exitingPlayer = players.Find(x => x.id == iid);
            Console.WriteLine("GAMESTATE: Player with id: " + exitingPlayer.id + " is exiting the session with a score of " + exitingPlayer.points.ToString());
            if (exitingPlayer.points > sessionBest)
                sessionBest = exitingPlayer.points;
            players.Remove(exitingPlayer);
        }

        /// <summary>
        /// Returns this sessions highest recorded score.
        /// </summary>
        /// <returns>The score as an integer.</returns>
        static public int GetSessionBest()
        {
            return sessionBest;
        }

        static public Actors.Target GetTarget()
        {
            return currentTarget;
        }

        static public string GetTargetMessage()
        {
            Message clientTargetMessage = new Message();
            clientTargetMessage.message = "newtarget";
            clientTargetMessage.target = currentTarget;
            return JsonConvert.SerializeObject(clientTargetMessage);
        }

        internal static void RegisterPoint(Actors.Player player)
        {
            int idx = players.IndexOf(player);
            players[idx].points++;
            currentTarget = new Actors.Target();
            currentTarget.GenerateTargetValues();
            // TODO: Invoke cross thread event to update target for all sockets?
        }
    }
}
