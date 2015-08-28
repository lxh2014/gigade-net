/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderMasterPatternDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/02/26
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
    public class OrderMasterPatternDao : IOrderMasterPatternImplDao
    {
        private IDBAccess _dbAccess;
        public OrderMasterPatternDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region 公關單與報廢單功能  + Save(OrderMasterPattern op)
        /// <summary>
        /// 保存 公關單與報廢單 中 新增加 的數據   add by zhuoqin0830w  2015/02/26
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public string Save(OrderMasterPattern op)
        {
            StringBuilder sql = new StringBuilder("INSERT INTO order_master_pattern(order_id,pattern,dep) VALUES({0}");
            return sql.AppendFormat(",{0},{1})", op.Pattern, op.Dep).ToString();
        }
        #endregion
    }
}