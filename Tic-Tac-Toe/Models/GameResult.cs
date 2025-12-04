using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows;

namespace Tic_Tac_Toe.Models
{
    public class GameResult
    {
        [Key]
        public required int Id { get; set; }

        [Required]
        public required int PlayerX { get; set; }

        [Required]
        public required int PlayerO { get; set; }
        
        [Required]
        public required string Winner { get; set; }

        [Required]
        public required TimeSpan Duration { get; set; }

        [Required]
        public required DateTime PlayedAt { get; set; }

        public override string ToString()
        {
            return $"GameResult ID: {Id}, Player X ID: {PlayerX}, Player O ID: {PlayerO}, Winner: {Winner}, Duration: {Duration}, Played At: {PlayedAt}";
        }
    }
}
