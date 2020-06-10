using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    public enum ERemoteCommand
    {
        None,
        StartGame,
        EndGame,
        GuessRoom,
        AddPoints,
        GetMap,
        GetPosition
    }
}
