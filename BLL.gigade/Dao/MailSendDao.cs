using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class MailSendDao
    {
        IDBAccess _accessMySql;
        string connStr = string.Empty;
        public MailSendDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 獲取數據
        /// <summary>
        /// 獲取數據
        /// </summary>
        /// <param name="sqltText">執行語句</param>
        /// <returns>返回數據表</returns>
        public List<MailSendQuery> GetData(MailSendQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            try
            {
                totalCount = 0;
                ///獲得數據sql 
                sql.AppendFormat("SELECT rowid, mailfrom, mailto, subject, mailbody,status,kuser, kdate, senddate,source,weight");
                ///查詢總數sql
                sqlCount.Append(" select count(rowid) as totalCount ");
                ///條件限制aql
                sqlCondi.Append(" from mail_send  where 1=1 ");
                if (!string.IsNullOrEmpty(query.startT)&&!string.IsNullOrEmpty(query.endT))
                {

                    sqlCondi.AppendFormat(" AND kdate BETWEEN '{0}' AND '{1}' ",Convert.ToDateTime(query.startT).ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(query.endT).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(query.search))
                {
                    sqlCondi.AppendFormat(" AND ( mailto LIKE N'%{0}%' or subject LIKE N'%{1}%') ",query.search,query.search);
                }
                ///是否分頁
                if (query.IsPage)
                {
                    sqlCount.Append(sqlCondi.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        //得到滿足條件的總行數
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                ///得到應分頁的數據
                sqlCondi.AppendFormat(" ORDER BY rowid ASC LIMIT {0},{1} ;", query.Start, query.Limit);
                List<MailSendQuery> listStore = _accessMySql.getDataTableForObj<MailSendQuery>(sql.Append(sqlCondi).ToString());
               return listStore;
            }
            catch (Exception ex)
            {
                throw new Exception("MailSendDao.GetData-->" + ex.Message+sql.ToString());
            }
        }
        #endregion

    }
}
