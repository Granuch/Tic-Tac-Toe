using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.Services.Interfaces
{
    public interface IAppLogger
    {
        void LogInfo(string message);
        void LogError(string message, Exception? ex = null);
        void LogWarning(string message);
    }
}
