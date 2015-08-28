/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 13:59:53 
 * 
 */

using System.Collections.Generic;
using System.Collections;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductImplMgr
    {
        DataTable dt();

        List<BLL.gigade.Model.Product> Query(BLL.gigade.Model.Product query);
        List<Model.Custom.PriceMasterCustom> Query(Model.Query.QueryVerifyCondition query);
        string Save(BLL.gigade.Model.Product product);
        string Update(Model.Product product, int kuser = 0);
        bool ExecUpdate(Model.Product product);
        /// <summary>
        /// 更改排序
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        bool ExecUpdateSort(List<Model.Product> productList);
        uint GetProductSort(uint brandId);
        int TempMove2Pro(int writerId, int combo_type, string product_Id);
        bool ProductMigration(Model.Product pro, List<Model.PriceMaster> priceMasters, List<Model.ProductItem> items, List<List<Model.ItemPrice>> itemPrices, ArrayList sqls, ArrayList specs);
        bool Update_Product_Spec_Picture(BLL.gigade.Model.Product p, List<BLL.gigade.Model.ProductSpec> pSList, List<BLL.gigade.Model.ProductPicture> pPList, List<ProductPicture> appList = null);
        bool Delete(uint product_Id);
        List<Model.Custom.QueryandVerifyCustom> QueryandVerify(Model.Query.QueryVerifyCondition qcCon, ref int total);
        List<Model.Custom.QueryandVerifyCustom> QueryByProSite(Model.Query.QueryVerifyCondition query, out int totalCount);
        List<Model.Custom.QueryandVerifyCustom> QueryByProSite(string priceMasterIds);
        //add by jiajun 2014/08/22
        List<Model.Custom.QueryandVerifyCustom> QueryForStation(Model.Query.QueryVerifyCondition query, out int totalCount);
        List<Model.Custom.QueryandVerifyCustom> GetProductInfoByID(string productID);//add by wangwei 0214/8/26
        /// <summary>
        /// 待審核列表查詢
        /// </summary>
        /// <param name="qcCon"></param>
        /// <returns></returns>
        List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount);
        Model.Custom.ProductDetailsCustom ProductDetail(Model.Product query);
        Model.Custom.OrderComboAddCustom OrderQuery(Model.Product query, uint user_level, uint user_id, uint site_id);
        int QueryClassId(int pid);

        List<Model.Custom.ProductDetailsCustom> GetAllProList(Model.Query.ProductQuery query, out int totalCount);
        #region 供應商後臺相關
        /// <summary>
        /// 獲取供應商商品
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>與供應商相關的商品列表</returns>
        List<VenderProductListCustom> GetVendorProduct(VenderProductList query, out int totalCount);
        Model.Custom.ProductDetailsCustom VendorProductDetail(ProductTemp query);
        List<Product> VendorQuery(ProductTemp query);
        bool Vendor_Delete(string product_Id);
        int Vendor_TempMove2Pro(int writerId, int combo_type, string product_Id, ProductTemp pt);
        #endregion
        System.IO.MemoryStream ExportProductToExcel(BLL.gigade.Model.Query.QueryVerifyCondition query, int exportFlag, string cols, string fileName);//add by xiangwang0413w 2014/10/20

        int Yesornoexist(int i, int j);//判斷產品ID和類別id在product_category_set表中是否存在    add by dongya0410j 2015/01/05
        int Yesornoexistproduct(int i);
        int Yesornoexistproductcategory(int i);
        int Updateproductcategoryset(string condition);//更新表product_category_set表
        DataTable Updownerrormessage(ProductCategory pc);
        string GetNameForID(int prodID, int classID, int brandID, int isHidCombo = 0);
        //add by wwei0216w 2015/1/28

        /// <summary>
        /// 查詢商品的Status信息
        /// </summary>
        /// <param name="p">查詢條件</param>
        /// <returns>List<ProductCustom>集合</returns>
        Product UpdateSaleStatus(uint productId);

        //add by wwei0216w 2015/1/27

        /// <summary>
        /// 獲得Product.SaleStatus狀態以及相關的列
        /// </summary>
        /// <returns>List<ProductCustom>集合</returns>
        void UpdateSaleStatusBatch();

        /// <summary>
        /// 根據條件批量修改 販售狀態
        /// </summary>
        /// <param name="nowTime"></param>
        /// <param name="p"></param>
        bool UpdateSaleStatusByCondition(Product p);

        /// <summary>
        /// 批量更新商品狀態
        /// </summary>
        /// <param name="p">更新的條件</param>
        /// <returns>success or fail</returns>
        // bool UpdateSaleStatusByBatch(Product p);

        //add by wwei0216w 2015/1/28
        /// <summary>
        /// 更新product表中的某一個具體列的值
        /// </summary>
        /// <returns>sql語句</returns>
        string UpdateColumn(Product p, string columnName);
        int GetDefaultArriveDays(Product prod);

        /// <summary>
        /// 供應商下的商品
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetVendorProductList(ProductQuery query, out int totalCount);

        Product QueryClassify(uint product_id);//獲取商品是食品還是用品  add by mengjuan0826j 2015.03.11

        DataTable GetVendorProductSpec(ProductQuery query, out int totalCount);
        bool UpdateStock(int newstock, int oldstock, int item_id, int vendor);
        List<Product> GetProductName(Product p);
        bool UpdateStatus(int spec_id, int spce_status);

        #region 商品詳情說明
        DataTable GetProductList(ProductQuery p, out int totalCount);
        int UpdateProductDeatail(Product p);
        DataTable GetVendor(Vendor v);
        DataTable GetProductDetialText(ProductQuery p, string id, out string notFind);
        #endregion

        /// <summary>
        /// 設置商品失格(一旦設置,無法還原)
        /// </summary>
        /// <param name="product_id">需要設置商品失格的id</param>
        /// <returns>成功 OR 失敗</returns>
        int GetProductType(Product query);
        bool UpdateOff_Grade(uint product_id, int off_grade);//add by wwei0216w 2015/6/24
    }
}
