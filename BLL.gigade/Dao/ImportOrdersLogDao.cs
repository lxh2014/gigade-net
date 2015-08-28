/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ImportOrdersLogDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/14 9:19:10 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ImportOrdersLogDao:IImportOrdersLogImplDao
    {
        private IDBAccess _dbAccess;
        public ImportOrdersLogDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public int Save(ImportOrdersLog importOrdersLog)
        {
            importOrdersLog.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into import_orders_log(`channel_id`,`tcount`,`success_count`,`file_name`,`import_date`,`exec_name`)values(");
            strSql.AppendFormat("{0},{1},{2},'{3}',", importOrdersLog.Channel_Id, importOrdersLog.TCount, importOrdersLog.Success_Count, importOrdersLog.File_Name);
            strSql.Append(importOrdersLog.Import_Date == DateTime.MinValue ? "null" : "'" + importOrdersLog.Import_Date.ToString("yyyy/MM/dd HH:mm:ss") + "'");
            strSql.AppendFormat(",'{0}')", importOrdersLog.Exec_Name);
            return _dbAccess.execCommand(strSql.ToString());
        }
    }
}
