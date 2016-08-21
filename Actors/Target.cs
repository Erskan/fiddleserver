using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Actors
{
    [Serializable]
    class Target
    {
        Guid id;
        int x, y, size;

        public Target()
        {

        }

        public void GenerateTargetValues()
        {
            Random rnd = new Random();
            Id = Guid.NewGuid(); // Guid as a way of tracking targets if needed.
            X = rnd.Next(2, 98); // Percentage of screen area to be independent of size.
            Y = rnd.Next(2, 98); // Percentage of screen area to be independent of size.
            Size = 10; // TODO: Generate fitting sizes depending on player? Maybe?
            Console.WriteLine("TARGET: id: " + Id.ToString() + " x: " + X.ToString() + " y: " + Y.ToString() + " size: " + Size.ToString());
        }

        public Guid Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public int X
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

        public int Y
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
    }
}
