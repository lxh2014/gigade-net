/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductItemMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 14:01:18 
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model.Custom;
using System.Collections;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr
{
    public class ProductItemMgr : IProductItemImplMgr
    {
        private IProductItemImplDao _productItemDao;
        private ISerialImplMgr _serialMgr;
        private ProductItemDao productItemDao;
        string connectionStr;
        IFunctionImplMgr _functionMgr;
        public ProductItemMgr(string connectionStr)
        {
            this.connectionStr = connectionStr;
            _productItemDao = new ProductItemDao(connectionStr);
            _serialMgr = new SerialMgr(connectionStr);
            productItemDao = new ProductItemDao(connectionStr);
            _functionMgr = new FunctionMgr(connectionStr);
        }

        public List<ProductItem> Query(ProductItem query)
        {
            return _productItemDao.Query(query);
        }

        public List<ProductItem> QueryPrice(ProductItem query)
        {
            return _productItemDao.QueryPrice(query);
        }

        public List<ProductItem> Query(Model.Custom.PriceMasterCustom query)
        {
            return _productItemDao.Query(query);
        }

        public string UpdateStock(BLL.gigade.Model.ProductItem item)
        {
            return _productItemDao.UpdateStock(item);
        }

        //更新product_item表export_flag字段 edit by xiangwang0413w 2014/06/30
        public void UpdateExportFlag(BLL.gigade.Model.ProductItem item)
        {
             _productItemDao.UpdateExportFlag(item);
        }

        public bool Save(List<Model.ProductItem> saveList)
        {
            try
            {
                bool result = true;
                string product_id = string.Empty;
                try
                {
                    foreach (ProductItem item in saveList)
                    {
                        item.Item_Id = uint.Parse(_serialMgr.NextSerial(19).ToString());
                        if (_productItemDao.Save(item) <= 0)
                        {
                            result = false;
                        }
                        product_id = item.Product_Id.ToString();
                    }
                    _productItemDao.UpdateErpId(product_id);
                }
                catch (Exception ex)
                {
                    result = false;
                    throw ex;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }


        public string SaveSql(ProductItem item)
        {
            return _productItemDao.SaveSql(item);
        }

        public string Update(Model.ProductItem item)
        {
            return _productItemDao.Update(item);
        }

        //查詢單一商品庫存
        public string QueryStock(ProductItem pItem)
        {
            try
            {
                StringBuilder stb = new StringBuilder();
                List<Model.ProductItem> result = _productItemDao.Query(pItem);
                int defaultArriveDays = new ProductMgr(connectionStr).GetDefaultArriveDays(new Product { Product_Id=pItem.Product_Id });

                if (result.Count == 0)
                {
                    return "{success:true,items:[]}";
                }
                else
                {
                    stb.Append("{success:true,items:[");
                    foreach (var item in result)
                    {
                        //ediy by wwei0216w 分開顯示Arrive_Days + defaultArriveDays 所以不需要計算和 2015/10/12
                        //item.Arrive_Days += defaultArriveDays;
                        stb.Append("{");
                        stb.AppendFormat("\"spec_title_1\":\"{0}\",\"spec_title_2\":\"{1}\",\"item_stock\":\"{2}\",\"item_alarm\":\"{3}\",\"barcode\":\"{4}\",\"spec_id_1\":\"{5}\",\"spec_id_2\":\"{6}\",\"item_id\":\"{7}\",\"item_code\":\"{8}\",\"erp_id\":\"{9}\",\"remark\":\"{10}\",\"arrive_days\":\"{11}\",\"default_arrive_days\":\"{12}\"", item.Spec_Name_1, item.Spec_Name_2, item.Item_Stock, item.Item_Alarm, item.Barcode, item.Spec_Id_1, item.Spec_Id_2, item.Item_Id, item.Item_Code, item.Erp_Id, item.Remark, item.Arrive_Days, defaultArriveDays);//edit by xiangwang0413w 2014/06/18 (增加ERP廠商編號erp_id)  // add by zhuoqin0830w 2014/02/05 增加備註  //add by zhuoqin0830w 2014/03/20 增加運達天數
                        stb.Append("}");
                    }
                    stb.Append("]}");
                    return stb.ToString().Replace("}{", "},{");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr.QueryStock-->" + ex.Message, ex);
            }
        }

        public List<Model.Custom.StockDataCustom> QueryItemStock(int product_id, int pile_id)
        {
            return _productItemDao.QueryItemStock(product_id, pile_id);
        }
       public List<Model.Custom.StockDataCustom> VendorQueryItemStock(string product_id, int pile_id)
        {
            return _productItemDao.VendorQueryItemStock(product_id, pile_id);
       }
        public string Delete(ProductItem delModel)
        {
            return _productItemDao.Delete(delModel);
        }

        public string UpdateCopySpecId(ProductItem proItem)
        {
            return _productItemDao.UpdateCopySpecId(proItem);
        }

        public List<ProductItem> GetProductNewItem_ID(int product_id)
        {
            return _productItemDao.GetProductNewItem_ID(product_id);
        }

        /// <summary>
        /// 根據Id獲取productitem的信息
        /// </summary>
        /// <param name="productId">商品Id</param>
        /// <returns>符合條件的集合</returns>
        /// add by wangwei0216w 2014/9/22
        public List<ProductItem> GetProductItemByID(int productId)
        {
            return _productItemDao.GetProductItemByID(productId);
        }

        /// <summary>
        /// 獲取商品建議採購量信息
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>商品建議採購列表</returns>
        public DataTable GetSuggestPurchaseInfo(ProductItemQuery query,out int totalCount)
        {
            try
            {
                return productItemDao.GetSuggestPurchaseInfo(query,out totalCount );
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr-->GetSuggestPurchaseInfo-->" + ex.Message,ex);
            }
        }
        /// <summary>
        /// chaojie1124j add by 2015/10/26 實現下架狀態明細表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public DataTable GetStatusListLowerShelf(ProductQuery query, out int TotalCount)//List<ProductQuery>
        {
            try
            {
                return productItemDao.GetStatusListLowerShelf(query, out TotalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr-->GetStatusListLowerShelf-->" + ex.Message, ex);
            }
        }
        public ProductItemCustom GetProductArriveDay(ProductItem pi,string type)
        {
            try
            {
                return productItemDao.GetProductArriveDay(pi, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr-->GetProductArriveDay" + ex.Message, ex);
            }
        }
        public List<ProductItemQuery> GetProductItemByID(ProductItemQuery query)
        {
            try
            {
                return productItemDao.GetProductItemByID(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr-->GetProductItemByID" + ex.Message, ex);
            }
        }

        public List<ProductItemQuery> GetInventoryQueryList(ProductItemQuery query, out int totalCount)
        {
            try
            {
                List<ProductItemQuery> store = new List<ProductItemQuery>();
                store = _productItemDao.GetInventoryQueryList(query, out totalCount);
                foreach (var item in store)
                {
                    item.product_spec = item.Spec_Name_1;
                    item.product_spec += string.IsNullOrEmpty(item.Spec_Name_1) ? item.Spec_Name_2 : (string.IsNullOrEmpty(item.Spec_Name_2) ? "" : " / " + item.Spec_Name_2);
                    if (item.ignore_stock == 0)
                    {
                        item.ignore_stock_string = "否";
                    }
                    if (item.ignore_stock == 1)
                    {
                        item.ignore_stock_string = "是";
                    }
                }

                return store;

            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr->GetInventoryQueryList" + ex.Message);
            }
        }
        public int UpdateItemStock(ProductItem query,string path,BLL.gigade.Model.Caller user) 
        {
            Boolean result = false;
            try
            {
                ArrayList aList = new ArrayList();
                ITableHistoryImplMgr _tableHistoryMgr = new TableHistoryMgr(connectionStr);//實例化歷史記錄的類

                Int64 n_Time = BLL.gigade.Common.CommonFunction.GetPHPTime();
                Function myFun = new Function();
                myFun.FunctionCode = path;
                List<Function> funList = _functionMgr.Query(myFun);
                int functionid = funList.Count == 0 ? 0 : funList[0].RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid };
                batch.kuser = user.user_email;

                //獲取歷史記錄SQL
                string Check = productItemDao.UpdateItemStock(query);

                //獲取修改庫存SQL  
               
                ProductItem item = new ProductItem();
                item = productItemDao.Query(query).Count > 0 ? productItemDao.Query(query)[0] : item;

                batch.batchno = n_Time + "_" + user.user_id + "_" + item.Product_Id;
                if (item != null)
                {
                    item.Item_Stock =  item.Item_Stock +query.Item_Stock;
                    
                    aList.Add(Check);
                    result = _tableHistoryMgr.SaveHistory<ProductItem>(item, batch, aList);
                }

                ////獲取修改商品Ignore SQL  
                //string Ignore_Stock = string.Empty;
                //Product product = new Product();
                //product = productItemDao.GetTaxByItem(query.Item_Id);
                //if (product != null)
                //{
                //    product.Ignore_Stock = 0;
                //    aList.Clear();
                //    aList.Add(Ignore_Stock);
                //    result = _tableHistoryMgr.SaveHistory<Product>(product, batch, aList);
                //}

                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr-->UpdateItemStock" + ex.Message, ex);
            }
        }

        public List<ProductItemQuery> GetWaitLiaoWeiList(ProductItemQuery query, out int totalCount)// 等待料位報表
        {
            try
            {
                List<ProductItemQuery> store = new List<ProductItemQuery>();
                store = _productItemDao.GetWaitLiaoWeiList(query, out totalCount);
                foreach (var item in store)
                {
                    item.product_createdate_string = CommonFunction.GetNetTime(item.product_createdate).ToString("yyyy-MM-dd HH:mm:ss");
                    item.product_start_string = CommonFunction.GetNetTime(item.product_start).ToString("yyyy-MM-dd HH:mm:ss");
                    item.product_spec = item.Spec_Name_1;
                    item.product_spec += string.IsNullOrEmpty(item.Spec_Name_1.Trim()) ? item.Spec_Name_2 : (string.IsNullOrEmpty(item.Spec_Name_2.Trim()) ? "" : " / " + item.Spec_Name_2);
                    //商品類型
                    if (item.combination == 1)
                    {
                        item.combination_string = "單一商品";
                    }
                    if (item.combination == 2)
                    {
                        item.combination_string = "固定組合";
                    }
                    if (item.combination == 3)
                    {
                        item.combination_string = "任選組合";
                    }
                    if (item.combination == 4)
                    {
                        item.combination_string = "群組搭配";
                    }

                    //出貨方式
                    if (item.product_mode == 1)
                    {
                        item.product_mode_string = "自出";
                    }
                    if (item.product_mode == 2)
                    {
                        item.product_mode_string = "寄倉";
                    }
                    if (item.product_mode == 3)
                    {
                        item.product_mode_string = "調度";
                    }
                    //商品狀態
                    if (item.product_status == 0)
                    {
                        item.product_status_string = "新建立商品";
                    }
                    if (item.product_status == 1)
                    {
                        item.product_status_string = "申請審核";
                    }
                    if (item.product_status == 2)
                    {
                        item.product_status_string = "審核通過";
                    }
                    if (item.product_status == 5)
                    {
                        item.product_status_string = "上架";
                    }
                    //溫層
                    if (item.delivery_freight_set == 1)
                    {
                        item.product_freight_set_string = "常溫";
                    }
                    if (item.delivery_freight_set == 2)
                    {
                        item.product_freight_set_string = "冷凍";
                    }
                    //採購單單號
                    if (!string.IsNullOrEmpty(item.po_id))
                    {
                        item.po_id = " " + item.po_id;
                    }             
                }

                return store;

            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMgr->GetWaitLiaoWeiList" + ex.Message);
            }
        }

    }
}
