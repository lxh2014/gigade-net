/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 14:01:08 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Collections;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;
using System.Text;
using System.IO;
using System.Xml.Linq;
using gigadeExcel.Comment;
using System.Text.RegularExpressions;
using BLL.gigade.Common;
using System.Data;
using System.Web.UI.WebControls;

namespace BLL.gigade.Mgr
{
    public class ProductMgr : IProductImplMgr
    {
        private IProductImplDao _productDao;
        private IProductSpecImplMgr _productSpecMgr;
        private IProductPictureImplDao _productPicDao;

        private IPriceMasterImplDao pmDao;
        private IPriceMasterTsImplDao pmTSDao;
        private IPriceUpdateApplyImplDao puApplyDao;
        private MySqlDao _mysqlDao;
        private ProductDao productDao;
        private string connectionStr;
        public ProductMgr(string connectionStr)
        {
            _productDao = new ProductDao(connectionStr);
            _productPicDao = new ProductPictureDao(connectionStr);
            pmDao = new PriceMasterDao(connectionStr);
            pmTSDao = new PriceMasterTsDao(connectionStr);
            puApplyDao = new PriceUpdateApplyDao(connectionStr);
            this.connectionStr = connectionStr;
            _mysqlDao = new MySqlDao(connectionStr);
            productDao = new ProductDao(connectionStr);
        }

        public DataTable dt()
        {
            try
            {
                return _productDao.dt();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.OrderQuery-->" + ex.Message, ex);
            }
        }

        public Model.Custom.OrderComboAddCustom OrderQuery(Product query, uint user_level, uint user_id, uint site_id)
        {
            try
            {
                return _productDao.OrderQuery(query, user_level, user_id, site_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.OrderQuery-->" + ex.Message, ex);
            }
        }


        public List<Product> Query(Product query)
        {
            return _productDao.Query(query);
        }

        public string Save(Product product)
        {
            return _productDao.Save(product);
        }

        public string Update(Model.Product product, int kuser)
        {
            StringBuilder stb = new StringBuilder();
            stb.Append(_productDao.Update(product));
            //if (this.Query(new Product { Product_Id = product.Product_Id }).First().Prod_Sz != product.Prod_Sz) {
            //    pmDao.Query(new PriceMaster { product_id = product.Product_Id }).ForEach(m => {
            //        m.apply_id = (uint)puApplyDao.Save(new PriceUpdateApply { price_master_id = m.price_master_id, apply_user = uint.Parse(kuser.ToString()) });

            //        string[] tmp = Regex.Split(PriceMaster.Product_Name_Op(m.product_name), "`LM`");
            //        tmp[2] = product.Prod_Sz;
            //        string result = tmp[0] + "`LM`" + tmp[1] + "`LM`" + tmp[2] + "`LM`" + tmp[3];
            //        m.product_name = PriceMaster.Product_Name_FM(result);
            //        m.price_status = 2;
            //        stb.Append(pmTSDao.UpdateTs(m));
            //    });                    
            //}
            //stb.Append(pmDao.UpdateProductName(product.Prod_Sz, product.Product_Id.ToString()));
            return stb.ToString();
        }

        public bool ExecUpdate(Model.Product product)
        {
            return _mysqlDao.ExcuteSqls(new ArrayList { _productDao.Update(product) });
        }

        public string UpdateSort(Model.Product product)
        {
            return _productDao.UpdateSort(product);
        }

        public uint GetProductSort(uint brandId)
        {
            return _productDao.QueryMaxSort(brandId) + 1;
        }

        public bool ExecUpdateSort(List<Model.Product> productList)
        {
            ArrayList arryList = new ArrayList();
            foreach (Product item in productList)
            {
                arryList.Add(UpdateSort(item));
            }

            return _mysqlDao.ExcuteSqls(arryList);

        }

        /// <summary>
        /// 多表更新
        /// </summary>
        /// <param name="p">product表</param>
        /// <param name="pIList">ProductSpec表</param>
        /// <param name="pSList">ProductPicture</param>
        /// <returns></returns>
        public bool Update_Product_Spec_Picture(Product p, List<ProductSpec> pSList, List<ProductPicture> pPList, List<ProductPicture> appList = null)
        {
            ArrayList sqlList = new ArrayList();
            _productSpecMgr = new ProductSpecMgr(connectionStr);

            List<Product> pList = _productDao.Query(p);

            pList[0].Product_Image = p.Product_Image;
            pList[0].product_media = p.product_media;
            pList[0].Mobile_Image = p.Mobile_Image; // edit by wwei0216w 2015/3/18 添加關於Mobile的修改
            pList[0].Product_alt = p.Product_alt;
            if (p.Product_Image != "" || p.product_media != "" || p.Mobile_Image != "" || p.Product_alt != "")// edit by wwei0216w 2015/3/18 添加關於Mobile的修改
            {
                sqlList.Add(_productDao.Update(pList[0]));
            }

            //ProductSpec表
            foreach (var item in pSList)
            {
                //查詢product_item表
                ProductSpec spec = _productSpecMgr.query(int.Parse(item.spec_id.ToString()));
                spec.spec_image = item.spec_image;
                spec.spec_sort = item.spec_sort;
                spec.spec_status = item.spec_status;
                sqlList.Add(_productSpecMgr.Update(spec));
            }

            //ProductPicture表
            sqlList.Add(_productPicDao.Delete(int.Parse(p.Product_Id.ToString())));
            foreach (var item in pPList)
            {
                sqlList.Add(_productPicDao.Save(item));
            }


            sqlList.Add(_productPicDao.Delete(int.Parse(p.Product_Id.ToString()), 2)); //1:商品說明表 2：手機app圖檔表
            foreach (var item in appList)
            {
                sqlList.Add(_productPicDao.Save(item, 2));
            }
            MySqlDao mySqlDao = new MySqlDao(connectionStr);
            return mySqlDao.ExcuteSqls(sqlList);
        }
        /// <summary>
        /// 刪除商品
        /// </summary>
        /// <returns></returns>
        public bool Delete(uint product_Id)
        {
            ArrayList delList = new ArrayList();
            ProductComboMgr proComboMgr = new ProductComboMgr("");
            delList.Add(proComboMgr.Delete(Convert.ToInt32(product_Id)));

            ItemPriceMgr itemPriceMgr = new ItemPriceMgr("");
            delList.Add(itemPriceMgr.DeleteByProductId(Convert.ToInt32(product_Id)));

            PriceMasterMgr priceMaster = new PriceMasterMgr("");
            delList.Add(priceMaster.DeleteByProductId(Convert.ToInt32(product_Id)));

            ProductTagSetMgr proTagSetMgr = new ProductTagSetMgr("");
            delList.Add(proTagSetMgr.Delete(new ProductTagSet { product_id = product_Id }));

            ProductNoticeSetMgr proNoticeSetMgr = new ProductNoticeSetMgr("");
            delList.Add(proNoticeSetMgr.Delete(new ProductNoticeSet { product_id = product_Id }));

            ProductPictureMgr proPicMgr = new ProductPictureMgr("");
            delList.Add(proPicMgr.Delete(Convert.ToInt32(product_Id)));

            ProductSpecMgr proSpecMgr = new ProductSpecMgr("");
            delList.Add(proSpecMgr.Delete(product_Id));

            ProductCategorySetMgr proCategorySetMgr = new ProductCategorySetMgr("");
            delList.Add(proCategorySetMgr.Delete(new ProductCategorySet { Product_Id = product_Id }));

            #region 課程相關
            CourseProductMgr courProdMgr = new CourseProductMgr("");
            delList.Add(courProdMgr.Delete(product_Id));

            CourseDetailItemMgr courDetaItemMgr = new CourseDetailItemMgr("");
            delList.Add(courDetaItemMgr.Delete(product_Id));
            #endregion

            ProductItemMgr proItemMgr = new ProductItemMgr("");
            delList.Add(proItemMgr.Delete(new ProductItem { Product_Id = product_Id }));

            ProductStatusHistoryMgr proStatusHistoryMgr = new ProductStatusHistoryMgr("");
            delList.Add(proStatusHistoryMgr.Delete(new ProductStatusHistory { product_id = product_Id }));

            delList.Add(_productDao.Delete(product_Id));

            MySqlDao mySqlDao = new MySqlDao(connectionStr);
            return mySqlDao.ExcuteSqls(delList);
        }

        public int TempMove2Pro(int writerId, int combo_type, string product_Id)
        {
            ArrayList sqls = new ArrayList();
            int product_id = 0;
            ProductTempMgr proTempMgr = new ProductTempMgr("");
            ProductTemp proTemp = new ProductTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id, Create_Channel = 1 };//1:後台管理者(manage_user) edit by xiagnwang0413w 2014/08/09
            string movePro = proTempMgr.MoveProduct(proTemp);
            sqls.Add(proTempMgr.Delete(proTemp));

            /*********start*********/
            //將ProductDeliverySetTemp表數據導入正式表 edit by xiangwang0413w 2014/11/06
            IProductDeliverySetTempImplMgr _proDelSetTemp = new ProductDeliverySetTempMgr("");
            var proDelSetTemp = new ProductDeliverySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_id = int.Parse(product_Id) };
            sqls.Add(_proDelSetTemp.MoveProductDeliverySet(proDelSetTemp));
            sqls.Add(_proDelSetTemp.Delete(proDelSetTemp));
            /*******end***********/

            ProductNoticeSetTempMgr proNoticeSetTempMgr = new ProductNoticeSetTempMgr("");
            ProductNoticeSetTemp proNoticeSetTemp = new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id };
            sqls.Add(proNoticeSetTempMgr.MoveNotice(proNoticeSetTemp));
            sqls.Add(proNoticeSetTempMgr.Delete(proNoticeSetTemp));

            ProductTagSetTempMgr proTagSetTempMgr = new ProductTagSetTempMgr("");
            ProductTagSetTemp proTagSetTemp = new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id };
            sqls.Add(proTagSetTempMgr.MoveTag(proTagSetTemp));
            sqls.Add(proTagSetTempMgr.Delete(proTagSetTemp));

            ProductPictureTempImplMgr proPicTempMgr = new ProductPictureTempImplMgr("");
            ProductPictureTemp proPictureTemp = new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_Id };
            sqls.Add(proPicTempMgr.MoveToProductPicture(proPictureTemp, 1));//更新說明圖表
            sqls.Add(proPicTempMgr.MoveToProductPicture(proPictureTemp, 2));//更新APP圖表
            sqls.Add(proPicTempMgr.Delete(proPictureTemp, 1));//刪除說明圖臨時表
            sqls.Add(proPicTempMgr.Delete(proPictureTemp, 2)); //刪除app臨時表 add by wwei0216w 2014/11/11

            ProductCategorySetTempMgr proCateSetTempMgr = new ProductCategorySetTempMgr("");
            ProductCategorySetTemp proCategorySetTemp = new ProductCategorySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id.ToString() };
            sqls.Add(proCateSetTempMgr.TempMoveCategory(proCategorySetTemp));
            sqls.Add(proCateSetTempMgr.TempDelete(proCategorySetTemp));

            ProductStatusHistoryMgr proStatusHistoryMgr = new ProductStatusHistoryMgr("");
            sqls.Add(proStatusHistoryMgr.SaveNoProductId(new ProductStatusHistory { product_status = 0, user_id = Convert.ToUInt32(writerId), type = 5 }));

            ItemPriceTempMgr itemTempPriceMgr = new ItemPriceTempMgr("");
            sqls.Add(itemTempPriceMgr.Delete(product_Id, combo_type, writerId));

            PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
            PriceMasterTemp priceMasterTemp = new PriceMasterTemp { writer_Id = writerId, product_id = product_Id, combo_type = combo_type };
            sqls.Add(priceMasterTempMgr.Delete(priceMasterTemp));

            //判斷是單一商品還是組合商品
            if (combo_type == 1)
            {//單一商品
                IProductItemImplDao piDao = new ProductItemDao(connectionStr);
                ProductItemTempMgr proItemTempMgr = new ProductItemTempMgr("");
                ProductItemTemp proItemTemp = new ProductItemTemp { Writer_Id = writerId, Product_Id = product_Id };
                string selItem = proItemTempMgr.QuerySql(proItemTemp);
                string moveItem = proItemTempMgr.MoveProductItem(proItemTemp);
                sqls.Add(proItemTempMgr.DeleteSql(proItemTemp));

                /*************start*課程相關*****************/
                //CourseDetailItem
                ICourseDetailItemTempImplMgr _cdItemMgr = new CourseDetailItemTempMgr("");
                string moveCourDetaItem = _cdItemMgr.MoveCourseDetailItem(writerId);
                sqls.Add(_cdItemMgr.DeleteSql(writerId));

                //CourseProduct
                ICourseProductTempImplMgr _courProdTempMgr = new CourseProductTempMgr("");
                var courProdTemp = new CourseProductTemp { Writer_Id = writerId, Product_Id = uint.Parse(product_Id) };
                string moveCourProd = _courProdTempMgr.MoveCourseProduct(courProdTemp);
                sqls.Add(_courProdTempMgr.DeleteSql(courProdTemp));
                /*************end**********************************/



                ProductSpecTempMgr proSpecTempMgr = new ProductSpecTempMgr("");
                ProductSpecTemp proSpecTemp = new ProductSpecTemp { Writer_Id = writerId, product_id = product_Id };
                sqls.Add(proSpecTempMgr.TempMoveSpec(proSpecTemp));
                sqls.Add(proSpecTempMgr.TempDelete(proSpecTemp));

                string priceMaster = priceMasterTempMgr.Move2PriceMaster(priceMasterTemp);

                ItemPriceMgr itemPriceMgr = new ItemPriceMgr("");
                string itemPrice = itemPriceMgr.SaveFromItem(writerId, product_Id);

                product_id = _productDao.TempMove2Pro(movePro, moveCourProd, moveItem, moveCourDetaItem, selItem, priceMaster, itemPrice, sqls);
                if (product_id > 0)
                {
                    piDao.UpdateErpId(product_id.ToString());
                }
                return product_id;
            }
            else
            {//組合商品
                ProductComboTempMgr pcTempMgr = new ProductComboTempMgr("");
                ProductComboTemp proComboTemp = new ProductComboTemp { Writer_Id = writerId, Parent_Id = product_Id };
                sqls.Add(pcTempMgr.TempMoveCombo(proComboTemp));
                sqls.Add(pcTempMgr.TempDelete(proComboTemp));
                string selPrice = priceMasterTempMgr.SelectChild(priceMasterTemp);
                string priceMaster = priceMasterTempMgr.Move2PriceMasterByMasterId();
                ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                string itemPrice = itemPriceTempMgr.Move2ItemPrice();
                sqls.Add(itemPriceTempMgr.Delete(product_Id, combo_type, writerId));
                return _productDao.TempMove2Pro(movePro, "", "", "", selPrice, priceMaster, itemPrice, sqls);
            }
        }

        public bool ProductMigration(Product pro, List<PriceMaster> priceMasters, List<ProductItem> items, List<List<ItemPrice>> itemPrices, ArrayList sqls, ArrayList specs)
        {
            string proSql = _productDao.Save(pro);

            ArrayList prices = new ArrayList();
            if (priceMasters != null)
            {
                PriceMasterMgr priceMasterMgr = new PriceMasterMgr("");
                foreach (var item in priceMasters)
                {
                    prices.Add(priceMasterMgr.SaveNoProId(item));
                }
            }

            ArrayList proItems = new ArrayList();
            if (items != null)
            {
                ProductItemMgr productItemMgr = new ProductItemMgr("");
                foreach (var item in items)
                {
                    proItems.Add(productItemMgr.SaveSql(item));
                }
            }

            ArrayList itemPrice = new ArrayList();
            if (itemPrices != null)
            {
                ArrayList temp;
                ItemPriceMgr itemPriceMgr = new ItemPriceMgr("");
                foreach (var item in itemPrices)
                {
                    temp = new ArrayList();
                    foreach (var p in item)
                    {
                        temp.Add(itemPriceMgr.SaveNoItemId(p));
                    }
                    itemPrice.Add(temp);
                }
            }



            return _productDao.ProductMigration(proSql, prices, proItems, itemPrice, sqls, specs);
        }

        public List<Model.Custom.QueryandVerifyCustom> QueryandVerify(Model.Query.QueryVerifyCondition qcCon, ref int total)
        {
            return _productDao.QueryandVerify(qcCon, ref total);
        }


        public List<Model.Custom.QueryandVerifyCustom> QueryByProSite(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            return _productDao.QueryByProSite(query, out totalCount);
        }

        public List<Model.Custom.QueryandVerifyCustom> QueryByProSite(string priceMasterId)
        {
            List<Model.Custom.QueryandVerifyCustom> result = _productDao.QueryByProSite(priceMasterId);
            result.ForEach(p =>
            {
                if (p.prepaid == 1)
                {

                    p.event_cost = p.cost;
                }

            });
            return result;
        }

        public List<Model.Custom.QueryandVerifyCustom> QueryForStation(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            return _productDao.QueryForStation(query, out  totalCount);
        }


        public List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
        {
            return _productDao.verifyWaitQuery(qcCon, out totalCount);
        }
        public Model.Custom.ProductDetailsCustom ProductDetail(Model.Product query)
        {
            return _productDao.ProductDetail(query);
        }
        public int QueryClassId(int pid)
        {
            return _productDao.QueryClassId(pid);
        }
        public List<PriceMasterCustom> Query(Model.Query.QueryVerifyCondition query)
        {
            return _productDao.Query(query);
        }

        public List<Model.Custom.QueryandVerifyCustom> GetProductInfoByID(string productID)
        {
            var result = _productDao.GetProductInfoByID(productID);
            result.ForEach(p =>
            {
                if (p.prepaid == 1)
                {
                    p.event_cost = p.cost;
                }
            });
            return result;
        }

        public List<Model.Custom.ProductDetailsCustom> GetAllProList(Model.Query.ProductQuery query, out int totalCount)
        {
            try
            {
                return _productDao.GetAllProList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetAllProList-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據查詢條件匯出對應欄位
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="columns">對應欄位</param>
        /// <returns></returns>
        public MemoryStream ExportProductToExcel(Model.Query.QueryVerifyCondition query, int exportFlag, string cols, string fileName)
        {
            XDocument xml = XDocument.Load(fileName);//加载xml 
            Dictionary<string, string> columns = xml.Elements().Elements().ToDictionary(p => p.Attribute("key").Value, p => p.Value);//將xml轉換成Dictionary
            query.IsPage = false;
            string price_master_id = "";
            switch (exportFlag)
            {
                case 1://庫存資料匯出(單一商品)
                    List<ProductItemCustom> productItems = _productDao.GetStockInfo(query);
                    return ExcelHelperXhf.ExportExcel(productItems, columns);//break;
                case 2://商品資料匯出
                case 3://商品價格匯出:權限控管
                    int total = 0;
                    List<QueryandVerifyCustom> items = _productDao.QueryByProSite(query, out total);
                    List<string> authority = exportFlag == 3 ? cols.Split(',').ToList() : null;
                    return ExcelHelperXhf.ExportExcel(items, columns, authority);
                case 4:///子商品價格信息匯出
                    int sum = 0;
                    List<QueryandVerifyCustom> product = _productDao.QueryByProSite(query,out sum);///獲得根據條件查詢到的相關信息

                    foreach (var p in product)
                    {
                        price_master_id += p.price_master_id + ",";
                    }

                    if (price_master_id == "")///如果該字符串無值
                    {
                        return null;//return
                    }
                    price_master_id = price_master_id.Remove(price_master_id.Length - 1, 1);//否則刪去最後一個,
                    IPriceMasterImplMgr _priceMgr = new PriceMasterMgr(connectionStr);
                    List<PriceMasterCustom> listPirce = _priceMgr.GetExcelItemIdInfo(price_master_id);
                    IParametersrcImplDao _paramerDao = new ParametersrcDao(connectionStr);
                    List<Parametersrc> listParameter = _paramerDao.QueryParametersrcByTypes("price_status");
                    foreach (PriceMasterCustom p in listPirce)
                    {
                        var listTemp = listParameter.Find(m => m.ParameterCode == p.price_status.ToString());
                        if(listTemp!=null)
                        {
                            p.price_status_str = listTemp.parameterName;
                        }
                    }
                   
                    return ExcelHelperXhf.ExportExcel(listPirce, columns);
                default:
                    throw new Exception("unaccepted exportFlag!!!");
            }
        }

        #region 與供應商商品相關
        /// <summary>
        /// 獲取供應商商品
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>與供應商相關的商品列表</returns>
        public List<VenderProductListCustom> GetVendorProduct(VenderProductList query, out int totalCount)
        {
            return _productDao.GetVendorProduct(query, out totalCount);
        }
        public Model.Custom.ProductDetailsCustom VendorProductDetail(ProductTemp query)
        {
            try
            {
                return _productDao.VendorProductDetail(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr-->VendorProductDetails-->" + ex.Message, ex);
            }
        }
        public List<Product> VendorQuery(ProductTemp query)
        {
            return _productDao.VendorQuery(query);
        }
        public bool Vendor_Delete(string product_Id)
        {// producd_id uint change string 臨時表刪除
            //20140905  暫時先刪除主表信息 確定之後再刪除關聯信息
            ArrayList delList = new ArrayList();
            //ProductComboMgr proComboMgr = new ProductComboMgr("");
            //delList.Add(proComboMgr.Delete(Convert.ToInt32(product_Id)));
            //ItemPriceMgr itemPriceMgr = new ItemPriceMgr("");
            //delList.Add(itemPriceMgr.DeleteByProductId(Convert.ToInt32(product_Id)));
            //PriceMasterMgr priceMaster = new PriceMasterMgr("");
            //delList.Add(priceMaster.DeleteByProductId(Convert.ToInt32(product_Id)));
            //ProductTagSetMgr proTagSetMgr = new ProductTagSetMgr("");
            //delList.Add(proTagSetMgr.Delete(new ProductTagSet { product_id = product_Id }));
            //ProductNoticeSetMgr proNoticeSetMgr = new ProductNoticeSetMgr("");
            //delList.Add(proNoticeSetMgr.Delete(new ProductNoticeSet { product_id = product_Id }));
            //ProductPictureMgr proPicMgr = new ProductPictureMgr("");
            //delList.Add(proPicMgr.Delete(Convert.ToInt32(product_Id)));
            //ProductSpecMgr proSpecMgr = new ProductSpecMgr("");
            //delList.Add(proSpecMgr.Delete(product_Id));
            //ProductCategorySetMgr proCategorySetMgr = new ProductCategorySetMgr("");
            //delList.Add(proCategorySetMgr.Delete(new ProductCategorySet { Product_Id = product_Id }));
            //ProductItemMgr proItemMgr = new ProductItemMgr("");
            //delList.Add(proItemMgr.Delete(new ProductItem { Product_Id = product_Id }));
            //ProductStatusHistoryMgr proStatusHistoryMgr = new ProductStatusHistoryMgr("");
            //delList.Add(proStatusHistoryMgr.Delete(new ProductStatusHistory { product_id = product_Id }));

            delList.Add(_productDao.Vendor_Delete(product_Id));

            MySqlDao mySqlDao = new MySqlDao(connectionStr);
            return mySqlDao.ExcuteSqls(delList);
        }
        public int Vendor_TempMove2Pro(int writerId, int combo_type, string product_Id, ProductTemp pt)
        {
            ArrayList sqls = new ArrayList();
            int product_id = 0;
            ProductTempMgr proTempMgr = new ProductTempMgr("");
            ProductTemp proTemp = new ProductTemp { Combo_Type = combo_type, Product_Id = product_Id, Create_Channel = 2 };//1:後台管理者(manage_user) edit by xiagnwang0413w 2014/08/09
            string movePro = proTempMgr.Vendor_MoveProduct(proTemp);
            sqls.Add(proTempMgr.Vendor_Delete(proTemp));

            ProductNoticeSetTempMgr proNoticeSetTempMgr = new ProductNoticeSetTempMgr("");
            ProductNoticeSetTemp proNoticeSetTemp = new ProductNoticeSetTemp { Combo_Type = combo_type, product_id = product_Id };
            sqls.Add(proNoticeSetTempMgr.Vendor_MoveNotice(proNoticeSetTemp));
            sqls.Add(proNoticeSetTempMgr.Vendor_Delete(proNoticeSetTemp));

            ProductTagSetTempMgr proTagSetTempMgr = new ProductTagSetTempMgr("");
            ProductTagSetTemp proTagSetTemp = new ProductTagSetTemp { Combo_Type = combo_type, product_id = product_Id };
            sqls.Add(proTagSetTempMgr.Vendor_MoveTag(proTagSetTemp));
            sqls.Add(proTagSetTempMgr.Vendor_Delete(proTagSetTemp));

            ProductPictureTempImplMgr proPicTempMgr = new ProductPictureTempImplMgr("");
            ProductPictureTemp proPictureTemp = new ProductPictureTemp { combo_type = combo_type, product_id = product_Id };
            sqls.Add(proPicTempMgr.Vendor_MoveToProductPicture(proPictureTemp));
            sqls.Add(proPicTempMgr.Vendor_Delete(proPictureTemp));

            ProductCategorySetTempMgr proCateSetTempMgr = new ProductCategorySetTempMgr("");
            ProductCategorySetTemp proCategorySetTemp = new ProductCategorySetTemp { Combo_Type = combo_type, Product_Id = product_Id.ToString() };
            sqls.Add(proCateSetTempMgr.Vendor_TempMoveCategory(proCategorySetTemp));
            sqls.Add(proCateSetTempMgr.Vendor_TempDelete(proCategorySetTemp));

            //product_status_history.type  1,申請審核 2,核可 3,駁回 4,下架 5,新建商品 6,上架 7,系統移轉建立 8,取消送審       parametertype='verify_operate_type'
            //product_status_history.product_status 0,新建立商品 1,申請審核 2,審核通過 5,上架 6,下架 20,供應商新建商品       parametertype='product_status'
            ProductStatusHistoryMgr proStatusHistoryMgr = new ProductStatusHistoryMgr("");
            sqls.Add(proStatusHistoryMgr.SaveNoProductId(new ProductStatusHistory { product_status = 20, user_id = Convert.ToUInt32(pt.Writer_Id), type = 5 }));//供應商新建立商品
            sqls.Add(proStatusHistoryMgr.SaveNoProductId(new ProductStatusHistory { product_status = 0, user_id = Convert.ToUInt32(writerId), type = 2 }));          //管理員核可

            ItemPriceTempMgr itemTempPriceMgr = new ItemPriceTempMgr("");
            sqls.Add(itemTempPriceMgr.Vendor_Delete(product_Id, combo_type, 0));

            PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
            PriceMasterTemp priceMasterTemp = new PriceMasterTemp { product_id = product_Id, combo_type = combo_type };
            sqls.Add(priceMasterTempMgr.Vendor_Delete(priceMasterTemp));

            //判斷是單一商品還是組合商品
            if (combo_type == 1)
            {//單一商品
                IProductItemImplDao piDao = new ProductItemDao(connectionStr);
                ProductItemTempMgr proItemTempMgr = new ProductItemTempMgr("");
                ProductItemTemp proItemTemp = new ProductItemTemp { Product_Id = product_Id };
                string selItem = proItemTempMgr.VendorQuerySql(proItemTemp);
                string moveItem = proItemTempMgr.VendorMoveProductItem(proItemTemp);//方法修改了writerId
                sqls.Add(proItemTempMgr.DeleteVendorSql(proItemTemp));

                ProductSpecTempMgr proSpecTempMgr = new ProductSpecTempMgr("");
                ProductSpecTemp proSpecTemp = new ProductSpecTemp { product_id = product_Id };
                sqls.Add(proSpecTempMgr.VendorTempMoveSpec(proSpecTemp));
                sqls.Add(proSpecTempMgr.VendorTempDelete(proSpecTemp));

                string priceMaster = priceMasterTempMgr.VendorMove2PriceMaster(priceMasterTemp);

                ItemPriceMgr itemPriceMgr = new ItemPriceMgr("");
                string itemPrice = itemPriceMgr.SaveFromItem(pt.Writer_Id, pt.Product_Id);
                // add 處理price_mater_temp and product_combo_temp
                sqls.Add(proTempMgr.VendorEditCM(proTemp));

                product_id = _productDao.TempMove2Pro(movePro, "", moveItem, "", selItem, priceMaster, itemPrice, sqls);
                if (product_id > 0)
                {
                    piDao.UpdateErpId(product_id.ToString());
                }
                return product_id;
            }
            else
            {//組合商品
                ProductComboTempMgr pcTempMgr = new ProductComboTempMgr("");
                ProductComboTemp proComboTemp = new ProductComboTemp { Parent_Id = product_Id };
                sqls.Add(pcTempMgr.Vendor_TempMoveCombo(proComboTemp));
                sqls.Add(pcTempMgr.TempDeleteByVendor(proComboTemp));
                string selPrice = priceMasterTempMgr.SelectChild(priceMasterTemp);
                string priceMaster = priceMasterTempMgr.VendorMove2PriceMaster(priceMasterTemp);

                ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                string itemPrice = itemPriceTempMgr.VendorMove2ItemPrice();
                sqls.Add(itemPriceTempMgr.Vendor_Delete(product_Id, combo_type, 0));

                return _productDao.TempMove2Pro(movePro, "", "", "", selPrice, priceMaster, itemPrice, sqls);
            }
            //記錄的sql語句
            //insert into product(product_id,brand_id,product_vendor_code,product_name,product_price_list,product_spec,spec_title_1,spec_title_2,product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,user_id,show_listprice,expect_msg,create_channel ) select 15164 as product_id,brand_id,product_vendor_code,product_name,product_price_list,product_spec,spec_title_1,spec_title_2,product_freight_set,product_buy_limit,'0' as product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,1411033321 as product_createdate,product_updatedate,product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,writer_id,show_listprice,expect_msg,create_channel from product_temp where 1=1  and combo_type=2 and create_channel=2 and product_id='T578';
            //select price_master_id,product_id,child_id from price_master_temp where 1=1 and combo_type=2 and product_id='T578';
            //insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`,`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`) 
            //select 15164 as product_id,site_id,user_level,user_id,product_name,accumulated_bonus,bonus_percent,default_bonus_percent,same_price,event_start,event_end,price_status,price,event_price,15164 as child_id,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end from price_master_temp where 1=1 and combo_type=2 and product_id='T578';select @@identity;
            //insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`) select 5699 as price_master_id,item_id,item_money,item_cost,event_money,event_cost from item_price_temp where price_master_id=1751
            //set sql_safe_updates = 0;update price_master set child_id=15164 where price_master_id=5699; set sql_safe_updates = 1;

            //set sql_safe_updates = 0;delete from product_temp where product_id='T578';set sql_safe_updates = 1
            //insert into product_notice_set(product_id,notice_id) select 15164 as product_id,notice_id from product_notice_set_temp where 1=1  and combo_type = 2 and product_id='T578';
            //set sql_safe_updates=0;delete from product_notice_set_temp where product_id='T578';set sql_safe_updates=1;
            //insert into product_tag_set(`product_id`,`tag_id`) select 15164 as product_id,tag_id from product_tag_set_temp where 1=1 and combo_type=2 and product_id='T578';
            //set sql_safe_updates=0;delete from product_tag_set_temp where product_id='T578';set sql_safe_updates=1;
            //insert into product_picture(product_id,image_filename,image_sort,image_state,image_createdate) select 15164 as product_id,image_filename,image_sort,image_state,image_createdate from product_picture_temp where 1=1 and product_id='T578' and combo_type=2;
            //set sql_safe_updates=0; delete from product_picture_temp where  product_id='T578';set sql_safe_updates = 1;
            //insert into product_category_set(product_id,category_id,brand_id) select 15164 as product_id,category_id,brand_id from product_category_set_temp where 1=1 and product_id='T578' and combo_type = 2
            //set sql_safe_updates = 0; delete from product_category_set_temp where 1=1 and combo_type = 2 and product_id='T578';set sql_safe_updates = 1;
            //set sql_safe_updates = 0; insert into product_status_history (`product_id`,`user_id`,`create_time`,`type`,`product_status`,`remark`) values (15164,0,now(),5,20,'');set sql_safe_updates = 1;
            //set sql_safe_updates = 0; insert into product_status_history (`product_id`,`user_id`,`create_time`,`type`,`product_status`,`remark`) values (15164,122,now(),2,0,'');set sql_safe_updates = 1;
            //set sql_safe_updates=0;delete item_price_temp from price_master_temp left join item_price_temp on item_price_temp.price_master_id=price_master_temp.price_master_id where 1=1 and price_master_temp.product_id='T578' and price_master_temp.combo_type=2 ;set sql_safe_updates=1;
            //set sql_safe_updates = 0;delete from price_master_temp where 1=1 and product_id='T578' and combo_type=2;set sql_safe_updates = 1;
            //insert into product_combo(`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`) select 15164 as parent_id,child_id,s_must_buy,g_must_buy,pile_id,buy_limit from product_combo_temp where 1=1  and parent_id='T578';
            //set sql_safe_updates = 0; delete from product_combo_temp where writer_id = 0 and parent_id='T578'; set sql_safe_updates= 1;
            //set sql_safe_updates=0;delete item_price_temp from price_master_temp left join item_price_temp on item_price_temp.price_master_id=price_master_temp.price_master_id where 1=1 and price_master_temp.product_id='T578' and price_master_temp.combo_type=2 ;set sql_safe_updates=1;

            //SELECT * from product where product_id='15164'
        }
        #endregion


        public int Yesornoexist(int i, int j)
        {
            try
            {
                return _productDao.Yesornoexist(i, j);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Yesornoexist-->" + ex.Message, ex);
            }
        }

        public int Updateproductcategoryset(string condition)
        {
            try
            {
                return _productDao.Updateproductcategoryset(condition);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Updateproductcategoryset-->" + ex.Message, ex);
            }
        }


        public System.Data.DataTable Updownerrormessage(ProductCategory pc)
        {
            try
            {
                return _productDao.Updownerrormessage(pc);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Updownerrormessage-->" + ex.Message, ex);
            }
        }


        public int Yesornoexistproduct(int i)
        {
            try
            {
                return _productDao.Yesornoexistproduct(i);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Yesornoexistproduct-->" + ex.Message, ex);
            }
        }

        public int Yesornoexistproductcategory(int i)
        {
            try
            {
                return _productDao.Yesornoexistproductcategory(i);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Yesornoexistproductcategory-->" + ex.Message, ex);
            }
        }

        public string GetNameForID(int prodID, int classID, int brandID, int isHidCombo = 0)
        {
            try
            {
                DataTable _dt = _productDao.GetNameForID(prodID, classID, brandID);
                if (_dt.Rows.Count > 0)
                {
                    if (isHidCombo == 1)//隱藏組合商品，只顯示單一商品
                    {
                        if (_dt.Rows[0]["combination"].ToString() != "1")
                        {
                            return "不可選擇組合商品！";
                        }
                        else
                        {
                            return _dt.Rows[0]["product_name"].ToString();
                        }

                    }
                    else
                    {
                        return _dt.Rows[0]["product_name"].ToString();
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetNameForID-->" + ex.Message, ex);
            }
        }



        //add by wwei0216w 2015/1/27
        /// <summary>
        /// 獲得Product.SaleStatus狀態以及相關的列
        /// </summary>
        /// <param name="p">查詢條件</param>
        /// <returns>List<ProductCustom>集合</returns>
        public void UpdateSaleStatusBatch()
        {
            try
            {
                Int64 dateTime = CommonFunction.GetPHPTime(DateTime.Now.ToString());
                MySqlDao sqldao = new MySqlDao(connectionStr);
                ArrayList list = new ArrayList();
                list.Add(_productDao.UpdateSaleStatusBatch(dateTime));
                sqldao.ExcuteSqls(list);
            }
            catch (Exception ex)
            {
                throw new Exception("productMgr-->UpdateSaleStatusBatch" + ex.Message, ex);
            }
        }

        public bool UpdateSaleStatusByCondition(Product p)
        {
            try
            {
                Int64 dateTime = CommonFunction.GetPHPTime(DateTime.Now.ToString());
                MySqlDao sqldao = new MySqlDao(connectionStr);
                ArrayList list = new ArrayList();
                string str = _productDao.UpdateSaleStatusByCondition(dateTime, p);
                if (str != "")
                {
                    list.Add(str);
                    return sqldao.ExcuteSqls(list);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("productMgr-->UpdateSaleStatusByCondition" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/1/28
        /// <summary>
        /// 查詢商品的Status信息
        /// </summary>
        /// <param name="p">查詢條件</param>
        /// <returns>List<ProductCustom>集合</returns>
        public Product UpdateSaleStatus(uint porductId)
        {
            if (porductId == 0) return null;
            List<Product> prods;
            List<ProductItem> prodItems;
            PriceMaster gigaPriceMaster = new PriceMaster();
            Int64 dateTime = Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()); //獲得當前時間戳
            ITableHistoryImplMgr _tableHistoryMgr = new TableHistoryMgr(connectionStr);
            ProductCustom product = _productDao.QueryProductInfo(porductId, out prods, out prodItems, out gigaPriceMaster);//查找單一商,子項庫存是否有為0
            if (gigaPriceMaster == null)
            {
                gigaPriceMaster = new PriceMaster();
            };
            gigaPriceMaster.product_id = porductId;
            if (product == null) return product;
            try
            {
                if (product.Vendor_Status == 2) //供應商下架
                {
                    product.Sale_Status = 24;
                }
                else if (product.Brand_Status == 2) //品牌下架
                {
                    product.Sale_Status = 25;
                }
                else if (product.Product_Status != 5) //商品未上架
                {
                    product.Sale_Status = 21;
                }
                else if (product.Product_Start > dateTime || dateTime > product.Product_End) //上架時間未滿足
                {
                    product.Sale_Status = 22;
                }
                else if (product.Price_Status != 1 || EstimatePrice(gigaPriceMaster)) //價格未審核,或價格==0
                {
                    product.Sale_Status = 23;
                }
                else if (product.Shortage == 1) //商品停賣
                {
                    product.Sale_Status = 11;
                }
                else if (product.Ignore_Stock == 0 && prodItems.Count > 0) //庫存不足
                {
                    product.Sale_Status = 12;
                }
                else
                {
                    if (product.Combination != 1 && prods != null) //非單一商品
                    {

                        List<Product> li = prods.FindAll(m => m.Sale_Status == 11); //查詢該組合商品的子商品中是否有販售狀態等於停賣(11)的
                        List<Product> li2 = prods.FindAll(m => m.Sale_Status == 12);//查詢該組合商品的子商品中是否有販售狀態等於庫存不足(12)的
                        if (li.Count > 0) //子商品停賣(組合商品用)
                        {
                            product.Sale_Status = 13;
                        }
                        else if (li2.Count > 0)//子商品庫存不足(組合商品用)
                        {
                            product.Sale_Status = 14;
                        }
                    }
                    else
                    {
                        product.Sale_Status = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr-->QuerySaleStatus" + ex.Message, ex);
            }

            return product;
        }

        /// <summary>
        /// 批量更新狀態
        /// </summary>
        /// <param name="p">更新條件</param>
        /// <returns></returns>
        //public bool UpdateSaleStatusByBatch(Product p)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    try
        //    {
        //        p.Vendor_Id = 2;
        //        List<Product> list = _productDao.Query(new Product { Vendor_Id=p.Vendor_Id});
        //        List<Product> updateList = new List<Product>();
        //        ArrayList listSql = new ArrayList();
        //        foreach (Product _p in list)
        //        {
        //            updateList.Add(UpdateSaleStatus(_p.Product_Id));
        //            listSql.Add(UpdateColumn(_p, "sale_status"));
        //        }
        //        MySqlDao _sqlDao = new MySqlDao(connectionStr);
        //        return _sqlDao.ExcuteSqls(listSql);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ProductMgr-->UpdateSaleStatusByBatch" + ex.Message,ex);
        //    }
        //}

        //add by wwei0216w 2015/1/28
        /// <summary>
        /// 更新product表中的某一個具體列的值
        /// </summary>
        /// <returns>sql語句</returns>
        public string UpdateColumn(Product p, string columnName)
        {
            try
            {
                return _productDao.UpdateColumn(p, columnName);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr-->UpdateColumn" + ex.Message, ex);
            }
        }


        public DataTable GetVendorProductList(ProductQuery query, out int totalCount)
        {
            try
            {
                return _productDao.GetVendorProductList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.GetNewPromoPresentList-->" + ex.Message, ex);
            }
        }

        ///add by mengjuan0826j
        /// <summary>
        ///獲取商品屬於食品館還是用品館 屬性
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns></returns>
        public Product QueryClassify(uint product_id)
        {
            try
            {
                return _productDao.QueryClassify(product_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr-->QueryClassify" + ex.Message, ex);
            }
        }


        public DataTable GetVendorProductSpec(ProductQuery query, out int totalCount)
        {
            try
            {
                return _productDao.GetVendorProductSpec(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.GetVendorProductSpec-->" + ex.Message, ex);
            }
        }





        public bool UpdateStock(int newstock, int oldstock, int item_id, int vendor)
        {
            try
            {
                return _mysqlDao.ExcuteSqls(new ArrayList { _productDao.UpdateStock(newstock, oldstock, item_id, vendor) });
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.UpdateStock-->" + ex.Message, ex);
            }
        }


        //add by wwei0216 2015/3/4 用來判斷商品的價格狀態是否為0
        public bool EstimatePrice(PriceMaster p)
        {
            if (p == null) return false;
            Int64 now = Common.CommonFunction.GetPHPTime(DateTime.Now.ToString());
            if (p.same_price == 1)
            {
                if (p.price != 0)
                {
                    //如果當前時間小於開始時間,或者當前時間大於結束時間,證明不在活動區間內
                    if (now < p.event_start || now > p.event_end)
                    {
                        return false;
                    }
                    else
                    {
                        if (p.event_price != 0)
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                IItemPriceImplMgr _piMgr = new ItemPriceMgr(connectionStr);
                List<ItemPrice> ipItem = _piMgr.GetItem(Convert.ToInt32(p.product_id));
                //查找ipItem中item_money不等於0的
                if (ipItem.FindAll(m => m.item_money != 0).Count > 0)
                {
                    //如果當前時間小於開始時間,或者當前時間大於結束時間,證明不在活動區間內
                    if (now < p.event_start || now > p.event_end)
                    {
                        return false;
                    }
                    else
                    {
                        if (ipItem.FindAll(m => m.event_money != 0).Count > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public int GetDefaultArriveDays(Product prod)
        {
            return _productDao.GetDefaultArriveDays(prod);
        }
        public List<Product> GetProductName(Product p)
        {
            try
            {
                return _productDao.GetProductName(p);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.GetProductName-->" + ex.Message, ex);
            }
        }


        public bool UpdateStatus(int spec_id, int spce_status)
        {
            try
            {
                return _mysqlDao.ExcuteSqls(new ArrayList { _productDao.UpdateStatus(spec_id, spce_status) });
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.UpdateStatus-->" + ex.Message, ex);
            }
        }
        #region 商品詳情說明
        public DataTable GetProductList(ProductQuery p, out int totalCount)
        {
            try
            {
                return _productDao.GetProductList(p, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.GetProductList-->" + ex.Message, ex);
            }
        }
        public int UpdateProductDeatail(Product p)
        {
            try
            {
                return _productDao.UpdateProductDeatail(p);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.UpdateProductDeatail-->" + ex.Message, ex);
            }
        }
        public DataTable GetVendor(Vendor v)
        {
            try
            {
                return _productDao.GetVendor(v);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.GetVendor-->" + ex.Message, ex);
            }
        }
        public DataTable GetProductDetialText(ProductQuery p, string id, out string notFind)
        {
            string[] num = id.Split(',');
            DataTable _dt = new DataTable();
            uint pro_id = 0;
            notFind = string.Empty;
            DataTable _result = new DataTable();
            _result.Columns.Add("product_detail_text");
            try
            {
                foreach (var item in num)
                {
                    if (uint.TryParse(item.Trim(), out pro_id))
                    {
                        p.Product_Id = pro_id;
                        _dt = _productDao.GetProductDetialText(p);
                        if (_dt != null && _dt.Rows.Count != 0)
                        {
                            if (!string.IsNullOrEmpty(_dt.Rows[0][0].ToString()))
                            {
                                DataRow _dr = _result.NewRow(); ;
                                _dr[0] = _dt.Rows[0][0];
                                _result.Rows.Add(_dr);
                            }
                            else
                            {
                                notFind = item;
                                return null;
                            }
                        }
                        else
                        {
                            notFind = item;
                            return null;
                        }

                    }
                    continue;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("ProductMgr.GetProductDetialText-->" + ex.Message, ex);
            }

            return _result;
        }
        #endregion

        /// <summary>
        /// 根據商品編號更新商品補貨中停止販售狀態
        /// </summary>
        /// <param name="product_id">商品編號</param>
        /// <param name="shortage">販售狀態</param>
        /// <returns>更新結果</returns>
        public int UpdateShortage(int product_id, int shortage)
        {
            try
            {
                return productDao.UpdateShortage(product_id, shortage);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr.UpdateShortage-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 設置商品失格(一旦設置,無法還原)
        /// </summary>
        /// <param name="product_id">需要設置商品失格的id</param>
        /// <returns>成功 OR 失敗</returns>
        public bool UpdateOff_Grade(uint product_id, int off_grade)
        {
            try
            {
                return _productDao.UpdateOff_Grade(product_id, off_grade);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr-->UpdateOff_Grade" + ex.Message, ex);
            }
        }


        public int GetProductType(Product query)
        {
            try
            {
                return _productDao.GetProductType(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductMgr-->GetProductType" + ex.Message, ex);
            }
        }
    }
}
