using System;
using System.Collections.Generic;
using System.Text;

namespace Tic_Tac_Toe.RepositoryPattern
{
    public interface IUnitOfWork : IDisposable
    {
        IPlayerRepository Players { get; }
        IGameResultRepository GameResults { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
