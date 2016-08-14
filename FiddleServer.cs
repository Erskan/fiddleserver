using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FiddleServer.SocketServer;

namespace FiddleServer
{
    class FiddleServer
    {
        static void Main(string[] args)
        {
            SocketServer.SocketServer ws = new SocketServer.SocketServer();
            ws.Start("http://localhost:9000/websocket/");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
