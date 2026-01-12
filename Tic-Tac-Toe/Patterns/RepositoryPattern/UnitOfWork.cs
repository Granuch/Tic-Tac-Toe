using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Tic_Tac_Toe.DbContext;
using System.Threading;

namespace Tic_Tac_Toe.Patterns.RepositoryPattern
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

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
