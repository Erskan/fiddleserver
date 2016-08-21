using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Actors
{
    [Serializable]
    class Alert
    {
        private string alertmsg;

        public string AlertMessage
        {
            get
            {
                return alertmsg;
            }

            set
            {
                alertmsg = value;
            }
        }
    }
}
