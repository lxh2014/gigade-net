using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class DeliveryStorePlaceMgr
    {
        DeliveryStorePlaceDao dao;
        public DeliveryStorePlaceMgr(string connectionStr)
        {
            dao = new DeliveryStorePlaceDao(connectionStr);
        }
        public List<DeliveryStorePlaceQuery> GetDeliveryStorePlaceList(DeliveryStorePlace model, out int total)
        {
            try
            {
                return dao.GetDeliveryStorePlaceList(model,out total);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceMgr-->GetDeliveryStorePlaceList-->" + ex.Message, ex);
            }
        }

        public int UpdateDeliveryStorePlace(DeliveryStorePlace model)
        {
            try
            {
                return dao.UpdateDeliveryStorePlace(model);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceMgr-->UpdateDeliveryStorePlace-->" + ex.Message, ex);
            }
        }

        public int InsertDeliveryStorePlace(DeliveryStorePlace model)
        {
            try
            {
                return dao.InsertDeliveryStorePlace(model);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceMgr-->InsertDeliveryStorePlace-->" + ex.Message, ex);
            }
        }
        public int DeleteDeliveryStorePlace(DeliveryStorePlaceQuery query)
        {
            try
            {
                return dao.DeleteDeliveryStorePlace(query);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceMgr-->DeleteDeliveryStorePlace-->" + ex.Message, ex);
            }
        }

        public int SelectDspName(DeliveryStorePlace model)
        {
            try
            {
                return dao.SelectDspName(model);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceMgr-->SelectDspName-->" + ex.Message, ex);
            }
        }
    }
}
