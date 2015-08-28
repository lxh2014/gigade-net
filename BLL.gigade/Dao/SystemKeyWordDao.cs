/*
* 文件名稱 :SystemKeyWordDao.cs
* 文件功能描述 :系統關鍵字訪問數據庫
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class SystemKeyWordDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public SystemKeyWordDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 獲取系統關鍵字列表
        public List<SphinxKeywordQuery> GetSystemKeyWord(SphinxKeywordQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append(@"SELECT sk.row_id,sk.key_word,sk.flag,sk.kdate,sk.kuser,sk.mddate,sk.moduser ");
                sqlCount.Append(@"SELECT count( key_word) as totalCount ");
                sqlCondi.Append(@"from sphinx_keyword sk where 1=1 ");
                if (!string.IsNullOrEmpty(query.searchKey))
                {
                    sqlCondi.AppendFormat("AND sk.key_word  LIKE N'%{0}%'", query.searchKey);
                }
                if (query.startTime > DateTime.MinValue)
                {
                    sqlCondi.AppendFormat("AND sk.kdate BETWEEN '{0}' and '{1}' ", CommonFunction.DateTimeToString(query.startTime), CommonFunction.DateTimeToString(query.endTime));
                }
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
                sqlCondi.AppendFormat("LIMIT {0},{1}", query.Start, query.Limit);
                sql.Append(sqlCondi.ToString());
                return _accessMySql.getDataTableForObj<SphinxKeywordQuery>(sql.ToString());

            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordDao-->GetSystemKeyWord-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 新增系統關鍵字 + AddSystemKeyWord(SphinxKeywordQuery query)
        public int AddSystemKeyWord(SphinxKeywordQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            StringBuilder sqlCondiValue = new StringBuilder();
            int state = 0;
            try
            {
                sql.Append(@"INSERT INTO ");
                sqlCondi.Append(@" sphinx_keyword ( ");
                sqlCondiValue.AppendFormat(@" VALUES( ");
                if (!string.IsNullOrEmpty(query.key_word))
                {
                    sqlCondi.Append("key_word ,");
                    sqlCondiValue.AppendFormat(@"'{0}',", query.key_word);
                }
                if (!string.IsNullOrEmpty(query.flag))
                {
                    sqlCondi.Append("flag ,");
                    sqlCondiValue.AppendFormat(@"'{0}',", query.flag);
                }
                if (!string.IsNullOrEmpty(query.kuser))
                {
                    sqlCondi.Append("kuser ,");
                    sqlCondiValue.AppendFormat(@"'{0}',", query.kuser);
                }
                if (query.kdate > DateTime.MinValue)
                {
                    sqlCondi.Append("kdate ,");
                    sqlCondiValue.AppendFormat(@"'{0}',", CommonFunction.DateTimeToString(query.kdate));
                }
                if (!string.IsNullOrEmpty(query.moduser))
                {
                    sqlCondi.Append("moduser ,");
                    sqlCondiValue.AppendFormat(@"'{0}',", query.moduser);
                }
                if (query.mddate > DateTime.MinValue)
                {
                    sqlCondi.Append("mddate ,");
                    sqlCondiValue.AppendFormat(@"'{0}',", CommonFunction.DateTimeToString(query.mddate));
                }
                sqlCondiValue.Append(")");
                sqlCondi.Append(")");
                string strSqlCondi = sqlCondi.ToString().Remove(sqlCondi.ToString().LastIndexOf(","), 1);
                string strSqlCondiValue = sqlCondiValue.ToString().Remove(sqlCondiValue.ToString().LastIndexOf(","), 1);
                strSqlCondi += strSqlCondiValue;
                sql.Append(strSqlCondi);
                ///新增失敗返回0
                state = _accessMySql.execCommand(sql.ToString());
                return state;
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordDao-->AddSystemKeyWord-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 修改系統關鍵字 + UpdateSystemKeyWord(SphinxKeywordQuery query)
        public int UpdateSystemKeyWord(SphinxKeywordQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            int state = 0;
            try
            {
                sql.Append(@" set sql_safe_updates = 0;  UPDATE sphinx_keyword");
                sqlCondi.Append(@" SET ");
                if (!string.IsNullOrEmpty(query.key_word))
                {
                    sqlCondi.AppendFormat("key_word='{0}',", query.key_word);
                }
                if (!string.IsNullOrEmpty(query.flag))
                {
                    sqlCondi.AppendFormat("flag='{0}',", query.flag);
                }
                if (!string.IsNullOrEmpty(query.moduser))
                {
                    sqlCondi.AppendFormat("moduser='{0}',", query.moduser);
                }
                if (query.mddate > DateTime.MinValue)
                {
                    sqlCondi.AppendFormat("mddate='{0}',", CommonFunction.DateTimeToString(query.mddate));
                }
                sqlCondi.AppendFormat("where row_id={0};", query.row_id);
                sql.Append(sqlCondi.ToString().Remove(sqlCondi.ToString().LastIndexOf(','), 1));
                sql.Append("set sql_safe_updates = 1;");
                ///新增失敗返回0
                state = _accessMySql.execCommand(sql.ToString());
                return state;
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordDao-->UpdateSystemKeyWord-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 刪除系統關鍵字 + DelSystemKeyWord(SphinxKeywordQuery query)
        public int DelSystemKeyWord(SphinxKeywordQuery query)
        {
            int state = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" DELETE from sphinx_keyword WHERE 1=1");
                if (query.ArrId != null)
                {
                    sqlCondi.Append(" and row_id IN (");
                    foreach (var row_id in query.ArrId)
                    {
                        sqlCondi.AppendFormat("{0},", row_id);
                    }
                    sqlCondi.Append(" )");
                }
                sql.Append(sqlCondi.ToString().Remove(sqlCondi.ToString().LastIndexOf(','), 1));
                int line = _accessMySql.execCommand(sql.ToString());
                if (line >= 1)
                {
                    state = 1;
                }
                else
                    state = 0;
                return state;
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordDao-->DelSystemKeyWord-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 獲取匯出的數據
        public List<SphinxKeywordQuery> GetKeyWordExportList(SphinxKeywordQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@"SELECT sk.key_word,sk.flag");
                sqlCount.Append(@"SELECT count(row_id) as totalCount ");
                sqlCondi.Append(@" from sphinx_keyword sk where 1=1 ");
                if (!string.IsNullOrEmpty(query.searchKey))
                {
                    sqlCondi.AppendFormat("AND sk.key_word  LIKE N'%{0}%'", query.searchKey);
                }
                if (query.startTime > DateTime.MinValue)
                {
                    sqlCondi.AppendFormat("AND sk.kdate BETWEEN '{0}' and '{1}' ", CommonFunction.DateTimeToString(query.startTime), CommonFunction.DateTimeToString(query.endTime));
                }
                sqlCount.Append(sqlCondi.ToString());
                DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                sql.Append(sqlCondi.ToString());
                return _accessMySql.getDataTableForObj<SphinxKeywordQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordDao-->DelSystemKeyWord-->" + ex.Message + sql.ToString(), ex);
            }
        } 
        #endregion

        #region 判斷是否存在此關鍵字
        public int CheckKeyWordExsit(string key_word)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int result = 0;
                sql.AppendFormat(@" SELECT row_id from sphinx_keyword where key_word='{0}';", key_word);
                if (_accessMySql.getDataTable(sql.ToString()).Rows.Count > 0)
                {
                    result = _accessMySql.getDataTable(sql.ToString()).Rows.Count;
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("IplasDao-->YesOrNoLocIdExsit-->" + ex.Message + sql.ToString(), ex);
            }
        } 
        #endregion
    
    }
}
