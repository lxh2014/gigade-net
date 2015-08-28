using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao
{
    public class SecretInfoLogDao
    {
        private IDBAccess _access;

        public SecretInfoLogDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 查詢

        public DataTable GetSecretInfoLog(SecretInfoLog query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbCountSql = new StringBuilder();
            StringBuilder sbSqlCondi = new StringBuilder();

            totalCount = 0;
            sbSql.Append("SELECT count(sl.user_id) as countIp,log_id,sl.user_id,mu.user_email,sl.createdate,ipfrom,url,sl.input_pwd_date ");
            sbSql.Append(",sl.type, sl.related_id,'' as related_name ");

            sbCountSql.Append("SELECT log_id,sl.user_id,mu.user_email,sl.createdate,ipfrom,url,sl.input_pwd_date ");
            sbCountSql.Append(",sl.type, sl.related_id,'' as related_name ");

            sbSqlCondi.Append(" FROM secret_info_log as sl ");
            sbSqlCondi.Append(" LEFT JOIN manage_user as mu ON  mu.user_id=sl.user_id");

            sbSqlCondi.Append(" WHERE 1=1 ");
            if (!string.IsNullOrEmpty(query.user_email))
            {
                sbSqlCondi.AppendFormat(" and mu.user_email like N'%{0}%'", query.user_email);
            }

            if (!string.IsNullOrEmpty(query.ipfrom))
            {
                sbSqlCondi.AppendFormat(" and ipfrom like N'%{0}%' ", query.ipfrom);
            }
            if (query.type != 0)
            {
                sbSqlCondi.AppendFormat(" and sl.type ={0}", query.type);
            }
            if (query.user_id != 0)
            {
                sbSqlCondi.AppendFormat(" and sl.user_id ={0}", query.user_id);
            }
            if (query.type != 0 && query.type != -1)
            {
                sbSqlCondi.AppendFormat(" and sl.type ={0}", query.type);
            }

            if (query.date_one != DateTime.MinValue && query.date_two != DateTime.MinValue)
            {
                sbSqlCondi.AppendFormat(" and sl.createdate between '{0}' and '{1}' ", Common.CommonFunction.DateTimeToString(query.date_one), Common.CommonFunction.DateTimeToString(query.date_two));
            }
            if (query.countClass == 2)
            {
                if (query.date_one != DateTime.MinValue && query.date_two != DateTime.MinValue)
                {
                    sbSqlCondi.AppendFormat(" and sl.input_pwd_date between '{0}' and '{1}' ", Common.CommonFunction.DateTimeToString(query.date_one), Common.CommonFunction.DateTimeToString(query.date_two));
                }
                sbSqlCondi.AppendFormat(" and  NOT ISNULL(sl.input_pwd_date) and sl.input_pwd_date!='0001-01-01 00:00:00'  ");

            }
            if (query.ismail == 0)
            {
                sbSqlCondi.Append(" GROUP BY ipfrom,sl.user_id,type ");
            }
            if (query.ismail == 1)
            {
                sbSqlCondi.Append(" GROUP BY  sl.user_id,type ");
            }
            if (query.ismail == 2)
            {
                sbSqlCondi.Append(" GROUP BY  ipfrom,type ");
            }

            if (query.sumtotal != 0)
            {
                if (query.ismail == 1 || query.ismail == 0)
                {
                    sbSqlCondi.AppendFormat(@" HAVING COUNT(sl.user_id)>= {0} ", query.sumtotal);
                }
                else if (query.ismail == 2)
                {
                    sbSqlCondi.AppendFormat(@" HAVING COUNT(ipfrom)>= {0} ", query.sumtotal);
                }
            }
            sbSqlCondi.Append(" ORDER BY log_id DESC");
            try
            {
                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable("select sl.log_id as totalCount  " + sbSqlCondi.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = dt.Rows.Count;
                    }
                    sbSqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                if (query.ismail != -1 && query.countClass != -1)
                {
                    sql.Append(sbSql.ToString() + " " + sbSqlCondi.ToString());
                }
                else
                {
                    sql.Append(sbCountSql.ToString() + " " + sbSqlCondi.ToString());
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogDao-->GetSecretInfoLog-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public List<SecretInfoLog> GetSecretInfoLog(SecretInfoLog query)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondi = new StringBuilder();
            try
            {
                sbSql.Append("SELECT log_id,sl.user_id,sl.createdate,ipfrom,url,input_pwd_date");
                sbSql.Append(",sl.type,sl.related_id ");
                sbSqlCondi.Append(" FROM secret_info_log as sl ");
                sbSqlCondi.Append(" WHERE 1=1 ");
                if (query.user_id != 0)
                {
                    sbSqlCondi.AppendFormat(" and sl.user_id = '{0}'", query.user_id);
                }
                if (!string.IsNullOrEmpty(query.ipfrom))
                {
                    sbSqlCondi.AppendFormat(" and sl.ipfrom='{0}'", query.ipfrom);
                }
                if (query.date_one != DateTime.MinValue)
                {
                    sbSqlCondi.AppendFormat(" and sl.createdate >= '{0}'", Common.CommonFunction.DateTimeToString(query.date_one));
                }
                if (query.date_two != DateTime.MinValue)
                {
                    sbSqlCondi.AppendFormat(" and  sl.createdate <= '{0}'", Common.CommonFunction.DateTimeToString(query.date_two));
                }
                sbSqlCondi.Append(" order by sl.input_pwd_date desc,sl.log_id desc ");//非常重要,不可更改
                sbSql.Append(sbSqlCondi.ToString());
                return _access.getDataTableForObj<SecretInfoLog>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogDao-->GetSecretInfoLog(SecretInfoLog query)-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        public List<SecretInfoLog> GetMaxCreateLog(SecretInfoLog query)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondi = new StringBuilder();
            try
            {
                sbSql.Append("SELECT log_id,sl.user_id,sl.createdate,ipfrom,url,input_pwd_date");
                sbSql.Append(",sl.type,sl.related_id ");
                sbSqlCondi.Append(" FROM secret_info_log as sl ");
                sbSqlCondi.Append(" WHERE 1=1 ");
                if (query.user_id != 0)
                {
                    sbSqlCondi.AppendFormat(" and sl.user_id = '{0}'", query.user_id);
                }
                if (!string.IsNullOrEmpty(query.ipfrom))
                {
                    sbSqlCondi.AppendFormat(" and sl.ipfrom='{0}'", query.ipfrom);
                }
                sbSqlCondi.Append(" order by sl.createdate desc,sl.log_id desc ");//非常重要,不可更改
                sbSql.Append(sbSqlCondi.ToString());
                return _access.getDataTableForObj<SecretInfoLog>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogDao-->GetSecretInfoLog(SecretInfoLog query)-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion

        #region 插入
        public int InsertSecretInfoLog(SecretInfoLog query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"INSERT INTO secret_info_log(user_id,createdate,ipfrom,url,input_pwd_date,type,related_id )");
            sql.AppendFormat(@" VALUES('{0}','{1}','{2}','{3}','{4}',", query.user_id, Common.CommonFunction.DateTimeToString(query.createdate), query.ipfrom, query.url, Common.CommonFunction.DateTimeToString(query.input_pwd_date));
            sql.AppendFormat(@" '{0}','{1}' );", query.type, query.related_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogDao-->InsertSecretInfoLog" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 更新
        public int UpdateSecretInfoLog(SecretInfoLog query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"set sql_safe_updates = 0; UPDATE secret_info_log SET input_pwd_date='{0}'", Common.CommonFunction.DateTimeToString(query.input_pwd_date));
            sql.AppendFormat(@" WHERE log_id ='{0}'; set sql_safe_updates = 1;", query.log_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogDao-->UpdateSecretInfoLog" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
