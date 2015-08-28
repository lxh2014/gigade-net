using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBAccess;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class DisableKeywordsDao
    {
        private string connectStr;
        private DBAccess.IDBAccess _dbAccess;
        public DisableKeywordsDao(string connectStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectStr);
            this.connectStr = connectStr;
        }
        public DataTable GetKeyWordsList(DisableKeywordsQuery query,out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            DataTable store = new DataTable();
            totalCount = 0;
            try
            {
                strSql.AppendFormat(@"SELECT dk_id,dk_string,u.user_username as user_name,dk_created,dk_modified,dk_active from disable_keywords dk,manage_user u WHERE u.user_id = dk.user_id");
                count.AppendFormat(@"select count(dk_id) as totalCount from disable_keywords WHERE 1=1");
                if (query.end != DateTime.MinValue && query.start != DateTime.MinValue)
                {
                    strSql.AppendFormat(" AND dk_created BETWEEN '{0}' AND '{1}'", Common.CommonFunction.DateTimeToString(query.start), Common.CommonFunction.DateTimeToString(query.end));
                    count.AppendFormat("  AND dk_created BETWEEN '{0}' AND '{1}'", Common.CommonFunction.DateTimeToString(query.start), Common.CommonFunction.DateTimeToString(query.end));
                }
                if (query.search_text != "")
                {
                    strSql.AppendFormat(" AND dk_string LIKE N'%{0}%'", query.search_text);
                    count.AppendFormat(" AND dk_string LIKE N'%{0}%'", query.search_text);
                }
                if (query.IsPage)
                {
                    DataTable dt = _dbAccess.getDataTable(count.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0][0]);
                    }
                }
                strSql.AppendFormat(" order by dk_id desc limit {0},{1}", query.Start, query.Limit);
                store = _dbAccess.getDataTable(strSql.ToString());
                return store;
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->GetKeyWordsList-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string AddKeyWords(DisableKeywordsQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                strSql.AppendFormat(@"INSERT INTO disable_keywords(dk_string,user_id,dk_created) VALUES ('{0}','{1}',NOW());", query.dk_string, query.user_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->AddKeyWords-->" + ex.Message + strSql.ToString(), ex);
            }
        }


        public string GetCount(string keyword)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"select dk_id from disable_keywords where dk_string='{0}';",keyword);
                return strSql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->GetCount-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string UpdateKeyWords(DisableKeywordsQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"UPDATE disable_keywords SET dk_string='{0}' WHERE dk_id='{1}'", query.dk_string, query.dk_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->UpdateKeyWords-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string DeleteKeyWords(string id)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"DELETE from disable_keywords WHERE dk_id={0}", id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->DeleteKeyWords-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string UpdateStatus(DisableKeywordsQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"Update disable_keywords set dk_active='{1}' WHERE dk_id={0}", query.dk_id, query.dk_active);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("DisableKeywordsDao-->UpdateStatus-->" + ex.Message + strSql.ToString(), ex);
            }
        }
    }
}
