using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Player
{
    class Player : IEquatable<Player>
    {
        public string name, ip;
        public double x, y, speedx, speedy;
        public int points;
        public Object model;
                
        public bool Equals(Player other)
        {
            return (this.ip.Equals(other.ip));
        }
    }
}
