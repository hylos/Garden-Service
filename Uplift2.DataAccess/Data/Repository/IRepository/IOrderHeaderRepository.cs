using System;
using System.Collections.Generic;
using System.Text;
using Uplift2.Models;

namespace Uplift2.DataAccess.Data.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void ChangeOrderStatus(int orderHeaderId, string status);
    }
}
