using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
   public class EventPromoAdditionalPriceGroupMgr
    {
       EventPromoAdditionalPriceGroupDao dao;
       public EventPromoAdditionalPriceGroupMgr(string connectionStr)
       {
           dao = new EventPromoAdditionalPriceGroupDao(connectionStr);
       }
       public List<EventPromoAdditionalPriceGroupQuery> GetList(EventPromoAdditionalPriceGroup model)
       {
           try
           {
               return dao.GetList(model);
           }
           catch (Exception ex)
           {
               throw new Exception("EventPromoAdditionalPriceGroupMgr-->GetList-->" + ex.Message, ex);
           }
       }
       public int InsertModel(EventPromoAdditionalPriceGroup model)
       {
           try
           {
               return dao.InsertModel(model);
           }
           catch (Exception ex)
           {
               throw new Exception("EventPromoAdditionalPriceGroupMgr-->InsertModel-->" + ex.Message, ex);
           }
       }

       public int UpdateModel(EventPromoAdditionalPriceGroup model)
       {
           try
           {
               return dao.UpdateModel(model);
           }
           catch (Exception ex)
           {
               throw new Exception("EventPromoAdditionalPriceGroupMgr-->UpdateModel-->" + ex.Message, ex);
           }
       }

       public int DeleteModel(EventPromoAdditionalPriceGroupQuery model)
       {
           try
           {
               return dao.DeleteModel(model);
           }
           catch (Exception ex)
           {
               throw new Exception("EventPromoAdditionalPriceGroupMgr-->DeleteModel-->" + ex.Message, ex);
           }
       }
    }
}
