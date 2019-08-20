using System;
using System.Collections.Generic;
using System.Text;
using Uplift2.DataAccess.Data.Repository.IRepository;

namespace Uplift2.DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Frequency = new FrequencyRepository(_db);
            Service = new ServiceRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            User = new UserRepository(_db);
            SP_Call = new SP_Call(_db);

        }

        //set is set to private, we don't want it to be set outside this 
        public ICategoryRepository Category { get; private set; }

        public IFrequencyRepository Frequency { get; private set; }
        public IServiceRepository Service { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IUserRepository User { get; private set; }

        public ISP_Call SP_Call {get; private set;}

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
