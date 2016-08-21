﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiddleServer.Player;
using Newtonsoft.Json;

namespace FiddleServer.Server
{
    static class GameState
    {
        static volatile List<Player.Player> players = new List<Player.Player>();
        static volatile Actors.Target currentTarget = new Actors.Target();
        public static volatile int sessionBest = 0;

        /// <summary>
        /// Initialize the static class before we need so it has a target ready
        /// </summary>
        static GameState()
        {
            Console.WriteLine("GAMESTATE: Initialized GameState...");
        }

        /// <summary>
        /// Adds player to list
        /// </summary>
        /// <param name="newPlayer">The player to add</param>
        static public void ConnectPlayer(Player.Player newPlayer)
        {
            players.Add(newPlayer);
            Console.WriteLine("GAMESTATE: Player with id: " + newPlayer.id + " is now being tracked...");
        }

        /// <summary>
        /// Updates or creates a player depending on if they can be found in the list.
        /// </summary>
        /// <param name="p">Decoded player object</param>
        static public void HandleIncomingPlayer(Player.Player p)
        {
            Player.Player pExist = players.Find(x => x.id == p.id);
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
            Player.Player exitingPlayer = players.Find(x => x.id == iid);
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

        /// <summary>
        /// Serializes the current target into a Json string and returns it
        /// </summary>
        /// <returns>The target as Json</returns>
        static public string GetTarget()
        {
            string target = JsonConvert.SerializeObject(currentTarget);
            return target;
        }

        static public string GetTargetMessage()
        {
            Message clientTargetMessage = new Message();
            clientTargetMessage.message = "newtarget";
            clientTargetMessage.target = currentTarget;
            return JsonConvert.SerializeObject(clientTargetMessage);
        }

        internal static void RegisterPoint(Player.Player player)
        {
            player.points++;
            currentTarget = new Actors.Target();
            // TODO: Invoke cross thread event to update target for all sockets
        }
    }
}
