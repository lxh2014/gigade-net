#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：OrderBrandProducesDao.cs        
*摘要 
*
* 品牌訂單查詢與資料庫的交互
*當前版本：v1.1 
*
版本號：每次修改文件之後需要將版本號+1
* 作 者：changjian0408j                                          
      
* 完成日期：2014/8/20
* 
* 修改歷史
* v1.1修改日期：
*         
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;
using System.Data;
using BLL.gigade.Common;
namespace BLL.gigade.Dao
{
    public class OrderBrandProducesDao
    {
        public IDBAccess _accessMySql;
        private string connStr;
        public OrderBrandProducesDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }

        #region 品牌訂單查詢
        public List<OrderBrandProducesQuery> GetOrderBrandProduces(OrderBrandProducesQuery store, out int totalCount, string conditionStr)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            List<OrderBrandProducesQuery> list = new List<OrderBrandProducesQuery>();
            StringBuilder search = new StringBuilder();
            try
            {
                //SELECT om.order_id,os.slave_id,od.detail_id,od.item_id,vb.brand_name,v.vendor_name_simple,od.product_name,od.product_spec_name,od.item_mode,pm.parameterName as 'payments',od.event_cost,od.single_cost,od.single_money,od.buy_num,parent_num,od.deduct_bonus,od.deduct_welfare,om.order_name,st.remark as 'states',FROM_UNIXTIME(om.order_createdate) as order_createdates,FROM_UNIXTIME(om.order_date_pay) as order_date_pays,FROM_UNIXTIME(os.slave_date_delivery) as slave_date_deliverys,om.note_order

                sqlcount.Append(@" SELECT Count(od.detail_id) as count FROM order_master om ");
                sql.Append(@" SELECT od.detail_id,om.user_id,od.slave_id,od.item_id,od.item_vendor_id,od.product_freight_set,od.product_mode,od.product_name,od.product_spec_name,od.single_cost,od.event_cost,od.single_price,od.single_money,od.single_money as SingleMoney ,od.deduct_bonus,od.deduct_welfare,od.deduct_happygo,od.deduct_happygo_money,od.deduct_account,od.deduct_account_note,od.accumulated_bonus,od.accumulated_happygo,od.buy_num,od.detail_status,od.detail_note,od.item_code,od.arrival_status,od.delay_till,od.lastmile_deliver_serial,od.lastmile_deliver_datetime,od.lastmile_deliver_agency,od.bag_check_money,od.channel_detail_id,od.combined_mode,od.item_mode,od.parent_id,od.parent_name,od.parent_num,od.price_master_id,od.pack_id,od.sub_order_id,os.slave_status, FROM_UNIXTIME(os.slave_date_delivery) as slave_date_deliverys,os.order_id,om.user_id,FROM_UNIXTIME(om.order_createdate) as order_createdates,om.order_name,om.order_mobile,om.note_order,om.order_payment , om.delivery_name,om.delivery_gender, om.delivery_zip,om.delivery_address,v.vendor_name_simple,FROM_UNIXTIME(om.order_date_pay) as order_date_pays,vb.brand_name FROM order_master om ");
                sqlfrom.AppendFormat(@" LEFT JOIN order_slave os ON om.order_id=os.order_id LEFT JOIN order_detail od ON os.slave_id=od.slave_id LEFT JOIN vendor v  on  v.vendor_id=od.item_vendor_id LEFT JOIN product_item pi on pi.item_id=od.item_id LEFT JOIN product p ON p.product_id=pi.product_id 
LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id WHERE 1=1 ");
                //, st.remark 'states',pm.parameterName 'payments' LEFT JOIN (select parameterCode,parameterName from  t_parametersrc  where parameterType='payment') pm on om.order_payment=pm.parameterCode LEFT JOIN (select parameterCode,remark from  t_parametersrc  where parameterType='order_status') st on os.slave_status =st.parameterCode
                #region 條件
                if (!string.IsNullOrEmpty(store.searchcon))
                {
                    switch (store.selecttype)
                    {
                        case "1":
                            sqlwhere.AppendFormat(" AND od.product_name LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "2":
                            sqlwhere.AppendFormat(" AND om.user_id LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "3":
                            sqlwhere.AppendFormat(" AND vb.brand_name LIKE  '%{0}%' ", store.searchcon);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(store.date_type))
                {
                    switch (store.date_type)
                    {
                        case "1":
                            if (store.dateOne > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate >= '{0}' ", CommonFunction.GetPHPTime(store.dateOne.ToString()));
                            }
                            if (store.dateTwo > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}' ", CommonFunction.GetPHPTime(store.dateTwo.ToString()));
                            }
                            break;
                        case "2":
                            if (store.dateOne > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay >= '{0}' ", CommonFunction.GetPHPTime(store.dateOne.ToString()));
                            }
                            if (store.dateTwo > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay <= '{0}' ", CommonFunction.GetPHPTime(store.dateTwo.ToString()));
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(store.slave) && store.slave != "-1")
                {//訂單狀態
                    sqlwhere.AppendFormat(" and os.slave_status='{0}'", store.slave);
                }
                if (store.order_payment > 0)
                {//付款方式
                    sqlwhere.AppendFormat(" and order_payment='{0}'", store.order_payment);
                }
                #endregion
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sqlcount.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                    }
                    sqlwhere.AppendFormat(" ORDER BY om.order_createdate DESC  limit {0},{1}", store.Start, store.Limit);
                }
                return _accessMySql.getDataTableForObj<OrderBrandProducesQuery>(sql.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderBrandProducesDao-->GetOrderBrandProduces-->" + sql.ToString() + sqlfrom.ToString() + sqlwhere.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 品牌訂單csv匯出
        public List<OrderBrandProducesQuery> OrderBrandProducesExport(OrderBrandProducesQuery store, string conditionStr)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try 
	        {
                //sql.AppendFormat(@"SELECT od.detail_id,od.slave_id,od.item_id,od.item_vendor_id,  od.product_freight_set,od.product_mode,od.product_name,od.product_spec_name, od.single_cost,od.event_cost,od.single_price,od.single_money, od.deduct_bonus,od.deduct_welfare,od.deduct_happygo,od.deduct_happygo_money, od.deduct_account,od.deduct_account_note,od.accumulated_bonus,od.accumulated_happygo, od.buy_num,od.detail_status,od.detail_note,od.item_code,od.arrival_status,od.delay_till, od.lastmile_deliver_serial,od.lastmile_deliver_datetime,od.lastmile_deliver_agency,od.bag_check_money, od.channel_detail_id,od.combined_mode,od.item_mode,od.parent_id,od.parent_name, od.parent_num,od.price_master_id,od.pack_id,od.sub_order_id,  os.slave_status, st.remark 'states', FROM_UNIXTIME(os.slave_date_delivery) as slave_date_deliverys,os.order_id, om.user_id,FROM_UNIXTIME(om.order_createdate) as order_createdates,om.order_name,om.order_mobile,om.note_order, om.note_admin,om.order_payment, pm.parameterName 'payments', om.delivery_name,case om.delivery_gender when '1' then '男' else '女'end as delivery_genders,om.delivery_zip, om.delivery_address,om.delivery_mobile,om.delivery_phone,v.vendor_name_simple,FROM_UNIXTIME(om.order_date_pay) as order_date_pays,vb.brand_name, FROM_UNIXTIME(u.user_reg_date) as user_reg_dates,concat(u.user_birthday_year,'/',u.user_birthday_month,'/',u.user_birthday_day) as user_birthday,u.user_email,CONCAT(big,small) as delivery_zips  ");
                sql.AppendFormat(@"SELECT od.product_freight_set,od.product_name,od.product_spec_name, od.single_cost,od.event_cost,od.deduct_welfare,od.deduct_happygo_money,od.single_money, od.deduct_bonus,od.buy_num,st.remark 'states', FROM_UNIXTIME(os.slave_date_delivery) as slave_date_deliverys,os.order_id,FROM_UNIXTIME(om.order_createdate) as order_createdates,om.order_name,om.note_order, om.note_admin,pm.parameterName 'payments', om.delivery_name,case om.delivery_gender when '1' then '男' else '女'end as delivery_genders,om.delivery_address,om.delivery_mobile,om.delivery_phone,v.vendor_name_simple,FROM_UNIXTIME(om.order_date_pay) as order_date_pays,vb.brand_name,od.parent_num,od.item_mode, FROM_UNIXTIME(u.user_reg_date) as user_reg_dates,concat(u.user_birthday_year,'/',u.user_birthday_month,'/',u.user_birthday_day) as user_birthday,u.user_email,CONCAT(zipcode,middle,small) as delivery_zips  ");
                sql.AppendFormat(" FROM order_detail od LEFT JOIN order_slave os ON os.slave_id=od.slave_id LEFT JOIN order_master om ON os.order_id=om.order_id LEFT JOIN vendor v  on  v.vendor_id=od.item_vendor_id LEFT JOIN product_item pi on pi.item_id=od.item_id LEFT JOIN product p ON p.product_id=pi.product_id LEFT JOIN (select * from  t_parametersrc  where parameterType='payment') pm on pm.parameterCode=om.order_payment  LEFT JOIN (select * from  t_parametersrc  where parameterType='order_status') st on st.parameterCode=os.slave_status LEFT JOIN users u on u.user_id=om.user_id  LEFT JOIN t_zip_code  tzc ON om.delivery_zip=tzc.zipcode LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id  WHERE 1=1 " + conditionStr);
                #region 條件
                if (!string.IsNullOrEmpty(store.searchcon))
                {
                    switch (store.selecttype)
                    {
                        case "1":
                            sqlwhere.AppendFormat(" AND od.product_name LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "2":
                            sqlwhere.AppendFormat(" AND om.user_id LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "3":
                            sqlwhere.AppendFormat(" AND vb.brand_name LIKE  '%{0}%' ", store.searchcon);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(store.date_type))
                {
                    switch (store.date_type)
                    {
                        case "1":
                            if (store.dateOne > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate >= '{0}' ", CommonFunction.GetPHPTime(store.dateOne.ToString()));
                            }
                            if (store.dateTwo > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}' ", CommonFunction.GetPHPTime(store.dateTwo.ToString()));
                            }
                            break;
                        case "2":
                            if (store.dateOne > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay >= '{0}' ", CommonFunction.GetPHPTime(store.dateOne.ToString()));
                            }
                            if (store.dateTwo > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay <= '{0}' ", CommonFunction.GetPHPTime(store.dateTwo.ToString()));
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(store.slave) && store.slave != "-1")
                {//訂單狀態
                    sqlwhere.AppendFormat(" and os.slave_status='{0}'", store.slave);
                }
                else
                {
                    sqlwhere.Append(" and os.slave_status in (0,2,4,6,99) ");
                }
                if (store.order_payment > 0)
                {//付款方式
                    sqlwhere.AppendFormat(" and order_payment='{0}'", store.order_payment);
                }
                sqlwhere.AppendFormat(" ORDER BY brand_sort, brand_name , od.detail_id ASC ");
                #endregion
                return _accessMySql.getDataTableForObj<OrderBrandProducesQuery>(sql.ToString() + sqlwhere);
             }
            catch (Exception ex)
            {
                throw new Exception("OrderBrandProducesDao-->OrderBrandProducesExport-->" + sql.ToString() + sqlwhere+ ex.Message, ex);
            }
        }

        #endregion

        #region 品牌营业额查询
        /// <summary>
        /// 查询一个时间段的某一个品牌的营业额，或者所有品牌的营业额
        /// </summary>chaojie_zz 添加于2014/10/17 04:36 pm
        /// <param name="sqland">sqland 根据一个查询条件而或多链接一个表</param>
        /// <returns>DataTable表</returns>
        public DataTable GetOrderVendorRevenuebyday(string sqlappend,string brand=null)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {               
                sql.AppendFormat(@"SELECT sum(single_money*buy_num) as subtotal,vb.brand_id,vb.vendor_id,om.order_createdate , vb.brand_sort,vb.brand_name,v.vendor_name_simple ");
                string sqlfrom = "  FROM order_detail od, order_slave os, vendor v, order_master om, product_item pi,product p, vendor_brand vb";
                sqlwhere.AppendFormat("  WHERE os.slave_id = od.slave_id AND os.order_id = om.order_id AND v.vendor_id = od.item_vendor_id AND pi.item_id = od.item_id AND p.product_id = pi.product_id");
                sqlwhere.AppendFormat(" AND vb.brand_id = p.brand_id AND item_mode in (0 , 1)");
                string sq = sql.ToString() + sqlfrom + sqlwhere + sqlappend;
                if (brand == null)
                {
                    sq += " group by vb.brand_id,vb.vendor_id ";
                }
                string sqlorder = "  ORDER BY  vb.brand_sort,vb.brand_name";
                sq += sqlorder;
                return _accessMySql.getDataTable(sq);             
             }
            catch (Exception ex)
            {
                throw new Exception("OrderBrandProducesDao-->GetOrderVendorRevenuebyday-->" + sql.ToString()+sqlwhere.ToString() + ex.Message, ex);
            }           
        }
        #endregion

    }
}
