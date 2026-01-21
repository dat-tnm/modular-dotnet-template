using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }

        Task BeginAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
