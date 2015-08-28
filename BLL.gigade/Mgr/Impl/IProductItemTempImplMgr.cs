/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductItemTempImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/28 11:28:56 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductItemTempImplMgr
    {
        bool Save(List<Model.ProductItemTemp> saveTempList);
        List<Model.ProductItemTemp> Query(Model.ProductItemTemp proItemTemp);
        string QueryStock(Model.ProductItemTemp proItemTemp);
        bool UpdateCostMoney(List<Model.ProductItemTemp> productItemTemp);
        bool UpdateStockAlarm(List<Model.ProductItemTemp> productItemTemp);
        bool UpdateItemStock(Model.ProductItemTemp productItemTemp);
        bool Delete(Model.ProductItemTemp delTemp);
        string QuerySql(Model.ProductItemTemp proItemTemp);
        string DeleteVendorSql(Model.ProductItemTemp proItemTemp);
        string MoveProductItem(Model.ProductItemTemp proItemTemp);
        string DeleteSql(Model.ProductItemTemp proItemTemp);
        string SaveFromProItem(Model.ProductItemTemp proItemTemp);
        string UpdateCopySpecId(Model.ProductItemTemp proItemTemp);
        List<Model.ProductItemTemp> QueryByVendor(Model.ProductItemTemp proItemTemp);
        bool SaveByVendor(List<Model.ProductItemTemp> saveTempList);
        bool DeleteByVendor(Model.ProductItemTemp delTemp);
        string QueryStockByVendor(Model.ProductItemTemp proItemTemp);
        bool UpdateStockAlarmByVendor(List<Model.ProductItemTemp> productItemTemp);
        string VendorSaveFromProItem(ProductItemTemp proItemTemp, string old_id);
        string VendorUpdateCopySpecId(ProductItemTemp proItemTemp);
        string UpdateByVendor(Model.ProductItemTemp item);
        string VendorQuerySql(Model.ProductItemTemp proItemTemp);
        string VendorMoveProductItem(Model.ProductItemTemp proItemTemp);
    }
}
