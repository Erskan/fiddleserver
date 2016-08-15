using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Player
{
    class Player : IEquatable<Player>
    {
        private string name, ip;
        private double x, y;
        private double[] speed;
        private int points;

        public Player(string iname, double ix, double iy, string iip)
        {
            this.Name = iname;
            this.X = ix;
            this.Y = iy;
            this.Ip = iip;
            this.Points = 0;
            this.Speed = new double[2] { 0, 0 };
        }
        
        public bool Equals(Player other)
        {
            return (this.Ip.Equals(other.Ip));
        }

        #region Get and set properties
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public double[] Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }

        public int Points
        {
            get
            {
                return points;
            }

            set
            {
                points = value;
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        #endregion
    }
}
