using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Common;
using System.Data;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class IialgDao : IIialgImplDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public IialgDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
              this.connStr = connectionString;
        }
        public int insertiialg(IialgQuery q)
        {
            StringBuilder sql = new StringBuilder();
            try
            {//適當進行修改
                sql.AppendFormat(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no,po_id,made_dt,cde_dt,adj_qty,remarks,c_made_dt,c_cde_dt )
values ('{0}','{1}','{2}','{3}','{4}','{5}', '{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}') ",q.loc_id.ToString().ToUpper(),q.item_id,q.iarc_id,q.qty_o,
CommonFunction.DateTimeToString(q.create_dtim),q.create_user, q.doc_no,q.po_id,
CommonFunction.DateTimeToString(q.made_dt),CommonFunction.DateTimeToString(q.cde_dt),q.adj_qty,q.remarks,CommonFunction.DateTimeToString(q.c_made_dt),CommonFunction.DateTimeToString(q.c_cde_dt));
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" IialgDao-->insertiialg-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int HuiruInsertiialg(System.Data.DataRow[] dr, out int iialgtotal, out int iinvdtotal)
        {
            int result = 0;
            iialgtotal = 0;
            iinvdtotal = 0;
            DataTable dt = new DataTable();
            Iinvd invd = new Iinvd();
            IialgQuery ialg = new IialgQuery();
            StringBuilder sb = new StringBuilder();
            StringBuilder invdsb = new StringBuilder();
            StringBuilder ialgsb = new StringBuilder();
            ialg.doc_no = "K" + DateTime.Now.ToString("yyyyMMddHHmmss");//庫調單號
            for (int i = 1; i < dr.Length; i++)
            {
                string row_id = string.Empty;
                row_id = dr[i][0].ToString().Trim();
                if (!string.IsNullOrEmpty(row_id))
                {
                    invd.row_id = int.Parse(dr[i][0].ToString().Trim());
                    invd.prod_qty = int.Parse(dr[i][5].ToString().Trim());
                    invdsb.AppendFormat(" update iinvd set prod_qty ='{0}' where row_id='{1}'; ", invd.prod_qty, invd.row_id);
                    iialgtotal++;
                    if (!string.IsNullOrEmpty(dr[i][4].ToString().Trim()))//當存在復盤值時
                    {
                        ialg.row_id = int.Parse(dr[i][0].ToString().Trim());
                        ialg.qty_o = int.Parse(dr[i][5].ToString().Trim());
                        ialg.adj_qty = int.Parse(dr[i][4].ToString().Trim()) - int.Parse(dr[i][5].ToString().Trim());
                        ialg.loc_id = dr[i][1].ToString().Trim();
                        ialg.item_id = Convert.ToUInt32(dr[i][3].ToString().Trim());
                        if (!string.IsNullOrEmpty(dr[i][10].ToString().Trim()))
                        {
                            ialg.made_dttostr = dr[i][10].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(dr[i][11].ToString().Trim()))
                        {
                            ialg.cde_dttostr = dr[i][11].ToString().Trim();
                        }
                        ialg.create_dtim = DateTime.Now;
                        ialg.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                        ialg.iarc_id = "PC";
                        ialg.po_id = string.Empty;
                        ialgsb.AppendFormat(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no,po_id, made_dt,cde_dt,adj_qty)
 values ('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}','{7}','{8}','{9}','{10}'); ", ialg.loc_id, ialg.item_id, ialg.iarc_id, ialg.qty_o, CommonFunction.DateTimeToString(ialg.create_dtim),
  ialg.create_user, ialg.doc_no, ialg.po_id, ialg.made_dttostr, ialg.cde_dttostr, ialg.adj_qty);
                        iinvdtotal++;
                    }
                }
            }
            sb.AppendFormat(invdsb.ToString()+ialgsb.ToString());
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
           
                mySqlCmd.CommandText = sb.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("IialgDao.HuiruInsertiialg-->" + ex.Message + sb.ToString(), ex);
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

        public List<IialgQuery> GetIialgList(IialgQuery q, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append(@"select ia.doc_no,ia.row_id, ip.loc_id,ia.loc_id as loc_R,ia.item_id,p.product_name,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz
        ,ia.made_dt,ia.cde_dt,ia.qty_o,ia.adj_qty,ia.iarc_id,ia.create_dtim ,m.user_username AS name,ia.po_id,ia.remarks,ia.c_made_dt,ia.c_cde_dt  from iialg ia 
        left join product_item pi on ia.item_id=pi.item_id  
        left join product p on p.product_id=pi.product_id 
        LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id 
        LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id 
        LEFT JOIN iinvd i on ia.item_id = i.item_id AND ia.made_dt=i.made_date  AND ia.loc_id=i.plas_loc_id
        LEFT JOIN iplas ip on ia.item_id = ip.item_id
        LEFT JOIN manage_user m ON ia.create_user=m.user_id where 1=1");
                if (q.item_id>0)
                {
                    sql.AppendFormat(" and ia.item_id='{0}' ", q.item_id);
                }
                if (!string.IsNullOrEmpty(q.loc_id))
                {
                    sql.AppendFormat(" and  ia.loc_id='{0}' ", q.loc_id);
                }
                if (!string.IsNullOrEmpty(q.po_id))
                {
                    sql.AppendFormat(" and  ia.po_id='{0}' ", q.po_id);
                }
                //if (q.iarc_id.ToString() != "0")
                //{
                //    sql.AppendFormat(" and iarc_id='{0}' ", q.iarc_id);
                //}


                if (q.starttime > DateTime.MinValue)
                {
                    sql.AppendFormat(" and ia.create_dtim>='{0}' ", q.starttime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (q.endtime > DateTime.MinValue)
                {
                    sql.AppendFormat(" and ia.create_dtim<='{0}' ", q.endtime.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(q.doc_no))//by zhaozhi0623j add 20151006 用於庫存調整單號查詢
                {
                    sql.AppendFormat(" and  ia.doc_no='{0}' ", q.doc_no);
                }
                if (q.doc_userid != 0)//by zhaozhi0623j add 20151006 用於庫存調整管理員查詢
                {
                    sql.AppendFormat(" and  ia.create_user='{0}' ", q.doc_userid);
                }
                if (q.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat(" order by  ia.row_id limit {0},{1};", q.Start, q.Limit);
                }
                return _access.getDataTableForObj<IialgQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IialgDao-->GetIialgList-->" + ex.Message + sql.ToString(), ex);
            }
        }



        public List<IialgQuery> GetExportIialgList(IialgQuery q)
        {
            StringBuilder sql = new StringBuilder();
        
            try
            {
                sql.Append(@"select ia.doc_no,ia.row_id, ip.loc_id,ia.loc_id as loc_R,ia.item_id,p.product_name,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz
        ,ia.made_dt,ia.cde_dt,ia.qty_o,ia.adj_qty,ia.iarc_id,ia.create_dtim ,m.user_username AS name,ia.po_id,ia.remarks,ia.c_made_dt,ia.c_cde_dt  from iialg ia 
        left join product_item pi on ia.item_id=pi.item_id  
        left join product p on p.product_id=pi.product_id 
        LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id 
        LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id 
        LEFT JOIN iinvd i on ia.item_id = i.item_id AND ia.made_dt=i.made_date  AND ia.loc_id=i.plas_loc_id
        LEFT JOIN iplas ip on ia.item_id = ip.item_id
        LEFT JOIN manage_user m ON ia.create_user=m.user_id where 1=1");
                if (q.item_id > 0)
                {
                    sql.AppendFormat(" and ia.item_id='{0}' ", q.item_id);
                }
                if (!string.IsNullOrEmpty(q.loc_id))
                {
                    sql.AppendFormat(" and  ia.loc_id='{0}' ", q.loc_id);
                }
                if (!string.IsNullOrEmpty(q.po_id))
                {
                    sql.AppendFormat(" and  ia.po_id='{0}' ", q.po_id);
                }
                //if (q.iarc_id.ToString() != "0")
                //{
                //    sql.AppendFormat(" and iarc_id='{0}' ", q.iarc_id);
                //}
                if (q.starttime > DateTime.MinValue)
                {
                    sql.AppendFormat(" and ia.create_dtim>='{0}' ", q.starttime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (q.endtime > DateTime.MinValue)
                {
                    sql.AppendFormat(" and ia.create_dtim<='{0}' ", q.endtime.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(q.doc_no))//by zhaozhi0623j add 20151006 用於庫存調整單號查詢
                {
                    sql.AppendFormat(" and  ia.doc_no='{0}' ", q.doc_no);
                }
                if (q.doc_userid != 0)//by zhaozhi0623j add 20151006 用於庫存調整管理員查詢
                {
                    sql.AppendFormat(" and  ia.create_user='{0}' ", q.doc_userid);
                }
               
                    sql.AppendFormat(" order by  ia.row_id ;");
              
                return _access.getDataTableForObj<IialgQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IialgDao-->GetExportIialgList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /**
         *chaojie1124j添加于2515/08/17
         *實現RF理貨庫存不足而進行調整的功能  
         */
        public int addIialgIstock(IialgQuery q)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr=new StringBuilder();

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
                sql.AppendFormat("select pwy_dte_ctl,cde_dt_incr from product_ext where item_id='{0}';",q.item_id);//查看是否是有效期控管的商品
                DataTable _dtProExt=_access.getDataTable(sql.ToString());
                sql.Clear(); 
                DateTime cde_dt = DateTime.Now;
                sql.AppendFormat("select iinvd.row_id from iinvd  left join iloc on iloc.loc_id=iinvd.plas_loc_id  where iinvd.item_id='{0}' and iinvd.made_date ='{1}' and iloc.lcat_id='S' and iinvd.ista_id='A' ; ", q.item_id, CommonFunction.DateTimeToString(q.made_dt).Substring(0, 10));//查詢此數據未鎖定的
                
                DataTable _dtIinvd = _access.getDataTable(sql.ToString());
                sql.Clear();
                sql.AppendFormat("select iinvd.row_id from iinvd  left join iloc on iloc.loc_id=iinvd.plas_loc_id  where iinvd.item_id='{0}' and iinvd.made_date ='{1}' and iloc.lcat_id='S' and iinvd.ista_id='H' ; ", q.item_id, CommonFunction.DateTimeToString(q.made_dt).Substring(0, 10));//查詢此數據已鎖定的
                DataTable _Iinvd = _access.getDataTable(sql.ToString());
                sql.Clear();
                sql.AppendFormat("select iinvd.row_id from iinvd left join iloc on iloc.loc_id=iinvd.plas_loc_id where iinvd.item_id='{0}' and iinvd.made_date ='{1}' and iloc.lcat_id='S' and iinvd.ista_id='H' ; ", q.item_id, CommonFunction.DateTimeToString(cde_dt).Substring(0, 10));//查詢今日庫存是否已鎖
                DataTable _TodayIinvd = _access.getDataTable(sql.ToString());
                sql.Clear();
                sql.AppendFormat("select iinvd.row_id from iinvd left join iloc on iloc.loc_id=iinvd.plas_loc_id  where iinvd.item_id='{0}' and iinvd.made_date ='{1}' and iloc.lcat_id='S' and iinvd.ista_id='A' ; ", q.item_id, CommonFunction.DateTimeToString(cde_dt).Substring(0, 10));//查詢今日庫存是否已鎖
                DataTable _dtTodayIinvd = _access.getDataTable(sql.ToString());
                sql.Clear();
                int qty_o = q.qty_o;
                #region 新改
                if (_dtProExt.Rows.Count > 0)//判斷是否有效期控管
                {
                    if (_dtProExt.Rows[0]["pwy_dte_ctl"].ToString().ToUpper().Equals("Y"))/*查询是否是有效期控管的商品*/
                    {
                        cde_dt = q.made_dt.AddDays(Convert.ToInt32(_dtProExt.Rows[0]["cde_dt_incr"]));//有效日期是製造日期加上有效期天數
                    }
                    if (_Iinvd.Rows.Count > 0)//查詢此數據已鎖定的
                    {
                        return 2;//進入的庫存已鎖，不能庫調
                    }
                    else //庫存未鎖
                    {
                        if (_dtIinvd.Rows.Count > 0)//存在此數據
                        {
                            sqlstr.AppendFormat(" update iinvd set prod_qty='{0}',change_user='{1}',change_dtim='{2}' where row_id='{3}';", q.pnum, q.create_user, CommonFunction.DateTimeToString(DateTime.Now), _dtIinvd.Rows[0][0]);//RF理貨需要的庫存
                            mySqlCmd.CommandText = sqlstr.ToString();
                            mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                        }
                        else //不存在此數據
                        {
                            qty_o = 0;
                            IinvdQuery ivd = new IinvdQuery();
                            sqlstr.AppendLine(@"insert into iinvd (lic_plt_id,dc_id,whse_id,po_id,plas_id,prod_qty,");
                            sqlstr.AppendLine(@"rcpt_id,lot_no,hgt_used,create_user,create_dtim,");
                            sqlstr.AppendLine(@"change_user,change_dtim,cde_dt,ista_id,receipt_dtim,");
                            sqlstr.AppendLine(@"stor_ti,stor_hi,inv_pos_cat,qity_id,");
                            sqlstr.AppendLine(@"plas_loc_id,item_id,plas_prdd_id,made_date) VALUES (");
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.lic_plt_id, ivd.dc_id, ivd.whse_id, ivd.po_id);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.plas_id, q.pnum - q.qty_o, ivd.rcpt_id, ivd.lot_no);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.hgt_used, q.create_user, CommonFunction.DateTimeToString(DateTime.Now), q.create_user);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(cde_dt), "A", CommonFunction.DateTimeToString(ivd.receipt_dtim));
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.stor_ti, ivd.stor_hi, ivd.inv_pos_cat, ivd.qity_id);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}');", q.loc_id.ToString().ToUpper(), q.item_id, ivd.plas_prdd_id, CommonFunction.DateTimeToString(DateTime.Now));
                            mySqlCmd.CommandText = sqlstr.ToString();
                            mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                        }
                    }
                }
                else //非有效期控管的商品
                {
                    q.made_dt = DateTime.Now;
                    if (_TodayIinvd.Rows.Count > 0)//非有效期控管的今天的是否鎖上
                    {
                        return 2;
                    }
                    else 
                    {
                        if (_dtTodayIinvd.Rows.Count > 0)//有今天上架的了，那就更改
                        {
                            sqlstr.AppendFormat(" update iinvd set prod_qty='{0}',change_user='{1}',change_dtim='{2}' where row_id='{3}';", q.pnum, q.create_user, CommonFunction.DateTimeToString(DateTime.Now), _dtTodayIinvd.Rows[0][0]);//RF理貨需要的庫存
                            mySqlCmd.CommandText = sqlstr.ToString();
                            mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                        }
                        else
                        {
                            qty_o = 0;
                            IinvdQuery ivd = new IinvdQuery();
                            sqlstr.AppendLine(@"insert into iinvd (lic_plt_id,dc_id,whse_id,po_id,plas_id,prod_qty,");
                            sqlstr.AppendLine(@"rcpt_id,lot_no,hgt_used,create_user,create_dtim,");
                            sqlstr.AppendLine(@"change_user,change_dtim,cde_dt,ista_id,receipt_dtim,");
                            sqlstr.AppendLine(@"stor_ti,stor_hi,inv_pos_cat,qity_id,");
                            sqlstr.AppendLine(@"plas_loc_id,item_id,plas_prdd_id,made_date) VALUES (");
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.lic_plt_id, ivd.dc_id, ivd.whse_id, ivd.po_id);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.plas_id, q.pnum - q.qty_o, ivd.rcpt_id, ivd.lot_no);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.hgt_used, q.create_user, CommonFunction.DateTimeToString(DateTime.Now), q.create_user);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(cde_dt), "A", CommonFunction.DateTimeToString(ivd.receipt_dtim));
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.stor_ti, ivd.stor_hi, ivd.inv_pos_cat, ivd.qity_id);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}');", q.loc_id.ToString().ToUpper(), q.item_id, ivd.plas_prdd_id, CommonFunction.DateTimeToString(DateTime.Now));
                            mySqlCmd.CommandText = sqlstr.ToString();
                            mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                        }
                    }

                }

                #endregion
                sqlstr.Append(@"insert into iialg (loc_id,item_id,iarc_id,qty_o,create_dtim,create_user,doc_no,po_id,made_dt,cde_dt,adj_qty,remarks,c_made_dt,c_cde_dt )values (");
                sqlstr.AppendFormat(" '{0}','{1}','{2}','{3}' ", q.loc_id.ToString().ToUpper(), q.item_id, "PC", qty_o);//qty_o=0;新增的時候
                sqlstr.AppendFormat(" ,'{0}','{1}', '{2}','{3}' ", CommonFunction.DateTimeToString(DateTime.Now), q.create_user,"", "");
                sqlstr.AppendFormat(" ,'{0}','{1}','{2}','{3}'", CommonFunction.DateTimeToString(q.made_dt), CommonFunction.DateTimeToString(cde_dt), q.pnum - q.qty_o, "庫調:"+q.order_id);
                sqlstr.AppendFormat(" ,'{0}','{1}');", CommonFunction.DateTimeToString(q.c_made_dt), CommonFunction.DateTimeToString(q.c_cde_dt));
                mySqlCmd.CommandText = sqlstr.ToString();

                mySqlCmd.ExecuteNonQuery();
                sqlstr.Clear();
                sql.AppendFormat("select sum(prod_qty) as prod_qty from iinvd where item_id='{0}' and ista_id='A'", q.item_id);
                DataTable _dtprod_qty = _access.getDataTable(sql.ToString());
                if (string.IsNullOrEmpty(_dtprod_qty.Rows[0][0].ToString()))
                {
                    _dtprod_qty.Rows[0][0]=0;
                }
                sql.Clear();
                sqlstr.AppendFormat("insert into istock_change(sc_trans_id,sc_cd_id,item_id,sc_trans_type,sc_num_old,sc_num_chg,sc_num_new,sc_time,sc_user,sc_note,sc_istock_why) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", q.order_id, "", q.item_id, 3, Convert.ToInt32(_dtprod_qty.Rows[0][0]), (q.pnum - q.qty_o), Convert.ToInt32(_dtprod_qty.Rows[0][0]) - q.qty_o + q.pnum, CommonFunction.DateTimeToString(DateTime.Now), q.create_user, "理貨庫調", 4);
                mySqlCmd.CommandText = sqlstr.ToString();

                mySqlCmd.ExecuteNonQuery();
                sqlstr.Clear();
                mySqlCmd.Transaction.Commit();
                return 100;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception(" IialgDao-->addIialgIstock-->" + ex.Message + sql.ToString(), ex);
            }

        }
    }
}
