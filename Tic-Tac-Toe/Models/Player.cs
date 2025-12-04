using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tic_Tac_Toe.Models
{
    public class Player
    {
        [Key]
        public required int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public override string ToString()
        {
            return $"Player ID: {Id}, Player Name: {Name}";
        }
    }
}
