/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderSlaveImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:06:24 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderSlaveImplMgr
    {
        string Save(BLL.gigade.Model.OrderSlave orderSlave);
        int Delete(int slaveId);
        List<OrderSlaveQuery> GetOrderWaitDeliver(OrderSlaveQuery store, string str, out int totalCount);
        List<OrderSlaveQuery> GetAllOrderWait(OrderSlaveQuery store, string str, out int totalCount);
        OrderSlaveQuery GetOrderDatePay(OrderSlaveQuery query);
        List<OrderSlaveQuery> GetVendorWaitDeliver(OrderSlaveQuery store, string str, out int totalCount);
        DataTable GetList(OrderSlaveQuery orderSlaveQuery, out int totalCount);
        DataTable GetListPrint(OrderSlaveQuery query, string addsql = null);
    }
}
