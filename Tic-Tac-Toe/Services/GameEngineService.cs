using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public class GameEngineService : IGameEngine
    {
        public char[] Board { get; private set; }
        public char CurrentPlayer { get; private set; }

        public GameEngineService()
        {
            Board = new char[9];
            ResetBoard();
        }

        public void ResetBoard()
        {
            Board = new char[9];
            CurrentPlayer = 'X';
        }

        public bool MakeMove(int index)
        {
            if (index < 0 || index >= 9 || Board[index] != '\0')
                return false;

            Board[index] = CurrentPlayer;
            return true;
        }

        public bool CheckWinner()
        {
            char p = CurrentPlayer;

            int[,] winPatterns = new int[,]
            {
                {0,1,2}, {3,4,5}, {6,7,8},
                {0,3,6}, {1,4,7}, {2,5,8},
                {0,4,8}, {2,4,6}
            };

            for (int i = 0; i < 8; i++)
            {
                if (Board[winPatterns[i, 0]] == p &&
                    Board[winPatterns[i, 1]] == p &&
                    Board[winPatterns[i, 2]] == p)
                    return true;
            }

            return false;
        }

        public bool CheckDraw()
        {
            return Board.All(cell => cell != '\0');
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == 'X' ? 'O' : 'X';
        }
    }
}
