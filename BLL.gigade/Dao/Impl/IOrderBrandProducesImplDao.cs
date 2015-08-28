#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IOrderBrandProducesImplDao.cs        
*摘要 
*
* 品牌訂單查詢與Dao的接口  
*當前版本：v1.1 
*
版本號：每次修改文件之後需要將版本號+1
* 作 者：changjian0408j                                          
      
* 完成日期：2014/8/20
* 
* 修改歷史
* v1.1修改日期：
*         
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Dao.Impl
{
    #region IOrderBrandProducesImplDao接口
    public interface IOrderBrandProducesImplDao
    {
        List<OrderBrandProducesQuery> GetOrderBrandProduces(OrderBrandProducesQuery store, out int totalCount, string conditionStr);
        List<OrderBrandProducesQuery> OrderBrandProducesExport(OrderBrandProducesQuery store, string conditionStr);
        DataTable GetOrderVendorRevenuebyday(string sqland, string brand = null);
    }
    #endregion
}

