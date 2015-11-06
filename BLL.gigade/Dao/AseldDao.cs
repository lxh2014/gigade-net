using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;
/**
 *chaojie1124j添加 UpdateAseld方法于2014/11/13用於返回插入 iwms_record 表的sql語句,然後執行存儲過程
 *chaojie1124j添加 DecisionBulkPicking 方法于2014/11/17用於判斷調度是否檢完->是否檢夠->寄倉是否完成->裝箱
 */
namespace BLL.gigade.Dao
{
    public class AseldDao : IAseldImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public AseldDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 列表頁查詢
        public List<Model.Query.OrderMasterQuery> GetOrderMasterList(Model.Query.OrderMasterQuery oderMaster, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT dd.deliver_id,CONCAT('D',right(CONCAT('00000000',dd.deliver_id),8)) as 'deliver_code',om.order_id,tp.parameterName as deliver_store_name 
from ticket t 
RIGHT JOIN deliver_master dm on dm.ticket_id = t.ticket_id 
RIGHT JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id 
LEFT JOIN order_detail od on od.detail_id=dd.detail_id 
LEFT JOIN order_slave os on os.slave_id=od.slave_id 
LEFT JOIN order_master om on om.order_id = os.order_id 
LEFT JOIN t_parametersrc tp on t.delivery_store =tp.parameterCode 
WHERE tp.parameterType='Deliver_Store' and t.export_id='{0}' and od.detail_status in (2,3,6) 
and od.item_mode!=1 and dm.work_status=0  ", oderMaster.export_id);
            if (oderMaster.jdtype == 1)//無調度
            {
                if (oderMaster.deliver_type != 0 && oderMaster.deliver_type != null)//是否選擇了這個條件
                {
                    sb.AppendFormat(@" and t.delivery_store='{0}'", oderMaster.deliver_type);
                }
                sb.AppendFormat(@" and dd.deliver_id not in 
(SELECT dd.deliver_id from ticket t 
right JOIN deliver_master dm on dm.ticket_id = t.ticket_id 
RIGHT JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id 
LEFT JOIN order_detail od on od.detail_id=dd.detail_id 
where od.product_mode=3  and t.export_id='{0}' and od.item_mode!=1 and od.detail_status in (2,3,6) and dm.work_status=0)
GROUP BY dd.deliver_id order by om.order_id,dd.deliver_id ASC, 
od.product_mode DESC ", oderMaster.export_id);
            }
            else//有調度
            {
                if (oderMaster.deliver_type != 0 && oderMaster.deliver_type != null)//是否選擇了這個條件
                {
                    sb.AppendFormat(@" and t.delivery_store='{0}'", oderMaster.deliver_type);
                }
                sb.AppendFormat(@" and dd.deliver_id in 
(SELECT dd.deliver_id from ticket t 
right JOIN deliver_master dm on dm.ticket_id = t.ticket_id 
RIGHT JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id 
LEFT JOIN order_detail od on od.detail_id=dd.detail_id 
where od.product_mode=3  and t.export_id='{0}' and od.item_mode!=1 and od.detail_status in (2,3,6) and dm.work_status=0)
GROUP BY dd.deliver_id order by om.order_id,dd.deliver_id ASC, 
od.product_mode DESC ", oderMaster.export_id);
            }
            totalCount = 0;
            try
            {
                if (oderMaster.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", oderMaster.Start, oderMaster.Limit);
                }
                return _access.getDataTableForObj<Model.Query.OrderMasterQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.GetOrderMasterList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public List<Model.Query.OrderMasterQuery> GetAllOrderDetail(Model.Query.OrderMasterQuery oderMaster)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" SELECT om.order_id,od.product_mode from ticket t 
            right JOIN deliver_master dm on dm.ticket_id = t.ticket_id 
            RIGHT JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id 
            LEFT JOIN order_detail od on od.detail_id=dd.detail_id 
            LEFT JOIN order_slave os on os.slave_id=od.slave_id 
            LEFT JOIN order_master om on om.order_id = os.order_id 
            LEFT JOIN t_parametersrc tp on t.delivery_store =tp.parameterCode  
            WHERE tp.parameterType='Deliver_Store' and  t.export_id='{0}' and od.detail_status in (2,3,6) and od.item_mode!=1 and dm.work_status=0 
            and dd.deliver_id='{1}' ", oderMaster.export_id, oderMaster.deliver_id);//1表示自出
            if (oderMaster.jdtype == 1)
            {
                sb.AppendFormat(" and od.product_mode =2 ");//無調度
            }
            else if (oderMaster.jdtype == 2)
            {
                sb.AppendFormat(" and od.product_mode!=1 ");
            }
            else
            {
                sb.AppendFormat(" and od.product_mode!=1  ");
            }
            if (oderMaster.deliver_type != 0 && oderMaster.deliver_type != null)
            {
                sb.AppendFormat(" and t.delivery_store='{0}' ", oderMaster.deliver_type);
            }
            else
            {
                sb.AppendFormat(" ");
            }
            try
            {
                return _access.getDataTableForObj<Model.Query.OrderMasterQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.GetAllOrderDetail-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 生成理貨單
        public string Insert(Aseld m)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"INSERT INTO aseld (dc_id,whse_id,ord_id,sgmt_id,ordd_id,cust_id,item_id,prdd_id,assg_id,sety_id,
unit_ship_cse,prod_cub,prod_wgt,prod_qty,sel_loc,ckpt_id,curr_pal_no,cse_lbl_lmt,wust_id,lic_plt_id,
description,prod_sz,hzd_ind,cust_name,order_type_id,stg_dcpt_id,stg_dcpd_id,invc_id,route_id,stop_id,
batch_id,batch_seq,start_dtim,complete_dtim,change_dtim,change_user,create_dtim,create_user,ord_msg_id,door_dcpd_id,
door_dcpt_id,catch_wgt_cntl,lot_no,commodity_type,sect_id,ucn,hzd_class,pkde_id,ord_rqst_del_dt,ord_rqst_del_tim,
spmd_id,flow_dcpt_id,flow_dcpd_id,flow_assg_flg,sel_seq_loc,out_qty,eqpt_class_id,sel_x_coord,sel_y_coord,sel_z_coord,
upc_id,ft_id,ftd_id,act_pick_qty,ord_qty,family_group,deliver_id,deliver_code) VALUES (");
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", m.dc_id, m.whse_id, m.ord_id, m.sgmt_id, m.ordd_id, m.cust_id, m.item_id, m.prdd_id, m.assg_id, m.sety_id);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", m.unit_ship_cse, m.prod_cub, m.prod_wgt, m.prod_qty, m.sel_loc, m.ckpt_id, m.curr_pal_no, m.cse_lbl_lmt, m.wust_id, m.lic_plt_id);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", m.description, m.prod_sz, m.hzd_ind, m.cust_name, m.order_type_id, m.stg_dcpd_id, m.stg_dcpt_id, m.invc_id, m.route_id, m.stop_id);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", m.batch_id, m.batch_seq, Common.CommonFunction.DateTimeToString(m.start_dtim), Common.CommonFunction.DateTimeToString(m.complete_dtim), Common.CommonFunction.DateTimeToString(m.change_dtim), m.change_user, Common.CommonFunction.DateTimeToString(m.create_dtim), m.create_user, m.ord_msg_id, m.door_dcpd_id);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", m.door_dcpt_id, m.catch_wgt_cntl, m.lot_no, m.commodity_type, m.sect_id, m.ucn, m.hzd_class, m.pkde_id, Common.CommonFunction.DateTimeToString(m.ord_rqst_del_dt), Common.CommonFunction.DateTimeToString(m.ord_rqst_del_tim));
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", m.spmd_id, m.flow_dcpt_id, m.flow_dcpd_id, m.flow_assg_flg, m.sel_seq_loc, m.out_qty, m.eqpt_class_id, m.sel_x_coord, m.sel_y_coord, m.sel_z_coord);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", m.upc_id, m.ft_id, m.ftd_id, m.act_pick_qty, m.ord_qty, m.family_group, m.deliver_id, m.deliver_code);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao-->Insert-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public DataTable SelOrderDetail(string id, string fre, int radioselect)
        {
            StringBuilder sb = new StringBuilder();
            try
            {//t.ticket_id
                sb.AppendFormat(@"SELECT  od.item_id,od.item_mode,od.detail_id,case when i.loc_id is null then (CASE(od.product_mode) when 2 THEN 'YY999999' when 3 then 'ZZ999999' END) else i.loc_id end as 'sel_loc',od.product_name as 'description',od.product_spec_name as 'prod_sz',om.delivery_name as 'cust_name',od.buy_num,od.parent_num,om.user_id as 'cust_id',iu.upc_id,od.product_mode,om.order_id,dm.deliver_id,om.user_id  from deliver_master dm 
LEFT JOIN deliver_detail dd ON dm.deliver_id=dd.deliver_id
LEFT JOIN order_detail od ON od.detail_id = dd.detail_id
LEFT JOIN iplas i ON od.item_id = i.item_id 
LEFT JOIN order_slave os ON os.slave_id = od.slave_id
LEFT JOIN (SELECT item_id,max(upc_id)as upc_id from iupc GROUP BY iupc.item_id)  iu ON od.item_id = iu.item_id 
LEFT JOIN order_master om on om.order_id=os.order_id 
LEFT JOIN ticket t on t.ticket_id=dm.ticket_id
WHERE dm.deliver_id IN ({0}) and t.export_id='{1}' and od.detail_status in (2,3,6) AND od.item_mode in (0,2) AND dm.work_status=0 ", id, fre);
                if (radioselect == 1)//1表示沒有調度
                {
                    sb.AppendFormat(" and od.product_mode=2 ");
                }
                else if (radioselect == 2)
                {
                    sb.AppendFormat("  ");
                }

                sb.AppendFormat(" ORDER BY sel_loc ;");
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.SelOrderDetail-->" + ex.Message + sb.ToString(), ex);
            }

        }
        public int InsertSql(string sql)
        {//事物插入數據
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
        public string UpdTicker(string id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" set sql_safe_updates = 0;UPDATE deliver_master SET work_status='1' WHERE deliver_id IN ({0});  set sql_safe_updates = 1; ", id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao-->UpdTicker-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 理貨員工作

        #region 根據工作代號查找數據
        public List<AseldQuery> GetAseldList(Aseld a)
        {
            StringBuilder sb = new StringBuilder();//left join iloc ic on i.plas_loc_id=ic.loc_id 
            sb.AppendFormat(@"SELECT seld_id,assg_id,case when ip.loc_id is null then 'YY999999' else ip.loc_id end as sel_loc,CONCAT('(',a.item_id,')',v.brand_name,'-',p.product_name) as description,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,ord_qty,out_qty,ord_id,a.item_id,ordd_id,upc_id,i.cde_dt,pe.cde_dt_shp,deliver_id,deliver_code,o.note_order,ic.hash_loc_id 
FROM aseld a LEFT JOIN iinvd i ON a.item_id=i.item_id 

LEFT JOIN product_ext pe ON i.item_id = pe.item_id 
LEFT JOIN iplas ip on a.item_id=ip.item_id 
left join iloc ic on ip.loc_id=ic.loc_id  
LEFT JOIN product_item pi ON a.item_id = pi.item_id 
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id
LEFT JOIN product p ON pi.product_id=p.product_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
LEFT JOIN order_master o ON a.ord_id=o.order_id
            WHERE assg_id='{0}' AND wust_id<>'COM' AND commodity_type='2' and scaned='0' ORDER BY sel_loc,seld_id LIMIT 1;", a.assg_id);
            try
            {
                return _access.getDataTableForObj<AseldQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.GetAseldList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據工作代號、產品條碼查找數據
        public List<AseldQuery> GetAseldListByItemid(Aseld a)
        {
            StringBuilder sb = new StringBuilder();//left join iloc ic on i.plas_loc_id=ic.loc_id 
            sb.AppendFormat(@"SELECT seld_id,assg_id,case when ip.loc_id is null then 'YY999999' else ip.loc_id end as sel_loc,CONCAT('(',a.item_id,')',v.brand_name,'-',p.product_name) as description,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,ord_qty,out_qty,ord_id,a.item_id,ordd_id,upc_id,i.cde_dt,pe.cde_dt_shp,deliver_id,deliver_code,o.note_order,ic.hash_loc_id 
FROM aseld a LEFT JOIN iinvd i ON a.item_id=i.item_id 

LEFT JOIN product_ext pe ON i.item_id = pe.item_id 
LEFT JOIN iplas ip on a.item_id=ip.item_id 
left join iloc ic on ip.loc_id=ic.loc_id  
LEFT JOIN product_item pi ON a.item_id = pi.item_id 
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id
LEFT JOIN product p ON pi.product_id=p.product_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
LEFT JOIN order_master o ON a.ord_id=o.order_id
            WHERE assg_id='{0}' AND wust_id<>'COM' AND commodity_type='2' and scaned='0' ", a.assg_id);
            if (a.item_id != 0)
            {
                sb.AppendFormat(" and a.item_id='{0}' ",a.item_id);
            }
            sb.AppendFormat(" ORDER BY sel_loc,seld_id LIMIT 1;");
            
            try
            {
                return _access.getDataTableForObj<AseldQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.GetAseldListByItemid-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        /// <summary>
        /// 調度頁面數據
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderProductInformation(AseldQuery ase)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT assg_id,ase.seld_id,CONCAT(v.brand_name,'-',p.product_name) as description,ase.prod_sz,ase.ord_qty,ase.out_qty,ase.ordd_id,");
            sql.AppendFormat(@"pe.cde_dt_shp,pe.pwy_dte_ctl,ase.deliver_id,ase.deliver_code,o.note_order from aseld ase");
            sql.AppendLine(@" left JOIN product_ext pe on pe.item_id=ase.item_id  ");
            sql.AppendLine(@"  LEFT JOIN product_item pi ON pi.item_id=ase.item_id  ");
            sql.AppendLine(@" LEFT JOIN product p ON pi.product_id=p.product_id   ");
            sql.AppendLine(@" LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id  ");
            sql.AppendLine(@"  LEFT JOIN order_master o ON ase.ord_id=o.order_id  ");           

            if (!string.IsNullOrEmpty(ase.upc_id))
            {
                sql.AppendLine(@" left JOIN iupc upc on upc.item_id=ase.item_id ");
            }
            sql.AppendFormat(@"where ord_id='{0}' and commodity_type=3 and wust_id <>'COM'", ase.ord_id);
            if (ase.item_id != 0)
            {
                sql.AppendFormat(@" and ase.item_id='{0}'", ase.item_id);
            }
            if (!string.IsNullOrEmpty(ase.upc_id))
            {
                sql.AppendFormat(@" and upc.upc_id='{0}'", ase.upc_id);
            }
            if (!string.IsNullOrEmpty(ase.deliver_code))
            {
                sql.AppendFormat(@" and deliver_code='{0}'", ase.deliver_code);
            }
            if (!string.IsNullOrEmpty(ase.freight_set) && ase.freight_set!="0")
            {
                sql.AppendFormat(@" and assg_id like '{0}%' ", ase.freight_set);
            }
            sql.AppendLine(@" limit 1 ");
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseidDao.GetOrderProductInformation-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #region 處理aseld庫存
        public string UpdAseld(Aseld m)
        {//初始值AVL.ABN放弃不回头捡,留到下次启动JOB再捡.SKP跳过料位,待会回头捡.BSY正在捡这个料位.料位的预期捡货量满足了才变COM.
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"  UPDATE aseld SET wust_id='{1}',out_qty='{2}',act_pick_qty='{3}',change_dtim='{4}',scaned='1' WHERE seld_id='{0}';  ", m.seld_id, m.wust_id, m.out_qty, m.act_pick_qty, CommonFunction.DateTimeToString(DateTime.Now));
            try
            {
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.UpdAseld-->" + ex.Message + sb.ToString(), ex);
            }
        }
        //是否還有沒有撿的商品
        public int SelCom(Aseld m)
        {//
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select item_id from aseld WHERE assg_id='{0}' and ord_id='{1}' and wust_id <> 'COM' AND deliver_id='{2}' ; ", m.assg_id, m.ord_id, m.deliver_id);
            try
            {
                return _access.getDataTable(sb.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.SelCom-->" + ex.Message + sb.ToString(), ex);
            }
        }
        //該項目訂單是否完成
        public int SelComA(Aseld m)
        {//and scaned<>'0'
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select item_id from aseld WHERE assg_id='{0}' and commodity_type=2 and  wust_id<>'COM' and scaned = '0'  ; ", m.assg_id);
            try
            {
                return _access.getDataTable(sb.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.SelComA-->" + ex.Message + sb.ToString(), ex);
            }
        }
        //該項目訂單是否有臨時退/換貨的商品
        public int SelComC(Aseld m)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select a.item_id from aseld a 
LEFT JOIN order_slave os ON a.ord_id=os.order_id
LEFT JOIN order_detail od ON os.slave_id=od.slave_id AND a.item_id=od.item_id
 WHERE assg_id='{0}' and a.ord_id='{1}'  AND od.detail_status in (90,10,89,20,91,92) ;", m.assg_id, m.ord_id);
            try
            {
                return _access.getDataTable(sb.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.SelComC-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        /// <summary>
        /// 检一次货物，插入一条数据进入检货记录
        /// </summary>
        /// <param name="a">实体，通过订单编号和条码编号修改</param>
        /// <returns></returns>
        public string AddIwsRecord(AseldQuery m)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //m.item_id;
                sb.AppendFormat("INSERT INTO iwms_record (order_id,detail_id,act_pick_qty,cde_dt,status,create_date,create_user_id,cde_dt_incr,made_dt,cde_dt_shp) VALUES(");
                sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", m.ord_id, m.ordd_id, m.act_pick_qty, CommonFunction.DateTimeToString(m.cde_dt), 1, CommonFunction.DateTimeToString(DateTime.Now), m.change_user, m.cde_dt_incr, CommonFunction.DateTimeToString(m.made_dt),m.cde_dt_shp);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("AseidDao-->AddIwsRecord-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int SelectCount(Aseld m)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select item_id from aseld where ord_id='{0}' and commodity_type=3 and wust_id !='COM' ", m.ord_id);
                return _access.getDataTable(sql.ToString()).Rows.Count;//返回條數
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao-->SelectCount-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 通過item_id查詢product_ext里的數據
        /// </summary>
        /// <param name="ase"></param>
        /// <returns></returns>

        #region 判斷調度是否完成
        /// <summary>
        /// 判斷調度是否檢完，檢完了是否檢夠，檢夠了訂單下的寄倉+調度是否檢夠，然後返回是否可以裝箱
        /// </summary>
        /// <param name="ase">實體</param>
        /// <param name="commodity_type">調度 3</param>
        /// <returns></returns>
        public int DecisionBulkPicking(AseldQuery ase, int commodity_type)//判断检货
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 判断调度检货
                sb.AppendFormat(@"select seld_id from aseld where ord_id='{0}' and commodity_type='{1}' and deliver_id='{2}' and wust_id <>'COM' and scaned='0';", ase.ord_id, commodity_type, ase.deliver_id);/*调度，先判断检完没有*/
                DataTable result = _access.getDataTable(sb.ToString());
                sb.Clear();
                if (result.Rows.Count > 0)//调度未检的货物
                {
                    //訂單下的商品未撿完，繼續循環
                    return 1;
                }
                //撿完
                else
                {
                    sb.AppendFormat(@"select seld_id from aseld where ord_id='{0}' and commodity_type='{1}' and wust_id <> 'COM';", ase.ord_id, commodity_type);/*调度，先判断检完没有*/
                    DataTable dt = _access.getDataTable(sb.ToString());
                    sb.Clear();
                    if (dt.Rows.Count == 0)
                    {
                        sb.AppendFormat(@"select seld_id from aseld where ord_id='{0}' and wust_id <> 'COM' ;", ase.ord_id, commodity_type);/*查询此訂單下所有商品，不管寄仓或调度*/
                        DataTable result2 = _access.getDataTable(sb.ToString());
                        if (result2.Rows.Count == 0)
                        {
                            if (SelComC(ase) == 0)
                            {
                                return 3;//提示所有商品<寄仓+调度>已经检够。可以装箱
                            }
                            else
                            {
                                return 5;//提示有退換貨商品,請示領導!
                            }
                        }
                        else
                        {
                            return 4;//此訂單下尚有寄倉商品未撿完
                        }

                    }
                    else
                    {
                        //此訂單下調度商品已撿完,但有商品缺貨
                        return 2;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao-->DecisionBulkPicking-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 第二次進行揀貨時把scaned還原成0
        public int UpdScaned(Aseld a)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(a.assg_id))
                {
                    sb.AppendFormat(" set sql_safe_updates = 0;UPDATE aseld SET scaned='0' where assg_id ='{0}' and wust_id !='COM'; set sql_safe_updates = 1; ", a.assg_id);
                }
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao-->UpdScaned-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #endregion

        #region 未完成理貨工作缺貨明細
        /// <summary>
        /// 缺貨明細報表
        /// </summary>
        /// <returns></returns>
        public DataTable GetNComJobDetail(string jobNumbers)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT assg_id as '工作代號',deliver_code as '出貨單號',CASE when assg_id LIKE 'N%' THEN '常溫' when assg_id LIKE 'F%' THEN '冷凍' END AS '溫層',");
            sql.AppendLine(@"ord_id as '訂單號',aseld.item_id as '品號',CONCAT(v.brand_name,'-',p.product_name) as '品名',concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as '規格',");
            sql.AppendLine(@"CASE when plas.loc_id IS NULL THEN CASE(commodity_type) when 2 THEN 'YY999999' when 3 then 'ZZ999999' END  ELSE plas.loc_id END as '料位'  ,ord_qty as '訂貨量',act_pick_qty as '已撿數量',out_qty as '缺貨數量', ");
            sql.AppendLine(@"CASE(commodity_type) when 2 THEN '寄倉' WHEN 3 THEN '調度' END AS '寄倉/調度' ");
            sql.AppendLine(@" from aseld LEFT JOIN  iplas plas ON plas.item_id=aseld.item_id ");
            sql.AppendFormat(@" LEFT JOIN product_item pi ON pi.item_id=aseld.item_id
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id 
LEFT JOIN product p ON pi.product_id=p.product_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
");

            sql.AppendFormat("where wust_id <>'COM' ");
            if (!string.IsNullOrEmpty(jobNumbers))
            {
                sql.AppendFormat(" AND assg_id in({0})", jobNumbers);
            }
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.GetNComJobDetail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 缺貨總報表
        /// </summary>
        /// <returns></returns>
        public DataTable GetNComJobSimple()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT ase.assg_id as '工作代號',CASE when ase.assg_id LIKE 'N%' THEN '常溫' WHEN ase.assg_id LIKE 'F%' THEN '冷凍' END AS '溫層',");
            sql.AppendLine(@"SUM(out_qty) as'缺貨數量' ,CASE(commodity_type) when 2 THEN '寄倉' WHEN 3 THEN '調度' END AS '寄倉/調度',SUBSTRING(am.create_time,1,10 )as '產生時間'");
            sql.AppendLine(@" from aseld_master am INNER JOIN aseld ase on am.assg_id =ase.assg_id");
            sql.AppendLine(@" where wust_id <>'COM'  GROUP BY ase.assg_id, commodity_type");
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.GetNComJobSimple-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion

        public DataTable ExportDeliveryStatement(int counts, int types)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT dm.export_id,t.ticket_id,om.order_id,od.item_id,ips.loc_id,pt.product_id,
CONCAT(v.brand_name,'-',pt.product_name) AS 'product_name',concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,
vd.vendor_name_full,od.product_mode,sum(od.buy_num) as buy_num  from ticket t 
right JOIN deliver_master dm on dm.ticket_id = t.ticket_id 
right JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id 
left JOIN order_detail od on od.detail_id=dd.detail_id 
left JOIN order_slave os on os.slave_id=od.slave_id 
left JOIN order_master om on om.order_id = os.order_id 
left JOIN iplas ips on od.item_id=ips.item_id 
left JOIN product_item ptim  on ptim.item_id=od.item_id 
LEFT JOIN product_spec ps1 ON ptim.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON ptim.spec_id_2 = ps2.spec_id
left JOIN product pt on pt.product_id = ptim.product_id 
left JOIN vendor vd on vd.vendor_id =os.vendor_id
LEFT JOIN vendor_brand v ON pt.brand_id=v.brand_id
WHERE  od.detail_status in (2,3,6) and od.product_mode!=1 and dm.export_id='{0}'
GROUP BY od.item_id ORDER BY  product_mode ASC ,buy_num DESC limit {1};", types,counts);
            try
            {
                DataTable _dt = _access.getDataTable(sb.ToString());
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.ExportDeliveryStatement-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int UpdateScnd(Aseld m)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" set sql_safe_updates = 0; UPDATE aseld SET scaned='0',change_dtim='{1}',change_user='{2}' WHERE ord_id='{0}'; set sql_safe_updates = 1; ", m.ord_id, CommonFunction.DateTimeToString(m.change_dtim), m.change_user);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.UpdateScnd-->" + ex.Message + sb.ToString(), ex);
            }
        }

        #region 商品剛進變狀態
        public int Updwust(Aseld m)
        {//BSY正在捡这个料位S.
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" UPDATE aseld SET wust_id='{1}',change_dtim='{2}',create_user='{3}' WHERE seld_id='{0}'; ", m.seld_id, m.wust_id, CommonFunction.DateTimeToString(DateTime.Now), m.create_user);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.Updwust-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion


        public DataTable ExportAseldMessage(AseldQuery ase)
        {
            StringBuilder strsb = new StringBuilder();
            if (!string.IsNullOrEmpty(ase.assg_id))
            {
                strsb.AppendFormat(" and asd.assg_id='{0}' ", ase.assg_id);
            }
            if (!string.IsNullOrEmpty(ase.commodity_type))
            {
                strsb.AppendFormat(" and asd.commodity_type ='{0}'", ase.commodity_type);
            }
            strsb.Append(" order by loc_id;");
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
SELECT CONCAT(v.brand_name,'-',p.product_name) AS 'product_name',asd.ord_id,asd.assg_id,asd.item_id,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,
CASE when plas.loc_id IS NULL THEN CASE(commodity_type) when 2 THEN 'YY999999' when 3 then 'ZZ999999' END  ELSE plas.loc_id END loc_id,asd.deliver_code,
asd.wust_id, asd.out_qty,asd.commodity_type
FROM aseld  asd 
LEFT JOIN product_item pi on asd.item_id =pi.item_id
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id
LEFT JOIN product p on pi.product_id =p.product_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id
LEFT JOIN iplas plas ON plas.item_id=asd.item_id WHERE asd.wust_id <> 'COM' ");
            //LEFT JOIN (select item_id,upc_id from iupc GROUP BY item_id) upc on upc.item_id= pi.item_id   upc.upc_id,

            sb.Append(strsb.ToString());
            try
            {
                DataTable _dt = _access.getDataTable(sb.ToString());
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.ExportAseldMessage-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public void ConsoleAseldBeforeInsert(int detail_id)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("set sql_safe_updates = 0; delete from aseld  where ordd_id={0};set sql_safe_updates = 1; ", detail_id);
            try
            {
                _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.ConsoleAseldBeforeInsert-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        public string Getfreight(string ord_id)
        {
            StringBuilder sbSql = new StringBuilder();
            DataTable dt=new DataTable();
            try
            {
                sbSql.AppendFormat("SELECT DISTINCT assg_id from aseld WHERE ord_id='{0}';", ord_id);
                dt=_access.getDataTable(sbSql.ToString());
                if(dt.Rows.Count ==1)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AseldDao.ConsoleAseldBeforeInsert-->" + ex.Message + sbSql.ToString(), ex);
            }
        
        }
    }
}
