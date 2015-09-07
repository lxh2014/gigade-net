using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    class ParametersrcDao : IParametersrcImplDao
    {
        private IDBAccess _accessMySql;
        private MySqlDao _mayDao;
        private string connStr;
        public ParametersrcDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;        
            _mayDao = new MySqlDao(connectionString);
        }
        #region IParametersrcImplDao 成员
        public List<Parametersrc> Query(Parametersrc para) 
        {
            para.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select rowid,parametertype,parameterproperty,parametercode,parametername,remark,kdate,kuser,used,sort,topvalue from t_parametersrc where 1=1 ");
            try
            {
                if (para.Rowid != 0)
                {
                    strSql.AppendFormat(" and rowid='{0}' ", para.Rowid);
                }
                if (!string.IsNullOrEmpty(para.ParameterType))
                {
                    strSql.AppendFormat(" and parametertype='{0}' ", para.ParameterType);
                }
                if (!string.IsNullOrEmpty(para.ParameterCode))
                {
                    strSql.AppendFormat(" and parametercode='{0}' ", para.ParameterCode);
                }
                if (!string.IsNullOrEmpty(para.parameterName))
                {
                    strSql.AppendFormat(" and parametername='{0}' ", para.parameterName);
                }
                if (!string.IsNullOrEmpty(para.TopValue))
                {
                    strSql.AppendFormat(" and topvalue = '{0}'", para.TopValue);
                }
                strSql.AppendFormat(" and used='{0}' ", para.Used);
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDap-->Query"+ex.Message+strSql.ToString(),ex);
            }
        }

        public List<Parametersrc> QueryForTopValue(Parametersrc para)
        {
            para.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select rowid,parametertype,parameterproperty,parametercode,parametername,remark,kdate,kuser,used,sort,topvalue");
            try
            {
                strSql.AppendFormat("  from t_parametersrc where 1=1  and topvalue = '{0}' and parametertype='{1}' ", para.TopValue, para.ParameterType);
                strSql.AppendFormat(" and used={0}", para.Used);
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDap-->QueryForTopValue" + ex.Message + strSql.ToString(), ex);
            }

        }

        public string Save(Parametersrc p)
        {
            StringBuilder stb = new StringBuilder("insert into t_parametersrc (parametertype,parameterproperty,parametercode,parametername,remark,kdate,kuser,used,sort,topvalue) values ");
            try
            {
                stb.AppendFormat("('{0}','{1}','{2}','{3}','{4}',now(),'{5}','{6}','{7}','{8}')", p.ParameterType, p.ParameterProperty, p.ParameterCode, p.parameterName, p.remark, p.Kuser, p.Used, p.Sort, p.TopValue);
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDap-->Save" + ex.Message + stb.ToString(), ex);
            }
        }

        public bool Save(List<Parametersrc> saveList)
        {
            try
            {
                System.Collections.ArrayList c_array = new System.Collections.ArrayList();
                saveList.ForEach(m => c_array.Add(Save(m)));
                return _mayDao.ExcuteSqls(c_array);
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->Save(List<Parametersrc> saveList)-->" + ex.Message, ex);
            }
        }

        public string Update(Parametersrc p)
        {
            StringBuilder stb = new StringBuilder("set sql_safe_updates = 0;update t_parametersrc set");
            try
            {
                stb.AppendFormat(" parametertype = '{0}',parameterproperty='{1}',parametercode = '{2}',parametername='{3}',remark='{4}',kuser='{6}',used='{7}',sort='{8}',topvalue='{9}'", p.ParameterType, p.ParameterProperty, p.ParameterCode, p.parameterName, p.remark, p.Kdate, p.Kuser, p.Used, p.Sort, p.TopValue);
                stb.AppendFormat("  where rowid = {0};set sql_safe_updates = 1;", p.Rowid);
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->Update" + ex.Message + stb.ToString(), ex);
            }
        }


        public bool Update(List<Parametersrc> updateList)
        {
            try
            {
                System.Collections.ArrayList c_array = new System.Collections.ArrayList();
                updateList.ForEach(m => c_array.Add(Update(m)));
                return _mayDao.ExcuteSqls(c_array);
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->Update(List<Parametersrc> updateList)-->" + ex.Message, ex);
            }
        }

        public string DeleteByType(Parametersrc p)
        {
            return string.Format("set sql_safe_updates = 0; delete from t_parametersrc where parametertype = '{0}';set sql_safe_updates = 1;", p.ParameterType);
        }

        /// <summary>
        /// 查詢異動的所有type
        /// </summary>
        /// <param name="NotIn">不需要的parametertype的值</param>
        /// <returns></returns>
        public List<Parametersrc> QueryType(string NotIn)
        {
            string strSql = "select distinct parametertype from t_parametersrc where parametertype like 'warn_%'";
            try
            {
                if (NotIn != "")
                {
                    strSql += " and parametertype <>'" + NotIn + "'";
                }
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->QueryType" + ex.Message + strSql.ToString(), ex);
            }
        }


        //public DataTable QueryPropertySite()
        //{
        //    StringBuilder sbsql = new StringBuilder();
        //    sbsql.Append("select parameterProperty from t_parametersrc where 1=1 ");
        //    if (Cquery.TopValue != "")
        //    {
        //        sbsql.AppendFormat(" and topValue='{0}'", Cquery.TopValue);
        //    }
        //    if (Cquery.ParameterType != "")
        //    {
        //        sbsql.AppendFormat(" and ParameterType='{0}'", Cquery.ParameterType);
        //    }
        //    if (Cquery.ParameterCode != "")
        //    {
        //        sbsql.AppendFormat(" and ParameterCode='{0}'", Cquery.ParameterCode);
        //    }

        //    return _accessMySql.getDataTable(sbsql.ToString());
        //}
        public DataTable QueryProperty(Parametersrc Pquery, Parametersrc Cquery)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                int fathar_id = 0;
                str.AppendFormat("select * from t_parametersrc where ParameterType='{0}' and ParameterCode='{1}'", Pquery.ParameterType, Pquery.ParameterCode);
                System.Data.DataTable _dt = _accessMySql.getDataTable(str.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    fathar_id = Convert.ToInt32(_dt.Rows[0]["rowid"]);
                    Cquery.TopValue = fathar_id.ToString();
                }

                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("select parameterProperty from t_parametersrc where 1=1 ");
                if (Cquery.TopValue != "")
                {
                    sbsql.AppendFormat(" and topValue='{0}'", Cquery.TopValue);
                }
                if (Cquery.ParameterType != "")
                {
                    sbsql.AppendFormat(" and ParameterType='{0}'", Cquery.ParameterType);
                }
                if (Cquery.ParameterCode != "")
                {
                    sbsql.AppendFormat(" and ParameterCode='{0}'", Cquery.ParameterCode);
                }

                return _accessMySql.getDataTable(sbsql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->QueryProperty"+ex.Message + str.ToString(), ex);
            }
        }


        /// <summary>
        /// 根據code得到相關parameter信息
        /// </summary>
        /// add by wangwei
        /// <returns></returns>
        public List<Parametersrc> GetParameterByCode(string code)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select rowid,parameterType,parameterProperty,parameterCode,parameterName,topValue from t_parametersrc where parameterType ='product_cate'  and ((topValue = 0 and parameterCode = '{0}') or topValue = '{0}') ", code);
                List<Parametersrc> list = _accessMySql.getDataTableForObj<Parametersrc>(sb.ToString());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->GetParameterByCode" + ex.Message + sb.ToString(), ex);
            }
        }

        #endregion

        #region 參數表列表顯示+List<Parametersrc> GetParametersrcList(Parametersrc store, out int totalCount)


        public List<Parametersrc> GetParametersrcList(Parametersrc store, out int totalCount)
        {
            store.Replace4MySQL();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder strSql = new StringBuilder("select rowid,parametertype,parameterproperty,parametercode,parametername,remark,kdate,kuser,used,sort,topvalue  ");
            sqlfrom.AppendFormat(" from t_parametersrc where 1=1 ");
            sqlcount.AppendFormat(@" select count(*) as searchtotal ");
            if (store.Rowid != 0)
            {
                sqlfrom.AppendFormat(" and rowid='{0}' ", store.Rowid);
            }
            if (!string.IsNullOrEmpty(store.ParameterType))
            {
                sqlfrom.AppendFormat(" and parameterType like N'%{0}%' ", store.ParameterType);
                sqlfrom.AppendFormat(" or parameterProperty like N'%{0}%'", store.ParameterType);
                sqlfrom.AppendFormat(" or parameterCode like N'%{0}%'", store.ParameterType);
                sqlfrom.AppendFormat(" or parameterName like N'%{0}%'", store.ParameterType);
                sqlfrom.AppendFormat(" or remark like N'%{0}%'", store.ParameterType);
            }
            sqlfrom.AppendFormat(" order by rowid desc ");
            totalCount = 0;
            if (store.IsPage)
            {
                DataTable _dt = _accessMySql.getDataTable(sqlcount.ToString() + sqlfrom.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["searchtotal"]);
                }
                sqlfrom.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
            }

            try
            {
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception(" ParametersrcDao-->GetParametersrcList-->" + strSql.ToString() + sqlfrom.ToString() + ex.Message, ex);
            }
          
            
        }
        #endregion

        #region 參數表新增修改保存方法+int ParametersrcSave(Parametersrc para)
        /// <summary>
        /// 參數表新增修改保存方法
        /// 2014/10/20號zhejiangj新增
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public int ParametersrcSave(Parametersrc para)
        {
            string strSql = string.Empty;
            try
            {
                if (para.Rowid == 0)
                {
                    strSql = Save(para);
                    return _accessMySql.execCommand(strSql);
                }
                else
                {
                    strSql = Update(para);
                    return _accessMySql.execCommand(strSql);
                }
               
            }
            catch (Exception ex)
            {

                throw new Exception("ParametersrcDao-->ParametersrcSave-->sql:" + ex.Message + strSql.ToString(), ex);
            }
        }
        #region 查詢一條數據根據RowId+List<Parametersrc> QuerySinggleByID(Parametersrc para)
        /// <summary>
        /// 查詢一條數據根據RowId
        /// 2014/10/20號zhejiangj新增
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public List<Parametersrc> QuerySinggleByID(Parametersrc para)
        {
            para.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select rowid,parametertype,parameterproperty,parametercode,parametername,remark,kdate,kuser,used,sort,topvalue from t_parametersrc where 1=1 ");
            try
            {
                if (para.Rowid != 0)
                {
                    strSql.AppendFormat(" and rowid='{0}' ", para.Rowid);
                }
                if (!string.IsNullOrEmpty(para.ParameterType))
                {
                    strSql.AppendFormat(" and parametertype='{0}' ", para.ParameterType);
                }
                if (!string.IsNullOrEmpty(para.ParameterCode))
                {
                    strSql.AppendFormat(" and parametercode='{0}' ", para.ParameterCode);
                }
                if (!string.IsNullOrEmpty(para.parameterName))
                {
                    strSql.AppendFormat(" and parametername='{0}' ", para.parameterName);
                }
                if (!string.IsNullOrEmpty(para.TopValue))
                {
                    strSql.AppendFormat(" and topvalue = '{0}'", para.TopValue);
                }
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->QuerySinggleByID" + ex.Message + strSql.ToString(), ex);
            }

        } 
        #endregion
        #endregion

        #region 更改參數表狀態+int UpdateUsed(Parametersrc store)


        public int UpdateUsed(Parametersrc store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" update t_parametersrc set used='{0}' where rowid='{1}'", store.Used, store.Rowid);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ParametersrcDao-->UpdateUsed-->" + sql.ToString() + ex.Message, ex);
            }
           
        }

        #endregion

        #region 付款單狀態下拉列表+List<Parametersrc> PayforType(string parameterType)


        public List<Parametersrc> PayforType(string parameterType)
        {
            StringBuilder strSql = new StringBuilder("select parametercode,remark from t_parametersrc where 1=1 ");

            strSql.AppendFormat(" and parameterType='{0}' ", parameterType);
            try
            {
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ParametersrcDao-->PayforType-->"+strSql.ToString()+ex.Message,ex); 
            }
           
        }
        #endregion


        public List<Parametersrc> GetElementType(string types)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select parametercode,parameterName from t_parametersrc where parameterType='{0}'", types);
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ParametersrcDao-->GetElementType-->" + strSql.ToString() + ex.Message, ex);
            }
           
        }

        /// <summary>
        /// 根據字段取參數表查詢所需要的參數
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<Parametersrc> GetAllKindType(string types)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select parametercode,parameterName,remark from t_parametersrc where parameterType='{0}'", types);
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ParametersrcDao-->GetElementType-->" + strSql.ToString() + ex.Message, ex);
            }
        }

        #region 庫調參數維護
        public List<Parametersrc> GetIialgParametersrcList(Parametersrc store, out int totalCount)
        {
            store.Replace4MySQL();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder strSql = new StringBuilder("select t.rowid,t.parametertype,t.parameterproperty,t.parametercode,t.parametername,t.remark,t.kdate,t.kuser,t.used,t.sort,tp.parameterName as 'TopValue'  ");
            sqlfrom.AppendFormat(" FROM t_parametersrc t LEFT JOIN t_parametersrc tp ON t.topValue = tp.rowid  where 1=1 and tp.parameterType='wms_parameter' ");//parameterType='wms_parameter'
            sqlcount.AppendFormat(@" select count(*) as searchtotal ");
            //if (store.Rowid != 0)
            //{
            //    sqlfrom.AppendFormat(" and rowid='{0}' ", store.Rowid);
            //}
            if (!string.IsNullOrEmpty(store.remark))//這裡remark做為搜獲的內容
            {
                sqlfrom.AppendFormat(" and (t.parameterType like'%{0}%' or t.parameterName like '%{0}%') ", store.remark);
             }
            sqlfrom.AppendFormat(" order by t.rowid desc ");
            totalCount = 0;
            if (store.IsPage)
            {
                DataTable _dt = _accessMySql.getDataTable(sqlcount.ToString() + sqlfrom.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["searchtotal"]);
                }
                sqlfrom.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
            }
            try
            {
                return _accessMySql.getDataTableForObj<Parametersrc>(strSql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ParametersrcDao-->GetIialgParametersrcList-->" + strSql.ToString() + sqlfrom.ToString() + ex.Message, ex);
            }
        }

        public int Delete(Parametersrc m)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sb = new StringBuilder();
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                PromoPair pp = new PromoPair();
                if (m.Rowid != 0)
                {
                    sb.AppendFormat("Delete from t_parametersrc where Rowid ='{0}';", m.Rowid);
                }
                mySqlCmd.CommandText = sb.ToString();
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoPair-->Delete-->" + ex.Message +",sql:"+ sb.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return i;
        }
        public List<Parametersrc> GetParameter(Parametersrc p)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select rowid,parametername from t_parametersrc where 1=1 ");
                if (!String.IsNullOrEmpty(p.ParameterType))
                {
                    sb.AppendFormat(" AND ParameterType='{0}' ", p.ParameterType); 
                }
                if (!String.IsNullOrEmpty(p.parameterName))
                {
                    sb.AppendFormat(" AND parameterName='{0}' ",p.parameterName);
                }
                return _accessMySql.getDataTableForObj<Parametersrc>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ParametersrcDao-->GetParameter-->"+ ex.Message +",sql:" + sb.ToString(), ex);
            }
        }
        public DataTable GetParametercode(Parametersrc p)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select rowid,parametername,parameterCode from t_parametersrc where 1=1 ");
                if (!String.IsNullOrEmpty(p.ParameterType))
                {
                    sb.AppendFormat(" AND ParameterType='{0}' ", p.ParameterType);
                }
                if (!String.IsNullOrEmpty(p.parameterName))
                {
                    sb.AppendFormat(" AND parameterName='{0}' ", p.parameterName);
                }
                if (!String.IsNullOrEmpty(p.TopValue))
                {
                    sb.AppendFormat(" AND TopValue='{0}' ", p.TopValue);
                }
                if (!String.IsNullOrEmpty(p.ParameterCode))
                {
                    sb.AppendFormat(" AND ParameterCode='{0}' ", p.ParameterCode);
                }
                sb.Append(" ORDER BY rowid DESC;");
                return _accessMySql.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ParametersrcDao-->GetParametercode-->" + ex.Message + ",sql:" + sb.ToString(), ex);
            }
        }
        public int InsertTP(Parametersrc p)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("Insert into t_parametersrc(parametertype,parametercode,parametername,remark,kdate,kuser,used,sort,TopValue)");
                sb.AppendFormat(" VALUES ('{0}','{1}','{2}','{3}',",p.ParameterType,p.ParameterCode,p.parameterName,p.remark);
                sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}');",CommonFunction.DateTimeToString(p.Kdate),p.Kuser,p.Used,p.Sort,p.TopValue);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->InsertTP-->" + ex.Message + ",sql:" + sb.ToString(), ex);
            }
        }
        public int UpdTP(Parametersrc p)
        {
            StringBuilder sb = new StringBuilder("set sql_safe_updates = 0;update t_parametersrc set ");
            sb.AppendFormat(" parametertype='{0}', parametercode = '{1}',parametername='{2}',remark='{3}' ", p.ParameterType, p.ParameterCode, p.parameterName, p.remark);
            sb.AppendFormat("  where rowid = {0};set sql_safe_updates = 1;", p.Rowid);
            try
            {
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ParametersrcDao-->UpdTP-->"+ ex.Message + ",sql:" + sb.ToString() , ex);
            }
        }

        #endregion
        public string GetOrderStatus(int pc)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}';",pc);
                return _accessMySql.getDataTable(strSql.ToString()).Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
           
                throw new Exception("ParametersrcDao-->GetElementType-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        public List<Parametersrc> QueryParametersrcByTypes(params string[] types)
        {
            try
            {
                var sqlStr = string.Format("select parametername,parametercode,parameterType,remark from t_parametersrc where parametertype in ('{0}')", string.Join("','", types));
                return _accessMySql.getDataTableForObj<Parametersrc>(sqlStr);
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->QueryParametersrcByTypes" + ex,ex);
            }

        }

        /// <summary>
        /// 列表頁帶參數值優化
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public DataTable GetTP(Parametersrc p)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select rowid,ParameterName,parameterCode,remark from t_parametersrc where 1=1 ");
                if (!String.IsNullOrEmpty(p.ParameterType))
                {
                    sb.AppendFormat(" AND ParameterType='{0}' ", p.ParameterType);
                }
                if (!String.IsNullOrEmpty(p.parameterName))
                {
                    if (p.parameterName == "自取")
                    {
                        sb.AppendFormat(" AND parameterName like '{0}%' ", p.parameterName);
                    }
                    else
                    {
                        sb.AppendFormat(" AND parameterName='{0}' ", p.parameterName);
                    }
                }
                if (!String.IsNullOrEmpty(p.TopValue))
                {
                    sb.AppendFormat(" AND TopValue='{0}' ", p.TopValue);
                }
                if (!String.IsNullOrEmpty(p.ParameterCode))
                {
                    sb.AppendFormat(" AND ParameterCode='{0}' ", p.ParameterCode);
                }
                sb.Append(" ORDER BY rowid DESC;");
                return _accessMySql.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ParametersrcDao-->GetTP-->" + ex.Message + ",sql:" + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 獲取測試用的mail
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string Getmail(string p)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT remark FROM t_parametersrc WHERE parameterType='{0}' ", p);
                return _accessMySql.getDataTable(strSql.ToString()).Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcDao-->Getmail-->" + strSql.ToString() + ex.Message, ex);
            }
        }

        public List<Parametersrc> ReturnParametersrcList()
        {
            StringBuilder sbStr = new StringBuilder();
            try
            {
                 sbStr.Append("SELECT parameterName,parameterCode from t_parametersrc t where t.parameterType='Deliver_Store'");
                return _accessMySql.getDataTableForObj<Parametersrc>(sbStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" ParametersrcDao-->GetParametersrcList()-->" + ex.Message + ",sql:" + sbStr.ToString(), ex);
            }
        }
    }
}
