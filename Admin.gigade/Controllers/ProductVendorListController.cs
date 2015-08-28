#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductVendorListController.cs
* 摘 要：
* 供應商上下架審核列表Controller和供應商商品審核列表Controller
* 当前版本：v1.0
* 作 者： mengjuan0826j
* 完成日期：
* 修改歷史：
*         v1.1修改日期：2014/8/18 
*         v1.1修改人員：shuangshuang0420jj 
*         v1.1修改内容：添加供應商商品審核列表
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using Newtonsoft.Json;
using System.Configuration;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;
using System.IO;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class ProductVendorListController : Controller
    {
        //
        // GET: /ProductVendorList/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//add by hufeng0813w xml的文件
        private IProductTempImplMgr _prodTempMgr;
        //private IProductStatusApplyImplMgr _applyMgr;
        //private IProductStatusHistoryImplMgr _statusHistoryMgr;
        private IProductImplMgr _prodMgr;
        private IPriceMasterImplMgr _pMaster;
        //private ITableHistoryImplMgr _tableHistoryMgr;
        //private IFunctionImplMgr _functionMgr;
        //private IPriceUpdateApplyHistoryImplMgr _pHMgr;
        private IProductComboTempImplMgr _pctMgr;
        //20140912 by jialei
        private IProductTagImplMgr _productTagMgr;
        private IProductTagSetImplMgr _productTagSetMgr;
        private IProductTagSetTempImplMgr _productTagSetTempMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetImplMgr _productNoticeSetMgr;
        private IProductNoticeSetTempImplMgr _productNoticeSetTempMgr;
        private ISiteConfigImplMgr _siteConfigMgr;
        private ProductCategoryMgr _procateMgr;
        private IProductCategorySetImplMgr _categorySetMgr;
        private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        private IPriceMasterTempImplMgr _priceMasterTempMgr;
        private int COMBO_TYPE = 1;//單一商品
        private int COMBO_TYPE_2 = 2;//組合商品
        private ParameterMgr paraMgr;
        private IProductItemTempImplMgr _productItemTempMgr;
        private IProductComboTempImplMgr _combTempMgr;
        private IProductSpecTempImplMgr _specTempMgr;
        private IProductSpecImplMgr _specMgr;
        private IProductItemImplMgr _productItemMgr;
        private static IProdVdReqImplMgr _prodVdReq;
        private IProductPictureImplMgr _pPicMgr;
        private IProductPictureTempImplMgr _pPicTempMgr;

        #region Pic Information
        string prodPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prodPath);
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.specPath);
        string spec100Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.spec100Path);
        string spec280Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.spec280Path);

        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);
        string prod50Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod50Path);
        string tagPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_tagPath);
        string noticePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_noticePath);
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        string descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descPath);

        #endregion
        #region 視圖
        public ActionResult ProdVdReq()
        {
            return View();
        }
        #region 商品信息詳情視圖
        public ActionResult ProductDetails()
        {
            return View();
        }
        #endregion
        #endregion
        #region 供應商商品審核列表視圖+ ActionResult VendorVerifyList()
        /// <summary>
        /// 供應商商品審核列表視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult VendorVerifyList()
        {
            return View();
        }
        #endregion

        #region 待審核供應商商品列表查詢+HttpResponseBase waitVerifyQuery()
        /// <summary>
        /// 待審核商品列表查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase waitVerifyQuery()
        {
            //todo:供應商商品審核列表需要修改
            List<BLL.gigade.Model.Custom.VenderProductListCustom> resultList = new List<BLL.gigade.Model.Custom.VenderProductListCustom>();
            string result = "{success:false}";
            try
            {
                uint brand_id;
                uint category_id;
                int combination;
                int prev_status;
                uint.TryParse(Request.Params["brand_id"] ?? "0", out brand_id);
                uint.TryParse(Request.Params["category_id"] ?? "0", out category_id);
                int.TryParse(Request.Params["combination"] ?? "0", out combination);
                int.TryParse(Request.Params["prev_status"] ?? "0", out prev_status);

                QueryVerifyCondition query = new QueryVerifyCondition()
                {
                    product_status = 1,//待審核商品列表只查詢商品狀態為審請審核的
                    brand_id = brand_id,
                    combination = combination,
                    prev_status = prev_status,
                    date_type = Request.Params["date_type"],
                    name_number = Request.Params["key"],
                    Start = Convert.ToInt32(Request.Form["start"] ?? "0"),
                    Limit = Convert.ToInt32(Request.Form["limit"] ?? "25")
                };
                if (!string.IsNullOrEmpty(Request.Params["cate_id"]))
                {
                    query.cate_id = Request.Params["cate_id"].Trim();
                }
                if (category_id != 2)    //ROOT 表全选
                {
                    query.category_id = category_id;
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    if (query.date_type != "apply_time")            //time 為整型
                    {
                        query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                        query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                    }
                    else
                    {
                        query.time_start = Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy/MM/dd 00:00:00");
                        query.time_end = Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy/MM/dd 23:59:59");
                    }
                }

                _prodTempMgr = new ProductTempMgr(connectionString);
                int totalCount = 0;
                resultList = _prodTempMgr.verifyWaitQuery(query, out totalCount);
                foreach (VenderProductListCustom item in resultList)
                {
                    if (item.product_image != "")
                    {
                        item.product_image = imgServerPath + prod50Path + GetDetailFolder(item.product_image) + item.product_image;
                    }
                    else
                    {
                        item.product_image = defaultImg;
                    }
                }
                result = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(resultList) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            Response.Clear();
            Response.Write(result);
            Response.End();
            return this.Response;
        }

        #endregion


        #region 供應商待審核組合商品子商品狀態判斷
        /// <summary>
        /// 供應商待審核組合商品子商品狀態判斷
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public string ChildStatus(string productid)
        {
            string json = string.Empty;
            string msg = string.Empty;

            try
            {
                bool result = true;

                _pctMgr = new ProductComboTempMgr(connectionString);
                List<ProductTemp> childpro = _pctMgr.QueryChildStatusVendor(new ProductComboTemp { Parent_Id = productid });
                if (childpro.Count != 1)
                {
                    foreach (var item in childpro)
                    {
                        if (item.Product_Id != childpro[0].Product_Id)
                        {
                            if (item.Product_Status != 2 && item.Product_Status != 5 && item.Product_Status != 6)
                            {
                                result = false;
                                msg += "<br/>子商品" + item.Product_Id + "狀態必須是審核通過/上架/下架狀態;";
                                // break;
                            }
                            if (item.Product_Mode != childpro[0].Product_Mode)
                            {//判斷運費
                                result = false;
                                msg += "<br/>子商品" + item.Product_Id + "出貨方式必須與子商品出貨方式一致;";
                                //break;
                            }
                            switch (childpro[0].Product_Freight_Set)
                            {
                                case 1:
                                case 3:
                                    if (item.Product_Freight_Set != 1 && item.Product_Freight_Set != 3)
                                    {
                                        result = false;
                                        msg += "<br/>子商品" + item.Product_Id + "運送方式必須符合父商品運送方式;";
                                        break;
                                    }
                                    break;
                                case 2:
                                case 4:
                                    if (item.Product_Freight_Set != 2 && item.Product_Freight_Set != 4)
                                    {
                                        result = false;
                                        msg += "<br/>子商品" + item.Product_Id + "運送方式必須符合父商品運送方式;";
                                        break;
                                    }
                                    break;
                                case 5:
                                case 6:
                                    if (item.Product_Freight_Set != 5 && item.Product_Freight_Set != 6)
                                    {
                                        result = false;
                                        msg += "<br/>子商品" + item.Product_Id + "運送方式必須符合父商品運送方式;";
                                        break;
                                    }
                                    break;
                                default: result = false; break;
                            }
                        }

                    }
                    if (!result)
                    {
                        json += "編號為" + productid + "組合商品子商品狀態錯誤：" + msg;
                    }
                }
                else
                {
                    json += "編號為" + productid + "子商品錯誤;";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return json;
        }
        #endregion
        #region 供應商待審核商品核可
        /// <summary>
        /// 供應商待審核商品核可(臨時表數據匯入正式表)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase vaiteVerifyPass()
        {
            #region //
            //string resultStr = "{success:false}";
            //bool result = true;
            //try
            //{
            //    Caller _caller = (Session["caller"] as Caller);
            //    _applyMgr = new ProductStatusApplyMgr(connectionString);
            //    _statusHistoryMgr = new ProductStatusHistoryMgr(connectionString);
            //    _prodMgr = new ProductMgr(connectionString);
            //    _pMaster = new PriceMasterMgr(connectionString);
            //    _tableHistoryMgr = new TableHistoryMgr(connectionString);
            //    _prodTempMgr = new ProductTempMgr(connectionString);
            //    _functionMgr = new FunctionMgr(connectionString);

            //    string productIds = Request.Params["prodcutIdStr"];
            //    string[] products = productIds.Split(',');

            //    string function = Request.Params["function"] ?? "";
            //    Function fun = _functionMgr.QueryFunction(function, "/ProductList/VerifyList");
            //    int functionid = fun == null ? 0 : fun.RowId;

            //    HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
            //    string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

            //    foreach (string item in products)
            //    {
            //        ProductTemp queryProdTemp = new ProductTemp()
            //        {
            //            //Temp_Status = 12,
            //            //Create_Channel = 2,
            //            Product_Id = item
            //        };
            //        ProductTemp modelProdTemp = _prodTempMgr.GetProTempByVendor(queryProdTemp).FirstOrDefault();
            //        //新增資料至product，并刪除product_temp中的數據
            //        if (_prodMgr.TempMove2Pro(modelProdTemp.Writer_Id, modelProdTemp.Combo_Type, modelProdTemp.Product_Id) != -1)
            //        {
            //            Product product = _prodMgr.Query(new Product { Product_Id = uint.Parse(item) }).FirstOrDefault();

            //            ArrayList sqls = new ArrayList();
            //            if (_applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) }) != null)
            //            {
            //                batch.batchno = batchNo + product.Product_Id;
            //                //更改商品价格之状态
            //                PriceMaster queryPriceMaster = new PriceMaster();
            //                if (product.Combination != 0 && product.Combination != 1)   //组合商品
            //                {
            //                    queryPriceMaster.child_id = int.Parse(item);
            //                }
            //                else
            //                {
            //                    queryPriceMaster.child_id = 0;
            //                }
            //                queryPriceMaster.product_id = uint.Parse(item);
            //                queryPriceMaster.price_status = 2;       //只更改价格状态为申请审核的商品价格        
            //                List<PriceMaster> listPriceMaster = _pMaster.PriceMasterQuery(queryPriceMaster);
            //                if (listPriceMaster != null && listPriceMaster.Count() > 0)
            //                {
            //                    _pHMgr = new PriceUpdateApplyHistoryMgr(connectionString);
            //                    List<PriceUpdateApplyHistory> pHList = new List<PriceUpdateApplyHistory>();
            //                    foreach (var priceMaster in listPriceMaster)
            //                    {
            //                        ArrayList priceUpdateSqls = new ArrayList();
            //                        priceMaster.price_status = 1;      //价格状态为上架
            //                        priceMaster.apply_id = 0;
            //                        priceUpdateSqls.Add(_pMaster.Update(priceMaster));
            //                        if (!_tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, priceUpdateSqls))
            //                        {
            //                            result = false;
            //                            break;
            //                        }

            //                        //价格异动记录（price_update_apply_history）                            
            //                        PriceUpdateApplyHistory pH = new PriceUpdateApplyHistory();
            //                        pH.apply_id = int.Parse(priceMaster.apply_id.ToString());
            //                        pH.user_id = (Session["caller"] as Caller).user_id;
            //                        pH.price_status = 1;
            //                        pH.type = 1;
            //                        pHList.Add(pH);
            //                    }
            //                    if (!_pHMgr.Save(pHList))
            //                    {
            //                        result = false;
            //                        break;
            //                    }
            //                }

            //                //更改商品之状态
            //                ProductStatusApply queryApply = _applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) });
            //                uint online_mode = queryApply.online_mode;
            //                //申請狀態為審核後立即上架時將上架時間改為當前時間，商品狀態改為上架
            //                if (online_mode == 2)
            //                {
            //                    product.Product_Status = 5;
            //                    product.Product_Start = uint.Parse(BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToLongTimeString()).ToString());
            //                }
            //                else
            //                {
            //                    product.Product_Status = 2;
            //                    //product.Product_Start = online_mode;
            //                }
            //                sqls.Add(_prodMgr.Update(product));

            //                ProductStatusHistory saveOne = new ProductStatusHistory();
            //                saveOne.product_id = product.Product_Id;
            //                saveOne.user_id = product.Brand_Id;//供應商品牌名稱
            //                saveOne.type = 8;           //操作類型(供應商新建商品)
            //                saveOne.product_status = 0;//新建商品
            //                sqls.Add(_statusHistoryMgr.Save(saveOne));         //保存历史记录
            //                ProductStatusHistory saveTwo = new ProductStatusHistory();
            //                saveTwo.product_id = product.Product_Id;
            //                saveTwo.user_id = uint.Parse(_caller.user_id.ToString());//pm名稱
            //                saveTwo.type = 9;           //操作類型(PM核可)
            //                saveTwo.product_status = 0;//新建商品
            //                sqls.Add(_statusHistoryMgr.Save(saveTwo));   //保存历史记录
            //                sqls.Add(_applyMgr.Delete(queryApply));         //刪除審核申請表中的數據

            //                if (!_tableHistoryMgr.SaveHistory<Product>(product, batch, sqls))
            //                {
            //                    result = false;
            //                    break;
            //                }
            //            }
            //            else
            //            {
            //                result = false;
            //                break;
            //            }
            //        }
            //    }

            //    resultStr = "{success:" + result.ToString().ToLower() + "}";
            //}
            //catch (Exception ex)
            //{
            //    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
            //    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
            //    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            //    log.Error(logMessage);
            //}

            //Response.Clear();
            //Response.Write(resultStr);
            //Response.End();
            //return this.Response; 
            #endregion
            //todo:此處有待測試
            string json = string.Empty;
            int writerId = (Session["caller"] as Caller).user_id;
            _prodMgr = new ProductMgr(connectionString);
            _prodTempMgr = new ProductTempMgr(connectionString);
            try
            {
                string strProductIds = Request.Params["prodcutIdStr"];
                string[] productIds = strProductIds.Split(',');
                string msg = string.Empty;
                foreach (var product_id in productIds)
                {
                    try
                    {
                        int productId;
                        ProductTemp productTemp = _prodTempMgr.GetVendorProTemp(new ProductTemp() { Product_Id = product_id });
                        if (productTemp.Combo_Type == 2)
                        {
                            msg = ChildStatus(productTemp.Product_Id);
                            if (!string.IsNullOrEmpty(msg))
                            {
                                continue;
                            }
                            else
                            {
                                productId = _prodMgr.Vendor_TempMove2Pro(writerId, productTemp.Combo_Type, product_id, productTemp);
                            }
                        }
                        else
                        {
                            productId = _prodMgr.Vendor_TempMove2Pro(writerId, productTemp.Combo_Type, product_id, productTemp);
                        }

                        if (productId == -1)
                        {
                            msg += "編號為" + productTemp.Product_Id + "商品審核出錯;";
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                        json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                    }

                }
                if (!string.IsNullOrEmpty(msg))
                {
                    json = "{success:false,msg:\"" + msg + "\"}";
                }
                else
                {
                    json = "{success:true}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }

        #endregion
        #region 供應商待審核商品駁回
        /// <summary>
        /// 供應商待審核商品駁回
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase vaiteVerifyBack()
        {
            bool flag = false;
            string resultStr = "";
            _prodTempMgr = new ProductTempMgr(connectionString);
            try
            {
                string productIds = Request.Params["productIds"];
                //string backReason = Request.Params["backReason"]; //駁回原因
                string[] products = productIds.Split(',');
                foreach (string item in products)
                {
                    ProductTemp query = new ProductTemp();
                    query.Product_Id = item;
                    ProductTemp prodTempModel = _prodTempMgr.GetProTempByVendor(query).FirstOrDefault();
                    if (prodTempModel != null)
                    {
                        prodTempModel.Product_Status = 20;  //將商品狀態修改為新建立完成的商品

                        if (_prodTempMgr.UpdateAchieve(prodTempModel))
                        {
                            flag = true;
                        }
                    }
                }
                resultStr = "{success:" + flag.ToString().ToLower() + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }
        #endregion

        #region 根據圖片名反推文件目錄+string GetDetailFolder(string picName)
        /// <summary>
        /// 根據圖片名反推文件目錄
        /// </summary>
        /// <param name="picName">文件名</param>
        /// <returns>返回文件目錄</returns>
        public string GetDetailFolder(string picName)
        {
            string firthFolder = picName.Substring(0, 2) + "/";
            string secondFolder = picName.Substring(2, 2) + "/";

            return firthFolder + secondFolder;
        }
        #endregion

        #region 供應商商品列表查詢 + HttpResponseBase ProdVdReqListQuery()
        /// <summary>
        /// 供應商商品列表查詢
        /// </summary>
        /// <returns>json數組</returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProdVdReqListQuery()
        {
            string json = string.Empty;
            try
            {
                ProdVdReqQuery query = new ProdVdReqQuery();
                #region 查询条件填充

                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["brand_id"]))
                {
                    query.brand_id = Convert.ToUInt32(Request.Form["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_id"]))
                {
                    query.product_id = int.Parse(Request.Form["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["req_status"]))
                {
                    query.req_status = int.Parse(Request.Form["req_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["time_start"]))
                {
                    query.time_start = Convert.ToDateTime(Convert.ToDateTime(Request.Form["time_start"]).ToString("yyyy/MM/dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Form["time_end"]))
                {
                    query.time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Form["time_end"]).ToString("yyyy/MM/dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(Request.Form["req_type"]))
                {
                    query.req_type = int.Parse(Request.Form["req_type"]);
                }


                #endregion
                _prodVdReq = new ProdVdReqMgr(connectionString);
                int totalCount = 0;
                List<ProdVdReqQuery> pros = _prodVdReq.QueryProdVdReqList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{succes:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(pros, Formatting.Indented, timeConverter) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{succes:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 供應商商品申請上下架駁回操作 +HttpResponseBase ProdVdReqBack()
        /// <summary>
        /// 供應商商品申請上下架駁回操作
        /// </summary>
        /// <returns>json數組</returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProdVdReqBack()
        {
            string resultStr = "{success:true}";
            try
            {
                string rIdAndProdId = Request.Params["rIdAndProdId"];
                string[] rIdAndProdIdS = rIdAndProdId.Split(';');
                string backReason = Request.Params["backReason"];

                foreach (string item in rIdAndProdIdS)
                {
                    var rid = item.Split(',')[0];
                    var product_id = item.Split(',')[1];

                    ProdVdReqQuery prodQuery = new ProdVdReqQuery();
                    prodQuery.rid = int.Parse(rid);
                    prodQuery.product_id = int.Parse(product_id);
                    int totalCount = 0;
                    ProdVdReq prodRquery = _prodVdReq.QueryProdVdReqList(prodQuery, out totalCount).FirstOrDefault();

                    if (prodRquery != null)
                    {
                        //prodQuery.rid = int.Parse(rid);
                        //prodQuery.product_id = int.Parse(product_id);
                        prodRquery.req_status = 3;
                        prodRquery.reply_note = backReason;
                        prodRquery.reply_datetime = DateTime.Now;
                        prodRquery.user_id = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                        if (_prodVdReq.Update(prodRquery) < 0)
                        {
                            resultStr = "{success:false,msg:0}";//返回json數據
                        }
                    }
                    else
                    {
                        resultStr = "{success:false,msg:0}";//返回json數據
                    }
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        #endregion

        #region 供應商商品申請上下架核可操作 + HttpResponseBase ProdVdReqPass()
        /// <summary>
        /// 供應商商品申請上下架核可操作
        /// </summary>
        /// <returns>json數組</returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProdVdReqPass()
        {
            string resultStr = "{success:true}";
            try
            {
                string rIdAndProdId = Request.Params["rIdAndProdId"];
                string[] rIdAndProdIdS = rIdAndProdId.Split(';');

                foreach (string item in rIdAndProdIdS)
                {
                    var rid = item.Split(',')[0];
                    var product_id = item.Split(',')[1];


                    ProdVdReqQuery prodQuery = new ProdVdReqQuery();
                    prodQuery.rid = int.Parse(rid);
                    prodQuery.product_id = int.Parse(product_id);
                    int totalCount = 0;
                    ProdVdReq prodRquery = _prodVdReq.QueryProdVdReqList(prodQuery, out totalCount).FirstOrDefault();
                    _prodMgr = new ProductMgr(connectionString);
                    Product p = _prodMgr.Query(new Product { Product_Id = Convert.ToUInt32(prodRquery.product_id) }).FirstOrDefault();
                    if (p.Product_Status != 5 && prodRquery.req_type == 1)
                    {

                        resultStr = "{success:false,msg:\"" + "請先核可商品狀態,以完成上下架申請" + "\"}";//返回json數據
                        break;
                    }
                    else if (p.Product_Status != 6 && prodRquery.req_type == 2)
                    {
                        resultStr = "{success:false,msg:\"" + "請先核可商品狀態,以完成上下架申請" + "\"}";//返回json數據
                        break;
                    }
                    else
                    {
                        prodRquery.req_status = 2;
                        prodRquery.reply_datetime = DateTime.Now;
                        prodRquery.user_id = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                        if (_prodVdReq.Update(prodRquery) < 0)
                        {
                            resultStr = "{success:false,msg'" + Resources.Product.SAVE_FAIL + "'}";
                        }
                    }
                }
                //resultStr = "{success:true,msg'" + Resources.Product.SAVE_SUCCESS + "'}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{success:false,msg'" + Resources.Product.SAVE_FAIL + "'}";
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        #endregion





        #region 供應商申請審核信息詳情
        #region 商品詳細信息查詢
        [HttpPost]
        public HttpResponseBase ProductDetailsQuery()
        {
            uint productid = 0;
            string json = string.Empty;
            _prodMgr = new ProductMgr(connectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    string product_id = Request.Form["ProductId"];
                    ProductDetailsCustom product = new ProductDetailsCustom();
                    if (uint.TryParse(product_id, out productid))
                    {
                        product = _prodMgr.ProductDetail(new ProductTemp { Product_Id = product_id });
                    }
                    else
                    {
                        product = _prodMgr.VendorProductDetail(new ProductTemp { Product_Id = product_id });
                    }
                    //IE
                    product.page_content_1 = product.page_content_1.Replace("\r\n", "<br/>");
                    product.page_content_2 = product.page_content_2.Replace("\r\n", "<br/>");
                    product.page_content_3 = product.page_content_3.Replace("\r\n", "<br/>");
                    //FireFox
                    product.page_content_1 = product.page_content_1.Replace("\n", "<br/>");
                    product.page_content_2 = product.page_content_2.Replace("\n", "<br/>");
                    product.page_content_3 = product.page_content_3.Replace("\n", "<br/>");
                    json = "{success:true,data:" + JsonConvert.SerializeObject(product) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion
        #region 標籤
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProTag()
        {
            string json = string.Empty;
            try
            {
                StringBuilder strJson = new StringBuilder();
                _productTagMgr = new ProductTagMgr(connectionString);
                List<ProductTag> tags = _productTagMgr.Query(new ProductTag { tag_status = 1 });
                if (tags != null)
                {
                    uint pid = 0;
                    string productID = string.Empty;
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        if (uint.TryParse(Request.Form["ProductId"].ToString(), out pid))
                        {
                            pid = uint.Parse(Request.Form["ProductId"].ToString());
                        }
                        else
                        {
                            productID = Request.Form["ProductId"].ToString();
                        }
                        if (pid != 0)
                        {
                            _productTagSetMgr = new ProductTagSetMgr(connectionString);
                            List<ProductTagSet> tagSets = _productTagSetMgr.Query(new ProductTagSet { product_id = pid });
                            foreach (var item in tags)
                            {
                                if (tagSets.Exists(m => m.tag_id == item.tag_id))
                                {
                                    strJson.AppendFormat("<img  src='{1}' style='margin-right:5px' /></label>", item.tag_id, imgServerPath + tagPath + GetDetailFolder(item.tag_filename) + item.tag_filename);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(productID))
                        {
                            _productTagSetTempMgr = new ProductTagSetTempMgr(connectionString);
                            ProductTagSetTemp ptstQuery = new ProductTagSetTemp();
                            ptstQuery.product_id = productID;
                            List<ProductTagSetTemp> tagTempSets = _productTagSetTempMgr.QueryVendorTagSet(ptstQuery);
                            foreach (var item in tags)
                            {
                                if (tagTempSets.Exists(m => m.tag_id == item.tag_id))
                                {
                                    strJson.AppendFormat("<img  src='{1}' style='margin-right:5px' /></label>", item.tag_id, imgServerPath + tagPath + GetDetailFolder(item.tag_filename) + item.tag_filename);
                                }
                            }
                        }
                        json = strJson.ToString();
                        if (string.IsNullOrEmpty(json))
                        {
                            json = "暫無圖片";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 商品公告
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProNotice()
        {
            string json = string.Empty;
            try
            {
                StringBuilder strJson = new StringBuilder();
                _productNoticeMgr = new ProductNoticeMgr(connectionString);
                List<ProductNotice> notices = _productNoticeMgr.Query(new ProductNotice { notice_status = 1 });
                if (notices != null)
                {
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        uint pid = 0;
                        string prodID = string.Empty;
                        if (uint.TryParse(Request.Form["ProductId"].ToString(), out pid))
                        {
                            pid = uint.Parse(Request.Form["ProductId"]);
                        }
                        else
                        {
                            prodID = Request.Form["ProductId"].ToString();
                        }
                        if (pid != 0)
                        {
                            _productNoticeSetMgr = new ProductNoticeSetMgr(connectionString);
                            List<ProductNoticeSet> noticeSets = _productNoticeSetMgr.Query(new ProductNoticeSet { product_id = pid });
                            foreach (var item in notices)
                            {
                                if (noticeSets.Exists(m => m.notice_id == item.notice_id))
                                {
                                    strJson.AppendFormat("{0}", item.notice_name);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(prodID))
                        {
                            _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connectionString);
                            ProductNoticeSetTemp queryProductNotice = new ProductNoticeSetTemp();
                            queryProductNotice.product_id = prodID;
                            //queryProductNotice.Writer_Id = (Session["caller"] as Caller).user_id;
                            List<ProductNoticeSetTemp> noticeTempSets = _productNoticeSetTempMgr.QueryVendorProdNotice(queryProductNotice);
                            foreach (var item in notices)
                            {
                                if (noticeTempSets.Exists(m => m.notice_id == item.notice_id))
                                {
                                    strJson.AppendFormat("{0}", item.notice_name);
                                }
                            }
                        }
                    }
                    json = strJson.ToString();
                    if (string.IsNullOrEmpty(json))
                    {
                        json = "暫無圖片";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 查詢品類分類
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetProCategory()
        {
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            string resultStr = "";
            try
            {
                _procateMgr = new ProductCategoryMgr(connectionString);
                cateList = _procateMgr.cateQuery(0);
                GetCategoryList(ref cateList);
                resultStr = JsonConvert.SerializeObject(cateList);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #region 遞歸得到分類節點
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(ref List<ProductCategoryCustom> catelist)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                item.id = item.parameterCode;
                List<ProductCategoryCustom> childList = _procateMgr.cateQuery(int.Parse(item.parameterCode));
                item.children = childList;
                if (childList.Count() > 0)
                {
                    GetCategoryList(ref childList);
                }
            }
        }
        #endregion
        #endregion
        #region 查詢前台分類
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetFrontCatagory()
        {
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            string resultStr = "";
            try
            {
                _procateMgr = new ProductCategoryMgr(connectionString);
                categoryList = _procateMgr.QueryAll(new ProductCategory { });
                cateList = getCate(categoryList, "0");
                GetFrontCateList(categoryList, ref cateList);
                resultStr = JsonConvert.SerializeObject(cateList);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #region 遞歸得到分類節點
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetFrontCateList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, item.id.ToString());
                item.children = childList;

                if (childList.Count() > 0)
                {
                    GetFrontCateList(categoryList, ref childList);
                }
            }
        }
        #endregion

        #endregion
        #region 得到所有前台分類 add jialei 複製過來的
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetCatagory(string id = "true")
        {
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            string resultStr = "";
            try
            {
                _procateMgr = new ProductCategoryMgr(connectionString);
                categoryList = _procateMgr.QueryAll(new ProductCategory { });
                cateList = getCate(categoryList, "5");
                List<ProductCategorySetTemp> TempresultList = new List<ProductCategorySetTemp>();
                List<ProductCategorySet> resultList = new List<ProductCategorySet>();
                ProductCategorySetTemp query = new ProductCategorySetTemp();
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id)) //正式表數據
                    {
                        _categorySetMgr = new ProductCategorySetMgr(connectionString);
                        resultList = _categorySetMgr.Query(new ProductCategorySet { Product_Id = uint.Parse(Request.Params["ProductId"]) });
                    }
                    else
                    {
                        query.Product_Id = Request.Params["ProductId"];
                    }
                }
                //query.Writer_Id = (int)vendorModel.vendor_id;
                //query.Combo_Type = COMBO_TYPE;
                _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
                TempresultList = (from c in _categoryTempSetMgr.QueryByVendor(query)
                                  select new
                                  {
                                      Id = c.Id,
                                      Product_Id = c.Product_Id,
                                      Category_Id = c.Category_Id,
                                      Brand_Id = c.Brand_Id
                                  }).ToList().ConvertAll<ProductCategorySetTemp>(m => new ProductCategorySetTemp
                                  {
                                      Id = m.Id,

                                      Product_Id = m.Product_Id,
                                      Category_Id = m.Category_Id,
                                      Brand_Id = m.Brand_Id
                                  });
                GetCategoryList(categoryList, ref cateList, TempresultList, resultList);

                List<ProductCategoryCustom> cateListResult = new List<ProductCategoryCustom>();
                cateListResult = getCate(categoryList, "0");
                cateListResult[0].children = getCate(categoryList, cateListResult[0].id.ToString());
                int cateLen = cateListResult[0].children.Count;
                int i = 0;
                while (cateLen > 0)
                {
                    if (cateListResult[0].children[i].id == "5")
                    {
                        i++;
                    }
                    else
                    {
                        cateListResult[0].children.Remove(cateListResult[0].children[i]);
                    }
                    cateLen--;
                }
                cateListResult[0].children[0].children = cateList;
                resultStr = JsonConvert.SerializeObject(cateListResult);
                resultStr = resultStr.Replace("Checked", "checked");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        public List<ProductCategoryCustom> getCate(List<ProductCategory> categoryList, string fatherId)
        {
            var cateList = (from c in categoryList
                            where c.category_father_id.ToString() == fatherId
                            select new
                            {
                                id = c.category_id,
                                text = c.category_name
                            }).ToList().ConvertAll<ProductCategoryCustom>(m => new ProductCategoryCustom
                            {
                                id = m.id.ToString(),
                                text = m.text
                            });
            return cateList;
        }

        #region 遞歸得到分類節點 +void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, List<ProductCategorySet> resultList)
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, List<ProductCategorySetTemp> TempresultList, List<ProductCategorySet> resultList)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, item.id.ToString());
                item.children = childList;
                ProductCategorySet resultTemp = new ProductCategorySet();
                if (TempresultList != null)
                {
                    resultTemp = TempresultList.Where(m => m.Category_Id.ToString() == item.id).FirstOrDefault();
                }
                else if (resultList != null)
                {
                    resultTemp = resultList.Where(m => m.Category_Id.ToString() == item.id).FirstOrDefault();
                }
                if (resultTemp != null)
                {
                    item.Checked = true;
                }

                if (childList.Count() > 0)
                {
                    GetCategoryList(categoryList, ref childList, TempresultList, resultList);
                }
            }
        }
        #endregion


        #endregion
        #region 查詢商品信息
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProduct()
        {
            string json = string.Empty;
            string prodTempID = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id))
                    {
                        _prodMgr = new ProductMgr(connectionString);
                        Product product = _prodMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();

                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(product.Product_Image))
                            {
                                product.Product_Image = imgServerPath + prodPath + GetDetailFolder(product.Product_Image) + product.Product_Image;
                            }
                            else
                            {
                                product.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                            }
                        }
                        json = "{success:true,data:" + JsonConvert.SerializeObject(product) + "}";
                    }
                    else
                    {
                        _prodTempMgr = new ProductTempMgr(connectionString);
                        //int writerId = (Session["caller"] as Caller).user_id;
                        if (!string.IsNullOrEmpty(Request.Form["ProductId"]))//非新增時
                        {
                            prodTempID = Request.Form["ProductId"].ToString();
                        }
                        ProductTemp queryProdTemp = new ProductTemp();
                        queryProdTemp.Product_Id = prodTempID;
                        //queryProdTemp.Writer_Id = writerId;
                        if (string.IsNullOrEmpty(queryProdTemp.Product_Id))
                        {
                            queryProdTemp.Temp_Status = 11;
                        }
                        queryProdTemp.Create_Channel = 2;
                        //queryProdTemp.Combo_Type = COMBO_TYPE; 查詢組合商品查不出來數據
                        ProductTemp prodTemp = _prodTempMgr.GetProTempByVendor(queryProdTemp).FirstOrDefault();
                        if (prodTemp != null)
                        {
                            if (!string.IsNullOrEmpty(prodTemp.Product_Image))
                            {
                                prodTemp.Product_Image = imgServerPath + prodPath + GetDetailFolder(prodTemp.Product_Image) + prodTemp.Product_Image;
                            }
                            else
                            {
                                prodTemp.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                            }
                        }
                        json = "{success:true,data:" + JsonConvert.SerializeObject(prodTemp) + "}";
                    }
                }
                else
                {
                    _prodTempMgr = new ProductTempMgr(connectionString);
                    int writerId = (Session["caller"] as Caller).user_id;
                    ProductTemp query = new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    ProductTemp proTemp = _prodTempMgr.GetProTemp(query);
                    if (proTemp != null)
                    {
                        if (proTemp.Product_Image != "")
                        {
                            proTemp.Product_Image = imgServerPath + prodPath + GetDetailFolder(proTemp.Product_Image) + proTemp.Product_Image;
                        }
                        else
                        {
                            proTemp.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                        }
                    }
                    json = "{success:true,data:" + JsonConvert.SerializeObject(proTemp) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase ProcomQueryProduct()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))//編輯數據
                {
                    uint product_id = 0;
                    if (!uint.TryParse(Request.Form["ProductId"], out product_id)) //臨時數據編輯處理
                    {
                        _prodTempMgr = new ProductTempMgr(connectionString);
                        ProductTemp productTemp = new ProductTemp();
                        if (!string.IsNullOrEmpty(Request.Form["childId"]) && Request.Form["childId"] == "true")//為組合商品添加臨時表裡面的子商品
                        {
                            productTemp = _prodTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Form["ProductId"].ToString() }).FirstOrDefault();
                        }
                        else
                        {
                            productTemp = _prodTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Form["ProductId"].ToString(), Create_Channel = 2 }).FirstOrDefault();
                        }
                        if (productTemp != null)
                        {
                            if (productTemp.Product_Image != "")
                            {
                                productTemp.Product_Image = imgServerPath + prod50Path + GetDetailFolder(productTemp.Product_Image) + productTemp.Product_Image;
                            }
                            else
                            {
                                productTemp.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                            }
                        }
                        json = "{success:true,data:" + JsonConvert.SerializeObject(productTemp) + "}";
                    }
                    else
                    {
                        _prodMgr = new ProductMgr(connectionString);
                        Product product = _prodMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                        if (product != null)
                        {
                            if (product.Product_Image != "")
                            {
                                product.Product_Image = imgServerPath + prod50Path + GetDetailFolder(product.Product_Image) + product.Product_Image;
                            }
                            else
                            {
                                product.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                            }
                        }
                        json = "{success:true,data:" + JsonConvert.SerializeObject(product) + "}";
                    }
                }
                else//新增數據 
                {
                    _prodTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp query = new ProductTemp { Writer_Id = (Session["caller"] as Caller).user_id, Combo_Type = COMBO_TYPE_2, Temp_Status = 11, Create_Channel = 2 };
                    //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))//複製商品
                    //{
                    //    query.Vendor_Product_Id = Request.Form["OldProductId"];
                    //}
                    ProductTemp proTemp = _prodTempMgr.GetProTempByVendor(query).FirstOrDefault();
                    if (proTemp != null)
                    {
                        if (proTemp.Product_Image != "")
                        {
                            proTemp.Product_Image = imgServerPath + prod50Path + GetDetailFolder(proTemp.Product_Image) + proTemp.Product_Image;
                        }
                        else
                        {
                            proTemp.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                        }
                    }
                    json = "{success:true,data:" + JsonConvert.SerializeObject(proTemp) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion
        #region 查询單一商品價格信息
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetPriceMaster()
        {
            string json = string.Empty;
            bool isEdit = false;
            try
            {
                _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                PriceMasterTemp query = new PriceMasterTemp();
                query.writer_Id = (Session["caller"] as Caller).user_id;
                query.combo_type = COMBO_TYPE;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    query.product_id = Request.Form["ProductId"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    query.product_id = Request.Form["OldProductId"].ToString();
                //}
                if (!string.IsNullOrEmpty(Request.Params["IsEdit"]))
                {
                    isEdit = Request.Params["IsEdit"].ToString() == "true" ? true : false;
                }
                if (isEdit)
                {
                    List<PriceMasterCustom> proSiteCustom = _priceMasterTempMgr.QueryProdSiteByVendor(query);
                    StringBuilder strJson = new StringBuilder("[");
                    if (proSiteCustom != null)
                    {
                        foreach (var item in proSiteCustom)
                        {
                            strJson.Append("{");
                            strJson.AppendFormat("price_master_id:{0},product_id:\"{1}\",site_id:{2},site_name:\"{3}\"", item.price_master_id, item.product_id, item.site_id, item.site_name);
                            strJson.AppendFormat(",product_name:\"{0}\",bonus_percent:{1},default_bonus_percent:{2}", item.product_name, item.bonus_percent, item.default_bonus_percent);
                            strJson.AppendFormat(",user_level:{0},user_id:{1},user_email:\"{2}\",user_level_name:\"{3}\"", item.user_level, item.user_id, item.user_email, item.user_level_name);
                            strJson.AppendFormat(",event_start:\"{0}\"", item.event_start);
                            strJson.AppendFormat(",event_end:\"{0}\",status:\"{1}\",accumulated_bonus:\"{2}\"", item.event_end, item.status, item.accumulated_bonus);
                            strJson.AppendFormat(",bonus_percent_start:\"{0}\",bonus_percent_end:\"{1}\",valid_start:\"{2}\",valid_end:\"{3}\"", item.bonus_percent_start, item.bonus_percent_end, item.valid_start, item.valid_end);
                            if (item.same_price == 1)
                            {
                                strJson.Append(",same_price:\"on\"");
                                strJson.AppendFormat(",item_cost:{0},item_money:{1}", item.cost, item.price);
                                strJson.AppendFormat(",event_cost:{0},event_money:{1}", item.event_cost, item.event_price);
                            }
                            strJson.Append("}");
                        }
                    }
                    strJson.Append("]");
                    json = strJson.ToString().Replace("}{", "},{");
                }
                else
                {
                    json = JsonConvert.SerializeObject(_priceMasterTempMgr.QueryByVendor(query));
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 查询組合商品價格信息
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProcmoGetPriceMaster()
        {
            string json = string.Empty;
            try
            {
                List<PriceMasterCustom> proSiteCustom = new List<PriceMasterCustom>();
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    string Vendor_PId = Request.Params["ProductId"];
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id)) //正式表數據
                    {
                        _pMaster = new PriceMasterMgr(connectionString);
                        proSiteCustom = _pMaster.Query(new PriceMaster { product_id = product_id, child_id = Convert.ToInt32(product_id) });
                    }
                    else
                    {
                        _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                        proSiteCustom = _priceMasterTempMgr.QueryProdSiteByVendor(new PriceMasterTemp { product_id = Vendor_PId, combo_type = COMBO_TYPE, child_id = Vendor_PId, writer_Id = (Session["caller"] as Caller).user_id });
                    }
                    StringBuilder strJson = new StringBuilder("[");
                    if (proSiteCustom != null)
                    {
                        foreach (var item in proSiteCustom)
                        {
                            strJson.Append("{");
                            strJson.AppendFormat("price_master_id:{0},product_id:\"{1}\",site_id:{2},site_name:\"{3}\"", item.price_master_id, item.product_id, item.site_id, item.site_name);
                            strJson.AppendFormat(",product_name:\"{0}\",bonus_percent:{1},default_bonus_percent:{2}", item.product_name, item.bonus_percent, item.default_bonus_percent);
                            strJson.AppendFormat(",user_level_name:\"{0}\",user_email:\"{1}\",user_level:{2}", item.user_level_name, item.user_email, item.user_level);
                            strJson.AppendFormat(",event_start:\"{0}\",user_id:{1}", item.event_start, item.user_id);
                            strJson.AppendFormat(",event_end:\"{0}\"", item.event_end);
                            strJson.AppendFormat(",cost:\"{0}\"", item.cost);
                            strJson.AppendFormat(",event_cost:\"{0}\"", item.event_cost);
                            strJson.AppendFormat(",price_status:\"{0}\"", item.price_status);
                            //if (item.same_price == 1)//edit by hufeng0813w 2014/06/16 Reason:所有單一商品規格不同價時 也去master表中的價格和活動價格
                            //{
                            strJson.AppendFormat(",price:\"{0}\"", item.price);
                            strJson.AppendFormat(",event_price:\"{0}\"", item.event_price);
                            //}
                            strJson.AppendFormat(",status:\"{0}\",accumulated_bonus:{1},bonus_percent_start:\"{2}\",bonus_percent_end:\"{3}\",same_price:{4},valid_start:\"{5}\",valid_end:\"{6}\"", item.status, item.accumulated_bonus, item.bonus_percent_start, item.bonus_percent_end, item.same_price, item.valid_start, item.valid_end);
                            strJson.Append("}");
                        }
                    }
                    strJson.Append("]");
                    json = strJson.ToString().Replace("}{", "},{");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        public HttpResponseBase GetSelectedCage()
        {
            string resultStr = "{success:false}";
            try
            {
                string strCateId = string.Empty;
                string vendor_pid = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    vendor_pid = Request.Params["ProductId"];
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    vendor_pid = Request.Form["OldProductId"];
                //}
                _prodTempMgr = new ProductTempMgr(connectionString);
                ProductTemp query = new ProductTemp();
                query.Combo_Type = COMBO_TYPE;
                query.Product_Id = vendor_pid;

                ProductTemp tempResult = _prodTempMgr.GetVendorProTemp(query);
                if (tempResult != null)
                {
                    strCateId = tempResult.Cate_Id;
                }
                paraMgr = new ParameterMgr(connectionString);
                Parametersrc cate2Result = paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate", ParameterCode = strCateId }).FirstOrDefault();
                if (cate2Result != null)
                {
                    Parametersrc cate1Result = paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate", ParameterCode = cate2Result.TopValue.ToString() }).FirstOrDefault();
                    if (cate1Result != null)
                    {
                        StringBuilder stb = new StringBuilder("{");
                        stb.AppendFormat("cate1Name:\"{0}\",cate1Value:\"{1}\",cate1Rowid:\"{2}\",cate1TopValue:\"{3}\"", cate1Result.parameterName, cate1Result.ParameterCode, cate1Result.Rowid, cate1Result.TopValue);
                        stb.AppendFormat(",cate2Name:\"{0}\",cate2Value:\"{1}\",cate2Rowid:\"{2}\",cate2TopValue:\"{3}\"", cate2Result.parameterName, cate2Result.ParameterCode, cate2Result.Rowid, cate2Result.TopValue);
                        stb.Append("}");
                        resultStr = "{success:true,data:" + stb.ToString() + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #region 商品細項價格+HttpResponseBase GetProItems()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProItems()
        {
            string json = string.Empty;
            string productID = string.Empty;
            try
            {
                ProductItemTemp query = new ProductItemTemp();
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {   //product_item
                    productID = Request.Form["ProductId"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    productID = Request.Form["OldProductId"].ToString();
                //}
                query.Product_Id = productID;
                //query.Writer_Id = (Session["caller"] as Caller).user_id;
                _productItemTempMgr = new ProductItemTempMgr(connectionString);
                List<ProductItemTemp> proItemTemp = _productItemTempMgr.QueryByVendor(query);
                json = JsonConvert.SerializeObject(proItemTemp);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 編輯時查詢組合商品下單一商品價格信息 + HttpResponseBase QuerySingleProPrice()
        public HttpResponseBase QuerySingleProPrice()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id)) //正式表數據
                    {
                        _pMaster = new PriceMasterMgr(connectionString);
                        List<SingleProductPrice> singleList = _pMaster.SingleProductPriceQuery(uint.Parse(Request.Params["product_id"]), int.Parse(Request.Params["pile_id"]));
                        json = "{data:" + JsonConvert.SerializeObject(singleList) + "}";
                    }
                    else//臨時表數據處理
                    {
                        _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                        List<SingleProductPriceTemp> singleList = _priceMasterTempMgr.SingleProductPriceQueryByVendor(Request.Params["product_id"], int.Parse(Request.Params["pile_id"]));
                        json = "{data:" + JsonConvert.SerializeObject(singleList) + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 獲取組合商品的名字以及信息 +string groupNameQuery()
        [HttpPost]
        [CustomHandleError]
        public string groupNameQuery()
        {
            string json = string.Empty;
            try
            {
                string ParentId = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    ParentId = Request.Params["ProductId"];
                }
                List<ProductComboTemp> resultList = null;
                _combTempMgr = new ProductComboTempMgr(connectionString);
                ProductComboTemp query = new ProductComboTemp();
                query.Parent_Id = ParentId;
                resultList = _combTempMgr.groupNumQueryByVendor(query);
                json = JsonConvert.SerializeObject(resultList);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return json;
        }
        #endregion
        #region 根據id獲取供應商商品規格頁面數據 + HttpResponseBase combSpecQuery()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecQuery()
        {
            string json = "{success:true}";
            try
            {
                int pileId = 0;
                int.TryParse(Request.Params["pileId"] ?? "0", out pileId);
                string parentId = string.Empty;
                _combTempMgr = new ProductComboTempMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    parentId = Request.Params["ProductId"];
                }
                ProductComboCustomVendor query = new ProductComboCustomVendor { Pile_Id = pileId, create_channel = 2, temp_status = 12, Parent_Id = parentId };
                json = "{success:true,data:" + JsonConvert.SerializeObject(_combTempMgr.combQueryByVendor(query)) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            Response.Clear();
            Response.Write(json);
            Response.End();
            return this.Response;
        }
        #endregion
        [HttpPost]
        public string QuerySpecPic()
        {
            string serverSpecPath = imgServerPath + specPath;
            string serverSpec100Path = imgServerPath + spec100Path;
            string serverSpec280Path = imgServerPath + spec280Path;
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);
            string json = string.Empty;
            ProductSpecTemp psTemp = new ProductSpecTemp();
            psTemp.spec_type = 1;
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                psTemp.product_id = Request.Params["product_id"];
            }
            List<ProductSpecTemp> results = _specTempMgr.VendorQuery(psTemp); //JsonConvert.SerializeObject();
            foreach (var item in results)
            {
                if (item.spec_image != "")
                {
                    item.spec_image = serverSpecPath + GetDetailFolder(item.spec_image) + item.spec_image;
                }
                else
                {
                    item.spec_image = imgServerPath + "/product/nopic_50.jpg";
                }
            }
            json = "{success:true,items:" + JsonConvert.SerializeObject(results) + "}";
            json = json.Replace("spec_image", "img");
            return json;
        }

        #region 規格
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase spec1TempQuery()
        {
            string resultStr = "{success:false}";
            try
            {
                //product_spec_temp
                _specTempMgr = new ProductSpecTempMgr(connectionString);
                ProductSpecTemp query = new ProductSpecTemp();
                query.spec_type = 1;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    query.product_id = Request.Form["ProductId"].ToString();
                }
                resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specTempMgr.VendorQuery(query)) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase spec2TempQuery()
        {
            string resultStr = "{success:false}";
            try
            {   //product_spec_temp
                _specTempMgr = new ProductSpecTempMgr(connectionString);
                ProductSpecTemp query = new ProductSpecTemp { spec_type = 2 };
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    query.product_id = Request.Form["ProductId"].ToString();
                }
                resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specTempMgr.VendorQuery(query)) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        [HttpPost]
        [CustomHandleError]
        public string getCateType()
        {
            string resultStr = "";
            try
            {
                ParameterMgr paraMgr = new ParameterMgr(connectionString);
                resultStr = paraMgr.Query("product_spec");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return resultStr;
        }
        #endregion
        #region 庫存

        #region 查詢庫存信息+string QueryStock()
        [HttpPost]
        [CustomHandleError]
        public string QueryStock()
        {
            string json = string.Empty;
            ProductItemTemp query = new ProductItemTemp();
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                query.Product_Id = Request.Params["product_id"].ToString();
            }
            //if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))
            //{
            //    query.Product_Id = Request.Params["OldProductId"].ToString();
            //}
            //query.Writer_Id = (Session["caller"] as Caller).user_id;

            //查找臨時表是否有記錄
            _productItemTempMgr = new ProductItemTempMgr(connectionString);
            json = _productItemTempMgr.QueryStockByVendor(query);

            return json;
        }
        #endregion

        [HttpPost]
        public string VendorQueryItemStock()
        {
            string json = string.Empty;
            _productItemMgr = new ProductItemMgr(connectionString);
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                try
                {
                    int pile_id = 0;
                    if (!string.IsNullOrEmpty(Request.Params["pile_id"]))
                    {
                        pile_id = Convert.ToInt32(Request.Params["pile_id"]);
                    }
                    List<StockDataCustom> stockCustomList = _productItemMgr.VendorQueryItemStock(Request.Params["product_id"].ToString(), pile_id);
                    json = "{items:" + JsonConvert.SerializeObject(stockCustomList) + "}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "[]";
                }
            }
            return json;
        }

        #endregion
        #region 商品狀態更動歷程
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductStatusHistoryQuery()
        {
            string json = "{success:false}";
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    ProductStatusHistoryMgr _mgr = new ProductStatusHistoryMgr(connectionString);
                    List<ProductStatusHistoryCustom> resultList = _mgr.HistoryQuery(new ProductStatusHistoryCustom { product_id = uint.Parse(Request.Params["ProductId"]) });
                    json = "{success:true,data:" + JsonConvert.SerializeObject(resultList) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 商品預覽和連接編碼

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductPreview()
        {
            _siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            _pMaster = new PriceMasterMgr(connectionString);
            List<SiteConfig> configList = _siteConfigMgr.Query();
            PriceMaster Pm = new PriceMaster();
            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
            string DomainName = configList.Where(m => m.Name.Equals("DoMain_Name")).FirstOrDefault().Value;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))//商品ID
                {
                    string type = Request.Form["Type"];
                    string product_id = Request.Form["Product_Id"].ToString();
                    string site_id = Request.Form["Site_Id"] == null ? "" : Request.Form["Site_Id"].ToString();
                    string user_level = Request.Form["Level"] == null ? "" : Request.Form["Level"].ToString();
                    string user_id = Request.Form["Master_User_Id"] == null ? "" : Request.Form["Master_User_Id"].ToString();
                    string result = "";
                    if (type == "0")
                    {
                        result += "http://" + DomainName + "/product.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");//商品預覽
                    }
                    if (type == "1")
                    {
                        //商品預覽+價格頁面
                        result += "http://" + DomainName + "/product.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "&sid=" + site_id + "&ulv=" + user_level + "&uid=" + user_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");
                        result += "|";
                        result += "http://" + DomainName + "/product.php?pid=" + product_id + "&sid=" + site_id + "&code=" + hash.Md5Encrypt(product_id + "&sid=" + site_id, "32");//商品隱賣連結: + "&ulv=" + user_level
                    }
                    json = result.ToString().ToLower();
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "無預覽信息";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        public string QueryExplainPic()
        {
            int writerID = (Session["caller"] as Caller).user_id;
            string json = string.Empty;
            string serverDescPath = imgServerPath + descPath;
            ProductPictureTemp query = new ProductPictureTemp();
            query.writer_Id = writerID;
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                query.product_id = Request.Form["product_id"];
            }
            uint pid = 0;
            if (uint.TryParse(Request.Params["product_id"], out pid))
            {
                #region 正式表
                _pPicMgr = new ProductPictureMgr(connectionString);
                List<ProductPicture> picProList = _pPicMgr.Query(Convert.ToInt32(Request.Form["product_id"]));
                foreach (var item in picProList)
                {
                    if (item.image_filename != "")
                    {
                        item.image_filename = serverDescPath + GetDetailFolder(item.image_filename) + item.image_filename;
                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(picProList) + "}";
                json = json.Replace("image_filename", "img");
                #endregion
            }
            else
            {
                #region 供應商 臨時表
                _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
                query.writer_Id = 0;//臨時數據不能加writer_Id
                List<ProductPictureTemp> picList = _pPicTempMgr.VendorQuery(query);
                foreach (var item in picList)
                {
                    if (item.image_filename != "")
                    {
                        item.image_filename = serverDescPath + GetDetailFolder(item.image_filename) + item.image_filename;
                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(picList) + "}";
                json = json.Replace("image_filename", "img");
                #endregion
            }
            return json;
        }
        #endregion



    }
}
