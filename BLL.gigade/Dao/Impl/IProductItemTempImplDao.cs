/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductItemTempImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/28 11:29:53 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    interface IProductItemTempImplDao
    {
        int Save(List<ProductItemTemp> saveTempList);
        int Delete(Model.ProductItemTemp delTemp);
        List<Model.ProductItemTemp> Query(Model.ProductItemTemp proItemTemp);
        string UpdateCostMoney(Model.ProductItemTemp productItemTemp);
        string UpdateStockAlarm(Model.ProductItemTemp productItemTemp);
        int UpdateItemStock(Model.ProductItemTemp productItemTemp);
        string QuerySql(Model.ProductItemTemp proItemTemp);

        string MoveProductItem(Model.ProductItemTemp proItemTemp);
        string DeleteSql(Model.ProductItemTemp proItemTemp);
        string DeleteVendorSql(Model.ProductItemTemp proItemTemp);
        string SaveFromProItem(Model.ProductItemTemp proItemTemp);
        string UpdateCopySpecId(Model.ProductItemTemp proItemTemp);
        List<Model.ProductItemTemp> QueryByVendor(ProductItemTemp proItemTemp);
        int SaveByVendor(ProductItemTemp saveTemp);
        int DeleteByVendor(Model.ProductItemTemp delTemp);
        string UpdateStockAlarmByVendor(Model.ProductItemTemp productItemTemp);
        string VendorSaveFromProItem(ProductItemTemp proItemTemp, string old_product_id);
        string VendorUpdateCopySpecId(ProductItemTemp proItemTemp);
        string UpdateByVendor(Model.ProductItemTemp item);
        string VendorQuerySql(ProductItemTemp proItemTemp);
        string VendorMoveProductItem(ProductItemTemp proItemTemp);
    }
}
