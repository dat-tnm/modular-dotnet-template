using CompanyName.ProjectName.Shared.UnitOfWork.Contracts;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Implements
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbConnection? _connection;
        private IDbTransaction? _transaction;
        private bool _disposed;

        public DapperUnitOfWork(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = _connectionFactory.CreateConnection();
                }

                if (_connection.State != ConnectionState.Open)
                {
                    if (_connection is SqlConnection sqlConnection)
                    {
                        sqlConnection.OpenAsync();
                    }
                    else
                    {
                        _connection.Open();
                    }
                }

                return _connection;
            }
        }

        public IDbTransaction? Transaction => _transaction;

        public async Task BeginAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Transaction already started.");
            }

            _transaction = Connection.BeginTransaction();
        }

        public Task CommitAsync(CancellationToken ct = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            try
            {
                _transaction.Commit();
                return Task.CompletedTask;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to rollback.");
            }
            try
            {
                _transaction?.Rollback();
                return Task.CompletedTask;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
