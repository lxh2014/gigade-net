using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class EventChinaTrustBagMapMgr
    {
        private EventChinaTrustBagMapDao _chinaTrustDao;
        public EventChinaTrustBagMapMgr(string connectionStr)
        {
            _chinaTrustDao = new EventChinaTrustBagMapDao(connectionStr);
        }

        public List<EventChinaTrustBagMapQuery> GetChinaTrustBagMapList(EventChinaTrustBagMapQuery query, out int totalCount)
        {
            try
            {
                return _chinaTrustDao.GetChinaTrustBagMapList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinaTrustBagMapMgr-->GetChinaTrustBagMapList-->" + ex.Message, ex);
            }
        }

        public int Save(EventChinaTrustBagMapQuery q)
        {
            try
            {
                return _chinaTrustDao.Save(q);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinaTrustBagMapMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(EventChinaTrustBagMapQuery q)
        {
            try
            {
                return _chinaTrustDao.Update(q);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinaTrustBagMapMgr-->Update-->" + ex.Message, ex);
            }
        }
        public int UpdateStatus(EventChinaTrustBagMapQuery query)
        {
            try
            {
                return _chinaTrustDao.UpdateStatus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinaTrustBagMapMgr-->UpdateStatus-->" + ex.Message, ex);
            }
        }

        public DataTable GetMapSort(EventChinaTrustBagMapQuery query)
        {
            try
            {
                return _chinaTrustDao.GetMapSort(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinaTrustBagMapMgr-->GetMapSort-->" + ex.Message, ex);
            }
        }
        public bool IsProductId(string id)
        {
            try
            {
                return _chinaTrustDao.IsProductId(id);
            }
            catch (Exception ex)
            {
                throw new Exception("EventChinaTrustBagMapMgr-->IsProductId-->" + ex.Message, ex);
            }
        }

    }
}
