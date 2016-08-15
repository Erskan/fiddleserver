using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Server
{
    static class GameState
    {
        static volatile List<Player.Player> players;

        /// <summary>
        /// Adds player to list
        /// </summary>
        /// <param name="newPlayer">The player to add</param>
        static public void ConnectPlayer(Player.Player newPlayer)
        {
            players.Add(newPlayer);
        }

        /// <summary>
        /// Removes player by IP if player exists in list
        /// </summary>
        /// <param name="iip">IP of player to remove. Used to identify</param>
        static public void DisconnectPlayer(string iip)
        {
            players.Remove(players.Find(x => x.Ip == iip));
        }
    }
}
