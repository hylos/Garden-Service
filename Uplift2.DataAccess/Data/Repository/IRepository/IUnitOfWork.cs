using System;
using System.Collections.Generic;
using System.Text;

namespace Uplift2.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }

        IFrequencyRepository Frequency{get;}

        IServiceRepository Service { get; }

        IOrderHeaderRepository OrderHeader { get; }

        ISP_Call SP_Call { get; }

        IOrderDetailsRepository OrderDetails { get; }

        IUserRepository User { get; }

        void Save();
    }
}
