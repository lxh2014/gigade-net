using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;
using MySql.Data.MySqlClient;
/*
 *創建者：張瑜
 *創建時間：2014-11-4
 *v1.1修改內容：chaojie1124j 添加 GetIupcList方法
 * */
namespace BLL.gigade.Dao
{
    public class IupcDao : IiupcImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public IupcDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public int Delete(Iupc iupc)
        {
            string sbSql = String.Empty;
            try
            {
                sbSql = String.Format("set sql_safe_updates = 0; delete  from iupc where row_id={0} ;set sql_safe_updates = 1;", iupc.row_id);
                return _access.execCommand(sbSql);
            }
            catch (Exception ex)
            {                
                throw new Exception("IupcDao-->Delete-->" + ex.Message + sbSql, ex);
            }
        }
        /// <summary>
        /// 判斷輸入的商品編號和條碼編號是否有效
        /// </summary>
        /// <param name="iupc"></param>
        /// <returns></returns>
        public string IsExist(Iupc iupc)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder str=new StringBuilder();
            StringBuilder sb =new StringBuilder();
            int i = 0;
            try
            {
                sql.AppendFormat(@"SELECT item_id from product_item where item_id='{0}'", iupc.item_id);//有無商品
                if (_access.getDataTable(sql.ToString()).Rows.Count > 0)//判斷這個商品是否存在
                {
                    sql.Clear();
                    sql.AppendFormat(" SELECT row_id from iupc WHERE upc_id='{0}' ", iupc.upc_id);//有無條碼，不管國際國內，都不能重複

                    i = _access.getDataTable(sql.ToString()).Rows.Count;
                    str.AppendFormat(" SELECT row_id from iupc WHERE item_id='{0}' and upc_type_flg=1 ", iupc.item_id);//這個商品有無國際碼
                    int result = _access.getDataTable(str.ToString()).Rows.Count;
                    sb.AppendFormat(" select row_id from iupc where row_id='{0}' and upc_type_flg=1 ",iupc.row_id);//
                    int sbresult=_access.getDataTable(sb.ToString()).Rows.Count;
                    if (iupc.row_id == 0)//如果是新增
                    {
                        if (i < 1)//條碼不重複
                        {
                            if (iupc.upc_type_flg == "1")//如果新增的是國際碼，那個先看這個商品是否已有國際碼
                            {
                                if (result < 1)//沒有國際碼新增
                                {
                                    return "0";
                                }
                                else
                                {
                                    return "3";
                                }
                            }
                            else//店內碼隨便添加
                            {
                                return "0";
                            }
                        }
                        else
                        {
                            return "2";//條碼重複
                        }
                    }
                    //修改
                    else
                    {//

                        if (i == 0)//條碼不重複
                        {
                            if (iupc.upc_type_flg == "1")//修改城國際碼
                            {
                                if (result < 1)//沒有國際碼可以改
                                {
                                    return "0";
                                }
                                else//有國際碼，判斷是否是這個
                                {
                                    //sb.AppendFormat(" and  upc_id='{0}' ", iupc.upc_id);
                                    if (sbresult==1)//國際改國際
                                    {
                                        return "0";
                                    }
                                    else
                                    {
                                        return "3";//此商品已有國際碼
                                    }
                                }

                            }
                            else
                            {
                                return "0";
                            }

                        }
                        else //條碼重複，有可能是自身
                        {
                            //是自身還是別人的條碼
                            sql.AppendFormat(" and  row_id='{0}' ", iupc.row_id);
                            if (_access.getDataTable(sql.ToString()).Rows.Count == 1)//重複的是自身
                            {
                                if (iupc.upc_type_flg == "1")//修改成國際碼 
                                {
                                    //判斷自身是否已有國際碼
                                    str.AppendFormat(" and upc_id<>'{0}' ", iupc.upc_id);
                                    if (_access.getDataTable(str.ToString()).Rows.Count < 1)
                                    {
                                        return "0";
                                    }
                                    else 
                                    {
                                        return "3";
                                    }
                                    //sb.AppendFormat(" and  upc_id='{0}' ", iupc.upc_id);//國際改國際
                                    //if (_access.getDataTable(sql.ToString()).Rows.Count == 1)
                                    //{
                                    //    return "0";
                                    //}
                                    //else //普通改國際--沒國際碼
                                    //{
                                    //    if (result < 1)
                                    //    {
                                    //        return "0";
                                    //    }
                                    //    else
                                    //    {//已有國際碼
                                    //        return "3";
                                    //    }

                                    //}
                                }
                                else //修改普通碼
                                {
                                    return "0";
                                }

                            }
                            else //不是自身，條碼重複
                            {
                                return "2";
                            }
                        }
                    }
                    
                }
                else
                {
                    return "1";
                }
            }
            catch(Exception ex)
            {
                throw new Exception("IupcDao-->IsExist-->" + ex.Message + sql, ex);
            }
        }
        public int Insert(Iupc iupc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"insert into iupc (upc_id,suppr_upc,lst_ship_dte,lst_rct_dte,create_dtim,create_user,upc_type_flg,item_id) ");
            sql.AppendFormat(@"VALUES ('{0}','{1}','{2}','{3}',", iupc.upc_id, iupc.suppr_upc,CommonFunction.DateTimeToString(iupc.lst_ship_dte),CommonFunction.DateTimeToString(iupc.lst_rct_dte));
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}')", CommonFunction.DateTimeToString(iupc.create_dtim), iupc.create_user, iupc.upc_type_flg, iupc.item_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->Insert-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Update(Iupc iupc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE iupc set item_id='{0}',upc_id='{1}',upc_type_flg='{2}' where row_id='{3}'", iupc.item_id, iupc.upc_id, iupc.upc_type_flg, iupc.row_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #region  條碼列表+ List<IupcQuery> GetIupcList(IupcQuery iupc, out int totalCount)
        /// <summary>
        /// 返回條碼列表
        /// </summary>
        /// <param name="iupc">實體</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>列表頁</returns>
        public List<IupcQuery> GetIupcList(IupcQuery iupc, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append(@"select mu.user_username as create_users,i.row_id,i.upc_id,i.item_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,i.create_dtim,i.upc_type_flg from iupc i");
                sqlCondi.Append(@" left JOIN manage_user mu on mu.user_id=i.create_user ");
                sqlCondi.Append(@" left JOIN product_item pi on i.item_id=pi.item_id 
inner join product p on p.product_id=pi.product_id 
left join vendor_brand vb on p.brand_id=vb.brand_id  
where 1=1 ");//LEFT JOIN (select parametercode,parameterName from t_parametersrc where parameterType='iupc_type') para on para.parametercode=i.upc_type_flg ,para.parameterName as 'upc_type_flg_string'

                if (!string.IsNullOrEmpty(iupc.searchcontent))
                {
                    //sql.AppendFormat(" and (i.item_id like '%{0}%'  or i.upc_id like '%{0}%' )", iupc.searchcontent );
                    sqlCondi.AppendFormat(" and (i.item_id in ({0})  or i.upc_id in ({0}) )", iupc.searchcontent);
                }
                if (!string.IsNullOrEmpty(iupc.create_time_start))
                {
                    sqlCondi.AppendFormat(" and i.create_dtim >= '{0}' ", iupc.create_time_start);
                }
                if (!string.IsNullOrEmpty(iupc.create_time_end))
                {
                    sqlCondi.AppendFormat(" and i.create_dtim <='{0}' ", iupc.create_time_end);
                }
                if (!string.IsNullOrEmpty(iupc.searchcontentstring))
                {
                    string[] list = iupc.searchcontentstring.Split(',');
                    if (!string.IsNullOrEmpty(iupc.searchcontent))
                    {
                        for (int i = 0; i < list.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(list[i]))
                            {
                                sqlCondi.AppendFormat(" or (i.item_id = '{0}'  or i.upc_id = '{0}' )", list[i].ToString());
                            }
                        }
                    }
                    else {
                        int count = 0;
                        for (int i = 0; i < list.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(list[i]))
                            {
                                count++;
                                if (count != 1)
                                {
                                    sqlCondi.AppendFormat(" or (i.item_id = '{0}'  or i.upc_id = '{0}' )", list[i].ToString());
                                }
                                else
                                {
                                    sqlCondi.AppendFormat(" and (i.item_id = '{0}'  or i.upc_id = '{0}' )", list[i].ToString());
                                }
                            }
                        }
                    }
                }
                totalCount = 0;
                if (iupc.IsPage)
                {
                    string count = "select count(i.row_id) as sum from iupc i ";
                    System.Data.DataTable _dt = _access.getDataTable(count+sqlCondi.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount =int.Parse(_dt.Rows[0]["sum"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", iupc.Start, iupc.Limit);
                }
                return _access.getDataTableForObj<IupcQuery>(sql.ToString()+sqlCondi.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetIupcList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion

        public DataTable upcid(Iupc m)
        {//查詢該itemid 下的所有條碼
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT item_id from iupc where upc_id='{0}'; ", m.upc_id);
                return _access.getDataTable(sb.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("IupcDao-->upcid-->" + ex.Message+"sql:" + sb.ToString(), ex);
            }

        }
        //
        //判斷此商品是否有國際碼
        public int upc_num(int m)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select iupc.row_id from iupc where item_id='{0}' and upc_type_flg=1; ", m);
                return _access.getDataTable(sb.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->upc_num-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }
        }

        /*查看條碼和商品細項編號在料位裡面是否存在*/
        /// <summary>
        /// 條碼維護，匯入Excel
        /// </summary>
        /// chaojie1124j添加于2015/4/15
        /// <param name="i">商品細項編號</param>
        /// <param name="j">條碼</param>
        /// <returns>查看商品西鄉編號和條碼在料位表裡面是否存在</returns>
        public int Yesornoexist(int i, string j)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"SELECT item_id FROM product_item WHERE item_id='{0}'  ", i);
                if (_access.getDataTable(strSql.ToString()).Rows.Count <= 0)
                {
                    return 1;//商品表不存在此商品細項編號
                }
                strSql.Clear();

                strSql.AppendFormat(@"SELECT row_id,upc_id FROM iupc WHERE  upc_id='{0}' ",j);
                if (_access.getDataTable(strSql.ToString()).Rows.Count > 0)
                {
                    return 2;//該條碼已經存在
                }
                return 0;//新增
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao.Yesornoexist-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 批量匯入數據到條碼表
        /// </summary>
        /// <param name="condition">循環拼接的sql語句</param>
        /// <returns></returns>
        public int ExcelImportIupc(string condition)
        {
            int result = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = condition.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("IupcDao.ExcelImportIupc-->" + ex.Message + condition.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return result;
        }

        #region 條碼維護匯出信息
        public List<IupcQuery> GetIupcExportList(IupcQuery iupc)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"select mu.user_username as create_users,i.row_id,i.upc_id,i.item_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,i.create_dtim,i.upc_type_flg from iupc i");
                sql.Append(@" left JOIN manage_user mu on mu.user_id=i.create_user ");
                sql.Append(@" left JOIN product_item pi on i.item_id=pi.item_id 
inner join product p on p.product_id=pi.product_id 
left join vendor_brand vb on p.brand_id=vb.brand_id
where 1=1 ");
                if (!string.IsNullOrEmpty(iupc.searchcontent))
                {
                    //sql.AppendFormat(" and (i.item_id like '%{0}%'  or i.upc_id like '%{0}%' )", iupc.searchcontent );
                    sql.AppendFormat(" and (i.item_id in ({0})  or i.upc_id in ({0}) )", iupc.searchcontent);
                }
                if (!string.IsNullOrEmpty(iupc.create_time_start))
                {
                    sql.AppendFormat(" and i.create_dtim >= '{0}' ", iupc.create_time_start);
                }
                if (!string.IsNullOrEmpty(iupc.create_time_end))
                {
                    sql.AppendFormat(" and i.create_dtim <='{0}' ", iupc.create_time_end);
                }
                if (!string.IsNullOrEmpty(iupc.searchcontentstring))
                {
                    string[] list = iupc.searchcontentstring.Split(',');
                    if (!string.IsNullOrEmpty(iupc.searchcontent))
                    {
                        for (int i = 0; i < list.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(list[i]))
                            {
                                sql.AppendFormat(" or (i.item_id = '{0}'  or i.upc_id = '{0}' )", list[i].ToString());
                            }
                        }
                    }
                    else
                    {
                        int count = 0;
                        for (int i = 0; i < list.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(list[i]))
                            {
                                count++;
                                if (count != 1)
                                {
                                    sql.AppendFormat(" or (i.item_id = '{0}'  or i.upc_id = '{0}' )", list[i].ToString());
                                }
                                else
                                {
                                    sql.AppendFormat(" and (i.item_id = '{0}'  or i.upc_id = '{0}' )", list[i].ToString());
                                }
                            }
                        }
                    }
                }
                return _access.getDataTableForObj<IupcQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetIupcExportList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        public string Getupc(string item_id,string type)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select upc_id from iupc where item_id='{0}' ", item_id);
                if (type == "1")
                {
                    sb.Append("and upc_type_flg ='1';");
                }
                else if(type == "2")
                {
                    sb.Append("and upc_type_flg ='2' ORDER BY create_dtim DESC;");
                }
                if (_access.getDataTable(sb.ToString()).Rows.Count > 0)
                {
                    return _access.getDataTable(sb.ToString()).Rows[0]["upc_id"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->Getupc-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 通過商品細項編號，查詢條碼，有國際碼截取第一個為國際碼，沒有國際碼，取店內碼，取最新的條碼
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<IupcQuery> GetIupcByItemID(IupcQuery query)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT upc_id FROM iupc ");
                sqlCondi.Append(" where 1=1");
                if(query.item_id!=0){
                    sqlCondi.AppendFormat(" and  item_id='{0}' ", query.item_id);
                }
                sqlCondi.Append(" ORDER BY upc_type_flg asc,create_dtim DESC;");
                return _access.getDataTableForObj<IupcQuery>(sb.ToString() + sqlCondi.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetIupcByItemID-->" + ex.Message + sb.ToString() + sqlCondi.ToString(), ex);
            }
        }
        public List<IupcQuery> GetIupcByType(IupcQuery query)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT upc_id FROM iupc ");
                sqlCondi.Append(" where 1=1");
                if (query.item_id != 0)
                {
                    sqlCondi.AppendFormat(" and  item_id='{0}' ", query.item_id);
                }
                if (query.item_id != 0)
                {
                    sqlCondi.AppendFormat(" and  upc_type_flg='{0}' ", query.upc_type_flg);
                }
                sqlCondi.Append(" ORDER BY create_dtim DESC;");
                return _access.getDataTableForObj<IupcQuery>(sb.ToString() + sqlCondi.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetIupcByType-->" + ex.Message + sb.ToString() + sqlCondi.ToString(), ex);
            }
        }
    }
}
 