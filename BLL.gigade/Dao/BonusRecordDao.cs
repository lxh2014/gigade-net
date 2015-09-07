/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：BonusRecordDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/08/25
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
    public class BonusRecordDao : IBonusRecordImplDao
    {
        private IDBAccess _dbAccess;
        public BonusRecordDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region 向 bonus_record 裱中添加 數據  add by zhuoqin0830w 2015/08/25
        /// <summary>
        /// 向 bonus_record 裱中添加 數據  add by zhuoqin0830w 2015/08/24
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public string InsertBonusRecord(BonusRecord br)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                //獲取使用者電腦IP
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                string ip = string.Empty;
                if (addlist.Length > 0)
                {
                    ip = addlist[0].ToString();
                }
                strSql.Append(@"INSERT INTO bonus_record(record_id,order_id,master_id,type_id,record_use,record_note,record_writer,record_createdate,record_updatedate,record_ipfrom)");
                strSql.AppendFormat(@" VALUES({0},", br.record_id);
                strSql.Append(@"'{0}',");
                strSql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", br.master_id, br.type_id, br.record_use, br.record_note, br.record_writer);
                strSql.AppendFormat(@"'{0}','{1}','{2}');", br.record_createdate, br.record_updatedate, ip);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("BonusRecordDao-->InsertBonusRecord(BonusRecord br)-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion
    }
}