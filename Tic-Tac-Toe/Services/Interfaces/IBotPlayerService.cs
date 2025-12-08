using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IBotPlayerService
    {
        int GetNextMove(char[] board, char botSymbol);
    }
}
