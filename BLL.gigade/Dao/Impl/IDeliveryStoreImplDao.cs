using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
    public interface IDeliveryStoreImplDao
    {
        int Save(DeliveryStore store);
        int Update(DeliveryStore store);
        int Delete(int rodId);

        /// <summary>
        /// 店家查詢
        /// </summary>
        /// <param name="store">Store Model</param>
        /// <param name="status">店家狀態</param>
        /// <returns>Store Model List</returns>
        List<DeliveryStoreQuery> Query(DeliveryStore store, out int totalCount);
    }
}
