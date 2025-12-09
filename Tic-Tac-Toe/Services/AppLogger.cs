using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.Services.Interfaces;

namespace Tic_Tac_Toe.Services
{
    public class AppLogger : IAppLogger
    {
        private readonly ILogger<AppLogger> _logger;

        public AppLogger(ILogger<AppLogger> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
            System.Diagnostics.Debug.WriteLine($"[INFO] {message}");
        }

        public void LogError(string message, Exception? ex = null)
        {
            _logger.LogError(ex, message);
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}");
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
            System.Diagnostics.Debug.WriteLine($"[WARNING] {message}");
        }
    }
}
