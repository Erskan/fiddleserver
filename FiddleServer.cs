using System;

namespace FiddleServer
{
    class FiddleServer
    {
        static void Main(string[] args)
        {
            Server.HTTPUpgrader ws = new Server.HTTPUpgrader();
            ws.Start("http://192.168.1.11:9000/websocket/");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
