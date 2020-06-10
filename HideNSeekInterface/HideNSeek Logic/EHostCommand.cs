using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    public enum EHostCommand
    {
        None,
        EndGame,
        Disconnect,
        GetRemainingTime,
        GuessRoom,
        ViewMaps,
        GetHiderPosition
    }
}
