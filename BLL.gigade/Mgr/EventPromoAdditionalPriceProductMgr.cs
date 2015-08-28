using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class EventPromoAdditionalPriceProductMgr
    {
        EventPromoAdditionalPriceProductDao dao;

        public EventPromoAdditionalPriceProductMgr(string conncetionStr)
        {
            dao = new EventPromoAdditionalPriceProductDao(conncetionStr);
        }

        public List<EventPromoAdditionalPriceProductQuery> GetList(EventPromoAdditionalPriceProductQuery model)
        {
            try
            {
                return dao.GetList(model);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductMgr-->GetList-->" + ex.Message, ex);
            }
        }
        public List<EventPromoAdditionalPriceGroup> GetPromoProductGroup(EventPromoAdditionalPriceGroup model)
        {
            try
            {
                return dao.GetPromoProductGroup(model);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductMgr-->GetPromoProductGroup-->" + ex.Message, ex);
            }
        }

        public string AddOrUpdate(EventPromoAdditionalPriceProduct model)
        {
            try
            {

                return dao.AddOrUpdate(model);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductMgr-->AddOrUpdate-->" + ex.Message, ex);
            }
        }


        public string Delete(string group_id, string row_ids)
        {
            try
            {
                return dao.Delete(group_id, row_ids);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductMgr-->Delete" + ex.Message, ex);
            }
        }
    }
}
