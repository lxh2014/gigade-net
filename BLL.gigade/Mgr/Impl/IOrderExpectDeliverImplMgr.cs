/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderExpectDeliverImplMgr.js 
 * 摘   要： 預購單
 *  
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j 
 * 完成日期：2014/10/21 14:10:10
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderExpectDeliverImplMgr
    {
        List<OrderExpectDeliverQuery> GetOrderExpectList(OrderExpectDeliverQuery query, out int totalCount);
        int OrderExpectModify(OrderExpectDeliverQuery store);
        List<OrderExpectDeliverQuery> GetModel(OrderExpectDeliverQuery store);
    }
}
