/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：HistoryBatchMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/2/6 9:40:36 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class HistoryBatchMgr : IHistoryBatchImplMgr
    {
        private IHistoryBatchImplDao _historyBatchDao;
        public HistoryBatchMgr(string connectionStr)
        {
            _historyBatchDao = new HistoryBatchDao(connectionStr);
        }

        #region IHistoryBatchImplMgr 成员

        public string Save(Model.HistoryBatch historyBatch)
        {
            return _historyBatchDao.Save(historyBatch);
        }

        public bool BatchExists(Model.HistoryBatch historyBatch)
        {
            return _historyBatchDao.BatchExists(historyBatch);
        }

        public List<Model.HistoryBatch> QueryToday(int itemType)
        {
            return _historyBatchDao.QueryToday(itemType);
        }

        public Model.HistoryBatch Query(Model.HistoryBatch historyBatch)
        {
            return _historyBatchDao.Query(historyBatch);
        }

        #endregion
    }
}
