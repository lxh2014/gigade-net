using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class EdmSendMgr
    {
        public EdmSendDao _edmSendDao;
        public EdmSendMgr(string connctionString)
        {
            _edmSendDao = new EdmSendDao(connctionString);
        }
        public List<EdmSendQuery> GetStatisticsEdmSend(EdmSendQuery query, out int totalCount)
        {
            try
            {
                return _edmSendDao.GetStatisticsEdmSend(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendMgr-->GetStatisticsEdmSend-->" + ex.Message);
            }
        }
        public List<EdmListQuery> GetStatisticsEdmList(EdmListQuery query, out int totalCount)
        {
            try
            {
                return _edmSendDao.GetStatisticsEdmList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendMgr-->GetStatisticsEdmList-->" + ex.Message);
            }
        }
        public DataTable EdmSendExportCSV(EdmSendQuery query)
        {
            try
            {
                return _edmSendDao.EdmSendExportCSV(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendMgr-->EdmSendExportCSV-->" + ex.Message);
            }
        }
        public int GetMaxOpen(EdmSendQuery query)
        {
            try
            {
                return _edmSendDao.GetMaxOpen(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendMgr-->GetMaxOpen-->" + ex.Message);
            }
        }
        public int GetMaxClick(EdmListQuery query,out int sum_total_click, out int sum_total_person)
        {
            try
            {
                return _edmSendDao.GetMaxClick(query,out sum_total_click, out sum_total_person);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendMgr-->GetMaxClick-->" + ex.Message);
            }
        }
        public EdmSendQuery EdmSendLoad(EdmSendQuery query)
        {
            try
            {
                return _edmSendDao.EdmSendLoad(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendMgr-->EdmSendLoad-->" + ex.Message);
            }
        }
        public DataTable GetSendRecordList(EdmSendQuery query,out int totalCount)
        {
            totalCount = 0;
            try
            {
                return _edmSendDao.GetSendRecordList(query,out totalCount);
            }
            catch (Exception ex)
            {
                
               throw new Exception("EdmSendMgr-->GetSendRecordList-->" + ex.Message);
            }
        }
    }
}
