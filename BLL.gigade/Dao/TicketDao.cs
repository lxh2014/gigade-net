using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Data;
using MySql.Data.MySqlClient;
using BLL.gigade.Model;
namespace BLL.gigade.Dao
{
    public class TicketDao : ITicketImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        private string connStr;
        public TicketDao(string connectionString)
        {
            this.connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<Model.Query.TicketQuery> GetTicketList(Model.Query.TicketQuery tqQuery, out int totalCount, string conditon)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder strsql = new StringBuilder();
            strsql.Append(@" SELECT COUNT(*)
                         FROM ticket AS Ticket 
                         LEFT JOIN vendor AS Export ON Export.vendor_id = Ticket.export_id ");
                         //LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='Deliver_Store') as tp on tp.parameterCode=Ticket.delivery_store");
            strsql.Append("WHERE 1=1 ");
            sb.Append(@" SELECT Ticket.ticket_id, Ticket.delivery_store,Ticket.warehouse_status, Ticket.ticket_status, Ticket.created,Ticket.modified, Ticket.seized_status, Ticket.ship_status,
                         Ticket.Freight_status, Ticket.export_id, Ticket.freight_set, Export.vendor_name_simple,Ticket.delivery_store AS deliver_ys_type_id
                         FROM ticket AS Ticket 
                         LEFT JOIN vendor AS Export ON Export.vendor_id = Ticket.export_id ");
                         //LEFT JOIN (SELECT * FROM t_parametersrc WHERE parameterType='Deliver_Store') as tp on tp.parameterCode=Ticket.delivery_store 
              sb.Append(" WHERE 1=1 ");
            sb.AppendFormat(conditon.ToString());
            sb.AppendFormat("  ORDER BY Ticket.ticket_id DESC ");
            totalCount = 0;
            try
            {
                if (tqQuery.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(strsql.ToString()+conditon.ToString());
                    if (_dt != null && Convert.ToInt32(_dt.Rows[0][0]) > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                    }
                    sb.AppendFormat(" limit {0},{1}", tqQuery.Start, tqQuery.Limit);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Deliver_Store");
                List<TicketQuery> list = _access.getDataTableForObj<TicketQuery>(sb.ToString());
                foreach (TicketQuery q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Deliver_Store" && m.ParameterCode == q.deliver_ys_type_id.ToString());
                    if (alist != null)
                    {
                        q.deliver_ys_type = alist.parameterName;
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDao.GetTicketList-->" + ex.Message + sb.ToString(), ex);
            }
        }

        #region 匯出揀貨單PDF+TicketQuery GetPickingDetail(TicketQuery query)
        /// <summary>
        /// 匯出揀貨單PDF
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TicketQuery> GetPickingDetail(TicketQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            StringBuilder strWhere = new StringBuilder();
            try
            {
                strSelect.Append("SELECT `ticket_id`,`OrderMaster`.`order_id`,`OrderMaster`.`order_date_pay`,`OrderMaster`.`money_collect_date`,`Vendor`.`vendor_name_simple`,`Product`.`product_name`,`ProductSpec1`.`spec_name`,`ProductSpec2`.`spec_name`,`OrderDetail`.`buy_num`,`OrderDetail`.`combined_mode`,`OrderDetail`.`item_mode`,`OrderDetail`.`parent_id`,`Deliver`.`deliver_id`,`Deliver`.`freight_set`,`Deliver`.`delivery_name`,`OrderMaster`.`order_name`,`OrderDetail`.`product_freight_set` ");
                strSelect.Append(" FROM `deliver_master` AS `Deliver` ");
                strSelect.Append(" LEFT JOIN `deliver_detail` AS `DeliverDetail` ON (`Deliver`.`deliver_id` = `DeliverDetail`.`deliver_id`) ");
                strSelect.Append(" LEFT JOIN `order_detail` AS `OrderDetail` ON (`OrderDetail`.`detail_id` = `DeliverDetail`.`detail_id`) ");
                strSelect.Append(" LEFT JOIN `order_slave` AS `OrderSlave` ON (`OrderSlave`.`slave_id` = `OrderDetail`.`slave_id`) ");
                strSelect.Append(" LEFT JOIN `product_item` AS `ProductItem` ON (`ProductItem`.`item_id` = `OrderDetail`.`item_id`) ");
                strSelect.Append(" LEFT JOIN `vendor` AS `Vendor` ON (`Vendor`.`vendor_id` = `OrderDetail`.`item_vendor_id`) ");
                strSelect.Append(" LEFT JOIN `order_master` AS `OrderMaster` ON (`OrderMaster`.`order_id` = `OrderSlave`.`order_id`) ");
                strSelect.Append(" LEFT JOIN `product` AS `Product` ON (`Product`.`product_id` = `ProductItem`.`product_id`) ");
                strSelect.Append(" LEFT JOIN `product_spec` AS `ProductSpec1` ON (`ProductSpec1`.`spec_id` = `ProductItem`.`spec_id_1`) ");
                strSelect.Append(" LEFT JOIN `product_spec` AS `ProductSpec2` ON (`ProductSpec2`.`spec_id` = `ProductItem`.`spec_id_2`) ");
                strWhere.Append(" WHERE 1=1 ");
                strWhere.AppendFormat(" and  `ticket_id` in ({0})  ", query.ticketIds);
                strWhere.Append(" ORDER BY `OrderMaster`.`order_id` ASC, `Vendor`.`vendor_id` ASC");
                strSql.Append(strSelect.ToString());
                strSql.Append(strWhere.ToString());
                return _access.getDataTableForObj<TicketQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDao-->GetPickingDetail-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion
        #region 匯出出貨明細PDF+TicketQuery GetTicketDetail(TicketQuery query)
        /// <summary>
        /// 匯出出貨明細PDF
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TicketQuery> GetTicketDetail(TicketQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strFrom = new StringBuilder();
            StringBuilder strCondi = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT dm.deliver_id, dm.freight_set,dm.delivery_name, dm.delivery_mobile,dm.delivery_zip,dm.delivery_address,dm.delivery_store,dm.estimated_arrival_period,OrderMaster.order_id,OrderMaster.order_name,OrderMaster.order_createdate,OrderMaster.order_date_pay,OrderMaster.money_collect_date,OrderMaster.note_order,OrderMaster.deduct_happygo_convert,OrderMaster.order_freight_normal,OrderMaster.order_freight_low,OrderMaster.channel,OrderMaster.retrieve_mode,OrderMaster.priority,OrderMaster.holiday_deliver, CONCAT(tzc.zipcode,tzc.middle,tzc.small,dm.delivery_address) as zip_name ");
                strFrom.AppendFormat(" FROM deliver_master dm LEFT JOIN order_master OrderMaster ON OrderMaster.order_id=dm.order_id ");
                strFrom.Append(" LEFT JOIN t_zip_code  tzc on dm.delivery_zip=tzc.zipcode");
                strCondi.AppendFormat(" WHERE deliver_id IN (SELECT deliver_id FROM deliver_master WHERE ticket_id in ({0}) ) and type=1 AND delivery_status!=6  ORDER BY OrderMaster.priority DESC,OrderMaster.order_id ASC ", query.ticketIds);
                return _access.getDataTableForObj<TicketQuery>(strSql.ToString() + strFrom.ToString() + strCondi.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TicketDao-->GetTicketDetail-->" + ex.Message + strSql.ToString() + strFrom.ToString() + strCondi.ToString(), ex);
            }
        }
        /// <summary>
        ///出貨明細
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable GetOrderDelivers(TicketQuery query)
        {
            int type = 0;//订单出货明细 1 出货明细
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@" SELECT dm.deliver_id,dm.freight_set,dm.delivery_name,dm.delivery_mobile,dm.delivery_zip,dm.delivery_address,");
            sql.AppendLine(@"            dm.type,dm.delivery_store,dm.estimated_arrival_period,om.order_id,om.order_name,");
            sql.AppendLine(@"            om.order_createdate,om.order_date_pay,om.money_collect_date,");
            sql.AppendLine(@"            om.note_order, om.deduct_happygo_convert,om.order_freight_normal,");
            sql.AppendLine(@"            om.order_freight_low,om.channel,channel.channel_name_simple,om.retrieve_mode,");
            sql.AppendLine(@"            om.priority, om.holiday_deliver,");
            sql.AppendLine(@"            od.item_id,od.product_name,od.product_spec_name,");
            sql.AppendLine(@"            od.product_freight_set,od.single_price,od.single_money,od.buy_num,od.product_mode,");
            sql.AppendLine(@"            od.combined_mode,od.item_mode,od.parent_id,od.parent_name,");
            sql.AppendLine(@"            od.detail_id,od.pack_id,od.parent_num,vb.brand_name ");
            sql.AppendLine(@"            from deliver_master dm LEFT JOIN order_master om on dm.order_id=om.order_id");
            sql.AppendLine(@"            LEFT JOIN channel channel on channel.channel_id=om.channel");
            if (type == 0)
            {
                sql.AppendLine(@"            LEFT JOIN order_slave os on os.order_id=dm.order_id");
                sql.AppendLine(@"            LEFT JOIN order_detail od on od.slave_id=os.slave_id");
            }
            else if (type == 1)
            {

                sql.AppendLine(@"  LEFT JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id");
                sql.AppendLine(@"  LEFT JOIN order_detail od on dd.detail_id=od.detail_id");

            }
            sql.AppendLine(@"            LEFT JOIN product_item pi on pi.item_id=od.item_id");
            sql.AppendLine(@"            LEFT JOIN product p on p.product_id=pi.product_id");
            sql.AppendLine(@"            LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id");
            sql.AppendFormat(@"          where dm.deliver_id='{0}' and dm.delivery_status <>6 ", query.deliver_id);
            if (type == 0)
            {
                sql.AppendLine(@"        and dm.type=1   and od.detail_status in(2,4,6,7) ");
            }
            else if (type == 1)
            {
                sql.AppendLine(@" and od.detail_status in(2, 3, 4, 6, 7) ");
            }
            sql.AppendFormat(@"    GROUP BY od.product_freight_set,od.item_id;");
            DataTable orderdeliver = _access.getDataTable(sql.ToString());
            DataColumn dc = new DataColumn("receivable", typeof(int));
            orderdeliver.Columns.Add(dc);
            #region 應收金額
            foreach (DataRow dr in orderdeliver.Rows)
            {
                //不是貨到付款的物流方式
                if (dr["delivery_store"].ToString() != "10" && dr["delivery_store"].ToString() != "17")
                {
                    dr["receivable"] = 0;
                }
                else
                {
                    DataTable dt = GetReceivable(dr["deliver_id"].ToString());
                    int product_subtotal = 0;
                    int deduct_bonus = 0;
                    int deduct_welfare = 0;
                    int deduct_happygo = 0;
                    double deduct_happygo_convert = (!string.IsNullOrEmpty(dr["deduct_happygo_convert"].ToString())) ? double.Parse(dr["deduct_happygo_convert"].ToString()) : 0;
                    int fare = 0;
                    if (dt.Rows.Count > 0)
                    {
                        product_subtotal = int.Parse(dt.Rows[0]["product_subtotal"].ToString());
                        deduct_bonus = int.Parse(dt.Rows[0]["deduct_bonus"].ToString());
                        deduct_welfare = int.Parse(dt.Rows[0]["deduct_welfare"].ToString());
                        deduct_happygo = int.Parse(dt.Rows[0]["deduct_happygo"].ToString());
                    }
                    if (dr["freight_set"].ToString() == "1")
                    {
                        fare = (!string.IsNullOrEmpty(dr["order_freight_normal"].ToString())) ? int.Parse(dr["order_freight_normal"].ToString()) : 0;
                    }
                    else
                    {
                        fare = (!string.IsNullOrEmpty(dr["order_freight_low"].ToString())) ? int.Parse(dr["order_freight_low"].ToString()) : 0;
                    }
                    dr["receivable"] = product_subtotal + fare - deduct_bonus - deduct_welfare - Math.Round(Decimal.Parse((deduct_happygo * deduct_happygo_convert).ToString()), 0);
                }

            }
            #endregion
            return orderdeliver;

        }
        /// <summary>
        /// 應收金額
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <returns></returns>
        public DataTable GetReceivable(string deliver_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT sum(od.single_money*buy_num)  as product_subtotal, sum(od.deduct_bonus) as deduct_bonus,");
            sql.AppendLine(@"sum(od.deduct_welfare) as deduct_welfare,od.deduct_happygo as deduct_happygo   from deliver_master dm LEFT JOIN deliver_detail dd on dm.deliver_id=dd.deliver_id");
            sql.AppendLine(@"LEFT JOIN order_detail od on dd.detail_id=od.detail_id");
            sql.AppendFormat(@" where od.detail_status not in(89,90,91,92) and od.item_mode <>2 and dm.deliver_id='{0}'  limit 1;", deliver_id);

            return _access.getDataTable(sql.ToString());
        }
        #endregion


        public int UpdateTicketStatus(TicketQuery query)
        {
            int result = 0;
            StringBuilder strsql = new StringBuilder();
            strsql.Append("set sql_safe_updates = 0;");
            if (query.type_id == 1)
            {
                strsql.AppendFormat("update ticket set seized_status=1 where ticket_id in ({0});",query.ticket_idto_str);
            }
            else if (query.type_id == 2)
            {
                strsql.AppendFormat("update ticket set ship_status=1 where ticket_id  in ({0});", query.ticket_idto_str);
            }
            else if (query.type_id == 3)
            {
                strsql.AppendFormat("update ticket set Freight_status=1 where ticket_id in ({0});", query.ticket_idto_str);
            }
            strsql.Append("set sql_safe_updates = 1;");
            try
            {
              result =  _access.execCommand(strsql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDao.UpdateTicketStatus-->" + ex.Message + strsql.ToString(), ex);
            }
           
            return result;
        }
    }
}
