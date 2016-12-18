using System;
using System.Collections.Generic;

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
        public ulong tick;
        public List<Actors.Player> players;
        public List<Actors.PlayerModel> models;
        public Actors.Target target;
        public Actors.Alert alertmessage;
    }
}
