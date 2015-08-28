using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class IplasDao : IIplasImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;

       #region 有參構造函數
        /// <summary>
        /// 有參構造函數
        /// </summary>
        /// <param name="connectionStr">數據庫連接字符串</param>
        public IplasDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        #endregion

       #region 查詢商品主料位
        /// <summary>
        /// 查詢商品主料位
		 /// </summary>
		 /// <param name="m">實體</param>
		 /// <param name="totalCount">總條數，用來分頁</param>
		 /// <returns>返回列表頁</returns>
        public List<IplasQuery> GetIplas(IplasQuery m, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sb.AppendLine("select Count(ip.plas_id) as totalcounts from iplas ip where 1=1 ");
                sql.AppendLine(@"select mu.user_username as create_users, CONCAT(vb.brand_name,'-',p.product_name) as product_name,plas_id,dc_id,whse_id,loc_id,lcus_id,ip.item_id,prdd_id,ip.create_user,ip.create_dtim,loc_stor_cse_cap from iplas ip ");
                sql.AppendLine(@"left JOIN product_item pi ON ip.item_id=pi.item_id ");
                sql.AppendLine(@"left JOIN manage_user mu on mu.user_id=ip.create_user  ");
                sql.AppendLine(@"left JOIN product p ON p.product_id=pi.product_id  ");
                sql.AppendLine(@"left join vendor_brand vb on p.brand_id=vb.brand_id where 1=1 ");
                if (m.serch_type != 0)
                {
                    if (!string.IsNullOrEmpty(m.searchcontent))
                    {

                        switch (m.serch_type)
                        {
                            case 1:
                                sqlwhere.AppendFormat(" and ip.loc_id in (select loc_id from iloc where loc_id='{0}' or hash_loc_id='{0}' )  ", m.searchcontent);
                                break;
                            case 2:
                                sqlwhere.AppendFormat(" and ip.item_id=(select item_id from iupc iu where iu.upc_id ='{0}') ", m.searchcontent);
                                break;
                            case 3:
                                sqlwhere.AppendFormat(" and ip.item_id in ({0})  ", m.searchcontent);
                                break;
                            default:
                                break;
                        }
                    }
                }
                DateTime dt = DateTime.Parse("1970-01-02 08:00:00");
                if (!string.IsNullOrEmpty(m.starttime.ToString()) && dt < m.starttime)
                {
                    sqlwhere.AppendFormat(" and ip.create_dtim>'{0}' ", CommonFunction.DateTimeToString(m.starttime));
                }
                if (!string.IsNullOrEmpty(m.endtime.ToString()) && dt < m.endtime)
                {
                    sqlwhere.AppendFormat(" and ip.create_dtim<'{0}' ", CommonFunction.DateTimeToString(m.endtime));
                }
                //DateTime dt = DateTime.MinValue;
                //if (!string.IsNullOrEmpty(m.starttime.ToString()) && m.starttime>dt)
                //{
                //    sqlwhere.AppendFormat(" and ip.create_dtim > '{0}' ", m.starttime);
                //}
                //if (!string.IsNullOrEmpty(m.endtime.ToString()) && m.endtime > dt)
                //{
                //    sqlwhere.AppendFormat(" and ip.create_dtim < '{0}' ", m.endtime);
                //}
                totalCount = 0;
                if (m.IsPage)
                {
                    DataTable _dt = _dbAccess.getDataTable(sb.ToString() + sqlwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    sqlwhere.AppendFormat(" limit {0},{1}", m.Start, m.Limit);
                }
                return _dbAccess.getDataTableForObj<IplasQuery>(sql.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IplasDao-->GetIplas-->" + ex.Message + sql.ToString() + sqlwhere.ToString(), ex);
            }
        }
     #endregion

       #region 新增
       public int InsertIplasList(Iplas m)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("Insert into iplas (dc_id,whse_id,loc_id,change_dtim,change_user,create_dtim,create_user,lcus_id,luis_id,item_id,prdd_id,loc_rpln_lvl_uoi,loc_stor_cse_cap,ptwy_anch,flthru_anch,pwy_loc_cntl) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", m.dc_id, m.whse_id, m.loc_id.ToString().ToUpper(),CommonFunction.DateTimeToString(m.change_dtim), m.change_user,CommonFunction.DateTimeToString( m.create_dtim), m.create_user, m.lcus_id, m.luis_id, m.item_id, m.prdd_id, m.loc_rpln_lvl_uoi, m.loc_stor_cse_cap, m.ptwy_anch, m.flthru_anch, m.pwy_loc_cntl);//插入數據到表iplas表中
            sql.AppendFormat(@" set sql_safe_updates = 0; update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}';set sql_safe_updates = 1; ", "A", m.change_user, Common.CommonFunction.DateTimeToString(m.change_dtim), m.loc_id.ToString().ToUpper());
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            int i = 0;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("IplaseDao.InsertIplasList-->" + ex.Message+sql.ToString(), ex);
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
        #endregion

       #region 修改商品主料位
       public int UpIplas(Iplas m)
       {
           StringBuilder sb = new StringBuilder();
           StringBuilder sql = new StringBuilder();
           DataTable dt =new DataTable();
           int result=0;
           string loc = "";
           try
           {
               //查找之前主料位
               sb.AppendFormat("select loc_id from iplas where item_id='{0}' ;", m.item_id);
               dt = _dbAccess.getDataTable(sb.ToString());
               if (dt.Rows.Count > 0)
               {
                   loc = dt.Rows[0]["loc_id"].ToString();
                   if (m.loc_id == loc)
                   {
                       sql.AppendFormat(@"update iplas set item_id='{0}',loc_id='{1}',loc_stor_cse_cap='{2}',", m.item_id, m.loc_id.ToString().ToUpper(), m.loc_stor_cse_cap);
                       sql.AppendFormat(" change_user='{0}',change_dtim='{1}',lcus_id='{2}',prdd_id='{3}' where plas_id='{4}' ;", m.change_user, CommonFunction.DateTimeToString(m.change_dtim), m.lcus_id, m.prdd_id, m.plas_id);
                   }
                   else
                   {
                       MySqlCommand mySqlCmd = new MySqlCommand();
                       MySqlConnection mySqlConn = new MySqlConnection(connStr);
                       //變更主料位表的料位欄位
                       sql.AppendFormat(@"set sql_safe_updates = 0; update iplas set item_id='{0}',loc_id='{1}',loc_stor_cse_cap='{2}',", m.item_id, m.loc_id.ToString().ToUpper(), m.loc_stor_cse_cap);
                       sql.AppendFormat(" change_user='{0}',change_dtim='{1}',lcus_id='{2}',prdd_id='{3}' where plas_id='{4}' ;", m.change_user, CommonFunction.DateTimeToString(m.change_dtim), m.lcus_id, m.prdd_id, m.plas_id);
                       //跟新iinvd表的上架料位
                       sql.AppendFormat(@"update iinvd set plas_loc_id='{0}' where item_id='{1}' AND plas_loc_id='{2}';", m.loc_id, m.item_id, loc);
                       //修改老料位為可用
                       sql.AppendFormat(@" update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}'; ", "F", m.change_user, Common.CommonFunction.DateTimeToString(m.change_dtim), loc);
                       //修改新料位為已指派
                       sql.AppendFormat(@" update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}';set sql_safe_updates = 1; ", "A", m.change_user, Common.CommonFunction.DateTimeToString(m.change_dtim), m.loc_id.ToString().ToUpper());
                       sql.AppendFormat(@" insert into iloc_change_detail(icd_item_id,icd_old_loc_id,icd_new_loc_id,icd_create_time,icd_create_user,icd_modify_time,icd_modify_user,icd_status) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','CRE') ;", m.item_id, loc, m.loc_id, Common.CommonFunction.DateTimeToString(m.change_dtim), m.change_user, Common.CommonFunction.DateTimeToString(m.change_dtim), m.change_user);

                       try
                       {
                           if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                           {
                               mySqlConn.Open();
                           }
                           mySqlCmd.Connection = mySqlConn;
                           mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                           mySqlCmd.CommandType = System.Data.CommandType.Text;

                           mySqlCmd.CommandText = sql.ToString();
                           result = mySqlCmd.ExecuteNonQuery();
                           mySqlCmd.Transaction.Commit();
                       }
                       catch (Exception ex)
                       {
                           mySqlCmd.Transaction.Rollback();
                           throw new Exception("IplasDao.UpIplas-->" + ex.Message + sql.ToString(), ex);
                       }
                       finally
                       {
                           if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                           {
                               mySqlConn.Close();
                           }
                       }                
                   }
               }
               return result;
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->UpIplas-->" + ex.Message + sb.ToString() + sql.ToString(), ex);
           }
       }
        #endregion

       #region 判斷商品是否存在+string IsTrue(Iplas m)


       public string IsTrue(Iplas m)
       {
           
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendFormat(@" SELECT item_id from product_item where item_id='{0}'", m.item_id);
               if (_dbAccess.getDataTable(sql.ToString()).Rows.Count > 0)
               {
                   return "success";
               }
               else
               {
                   return "false";
               }
           }
           catch (Exception ex)
           {

               throw new Exception("IplasDao-->IsTrue-->" + ex.Message + sql.ToString(), ex); 
           }
           
       }

       #endregion
        //查詢Ipls的實體
       public Iplas getplas(Iplas query)
       {
           StringBuilder sb = new StringBuilder();
           try
           {
               sb.AppendFormat(" select  i.plas_id  from iplas i where i.item_id='{0}' ",query.item_id);
               return _dbAccess.getSinggleObj<Iplas>(sb.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->getplas-->" + ex.Message + sb.ToString(), ex); 
           }
       
       }
       #region 判斷商品是否重複
       public int GetIplasid(IplasQuery plas)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendFormat(@"SELECT plas_id FROM iplas WHERE item_id='{0}' ", plas.item_id);
               return _dbAccess.getDataTable(sql.ToString()).Rows.Count;
           }

           catch (Exception ex)
           {

               throw new Exception("IplasDao-->GetIplasid-->" + ex.Message + sql.ToString(), ex); 
           }
         

       }
       #endregion

       #region 判斷主料位是否重複
       public List<Iplas> GetIplasCount(Iplas m)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendFormat(@" SELECT plas_id from iplas where loc_id='{0}' ", m.loc_id.ToString().ToUpper());
               if (!string.IsNullOrEmpty(m.item_id.ToString()))
               {//本身修改主料位的時候判斷出自己之外的料位的是否可用
                   sql.AppendFormat(" AND item_id<>'{0}' ",m.item_id);
               }
               return _dbAccess.getDataTableForObj<Iplas>(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->GetIplasCount-->" + ex.Message + sql.ToString(), ex); 
           }           
       }
       #endregion

       #region 判斷主料位是否存在


       public int GetLocCount(Iloc loc)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               int result = 0;
               sql.AppendFormat(@" SELECT row_id from iloc where loc_id='{0}' and lcat_id='S' and lsta_id='F' and loc_status=1", loc.loc_id.ToString().ToUpper());

               if (_dbAccess.getDataTable(sql.ToString()).Rows.Count > 0)
               {
                   result = _dbAccess.getDataTable(sql.ToString()).Rows.Count;
                   return result;
               }
               else
               {
                   return result;
               }
           }
           catch (Exception ex)
           {

               throw new Exception("IplasDao-->GetLocCount-->" + ex.Message + sql.ToString(), ex); 
           }
           
       }

       #endregion

       #region 通過條碼查詢商品編號
        public DataTable Getprodbyupc(string prodid)
       {
           StringBuilder sql = new StringBuilder();
           string result = string.Empty;
           try
           {
               sql.AppendFormat("SELECT item_id FROM iupc WHERE upc_id='{0}'", prodid);
               return _dbAccess.getDataTable(sql.ToString());
            
           }
           catch (Exception ex)
           {

               throw new Exception("IplasDao-->Getprodbyupc-->" + ex.Message + sql.ToString(), ex); 
           }
       }
       #endregion

       #region 匯出
       public List<IplasQuery> Export(IplasQuery m)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendLine(@"SELECT ip.loc_id,il.lcat_id,ip.item_id,sum(ii.prod_qty) as 'prod_qtys',p.product_name,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,iu.upc_id,ii.cde_dt ");
               sql.AppendLine(@"from iplas ip LEFT JOIN  (SELECT upc_id,item_id from iupc GROUP BY item_id) iu ON ip.item_id=iu.item_id ");
               sql.AppendLine(@"LEFT JOIN product_item pi on pi.item_id=ip.item_id ");
               sql.AppendLine(@"LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id ");
               sql.AppendLine(@"LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id "); 
               sql.AppendLine(@"LEFT JOIN iinvd ii ON ip.item_id = ii.item_id ");
               sql.AppendLine(@"LEFT JOIN iloc il ON ip.loc_id= il.loc_id ");
               sql.AppendLine(@"LEFT JOIN product p ON pi.product_id =p.product_id ");
               sql.AppendLine(@"WHERE upc_id is NULL  ");

                if (m.startloc != "")
               {
                   sql.AppendFormat(" and ip.loc_id>'{0}' ", m.startloc);
               }
               if (m.endloc != "")
               {
                   sql.AppendFormat(" and ip.loc_id<'{0}' ", m.endloc);
               }
               sql.Append(" GROUP BY ip.item_id ORDER BY ip.loc_id; ");
               return _dbAccess.getDataTableForObj<IplasQuery>(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->Export-->" + ex.Message + sql.ToString(), ex);
           }
       }
        #endregion

       #region 主料位摘除表
        /// <summary>
        /// 查詢主料位 裡面的商品 並且需要摘除主料位的商品信息
        /// </summary>
        /// <param name="query">商品實體</param>
        /// <param name="totalCount">分頁顯示</param>
        /// <returns>list頁面</returns>
       public DataTable GetIlocReportList(ProductQuery query, out int totalCount)
       {
           StringBuilder sql = new StringBuilder();
           totalCount = 0;
           try
           {
               sql.AppendFormat(@" SELECT loc.loc_id as loc_id, iplas.item_id, CONCAT(vb.brand_name,'-',p.product_name) as Product_Name,pi.product_id, 
                    p.brand_id, v.vendor_name_simple as vendor_name_simple, upc.upc_id as upc_id,p.product_mode,tp.parameterName, 
                    (select sum(prod_qty) from iinvd where iinvd.item_id=iplas.item_id) as product_qty,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz
                    from iplas iplas 
                    LEFT JOIN product_ext pe on pe.item_id = iplas.item_id LEFT JOIN iloc loc on loc.loc_id=iplas.loc_id 
                    LEFT JOIN product_item pi on pi.item_id=iplas.item_id
                    LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
                    LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id
                    LEFT JOIN product p on p.product_id=pi.product_id 
                    LEFT JOIN t_parametersrc tp on tp.parameterCode=p.product_mode and tp.parameterType='product_mode' 
                    LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id LEFT JOIN vendor v on v.vendor_id=vb.vendor_id 
                    LEFT JOIN (SELECT upc_id,item_id from iupc GROUP BY item_id) upc on upc.item_id=iplas.item_id 
                    where pe.pend_del='Y' and loc.lcat_id='S'  ");
               if (int.Parse(query.vendor_name_simple) != 0)
               {
                   sql.AppendFormat(" and v.vendor_id='{0}' ", query.vendor_name_simple);
               }
               totalCount = 0;           
               sql.AppendFormat(" ORDER BY loc_id asc ");
             
               return _dbAccess.getDataTable(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->GetIlocReportList-->" + ex.Message + sql.ToString(), ex);
           }
       }
   
       #endregion

       #region 獲取所有廠商商（用作下拉列表框）+List<Vendor> VendorQueryAll(Vendor query)
       /// <summary>
       /// 獲取所有供應商（用作下拉列表框）
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       public List<Vendor> VendorQueryAll(Vendor query,string AddSql=null)
       {
           StringBuilder sbSql = new StringBuilder();
           try
           {
               sbSql.Append("SELECT vendor_id,vendor_name_simple  FROM vendor where 1=1 ");
               if (!string.IsNullOrEmpty(AddSql))
               {
                   sbSql.Append(AddSql);
               }
               //sbSql.Append("  ORDER BY vendor_id ASC");
               return _dbAccess.getDataTableForObj<Vendor>(sbSql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception(" vendorDao-->VendorQueryAll-->" + ex.Message + sbSql.ToString(), ex);
           }
       }
       #endregion
      
       #region 查詢主料位是否有商品佔據+int GetIinvdItemId(IinvdQuery vd)
       public int GetIinvdItemId(IinvdQuery vd)
       {
           StringBuilder sql = new StringBuilder();
           int result = 0;
           try
           {
               sql.AppendFormat(@"SELECT prod_qty FROM iinvd WHERE plas_loc_id='{0}' ", vd.loc_id.ToString().ToUpper());
               if (_dbAccess.getDataTable(sql.ToString()).Rows.Count > 0)
               {
                   result = Convert.ToInt32(_dbAccess.getDataTable(sql.ToString()).Rows[0]["prod_qty"]);
               }
               else
               {
                   result = 0;//當iinvd表中不存在數據時,iplas表可以刪除
               }
               return result;
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->GetIinvdItemId-->" + ex.Message + sql.ToString(), ex); 
           }
       }
       #endregion

       #region 刪除沒有商品佔據的iplas 數據+ int DeleteIplasById(IplasQuery plas)
       public int DeleteIplasById(IplasQuery plas)
       {
           StringBuilder sql = new StringBuilder();

           sql.AppendFormat(" set sql_safe_updates = 0;DELETE FROM iplas WHERE iplas.loc_id ='{0}';set sql_safe_updates = 1; ", plas.loc_id.ToString().ToUpper());
           sql.AppendFormat(" set sql_safe_updates = 0; update iloc  set lsta_id='F' where loc_id='{0}';set sql_safe_updates = 1; ", plas.loc_id.ToString().ToUpper());
               MySqlCommand mySqlCmd = new MySqlCommand();
               MySqlConnection mySqlConn = new MySqlConnection(connStr);
               int i = 0;
               try
               {
                   if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                   {
                       mySqlConn.Open();
                   }
                   mySqlCmd.Connection = mySqlConn;
                   mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                   mySqlCmd.CommandType = System.Data.CommandType.Text;
                   mySqlCmd.CommandText = sql.ToString();
                   i = mySqlCmd.ExecuteNonQuery();
                   mySqlCmd.Transaction.Commit();
               }
               catch (Exception ex)
               {
                   mySqlCmd.Transaction.Rollback();
                   throw new Exception("IplaseDao.DeleteIplasById-->" + ex.Message + sql.ToString(), ex);
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
       #endregion
        
       #region 無主料位報表+DataTable NoIlocReportList(ProductQuery query)
        public DataTable NoIlocReportList(ProductQuery query)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendLine(@"select loc_id,product_item.product_id as Product_Id ,product_item.item_id,CONCAT(v.brand_name,'-',p.product_name) as Product_Name,'' as prod_sz,vendor.vendor_name_simple as vendor_name_simple,'' as upc_id,pe.inner_pack_len,pe.inner_pack_wid,pe.inner_pack_hgt from product_item ");
               //sql.AppendFormat(@" LEFT JOIN product_spec ps1 ON product_item.spec_id_1 = ps1.spec_id LEFT JOIN product_spec ps2 ON product_item.spec_id_2 = ps2.spec_id ");
               sql.AppendLine(@" LEFT  join product p on product_item.product_id=p.product_id 
LEFT  join vendor_brand v on p.brand_id=v.brand_id  ");
               sql.AppendLine(@"left join vendor on v.vendor_id=vendor.vendor_id ");
               //sql.AppendLine(@" left join (SELECT upc_id,product_item from iupc GROUP BY item_id) iupc on product_item.item_id=iupc.item_id  ");
               sql.AppendLine(@"left join iplas on iplas.item_id=product_item.item_id  ");
               sql.AppendLine("left join product_ext  pe on pe.item_id=product_item.item_id ");
               sql.AppendLine(@"where product_item.item_id not in( select item_id from iplas  ) and p.product_mode=2   ");
               if (int.Parse(query.vendor_name_simple) != 0)
               {
                   sql.AppendFormat(" and vendor.vendor_id='{0}'", query.vendor_name_simple);
               }
               sql.AppendFormat("order by vendor.vendor_id,product_item.product_id DESC LIMIT 65000 ");
              
               return _dbAccess.getDataTable(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("IplasDao-->NoIlocReportList-->" + ex.Message + sql.ToString(), ex);
           }
       }
          #endregion

        public DataTable ExportMessage(IplasQuery m)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendLine(@"SELECT ip.loc_id,il.lcat_id,ip.item_id,sum(ii.prod_qty) as 'prod_qtys',CONCAT(v.brand_name,'-',p.product_name) AS 'product_name',concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,iu.upc_id,ii.cde_dt  ");
                sql.AppendLine(@"from iplas ip LEFT JOIN  (SELECT upc_id,item_id from iupc GROUP BY item_id) iu ON ip.item_id=iu.item_id ");
                sql.AppendLine(@"LEFT JOIN product_item pi on pi.item_id=ip.item_id ");
                sql.AppendLine(@"LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id ");
                sql.AppendLine(@"LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id ");
                sql.AppendLine(@"LEFT JOIN iinvd ii ON ip.item_id = ii.item_id ");
                sql.AppendLine(@"LEFT JOIN iloc il ON ip.loc_id= il.loc_id ");
                sql.AppendLine(@"LEFT JOIN product p ON pi.product_id =p.product_id ");
                sql.AppendLine(@"LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id ");
                sql.AppendLine(@"WHERE upc_id is NULL  ");

                if (m.startloc != "")
                {
                    sql.AppendFormat(" and ip.loc_id>'{0}' ", m.startloc);
                }
                if (m.endloc != "")
                {
                    sql.AppendFormat(" and ip.loc_id<'{0}' ", m.endloc);
                }
                sql.Append(" GROUP BY ip.item_id ORDER BY ip.loc_id; ");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IplasDao-->ExportMessage-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<IplasQuery> GetIplasExportList(IplasQuery iplas)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendLine(@"select mu.user_username as create_users, CONCAT(vb.brand_name,'-',p.product_name) as product_name,plas_id,dc_id,whse_id,loc_id,change_dtim,change_user,lcus_id,ip.item_id,p.prepaid,");
                sql.AppendLine(@"prdd_id,ip.create_user,ip.create_dtim,loc_stor_cse_cap from iplas ip ");
                sql.AppendLine(@"left JOIN product_item pi ON ip.item_id=pi.item_id ");
                sql.AppendLine(@"left JOIN manage_user mu on mu.user_id=ip.create_user  ");
                sql.AppendLine(@"left JOIN product p ON p.product_id=pi.product_id  ");
                sql.AppendLine(@"left join vendor_brand vb on p.brand_id=vb.brand_id where 1=1 ");
                if (iplas.serch_type != 0)
                {
                    if (!string.IsNullOrEmpty(iplas.searchcontent))
                    {

                        switch (iplas.serch_type)
                        {
                            case 1:
                                sql.AppendFormat(" and ip.loc_id in  (select loc_id from iloc where loc_id='{0}' or hash_loc_id='{0}' )  ", iplas.searchcontent);
                                break;
                            case 2:
                                sql.AppendFormat(" and ip.item_id=(select item_id from iupc iu where iu.upc_id ='{0}') ", iplas.searchcontent);
                                break;
                            case 3:
                                sql.AppendFormat(" and ip.item_id in ({0})  ", iplas.searchcontent);
                                break;
                            default:
                                break;
                        }
                    }
                   
                       
                   

                }
                DateTime dt = DateTime.Parse("1970-01-02 08:00:00");
                if (!string.IsNullOrEmpty(iplas.starttime.ToString()) && dt < iplas.starttime)
                {
                    sql.AppendFormat(" and ip.create_dtim>'{0}' ", CommonFunction.DateTimeToString(iplas.starttime));
                }
                if (!string.IsNullOrEmpty(iplas.endtime.ToString()) && dt < iplas.endtime)
                {
                    sql.AppendFormat(" and ip.create_dtim<'{0}' ", CommonFunction.DateTimeToString(iplas.endtime));
                }
                //if (!string.IsNullOrEmpty(iplas.searchcontent))
                //{
                //    sql.AppendFormat(" and (ip.item_id like '%{0}%' or ip.loc_id like '%{0}%')", iplas.searchcontent);
                //}
                //DateTime dt = DateTime.MinValue;
                //if (!string.IsNullOrEmpty(iplas.starttime.ToString()) && iplas.starttime>dt)
                //{
                //    sql.AppendFormat(" and ip.create_dtim > '{0}' ", iplas.starttime);
                //}
                //if (!string.IsNullOrEmpty(iplas.endtime.ToString()) && iplas.endtime > dt)
                //{
                //    sql.AppendFormat(" and ip.create_dtim < '{0}' ", iplas.endtime);
                //}
                return _dbAccess.getDataTableForObj<IplasQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IplasDao-->GetIplasExportList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int YesOrNoExist(int item_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int result = 0;
                sql.AppendFormat(@" SELECT item_id from iplas where item_id='{0}';",item_id);
                if (_dbAccess.getDataTable(sql.ToString()).Rows.Count > 0)
                {
                    result = _dbAccess.getDataTable(sql.ToString()).Rows.Count;
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("IplasDao-->GetLocCount-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int YesOrNoLocIdExsit(string loc_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int result = 0;
                sql.AppendFormat(@" SELECT row_id from iloc where loc_id='{0}';", loc_id);
                if (_dbAccess.getDataTable(sql.ToString()).Rows.Count > 0)
                {
                    result = _dbAccess.getDataTable(sql.ToString()).Rows.Count;
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

        public int YesOrNoLocIdExsit(int item_id, string loc_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int result = 0;
                sql.AppendFormat(@" select iplas.plas_id from iplas  left join iloc on iplas.loc_id=iloc.loc_id where iloc.loc_id='{0}' and iplas.item_id='{1}' and lcat_id='S' ;", loc_id, item_id);
                if (_dbAccess.getDataTable(sql.ToString()).Rows.Count > 0)
                {
                    result = _dbAccess.getDataTable(sql.ToString()).Rows.Count;
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
        public int ExcelImportIplas(string condition)
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
                throw new Exception("IupcDao.ExcelImportIplas-->" + ex.Message + condition.ToString(), ex);
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

        public int sql(string sql)
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

                mySqlCmd.CommandText = sql.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("IplasDao.UpIplas-->" + ex.Message + sql.ToString(), ex);
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

        #region 通過料位查詢商品編號
        public string Getlocid(string loc_id)
        {
            StringBuilder sql = new StringBuilder();
            string result = string.Empty;
            try
            {
                sql.AppendFormat("SELECT item_id FROM iplas left join iloc on iplas.loc_id=iloc.loc_id WHERE iloc.loc_id='{0}'  or iloc.hash_loc_id='{0}' ;", loc_id);
                return _dbAccess.getDataTable(sql.ToString()).Rows[0]["item_id"].ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("IplasDao-->Getlocid-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
