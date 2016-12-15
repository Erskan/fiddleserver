using System;

namespace FiddleServer.Actors
{
    [Serializable]
    class Alert
    {
        private string alertmsg;

        #region GetSet
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
        #endregion
    }
}
