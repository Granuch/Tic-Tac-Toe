using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public enum BotDifficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    public class BotPlayerService : IBotPlayerService
    {
        private readonly BotDifficulty _difficulty;
        private readonly Random _random;

        public BotPlayerService(BotDifficulty difficulty)
        {
            _difficulty = difficulty;
            _random = new Random();
        }

        public int GetNextMove(char[] board, char botSymbol)
        {
            var emptyCells = GetEmptyCells(board);

            if (emptyCells.Count == 0)
                return -1;

            return _difficulty switch
            {
                BotDifficulty.Easy => GetEasyMove(emptyCells),
                BotDifficulty.Medium => GetMediumMove(board, botSymbol, emptyCells),
                BotDifficulty.Hard => GetHardMove(board, botSymbol, emptyCells),
                _ => GetEasyMove(emptyCells)
            };
        }

        private int GetEasyMove(List<int> emptyCells)
        {
            return emptyCells[_random.Next(emptyCells.Count)];
        }

        private int GetMediumMove(char[] board, char botSymbol, List<int> emptyCells)
        {
            char playerSymbol = botSymbol == 'O' ? 'X' : 'O';

            if (_random.Next(100) < 50)
            {
                return GetEasyMove(emptyCells);
            }

            int winMove = FindWinningMove(board, botSymbol);
            if (winMove != -1)
                return winMove;

            int blockMove = FindWinningMove(board, playerSymbol);
            if (blockMove != -1)
                return blockMove;

            if (board[4] == '\0')
                return 4;

            int[] corners = { 0, 2, 6, 8 };
            var emptyCorners = corners.Where(c => board[c] == '\0').ToList();
            if (emptyCorners.Any())
                return emptyCorners[_random.Next(emptyCorners.Count)];

            return emptyCells[_random.Next(emptyCells.Count)];
        }

        private int GetHardMove(char[] board, char botSymbol, List<int> emptyCells)
        {
            int bestScore = int.MinValue;
            int bestMove = emptyCells[0];

            foreach (int cell in emptyCells)
            {
                board[cell] = botSymbol;
                int score = Minimax(board, 0, false, botSymbol);
                board[cell] = '\0';

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = cell;
                }
            }

            return bestMove;
        }

        private int Minimax(char[] board, int depth, bool isMaximizing, char botSymbol)
        {
            char playerSymbol = botSymbol == 'O' ? 'X' : 'O';

            if (CheckWinner(board, botSymbol))
                return 10 - depth;

            if (CheckWinner(board, playerSymbol))
                return depth - 10;

            if (GetEmptyCells(board).Count == 0)
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                var emptyCells = GetEmptyCells(board);

                foreach (int cell in emptyCells)
                {
                    board[cell] = botSymbol;
                    int score = Minimax(board, depth + 1, false, botSymbol);
                    board[cell] = '\0';
                    bestScore = Math.Max(score, bestScore);
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                var emptyCells = GetEmptyCells(board);

                foreach (int cell in emptyCells)
                {
                    board[cell] = playerSymbol;
                    int score = Minimax(board, depth + 1, true, botSymbol);
                    board[cell] = '\0';
                    bestScore = Math.Min(score, bestScore);
                }

                return bestScore;
            }
        }

        private int FindWinningMove(char[] board, char symbol)
        {
            var emptyCells = GetEmptyCells(board);

            foreach (int cell in emptyCells)
            {
                board[cell] = symbol;
                bool isWin = CheckWinner(board, symbol);
                board[cell] = '\0';

                if (isWin)
                    return cell;
            }

            return -1;
        }

        private bool CheckWinner(char[] board, char symbol)
        {
            int[,] winPatterns = new int[,]
            {
                {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
                {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
                {0, 4, 8}, {2, 4, 6}
            };

            for (int i = 0; i < 8; i++)
            {
                if (board[winPatterns[i, 0]] == symbol &&
                    board[winPatterns[i, 1]] == symbol &&
                    board[winPatterns[i, 2]] == symbol)
                {
                    return true;
                }
            }

            return false;
        }

        private List<int> GetEmptyCells(char[] board)
        {
            var emptyCells = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == '\0')
                    emptyCells.Add(i);
            }
            return emptyCells;
        }
    }
}
