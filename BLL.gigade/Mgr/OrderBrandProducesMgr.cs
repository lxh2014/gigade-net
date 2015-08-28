#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：OrderBrandProducesMgr.cs      
*摘要 
*
* 品牌訂單查詢Mgr接口
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
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class OrderBrandProducesMgr
    {
        public OrderBrandProducesDao _iorderbrand;
        public OrderBrandProducesMgr(string connectionString)
        {
            _iorderbrand = new OrderBrandProducesDao(connectionString);
        }

        #region 品牌訂單查詢
        public List<OrderBrandProducesQuery> GetOrderBrandProduces(OrderBrandProducesQuery store, out int totalCount, string conditionStr)
        {
            try
            {
                return _iorderbrand.GetOrderBrandProduces(store, out totalCount, conditionStr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderBrandProducesDao-->GetOrderBrandProduces-->" +ex.Message, ex);
            }
        }
        #endregion

        public List<OrderBrandProducesQuery> OrderBrandProducesExport(OrderBrandProducesQuery store, string conditionStr)
        {
            try
            {
                return _iorderbrand.OrderBrandProducesExport(store, conditionStr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderBrandProducesDao-->OrderBrandProducesExport-->" + ex.Message, ex);
            }           
        }
        public DataTable GetOrderVendorRevenuebyday(string sqland,string brand=null)
        {
            try
            {
                return _iorderbrand.GetOrderVendorRevenuebyday(sqland,brand);
             }
            catch (Exception ex)
            {
                throw new Exception("OrderBrandProducesDao-->GetOrderVendorRevenuebyday-->" + ex.Message, ex);
            }
        }
    }
}
