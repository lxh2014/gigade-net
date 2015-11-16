/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderMasterImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:00:58 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    interface IOrderMasterImplDao
    {
        OrderMaster GetOrderMasterByOrderId4Change(int order_id);
        int OrderStatusModify(string sql);
        string Save(BLL.gigade.Model.OrderMaster orderMaster);
        int Delete(int orderId);
        bool SaveOrder(string orderMaster, string orderMasterPattern, string orderPayment, ArrayList orderSlaves, ArrayList orderDetails, ArrayList otherSqls, string bonusMaster, string bonusRecord);
        OrderMaster GetPaymentById(uint order_id);
        List<OrderMasterQuery> getOrderSearch(OrderMasterQuery query, string sqladdstr, out int totalCount, string addSerch);
        List<OrderMasterQuery> Export(OrderMasterQuery query, string sqladdstr, out int totalCount);
        string UpdateMoneycanale(int return_money, uint order_id);
        string UpdatePriority(uint order_id);

        OrderShowMasterQuery GetData(uint orderId);
        string VerifyData(uint orderId);
        int SaveNoteOrder(OrderShowMasterQuery store);
        int SaveNoteAdmin(OrderShowMasterQuery store);
        int SaveStatus(OrderShowMasterQuery store);
        UsersListQuery GetUserInfo(uint user_id);
        /// <summary>
        /// 根據條件獲取出貨列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">返回數據總條數</param>
        /// <returns>出貨列表信息</returns>
        List<OrderMasterQuery> GetShipmentList(OrderMasterQuery query, out int totalCount);
        /// <summary>
        /// 查詢購買次數
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>購買次數</returns>
        int GetBuyCount(OrderMasterQuery query);
        //int OrderMasterImport(DataTable dt);
        DataTable OrderMasterExport(OrderMasterQuery query);
        DataTable ArrorOrderList(OrderMasterQuery query, out int totalCount);
        DataTable ExportArrorOrderExcel(OrderMasterQuery query);
        DataTable OrderMasterExportList(OrderMasterQuery query, out int totalCount);
        DataTable OrderMasterHuiZong(OrderMasterQuery query);
        #region 現金,外站,貨到付款對賬
        List<OrderMasterQuery> GetOBCList(OrderMasterQuery query, out int totalCount);
        List<Channel> GetChannelList(Channel query);
        List<Parametersrc> GetPaymentList(OrderMasterQuery query);
        int UpdateOrderBilling(OrderMasterQuery query);
        DataTable ReportOrderBillingExcel(OrderMasterQuery query);//泛用對賬匯出
        DataTable CheckedImport(OrderMasterQuery query);//泛用對賬匯入
        #endregion
        /// <summary>
        /// 根據定單編號獲取定單信息
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>定單信息</returns>
        OrderMaster GetOrderMasterByOrderId(int order_id);

        /// <summary>
        /// 根據定單編號更新定單狀態以開立發票
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>執行結果</returns>
        int UpdateOrderToOpenInvoice(int order_id);
        bool UpdateOrderMaster(string sql);
        OrderMasterQuery GetOrderMasterInvoice(OrderMasterQuery query);
        bool IsNotOrderId(uint orderId);
        DataTable IsExistOrderId(uint order_id);
        string UpdateOac(DataTable DtTemp, OrderAccountCollection model);
        string InsertOac(OrderAccountCollection model);
        DataTable GetOrderidAndName(int order_id);
        string UpdateOrderMaster(OrderMasterQuery query, OrderShowMasterQuery osmQuery);
        string UpOrderMaster(OrderMasterQuery query);
        string UpOrderSlave(OrderSlaveQuery query);
        string UpOrderDetail(OrderDetailQuery query);
        DataTable GetInfo(OrderMasterQuery query);
        DataTable GetNextSerial(Serial serial);
        string OMSRecord(OrderMasterStatusQuery query);
        string UpDeliveryMaster(uint order_id, uint Delivery_Store);
        string UpdateOrderMasterStatus(OrderMaster om);
        DataTable IsVendorDeliver(uint order_id);
        string ModifyOrderStatus(OrderMasterQuery query);
        DataTable IsFirstTime(OrderMasterQuery query);
        string UpFirstTime(OrderMasterQuery query);
        DataTable CheckDeliveryStatus(OrderMasterQuery query);
        string UpdateDM(OrderMasterQuery query);
        string InsertOrderMasterStatus(OrderShowMasterQuery store);
        DataTable VerifySession(uint user_id);
        DataTable OrderSerchExport(OrderMasterQuery query);
        DataTable GetPara(string type, int order_status);
        DataTable GetParaByPayment(int payment);
    }
}
