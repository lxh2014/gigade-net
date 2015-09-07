using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class EdmEmailDao
    {
        private IDBAccess _access;
        public EdmEmailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 從edm_email查詢數據 + void GetData(string email_address, int id, out int largestId, out uint email_id, out string email_name)
        public void GetData(string email_address, out uint largestId, out uint email_id, out string email_name)
        {
            StringBuilder sqlLargestID = new StringBuilder();
            StringBuilder sqlSearchIDByAddress = new StringBuilder();
            StringBuilder sqlSearchNameByID = new StringBuilder();
            DataTable dt = new DataTable();
            largestId = 0;
            email_id = 0;
            email_name = string.Empty;
            try
            {
                sqlLargestID.AppendFormat(@"SELECT email_id from edm_email ORDER BY email_id DESC limit 0,1");
                dt = _access.getDataTable(sqlLargestID.ToString());
                if (dt.Rows.Count > 0)
                {
                    largestId = Convert.ToUInt32(dt.Rows[0][0]);
                    dt.Clear();
                }
                sqlSearchIDByAddress.AppendFormat(@"SELECT email_id FROM edm_email WHERE email_address='{0}'", email_address);
                dt = _access.getDataTable(sqlSearchIDByAddress.ToString());
                if (dt.Rows.Count > 0)
                {
                    email_id = Convert.ToUInt32(dt.Rows[0][0]);
                    dt.Clear();
                }
                sqlSearchNameByID.AppendFormat(@"SELECT email_name from edm_email WHERE email_id={0} ", email_id);
                dt = _access.getDataTable(sqlSearchNameByID.ToString());
                if (dt.Rows.Count > 0)
                {
                    email_name = dt.Rows[0][0].ToString();
                    dt.Clear();
                }
            }
            catch (Exception ex)
            {

                throw new Exception("EdmEmailDao-->GetEdmTestList-->" + ex.Message + sqlLargestID.ToString() + sqlSearchIDByAddress.ToString() + sqlSearchNameByID.ToString(), ex);
            }
        }
        #endregion
       
        public EdmEmail GetModel(string mail)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT ee.email_id from edm_email ee LEFT JOIN edm_group_email ege on ee.email_id=ege.email_id WHERE email_address ='{0}'; ", mail);
                return _access.getSinggleObj<EdmEmail>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmEmailDao-->ExistsEmail-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        public DataTable GetEmailByID(uint id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT email_address ,email_name from edm_email WHERE email_id={0}", id);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmEmailDao-->GetEmailByID-->" + sql.ToString() + ex.Message, ex); 
            }
        }
        #region 電子報人員名單列表
        public DataTable GetEdmPersonList(EdmEmailQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            totalCount = 0;
            DataTable dt = new DataTable();
            try
            {
                sql.AppendFormat(@"SELECT  DISTINCT ee.email_id,ee.email_address,ee.email_name ,ege.email_status,count(ee.email_id) as group_count 
FROM edm_email ee INNER  JOIN edm_group_email ege ON ee.email_id=ege.email_id  WHERE 1=1 ");
                count = sql;
                if (query.email_name != string.Empty)
                {
                    sql.AppendFormat(" AND ee.email_name LIKE N'%{0}%'", query.email_name);
                    count.AppendFormat(" AND ee.email_name LIKE N'%{0}%'", query.email_name);
                }
                if (query.email_address != string.Empty)
                {
                    sql.AppendFormat(" AND ee.email_address LIKE N'%{0}%'", query.email_address);
                    count.AppendFormat(" AND ee.email_address LIKE N'%{0}%'", query.email_address);
                }
                if (query.email_id != 0)
                {
                    sql.AppendFormat(" AND ee.email_id ={0}", query.email_id);
                    count.AppendFormat(" AND ee.email_id={0}", query.email_id);
                }
                sql.AppendFormat(" GROUP BY(ee.email_id)");
                if (query.IsPage)
                {
                    dt = _access.getDataTable(count.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", query.Start, query.Limit);

                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmEmailDao-->GetEdmPersonList-->" + sql.ToString() + ex.Message, ex);
            }
        } 
        #endregion

        #region 人員名單
        public DataTable GetPersonList(EdmEmailQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            totalCount = 0;
            DataTable dt = new DataTable();
            try
            {
                sql.AppendFormat(@"SELECT DISTINCT ege.group_id,eg.group_name,ege.email_id,ee.email_address,ee.email_name,email_status from edm_email ee INNER JOIN edm_group_email ege on ee.email_id=ege.email_id INNER JOIN edm_group eg on eg.group_id=ege.group_id WHERE  ee.email_id={0} ", query.email_id);
                count = sql;
                if (query.group_id != 0)
                {
                    sql.AppendFormat(" AND ege.group_id={0}", query.group_id);
                    count.AppendFormat(" AND ege.group_id={0}", query.group_id);
                }
                if (query.group_name != string.Empty)
                {
                    sql.AppendFormat(" AND eg.group_name LIKE N'%{0}%'", query.group_name);
                    count.AppendFormat(" AND eg.group_name LIKE N'%{0}%'", query.group_name);
                }
                if (query.IsPage)
                {
                    dt = _access.getDataTable(count.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmEmailDao-->GetPersonList-->" + sql.ToString() + ex.Message, ex);
            } 
        #endregion
        }
    }
}
