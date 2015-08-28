using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class EdmTestDao
    {
        private IDBAccess _access;
        public EdmTestDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public DataTable GetEdmTestList(EdmTestQuery query, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            DataTable store = new DataTable();
            totalCount = 0;
            try
            {
                strSql.AppendFormat(@"SELECT et.email_id,et.test_username,ee.email_address,et.test_status,FROM_UNIXTIME(et.test_createdate) as test_createdate,FROM_UNIXTIME(et.test_updatedate) as test_updatedate FROM edm_email ee,edm_test et WHERE et.email_id = ee.email_id");
                if (query.selectType == "0" && !string.IsNullOrEmpty(query.search_con))
                {
                    sqlWhere.AppendFormat(" and ee.email_address like '%{0}%'", query.search_con.ToString());
                }
                if (query.selectType == "1" && !string.IsNullOrEmpty(query.search_con))
                {
                    sqlWhere.AppendFormat(" and et.test_username like '%{0}%'", query.search_con.ToString());
                }
                if (query.dateCon == "1")//建立時間
                {
                    if (query.date_start!=0)
                    {
                        sqlWhere.AppendFormat(" and et.test_createdate > '{0}' ", query.date_start);
                    }
                    if (query.date_end != 0)
                    {
                        sqlWhere.AppendFormat(" and et.test_createdate<'{0}' ", query.date_end);
                    }
                }
                else if (query.dateCon == "2")
                {
                    if (query.date_start != 0)
                    {
                        sqlWhere.AppendFormat(" and et.test_updatedate > '{0}' ", query.date_start);
                    }
                    if (query.date_end != 0)
                    {
                        sqlWhere.AppendFormat(" and et.test_updatedate<'{0}' ", query.date_end);
                    }
                }
                if (query.test_status!=0)
                {
                    sqlWhere.AppendFormat(" and et.test_status='{0}' ", query.test_status);
                }

                if (query.email_id != 0)
                {
                    sqlWhere.AppendFormat(" and et.email_id = '{0}'", query.email_id);
                }
                count.AppendFormat(@"SELECT	count(et.email_id)as totalCount FROM edm_email ee,edm_test et WHERE ee.email_id=et.email_id ");
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(count.ToString()+sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    strSql.AppendFormat("{0} limit {1},{2}",sqlWhere.ToString(), query.Start, query.Limit);
                }
                store = _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTesDao-->GetEdmTestList-->" + ex.Message + strSql.ToString() + count.ToString(), ex);
            }
            return store;
        }

        public int DeleteEdmTest(int id)
        {
            int i = 0;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"DELETE FROM edm_test WHERE email_id={0} ", id);
                i = _access.execCommand(strSql.ToString());
                return i;
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTesDao-->DeleteEdmTest-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #region 新增测试名单
        public string InsertEdmTest(EdmTestQuery query)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"INSERT INTO edm_test(email_id,test_username,test_status,test_createdate,test_updatedate) VALUES({0},'{1}',{2},'{3}','{4}')", query.email_id, query.test_username, query.test_status, query.test_createdate, query.test_updatedate);
            return strSql.ToString();
        }
        public string InsertEdmEmail(EdmTestQuery query)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"INSERT INTO edm_email(email_id,email_name,email_address,email_createdate,email_updatedate) VALUES({0},'{1}','{2}','{3}','{4}')", query.email_id, query.test_username, query.email_address, query.test_createdate, query.test_updatedate);
            return strSql.ToString();
        }
        #endregion

        #region 修改测试名单
        public string UpdateEdmEmailName(EdmTestQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"UPDATE edm_email SET email_name='{0}' WHERE email_id={1};", query.test_username, query.email_id);
            strSql.AppendFormat(@"UPDATE edm_test SET test_username='{0}' WHERE email_id={1}", query.test_username, query.email_id);
            return strSql.ToString();
        }
        public string UpdateEdmEmailAddress(EdmTestQuery query)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"UPDATE edm_email SET email_address='{0}' WHERE email_id={1}", query.email_address, query.email_id);
            return strSql.ToString();
        }
        public string UpdateEdmTest(EdmTestQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"UPDATE edm_test SET test_username='{0}',test_status={1},test_updatedate='{2}' WHERE email_id={3}; ", query.test_username, query.test_status, query.test_updatedate, query.email_id);
            return strSql.ToString();
        }
        public bool SelectExists(uint id)
        {
            StringBuilder sql = new StringBuilder();
            bool result = false;
            try
            {
                sql.AppendFormat(@"SELECT email_id,test_username,test_status from edm_test  WHERE email_id={0}", id);
               DataTable dt= _access.getDataTable(sql.ToString());
               if (dt.Rows.Count > 0 && dt != null)
               {
                   result = true;
               }
               return result;
            }
            catch (Exception ex)
            {
                
                throw new Exception("EdmTesDao-->DeleteEdmTest-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
