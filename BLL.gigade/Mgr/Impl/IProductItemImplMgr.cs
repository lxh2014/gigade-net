/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductItemImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 14:00:06 
 * 
 */

using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;
using System.Collections.Generic;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductItemImplMgr
    {
        List<BLL.gigade.Model.ProductItem> Query(BLL.gigade.Model.ProductItem query);
        List<BLL.gigade.Model.ProductItem> QueryPrice(BLL.gigade.Model.ProductItem query);
        List<Model.ProductItem> Query(Model.Custom.PriceMasterCustom query);
        string UpdateStock(BLL.gigade.Model.ProductItem item);
        //更新product_item表export_flag字段 edit by xiangwang0413w 2014/06/30
        void UpdateExportFlag(Model.ProductItem item);
        bool Save(List<Model.ProductItem> saveList);
        string SaveSql(BLL.gigade.Model.ProductItem item);
        string Update(Model.ProductItem item);
        string QueryStock(Model.ProductItem pItem);
        List<Model.Custom.StockDataCustom> QueryItemStock(int product_id, int pile_id);
        List<Model.Custom.StockDataCustom> VendorQueryItemStock(string product_id, int pile_id);
        string Delete(Model.ProductItem delModel);
        string UpdateCopySpecId(Model.ProductItem proItem);
        List<Model.ProductItem> GetProductNewItem_ID(int product_id); //add by wangwei0216w 2014/9/19
        List<Model.ProductItem> GetProductItemByID(int productId);//add by wangwei0216w 2014/9/22
        ProductItemCustom GetProductArriveDay(ProductItem pi,string type);//add by wwei0216w 2015/6/4
        List<ProductItemQuery> GetProductItemByID(ProductItemQuery query);//根據item_id查詢商品規格以及商品的信息add chaojie1124j2015/8/31
        List<ProductItemQuery> GetInventoryQueryList(ProductItemQuery query, out int totalCount); //by yachao1120j 2015-9-10 商品库存查询
        int UpdateItemStock(ProductItem query, string path, BLL.gigade.Model.Caller user);//料位庫存調整的時候，商品庫存也做相應的調整add chaojie1124j2015/9/17
        List<ProductItemQuery> GetWaitLiaoWeiList(ProductItemQuery query, out int totalCount); //by yachao1120j 2015-10-20 等待料位報表
    }
}
