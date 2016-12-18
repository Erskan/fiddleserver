using System;
using System.Timers;

namespace FiddleServer.Server
{
    class TickLoop
    {
        private System.Timers.Timer _timer;
        private ulong _tickCount;
#region GetSet
        public ulong TickCount
        {
            get
            {
                return _tickCount;
            }

            set
            {
                _tickCount = value;
            }
        }
#endregion
        public TickLoop()
        {
            _timer = new System.Timers.Timer();
            TickCount = 0;
            Console.WriteLine("TICKLOOP: Server side game loop initialized.");
        }

        internal void StartAsync()
        {
            _timer.Elapsed += SendTick;
            _timer.Interval = 70; // 17 is about 60 tps
            _timer.Start();
        }

        /// <summary>
        /// Sends a tick message to every client when timer hits the interval
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void SendTick(object source, ElapsedEventArgs e)
        {
            TickCount++;
            GameState.BroadCastMessage( BuildMessage() );
        }

        /// <summary>
        /// Builds and returns a tick message containing the current game state
        /// </summary>
        /// <returns>Message to broadcast to clients (tick)</returns>
        private Message BuildMessage()
        {
            return new Message()
            {
                message = "tick",
                tick = TickCount,
                players = GameState.GetPlayers(),
                target = GameState.GetTarget(),
                alertmessage = null
            };
        }
    }
}
