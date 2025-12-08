using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IGameEngine
    {
        char[] Board { get; }
        char CurrentPlayer { get; }
        void ResetBoard();
        bool MakeMove(int index);
        bool CheckWinner();
        bool CheckDraw();
        void SwitchPlayer();
    }
}
