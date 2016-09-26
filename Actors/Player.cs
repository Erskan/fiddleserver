using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FiddleServer.Actors
{
    [Serializable]
    class Player : IEquatable<Player>
    {
        public string name, id;
        public double x, y, speedx, speedy;
        public int points;
        [JsonIgnore]
        public object model;
                
        public bool Equals(Player other)
        {
            return (this.id.Equals(other.id));
        }
    }
}
