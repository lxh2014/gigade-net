/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 13:24:16 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductImplDao
    {
        List<BLL.gigade.Model.Product> Query(Model.Product query);
        /// <summary>
        /// 根據商品列表條件查詢商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<Model.Custom.PriceMasterCustom> Query(Model.Query.QueryVerifyCondition query);
        List<Product> Query(uint[] productIds);
        string Save(Model.Product product);
        string Update(Model.Product product);
        /// <summary>
        /// 只更新排序
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        string UpdateSort(Model.Product product);
        /// <summary>
        /// 查找該品牌下商品最大的商品序號
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        uint QueryMaxSort(uint brandId);
        int TempMove2Pro(string product,string courseProduct, string proItem,string courDetItem, string selPro, string priceMaster, string itemPrice, ArrayList sqls);
        bool ProductMigration(string product, ArrayList priceMasters, ArrayList items, ArrayList itemPrices, ArrayList sqls, ArrayList specs);
        List<Model.Custom.QueryandVerifyCustom> QueryandVerify(Model.Query.QueryVerifyCondition qcCon, ref int total);
        /// <summary>
        /// 待審核列表查詢
        /// </summary>
        /// <param name="qcCon"></param>
        /// <returns></returns>
        List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount);
        List<Model.Custom.QueryandVerifyCustom> QueryByProSite(Model.Query.QueryVerifyCondition query, out int totalCount);
        List<Model.Custom.QueryandVerifyCustom> QueryByProSite(string priceMasterId);
        List<Model.Custom.QueryandVerifyCustom> QueryForStation(Model.Query.QueryVerifyCondition query, out int totalCount);//add 2014/08/25
        List<Model.Custom.QueryandVerifyCustom> GetProductInfoByID(string productID); //add 2014/08/25 wangwei0216w
        Model.Custom.ProductDetailsCustom ProductDetail(Model.Product query);
        string Delete(uint product_id);
        Model.Custom.OrderComboAddCustom OrderQuery(Model.Product query, uint user_level,uint user_id,uint site_id);
        int QueryClassId(int pid);
        List<Model.Custom.ProductItemCustom> GetStockInfo(Model.Query.QueryVerifyCondition query); //add by wangwei0216w 2014/10/20

        List<Model.Custom.ProductDetailsCustom> GetAllProList(Model.Query.ProductQuery query,  out int totalCount);

        #region 與供應商商品相關
        /// <summary>
        /// 獲取供應商商品
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>與供應商相關的商品列表</returns>
        List<VenderProductListCustom> GetVendorProduct(VenderProductList query, out int totalCount);
        Model.Custom.ProductDetailsCustom VendorProductDetail(ProductTemp query);
        List<Product> VendorQuery(ProductTemp query);
        string Vendor_Delete(string product_id);
        #endregion

        int Yesornoexist(int i, int j);//判斷產品ID和類別id在product_category_set表中是否存在    add by dongya0410j 2015/01/05
        int Yesornoexistproduct(int i);
        int Yesornoexistproductcategory(int i);
        int Updateproductcategoryset(string condition);//更新表product_category_set表
        DataTable Updownerrormessage(ProductCategory pc);
        //add by wwei0216w 2015/1/27
        /// <summary>
        /// 獲得Product.SaleStatus狀態以及相關的列
        /// </summary>
        /// <param name="p">查詢條件</param>
        /// <returns>List集合</returns>
        string UpdateSaleStatusBatch(Int64 nowTime);

        string UpdateSaleStatusByCondition(Int64 nowTime, Product p);   

        //add by wwei0216w 2015/1/28
        /// <summary>
        /// 更新product表中的某一個具體列的值
        /// </summary>
        /// <returns>sql語句</returns>
        string UpdateColumn(Product p, string columnName);

        /// <summary>
        /// 獲取可以判斷商品販售狀態的信息
        /// </summary>
        /// <param name="productId">需要判斷售狀態的商品Id</param>
        /// <param name="prods">組合商品中子商品的list集合</param>
        /// <param name="prodItems">商品子商品的庫存list集合</param>
        /// <returns></returns>
        ProductCustom QueryProductInfo(uint productId, out List<Product> prods, out List<ProductItem> prodItems, out PriceMaster gigaPriceMaster);

        DataTable GetNameForID(int prodID, int classID, int brandID);//抓取合格的商品名稱  add by shuangshuang0420j 2015.01.27

        DataTable dt();
        /// <summary>
        /// 供應商下面的商品
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetVendorProductList(ProductQuery query, out int totalCount);
        Product QueryClassify(uint product_id);//獲取商品是食品還是用品  add by mengjuan0826j 2015.03.11

        DataTable GetVendorProductSpec(ProductQuery query, out int totalCount);

        string UpdateStock(int newstock, int oldstock, int item_id, int vendor);
        int GetDefaultArriveDays(Product prod);
        List<Product> GetProductName(Product p);
        string UpdateStatus(int spec_id, int spce_status);

        #region 商品詳情說明
        DataTable GetProductList(ProductQuery p, out int totalCount);
        int UpdateProductDeatail(Product p);
        DataTable GetVendor(Vendor v);
        DataTable GetProductDetialText(Product p);
        #endregion
        List<Product> GetProductByVendor(int vendor_id);//add by wwei0216w 2015/6/3 根據供應商id獲得商品ids

        /// <summary>
        /// 設置商品失格(一旦設置,無法還原)
        /// </summary>
        /// <param name="product_id">需要設置商品失格的id</param>
        /// <returns>成功 OR 失敗</returns>
        bool UpdateOff_Grade(uint product_id, int off_grade);//add by wwei0216w 2015/6/24
        int GetProductType(Product query);
    }
}
