using Tic_Tac_Toe.Constants;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public class GameEngineService : IGameEngine
    {
        public char[] Board { get; private set; }
        public char CurrentPlayer { get; private set; }

        public GameEngineService()
        {
            Board = new char[GameConstants.BoardSize];
            ResetBoard();
        }

        public void ResetBoard()
        {
            Board = new char[GameConstants.BoardSize];
            CurrentPlayer = GameConstants.PlayerXSymbol;
        }

        public bool MakeMove(int index)
        {
            if (index < 0 || index >= GameConstants.BoardSize || Board[index] != GameConstants.EmptyCell)
                return false;

            Board[index] = CurrentPlayer;
            return true;
        }

        public bool CheckWinner()
        {
            char player = CurrentPlayer;

            for (int i = 0; i < GameConstants.WinPatterns.GetLength(0); i++)
            {
                if (Board[GameConstants.WinPatterns[i, 0]] == player &&
                    Board[GameConstants.WinPatterns[i, 1]] == player &&
                    Board[GameConstants.WinPatterns[i, 2]] == player)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckDraw()
        {
            return Board.All(cell => cell != GameConstants.EmptyCell);
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == GameConstants.PlayerXSymbol
                ? GameConstants.PlayerOSymbol
                : GameConstants.PlayerXSymbol;
        }
    }
}