using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class IlocChangeDetailMgr
    {
        private IlocChangeDetailDao _ilocDao;
        public IlocChangeDetailMgr(string connectionString)
        {
            _ilocDao = new IlocChangeDetailDao(connectionString);
        }

        public List<IlocChangeDetailQuery> GetIlocChangeDetailList(IlocChangeDetailQuery query, out int totalcount)
        {
            try
            {
                return _ilocDao.GetIlocChangeDetailList(query, out totalcount);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->GetIlocChangeDetailList-->" + ex.Message, ex);
            }
        }

        public DataTable GetIlocChangeDetailExcelList(IlocChangeDetailQuery ilocDetailQuery)
        {
            try
            {
                return _ilocDao.GetIlocChangeDetailExcelList(ilocDetailQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->GetIlocChangeDetailExcelList-->" + ex.Message, ex);
            }
        }
        public int UpdateIcdStatus(IlocChangeDetailQuery ilocDetailQuery)
        {
            try
            {
                return _ilocDao.UpdateIcdStatus(ilocDetailQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->UpdateIcdStatus-->" + ex.Message, ex);
            }
        }
    }
}
