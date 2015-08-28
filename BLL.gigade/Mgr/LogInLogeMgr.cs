using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class LogInLogeMgr:ILogInLogeImplMgr
    {
        private Dao.Impl.ILogInLogeImplDao _logInLogeDao;
        public LogInLogeMgr(string connectionString)
        {
            _logInLogeDao = new Dao.LogInLogeDao(connectionString);
        }
        public List<Model.Query.LogInLogeQuery> QueryList(LogInLogeQuery logInLogeQuery, out int totalCount)
        {
            try
            {
                return _logInLogeDao.QueryList(logInLogeQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("LogInLogeMgr-->QueryList-->" + ex.Message, ex);
            }
            
        }
    }
}
