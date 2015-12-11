using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class CbjobDetailDao : ICbjobDetailImplDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public CbjobDetailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public List<Model.Query.CbjobDetailQuery> GetMessage(Model.Query.CbjobDetailQuery cbjobQuery, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            totalCount = 0;
            try
            {//適當進行修改
                sql.AppendFormat(@"
SELECT cm.sta_id,idd.item_id,idd.st_qty,cd.cb_newid,cd.cb_jobid,idd.cde_dt,idd.made_date as made_dt,idd.plas_loc_id as         
loc_id,idd.prod_qty,cd.create_datetime,cd.create_user,pi.spec_id_1 ,p.spec_title_1,p.spec_title_2,pi.spec_id_2,cd.iinvd_id,mu.user_username,CONCAT(v.brand_name,'-',p.product_name) as 'product_name' FROM cbjob_detail cd 
left JOIN iinvd idd on cd.iinvd_id=idd.row_id 
inner JOIN product_item pi on pi.item_id=idd.item_id 
LEFT JOIN product p on p.product_id =pi.product_id
LEFT JOIN cbjob_master cm on cm.cbjob_id=cd.cb_jobid
LEFT JOIN manage_user mu on cd.create_user=mu.user_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
WHERE cd.cb_jobid='{0}'and cd.cb_newid>'{1}' and idd.ista_id='A' and cd.status=1 ", cbjobQuery.searchcontent, cbjobQuery.cb_newid);
                if (cbjobQuery.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", cbjobQuery.Start, cbjobQuery.Limit);
                }
                 List < Model.Query.CbjobDetailQuery > Store= _access.getDataTableForObj<CbjobDetailQuery>(sql.ToString());
                 IProductSpecImplDao _specDao = new ProductSpecDao(connStr);
                 for (int i = 0; i < Store.Count; i++)
                 {
                     ProductSpec spec1 = _specDao.query(int.Parse(Store[i].spec_id_1.ToString()));
                     ProductSpec spec2 = _specDao.query(int.Parse(Store[i].spec_id_2.ToString()));
                     if (spec1 != null)
                     {
                         Store[i].spec_title_1 = string.IsNullOrEmpty(Store[i].spec_title_1) ? "" :Store[i].spec_title_1 + ":" + spec1.spec_name;
                     }
                     if (spec2 != null)
                     {
                        Store[i].spec_title_2 = string.IsNullOrEmpty(Store[i].spec_title_2) ? "" : Store[i].spec_title_2 + ":" + spec2.spec_name;
                     }
                    Store[i].spec_title_1= string.IsNullOrEmpty(Store[i].spec_title_1) ? "" :Store[i].spec_title_1 + "  " + Store[i].spec_title_2;
                    Store[i].product_name += Store[i].spec_title_1;
                 
                 }
                     return Store;
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailDao-->GetMessage-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int DeleteCbjobmessage(CbjobDetailQuery cbjobQuery)
        {
            StringBuilder sql = new StringBuilder();
            int result = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder str = new StringBuilder();
            str.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}'and status=1", cbjobQuery.cb_jobid);
            if (_access.getDataTable(str.ToString()).Rows.Count == 0)//程序到此直接跳轉出去
            {
                return -2;//-2表示不存在該工作編號或已被刪除
            }
            sb.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}' and status=1 and sta_id= '{1}'", cbjobQuery.cb_jobid, "CNT");
            if (_access.getDataTable(sb.ToString()).Rows.Count == 0)//判斷該工作編號是否能被刪除
            {
                return -1;
            }
            sql.AppendFormat("set sql_safe_updates = 0;");
            sql.AppendFormat(@" update cbjob_detail set status='{0}' where cb_jobid='{1}';", 0, cbjobQuery.cb_jobid);
            sql.AppendFormat(@" update cbjob_master set status='{0}' where cbjob_id='{1}';", 0, cbjobQuery.cb_jobid);
            sql.AppendFormat("set sql_safe_updates = 1;");
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
                throw new Exception("CbjobDetailDao.DeleteCbjobmessage-->" + ex.Message + str.ToString() + sb.ToString() + sql.ToString(), ex);
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
        public int UpdateCbjobMaster(CbjobDetailQuery cbjobQuery)
        {
            StringBuilder sql = new StringBuilder();
            try
            {//適當進行修改
                sql.AppendFormat("set sql_safe_updates = 0;");
                sql.AppendFormat(@"update cbjob_master set sta_id='{0}',create_user='{1}',create_datetime='{2}' where cbjob_id='{3}';", "UPD", cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.cb_jobid);
                sql.AppendFormat("set sql_safe_updates = 1;");
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailDao-->UpdateCbjobMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 蓋帳按鈕--插入idiff_count_book表中即時差異報表/把COM的狀態改為END
        /// </summary>
        /// <param name="cbjobQuery"></param>
        /// <returns></returns>
        public int UpdateCbjobstaid(CbjobDetailQuery cbjobQuery)
        {
            int result = 0;
             int jbcount=0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder str = new StringBuilder();
            StringBuilder strsql = new StringBuilder();
            StringBuilder strsqltwo = new StringBuilder();
            string iinvdidstr = string.Empty;
            string jobnumber="P" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (!string.IsNullOrEmpty(cbjobQuery.cb_jobid))
            {
                str.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}'and status=1 and create_datetime>='{1}' and create_datetime<= '{2}'", cbjobQuery.cb_jobid,cbjobQuery.StartDate,cbjobQuery.EndDate);
                jbcount = _access.getDataTable(str.ToString()).Rows.Count;
                if (jbcount == 0)//程序到此直接跳轉出去
                {
                    return -2;//-2表示不存在該工作編號或已被刪除
                }
                sb.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}' and status=1 and sta_id= '{1}' and create_datetime>='{2}' and create_datetime<= '{3}'", cbjobQuery.cb_jobid, "COM", cbjobQuery.StartDate, cbjobQuery.EndDate);
                if (_access.getDataTable(sb.ToString()).Rows.Count == 0)//判斷該工作編號是否能被蓋帳
                {
                    return -1;
                }
                else//如果該工作編號未被蓋帳,並且處於COM狀態
                {
                    strsql.AppendFormat(@"SELECT cd.iinvd_id,cd.cb_jobid FROM cbjob_master cm LEFT JOIN cbjob_detail cd on cd.cb_jobid=cm.cbjob_id
LEFT JOIN iinvd invd on invd.row_id=cd.iinvd_id 
WHERE cm.cbjob_id='{0}'
and invd.st_qty<> invd.prod_qty 
and cm.status=1 and sta_id= 'COM' and invd.st_qty=0 and cm.create_datetime>='{1}' and cm.create_datetime<= '{2}' group by cd.iinvd_id;", cbjobQuery.cb_jobid, cbjobQuery.StartDate, cbjobQuery.EndDate);
                    DataTable _dt = _access.getDataTable(strsql.ToString());

                    strsqltwo.AppendFormat(@"SELECT cd.iinvd_id,cd.cb_jobid FROM cbjob_master cm LEFT JOIN cbjob_detail cd on cd.cb_jobid=cm.cbjob_id
LEFT JOIN iinvd invd on invd.row_id=cd.iinvd_id 
WHERE cm.cbjob_id='{0}'
and invd.st_qty<> invd.prod_qty 
and cm.status=1 and sta_id= 'COM'and invd.st_qty <> 0 and invd.st_qty <> invd.prod_qty and cm.create_datetime>='{1}' and cm.create_datetime<= '{2}' group by cd.iinvd_id;", cbjobQuery.cb_jobid, cbjobQuery.StartDate, cbjobQuery.EndDate);
                    DataTable _dttwo = _access.getDataTable(strsqltwo.ToString());
                    if (_dt.Rows.Count > 0)//表示有要求要刪除的數據
                    {
                        for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            StringBuilder sbstr = new StringBuilder();
                            StringBuilder sbstr2 = new StringBuilder();
                            sbstr.AppendFormat("select st_qty,plas_loc_id from iinvd where row_id ='{0}';", _dt.Rows[i][0]);
                          
                            DataTable qtdt = _access.getDataTable(sbstr.ToString());
                            sbstr.Clear();
                            sbstr2.AppendFormat("select row_id from iinvd where plas_loc_id='{0}';",qtdt.Rows[0][1]);
                            DataTable qtdt2 = _access.getDataTable(sbstr2.ToString());
                            sbstr.AppendFormat("select st_qty,plas_loc_id,item_id,prod_qty,made_date,cde_dt from iinvd where row_id ='{0}';", _dt.Rows[i][0]);
                            DataTable qtdt3 = _access.getDataTable(sbstr.ToString());
                            sbstr.Clear();
                            int adj_qty = Convert.ToInt32(qtdt3.Rows[0][0]) - Convert.ToInt32(qtdt3.Rows[0][3]);
                            iinvdidstr = iinvdidstr + string.Format(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no, made_dt,cde_dt,adj_qty )
 values ('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}','{7}', '{8}','{9}');", qtdt3.Rows[0][1], qtdt3.Rows[0][2], "PC", qtdt3.Rows[0][3], Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.create_user, jobnumber, Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt3.Rows[0][4])), Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt3.Rows[0][5])), adj_qty);

                            iinvdidstr = iinvdidstr + string.Format("delete from iinvd where row_id='{0}';", _dt.Rows[i][0]);
                            if (qtdt2.Rows.Count < 2)
                            {
                              iinvdidstr=iinvdidstr+string.Format("update iloc set lsta_id='F' where loc_id='{0}' and lcat_id='R';", qtdt.Rows[0][1]);
                            }
                        }
                    }
                   
                    if (_dttwo.Rows.Count > 0)
                    {
                        for (int i = 0; i < _dttwo.Rows.Count; i++)
                        {
                            StringBuilder sbstr = new StringBuilder();
                            sbstr.AppendFormat("select st_qty,plas_loc_id,item_id,prod_qty,made_date,cde_dt from iinvd where row_id ='{0}';", _dttwo.Rows[i][0]);
                            DataTable qtdt = _access.getDataTable(sbstr.ToString());
                            iinvdidstr = iinvdidstr + string.Format("update iinvd set prod_qty=st_qty ,change_user='{0}',change_dtim='{1}' where row_id='{2}' ;", cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), _dttwo.Rows[i][0]);
                            int adj_qty=Convert.ToInt32(qtdt.Rows[0][0])-Convert.ToInt32(qtdt.Rows[0][3]);
                            iinvdidstr = iinvdidstr + string.Format(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no, made_dt,cde_dt,adj_qty )
 values ('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}','{7}', '{8}','{9}');", qtdt.Rows[0][1], qtdt.Rows[0][2], "PC", qtdt.Rows[0][3],Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.create_user, jobnumber,Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][4])),Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][5])), adj_qty);
                            iinvdidstr = iinvdidstr + string.Format(@"INSERT into idiff_count_book(cb_jobid,loc_id,pro_qty,st_qty,create_user,create_time,item_id,made_date,cde_dt)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');"
                               , _dttwo.Rows[i][1], qtdt.Rows[0][1], qtdt.Rows[0][3], qtdt.Rows[0][0], cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(DateTime.Now), qtdt.Rows[0][2], Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][4])), Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][5])));
                        }
                    }
                }
            }
            else
            {
                #region 其他情況
                str.AppendFormat("select cbjob_id from cbjob_master where  sta_id= '{0}' and status=1 and  create_datetime>='{1}' and create_datetime<= '{2}'", "COM", cbjobQuery.StartDate, cbjobQuery.EndDate);
                jbcount = _access.getDataTable(str.ToString()).Rows.Count;
                if (jbcount == 0)//程序到此直接跳轉出去
                {
                    return -3;//-3沒有可蓋帳的
                }
                else//如果存在可蓋帳的 查詢出可蓋帳的信息
                {
                    strsql.AppendFormat(@"SELECT cd.iinvd_id,cd.cb_jobid FROM cbjob_master cm 
LEFT JOIN cbjob_detail cd on cd.cb_jobid=cm.cbjob_id
LEFT JOIN iinvd invd on invd.row_id=cd.iinvd_id 
WHERE invd.st_qty<> invd.prod_qty 
and cm.status=1 and sta_id= 'COM'and invd.st_qty = 0 and cm.create_datetime>='{0}' and cm.create_datetime<= '{1}'  group by cd.iinvd_id;",cbjobQuery.StartDate, cbjobQuery.EndDate);
                    DataTable _dt = _access.getDataTable(strsql.ToString()); 
                    
                    strsqltwo.AppendFormat(@"SELECT cd.iinvd_id,cd.cb_jobid FROM cbjob_master cm 
LEFT JOIN cbjob_detail cd on cd.cb_jobid=cm.cbjob_id
LEFT JOIN iinvd invd on invd.row_id=cd.iinvd_id 
WHERE invd.st_qty<> invd.prod_qty 
and cm.status=1 and sta_id= 'COM'and invd.st_qty <> 0 and invd.st_qty <> invd.prod_qty and  cm.create_datetime>='{0}' and cm.create_datetime<= '{1}' group by cd.iinvd_id;",cbjobQuery.StartDate, cbjobQuery.EndDate);
                    DataTable _dttwo = _access.getDataTable(strsqltwo.ToString());
                    if (_dt.Rows.Count > 0)//表示有要求要刪除的數據
                    {
                        for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            StringBuilder sbstr = new StringBuilder();
                            StringBuilder sbstr2 = new StringBuilder();
                            sbstr.AppendFormat("select st_qty,plas_loc_id from iinvd where row_id ='{0}';", _dt.Rows[i][0]);
                            DataTable qtdt = _access.getDataTable(sbstr.ToString());
                            sbstr.Clear();
                            sbstr2.AppendFormat("select row_id from iinvd where plas_loc_id='{0}';",qtdt.Rows[0][1]);
                            DataTable qtdt2 = _access.getDataTable(sbstr2.ToString());

                            sbstr.AppendFormat("select st_qty,plas_loc_id,item_id,prod_qty,made_date,cde_dt from iinvd where row_id ='{0}';", _dt.Rows[i][0]);
                            DataTable qtdt3 = _access.getDataTable(sbstr.ToString());
                            sbstr.Clear();
                            int adj_qty = Convert.ToInt32(qtdt3.Rows[0][0]) - Convert.ToInt32(qtdt3.Rows[0][3]);
                            iinvdidstr = iinvdidstr + string.Format(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no, made_dt,cde_dt,adj_qty )
 values ('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}','{7}', '{8}','{9}');", qtdt3.Rows[0][1], qtdt3.Rows[0][2], "PC", qtdt3.Rows[0][3], Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.create_user, jobnumber, Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt3.Rows[0][4])), Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt3.Rows[0][5])), adj_qty);

                            iinvdidstr = iinvdidstr + string.Format("delete from iinvd where row_id='{0}';", _dt.Rows[i][0]);
                            if (qtdt2.Rows.Count < 2)
                            {
                                iinvdidstr = iinvdidstr + string.Format("update iloc set lsta_id='F' where loc_id='{0}' and lcat_id='R';", qtdt.Rows[0][1]);
                            }
                        }
                    }               
                    if (_dttwo.Rows.Count > 0)
                    {
                        for (int i = 0; i < _dttwo.Rows.Count; i++)
                        {
                            StringBuilder sbstr = new StringBuilder();
                            sbstr.AppendFormat("select st_qty,plas_loc_id,item_id,prod_qty,made_date,cde_dt from iinvd where row_id ='{0}';", _dttwo.Rows[i][0]);
                            DataTable qtdt = _access.getDataTable(sbstr.ToString());
                            iinvdidstr = iinvdidstr + string.Format("update iinvd set prod_qty=st_qty ,change_user='{0}',change_dtim='{1}' where row_id='{2}' ;", cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), _dttwo.Rows[i][0]);
                            int adj_qty = Convert.ToInt32(qtdt.Rows[0][0]) - Convert.ToInt32(qtdt.Rows[0][3]);
                            iinvdidstr = iinvdidstr + string.Format(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no, made_dt,cde_dt,adj_qty)                                
  values ('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}','{7}', '{8}','{9}');", qtdt.Rows[0][1], qtdt.Rows[0][2], "PC", qtdt.Rows[0][3],Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.create_user, jobnumber, Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][4])),Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][5])), adj_qty );
                            iinvdidstr = iinvdidstr + string.Format(@"INSERT into idiff_count_book(cb_jobid,loc_id,pro_qty,st_qty,create_user,create_time,item_id,made_date,cde_dt)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');"
                                , _dttwo.Rows[i][1], qtdt.Rows[0][1], qtdt.Rows[0][3], qtdt.Rows[0][0], cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(DateTime.Now), qtdt.Rows[0][2], Common.CommonFunction.DateTimeToString(Convert.ToDateTime(qtdt.Rows[0][4])), Common.CommonFunction.DateTimeToString(Convert.ToDateTime( qtdt.Rows[0][5])));
                        }
                    }
                }
                #endregion
            }
            //適當進行修改
            sql.AppendFormat("set sql_safe_updates = 0;");//蓋帳是針對所有的蓋帳
            sql.AppendFormat(iinvdidstr);
            if (!string.IsNullOrEmpty(cbjobQuery.cb_jobid))
            {
                sql.AppendFormat(@"update cbjob_master set sta_id='{0}',create_user='{1}',create_datetime='{2}' where cbjob_id='{3}' and sta_id='COM' and create_datetime>='{4}' and create_datetime<= '{5}';", "END", cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.cb_jobid, Common.CommonFunction.DateTimeToString(cbjobQuery.StartDate), Common.CommonFunction.DateTimeToString(cbjobQuery.EndDate));
            }
            else
            {
                sql.AppendFormat(@"update cbjob_master set sta_id='{0}',create_user='{1}',create_datetime='{2}' where sta_id='COM'and create_datetime>='{3}' and create_datetime<= '{4}' ;", "END", cbjobQuery.create_user, Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), Common.CommonFunction.DateTimeToString(cbjobQuery.StartDate), Common.CommonFunction.DateTimeToString(cbjobQuery.EndDate));
            }
            sql.AppendFormat("set sql_safe_updates = 1;");

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
                throw new Exception("CbjobDetailDao.UpdateCbjobstaid-->" + ex.Message + str.ToString() + sb.ToString() + sql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            if (result > 0)//操作成功
            {
                return jbcount;
            }
            else//操作失敗
            {
                return result;
            }
           
        }

        public int FupanComplete(CbjobDetailQuery cbjobQuery)
        {
            StringBuilder sql = new StringBuilder();
            int result = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder str = new StringBuilder();
            if (!string.IsNullOrEmpty(cbjobQuery.cb_jobid))
            {
                str.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}'and status=1", cbjobQuery.cb_jobid);
                if (_access.getDataTable(str.ToString()).Rows.Count == 0)//程序到此直接跳轉出去
                {
                    return -2;//-2表示不存在該工作編號
                }
                sb.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}' and status=1 and sta_id= '{1}'", cbjobQuery.cb_jobid, "UPD");//是否存在狀態為UPD的可復盤的狀態
                if (_access.getDataTable(sb.ToString()).Rows.Count == 0)//程序到此直接跳轉出去
                {
                    return -1;//表示該狀態不為UPD
                }
                sql.AppendFormat("set sql_safe_updates = 0;");
                sql.AppendFormat(@" update cbjob_master set sta_id='COM',create_user='{0}',create_datetime='{1}' where cbjob_id='{2}';",cbjobQuery.create_user,Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime), cbjobQuery.cb_jobid);
                sql.AppendFormat("set sql_safe_updates = 1;");
            }
            else
            {
                str.AppendFormat("select cbjob_id from cbjob_master where status=1 and sta_id='UPD';");
                if (_access.getDataTable(str.ToString()).Rows.Count == 0)//程序到此直接跳轉出去
                {
                    return -3;//-2表示沒有可以復盤的
                }
                sql.AppendFormat("set sql_safe_updates = 0;");
                sql.AppendFormat(@" update cbjob_master set sta_id='COM',create_user='{0}',create_datetime='{1}' where sta_id='UPD';",cbjobQuery.create_user,Common.CommonFunction.DateTimeToString(cbjobQuery.create_datetime));
                sql.AppendFormat("set sql_safe_updates = 1;");
            }
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
                throw new Exception("CbjobDetailDao.FupanComplete-->" + ex.Message + sb.ToString() + str.ToString() + sql.ToString(), ex);
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

        public string insertsql(CbjobDetail cb)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"INSERT INTO cbjob_detail(cb_jobid,cb_newid,iinvd_id,create_user,change_user,create_datetime,change_datetime,status) 
                VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", cb.cb_jobid, cb.cb_newid, cb.iinvd_id, cb.create_user, cb.chang_user, Common.CommonFunction.DateTimeToString(cb.create_datetime), Common.CommonFunction.DateTimeToString(cb.change_datetime), cb.status);
                return sb.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception("CbjobDetailDao.insertsql-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int InsertSql(string sql)
        {//事物插入數據 same =AseldDao.InsertSql
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
                mySqlCmd.CommandText = sql;
                i = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("AseldDao.InsertSql-->" + ex.Message + ",sql:" + mySqlCmd.CommandText, ex);
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

        /// <summary>
        /// chaojie1124j add by 2015-12-09 05:32PM 實現料位盤點工作
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public DataTable GetDetailTable(CbjobDetail cb)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat(" select row_id from cbjob_master where cbjob_id='{0}' and sta_id<>'END' ", cb.cb_jobid);
                DataTable dt_result = _access.getDataTable(str.ToString());
                sql.Append(" SELECT cd.iinvd_id,pe.pwy_dte_ctl,cd.cb_jobid,ii.prod_qty,pi.spec_id_1,p.spec_title_1,p.spec_title_2,pi.spec_id_2,p.product_name, ");//icb.st_qty,icb.pro_qty,
                if (dt_result.Rows.Count > 0)
                {
                   sql.Append(" ii.plas_loc_id as loc_id,ii.item_id from cbjob_detail cd ");
                    sql.Append(" left join idiff_count_book icb on icb.cb_jobid=cd.cb_jobid ");
                    sql.Append(" inner join iinvd ii on ii.row_id=cd.iinvd_id ");
                    sql.Append(" inner join product_item pi on pi.item_id=ii.item_id ");
                    sql.Append(" inner join product p on pi.product_id=p.product_id ");
                    sql.Append(" left join product_ext pe on pe.item_id=pi.item_id ");
                    sql.Append(" where 1=1 ");
                }
                else
                {  
                    sql.Append(" icb.loc_id,icb.item_id from cbjob_detail cd ");
                    sql.Append(" left join idiff_count_book icb on icb.cb_jobid=cd.cb_jobid ");
                    sql.Append(" inner join iinvd ii on ii.row_id=cd.iinvd_id ");
                    sql.Append(" inner join product_item pi on pi.item_id=icb.item_id ");
                    sql.Append(" inner join product p on pi.product_id=p.product_id ");
                    sql.Append(" left join product_ext pe on pe.item_id=pi.item_id ");
                    sql.Append(" where 1=1 ");
                   
                }

               
               
                if (!string.IsNullOrEmpty(cb.cb_jobid))
                {
                    sql.AppendFormat(" and cd.cb_jobid='{0}' ", cb.cb_jobid);
                }
                DataTable dtResult = _access.getDataTable(sql.ToString());
                IProductSpecImplDao _specDao = new ProductSpecDao(connStr);
                foreach (DataRow dr in dtResult.Rows)
                {
                    if (!string.IsNullOrEmpty(dr["spec_id_1"].ToString()))
                    {
                        ProductSpec spec1 = _specDao.query(Convert.ToInt32(dr["spec_id_1"].ToString()));
                        if (spec1 != null)
                        {
                            dr["spec_title_1"] = string.IsNullOrEmpty(dr["spec_title_1"].ToString()) ? "" : dr["spec_title_1"] + ":" + spec1.spec_name;
                        }
                    }
                    if (!string.IsNullOrEmpty(dr["spec_id_2"].ToString()))
                    {
                        ProductSpec spec2 = _specDao.query(Convert.ToInt32(dr["spec_id_2"].ToString()));
                        if (spec2 != null)
                        {
                            dr["spec_title_2"] = string.IsNullOrEmpty(dr["spec_title_2"].ToString()) ? "" : dr["spec_title_2"] + ":" + spec2.spec_name;
                        }
                    }
                   
                   
                    dr["spec_title_1"] = string.IsNullOrEmpty(dr["spec_title_1"].ToString()) ? "" : dr["spec_title_1"].ToString() + "  " + dr["spec_title_2"];
                }
                return dtResult;
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailDao-->GetDetailTable-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
