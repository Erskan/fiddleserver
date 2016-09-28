using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiddleServer.Actors
{
    class PlayerModel
    {
        private string id;
        private byte[] model;

        #region GetSet
        public string Id
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

        public byte[] Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }
        #endregion
    }
}
