/*
* 文件名稱 :DeliverChangeLogDao.cs
* 文件功能描述 :出貨管理--出貨單期望到貨日
* 版權宣告 :
* 開發人員 : zhaozhi0623j
* 版本資訊 : 1.0
* 日期 : 2015-11-12
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
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
        public List<DeliverChangeLogQuery> GetDeliverChangeLogList(DeliverChangeLogQuery Query, out int totalCount)
        {
            try
            {
                return _IDeliverChangeLogDao.GetDeliverChangeLogList(Query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogMgr-->GetDeliverChangeLogList-->" + ex.Message, ex);
            }
        }
    }
}
