using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.DbContext;

namespace Tic_Tac_Toe.RepositoryPattern
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameContext _context;
        private IDbContextTransaction? _transaction;
        private IPlayerRepository? _playerRepository;
        private IGameResultRepository? _gameResultRepository;

        public UnitOfWork(GameContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IPlayerRepository Players
        {
            get
            {
                _playerRepository ??= new PlayerRepository(_context);
                return _playerRepository;
            }
        }

        public IGameResultRepository GameResults
        {
            get
            {
                _gameResultRepository ??= new GameResultRepository(_context);
                return _gameResultRepository;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
