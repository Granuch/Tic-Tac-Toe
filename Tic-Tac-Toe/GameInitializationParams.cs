using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe
{
    public class GameInitializationParams
    {
        public string PlayerXName { get; set; } = "";
        public string PlayerOName { get; set; } = "";
        public bool IsPlayingWithBot { get; set; }
        public int BotDifficulty { get; set; }
    }
}
