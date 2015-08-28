using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class SmsLogMgr:ISmsLogImplMgr
    {
         private ISmsLogImplDao _ISmsLogDao;

         public SmsLogMgr(string connectionString)
        {
            _ISmsLogDao = new SmsLogDao(connectionString);
        }

         public List<SmsLogQuery> GetSmsLog(SmsLogQuery slog, out int totalCount)
        {
            try
{
                return _ISmsLogDao.GetSmsLog(slog,out totalCount);
            }
            catch (Exception ex)
    {

                throw new Exception("SmsLogMgr-->GetSmsLog-->" + ex.Message, ex);
            }
        }
    }
}
