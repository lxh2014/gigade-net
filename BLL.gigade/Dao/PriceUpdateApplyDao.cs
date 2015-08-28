/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：PriceUpdateApplyDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/6 13:23:41 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class PriceUpdateApplyDao : IPriceUpdateApplyImplDao
    {
        private IDBAccess _dbAccess;
        public PriceUpdateApplyDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public int Save(Model.PriceUpdateApply priceUpdateApply)
        {

            StringBuilder strSql = new StringBuilder("insert into price_update_apply(`price_master_id`,`apply_time`,`apply_user`)values(");
            strSql.AppendFormat("{0},now(),{1});select @@identity;", priceUpdateApply.price_master_id, priceUpdateApply.apply_user);
            System.Data.DataTable _dt= _dbAccess.getDataTable(strSql.ToString());
            if (_dt != null && _dt.Rows.Count > 0)
            {
                return Convert.ToInt32(_dt.Rows[0][0]);
            }
            return -1;
        }
    }
}
