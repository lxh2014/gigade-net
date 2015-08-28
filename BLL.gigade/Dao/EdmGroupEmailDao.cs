using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class EdmGroupEmailDao:IEdmGroupEmailImpIDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public EdmGroupEmailDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        public List<Model.Query.EdmGroupEmailQuery> GetEdmGroupEmailList(Model.Query.EdmGroupEmailQuery query, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            StringBuilder sbcount = new StringBuilder();
            totalCount = 0;
            try
            {
                sb.Append("SELECT ege.group_id,eg.group_name,ee.email_address,ege.email_id,ee.email_name,ege.email_status,ege.email_createdate,ege.email_updatedate ");
                sbwhere.Append(" FROM edm_group_email ege ");
                sbwhere.Append(" LEFT JOIN edm_email ee ON ee.email_id=ege.email_id ");
                sbwhere.Append(" LEFT JOIN edm_group eg ON eg.group_id=ege.group_id ");
                sbwhere.AppendFormat(" WHERE ege.group_id='{0}' ",query.group_id);
                if (query.email_id != 0)
                {
                    sbwhere.AppendFormat(" AND ege.email_id='{0}' ", query.email_id);
                }
                if (query.selectType == "0" && !string.IsNullOrEmpty(query.search_con))
                {
                    sbwhere.AppendFormat(" and ee.email_address like '%{0}%'", query.search_con.ToString());
                }
                if (query.selectType == "1" && !string.IsNullOrEmpty(query.search_con))
                {
                    sbwhere.AppendFormat(" and ege.email_name like '%{0}%'", query.search_con.ToString());
                }
                if (query.email_status != 0)
                {
                    sbwhere.AppendFormat(" and ege.email_status = '{0}'", query.email_status);
                }
                
                sbcount.Append("select count(ege.email_id) as totalCount ");
                if (query.IsPage)
                {
                    sbcount.Append(sbwhere.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sbcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sbwhere.Append(" ORDER BY ege.email_id ASC ");
                sbwhere.AppendFormat(" limit {0},{1} ",query.Start,query.Limit);
                sb.Append(sbwhere.ToString());
                return _accessMySql.getDataTableForObj<EdmGroupEmailQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->GetEdmGroupEmailList" + ex.Message + sb.ToString()+sbcount.ToString(), ex);
            }
        }

        public Model.Query.EdmGroupQuery Load(Model.Query.EdmGroupQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT group_id,group_name,group_total_email FROM edm_group WHERE group_id='{0}'",query.group_id);
                return _accessMySql.getSinggleObj<EdmGroupQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->Load" + ex.Message + sb.ToString(), ex);
            }
        }


        public int DeleteEdmGroupEmail(EdmGroupEmailQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("DELETE FROM edm_group_email WHERE group_id='{0}' AND email_id IN ({1})", query.group_id,query.email_ids);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->DeleteEdmGroupEmail" + ex.Message + sb.ToString(), ex);
            }
        }


        public List<Model.EdmEmail> getList(Model.EdmEmail query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT email_id,email_name,email_address FROM edm_email WHERE email_address='{0}'", query.email_address);
                return _accessMySql.getDataTableForObj<EdmEmail>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->getList" + ex.Message + sb.ToString(), ex);
            }
        }


        public int UpdateEdmEmail(Model.EdmEmail query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE edm_email SET email_name='{0}',email_updatedate='{1}' WHERE email_id='{2}'",query.email_name,query.email_updatedate,query.email_id);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->UpdateEdmEmail" + ex.Message + sb.ToString(), ex);
            }
        }

        public string UpdateEdmEmailStr(EdmEmailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("UPDATE edm_email SET email_name='{0}',email_updatedate='{1}' WHERE email_id='{2}'", query.email_name, query.email_updatedate, query.email_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->UpdateEdmEmailStr" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 檢查在EGE表中是否存在
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<EdmGroupEmailQuery> Check(EdmGroupEmailQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT email_id FROM edm_group_email WHERE email_id='{0}' and group_id='{1}'", query.email_id, query.group_id);
                return _accessMySql.getDataTableForObj<EdmGroupEmailQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->Check" + ex.Message + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 新增ege表數據
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int insertEGEInfo(EdmGroupEmailQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO edm_group_email (group_id,email_id,email_name,email_status,email_createdate,email_updatedate) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", query.group_id, query.email_id,query.email_name,query.email_status,query.email_createdate,query.email_updatedate);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->insertEGEInfo" + ex.Message + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 更新EGE表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpdateEGE(EdmGroupEmailQuery query)
        {
             StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE edm_group_email SET email_name='{0}',email_status='{1}',email_updatedate='{2}' WHERE group_id='{3}' AND email_id='{4}'",query.email_name,query.email_status,query.email_updatedate, query.group_id, query.email_id);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->UpdateEGE" + ex.Message + sb.ToString(), ex);
            }
        }
        public int UpdateEGEname(EdmGroupEmailQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE edm_group_email SET email_name='{0}',email_updatedate='{1}' WHERE group_id='{2}' AND email_id='{3}'", query.email_name, query.email_updatedate, query.group_id, query.email_id);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->UpdateEGE" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 返回對應的value值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Serial execSql(string sql)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(sql);
                return _accessMySql.getSinggleObj<Serial>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->Check" + ex.Message + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 新增edm_email數據
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int insertEdmEmail(EdmEmail query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO edm_email (email_id,email_name,email_address,email_check,email_createdate,email_updatedate) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", query.email_id, query.email_name, query.email_address, query.email_check, query.email_createdate, query.email_updatedate);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->insertEdmEmail" + ex.Message + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 獲取群組訂閱人數
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public DataTable getCount(int group_id)
        {

            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT count(email_id) from edm_group_email WHERE group_id='{0}' AND email_status='1'",group_id);
                return _accessMySql.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->getCount" + ex.Message + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 更新群組的訂閱總人數
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int updateEdmGroupCount(EdmGroupQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE edm_group SET group_total_email='{0}' WHERE group_id='{1}'", query.group_total_email,query.group_id);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->updateEdmGroupCount" + ex.Message + sb.ToString(), ex);
            }
        }
        public List<EdmGroupEmailQuery> GetModel(EdmGroupEmail query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT ege.group_id,ee.email_address,ege.email_id,ee.email_name,ege.email_status,ege.email_createdate,ege.email_updatedate ");
                sb.Append(" FROM edm_group_email ege ");
                sb.Append(" LEFT JOIN edm_email ee ON ee.email_id=ege.email_id ");
                sb.AppendFormat(" WHERE ege.email_id='{0}' ", query.email_id);
                if (query.group_id !=0)
                {
                    sb.AppendFormat(" AND ege.group_id='{0}' ", query.group_id);
                }
                return _accessMySql.getDataTableForObj<EdmGroupEmailQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->GetModel" + ex.Message + sb.ToString(), ex);
            }
        }

         public DataTable GetGroupID(string email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT ege.group_id from edm_group_email ege LEFT JOIN edm_email ee ON ege.email_id=ee.email_id  WHERE ee.email_address='{0}'", email);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->GetModel" + ex.Message, ex);
            }
        
        }

    }
}
