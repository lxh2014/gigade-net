using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class SiteStatisticsMgr
    {
        private SiteStatisticsDao ssDao;
        public SiteStatisticsMgr(string connectionString)
        {
            ssDao = new SiteStatisticsDao(connectionString);
        }
        public DataTable GetSiteStatisticsList(SiteStatistics query, out int totalCount)
        {
            try
            {
                return ssDao.GetSiteStatisticsList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->GetSiteStatisticsList" + ex.Message, ex);
            }
        }
        public int Insert(SiteStatistics ss)
        {
            try
            {
                return ssDao.Insert(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Insert" + ex.Message, ex);
            }
        }
        public int Update(SiteStatistics ss)
        {
            try
            {
                return ssDao.Update(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Update" + ex.Message, ex);
            }
        }
        public int Delete(SiteStatistics ss)
        {
            try
            {
                return ssDao.Delete(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Delete" + ex.Message, ex);
            }
        }
    }
}
