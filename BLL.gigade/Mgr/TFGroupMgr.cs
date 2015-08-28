using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class TFGroupMgr : ITFGroupImplMgr
    {
         private ITFGroupImplDao _tfgroupDao;
         public TFGroupMgr(string connectionstring)
        {
            _tfgroupDao = new TFGroupDao(connectionstring);
        }
         public List<TFGroup> QueryAll(TFGroup m)
        {
            try
            {
                return _tfgroupDao.QueryAll(m);
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Add-->" + ex.Message, ex);
            }
        }
    }
}
