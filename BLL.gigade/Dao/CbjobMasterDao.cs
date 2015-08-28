using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CbjobMasterDao:ICbjobMasterImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public CbjobMasterDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        #region +插入數據 int Insert(CbjobMaster m)
        /// <summary>
        /// 插入數據
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int Insert(CbjobMaster m)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"insert into cbjob_master (cbjob_id,create_datetime,create_user,status)  
 values('{0}','{1}','{2}','{3}' );",m.cbjob_id, Common.CommonFunction.DateTimeToString(m.create_datetime), m.create_user, 1);
                return _accessMySql.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobMasterDao-->Insert-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        public string Insertsql(CbjobMaster m)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"insert into cbjob_master (cbjob_id,create_datetime,create_user,status)  
 values('{0}','{1}','{2}','{3}');", m.cbjob_id, Common.CommonFunction.DateTimeToString(m.create_datetime), m.create_user, 1);
                return sbSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobMasterDao-->Insertsql-->" + ex.Message + sbSql.ToString(), ex);
            }
        } 
        #endregion
    }
}
