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
    public class OrderVendorProducesDao  
    {
        public IDBAccess _accessMySql;
        private string connStr;

        public OrderVendorProducesDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }
        public List<OrderVendorProducesQuery> GetOrderVendorProduces(OrderVendorProducesQuery store, out int totalCount)/*返回供應商訂單查詢列表*/
        {

            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            List<OrderVendorProducesQuery> list = new List<OrderVendorProducesQuery>();
            StringBuilder search = new StringBuilder();
            try
            {
                sqlcount.Append(@" SELECT count(od.detail_id) as count ");
                sql.Append(@" SELECT od.item_id,om.user_id,od.product_mode,od.product_name,od.product_spec_name,od.single_cost,od.event_cost,od.single_money,od.single_money as SingleMoney,od.deduct_bonus,od.buy_num,od.item_mode,od.parent_num,FROM_UNIXTIME(os.slave_date_delivery) as slave_date_delivery,os.order_id,om.user_id,FROM_UNIXTIME(om.order_createdate) as order_createdate,om.order_name,om.order_mobile,om.note_order,
om.delivery_name,om.delivery_gender,om.delivery_zip,om.delivery_address,v.vendor_name_simple,FROM_UNIXTIME(om.order_date_pay) as order_date_pay,FROM_UNIXTIME(om.money_collect_date) as money_collect_date,mu.user_username as product_manage,om.order_payment,os.slave_status ");
                sqlfrom.AppendFormat(@" FROM order_master om LEFT JOIN order_slave os ON om.order_id=os.order_id LEFT JOIN order_detail od ON os.slave_id = od.slave_id
LEFT JOIN vendor v ON od.item_vendor_id = v.vendor_id LEFT JOIN manage_user mu ON v.product_manage = mu.user_id  ");
                //tp1.remark as slave_status,tp.parameterName as order_payment,   LEFT JOIN t_parametersrc tp ON om.order_payment = tp.parameterCode AND tp.parameterType = 'payment' LEFT JOIN t_parametersrc tp1 ON os.slave_status = tp1.parameterCode AND tp1.parameterType = 'order_status' 
                sqlwhere.AppendFormat(" WHERE  1=1 ");
                #region where
                if (store.Item_Vendor_Id > 0)
                {
                    sqlwhere.AppendFormat(" and od.item_vendor_id='{0}'", store.Item_Vendor_Id);
                }
                if (!string.IsNullOrEmpty(store.searchcon))
                {
                    switch (store.selecttype)
                    {
                        case "1":
                            sqlwhere.AppendFormat(" AND od.product_name LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "2":
                            sqlwhere.AppendFormat(" AND om.user_id LIKE '%{0}%' ", store.searchcon);
                            break;
                        case "3":
                            sqlwhere.AppendFormat(" AND v.vendor_name_simple LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "4":
                            sqlwhere.AppendFormat(" AND od.parent_id ='{0}' OR od.item_id ='{0}' ", store.searchcon);
                            break;
                        case "5":
                            sqlwhere.AppendFormat(" AND om.order_id ='{0}' ", store.searchcon);
                            break;
                        default:
                            break;
                    }
                }
                if (store.slave != "null" && !string.IsNullOrEmpty(store.slave) && store.slave != "-1")
                {
                    sqlwhere.AppendFormat(" and os.slave_status='{0}'", store.slave);
                }
                if (store.order_payment > 0)
                {//付款方式
                    sqlwhere.AppendFormat(" and om.order_payment={0}", store.order_payment);
                }
                if (!string.IsNullOrEmpty(store.product_freight_set_in))
                {//運送方式
                    sqlwhere.AppendFormat(@" AND od.product_freight_set IN ({0}) ", store.product_freight_set_in);
                }
                if (store.product_manage != "0" && !string.IsNullOrEmpty(store.product_manage))
                {//供應商管理者查詢條件
                    sqlwhere.AppendFormat(" AND v.product_manage='{0}'", store.product_manage);
                }
                if (!string.IsNullOrEmpty(store.date_type))
                {//日期條件
                    switch (store.date_type)
                    {
                        case "1":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "2":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.money_collect_date  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.money_collect_date < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "3":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "4":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.slave_date_delivery  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.slave_date_delivery < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "5":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_close  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_close < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sqlcount.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0][0].ToString());
                    }
                    sqlwhere.AppendFormat(" ORDER BY order_createdate DESC, od.detail_id ASC limit {0},{1};", store.Start, store.Limit);
                }
                return _accessMySql.getDataTableForObj<OrderVendorProducesQuery>(sql.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderVendorProducesDao.GetOrderVendorProduces-->" + ex.Message + sql.ToString() + sqlfrom.ToString() + sqlwhere.ToString(), ex);
            }
        }

        public DataTable ExportCsv(OrderVendorProducesQuery store)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                //sql.AppendLine(@"SELECT om.order_id, v.vendor_name_simple,CASE od.product_mode WHEN 1 THEN '供應商自行出貨' WHEN 2 THEN '寄倉' WHEN 3 THEN '調度' END AS product_mode,vb.brand_name, od.product_name, od.detail_id, od.product_spec_name, tppp.parameterName as order_payment,CASE od.item_mode WHEN 1 THEN '父商品' WHEN 2 THEN '子商品' ELSE '單一商品' END AS item_mode,CASE om.holiday_deliver when 0 OR 1 THEN '' END AS cost,od.single_cost,od.event_cost,CASE od.item_mode WHEN 2 THEN od.single_money/od.buy_num ELSE od.single_money END AS single_money,CASE od.item_mode WHEN 2 THEN od.buy_num*od.parent_num ELSE od.buy_num END AS buy_num,od.deduct_bonus, od.deduct_welfare, od.deduct_happygo_money,CASE od.item_mode WHEN 2 THEN od.single_money*od.parent_num-od.deduct_bonus-od.deduct_welfare-od.deduct_happygo_money ELSE od.single_money*od.buy_num-od.deduct_bonus-od.deduct_welfare-od.deduct_happygo_money END as subtotal,CASE od.event_cost when 0 THEN '-' ELSE '是' END AS `event`,  om.order_name, tp.remark as slave_status ,om.delivery_name, CASE om.delivery_gender WHEN 0 THEN '女' WHEN 1 THEN '男' END AS delivery_gender,CONCAT_WS( '', zip.zipcode, zip.middle, zip.small ) AS delivery_zip, om.delivery_address,om.delivery_mobile, om.delivery_phone,tpp.remark AS detail_status, FROM_UNIXTIME(om.order_createdate) AS order_createdate,CASE os.slave_date_delivery WHEN 0 THEN '-' ELSE FROM_UNIXTIME(os.slave_date_delivery) END AS slave_date_delivery,CASE om.order_date_pay WHEN 0 THEN '-' ELSE FROM_UNIXTIME(om.order_date_pay) END AS order_date_pay,CASE om.money_collect_date WHEN 0 THEN '-' ELSE FROM_UNIXTIME(om.money_collect_date) END AS money_collect_date,CASE imr.invoice_date WHEN 0 THEN '-' ELSE FROM_UNIXTIME(imr.invoice_date) END AS invoice_date,CASE os.slave_date_close WHEN 0 THEN '-' ELSE FROM_UNIXTIME(os.slave_date_close) END AS slave_date_close, om.note_order,om.note_admin, CASE od.product_freight_set WHEN 1 OR 3 THEN '常溫' WHEN 2 OR 4 THEN '低溫' END AS product_freight_set,FROM_UNIXTIME(u.user_reg_date) as user_reg_date, CONCAT_WS( '-', u.user_birthday_year, u.user_birthday_month, u.user_birthday_day ) AS user_birthday,mu.user_username, u.user_email, om.delivery_store,om.estimated_arrival_period,CASE om.estimated_arrival_period WHEN 0 THEN '不限時' WHEN 1 THEN '12:00以前' WHEN 2 THEN '12:00-17:00' WHEN 3 THEN '17:00-20:00' END AS delivery,od.item_id,CASE om.holiday_deliver WHEN 1 THEN '是' ELSE '否' END AS holiday_deliver ");
                //sql.AppendLine(@"SELECT om.order_id, v.vendor_name_simple,od.product_mode,vb.brand_name, od.product_name, od.detail_id, od.product_spec_name, tppp.parameterName as order_payment,od.item_mode, '' AS cost,od.single_cost,od.event_cost,CASE od.item_mode WHEN 2 THEN od.single_money/od.buy_num ELSE od.single_money END AS single_money,CASE od.item_mode WHEN 2 THEN od.buy_num*od.parent_num ELSE od.buy_num END AS buy_num,od.deduct_bonus, od.deduct_welfare, od.deduct_happygo_money,CASE od.item_mode WHEN 2 THEN od.single_money*od.parent_num-od.deduct_bonus-od.deduct_welfare-od.deduct_happygo_money ELSE od.single_money*od.buy_num-od.deduct_bonus-od.deduct_welfare-od.deduct_happygo_money END as subtotal,od.event_cost AS `event`,  om.order_name, tp.remark as slave_status ,om.delivery_name,om.delivery_gender,CONCAT_WS( '', zip.zipcode, zip.middle, zip.small ) AS delivery_zip, om.delivery_address,om.delivery_mobile, om.delivery_phone,tpp.remark AS detail_status, FROM_UNIXTIME(om.order_createdate) AS order_createdate,CASE os.slave_date_delivery WHEN 0 THEN '-' ELSE FROM_UNIXTIME(os.slave_date_delivery) END AS slave_date_delivery,CASE om.order_date_pay WHEN 0 THEN '-' ELSE FROM_UNIXTIME(om.order_date_pay) END AS order_date_pay,CASE om.money_collect_date WHEN 0 THEN '-' ELSE FROM_UNIXTIME(om.money_collect_date) END AS money_collect_date,CASE imr.invoice_date WHEN 0 THEN '-' ELSE FROM_UNIXTIME(imr.invoice_date) END AS invoice_date,CASE os.slave_date_close WHEN 0 THEN '-' ELSE FROM_UNIXTIME(os.slave_date_close) END AS slave_date_close, om.note_order,om.note_admin, od.product_freight_set,FROM_UNIXTIME(u.user_reg_date) as user_reg_date, CONCAT_WS( '-', u.user_birthday_year, u.user_birthday_month, u.user_birthday_day ) AS user_birthday,mu.user_username, u.user_email, om.delivery_store,om.estimated_arrival_period,'' AS delivery,od.item_id,om.holiday_deliver ");
                //CASE od.item_mode WHEN 2 THEN od.single_money*od.parent_num-od.deduct_bonus-od.deduct_welfare-od.deduct_happygo_money ELSE od.single_money*od.buy_num-od.deduct_bonus-od.deduct_welfare-od.deduct_happygo_money END as subtotal,
                sql.AppendLine(@"SELECT om.order_id, v.vendor_name_simple,od.product_mode,vb.brand_name, od.product_name, od.detail_id, od.product_spec_name, tppp.parameterName as order_payment,od.item_mode, '' AS cost,od.single_cost,od.event_cost,od.single_money,od.buy_num,od.parent_num,od.deduct_bonus, od.deduct_welfare, od.deduct_happygo_money,od.event_cost AS `event`,  om.order_name, tp.remark as slave_status ,om.delivery_name,om.delivery_gender,CONCAT_WS( '', zip.zipcode, zip.middle, zip.small ) AS delivery_zip, om.delivery_address,om.delivery_mobile, om.delivery_phone,tpp.remark AS detail_status, FROM_UNIXTIME(om.order_createdate) AS order_createdate,CASE os.slave_date_delivery WHEN 0 THEN '-' ELSE FROM_UNIXTIME(os.slave_date_delivery) END AS slave_date_delivery,CASE om.order_date_pay WHEN 0 THEN '-' ELSE FROM_UNIXTIME(om.order_date_pay) END AS order_date_pay,CASE om.money_collect_date WHEN 0 THEN '-' ELSE FROM_UNIXTIME(om.money_collect_date) END AS money_collect_date,CASE imr.invoice_date WHEN 0 THEN '-' ELSE FROM_UNIXTIME(imr.invoice_date) END AS invoice_date,CASE os.slave_date_close WHEN 0 THEN '-' ELSE FROM_UNIXTIME(os.slave_date_close) END AS slave_date_close, om.note_order,om.note_admin, od.product_freight_set,FROM_UNIXTIME(u.user_reg_date) as user_reg_date, CONCAT_WS( '-', u.user_birthday_year, u.user_birthday_month, u.user_birthday_day ) AS user_birthday,mu.user_username, u.user_email, om.delivery_store,om.estimated_arrival_period,'' AS delivery,od.item_id,om.holiday_deliver ");
                
                sql.AppendLine(@" FROM order_master om LEFT JOIN order_slave os ON om.order_id=os.order_id LEFT JOIN order_detail od ON os.slave_id = od.slave_id
LEFT JOIN product_item pi ON od.item_id = pi.item_id
LEFT JOIN product p ON pi.product_id = p.product_id
LEFT JOIN vendor_brand vb ON p.brand_id=vb.brand_id
LEFT JOIN vendor v ON od.item_vendor_id = v.vendor_id
LEFT JOIN manage_user mu ON v.product_manage = mu.user_id 
LEFT JOIN (select invoice_date,order_id from invoice_master_record  GROUP BY order_id) imr on om.order_id = imr.order_id
LEFT JOIN t_zip_code zip ON zip.zipcode = om.delivery_zip 
LEFT JOIN t_parametersrc tp ON os.slave_status = tp.parameterCode AND tp.parameterType = 'order_status' 
LEFT JOIN t_parametersrc tpp ON od.detail_status = tpp.parameterCode  AND tpp.parameterType = 'order_status' 
LEFT JOIN t_parametersrc tppp ON om.order_payment = tppp.parameterCode AND tppp.parameterType = 'payment'
LEFT JOIN users u ON om.user_id = u.user_id where 1=1 ");
                #region where
                if (store.Item_Vendor_Id > 0)
                {
                    sqlwhere.AppendFormat(" and od.item_vendor_id='{0}'", store.Item_Vendor_Id);
                }
                if (!string.IsNullOrEmpty(store.searchcon))
                {
                    switch (store.selecttype)
                    {
                        case "1":
                            sqlwhere.AppendFormat(" AND od.product_name LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "2":
                            sqlwhere.AppendFormat(" AND om.user_id LIKE '%{0}%' ", store.searchcon);
                            break;
                        case "3":
                            sqlwhere.AppendFormat(" AND v.vendor_name_simple LIKE  '%{0}%' ", store.searchcon);
                            break;
                        case "4":
                            sqlwhere.AppendFormat(" AND od.parent_id ='{0}' OR od.item_id ='{0}' ", store.searchcon);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(store.slave) && store.slave != "-1")
                {
                    sqlwhere.AppendFormat(" and os.slave_status={0}", store.slave);
                }
                if (store.order_payment > 0)
                {//付款方式
                    sqlwhere.AppendFormat(" and om.order_payment={0}", store.order_payment);
                }
                if (!string.IsNullOrEmpty(store.product_freight_set_in))
                {//運送方式
                    sqlwhere.AppendFormat(@" AND od.product_freight_set IN ({0}) ", store.product_freight_set_in);
                }
                if (store.product_manage != "0" && !string.IsNullOrEmpty(store.product_manage) && store.product_manage != "null")
                {//供應商管理者查詢條件
                    sqlwhere.AppendFormat(" AND v.product_manage='{0}'", store.product_manage);
                }
                if (!string.IsNullOrEmpty(store.date_type))
                {//日期條件
                    switch (store.date_type)
                    {
                        case "1":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_createdate < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "2":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.money_collect_date  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.money_collect_date < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "3":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_pay < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "4":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.slave_date_delivery  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.slave_date_delivery < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        case "5":
                            if (store.dateStart > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_close  >= '{0}' ", CommonFunction.GetPHPTime(store.dateStart.ToString()));
                            }
                            if (store.dateEnd > DateTime.MinValue)
                            {
                                sqlwhere.AppendFormat(" AND om.order_date_close < '{0}' ", CommonFunction.GetPHPTime(store.dateEnd.ToString()));
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                sqlwhere.AppendLine(@" ORDER BY order_createdate DESC , od.detail_id ASC");
                return _accessMySql.getDataTable(sql.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderVendorProducesDao.ExportCsv-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetProductItem()
        {
            string sql = string.Empty;
            sql = " SELECT pim.item_id,pim.spec_id_1,psc.spec_image FROM product_item pim left join product_spec psc ON pim.spec_id_1 = psc.spec_id ";
            return _accessMySql.getDataTable(sql);
        }
    }
}
