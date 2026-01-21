using CompanyName.ProjectName.Shared.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Implements
{
    public abstract class RepositoryBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected IDbConnection Connection => _unitOfWork.Connection;
        protected IDbTransaction? Transaction => _unitOfWork.Transaction;
    }
}
