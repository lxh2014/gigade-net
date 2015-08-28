using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Dao
{
    public class VendorAccountMonthDao : IVendorAccountMonthImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        private string connStr;
        public VendorAccountMonthDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }
        #region 供應商對帳總表+DataTable VendorAccountCountExport(VendorAccountDetailQuery query)
        #region 查詢訂單筆數
        /// <summary>
        /// 查詢訂單筆數
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable VendorAccountCountExport(VendorAccountMonthQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.Append(@"SELECT vam.vendor_id, v.erp_id,v.vendor_code,v.vendor_name_full,v.vendor_name_simple,v.vendor_invoice,v.product_mode,vam.account_month,");
                sql.Append(" vam.m_product_money,vam.m_product_cost,vam.m_money_creditcard_1,vam.m_money_creditcard_3,");
                sql.Append(" vam.m_freight_delivery_low,vam.m_freight_delivery_normal,vam.m_dispatch_freight_delivery_normal,vam.m_dispatch_freight_delivery_low,");
                sql.Append(" vam.m_freight_return_low,vam.m_freight_return_normal,vam.m_account_amount,vam.m_all_deduction,");
                sql.Append(" vam.m_gift,vam.dispatch,vam.m_bag_check_money");
                sqlfrom.AppendFormat(" FROM vendor_account_month vam LEFT JOIN vendor v ON v.vendor_id=vam.vendor_id where 1=1 ");
                if (query.type != 0)
                {
                    if (query.type == 1)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_email like '%{0}%'", query.keyworks);
                    }
                    if (query.type == 2)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_name_simple like '%{0}%'", query.keyworks);
                    }
                    if (query.type == 3)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_invoice like '%{0}%' ", query.keyworks);
                    }
                    if (query.type == 4)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_id = {0} ", query.keyworks);
                    }
                    if (query.type == 5)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_code like '%{0}%'", query.keyworks);
                    }
                }
             
                sqlfrom.AppendFormat(" and vam.account_year='{0}' ", query.account_year);
                sqlfrom.AppendFormat(" and vam.account_month='{0}' ", query.account_month);
                sql.AppendFormat(sqlfrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->VendorAccountCountExport-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 供應商總表部分信息
        /// <summary>
        /// 供應商總表部分信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable VendorAccountInfoExport(VendorAccountMonthQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            string strTemp = string.Empty;
            string configValue = string.Empty;
            try
            {
                string date = string.Empty;
                date += string.Format(query.account_year.ToString());
                date += string.Format("-");
                date += string.Format(query.account_month.ToString());
                date += string.Format("-1 00:00:00");
                strTemp = string.Format(" select config_value from config where config_name='is_not_billing_checked'");
                DataTable dt = _access.getDataTable(strTemp);

                if (dt.Rows.Count > 0)
                {
                    configValue = dt.Rows[0][0].ToString();
                    configValue = configValue.Remove(configValue.LastIndexOf(','));
                }
                sql.Append(@"SELECT vad.slave_id, vad.vendor_id,vad.order_id,vad.product_money,om.order_payment,vad.deduction");
                sql.Append(" from vendor_account_detail vad  left join  order_slave os on vad.slave_id = os.slave_id");
                sql.Append(" left join  order_master om on om.order_id = os.order_id");
                sqlfrom.AppendFormat("  where 1=1 and account_date >='{0}' and account_date <='{1}' ", CommonFunction.GetPHPTime(date), CommonFunction.GetPHPTime(Convert.ToDateTime(date).AddMonths(1).ToString()));
                sqlfrom.AppendFormat(" and om.order_payment in({0}) order by vad.vendor_id asc", configValue);
                sql.AppendFormat(sqlfrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->VendorAccountDetailExport-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #endregion

        #region 供應商業績報表列表+List<VendorAccountMonthQuery> GetVendorAccountMonthList(VendorAccountMonthQuery store, out int totalCount)
        /// <summary>
        /// 供應商業績報表列表
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<VendorAccountMonthQuery> GetVendorAccountMonthList(VendorAccountMonthQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.Append(@"SELECT vam.vendor_id, v.vendor_code,v.vendor_name_simple,vam.account_year,vam.account_month,");
                sql.Append(" vam.m_product_money,vam.m_product_cost,vam.m_money_creditcard_1,vam.m_money_creditcard_3,");
                sql.Append(" vam.m_freight_delivery_low,vam.m_freight_delivery_normal,vam.m_dispatch_freight_delivery_normal,vam.m_dispatch_freight_delivery_low,");
                sql.Append(" vam.m_freight_return_low,vam.m_freight_return_normal,vam.m_account_amount,vam.m_all_deduction,");
                sql.Append(" vam.m_gift,vam.dispatch,vam.m_bag_check_money");
                sqlfrom.AppendFormat(" FROM vendor_account_month vam LEFT JOIN vendor v ON v.vendor_id=vam.vendor_id where 1=1 ");
                //sqlfrom.AppendFormat(" group by vendor_account_month.vendor_id");
                string str = string.Format(" SELECT count(*) AS search_total  ");

                if (store.type != 0)
                {
                    if (store.type == 1)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_email like '%{0}%'", store.keyworks);
                    }
                    if (store.type == 2)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_name_simple like '%{0}%'", store.keyworks);
                    }
                    if (store.type == 3)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_invoice like '%{0}%' ", store.keyworks);
                    }
                    if (store.type == 4)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_id  ={0} ", store.keyworks);
                    }
                    if (store.type == 5)
                    {
                        sqlfrom.AppendFormat(" and v.vendor_code like '%{0}%'", store.keyworks);
                    }
                }
                sqlfrom.AppendFormat(" and vam.account_year='{0}' ", store.account_year);
                sqlfrom.AppendFormat(" and vam.account_month='{0}' ", store.account_month);


                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(str + sqlfrom.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {

                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sqlfrom.AppendFormat(" order by vam.vendor_id asc  ");
                    sqlfrom.AppendFormat(" limit {0},{1};", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<VendorAccountMonthQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->GetVendorAccountMonthList-->" + sqlfrom.ToString() + ex.Message, ex);
            }


        }
        #endregion

        #region 查詢業績明細

        #region 查詢供應商業績明細+List<VendorAccountDetailQuery> GetVendorAccountMonthDetailList(VendorAccountDetailQuery store, out int totalCount)
        /// <summary>
        /// 查詢供應商業績明細
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<VendorAccountDetailQuery> GetVendorAccountMonthDetailList(VendorAccountDetailQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.AppendFormat(" select vad.account_amount,vad.account_date,vad.bag_check_money,vad.bonus_percent,vad.creditcard_1_percent,vad.creditcard_3_percent, ");
                sql.AppendFormat(" vad.deduction,vad.freight_delivery_low,vad.freight_delivery_normal,vad.freight_low_limit,vad.freight_low_money,vad.freight_normal_limit, ");
                sql.AppendFormat("vad.freight_normal_money,vad.freight_return_low,vad.freight_return_low_money,vad.freight_return_normal,vad.freight_return_normal_money,");
                sql.AppendFormat("vad.gift,vad.money_creditcard_1,vad.money_creditcard_3, ");
                sql.AppendFormat("vad.order_id,vad.product_cost,vad.product_money,vad.sales_limit,vad.slave_id,vad.vendor_id,");
                //sql.AppendFormat("vad.order_id,vad.product_cost,vad.product_money,vad.sales_limit,vad.slave_id,vad.vendor_id,");
                sql.AppendFormat("od.detail_id,od.slave_id,od.item_id,od.product_freight_set,od.product_mode,od.product_name,");
                sql.AppendFormat("od.product_spec_name,od.single_cost,od.single_price,od.buy_num,od.parent_num,od.detail_status,od.event_cost,od.single_money,");
                sql.AppendFormat("os.slave_date_delivery,os.slave_date_close,om.order_payment,om.order_createdate,od.parent_id,od.pack_id,od.item_mode ");
                sql.AppendFormat(" from vendor_account_detail vad left join order_detail od on vad.vendor_id = od.item_vendor_id and vad.slave_id = od.slave_id ");
                sql.AppendFormat(" left join order_slave os on od.slave_id = os.slave_id left join	order_master om on vad.order_id = om.order_id ");
                sqlfrom.AppendFormat(" where 1=1 and vad.vendor_id = {0} and vad.account_date >= {1} and vad.account_date <= {2} and od.detail_status <> 89 ", store.vendor_id, store.search_start_time, store.search_end_time);
                string str = string.Format(" SELECT count(*) AS search_total from vendor_account_detail vad left join order_detail od on vad.vendor_id = od.item_vendor_id and vad.slave_id = od.slave_id left join order_slave os on od.slave_id = os.slave_id left join	order_master om on vad.order_id = om.order_id ");
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(str + sqlfrom.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {

                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sqlfrom.AppendFormat(" order by account_date asc, order_id asc,od.item_mode asc ");
                    sqlfrom.AppendFormat(" limit {0},{1};", store.Start, store.Limit);

                }
                List<VendorAccountDetailQuery> list = _access.getDataTableForObj<VendorAccountDetailQuery>(sql.ToString() + sqlfrom.ToString());
                return list;
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->GetVendorAccountMonthDetailList-->" + sqlfrom.ToString() + ex.Message, ex);
            }

        }

        #endregion
        #endregion

        #region 業績明細匯出+DataTable VendorAccountDetailExport(VendorAccountDetailQuery query)
        public List<VendorAccountCustom> VendorAccountDetailExport(VendorAccountDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                // sql.AppendFormat(" select p.tax_type, vad.slave_id,vad.vendor_id,vad.order_id,vad.creditcard_1_percent,vad.creditcard_3_percent,vad.sales_limit, ");
                sql.AppendFormat(" select vad.slave_id,vad.vendor_id,vad.order_id,vad.sales_limit, ");

                sql.AppendFormat(" vad.bonus_percent,vad.freight_low_limit,vad.freight_low_money,vad.freight_normal_limit,vad.freight_normal_money, ");
                sql.AppendFormat("vad.freight_return_normal_money,vad.product_money,vad.product_cost,vad.money_creditcard_1,vad.money_creditcard_3,");
                sql.AppendFormat("vad.freight_delivery_low,vad.freight_delivery_normal,vad.freight_return_low, ");
                //sql.AppendFormat("vad.freight_return_normal,vad.account_amount,FROM_UNIXTIME (vad.account_date,'%Y/%m/%d') account_date,vad.gift,vad.deduction,vad.bag_check_money,vad.freight_return_low_money,imr.free_tax,imr.total_amount,imr.tax_amount, ");
                sql.AppendFormat("vad.freight_return_normal,vad.account_amount,account_date,vad.gift,vad.deduction,vad.bag_check_money,vad.freight_return_low_money,imr.free_tax,imr.total_amount,imr.tax_amount, ");
                sql.AppendFormat("od.detail_id,od.slave_id,od.item_id,od.item_vendor_id,od.item_mode,od.product_freight_set,od.product_mode,od.product_name,od.product_spec_name,od.single_cost,od.deduct_account,od.parent_id,");
                //sql.AppendFormat("od.single_price,od.buy_num,od.event_cost,od.single_money,od.detail_status,FROM_UNIXTIME (os.slave_date_delivery,'%Y/%m/%d')   slave_date_delivery,os.slave_date_close,om.order_payment,");
                sql.AppendFormat("od.single_price,od.buy_num,od.event_cost,od.single_money,od.detail_status, slave_date_delivery,os.slave_date_close,om.order_payment,");
                sql.AppendFormat(" od.deduct_bonus,od.deduct_welfare,od.deduct_happygo_money, ");
                //sql.AppendFormat(" FROM_UNIXTIME( om.order_createdate,'%Y/%m/%d') order_createdate,om.note_admin,od.bag_check_money as od_bag_check_money,tp.parameterName,tp1.parameterName as product_freight,tp2.remark order_status_name ");

                sql.AppendFormat(" order_createdate,om.note_admin,od.bag_check_money as od_bag_check_money ");
                sqlfrom.AppendFormat(" from	order_detail od left join order_slave os on od.slave_id = os.slave_id ");
                sqlfrom.AppendFormat("left join vendor_account_detail vad on vad.slave_id = od.slave_id and	vad.vendor_id = od.item_vendor_id  ");
                sqlfrom.AppendFormat("left join order_master om on vad.order_id = om.order_id  ");
                //sqlfrom.AppendFormat("left join vendor v ON vad.vendor_id = v.vendor_id ");
                // sqlfrom.AppendFormat("left join product_item pi ON pi.item_id = od.item_id ");
                //sqlfrom.AppendFormat("left join product p ON p.product_id = pi.product_id ");
                sqlfrom.AppendFormat("left join invoice_master_record imr on om.order_id=imr.order_id and imr.tax_type=1  and invoice_attribute=1 ");

                // sqlfrom.AppendFormat("left join (select * from t_parametersrc where parameterType='payment') tp on om.order_payment = tp.parameterCode ");
                //sqlfrom.AppendFormat("left join (select * from t_parametersrc where parameterType='product_freight') tp1 on od.product_freight_set = tp1.parameterCode ");
                //sqlfrom.AppendFormat("left join (select * from t_parametersrc where parameterType='order_status') tp2 on od.detail_status = tp2.parameterCode ");

                sqlfrom.AppendFormat(" where 1=1 and vad.vendor_id = {0} and vad.account_date >= {1} and vad.account_date <= {2} and	od.detail_status <> 89 ", query.vendor_id, query.search_start_time, query.search_end_time);
                sqlfrom.AppendFormat(" order by account_date asc, order_id asc,vad.slave_id ASC, od.item_mode asc ");
                sql.AppendFormat(sqlfrom.ToString());

                //return _access.getDataTable(sql.ToString());




                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("payment", "product_freight", "order_status");
                List<VendorAccountCustom> list = _access.getDataTableForObj<Model.Custom.VendorAccountCustom>(sql.ToString());
                IProductItemImplDao _itemDao = new ProductItemDao(connStr);
                IVendorImplDao _vendordao = new VendorDao(connStr);
                foreach (VendorAccountCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == q.Order_Payment.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == q.Product_Freight_Set.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == q.Detail_Status.ToString());
                    if (alist != null)
                    {
                        q.paymentname = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.product_freight = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.order_status_name = clist.remark;
                    }
                    Product p = _itemDao.GetTaxByItem(Convert.ToUInt32(q.Item_Id));
                    DataTable dt = _vendordao.GetVendorDetail(" and vendor_id=" + q.Item_Vendor_Id + " ");
                    if (p != null)
                    {
                        q.tax_type = p.Tax_Type;
                    }
                    if (dt.Rows.Count != 0)
                    {
                        q.creditcard_1_percent = Convert.ToUInt32(dt.Rows[0]["creditcard_1_percent"].ToString());
                        // q.creditcard_3_percent = Convert.ToUInt32(dt.Rows[0]["creditcard_3_percent"].ToString());
                    }
                    q.accountdate = Common.CommonFunction.GetNetTime(q.account_date);
                    q.slavedate_delivery = Common.CommonFunction.GetNetTime(q.slave_date_delivery);
                    q.ordercreatedate = Common.CommonFunction.GetNetTime(q.Order_Createdate);
                }
                return list;

            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->VendorAccountDetailExport-->" + sql.ToString() + ex.Message, ex);
            }
        }

        #endregion

        #region 查詢供應商信息+VendorQuery GetVendorInfoByCon(VendorQuery query)
        /// <summary>
        /// 查詢供應商信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public VendorQuery GetVendorInfoByCon(VendorQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from vendor where vendor_id={0} ", query.vendor_id);
                return _access.getSinggleObj<VendorQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->GetVendorInfoByCon-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢供應商總賬+VendorAccountMonthQuery GetVendorAccountMonthZongZhang(VendorAccountMonthQuery query)
        /// <summary>
        /// 查詢供應商總賬
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetVendorAccountMonthZongZhang(VendorAccountMonthQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from vendor_account_month where vendor_id={0} and account_year={1} and account_month={2}", query.vendor_id, query.account_year, query.account_month);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorAccountMonthDao-->GetVendorAccountMonthZongZhang-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢供應商+VendorAccountMonthQuery GetVendorAccountMonthInfo(VendorAccountMonthQuery query)
        /// <summary>
        /// 查詢供應商
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetVendorAccountMonthInfo(VendorAccountMonthQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select v.vendor_code,v.vendor_name_full,v.vendor_id,vam.m_account_amount,vam.m_bag_check_money,v.dispatch,vam.m_product_money,v.freight_normal_money,v.freight_low_money,");
                sql.AppendFormat(" v.freight_normal_limit,v.freight_low_limit from vendor_account_month vam left join vendor v on v.vendor_id=vam.vendor_id");
                sql.AppendFormat(" where account_year={0} and account_month={1}", query.account_year, query.account_month);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorAccountMonthDao-->GetVendorAccountMonthInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢調度倉運費計算+DataTable GetFreightMoney(VendorAccountDetailQuery query)
        /// <summary>
        /// 查詢調度倉運費計算
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetFreightMoney(VendorAccountDetailQuery query, out int tempFreightDelivery_Normal, out int tempFreightDelivery_Low)
        {
            tempFreightDelivery_Normal = 0;
            tempFreightDelivery_Low = 0;
            StringBuilder sql = new StringBuilder();
            try
            {
                VendorQuery VQuery = new VendorQuery();
                VQuery.vendor_id = query.vendor_id;
                VendorQuery vendorTemp = GetVendorInfoByCon(VQuery);
                sql.AppendFormat("select * , sum(normal_subtotal) as gnormal_subtotal , sum(hypothermia_subtotal) as ghypothermia_subtotal from order_slave_master where 1=1 and deliver_time>={0} and deliver_time<={1} and creator = {2} group by deliver_time ", query.search_start_time, query.search_end_time, query.vendor_id);
                DataTable temp = _access.getDataTable(sql.ToString());
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    if (Convert.ToInt32(temp.Rows[i]["gnormal_subtotal"]) != 0)
                    {
                        if (vendorTemp.freight_normal_limit > Convert.ToInt32(temp.Rows[i]["gnormal_subtotal"]))
                        {
                            tempFreightDelivery_Normal += Convert.ToInt32(vendorTemp.freight_normal_money);
                        }
                    }
                    if (Convert.ToInt32(temp.Rows[i]["ghypothermia_subtotal"]) != 0)
                    {
                        if (vendorTemp.freight_low_limit > Convert.ToInt32(temp.Rows[i]["ghypothermia_subtotal"]))
                        {
                            tempFreightDelivery_Low += Convert.ToInt32(vendorTemp.freight_low_money);
                        }
                    }
                    //temp.Rows[i]["Temp_Freight_Delivery_Normal"] = tempFreightDelivery_Normal;
                    //temp.Rows[i]["Temp_Freight_Delivery_Low"] = tempFreightDelivery_Low;
                }

                return temp;
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->GetFreightMoney-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 批次出貨單明細+DataTable BatchOrderDetail(VendorAccountDetailQuery query)
        /// <summary>
        /// 批次出貨單明細
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable BatchOrderDetail(VendorAccountDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.AppendFormat("  select osd.slave_id,osm.slave_master_id,osm.code_num,osm.order_freight_normal,osm.order_freight_low,osm.normal_subtotal,osm.hypothermia_subtotal,");
                sql.AppendFormat("osm.paper,osm.deliver_store,osm.deliver_code,FROM_UNIXTIME (osm.deliver_time,'%Y/%m/%d') as  deliver_time,osm.deliver_note,osm.createdate,osm.creator,osm.on_check,om.order_id ");
                sqlfrom.AppendFormat(" from order_slave_detail osd left join order_slave os on osd.slave_id = os.slave_id left join ");
                sqlfrom.AppendFormat(" order_slave_master osm on osd.slave_master_id = osm.slave_master_id left join order_master om on om.order_id = os.order_id  ");
                sqlfrom.AppendFormat(" where 1=1 and osm.deliver_time +(86400 * 10)>={0} and osm.deliver_time+(86400 * 10)<={1} and creator={2} order by osm.deliver_time", query.search_start_time, query.search_end_time, query.vendor_id);
                sql.AppendFormat(sqlfrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->VendorAccountDetailExport-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 查詢應稅、免稅金額計算
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetTaxMoney(VendorAccountDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.AppendFormat(" SELECT free_tax,tax_amount,tax_type from invoice_master_record WHERE invoice_attribute=1 and order_id in ({0})", query.orderIds);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthDao-->VendorAccountDetailExport-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
