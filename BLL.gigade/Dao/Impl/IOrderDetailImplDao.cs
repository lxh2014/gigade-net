/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderDetailImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 17:00:49 
 * 
 */

using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    interface IOrderDetailImplDao
    {
        string Save(BLL.gigade.Model.OrderDetail orderDetail);
        int Delete(int detailId);
        List<OrderDetail> QueryReturnDetail(uint return_id);     
        List<Model.Query.OrderDetailQuery> GetOrderDetailList(Model.Query.OrderDetailQuery query, out int totalCount);
        List<BLL.gigade.Model.Query.OrderDetailQuery> SubPoenaDetail(BLL.gigade.Model.Query.OrderDetailQuery query);
        bool no_delivery(BLL.gigade.Model.Query.OrderDetailQuery query);
        int split_detail(BLL.gigade.Model.Query.OrderDetailQuery query);
        List<OrderDetailQuery> VendorPickPrint(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null);
        List<OrderDetailQuery> GetOrderDetailToSum(uint vendor_id, long time);
        List<OrderDetailQuery> AllOrderDeliver(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null);

        DataTable GetLeaveproductList(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null);
        List<OrderDetailQuery> OrderVendorProducesList(OrderDetailQuery query, out int totalCount);
        DataTable ProduceGroupCsv(OrderDetailQuery query);
        DataTable ProduceGroupExcel(OrderDetailQuery query);
        List<OrderDetailQuery> DeliveryInformation(OrderDetailQuery query, string str);
        List<OrderDetailCustom> GetArriveDay(uint detail_id);//獲得商品送達的天數信息
        List<OrderDetailQuery> OrderDetail(uint return_id);
        string UpdateOrderDetail(OrderDetailQuery query);
        string UpdateOrderDetailSome(OrderDetailQuery query);
        DataTable OrderDetailTable(uint return_id, uint item_id);

        List<OrderDetailQuery> GetOrderDetailList(OrderDetailQuery query);
        DataTable GetBuyAmountAndTaxType(OrderDetailQuery query);
        List<Vendor> GetVendor(Vendor query);
        DataTable GetCategorySummary(OrderDetailQuery query);
    }
}
