/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ImportOrdersLogMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/14 9:19:20 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ImportOrdersLogMgr:IImportOrdersLogImplMgr
    {
        private IImportOrdersLogImplDao _importOrdersLogDao;
        public ImportOrdersLogMgr(string connectionStr)
        {
            _importOrdersLogDao = new ImportOrdersLogDao(connectionStr);
        }

        public int Save(ImportOrdersLog importOrdersLog)
        {
            try
            {
                return _importOrdersLogDao.Save(importOrdersLog);
            }
            catch (Exception ex)
            {
                throw new Exception("ImportOrdersLogMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
            
        }
    }
}
