using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class IpoNvdLogMgr
    {
        private IpoNvdLogDao _IpoNvdLogDao;

       public IpoNvdLogMgr(string connectionString)
        {
            _IpoNvdLogDao = new IpoNvdLogDao(connectionString);
        }
       public List<IpoNvdLogQuery> GetIpoNvdLogList(IpoNvdLogQuery query, out int totalCount)
        {
            try
            {
                return _IpoNvdLogDao.GetIpoNvdLogList(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("IpoNvdLogMgr-->GetIpoNvdLogList-->" + ex.Message, ex);
            }
        }
       public bool GetInfoByItemId(uint itemId)
       {
           try
           {
               return _IpoNvdLogDao.GetInfoByItemId(itemId);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoNvdLogMgr-->GetInfoByItemId-->" + ex.Message, ex);
           }
       }
    }
}
