using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class EventpromoUserConditionMgr : IEventpromoUserConditionImplMgr
    {
        private IEventPromoUserConditionImplDao _ieuCondition;
        public EventpromoUserConditionMgr(string connectionStr)
        {
            _ieuCondition = new EventpromoUserConditionDao(connectionStr);

        }
        public DataTable GetList(Model.Query.EventPromoUserConditionQuery epQuery, out int totalCount)
        {
            try
            {
                return _ieuCondition.GetList(epQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoUserConditionMgr-->GetList--" + ex.Message, ex);
            }
        }


        public string GetEventCondi(Model.Query.EventPromoUserConditionQuery epQuery)
        {
            try
            {
                DataTable _dt = _ieuCondition.GetEventCondi(epQuery);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,data:["));
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in _dt.Rows)
                    {
                        stb.Append("{");
                        stb.Append(string.Format("\"condi_id\":\"{0}\",\"condi_name\":\"{1}\"", item["condition_id"].ToString(), item["condition_name"].ToString()));
                        stb.Append("}");
                    }
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");

            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoUserConditionMgr-->GetEventCondi--" + ex.Message, ex);
            }
        }

        public int AddOrUpdate(Model.Query.EventPromoUserConditionQuery epQuery)
        {
            try
            {
                return _ieuCondition.AddOrUpdate(epQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoUserConditionMgr-->AddOrUpdate--" + ex.Message, ex);
            }
        }

        public int Delete(Model.Query.EventPromoUserConditionQuery epQuery)
        {
            try
            {
                return _ieuCondition.Delete(epQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoUserConditionMgr-->Delete--" + ex.Message, ex);
            }
        }
    }
}
