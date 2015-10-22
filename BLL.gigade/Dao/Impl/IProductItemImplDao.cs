/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductItemImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 13:24:32 
 * 
 */

using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductItemImplDao
    {
        List<BLL.gigade.Model.ProductItem> Query(Model.ProductItem query);
        List<BLL.gigade.Model.ProductItem> QueryPrice(Model.ProductItem query);
        List<Model.ProductItem> Query(Model.Custom.PriceMasterCustom query);
        string UpdateStock(Model.ProductItem item);
        //更新product_item表export_flag字段 edit by xiangwang0413w 2014/06/30
        void UpdateExportFlag(Model.ProductItem item);
        string Update(Model.ProductItem item);
        string SaveSql(Model.ProductItem item);
        int Save(Model.ProductItem saveModel);
        string Delete(Model.ProductItem delModel);
        List<Model.Custom.StockDataCustom> QueryItemStock(int product_id, int pile_id);
        List<Model.Custom.StockDataCustom> VendorQueryItemStock(string product_id, int pile_id);
        string UpdateCopySpecId(Model.ProductItem proItem);

        /// <summary>
        /// 修改Erp_id，因為涉及到product_item.item_id所以，這裡在保存完了product_item后再統一對該product下的所有item進行Update
        ///        碼別	            碼別說明	    資料說明
        ///        第1碼	        類別	            都是3(商品)
        ///        第2-3碼	    大分類	        第2~5碼為product.cate_id
        ///         第4-5碼	    中分類	　
        ///        第6-11碼	    前台商品碼	product_item.item_id
        ///        第12-13碼	規格1	            若無則00            
        ///        第14-15碼	規格2	        若無則00
        ///        第16碼	        應免稅檢查	0:免稅 1:應稅
        /// </summary>
        /// <param name="pro_id"></param>
        void UpdateErpId(string pro_id);
        List<Model.ProductItem> GetProductNewItem_ID(int product_id);//add by wangwei0216w 2014/9/19
        List<Model.ProductItem> GetProductItemByID(int productId);//add by wangwei0216w 2014/9/22
        string UpdateItemStock(uint Item_Id, uint Item_Stock);//add by mengjuan0826j 2014/10/29
        ProductItemCustom GetProductArriveDay(ProductItem pi, string type);//查詢和運達天數相關的信息 add by wwei2016w 2015/6/6
        string GetItemInfoByProductIds(string productIds);
        Product GetTaxByItem(uint item_id);//根據item_is查詢稅額類型 add mengjuan0826j 2015/7/2
        List<ProductItemQuery> GetProductItemByID(ProductItemQuery query);//根據item_id查詢商品規格以及商品的信息add chaojie1124j2015/8/31       
        List<ProductItemQuery> GetInventoryQueryList(ProductItemQuery query, out int totalCount);
        string UpdateItemStock(ProductItem query);//料位庫存調整的時候，商品庫存也做相應的調整add chaojie1124j2015/9/17
        List<ProductItemQuery> GetWaitLiaoWeiList(ProductItemQuery query, out int totalCount);
    }
}
