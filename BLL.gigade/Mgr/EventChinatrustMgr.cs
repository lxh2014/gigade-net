using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
     public class EventChinatrustMgr
    {
        private  EventChinatrustDao _EventChinatrust;
        private MySqlDao _mysqlDao;
        public EventChinatrustMgr(string connectionStr)
        {
            _EventChinatrust = new EventChinatrustDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
        }
        public List<EventChinatrustQuery> GetEventChinatrustList(EventChinatrustQuery query, out int totalCount)
        {
            try
            {
                return _EventChinatrust.GetEventChinatrustList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinatrustMgr-->GetEventChinatrustList-->" + ex.Message, ex);
            }
        }
        public int AddEventChinatrust(EventChinatrustQuery model)
        {
            try
            {
                return _EventChinatrust.AddEventChinatrust(model);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinatrustMgr-->AddEventChinatrust-->" + ex.Message, ex);
            }
        }
        public int UpdateEventChinatrust(EventChinatrustQuery model)
        {
            try
            {
                return _EventChinatrust.UpdateEventChinatrust(model);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinatrustMgr-->UpdateEventChinatrust-->" + ex.Message, ex);
            }
        }
        public int UpEventChinatrustStatus(EventChinatrustQuery model)
        {
            try
            {
                return _EventChinatrust.UpEventChinatrustStatus(model);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinatrustMgr-->UpEventChinatrustStatus-->" + ex.Message, ex);
            }
        }
        public List<EventChinatrustQuery> GetChinaTrustStore()
        {
            try
            {
                return _EventChinatrust.GetChinaTrustStore();
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinatrustMgr-->GetChinaTrustStore-->" + ex.Message, ex);
            }
        }
    }
}
