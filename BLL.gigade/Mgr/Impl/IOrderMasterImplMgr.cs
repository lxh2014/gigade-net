/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderMasterImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:06:06 
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

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderMasterImplMgr
    {
        bool ModifyOrderMsaterForDeliver(OrderModifyModel m);
        string Save(BLL.gigade.Model.OrderMaster orderMaster, OrderMasterPattern op = null);
        int Delete(int orderId);
        bool SaveOrder(string orderMaster, string orderMasterPattern, string orderPayment, ArrayList orderSlaves, ArrayList orderDetails, ArrayList otherSqls, string bonusMaster, string bonusRecord);
        List<OrderMasterQuery> Export(OrderMasterQuery query, string sqladdstr, out int totalCount);
        OrderMaster GetPaymentById(uint order_id);

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
        /// <returns>出貨列表</returns>
        List<OrderMasterQuery> GetShipmentList(OrderMasterQuery query, out int totalCount);
        /// <summary>
        /// 查詢購買次數
        /// </summary>
        /// <param name="orderMasterQuery">查詢條件</param>
        /// <returns>購買次數</returns>
        int GetBuyCount(OrderMasterQuery orderMasterQuery);
        int OrderMasterImport(List<OrderAccountCollection> oacli);
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
        bool IsNotOrderId(uint orderId);
        DataTable IsExistOrderId(uint order_id);
        string ChangePayMent(OrderMasterQuery query);
        bool IsVendorDeliver(uint order_id);
        string ModifyDeliveryData(OrderMasterQuery om);
        int VerifySession(uint user_id);
        bool IsSendProduct(OrderMasterQuery om);
        DataTable OrderSerchExport(OrderMasterQuery query);
        string GetPara(string type, int order_status);
        string GetParaByPayment(int payment);
        DataTable GetOrderFreight(uint order_id);
        DataTable CagegoryDetialExportInfo(OrderDetailQuery query);
        DataTable OrderDetialExportInfo(OrderDetailQuery query);
        DataTable GetInvoiceData(uint order_id);
        DataTable GetInvoice(uint order_id, uint pid);
    }
}
