using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.Constants
{
    public static class GameConstants
    {
        public const int BoardSize = 9;
        public const int GridSize = 3;

        public const char PlayerXSymbol = 'X';
        public const char PlayerOSymbol = 'O';
        public const char EmptyCell = '\0';

        public const string DrawResult = "Draw";

        public const int BotThinkingMinTime = 500;
        public const int BotThinkingMaxTime = 1000;

        public const int RecentGamesLimit = 10;

        public static readonly int[,] WinPatterns = new int[,]
        {
            {0, 1, 2}, // Top row
            {3, 4, 5}, // Middle row
            {6, 7, 8}, // Bottom row
            {0, 3, 6}, // Left column
            {1, 4, 7}, // Middle column
            {2, 5, 8}, // Right column
            {0, 4, 8}, // Diagonal \
            {2, 4, 6}  // Diagonal /
        };
    }
}
