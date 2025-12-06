using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.DbContext
{
    public class GameContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        public DbSet<Models.Player> Players { get; set; }
        public DbSet<Models.GameResult> GameResults { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Player>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(p => p.Name);
            });

            modelBuilder.Entity<Models.GameResult>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.PlayerX)
                    .IsRequired();

                entity.Property(g => g.PlayerO)
                    .IsRequired();

                entity.Property(g => g.Winner)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(g => g.Duration)
                    .IsRequired();

                entity.Property(g => g.PlayedAt)
                    .IsRequired();

                entity.HasIndex(g => g.PlayedAt);
                entity.HasIndex(g => g.PlayerX);
                entity.HasIndex(g => g.PlayerO);
            });
        }
    }
}