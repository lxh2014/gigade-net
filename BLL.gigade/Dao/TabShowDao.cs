using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;
namespace BLL.gigade.Dao
{
    public class TabShowDao : ITabShowImplDao
    {
        private IDBAccess _access;
        public TabShowDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 狀態列表+List<Model.Query.OrderMasterStatusQuery> GetStatus(Model.Query.OrderMasterStatusQuery store, out int totalCount)

        public List<Model.Query.OrderMasterStatusQuery> GetStatus(Model.Query.OrderMasterStatusQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {
                sql.AppendFormat(@" SELECT serial_id,order_id,order_status , os.remark  AS states, status_description,status_ipfrom, FROM_UNIXTIME(status_createdate) AS status_createdates FROM order_master_status oms ");
                sql.AppendFormat("  LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='order_status' ) os  ON os.parameterCode=oms.order_status WHERE oms.order_id={0}", store.order_id);
                sql.AppendFormat("  ORDER BY serial_id ASC ");
                sqlcount.AppendFormat(@" select count(*) as search_total from order_master_status where order_id={0}", store.order_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderMasterStatusQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetStatus " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion
        #region 出貨單+List<OrderDeliverQuery> GetDeliver(OrderDeliverQuery store, out int totalCount)


        public List<OrderDeliverQuery> GetDeliver(OrderDeliverQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcondi = new StringBuilder();

            try
            {
                sql.AppendFormat(@" SELECT od.deliver_id,od.slave_id,od.deliver_status,tpurl.parameterName as Deliver_Store_Url,od.deliver_store,tps.parameterName as deliver_name, ");
                sql.AppendFormat(" od.deliver_code,FROM_UNIXTIME(od.deliver_time) AS delivertime,od.deliver_note,FROM_UNIXTIME(od.deliver_createdate) AS deliverup, ");
                sql.AppendFormat(" od.deliver_updatedate,od.deliver_ipfrom,os.order_id,os.vendor_id,v.vendor_name_full,v.vendor_name_simple ");
                sqlcondi.AppendFormat(" FROM order_deliver od LEFT JOIN order_slave os ON os.slave_id=od.slave_id ");
                sqlcondi.Append(" LEFT JOIN (select parameterCode,parameterName from t_parametersrc where parameterType='Deliver_Store_Url') tpurl on tpurl.parameterCode=od.deliver_store ");
                sqlcondi.Append(" LEFT JOIN (select parameterCode,parameterName from t_parametersrc where parameterType='Deliver_Store') tps on tps.parameterCode=od.deliver_store ");
                sqlcondi.AppendFormat(" LEFT JOIN vendor v ON os.vendor_id=v.vendor_id WHERE os.order_id={0} ORDER BY od.deliver_time ASC ", store.order_id);
                string sqlstr = " SELECT count(*) as search_total " + sqlcondi.ToString();//Deliver_Store_Url
              
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlstr);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sqlcondi.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderDeliverQuery>(sql.ToString()+sqlcondi.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetDeliver " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion


        #region 銀聯+ List<OrderPaymentUnionPay> GetUnionPayList(OrderPaymentUnionPay store, out int totalCount)

        public List<OrderPaymentUnionPay> GetUnionPayList(OrderPaymentUnionPay store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {

                sb.Append(@"select union_id,respcode,transtype,respmsg,merabbr,merid,orderamount,ordercurrency,resptime,cupReserved");
                sb.AppendFormat(" FROM order_payment_union_pay where order_id='{0}'", store.order_id);
                sb.Append(" ORDER BY union_id ASC");
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }

            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetUnionPayList " + ex.Message + sb.ToString(), ex);
            }
            return _access.getDataTableForObj<OrderPaymentUnionPay>(sb.ToString());
        }
        #endregion



        #region 新出貨單+List<DeliverMasterQuery> GetNewDeliver(DeliverMasterQuery store, out int totalCount)

        public List<DeliverMasterQuery> GetNewDeliver(DeliverMasterQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {


                sql.AppendLine(@"SELECT dm.deliver_id,dm.order_id,dm.ticket_id, CASE dm.type WHEN '1' THEN '統倉出貨' ELSE '供應商自行出貨' END AS types,");
                sql.AppendLine(@"dm.export_id,dm.import_id,dm.freight_set,dm.delivery_status,");
                sql.AppendLine(@"dm.delivery_name,dm.delivery_mobile,dm.delivery_phone,dm.delivery_zip,");
                sql.AppendLine(@"dm.delivery_address,dm.delivery_store,tp.parameterName AS stores ,dm.delivery_code,dm.delivery_freight_cost,");
                sql.AppendLine(@"dm.delivery_date,dm.sms_date,dm.arrival_date,dm.estimated_delivery_date,ps.parameterName AS states,");
                sql.AppendLine(@"dm.estimated_arrival_date,dm.estimated_arrival_period,para.parameterName as 'estimated_arrival_period_str',dm.creator,");
                sql.AppendLine(@"dm.verifier,dm.created,dm.modified,dm.export_flag,dm.data_chg,");
                sql.AppendLine(@"v.vendor_id,v.vendor_name_full,v.vendor_name_simple");
                sql.AppendLine(@"FROM deliver_master dm ");
                sql.AppendLine(@"LEFT JOIN order_master om ON om.order_id=dm.order_id");
                sql.AppendLine(@"LEFT JOIN vendor v ON v.vendor_id=dm.export_id");
                sql.AppendLine(@"LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType='Deliver_Store') tp ON tp.parameterCode=dm.delivery_store");
                sql.AppendLine(@"LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType='delivery_status') ps ON ps.parameterCode=dm.delivery_status");
                sql.AppendLine(@" LEFT JOIN (select parameterCode,parameterName from t_parametersrc where parameterType='Estimated_Arrival_Period') para on para.parameterCode=dm.estimated_arrival_period ");
                sql.AppendFormat(@"WHERE om.order_id={0} ORDER BY dm.delivery_date ASC", store.order_id);


                sqlcount.AppendFormat(@"SELECT count(*) as search_total  FROM deliver_master dm LEFT JOIN order_master om ON om.order_id=dm.order_id LEFT JOIN vendor v ON v.vendor_id=dm.export_id WHERE om.order_id={0}", store.order_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<DeliverMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetNewDeliver " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion


        #region 支付寶+ List<OrderPaymentAlipay> GetAlipayList(OrderPaymentAlipay store, out int totalCount)

        public List<OrderPaymentAlipay> GetAlipayList(OrderPaymentAlipay store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT alipay_id,merchantnumber,timepaid ,writeoffnumber,serialnumber,amount,tel,hash ");
                sb.AppendFormat("   FROM order_payment_alipay where ordernumber='{0}' ", store.ordernumber);
                sb.Append(" ORDER BY alipay_id ASC");
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                return _access.getDataTableForObj<OrderPaymentAlipay>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowDao-->GetAlipayList " + ex.Message + sb.ToString(), ex);
            }

        }

        #endregion

        #region 購物金扣除記錄
        public List<UsersDeductBonus> GetUserDeductBonus(UsersDeductBonus store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT  id,deduct_bonus,`status` ");
                sb.AppendFormat("   FROM	users_deduct_bonus WHERE	order_id ='{0}' ", store.order_id);
                totalCount = 0;

                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }

            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetUserDeductBonus " + ex.Message + sb.ToString(), ex);
            }

            return _access.getDataTableForObj<UsersDeductBonus>(sb.ToString());

        }


        #endregion

        #region 取消單+List<OrderCancelMasterQuery> GetCancel(OrderCancelMasterQuery store, out int totalCount)
        public List<OrderCancelMasterQuery> GetCancel(OrderCancelMasterQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {



                sql.AppendLine(@"SELECT ocm.cancel_id,CASE ocm.cancel_status WHEN '1' THEN '已歸檔' WHEN '2' THEN '刪除' ELSE '待歸檔' END AS cancle_status_string,FROM_UNIXTIME(ocm.cancel_createdate) AS cancel_createdate,ocm.cancel_note,");
                sql.AppendLine(@"od.item_id,od.product_name,od.product_spec_name,od.product_freight_set,");
                sql.AppendLine(@"od.buy_num,od.single_money,(od.single_money*od.buy_num) AS subtotal, od.deduct_bonus,");
                sql.AppendLine(@"ocd.detail_id,od.slave_id,od.detail_note,pf.parameterName AS product_freight_set_string,");
                sql.AppendLine(@"ocm.order_id,ocm.bank_note,FROM_UNIXTIME(ocm.cancel_updatedate) AS cancel_updatedate,ocm.cancel_ipfrom ");
                sql.AppendLine(@"FROM order_cancel_master ocm ");
                sql.AppendLine(@"LEFT JOIN order_cancel_detail ocd ON ocd.cancel_id=ocm.cancel_id ");
                sql.AppendLine(@"LEFT JOIN order_detail od ON od.detail_id=ocd.detail_id ");
                sql.AppendLine(@"LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='product_freight') pf ON pf.parameterCode=od.product_freight_set ");
                sql.AppendFormat(@"WHERE ocm.order_id={0}", store.order_id);


                sqlcount.AppendFormat(@"SELECT count(*) as search_total FROM order_cancel_master ocm LEFT JOIN order_cancel_detail ocd ON ocd.cancel_id=ocm.cancel_id LEFT JOIN order_detail od ON od.detail_id=ocd.detail_id WHERE ocm.order_id={0}", store.order_id);

                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderCancelMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetCancel " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 發票記錄
        public List<InvoiceMasterRecordQuery> GetInvoiceMasterRecord(InvoiceMasterRecordQuery store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@" SELECT invoice_id,invoice_number,tax_type,free_tax,tax_amount,total_amount,invoice_status,invoice_attribute,(FROM_UNIXTIME(invoice_date)) as open_date ");
                sb.AppendFormat("   FROM	invoice_master_record WHERE	order_id ='{0}' ", store.order_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                return _access.getDataTableForObj<InvoiceMasterRecordQuery>(sb.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetInvoiceMasterRecord " + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion
        #region 退貨單

            #region 退貨單上
            public List<OrderReturnContentQuery> GetOrderReturnContentQueryUp(OrderReturnContentQuery store, out int totalCount)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sqlCondi = new StringBuilder();
                try
                {
                    sb.Append(@"SELECT DISTINCT orm.return_id as return_id,v.vendor_name_simple ,v.company_phone ,CONCAT(z.middle,z.small,v.company_address) AS orc_address,t.parameterName as order_status_str ,ors_remark as orc_remark,orc_service_remark,ors_createdate as orc_deliver_date ");
                    sqlCondi.Append(" FROM order_return_master orm ");//ors_order_id as orc_order_id ,
                    sqlCondi.Append(" LEFT JOIN order_return_detail ord ON orm.return_id=ord.return_id ");
                    //sqlCondi.Append(" LEFT JOIN order_detail od ON ord.detail_id=od.detail_id ");
                    sqlCondi.Append(" LEFT JOIN order_return_content orc  ON orm.return_id=orc.return_id ");
                    sqlCondi.Append(" LEFT JOIN order_return_status os ON orm.return_id = os.return_id ");

                    sqlCondi.Append(" LEFT JOIN t_parametersrc t ON os.ors_status=t.parameterCode ");
                    sqlCondi.Append(" LEFT JOIN vendor v ON orm.vendor_id = v.vendor_id ");
                    sqlCondi.Append(" LEFT JOIN t_zip_code z on v.company_zip=z.zipcode ");
                    sqlCondi.Append(" where 1=1 and t.parameterType='return_status' ");

                    if (store.orc_order_id!=0)
                    {
                        sqlCondi.AppendFormat(" and  orm.order_id = '{0}' ", store.orc_order_id);
                    }
                    sqlCondi.Append(" order by ors_createdate desc ");
                    totalCount = 0;
                    string sqlcount = " select count(ors_id) as totalCount " + sqlCondi.ToString();
                    if (store.IsPage)
                    {
                        System.Data.DataTable _dt = _access.getDataTable(sqlcount);
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                        }
                        sqlCondi.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                    }
                    return _access.getDataTableForObj<OrderReturnContentQuery>(sb.ToString() + sqlCondi.ToString());
                }
                catch (Exception ex)
                {

                    throw new Exception("TabShowDao-->GetOrderReturnContentQueryUp " + ex.Message + sb.ToString(), ex);
                }
            }

            #endregion
            #region 退貨單下
            public List<OrderReturnMasterQuery> GetReturnMasterDown(OrderReturnMasterQuery store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sb.Append(@"SELECT orm.return_id,od.item_id,od.product_name,tp.parameterName as product_freight_set_string,od.buy_num,od.single_money,od.buy_num*od.single_money as subtotal,od.deduct_bonus ");
                sqlCondi.Append("  from order_detail od  ");
                sqlCondi.Append(" LEFT JOIN order_return_detail o ON o.detail_id=od.detail_id ");
                sqlCondi.Append(" LEFT JOIN order_return_master orm ON  orm.return_id=o.return_id  ");
                sqlCondi.Append(" LEFT JOIN (select parameterCode,parameterName  from t_parametersrc where parameterType ='product_freight')tp on tp.parameterCode=od.product_freight_set ");
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(store.detailId))
                {
                    sqlCondi.AppendFormat(" and  orm.order_id= '{0}' ", store.detailId);
                }
               
                totalCount = 0;
                string sqlcount = " select count(od.item_id) as totalCount " + sqlCondi.ToString();
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                return _access.getDataTableForObj<OrderReturnMasterQuery>(sb.ToString() + sqlCondi.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetReturnMasterDown " + ex.Message + sb.ToString(), ex);
            }

        }
            #endregion

        #endregion

        #region 物流出貨狀態
        public List<LogisticsDetailQuery> GetLogistics(LogisticsDetailQuery store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sb.Append(@" SELECT	ld.rid,ld.deliver_id,ld.set_time,tp.parameterName as delivery_store_name ,tps.parameterName as logistics_type ");
                sqlCondi.Append(" FROM	logistics_detail ld INNER JOIN deliver_master dm	on dm.deliver_id = ld.deliver_id");
                sqlCondi.Append(" left join (select parameterCode,parameterName from t_parametersrc where parameterType='Deliver_Store') tp on tp.parameterCode=ld.delivery_store_id ");
                sqlCondi.Append(" left join (select parameterCode,parameterName from t_parametersrc where parameterType='logistics_type') tps on tps.parameterCode=ld.logisticsType ");
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(store.order_id))
                {
                    sqlCondi.AppendFormat(" and dm.order_id = '{0}' ",store.order_id);
                }
                sqlCondi.Append(" 	ORDER BY ld.deliver_id,ld.rid ASC ");
                totalCount = 0;
                string sqlcount = " select count(ld.rid) as totalCount " + sqlCondi.ToString();
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                return _access.getDataTableForObj<LogisticsDetailQuery>(sb.ToString() + sqlCondi.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetLogistics " + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion
        #region 退貨單+List<OrderReturnMasterQuery> GetReturn(OrderReturnMasterQuery store, out int totalCount)


        public List<OrderReturnMasterQuery> GetReturn(OrderReturnMasterQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {




                sql.AppendLine(@"SELECT orm.return_id,orm.order_id,orm.vendor_id,");
                sql.AppendLine(@"CASE orm.return_status WHEN '1' THEN '已歸檔' WHEN '2' THEN '刪除' ELSE '待歸檔' END AS return_status_string,orm.return_note,orm.bank_note,");
                sql.AppendLine(@"orm.invoice_deal,orm.package,orm.return_zip,");
                sql.AppendLine(@"orm.return_address,orm.deliver_code, FROM_UNIXTIME(orm.return_createdate) AS createdate,");
                sql.AppendLine(@"orm.return_updatedate,orm.return_ipfrom,");
                sql.AppendLine(@"v.vendor_name_simple,v.company_phone,CONCAT(v.company_zip,big,small,v.company_address) as company_address,");
                sql.AppendLine(@"ord.return_id,ord.detail_id,od.slave_id,od.item_id,od.product_freight_set,");
                sql.AppendLine(@"od.product_name,od.product_spec_name,od.detail_note,pf.parameterName AS product_freight_set_string,");
                sql.AppendLine(@"od.single_money,od.buy_num,od.single_money,(od.single_money*od.buy_num) AS subtotal,od.deduct_bonus");
                sql.AppendLine(@"FROM order_return_master orm");
                sql.AppendLine(@"LEFT JOIN vendor v ON orm.vendor_id=v.vendor_id");
                sql.AppendLine(@"LEFT JOIN order_return_detail ord ON ord.return_id=orm.return_id");
                sql.AppendLine(@"LEFT JOIN  order_detail od ON ord.detail_id=od.detail_id");
                sql.AppendLine(@"LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='product_freight') pf ON pf.parameterCode=od.product_freight_set");
                sql.AppendLine(@"LEFT JOIN t_zip_code tzc ON tzc.zipcode=v.company_zip");
                sql.AppendFormat(@"WHERE orm.order_id={0}", store.order_id);




                sqlcount.AppendLine(@"SELECT count(*) AS search_total FROM order_return_master orm");
                sqlcount.AppendLine(@"LEFT JOIN vendor v ON orm.vendor_id=v.vendor_id");
                sqlcount.AppendLine(@"LEFT JOIN order_return_detail ord ON ord.return_id=orm.return_id");
                sqlcount.AppendLine(@"LEFT JOIN  order_detail od ON ord.detail_id=od.detail_id");
                sqlcount.AppendFormat(@"WHERE orm.order_id={0};", store.order_id);


                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderReturnMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetReturn " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 中國信託+List<OrderPaymentCt> GetOrderPaymentCtList(OrderPaymentCt store, out int totalCount)

        public List<OrderPaymentCt> GetOrderPaymentCtList(OrderPaymentCt store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT id,status,errcode ,authcode,originalamt,offsetamt,utilizedpoint,errdesc,xid,awardedpoint,pointbalance ");
                sb.AppendFormat("  FROM   order_master om,order_payment_ct opc WHERE 1=1 AND opc.lidm = om.order_id ");
                sb.AppendFormat(" AND opc.lidm ='{0}'", store.lidm);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }

            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetOrderPaymentCtList " + ex.Message + sb.ToString(), ex);
            }

            return _access.getDataTableForObj<OrderPaymentCt>(sb.ToString());

        }
        #endregion

        #region 華南匯款資料+ List<OrderPaymentHncb> QueryOrderHncb(OrderPaymentHncb store,out int totalCount)
        public List<OrderPaymentHncbQuery> QueryOrderHncb(OrderPaymentHncbQuery store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT bank,entday,(FROM_UNIXTIME(txtday) )as txtdate,sn,specific_currency,paid,type,outputbank,pay_type,e_date,note,vat_number ");
                sb.AppendFormat("  FROM    order_payment_hncb where paid <> 0 ");
                sb.AppendFormat(" AND order_id ='{0}'", store.order_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }

            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->QueryOrderHncb " + ex.Message + sb.ToString(), ex);
            }

            return _access.getDataTableForObj<OrderPaymentHncbQuery>(sb.ToString());

        }
        #endregion

        #region 退款單+ List<OrderMoneyReturnQuery> GetMoney(OrderMoneyReturnQuery store,out int totalCount);


        public List<OrderMoneyReturnQuery> GetMoney(OrderMoneyReturnQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {
                sql.AppendFormat(@"SELECT omr.money_id,omr.order_id,omr.money_type,pay.parameterName AS moneytype,omr.money_total,CASE omr.money_status WHEN '1' THEN '已退款' ELSE '待退款' END AS states,omr.money_note,omr.money_source,omr.cs_note,");
                sql.AppendFormat(@"omr.bank_name,omr.bank_branch,omr.bank_account,omr.bank_note,omr.account_name,FROM_UNIXTIME(omr.money_createdate) AS createdate,omr.money_updatedate,omr.money_ipfrom ");
                sql.AppendFormat(@"FROM order_money_return omr ");
                sql.AppendFormat(@"LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='payment') pay ON pay.parameterCode=omr.money_type ");
                sql.AppendFormat(@"WHERE omr.order_id={0}  ORDER BY money_createdate ASC, money_id ASC ", store.order_id);

                sqlcount.AppendFormat(@"SELECT count(*) as search_total FROM order_money_return omr LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='payment') pay ON pay.parameterCode=omr.money_type WHERE order_id={0} ", store.order_id);


                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderMoneyReturnQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetMoney " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 問題與回覆+List<OrderQuestionQuery> GetQuestion(OrderQuestionQuery store, out int totalCount)


        public List<OrderQuestionQuery> GetQuestion(OrderQuestionQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {

                sql.AppendLine(@"SELECT oqn.question_id,oqn.order_id,oqn.question_username,oqn.question_phone,");
                sql.AppendLine(@"oqn.question_email,oqn.question_type,para.parameterName as 'question_type_str',     oqn.question_reply,");
                sql.AppendLine(@"oqn.question_reply_time,oqn.question_status,oqn.question_content,");
                sql.AppendLine(@"oqn.question_ipfrom,FROM_UNIXTIME(oqn.question_createdate) AS question_createdates,oqn.question_file,");
                sql.AppendLine(@"ore.response_id,ore.response_content,FROM_UNIXTIME(ore.response_createdate) AS response_createdates");
                sql.AppendLine(@"FROM order_question oqn LEFT JOIN order_response ore ON oqn.question_id=ore.question_id");
                sql.AppendLine(@"LEFT JOIN (SELECT parameterCode,parameterName  FROM t_parametersrc WHERE parameterType='order_question') para on oqn.question_type=para.parameterCode ");
                sql.AppendFormat(@"WHERE oqn.order_id={0}", store.order_id);


                sqlcount.AppendFormat(@"SELECT count(oqn.question_id) AS search_total FROM order_question oqn LEFT JOIN order_response ore ON oqn.question_id=ore.question_id WHERE oqn.order_id={0}", store.order_id);



                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderQuestionQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetMoney " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 取消單問題+List<OrderCancelMsgQuery> GetCancelMsg(OrderCancelMsgQuery store, out int totalCount)


        public List<OrderCancelMsgQuery> GetCancelMsg(OrderCancelMsgQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {


                sql.AppendLine(@"SELECT ocm.cancel_id,ocm.order_id,tpc.parameterName as scancel_type,ocm.cancel_status,ocr.response_content,FROM_UNIXTIME(ocr.response_createdate) AS response_createdate,");
                sql.AppendLine(@"ocm.cancel_content,FROM_UNIXTIME(ocm.cancel_createdate) AS cancel_createdate,ocm.cancel_ipfrom,ocr.response_id");
                sql.AppendLine(@"FROM order_cancel_msg ocm LEFT JOIN order_cancel_response ocr ON ocm.cancel_id=ocr.cancel_id ");
                sql.AppendLine("  LEFT JOIN t_parametersrc tpc ON tpc.parameterCode=ocm.cancel_type and tpc.parameterType ='Order_Cancel_Reason' ");
                sql.AppendFormat(@"WHERE ocm.order_id={0} ", store.order_id);



                sqlcount.AppendFormat(@"SELECT count(*) AS search_total FROM order_cancel_msg ocm LEFT JOIN order_cancel_response ocr ON ocm.cancel_id=ocr.cancel_id  WHERE ocm.order_id={0} ;", store.order_id);




                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderCancelMsgQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetCancelMsg " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 聯合信用卡+List<OrderPaymentNcccQuery> GetNCCC(OrderPaymentNcccQuery store, out int totalCount)


        public List<OrderPaymentNcccQuery> GetNCCC(OrderPaymentNcccQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            try
            {
                sql.AppendLine(@"SELECT opnc.nccc_id,opnc.order_id,opnc.merchantid,opnc.terminalid,opnc.orderid,");
                sql.AppendLine(@"opnc.pan,opnc.bankname,opnc.transcode,opnc.transmode,opnc.transdate,opnc.transtime,");
                sql.AppendLine(@"opnc.transamt,opnc.approvecode,opnc.responsecode,opnc.responsemsg,opnc.installtype,");
                sql.AppendLine(@"opnc.`install`,opnc.firstamt,opnc.eachamt,opnc.fee,opnc.redeemtype,opnc.redeemused,");
                sql.AppendLine(@"opnc.redeembalance,opnc.creditamt,opnc.riskmark,opnc.foreign1,");
                sql.AppendLine(@"opnc.secure_status,FROM_UNIXTIME(opnc.nccc_createdate) AS nccc_createdates ,opnc.nccc_ipfrom,opnc.post_data");
                sql.AppendFormat(@"FROM order_payment_nccc opnc WHERE opnc.order_id={0}", store.order_id);
                sqlcount.AppendFormat(@"SELECT count(*) AS search_total FROM order_payment_nccc WHERE order_id={0} ", store.order_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<OrderPaymentNcccQuery>(sql.ToString());
            }
            catch (Exception ex)
            {


                throw new Exception("TabShowDao-->GetNCCC " + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region Hitrust-網際威信+ List<OrderPaymentHitrustQuery> GetOderHitrust(OrderPaymentHitrustQuery store, out int totalCount)
        public List<OrderPaymentHitrustQuery> GetOderHitrust(OrderPaymentHitrustQuery store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"select order_id,retcode,orderstatus,id,retcodename,authRRN,rettype,capDate,depositamount,approveamount,redem_discount_amount,redem_discount_point,FROM_UNIXTIME(createtime) as createdate,pan,bankname ");
                sb.AppendFormat("   from order_payment_hitrust ");
                sb.AppendFormat(" where order_id ='{0}' ", store.order_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                return _access.getDataTableForObj<OrderPaymentHitrustQuery>(sb.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowDao-->GetOderHitrust " + ex.Message + sb.ToString(), ex);
            }
        }
        public DataTable GetOderHitrustDT(int order_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"select id,order_id,pan,bankname ");
                sb.AppendFormat(" from order_payment_hitrust ");
                sb.AppendFormat(" where order_id ='{0}' ", order_id);
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowDao-->GetOderHitrustDT " + ex.Message + sb.ToString(), ex);
            }
        }

        #endregion
    }

}
