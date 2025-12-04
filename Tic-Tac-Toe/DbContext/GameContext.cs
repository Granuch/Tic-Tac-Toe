using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.DbContext
{
    public class GameContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options) { }
        public GameContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        public DbSet<Models.Player> Players { get; set; }
        public DbSet<Models.GameResult> GameResults { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}