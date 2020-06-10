using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    public class Hider : Player
    {
        #region Properties
        public bool IsFound { get; private set; }
        #endregion

        #region Fields
        #endregion

        #region Constructor

        #endregion

        #region Functions
        public void PlayerFound()
        {
            IsFound = true;
        }

        public List<Room> SetMap()
        {
            throw new NotImplementedException();
        }

        public void StartHiding()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods
        #endregion
    }
}
