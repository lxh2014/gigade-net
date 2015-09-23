using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ScheduleServiceMgr
    {
        private ScheduleServiceDao _secheduleServiceDao;
        public ScheduleServiceMgr(string connectionString)
        {
            try
            {
                _secheduleServiceDao = new ScheduleServiceDao(connectionString);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->SecheduleServiceMgr-->" + ex.Message, ex);
            }

        }
        public List<ScheduleServiceQuery> GetExeScheduleServiceList(ScheduleServiceQuery query)
        {
            try
            {
                return _secheduleServiceDao.GetExeScheduleServiceList(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetExeScheduleServiceList-->" + ex.Message, ex);
            }
        }
    }
}
