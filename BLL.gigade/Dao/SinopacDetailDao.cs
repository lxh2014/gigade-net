using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class SinopacDetailDao : ISinopacDetailImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public SinopacDetailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        /// <summary>
        /// 供應商出貨單中的訂單轉單日期
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public DataTable GetSinopacDetai(SinopacDetail store,string sql)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" select  order_id,FROM_UNIXTIME(sinopac_createdate) as pay_time  from sinopac_detail where 1=1 ");
                if (!string.IsNullOrEmpty(sql))
                {
                    sb.Append(sql);
                }
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SinopacDetail->GetSinopacDetai->"+ex.Message+sb.ToString() ,ex);
            }
          
        }
    }
}
