using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Tic_Tac_Toe.Patterns.RepositoryPattern
{
    public interface IUnitOfWork : IDisposable
    {
        IPlayerRepository Players { get; }
        IGameResultRepository GameResults { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
