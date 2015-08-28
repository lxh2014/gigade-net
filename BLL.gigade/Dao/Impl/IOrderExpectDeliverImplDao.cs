#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IOrderExpectDeliverImplDao.cs      
* 摘 要：                                                                               
* 預購單
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/10/21
* 修改歷史：                                                                     
*         
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderExpectDeliverImplDao
    {
         List<OrderExpectDeliverQuery> GetOrderExpectList(OrderExpectDeliverQuery query, out int totalCount);
         int OrderExpectModify(OrderExpectDeliverQuery store);
         List<OrderExpectDeliverQuery> GetModel(OrderExpectDeliverQuery store);
    }
}
