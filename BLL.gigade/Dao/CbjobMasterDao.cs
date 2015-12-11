using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
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

        public List<CbjobMasterQuery> GetjobMaster(CbjobMasterQuery m, out int totalCount)
        {
            StringBuilder sbclumn=new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            totalCount = 0;
            try
            {
                sbclumn.Append(" select cm.row_id,cm.cbjob_id,cm.create_datetime,cm.status,cm.create_user,cm.sta_id ");
                sqlCondi.Append(" from cbjob_master cm where 1=1 ");
                if (!string.IsNullOrEmpty(m.startDate))
                {
                    sqlCondi.AppendFormat(" and cm.create_datetime>='{0}' ", m.startDate);
                }
                if (!string.IsNullOrEmpty(m.endDate))
                {
                    sqlCondi.AppendFormat(" and cm.create_datetime<='{0}' ", m.endDate);
                }
                if (!string.IsNullOrEmpty(m.cbjob_id))
                {
                    sqlCondi.AppendFormat(" and cm.cbjob_id like'{0}' ", m.cbjob_id);
                }
                if (!string.IsNullOrEmpty(m.sta_id))
                {
                    sqlCondi.AppendFormat(" and cm.sta_id = '{0}' ", m.sta_id);
                }
                if (!string.IsNullOrEmpty(m.row_id_IN))
                {
                    sqlCondi.AppendFormat(" and cm.row_id in ({0}) ", m.row_id_IN);
                }
                if (m.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable("select count(cm.row_id) as totalCount " + sqlCondi.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", m.Start, m.Limit);
                }
                return _accessMySql.getDataTableForObj<CbjobMasterQuery>(sbclumn .ToString()+ sqlCondi.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobMasterDao-->GetjobMaster-->" + ex.Message + sbclumn.ToString() + sqlCondi.ToString(), ex);
            }
           
        }
    }
}
