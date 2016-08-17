using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Server
{
    static class GameState
    {
        static volatile List<Player.Player> players = new List<Player.Player>();

        /// <summary>
        /// Adds player to list
        /// </summary>
        /// <param name="newPlayer">The player to add</param>
        static public void ConnectPlayer(Player.Player newPlayer)
        {
            players.Add(newPlayer);
        }

        /// <summary>
        /// Updates or creates a player depending on if they can be found in the list.
        /// </summary>
        /// <param name="p">Decoded player object</param>
        static public void HandleIncomingPlayer(Player.Player p)
        {
            Player.Player pExist = players.Find(x => x.ip == p.ip);
            if (pExist == null)
            {
                ConnectPlayer(p);
                Console.WriteLine("GAMESTATE: Player " + p.name + " created.");
                return;
            }
            int idx = players.IndexOf(pExist);
            players[idx] = p;
        }

        /// <summary>
        /// Removes player by IP if player exists in list
        /// </summary>
        /// <param name="iip">IP of player to remove. Used to identify</param>
        static public void DisconnectPlayer(string iip)
        {
            players.Remove(players.Find(x => x.ip == iip));
        }
    }
}
