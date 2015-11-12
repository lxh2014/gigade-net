using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class DeliverChangeLogMgr : IDeliverChangeLogImplMgr
    {
        private IDeliverChangeLogImplDao _IDeliverChangeLogDao;
        public DeliverChangeLogMgr(string connectionString)
        {
            _IDeliverChangeLogDao = new DeliverChangeLogDao(connectionString);
        }
        public int insertDeliverChangeLog(DeliverChangeLog dCL)
        {
            try
            {
                return _IDeliverChangeLogDao.insertDeliverChangeLog(dCL);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogMgr-->insertDeliverChangeLog-->" + ex.Message, ex);
            }
        } 
    }
}
