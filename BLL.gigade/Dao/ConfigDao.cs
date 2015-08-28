
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：ConfigDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j 
 * 完成日期：2014/10/07 13:48:16 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class ConfigDao : IConfigImplDao
    {
        private IDBAccess _accessMySql;
        string strSql = string.Empty;
        private string connStr;
        public ConfigDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }

        /// <summary>
        /// 判斷product_mange的合法性 
        /// </summary>
        /// <param name="paramername"></param>
        /// <param name="parameternumber"></param>
        /// <returns></returns>
        public List<ConfigQuery> Query(string paramername, int parameternumber)
        {
            // List<ConfigQuery> lcq = new List<ConfigQuery>();
            //strSql = string.Format("select * FROM config where config_name LIKE '%" + paramername + "%';");
            //DataTable dt = _accessMySql.getDataTable(strSql);
            //if (dt != null)
            //{
            //    for (int i = 1; i < parameternumber; i++)
            //    {
            //        ConfigQuery query = new ConfigQuery();
            //        query.chaek = dt.Rows[3 * i]["config_value"].ToString();
            //        if (dt.Rows[3 * i + 1]["config_value"].ToString() != "")
            //        {
            //            query.name = dt.Rows[3 * i + 1]["config_value"].ToString();
            //        }
            //        else
            //        {
            //            if (dt.Rows[3 * i + 2]["config_value"].ToString() != "")
            //            {
            //                query.name = dt.Rows[3 * i + 2]["config_value"].ToString();
            //                query.name = query.name.Substring(0, query.name.LastIndexOf("@"));
            //            }
            //        }
            //        query.email = dt.Rows[3 * i + 2]["config_value"].ToString();
            //        lcq.Add(query);
            //    }
            //}

            List<ConfigQuery> lcq = new List<ConfigQuery>();

            string strSql = string.Format("select * from config where config_name LIKE '" + paramername + "%' order by id ;");
            DataTable dt = _accessMySql.getDataTable(strSql);
            if (dt != null)
            {
                ArrayList configNameList = new ArrayList();
                for (int i = 1; i <= parameternumber; i++)
                {
                    ConfigQuery query = new ConfigQuery();
                    string tempKeyCheck = paramername + "_chaek_" + i.ToString();
                    string tempKeyName = paramername + "_name_" + i.ToString();
                    string tempKeyEmail = paramername + "_email_" + i.ToString();


                    if (dt.Rows[3 * i - 3]["config_name"].ToString() == tempKeyCheck)
                    {
                        //check==1並且email不為空
                        if (dt.Rows[3 * i - 3]["config_value"].ToString() == "1" && !string.IsNullOrEmpty(dt.Rows[3 * i - 1]["config_value"].ToString()))
                        {
                            query.name = dt.Rows[3 * i - 2]["config_value"].ToString();
                            query.email = dt.Rows[3 * i - 1]["config_value"].ToString();
                            lcq.Add(query);
                        }
                    }

                }
            }
            return lcq;
        }
        public List<Model.ManageUser> getUserPm(string nameStr)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select user_id ,user_username as name from manage_user where user_username in ({0})", nameStr);
                return _accessMySql.getDataTableForObj<Model.ManageUser>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->getUserPm-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public uint QueryByEmail(string email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select id from config where config_value='{0}'", email);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                int n = _dt.Rows.Count;
                DataRow row = _dt.Rows[0];
                return Convert.ToUInt32(row[0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->QueryByEmail-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public uint QueryByName(string name)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select ((select id FROM config where config_value='{0}')+1) from config where config_value='{1}'", name, name);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                int n = _dt.Rows.Count;
                DataRow row = _dt.Rows[0];
                return Convert.ToUInt32(row[0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->QueryByName-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 獲取config表中的值
        public DataTable GetConfig(ConfigQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select id ,config_name, config_value,config_content from config where config_name='{0}'", query.config_name);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->GetConfig-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 新增config表數據
        public int InsertConfig(ConfigQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("INSERT INTO config (config_name,config_value,config_content) VALUES('{0}','{1}','{2}');", query.config_name, query.config_value, query.config_content);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->InsertConfig-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更新config表數據
        public int UpdateConfig(ConfigQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates=0;");
                sql.AppendFormat("UPDATE config SET config_value='{0}' WHERE config_name='{1}' ;", query.config_value,query.config_name);
                sql.Append("set sql_safe_updates=1;");
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->UpdateConfig-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 檢查config_name是否存在
        public DataTable ConfigCheck(ConfigQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select config_name, config_value,config_content from config where config_name='{0}' ORDER BY config_name ASC;", query.config_name);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->ConfigCheckName-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 根據like查詢
        public DataTable GetConfigByLike(ConfigQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT config_name,config_value FROM config WHERE config_name LIKE '%{0}%';", query.config_name);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->GetConfigByLike-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion


        #region 獲取優化名稱store
        public DataTable GetUser(ManageUserQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT user_id,user_username as config_name,user_email FROM manage_user where 1=1 ");
                if (query.user_id != 0)
                {
                    sql.AppendFormat(" and user_id='{0}' ", query.user_id);
                }
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->GetUser-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 新增時查看收件人名稱是否重複
        public List<ConfigQuery> CheckSingleConfig(ConfigQuery query)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT id from config where config_name like '%{0}%' AND config_value='{1}'",query.config_name,query.config_value);
                return _accessMySql.getDataTableForObj<ConfigQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->CheckSingleConfig-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 編輯時檢查用戶名是否重複
        public List<ConfigQuery> CheckUserName(ConfigQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT config_value FROM config where config_name = '{0}' ",query.config_name);
                return _accessMySql.getDataTableForObj<ConfigQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ConfigDao-->CheckUserName-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
