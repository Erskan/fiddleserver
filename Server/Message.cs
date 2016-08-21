using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Server
{
    /// <summary>
    /// Dump everything a message can contain in here,
    /// deserialize on recieved message and check message string
    /// </summary>
    [Serializable]
    class Message
    {
        public string message;
        public Player.Player player;
        public Actors.Target target;
        public Actors.Alert alertmessage;
    }
}
