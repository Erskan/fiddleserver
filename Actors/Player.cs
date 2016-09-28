using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Actors
{
    [Serializable]
    class Player : IEquatable<Player>
    {
        public string name, id;
        public double x, y, speedx, speedy;
        public int points;
                
        public bool Equals(Player other)
        {
            return (this.id.Equals(other.id));
        }
    }
}
