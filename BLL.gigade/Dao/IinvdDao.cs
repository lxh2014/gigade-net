using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
/*
 *創建者：張瑜
 *創建時間：2014-11-4
 *v1修改內容：chaojie1124j 添加查詢商品源料位列表或目的料位方法；
 * */
namespace BLL.gigade.Dao
{
    public class IinvdDao : IinvdImplDao
    {
        private IDBAccess _access;
        private string mySqlConnectionString;

        public IinvdDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }

        #region 上架料位列表頁
        /// <summary>
        /// 獲取上架料位列表信息
        /// </summary>
        /// <param name="ivd">上架料位對象</param>
        /// <param name="totalCount">返回的數據總條數</param>
        /// <returns>上架料位列表</returns>
        public List<IinvdQuery> GetIinvdList(Model.Query.IinvdQuery ivd, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
             StringBuilder sb = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sb.Append("select count(ii.row_id) as totalcounts from iinvd ii where 1=1 ");
                sql.Append(@"select Tp_table.parameterName as qity_name,ii.row_id,lic_plt_id,ii.dc_id,ii.whse_id,ii.made_date,po_id,prod_qty,rcpt_id,lot_no,hgt_used,ii.create_user,ii.create_dtim,ii.change_user,ii.change_dtim,cde_dt,ista_id,receipt_dtim,stor_ti,stor_hi,inv_pos_cat,qity_id,plas_loc_id,ii.item_id,plas_prdd_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,ip.loc_id,pe.cde_dt_var,pe.cde_dt_shp,pe.pwy_dte_ctl,pe.cde_dt_incr,us.user_username as user_name,vb.vendor_id from iinvd ii");
                sql.Append(" left join iplas ip on ii.item_id=ip.item_id ");
                // sql.Append(" left join iupc iu on iu.item_id=ii.item_id ");
                sql.Append(" left join product_item pi on ii.item_id=pi.item_id ");
                sql.Append(" left JOIN product_ext pe ON ii.item_id=pe.item_id ");
                sql.Append(" LEFT JOIN (SELECT parameterCode,parameterName from t_parametersrc where parameterType='loc_lock_msg') as Tp_table on ii.qity_id=Tp_table.parameterCode ");
                sql.Append(" left join product p on p.product_id=pi.product_id  ");
                sql.Append(" LEFT JOIN  manage_user us on ii.create_user=us.user_id  ");
                sql.Append(" left join vendor_brand vb on p.brand_id=vb.brand_id where 1=1 ");
                if (!string.IsNullOrEmpty(ivd.plas_loc_id))
                {
                    sbwhere.AppendFormat(" and ii.plas_loc_id='{0}' ", ivd.plas_loc_id.ToString().ToUpper());
                }
                if (ivd.serch_type != 0)
                {
                    if (!string.IsNullOrEmpty(ivd.serchcontent))
                    {
                        sbwhere.AppendFormat(" and ii.item_id in ({0})  ", ivd.serchcontent);
                        //switch (ivd.serch_type)
                        //{
                        //    case 1:
                        //        sbwhere.AppendFormat(" and ii.plas_loc_id in (select loc_id from iloc where loc_id='{0}' or hash_loc_id='{0}' )  ", ivd.serchcontent);
                        //        break;
                        //    case 2:
                        //        sbwhere.AppendFormat(" and ii.item_id=(select item_id from iupc iu where iu.upc_id ='{0}') ", ivd.serchcontent);
                        //        break;
                        //    case 3:
                        //        sbwhere.AppendFormat(" and ii.item_id in ({0})  ", ivd.serchcontent);
                        //        break;
                        //    default:
                        //        break;
                        //}
                    }
                }
                DateTime dt = DateTime.Parse("1970-01-02 08:00:00");
                if (!string.IsNullOrEmpty(ivd.starttime.ToString()) && dt < ivd.starttime)
                {
                    sbwhere.AppendFormat(" and ii.create_dtim>'{0}' ", CommonFunction.DateTimeToString(ivd.starttime));
                }
                if (!string.IsNullOrEmpty(ivd.endtime.ToString()) && dt < ivd.endtime)
                {
                    sbwhere.AppendFormat(" and ii.create_dtim<'{0}' ", CommonFunction.DateTimeToString(ivd.endtime));
                }
                //if (!string.IsNullOrEmpty(ivd.serchcontent))
                //{
                //    sbwhere.AppendFormat(" and ii.item_id in ({0})  ", ivd.serchcontent);
                //}

                if (ivd.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sb.ToString() + sbwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    if (!string.IsNullOrEmpty(ivd.ista_id))
                    {//理貨員工作用到庫存
                        sbwhere.AppendFormat(" and ii.ista_id='{0}'  order by ii.cde_dt ;", ivd.ista_id);
                    }
                    else
                    {//收貨上架列表頁
                        sbwhere.AppendFormat(" group by ii.row_id order by ii.row_id limit {0},{1};", ivd.Start, ivd.Limit);
                    }
                }
                return _access.getDataTableForObj<IinvdQuery>(sql.ToString() + sbwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetIupcList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 根據產品條碼獲取數據
        /// <summary>
        /// 根據產品條碼獲取庫存數據
        /// </summary>
        /// <param name="ivd">上架料位對象</param>
        /// <param name="totalCount">返回的數據總條數</param>
        /// <returns>上架料位列表</returns>
        public List<IinvdQuery> GetIinvdListByItemid(Model.Query.IinvdQuery ivd, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sb.Append("select count(ii.row_id) as totalcounts from iinvd ii  left join iupc iu on iu.item_id=ii.item_id where 1=1 ");
                sql.Append(@"select Tp_table.parameterName as qity_name,ii.row_id,lic_plt_id,ii.dc_id,ii.whse_id,ii.made_date,po_id,prod_qty,rcpt_id,lot_no,hgt_used,ii.create_user,ii.create_dtim,ii.change_user,ii.change_dtim,cde_dt,ista_id,receipt_dtim,stor_ti,stor_hi,inv_pos_cat,qity_id,plas_loc_id,ii.item_id,plas_prdd_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,ip.loc_id,pe.cde_dt_var,pe.cde_dt_shp,pe.pwy_dte_ctl,pe.cde_dt_incr,us.user_username as user_name,vb.vendor_id from iinvd ii");
                sql.Append(" left join iplas ip on ii.item_id=ip.item_id ");
                //sql.Append(" left join iupc iu on iu.item_id=ii.item_id ");
                sql.Append(" left join product_item pi on ii.item_id=pi.item_id ");
                sql.Append(" left JOIN product_ext pe ON ii.item_id=pe.item_id ");
                sql.Append(" LEFT JOIN (SELECT parameterCode,parameterName from t_parametersrc where parameterType='loc_lock_msg') as Tp_table on ii.qity_id=Tp_table.parameterCode ");
                sql.Append(" left join product p on p.product_id=pi.product_id  ");
                sql.Append(" LEFT JOIN  manage_user us on ii.create_user=us.user_id  ");
                sql.Append(" left join vendor_brand vb on p.brand_id=vb.brand_id where 1=1 ");
                if (ivd.item_id!=0)
                {
                    sbwhere.AppendFormat(" and ii.item_id='{0}' ", ivd.item_id);
                }
                DateTime dt = DateTime.Parse("1970-01-02 08:00:00");
                if (!string.IsNullOrEmpty(ivd.starttime.ToString()) && dt < ivd.starttime)
                {
                    sbwhere.AppendFormat(" and ii.create_dtim>'{0}' ", CommonFunction.DateTimeToString(ivd.starttime));
                }
                if (!string.IsNullOrEmpty(ivd.endtime.ToString()) && dt < ivd.endtime)
                {
                    sbwhere.AppendFormat(" and ii.create_dtim<'{0}' ", CommonFunction.DateTimeToString(ivd.endtime));
                }

                if (ivd.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sb.ToString() + sbwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    if (!string.IsNullOrEmpty(ivd.ista_id))
                    {//理貨員工作用到庫存
                        sbwhere.AppendFormat(" and ii.ista_id='{0}'  order by ii.cde_dt ;", ivd.ista_id);
                    }
                    else
                    {//收貨上架列表頁
                        sbwhere.AppendFormat(" group by ii.row_id order by ii.row_id limit {0},{1};", ivd.Start, ivd.Limit);
                    }
                }
                return _access.getDataTableForObj<IinvdQuery>(sql.ToString() + sbwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetIinvdListByItemid-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 新增
        public int Insert(Iinvd ivd)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"insert into iinvd (lic_plt_id,dc_id,whse_id,po_id,plas_id,prod_qty,");
            sql.AppendLine(@"rcpt_id,lot_no,hgt_used,create_user,create_dtim,");
            sql.AppendLine(@"change_user,change_dtim,cde_dt,ista_id,receipt_dtim,");
            sql.AppendLine(@"stor_ti,stor_hi,inv_pos_cat,qity_id,");
            sql.AppendLine(@"plas_loc_id,item_id,plas_prdd_id,made_date) VALUES (");
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.lic_plt_id, ivd.dc_id, ivd.whse_id, ivd.po_id);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.plas_id, ivd.prod_qty, ivd.rcpt_id, ivd.lot_no);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.hgt_used, ivd.create_user, CommonFunction.DateTimeToString(ivd.create_dtim), ivd.create_user);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", CommonFunction.DateTimeToString(ivd.change_dtim), CommonFunction.DateTimeToString(ivd.cde_dt), ivd.ista_id, CommonFunction.DateTimeToString(ivd.receipt_dtim));
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", ivd.stor_ti, ivd.stor_hi, ivd.inv_pos_cat, ivd.qity_id);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}')", ivd.plas_loc_id.ToString().ToUpper(), ivd.item_id, ivd.plas_prdd_id, CommonFunction.DateTimeToString(ivd.made_date));
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Insert-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 編輯
        public int Upd(Iinvd m)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            if (m.prod_qty > 0)
            {
                sql.Append("set sql_safe_updates = 0;");
                sql.AppendFormat("UPDATE iinvd SET prod_qty='{2}',change_dtim='{3}',change_user='{5}' WHERE plas_loc_id='{0}' and cde_dt='{4}' AND item_id='{1}';", m.plas_loc_id, m.item_id, m.prod_qty, CommonFunction.DateTimeToString(m.change_dtim), CommonFunction.DateTimeToString(m.cde_dt), m.create_user);
                sql.Append("set sql_safe_updates = 1;");
            }
            else
            {
                sb.AppendFormat("select item_id from iinvd where plas_loc_id='{0}' AND item_id='{1}';", m.plas_loc_id, m.item_id);
                DataTable dt = _access.getDataTable(sb.ToString());
                if (dt.Rows.Count > 1)
                {
                    sql.AppendFormat("Delete from iinvd where plas_loc_id='{0}' AND item_id='{1}' AND cde_dt='{2}';", m.plas_loc_id, m.item_id, CommonFunction.DateTimeToString(m.cde_dt));
                }
                else
                {
                    sql.Append("set sql_safe_updates = 0;");
                    sql.AppendFormat("UPDATE iloc SET Ista_id='F'  where lcat_id='R' AND plas_loc_id='{0}';", m.plas_loc_id);
                    sql.Append("set sql_safe_updates = 1;");
                    sql.AppendFormat("Delete from iinvd where plas_loc_id='{0}' AND item_id='{1}' AND cde_dt='{2}';", m.plas_loc_id, m.item_id, CommonFunction.DateTimeToString(m.cde_dt));
                }
            }
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Upd-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdProdqty(Iinvd m)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (m.prod_qty > 0)
                {
                    sql.Append("set sql_safe_updates = 0;");
                    sql.AppendFormat("UPDATE iinvd SET prod_qty='{1}',change_dtim='{2}',change_user='{3}' WHERE row_id='{0}';", m.row_id, m.prod_qty, CommonFunction.DateTimeToString(m.change_dtim), m.change_user);
                    sql.Append("set sql_safe_updates = 1;");
                }
                else
                {
                    sb.AppendFormat("SELECT plas_loc_id from iinvd where plas_loc_id=(select plas_loc_id from iinvd where row_id='{0}');", m.row_id);
                    DataTable dt = _access.getDataTable(sb.ToString());
                    if (dt.Rows.Count > 1)
                    {
                        sql.AppendFormat("Delete from iinvd where row_id='{0}';", m.row_id);
                    }
                    else
                    {
                        sql.Append("set sql_safe_updates = 0;");
                        sql.AppendFormat("UPDATE iloc SET lsta_id='F'  where lcat_id='R' AND loc_id='{0}';", dt.Rows[0]["plas_loc_id"].ToString());
                        sql.Append("set sql_safe_updates = 1;");
                        sql.AppendFormat("Delete from iinvd where row_id='{0}';", m.row_id);
                    }
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->UpdProdqty-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string InsertIinvdLog(IinvdLog il)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into iinvd_log  (nvd_id,from_num,change_num,create_user,create_date) VALUES('{0}','{1}','{2}','{3}','{4}');", il.nvd_id, il.from_num, il.change_num, il.create_user, CommonFunction.DateTimeToString(il.create_date));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->InsertIinvdLog-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 查詢或判斷其他數據
        public DataTable Getprodu(int id)
        {//根據item_id獲取數據
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT vb.vendor_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,ps.spec_name,ps2.spec_name,i.loc_id,pe.cde_dt_var,pe.cde_dt_shp,pe.pwy_dte_ctl,pe.cde_dt_incr from product_item pi 
LEFT JOIN product p ON pi.product_id=p.product_id 
LEFT JOIN product_spec ps ON pi.spec_id_1= ps.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2= ps2.spec_id
left join vendor_brand vb on p.brand_id=vb.brand_id
LEFT JOIN iplas i ON pi.item_id = i.item_id 
LEFT JOIN product_ext pe ON pi.item_id=pe.item_id where pi.item_id='{0}'  ;", id);
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Getprodu-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public DataTable Getprodubybar(string id)
        {//根據條碼獲取數據
            StringBuilder sql = new StringBuilder();
            StringBuilder sbStr = new StringBuilder();
            sql.AppendFormat(@"SELECT vb.vendor_id,i.upc_id,pi.item_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,ps.spec_name,ps2.spec_name,ip.loc_id,pe.cde_dt_var,pe.cde_dt_shp,pe.pwy_dte_ctl,pe.cde_dt_var,pe.cde_dt_incr 
from product_item pi
left JOIN iupc i on i.item_id=pi.item_id
LEFT JOIN product p ON pi.product_id=p.product_id 
LEFT JOIN product_spec ps ON pi.spec_id_1= ps.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2= ps2.spec_id
left join vendor_brand vb on p.brand_id=vb.brand_id
LEFT JOIN iplas ip ON pi.item_id=ip.item_id
LEFT JOIN product_ext pe ON pi.item_id=pe.item_id  where 1=1 ");
            sbStr.AppendFormat("select item_id from product_item where item_id='{0}';", id);
            DataTable _dtresult = _access.getDataTable(sbStr.ToString());
            if (_dtresult.Rows.Count > 0)
            {
                sql.AppendFormat(" and pi.item_id='{0}' ", id);
            }
            else
            {
                sql.AppendFormat(" and pi.item_id =(select item_id from iupc where upc_id='{0}'limit 1 )", id);
            }          
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Getprodubybar-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Islocid(string id, string zid, string item_id)
        {//是否存在loc_id
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (id == zid)
                {
                    return 6;//可用
                }
                else
                {
                    sql.AppendFormat("SELECT row_id,loc_id,lsta_id FROM iloc WHERE loc_id='{0}' and lcat_id='R' ", id);//獲取到可用的料位.或者本身的主料位的滿足個數  R表示副料位
                    if (_access.getDataTable(sql.ToString()).Rows.Count > 0)
                    {
                        string state = _access.getDataTable(sql.ToString()).Rows[0]["lsta_id"].ToString();
                        if (state == "F")
                        {
                            return 6;//可使用
                        }
                        else if (state == "A")
                        {
                            sb.AppendFormat("SELECT row_id FROM iinvd where plas_loc_id='{0}' and item_id<>'{1}'", id, item_id);
                            if (_access.getDataTable(sb.ToString()).Rows.Count <= 0)
                            {
                                return 6;//可使用
                            }
                            return 2;//已指派
                        }
                        else if (state == "H")
                        {
                            sqlstr.AppendFormat("select row_id FROM iinvd where plas_loc_id='{0}' and item_id='{1}'", id, item_id);
                            if (_access.getDataTable(sqlstr.ToString()).Rows.Count > 0)
                            {
                                return 6;
                            }
                            else
                            {
                                return 3; //已鎖定
                            }
                        }
                    }
                    else
                    {
                        return 1;//此料位不存在
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Islocid-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int IsUpd(Iinvd m)
        {//是否編輯
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT row_id from iinvd where plas_loc_id='{0}' AND item_id='{1}' AND cde_dt='{2}';", m.plas_loc_id, m.item_id, m.cde_dt.ToShortDateString());
                return _access.getDataTable(sql.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->IsUpd-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Selnum(Iinvd m)
        {//查詢之前庫存
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT prod_qty from iinvd where plas_loc_id='{0}' AND item_id='{1}' AND cde_dt='{2}';", m.plas_loc_id, m.item_id, CommonFunction.DateTimeToShortString(m.cde_dt));
                DataTable table = _access.getDataTable(sql.ToString());
                if(table.Rows.Count>0)
                {
                    return Int32.Parse(table.Rows[0]["prod_qty"].ToString());
                }
                return 0;   
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Selnum-->" + ex.Message + sql.ToString(), ex);
            }
        }//
        #endregion

        #region 理貨員工作的庫存操作+對iwms_record標的新增
        public DataTable getTime(AseldQuery a)
        {
            StringBuilder sql = new StringBuilder();
            DataTable dt = new DataTable();
            sql.AppendFormat(" SELECT cde_dt_incr,cde_dt_shp from product_ext where item_id ='{0}' and  pwy_dte_ctl='Y'  ;", a.item_id);
            dt = _access.getDataTable(sql.ToString());
            return dt;
        }
        public string updgry(Aseld a, Dictionary<string, string> iinvd)// need item_id,act_pick_qty
        {//理貨員減去庫存
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            int sum = GetProqtyByItemid(int.Parse(a.item_id.ToString()));
            try
            {
                foreach (KeyValuePair<string, string> item in iinvd)
                {
                    sql.Clear();
                    //查詢日期控管的商品
                    sql.AppendFormat(" select row_id,i.item_id,prod_qty,i.cde_dt,i.made_date,pe.cde_dt_incr,pe.cde_dt_shp from iinvd i LEFT JOIN product_ext pe ON i.item_id = pe.item_id and pwy_dte_ctl='Y' WHERE row_id='{0}';", item.Key);
                    dt = _access.getDataTable(sql.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        string cde_dt_incr = dt.Rows[0]["cde_dt_incr"].ToString();
                        string made_date = dt.Rows[0]["made_date"].ToString();
                        string cde_dt = dt.Rows[0]["cde_dt"].ToString();
                        string cde_dt_shp = "0";
                        if (!string.IsNullOrEmpty(dt.Rows[0]["cde_dt_shp"].ToString()))
                        {
                            cde_dt_shp = dt.Rows[0]["cde_dt_shp"].ToString();
                        }
                        if (string.IsNullOrEmpty(cde_dt_incr))
                        {
                            cde_dt_incr = "0";
                        }
                        if (string.IsNullOrEmpty(made_date))
                        {
                            made_date = DateTime.Now.ToString();
                        }
                        if (string.IsNullOrEmpty(cde_dt))
                        {
                            cde_dt = DateTime.Now.ToString();
                        }
                        int value = 0;
                        if (int.TryParse(item.Value, out value))
                        {
                            value = int.Parse(item.Value);
                        }
                        if (dt.Rows[0]["prod_qty"].ToString() == value.ToString())
                        {//刪除收貨上架表庫存,往iwms_record表添加數據
                            sb.AppendFormat("Delete from iinvd where row_id='{0}' ;", dt.Rows[0]["row_id"], int.Parse(dt.Rows[0]["prod_qty"].ToString()) - a.act_pick_qty);
                            sb.AppendFormat("INSERT INTO iwms_record (order_id,detail_id,act_pick_qty,cde_dt,create_date,create_user_id,made_dt,cde_dt_incr,cde_dt_shp) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", a.ord_id, a.ordd_id, value, CommonFunction.DateTimeToString(DateTime.Parse(cde_dt)), CommonFunction.DateTimeToString(DateTime.Now), a.change_user, CommonFunction.DateTimeToString(DateTime.Parse(made_date.ToString())), cde_dt_incr, cde_dt_shp);
                            sb.AppendFormat("insert into istock_change(sc_trans_id,item_id,sc_trans_type,sc_num_old,sc_num_chg,sc_num_new,sc_time,sc_user,sc_istock_why,sc_note) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", a.ord_id, dt.Rows[0]["item_id"], "3", sum, "-" + value.ToString(), sum - value, CommonFunction.DateTimeToString(DateTime.Now), a.change_user, 4, "理貨撿貨");
                            sum = sum - value;
                        }
                        else if (int.Parse(dt.Rows[0]["prod_qty"].ToString()) > value)
                        {//扣除收貨上架表數據,往iwms_record表添加數據
                            if (value > 0)
                            {
                                sb.AppendFormat("UPDATE iinvd SET prod_qty='{1}' where row_id='{0}' ;", dt.Rows[0]["row_id"], int.Parse(dt.Rows[0]["prod_qty"].ToString()) - value);
                                sb.AppendFormat("INSERT INTO iwms_record (order_id,detail_id,act_pick_qty,cde_dt,create_date,create_user_id,made_dt,cde_dt_incr,cde_dt_shp) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", a.ord_id, a.ordd_id, value, CommonFunction.DateTimeToString(DateTime.Parse(cde_dt)), CommonFunction.DateTimeToString(DateTime.Now), a.change_user, CommonFunction.DateTimeToString(DateTime.Parse(made_date.ToString())), cde_dt_incr, cde_dt_shp);
                                sb.AppendFormat("insert into istock_change(sc_trans_id,item_id,sc_trans_type,sc_num_old,sc_num_chg,sc_num_new,sc_time,sc_user,sc_istock_why,sc_note) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", a.ord_id, dt.Rows[0]["item_id"], "3", sum, "-" + value.ToString(), sum - value, CommonFunction.DateTimeToString(DateTime.Now), a.change_user, 4, "理貨撿貨");
                                sum = sum - value;
                            }
                        }
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->updgry-->" + ex.Message + sql.ToString() + " sb:" + sb.ToString(), ex);
            }
        }
        #endregion

        #region 料位鎖
        public int UpdateIinvdLock(Iinvd nvd)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" update iinvd set ista_id='{0}',change_user='{1}',change_dtim='{2}',qity_id='{3}' where row_id='{4}' ", nvd.ista_id, nvd.change_user, Common.CommonFunction.DateTimeToString(nvd.change_dtim), nvd.qity_id, nvd.row_id);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->UpdateIinvdLock-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 庫存補貨報表
        public DataTable ExportExcel(IinvdQuery vd)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT ");
            sql.Append(@"iifuliaowei.plas_loc_id as '副料位編號',");
            sql.Append(@"iifuliaowei.prod_qty as '副料位數量',");
            sql.Append(@"iplas.loc_id as '主料位編號',");
            sql.Append(@"(SELECT IFNULL(sum(prod_qty),0) from iinvd i right JOIN iplas ip on ip.loc_id=i.plas_loc_id where i.item_id  = pi.item_id and i.ista_id='A') as '主數量',");
            sql.Append(@"iplas.loc_stor_cse_cap AS '容量','' AS '實際補貨量',iifuliaowei.item_id AS '商品細項編號',");
            sql.Append(@"CONCAT(v.brand_name,'-',p.product_name) AS '品名',");
            sql.Append(@"concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as '規格',");
            sql.Append(@"SUBSTRING(date_format(iifuliaowei.cde_dt,'%Y-%m-%d'),1,10)  AS '有效期/FIFO'");
            //  sql.Append(@",upc.upc_id as '條碼' ");
            sql.Append(@"from iplas ");
            sql.Append(@" left JOIN iinvd iifuliaowei on iifuliaowei.item_id=iplas.item_id and iifuliaowei.plas_loc_id<>iplas.loc_id ");
            //sql.Append(@" LEFT JOIN (SELECT item_id,upc_id from iupc GROUP BY item_id) upc on upc.item_id=iplas.item_id");
            sql.Append(@" LEFT JOIN product_item pi ON pi.item_id=iplas.item_id");
            sql.Append(@" LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id ");
            sql.Append(@" LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id ");
            sql.Append(@" LEFT JOIN product p ON pi.product_id=p.product_id ");
            sql.Append(@" LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id ");
            sql.Append(@" where 1=1 ");
            if (vd.item_id != 0)
            {
                sql.AppendFormat(" and iplas.item_id='{0}'", vd.item_id);
            }
            //if (vd.upc_id != "0" && !string.IsNullOrEmpty(vd.upc_id))
            //{
            //    sql.AppendFormat(" AND iupc.upc_id='{0}'", vd.upc_id);
            //}
            if (!string.IsNullOrEmpty(vd.loc_id))
            {
                sql.AppendFormat(" and iplas.loc_id='{0}'", vd.loc_id);
            }
            if (!string.IsNullOrEmpty(vd.startIloc) && !string.IsNullOrEmpty(vd.endIloc))
            {
                sql.AppendFormat(" and iplas.loc_id between '{0}' and '{1}'", vd.startIloc, vd.endIloc);
            }

            if (vd.auto == 1)
            {
                sql.AppendFormat(" AND (SELECT IFNULL(sum(prod_qty),0) from iinvd i right join iplas ip on ip.loc_id=i.plas_loc_id where i.item_id  = pi.item_id and i.ista_id='A')< iplas.loc_stor_cse_cap");//LEFT JOIN iplas ip on ip.loc_id=i.plas_loc_id
            }
            else
            {
                sql.AppendFormat(" AND (SELECT IFNULL(sum(prod_qty),0) from iinvd i right JOIN  iplas ip on ip.loc_id=i.plas_loc_id where i.item_id  = pi.item_id  and i.ista_id='A')<={0}", vd.sums);
            }
            sql.AppendFormat(" and iifuliaowei.plas_loc_id is not NULL ORDER BY iifuliaowei.cde_dt ASC ");
            return _access.getDataTable(sql.ToString());
        }
        #endregion

        #region 即期品/過期品預告表
        public DataTable PastProductExportExcel(IinvdQuery vd)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();

            if (vd.notimeortimeout != 0)
            {
                if (vd.notimeortimeout == 1)
                {
                    sbt.AppendFormat(@" and nvd.cde_dt<=ADDDATE(now(),{0}+pext.cde_dt_shp) ", vd.yugaoDay);
                    sbt.AppendFormat(@" and nvd.cde_dt>=now() ");
                }
                else if (vd.notimeortimeout == 2)
                {
                    sbt.AppendFormat(@" and nvd.cde_dt<=ADDDATE(now(),{0}) ", vd.yugaoDay);//過期貨
                }
            }
            if (vd.endDay != 0)
            {
                sbt.AppendFormat(@"  and pext.cde_dt_incr>='{0}' and pext.cde_dt_incr<='{1}' ", vd.startDay, vd.endDay);
            }

            if (vd.startIloc != "" && vd.endIloc != "")
            {
                sbt.AppendFormat(@" and (loc.loc_id>='{0}' and loc.loc_id <='{1}') ", vd.startIloc, vd.endIloc);
            }
            sb.AppendFormat(@"SELECT SUBSTRING(nvd.cde_dt,1,10) as cde_dt,
nvd.plas_loc_id,  loc.lcat_id,pext.cde_dt_incr,pext.cde_dt_shp,pext.cde_dt_var,nvd.item_id,nvd.prod_qty,p.prepaid,nvd.ista_id,
CONCAT(v.brand_name,'-',p.product_name) AS 'product_name',concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,
dfsm.delivery_freight_set as product_freight_set ");
            sb.AppendFormat(@"FROM iinvd nvd 
LEFT JOIN iloc loc on loc.loc_id=nvd.plas_loc_id 
LEFT JOIN product_ext pext on pext.item_id=nvd.item_id 
LEFT JOIN product_item pi on pi.item_id=nvd.item_id 
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id 
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id 
LEFT JOIN product p on p.product_id= pi.product_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
left join delivery_freight_set_mapping dfsm on dfsm.product_freight_set=p.product_freight_set
where pext.pwy_dte_ctl='Y' ");
            //LEFT JOIN (SELECT item_id,upc_id from iupc GROUP BY item_id) upc on upc.item_id=nvd.item_id 
            sb.Append(sbt.ToString());
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao.PastProductExportExcel-->" + ex.Message + sb.ToString(), ex);
            }
        }

        #endregion


        #region 庫存匯出
        public List<IinvdQuery> KucunExport(IinvdQuery nvd)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();

            if (nvd.item_id == 0)
            {

                if (nvd.startIloc != "" && nvd.endIloc != "")
                {
                    sbt.AppendFormat(@" and (iid.plas_loc_id>='{0}' and iid.plas_loc_id <='{1}') ", nvd.startIloc, nvd.endIloc);
                }
                else
                {
                    sbt.AppendFormat(" ");
                }
            }
            else
            {
                sbt.AppendFormat(@" and iid.item_id='{0}'", nvd.item_id);
            }

            sb.AppendFormat(@"SELECT iid.plas_loc_id,iid.prod_qty,iid.item_id,iid.cde_dt,iid.qity_id,ptm.product_id,CONCAT(v.brand_name,'-',p.product_name) as 'product_name','' as upc_id,tp.parameterName,'' as prod_sz,p.prepaid,iid.ista_id FROM iinvd iid 
                                LEFT JOIN product_item ptm on iid.item_id=ptm.item_id                     
                                LEFT JOIN product p on ptm.product_id=p.product_id 
                                LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
                                LEFT JOIN t_parametersrc tp on iid.qity_id=tp.parameterCode
 where iid.ista_id='H' and tp.parameterType='loc_lock_msg' ");
            //LEFT JOIN product_spec ps1 ON ptm.spec_id_1 = ps1.spec_id
            //         LEFT JOIN product_spec ps2 ON ptm.spec_id_2 = ps2.spec_id
            //                                LEFT JOIN (SELECT item_id,upc_id from iupc GROUP BY item_id) ipc on iid.item_id=ipc.item_id 
            sb.Append(sbt.ToString());
            try
            {
                return _access.getDataTableForObj<IinvdQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.KucunExport-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 庫存調整
        public int kucunTiaozheng(Iinvd invd)
        {
            StringBuilder sb = new StringBuilder();
            if (invd.prod_qty == 0)
            {
                StringBuilder sbstr = new StringBuilder();
                StringBuilder sbstr2 = new StringBuilder();
                sbstr.AppendFormat("select plas_loc_id from iinvd where row_id ='{0}';", invd.row_id);
                DataTable qtdt = _access.getDataTable(sbstr.ToString());
                sbstr2.AppendFormat("select row_id from iinvd where plas_loc_id='{0}';", qtdt.Rows[0][0]);
                DataTable qtdt2 = _access.getDataTable(sbstr2.ToString());
                sb.Append("set sql_safe_updates = 0;");
                sb.AppendFormat("delete from iinvd where row_id='{0}';", invd.row_id);
                sb.Append("set sql_safe_updates = 1;");
                if (qtdt2.Rows.Count < 2)
                {
                    sb.AppendFormat("set sql_safe_updates = 0;");
                    sb.AppendFormat("update iloc set lsta_id='F' where loc_id='{0}' and lcat_id='R';", qtdt.Rows[0][0]);
                    sb.AppendFormat("set sql_safe_updates = 1;");
                }
            }
            else
            {
                sb.AppendFormat("set sql_safe_updates = 0;");
                sb.AppendFormat(@" update iinvd set prod_qty='{0}',change_user='{1}',change_dtim='{2}' where row_id='{3}'; ", invd.prod_qty, invd.change_user, Common.CommonFunction.DateTimeToString(invd.change_dtim), invd.row_id);
                sb.AppendFormat("set sql_safe_updates = 1;");
            }
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->kucunTiaozheng-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 盤點薄報表所用
        /// <summary>
        /// 盤點薄報表所用
        /// </summary>
        /// <returns></returns>
        public DataTable CountBook(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            StringBuilder sbjoin = new StringBuilder();
            try
            {
                if (!String.IsNullOrEmpty(m.startIloc))
                {
                    if (m.startIloc != "" && m.endIloc != "")
                    {
                        sbWhere.AppendFormat(" and (loc.loc_id>='{0}' and loc.loc_id <='{1}') ", m.startIloc, m.endIloc);
                    }
                    if (!string.IsNullOrEmpty(m.lot_no))
                    {
                        sbWhere.AppendFormat(" and SUBSTR(loc.loc_id,6,1) in ('{0}') ", m.lot_no);
                    }
                }
                if (!string.IsNullOrEmpty(m.Firstsd))
                {
                    if (m.Firstsd == "0")
                    {
                        sbWhere.Append("and SUBSTR(loc.loc_id,5,1) in ('1','3','5','7','9')");
                    }
                    else
                    {
                        sbWhere.Append(" and SUBSTR(loc.loc_id,5,1) in ('0','2','4','6','8')");
                    }
                }
                if (!string.IsNullOrEmpty(m.vender))
                {
                    sbWhere.AppendFormat(" AND (vv.vendor_code LIKE'%{0}%' OR vv.vendor_name_simple LIKE'%{0}%') ", m.vender);
                    sbjoin.Append(" LEFT JOIN vendor vv ON  v.vendor_id=vv.vendor_id ");
                }
                sbSql.Append("SELECT nvd.row_id, ");
                if (!string.IsNullOrEmpty(m.cb_jobid))
                {
                    sbSql.Append("cd.cb_newid, ");
                    sbjoin.AppendFormat(" LEFT JOIN cbjob_detail cd ON nvd.row_id=cd.iinvd_id and cd.cb_jobid='{0}' ", m.cb_jobid);
                }
                sbSql.AppendFormat(@" loc.loc_id,loc.lsta_id ,nvd.item_id,nvd.prod_qty ,nvd.made_date,nvd.cde_dt ,CONCAT(v.brand_name,'-',p.product_name) as 'product_name','' as prod_sz,p.prepaid ,ptet.cde_dt_incr,ptet.cde_dt_var ,ptet.cde_dt_shp,ptet.pwy_dte_ctl,p.product_id
from iloc loc
inner JOIN iinvd nvd ON loc.loc_id=nvd.plas_loc_id
LEFT JOIN product_item pi on pi.item_id = nvd.item_id
LEFT JOIN product p on p.product_id = pi.product_id 	 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id 
LEFT JOIN product_ext ptet on pi.item_id=ptet.item_id 
{1}		
where loc.lsta_id NOT in ('H','F') {0}  
ORDER BY loc.loc_id,nvd.made_date", sbWhere.ToString(), sbjoin.ToString());
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->CountBook-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }
        public DataTable GetIplasCountBook(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbjoin = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"SELECT ''as 'row_id',ip.loc_id,loc.lsta_id ,   
ip.item_id,'0' as 'prod_qty' ,'' as cost ,'' as made_date,'' as cde_dt ,                 
CONCAT(v.brand_name,'-',p.product_name) as 'product_name','' as prod_sz,p.prepaid,ptet.cde_dt_incr,ptet.cde_dt_var ,ptet.cde_dt_shp,ptet.pwy_dte_ctl,p.product_id
from iloc loc LEFT JOIN iplas ip ON loc.loc_id=ip.loc_id 
LEFT JOIN product_item pi on pi.item_id = ip.item_id 
LEFT JOIN product p on p.product_id = pi.product_id LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
LEFT JOIN product_ext ptet on pi.item_id=ptet.item_id  {1} 		
where loc.loc_id='{0}' {2} LIMIT 1;", m.loc_id, sbjoin, sbWhere);
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetIplasCountBook-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }
        public DataTable Getloc()
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append(@"SELECT loc_id from iloc loc WHERE loc.loc_id NOT IN (SELECT plas_loc_id FROM iinvd) AND loc.lsta_id NOT in ('H','F') order by loc_id;");
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Getloc-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }

        public DataTable getproduct(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            StringBuilder sbjoin = new StringBuilder();
            try
            {
                if (!String.IsNullOrEmpty(m.startIloc))
                {
                    if (m.startIloc != "" && m.endIloc != "")
                    {
                        sbWhere.AppendFormat(" and (loc.loc_id>='{0}' and loc.loc_id <='{1}') ", m.startIloc, m.endIloc);
                    }
                    if (!string.IsNullOrEmpty(m.lot_no))
                    {
                        sbWhere.AppendFormat(" and SUBSTR(loc.loc_id,6,1) in ('{0}') ", m.lot_no);
                    }
                }
                if (!string.IsNullOrEmpty(m.Firstsd))
                {
                    if (m.Firstsd == "0")
                    {
                        sbWhere.Append("and SUBSTR(loc.loc_id,5,1) in ('1','3','5','7','9')");
                    }
                    else
                    {
                        sbWhere.Append(" and SUBSTR(loc.loc_id,5,1) in ('0','2','4','6','8')");
                    }
                }
                if (m.prepaid != 0)
                {
                    sbWhere.AppendFormat(" and p.prepaid='{0}' ", m.prepaid);
                }
                if (!string.IsNullOrEmpty(m.vender))
                {
                    sbWhere.AppendFormat(" AND (vv.vendor_id ='{0}' OR vv.vendor_name_simple LIKE'%{0}%') ", m.vender);
                    sbjoin.Append(@"	LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id  LEFT JOIN vendor vv ON  v.vendor_id=vv.vendor_id ");
                }
                sbSql.AppendFormat(@" SELECT  loc.item_id, loc.loc_id, loc.product_id, loc.row_id FROM iloc INNER JOIN (
SELECT i.item_id,i.plas_loc_id as 'loc_id',pi.product_id,i.row_id from iinvd i LEFT JOIN product_item pi ON i.item_id = pi.item_id
UNION
SELECT i.item_id,i.loc_id,pi.product_id,'' as 'row_id' from iplas i LEFT JOIN  product_item pi ON i.item_id = pi.item_id) loc ON loc.loc_id=iloc.loc_id  LEFT JOIN product p on p.product_id = loc.product_id  {1} where iloc.lsta_id NOT in ('H','F')   {0}
ORDER BY loc.loc_id,loc.row_id DESC  ", sbWhere.ToString(), sbjoin.ToString());
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->CountBook-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }
        public DataTable GetIinvdCountBook(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbjoin = new StringBuilder();
            try
            {
                sbSql.Append("SELECT nvd.row_id,");
                if (!string.IsNullOrEmpty(m.cb_jobid))
                {
                    sbSql.Append("cd.cb_newid, ");
                    sbjoin.AppendFormat(" LEFT JOIN cbjob_detail cd ON nvd.row_id=cd.iinvd_id and cd.cb_jobid='{0}' ", m.cb_jobid);
                }
                sbSql.AppendFormat(@" loc.loc_id,nvd.ista_id as lsta_id ,nvd.item_id,nvd.prod_qty ,nvd.made_date,nvd.cde_dt ,CONCAT(v.brand_name,'-',p.product_name) as 'product_name','' as prod_sz,p.prepaid ,ptet.cde_dt_incr,ptet.cde_dt_var ,ptet.cde_dt_shp,ptet.pwy_dte_ctl,p.product_id
from iloc loc LEFT JOIN iinvd nvd on  loc.loc_id=nvd.plas_loc_id
LEFT JOIN product_item pi on pi.item_id = nvd.item_id
LEFT JOIN product p on p.product_id = pi.product_id 	 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id 
LEFT JOIN product_ext ptet on pi.item_id=ptet.item_id 
{1}		
where nvd.plas_loc_id='{0}' ORDER BY nvd.made_date", m.loc_id, sbjoin.ToString());
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetIinvdCountBook-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }

        #endregion

        #region 盤點差異報表所用
        /// <summary>
        /// 盤點薄報表所用
        /// </summary>
        /// <returns></returns>
        public DataTable DifCountBook(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            try
            {
                if (m.startIloc != "")
                {
                    sbWhere.AppendFormat(" and i.plas_loc_id>='{0}' ", m.startIloc);
                }
                if (m.endIloc != "")
                {
                    sbWhere.AppendFormat(" and i.plas_loc_id <='{0}' ", m.endIloc);
                }
                if (m.startcost != 0)
                {//金額
                    sbWhere.AppendFormat(" and ABS(prod_qty-st_qty)* pm.cost >='{0}' ", m.startcost);
                }
                if (m.endcost != 0)
                {
                    sbWhere.AppendFormat(" and ABS(prod_qty-st_qty)* pm.cost <='{0}' ", m.endcost);
                }
                if (m.startsum != 0)
                {//數量
                    sbWhere.AppendFormat(" and ABS(prod_qty-st_qty)*100/prod_qty>='{0}' ", m.startsum);
                }
                if (m.endsum != 0)
                {
                    sbWhere.AppendFormat(" and ABS(prod_qty-st_qty)*100/prod_qty <='{0}' ", m.endsum);
                }
                sbSql.AppendFormat(@"
SELECT  DISTINCT i.row_id,loc.loc_id,loc.lsta_id,i.item_id,pm.cost,CONCAT(v.brand_name,'-',p.product_name) as 'product_name',
'' as prod_sz, 
i.made_date,i.cde_dt,prod_qty,st_qty,
ABS(prod_qty-st_qty)*100/prod_qty as'qty',
ABS(prod_qty-st_qty)* pm.cost as 'money'
from iinvd i 
LEFT JOIN iloc loc on i.plas_loc_id=loc.loc_id
LEFT JOIN product_item pi on pi.item_id=i.item_id
LEFT JOIN product p ON pi.product_id=p.product_id
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
LEFT JOIN iialg iia on iia.cde_dt=i.cde_dt AND iia.item_id=i.item_id and iia.loc_id=i.plas_loc_id 
LEFT JOIN price_master pm on pm.product_id=pi.product_id and pm.site_id='1'
where ABS(prod_qty-st_qty)>0 and IFNULL(iia.iarc_id,'HH')<>'OB' {0} ", sbWhere.ToString());
                sbSql.Append(" ORDER BY iia.loc_id ;");
                //GROUP BY i.row_id  
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->DifCountBook-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }
        #endregion

        #region 盤點差異報表(OBK)
        /// <summary>
        /// 盤點薄報表所用
        /// </summary>
        /// <returns></returns>
        public DataTable CountBookOBK(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            try
            {
                if (m.startIloc != "")
                {
                    sbWhere.AppendFormat(" and i.plas_loc_id>='{0}' ", m.startIloc);
                }
                if (m.endIloc != "")
                {
                    sbWhere.AppendFormat(" and i.plas_loc_id<='{0}' ", m.endIloc);
                }
                if (m.startcost != 0)
                {//金額
                    sbWhere.AppendFormat(" and ABS(prod_qty-st_qty)* pm.cost>='{0}' ", m.startcost);
                }
                if (m.endcost != 0)
                {
                    sbWhere.AppendFormat(" and ABS(prod_qty-st_qty)* pm.cost <='{0}' ", m.endcost);
                }

                sbSql.AppendFormat(@"
SELECT DISTINCT i.row_id,i.item_id,'' as prod_sz, prod_qty as 'st_qty',st_qty as 'prod_qty',ABS(prod_qty-st_qty)* pm.cost as 'money',CONCAT(v.brand_name,'-',p.product_name) as 'product_name',loc.loc_id,loc.lsta_id,pm.cost,i.made_date,i.cde_dt
 from iinvd i 
LEFT JOIN iloc loc on i.plas_loc_id=loc.loc_id
LEFT JOIN iialg ia ON i.plas_loc_id=ia.loc_id AND i.made_date=ia.made_dt AND i.cde_dt=ia.cde_dt 
LEFT JOIN product_item pi on pi.item_id=i.item_id
LEFT JOIN product p ON pi.product_id=p.product_id
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
LEFT JOIN price_master pm on pm.product_id=pi.product_id and pm.site_id='1'
WHERE st_qty=0 AND ia.create_dtim> ADDDATE(NOW(),-3) AND iarc_id='OB' {0} ", sbWhere.ToString());
                //sbSql.AppendFormat(" group by i.row_id");
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->CountBookOBK-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }
        #endregion

        public DataTable GetRowMsg(Iinvd invd)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT made_date,cde_dt from iinvd where row_id='{0}' limit 1 ", invd.row_id);
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao.GetRowMsg-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int AboutItemidLocid(Iinvd invd)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            str.AppendFormat(@"SELECT plas_id FROM iplas WHERE loc_id='{0}' and item_id='{1}'", invd.plas_loc_id, invd.item_id);
            try
            {
                DataTable _dtone = _access.getDataTable(str.ToString());
                if (_dtone.Rows.Count > 0)
                {
                    return _dtone.Rows.Count;
                }
                else
                {
                    sb.AppendFormat(@" select row_id from iinvd where item_id='{0}' and plas_loc_id='{1}' ", invd.item_id, invd.plas_loc_id);
                    DataTable dt = _access.getDataTable(sb.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows.Count;
                    }
                    else
                    {
                        sql.AppendFormat(@" SELECT row_id FROM iloc WHERE loc_id='{0}' and loc_status=1 and lcat_id='R'", invd.plas_loc_id);
                        DataTable _locDt = new DataTable();
                        _locDt = _access.getDataTable(sql.ToString());
                        return _locDt.Rows.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->AboutItemidLocid-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int sum(Iinvd i, string lcat_id)
        {
            StringBuilder sb = new StringBuilder();
            int sum = 0;
            if (!String.IsNullOrEmpty(i.item_id.ToString()) && !String.IsNullOrEmpty(lcat_id))
            {
                sb.AppendFormat("SELECT sum(prod_qty) AS prod_qty from iinvd i LEFT JOIN iloc il ON i.plas_loc_id=il.loc_id where item_id='{0}' AND ista_id='A' AND il.lcat_id='{1}' ", i.item_id, lcat_id);
            }
            if (!string.IsNullOrEmpty(i.made_date.ToString()) && i.made_date != DateTime.MinValue)
            {
                sb.AppendFormat(" AND i.made_date='{0}' ", i.made_date.ToShortDateString());
            }
            try
            {
                if (string.IsNullOrEmpty(_access.getDataTable(sb.ToString()).Rows[0]["prod_qty"].ToString()))
                {
                    sum = 0;
                }
                else
                {
                    sum = int.Parse(_access.getDataTable(sb.ToString()).Rows[0]["prod_qty"].ToString());
                }
                return sum;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->sum-->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// chaojie1124j添加于2015/6/1 通過商品編號查詢所有的庫存。
        /// 實現料位管理->料位移動記錄的庫存欄位
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int SumProd_qty(Iinvd i)
        {
            StringBuilder sb = new StringBuilder();
            int sum = 0;
            try
            {

                if (!String.IsNullOrEmpty(i.item_id.ToString()))
                {
                    sb.AppendFormat("SELECT sum(prod_qty) AS prod_qty from iinvd i  where i.item_id='{0}' ", i.item_id);
                }
                if (string.IsNullOrEmpty(_access.getDataTable(sb.ToString()).Rows[0]["prod_qty"].ToString()))
                {
                    sum = 0;
                }
                else
                {
                    sum = int.Parse(_access.getDataTable(sb.ToString()).Rows[0]["prod_qty"].ToString());
                }
                return sum;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->SumProd_qty-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int Updateiinvdstqty(Iinvd invd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update iinvd set st_qty='{0}',create_user='{1}',create_dtim='{2}' where row_id='{3}'", invd.st_qty, invd.create_user, CommonFunction.DateTimeToString(invd.create_dtim), invd.row_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->AboutItemidLocid-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public List<Iinvd> GetIinvd(Iinvd i)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (i.row_id > 0)
                {
                    sb.AppendFormat("SELECT row_id,prod_qty,plas_loc_id,item_id,cde_dt,made_date from iinvd where row_id='{0}' LIMIT 1;", i.row_id);
                    return _access.getDataTableForObj<Iinvd>(sb.ToString());
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetIinvd-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public List<IinvdQuery> GetIinvdExprotList(IinvdQuery iq)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"select vb.vendor_id,Tp_table.parameterName as qity_name,ii.row_id,lic_plt_id,ii.dc_id,ii.whse_id,ii.made_date,ii.po_id,prod_qty,rcpt_id,lot_no,hgt_used,
ii.create_user,ii.create_dtim,ii.change_user,ii.change_dtim,ii.cde_dt,ista_id,receipt_dtim,stor_ti,stor_hi,inv_pos_cat,qity_id,plas_loc_id,ii.item_id,
plas_prdd_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,ip.loc_id,pe.cde_dt_var,pe.cde_dt_shp,pe.pwy_dte_ctl,pe.cde_dt_incr,iu.upc_id,
us.user_username as user_name from iinvd ii ");
                sql.Append(" left join iplas ip on ii.item_id=ip.item_id ");

                sql.Append(" left join product_item pi on ii.item_id=pi.item_id ");
                sql.Append(" left JOIN product_ext pe ON ii.item_id=pe.item_id ");
                sql.Append(" LEFT JOIN (SELECT parameterCode,parameterName from t_parametersrc where parameterType='loc_lock_msg') as Tp_table on ii.qity_id=Tp_table.parameterCode ");
                sql.Append(" left join product p on p.product_id=pi.product_id  ");
                sql.Append(" LEFT JOIN  manage_user us on ii.create_user=us.user_id  ");
                sql.Append(" LEFT JOIN  iupc iu on ii.item_id=iu.item_id and iu.upc_type_flg='1'  ");
                sql.Append(" left join vendor_brand vb on p.brand_id=vb.brand_id where 1=1 ");
                if (!string.IsNullOrEmpty(iq.plas_loc_id))
                {
                    sql.AppendFormat(" and ii.plas_loc_id='{0}' ", iq.plas_loc_id.ToString().ToUpper());
                }
                if (iq.serch_type != 0)
                {
                    if (!string.IsNullOrEmpty(iq.serchcontent))//有查詢內容就不管時間
                    {
                        sql.AppendFormat(" and ii.item_id in ({0})  ", iq.serchcontent);
                        //switch (iq.serch_type)
                        //{
                        //    case 1:
                        //        sql.AppendFormat(" and ii.plas_loc_id in( select loc_id from iloc where loc_id='{0}' or hash_loc_id='{0}') ", iq.serchcontent);
                        //        break;
                        //    case 2:
                        //        sql.AppendFormat(" and ii.item_id=(select item_id from iupc iu where iu.upc_id ='{0}') ", iq.serchcontent);
                        //        break;
                        //    case 3:
                        //        sql.AppendFormat(" and ii.item_id in ({0})  ", iq.serchcontent);
                        //        break;
                        //    default:
                        //        break;
                        //}
                    }
                }
                DateTime dt = DateTime.Parse("1970-01-02 08:00:00");
                if (!string.IsNullOrEmpty(iq.starttime.ToString()) && dt < iq.starttime)
                {
                    sql.AppendFormat(" and ii.create_dtim>'{0}' ", CommonFunction.DateTimeToString(iq.starttime));
                }
                if (!string.IsNullOrEmpty(iq.endtime.ToString()) && dt < iq.endtime)
                {
                    sql.AppendFormat(" and ii.create_dtim<'{0}' ", CommonFunction.DateTimeToString(iq.endtime));
                }
                //if (!string.IsNullOrEmpty(iq.serchcontent))
                //{
                //    sql.AppendFormat(" and (ii.item_id in ({0})  )", iq.serchcontent);
                //}               
                if (!string.IsNullOrEmpty(iq.ista_id))
                {//理貨員工作用到庫存
                    sql.AppendFormat(" and ii.ista_id='{0}'  order by ii.cde_dt ;", iq.ista_id);
                }
                else
                {//收貨上架列表頁
                    sql.AppendFormat(" order by ii.row_id ;");
                }
                return _access.getDataTableForObj<IinvdQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetIupcList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 獲取庫鎖備註
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public string remark(IinvdQuery q)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select remarks from iialg where item_id='{0}' ", q.item_id);
                if (!string.IsNullOrEmpty(q.made_date.ToString()) && q.made_date > DateTime.MinValue)
                {
                    sql.AppendFormat(" and made_dt='{0}' ", CommonFunction.DateTimeToString(q.made_date));
                }
                if (!string.IsNullOrEmpty(q.plas_loc_id))
                {
                    sql.AppendFormat(" and loc_id='{0}' ", q.plas_loc_id);
                }
                sql.Append("ORDER BY row_id DESC; ");
                if (_access.getDataTable(sql.ToString()).Rows.Count > 0)
                {
                    return _access.getDataTable(sql.ToString()).Rows[0]["remarks"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->remark-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string Getcost(string product_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select cost from price_master where site_id='1' and product_id='{0}' ", product_id);
                if (_access.getDataTable(sql.ToString()).Rows.Count > 0)
                {
                    return _access.getDataTable(sql.ToString()).Rows[0]["cost"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->Getcost-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public int GetProqtyByItemid(int item_id)
        {
            StringBuilder sql = new StringBuilder();
            int i = 0;
            try
            {
                sql.AppendFormat("SELECT SUM(prod_qty)  FROM iinvd WHERE item_id='{0}' and ista_id='A';", item_id);
                try
                {
                    if (Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0][0]) > 0)
                    {
                        return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0][0]);
                    }
                }
                catch (Exception ex)
                {
                    i = 0;
                }
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetProqtyByItemid-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /**
         *chaojie1124j庫調或收貨上架的時候，如果庫存鎖住，不能進行庫調
         */
        public List<IinvdQuery> GetSearchIinvd(Model.Query.IinvdQuery ivd)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            try
            {
                sql.Append(@"select row_id,plas_loc_id,made_date,cde_dt,prod_qty from iinvd ii");
                sql.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(ivd.plas_loc_id))
                {
                    sbwhere.AppendFormat(" and ii.plas_loc_id='{0}' ", ivd.plas_loc_id.ToString().ToUpper());
                }
                if (!string.IsNullOrEmpty(ivd.ista_id))
                {
                    sbwhere.AppendFormat(" and ii.ista_id='{0}' ", ivd.ista_id);
                }
                if (ivd.item_id != 0)
                {
                    sbwhere.AppendFormat(" and ii.item_id='{0}' ", ivd.item_id);
                }
                if (ivd.made_date != ivd.cde_dt && ivd.made_date > DateTime.MinValue)
                {
                    sbwhere.AppendFormat(" and ii.made_date='{0}' ", ivd.made_date.ToString("yyyy-MM-dd"));
                }
                if (ivd.made_date != ivd.cde_dt && ivd.cde_dt > DateTime.MinValue)
                {
                    sbwhere.AppendFormat(" and ii.cde_dt='{0}' ", ivd.cde_dt.ToString("yyyy-MM-dd"));
                }
                if (ivd.made_date == ivd.cde_dt && ivd.made_date > DateTime.MinValue)
                {
                    sbwhere.AppendFormat(" and ii.cde_dt='{0}' and ii.made_date='{1}' ", ivd.cde_dt.ToString("yyyy-MM-dd"), ivd.made_date.ToString("yyyy-MM-dd"));
                }
                return _access.getDataTableForObj<IinvdQuery>(sql.ToString() + sbwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetSearchIinvd-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /**
         * chaojie1124j待檢貨商品報表中，查詢主料位的額庫存，然後分批進行檢貨的 
         */
        public List<IinvdQuery> GetPlasIinvd(Model.Query.IinvdQuery ivd)
        {

            StringBuilder sql = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();

            try
            {
                sql.Append(@"select plas_loc_id,made_date,cde_dt,prod_qty ");
                sql.Append(@" from iplas ip left join iinvd ii  on ip.loc_id=ii.plas_loc_id ");
                sql.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(ivd.plas_loc_id))
                {
                    sbwhere.AppendFormat(" and ii.plas_loc_id='{0}' ", ivd.plas_loc_id.ToString().ToUpper());
                }
                if (!string.IsNullOrEmpty(ivd.ista_id))
                {
                    sbwhere.AppendFormat(" and ii.ista_id='{0}' ", ivd.ista_id);
                }
                if (ivd.item_id != 0)
                {
                    sbwhere.AppendFormat(" and ip.item_id='{0}' ", ivd.item_id);
                }
                if (ivd.made_date != ivd.cde_dt && ivd.made_date > DateTime.MinValue)
                {
                    sbwhere.AppendFormat(" and ii.made_date='{0}' ", ivd.made_date.ToString("yyyy-MM-dd"));
                }
                if (ivd.made_date != ivd.cde_dt && ivd.cde_dt > DateTime.MinValue)
                {
                    sbwhere.AppendFormat(" and ii.cde_dt='{0}' ", ivd.cde_dt.ToString("yyyy-MM-dd"));
                }
                if (ivd.made_date == ivd.cde_dt && ivd.made_date > DateTime.MinValue)
                {
                    sbwhere.AppendFormat(" and ii.cde_dt='{0}' and ii.made_date='{1}' ", ivd.cde_dt.ToString("yyyy-MM-dd"), ivd.made_date.ToString("yyyy-MM-dd"));
                }
                sbwhere.Append(" order by ii.cde_dt asc ");

                return _access.getDataTableForObj<IinvdQuery>(sql.ToString() + sbwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IupcDao-->GetPlasIinvd-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #region 料位循環盤點 add by yafeng0715j201511041535
        public List<IinvdQuery> GetIinvdList(string loc_id)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT pi.item_id, p.product_name,CONCAT_WS('-',ps1.spec_name,ps2.spec_name)AS spec, made_date,cde_dt,prod_qty,pe.pwy_dte_ctl,i.row_id,ic.lcat_id  FROM iinvd i ");
                sql.Append(" LEFT JOIN (SELECT loc_id,lcat_id FROM iloc GROUP BY loc_id) ic ON ic.loc_id=i.plas_loc_id");
                sql.Append(" LEFT JOIN product_item pi ON i.item_id=pi.item_id");
                sql.Append(" LEFT JOIN product p ON p.product_id =pi.product_id");
                sql.Append(" LEFT JOIN product_ext pe ON pe.item_id=pi.item_id");
                sql.Append(" LEFT JOIN product_spec ps1 ON ps1.spec_id=pi.spec_id_1");
                sql.Append(" LEFT JOIN product_spec ps2 ON ps2.spec_id=pi.spec_id_2");
                sql.Append(" WHERE ista_id='A'");
                if (!string.IsNullOrEmpty(loc_id))
                {
                    sbwhere.AppendFormat(" AND i.plas_loc_id='{0}'", loc_id);
                }
                return _access.getDataTableForObj<IinvdQuery>(sql.ToString() + sbwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetIinvdList-->" + ex.Message + sql.ToString() + sbwhere.ToString(), ex);
            }
        }

        public int GetIinvdCount(IinvdQuery iinvd)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT row_id,prod_qty FROM iinvd WHERE item_id={0} AND made_date='{1}' AND plas_loc_id='{2}' and ista_id='A';", iinvd.item_id, CommonFunction.DateTimeToShortString(iinvd.made_date), iinvd.plas_loc_id);
                DataTable table = _access.getDataTable(sql.ToString());
                if (table.Rows.Count > 0)
                {
                    string row_id = table.Rows[0][0].ToString();
                    string prod_qty = table.Rows[0][1].ToString();
                    sql.Clear();
                    iinvd.row_id = int.Parse(row_id);
                    iinvd.prod_qty = iinvd.prod_qty + int.Parse(prod_qty);
                    int row=SaveIinvd(iinvd);
                    return row + int.Parse(prod_qty);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetIinvdCount-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int SaveIinvd(IinvdQuery query)
        {
            query.Replace4MySQL();
            int falg = 0;
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.pwy_dte_ctl == "Y")
                {
                    if (query.row_id != 0)
                    {
                        if (query.prod_qty == 0)
                        {
                            sql.AppendFormat("DELETE FROM iinvd WHERE row_id={0};", query.row_id);
                        }
                        else
                        {
                            sql.AppendFormat("UPDATE iinvd  SET prod_qty={0},change_user={2},change_dtim='{3}' WHERE row_id={1};", query.prod_qty, query.row_id, query.change_user, CommonFunction.DateTimeToString(query.change_dtim));
                        }
                    }
                    else
                    {
                        sql.AppendFormat("UPDATE iinvd  SET prod_qty=prod_qty+{0},change_user={2},change_dtim='{3}' WHERE plas_loc_id='{1}';", query.prod_qty, query.plas_loc_id, query.change_user, CommonFunction.DateTimeToString(query.change_dtim));
                    }
                    _access.execCommand(sql.ToString()); falg = 1;
                }
                else
                {
                    if (query.row_id != 0)
                    {
                        sql.Append("update iinvd set prod_qty=" + query.prod_qty + ",change_user=" + query.change_user + ",change_dtim='" + CommonFunction.DateTimeToString(query.change_dtim) + "' where row_id =" + query.row_id);
                        return _access.execCommand(sql.ToString());
                    }
                    else
                    {
                        sql.AppendFormat("SELECT row_id,prod_qty,made_date,cde_dt  FROM iinvd WHERE plas_loc_id='{0}' AND ista_id='A' ORDER BY made_date;", query.plas_loc_id);
                        DataTable table = _access.getDataTable(sql.ToString());
                        string row_id = "";
                        string row_idend = "";
                        int row_id_end_prod_pty = 0;
                        query.prod_qtys = GetProd_qty((int)query.item_id, query.plas_loc_id, "", "");
                        int prod_qty = query.prod_qtys - query.prod_qty;
                        int sc_num_chg=0;
                        int i = 0;
                        IialgQuery iialg = new IialgQuery();
                        IialgMgr _iialgMgr;
                        DateTime date = DateTime.Now;
                        int temp = 0;
                        iialg.loc_id = query.plas_loc_id;
                        iialg.item_id = query.item_id;
                        iialg.iarc_id = "循環盤點";
                        iialg.create_user = query.create_user;
                        iialg.type = 2;

                        for (i = 0; i < table.Rows.Count; i++)
                        {
                            if ((int)table.Rows[i][1] <= prod_qty || (int)table.Rows[i][1] == 0)
                            {
                                row_id += table.Rows[i][0] + ",";
                                prod_qty = prod_qty - (int)table.Rows[i][1];
                                sc_num_chg+=(int)table.Rows[i][1];

                                DateTime.TryParse(table.Rows[i][3].ToString(), out date);
                                iialg.cde_dt = date;
                                int.TryParse(table.Rows[i][1].ToString(), out temp);
                                iialg.qty_o = temp;//原始庫存數量
                                iialg.adj_qty = -temp;
                                iialg.create_dtim = DateTime.Now;
                                iialg.doc_no = "C" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                if(DateTime.TryParse(table.Rows[i][3].ToString(), out date))
                                {
                                    iialg.made_dt = date;
                                }
                                _iialgMgr = new IialgMgr(mySqlConnectionString);
                                _iialgMgr.insertiialg(iialg);
                            }
                            else
                            {
                                row_idend = table.Rows[i][0].ToString();
                                row_id_end_prod_pty = prod_qty;
                                break;
                            }
                        }
                        if (row_id != "")
                        {
                            _access.execCommand("DELETE FROM iinvd WHERE row_id IN(" + row_id.TrimEnd(',') + ")");
                            falg = 1;
                        }
                        if (i != table.Rows.Count)
                        {
                            DateTime.TryParse(table.Rows[i][3].ToString(), out date);
                            iialg.cde_dt = date;
                            int.TryParse(table.Rows[i][1].ToString(), out temp);
                            iialg.qty_o = temp;//原始庫存數量
                            sc_num_chg += row_id_end_prod_pty;
                            iialg.adj_qty = -row_id_end_prod_pty;

                            iialg.create_dtim = DateTime.Now;
                            iialg.doc_no = "C" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            if (DateTime.TryParse(table.Rows[i][3].ToString(), out date))
                            {
                                iialg.made_dt = date;
                            }
                            _iialgMgr = new IialgMgr(mySqlConnectionString);
                            _iialgMgr.insertiialg(iialg);
                            falg= _access.execCommand("update iinvd set prod_qty=prod_qty-" + row_id_end_prod_pty + ",change_user=" + query.change_user + ",change_dtim='" + CommonFunction.DateTimeToString(query.change_dtim) + "' where row_id =" + row_idend);
                        }

                        IstockChangeQuery istock = new IstockChangeQuery();
                        istock.sc_trans_id = iialg.doc_no;
                        istock.item_id = query.item_id;
                        istock.sc_istock_why = 2;
                        istock.sc_trans_type = 2;
                        istock.sc_num_old = query.prod_qtys;//原始庫存數量
                        istock.sc_num_chg = -sc_num_chg;//轉移數量
                        istock.sc_num_new = GetProd_qty((int)query.item_id, query.plas_loc_id, "", "");//結餘數量
                        istock.sc_time = DateTime.Now;
                        istock.sc_user = query.create_user;
                        istock.sc_note = "循環盤點";
                        IstockChangeMgr istockMgr = new IstockChangeMgr(mySqlConnectionString);
                        istockMgr.insert(istock);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->SaveIinvd-->" + ex.Message + sql.ToString(), ex);
            }
            return falg;
        }

        public List<DateTime> GetCde_dt(int row_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                List<DateTime> list = new List<DateTime>();
                sql.AppendFormat("SELECT cde_dt,made_date FROM iinvd WHERE row_id={0};", row_id);
                DataTable table = _access.getDataTable(sql.ToString());
                DateTime cde_dt = DateTime.MinValue;
                if (table.Rows.Count > 0)
                {
                     cde_dt =Convert.ToDateTime(table.Rows[0][0].ToString());
                     list.Add(cde_dt);
                     cde_dt = Convert.ToDateTime(table.Rows[0][1].ToString());
                     list.Add(cde_dt);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetCde_dt-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int GetProd_qty(int item_id, string loc_id, string pwy_dte_ctl,string row_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (pwy_dte_ctl == "Y")
                {
                    sql.AppendFormat("SELECT prod_qty FROM iinvd WHERE row_id={0};",row_id);
                }
                else
                {
                    sql.AppendFormat("SELECT SUM(prod_qty) FROM iinvd WHERE item_id={0} AND ista_id='A' AND plas_loc_id='{1}';", item_id, loc_id);
                }
                
                DataTable table = _access.getDataTable(sql.ToString());
                int prod_qty = 0;
                if (table.Rows.Count > 0)
                {
                    int.TryParse(table.Rows[0][0].ToString(), out prod_qty);
                }
                return prod_qty;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->GetProd_qty-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 盤點薄工作中要盤點的料位
        /// <summary>
        /// 查詢出只要料位有庫存並且未鎖的料位信息
        /// chaojie_1124j 2015/12/04 05:05pm
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public DataTable getVentory(IinvdQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            StringBuilder sbjoin = new StringBuilder();
            try
            {
                if (!String.IsNullOrEmpty(m.startIloc))
                {
                    if (m.startIloc != "" && m.endIloc != "")
                    {
                        sbWhere.AppendFormat(" and (loc.loc_id>='{0}' and loc.loc_id <='{1}') ", m.startIloc, m.endIloc);
                    }
                    if (!string.IsNullOrEmpty(m.lot_no))
                    {
                        sbWhere.AppendFormat(" and SUBSTR(loc.loc_id,6,1) in ('{0}') ", m.lot_no);
                    }
                }
                if (!string.IsNullOrEmpty(m.Firstsd))
                {
                    if (m.Firstsd == "0")
                    {
                        sbWhere.Append("and SUBSTR(loc.loc_id,5,1) in ('1','3','5','7','9')");
                    }
                    else
                    {
                        sbWhere.Append(" and SUBSTR(loc.loc_id,5,1) in ('0','2','4','6','8')");
                    }
                }
                if (m.prepaid != 0)
                {
                    sbWhere.AppendFormat(" and p.prepaid='{0}' ", m.prepaid);
                }
                if (!string.IsNullOrEmpty(m.vender))
                {
                    sbWhere.AppendFormat(" AND (vv.vendor_id ='{0}' OR vv.vendor_name_simple LIKE'%{0}%') ", m.vender);
                    sbjoin.Append(@"	LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id  LEFT JOIN vendor vv ON  v.vendor_id=vv.vendor_id ");
                }
                sbSql.AppendFormat(@" SELECT  loc.item_id, loc.loc_id, loc.product_id, loc.row_id FROM iloc INNER JOIN (
SELECT i.item_id,i.plas_loc_id as 'loc_id',pi.product_id,i.row_id from iinvd i LEFT JOIN product_item pi ON i.item_id = pi.item_id where i.ista_id='A'
) loc ON loc.loc_id=iloc.loc_id  LEFT JOIN product p on p.product_id = loc.product_id  {1} where 1=1   {0}
ORDER BY loc.loc_id,loc.row_id DESC  ", sbWhere.ToString(), sbjoin.ToString());//iloc.lsta_id NOT in ('H','F')
                //UNION
                //SELECT i.item_id,i.loc_id,pi.product_id,'' as 'row_id' from iplas i LEFT JOIN  product_item pi ON i.item_id = pi.item_id
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao-->getVentory-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
        }

        #endregion

    }
    }

