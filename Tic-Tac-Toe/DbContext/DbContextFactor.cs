using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.DbContext
{
    class DbContextFactor : IDesignTimeDbContextFactory<GameContext>
    {
        private const string conStr = "Server=(localdb)\\mssqllocaldb;Database=TikTakToe;Trusted_Connection=True;";

        public GameContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            optionsBuilder.UseSqlServer(conStr);

            return new GameContext(optionsBuilder.Options);
        }
    }
}
