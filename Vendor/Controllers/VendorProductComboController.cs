#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：VendorProductComboController.cs
* 摘 要：
* * 供應商商品管理 組合商品新增
* 当前版本：v1.0
* 作 者： mengjuan0826j
* 完成日期：2014/08/26  供應商組合商品新增視圖  基本資料頁面加載ok
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using System.Configuration;
using Vendor.CustomHandleError;
using BLL.gigade.Mgr.Impl;
using Newtonsoft.Json;
using System.Text;
using System.Collections;
using BLL.gigade.Common;
using System.Web.Script.Serialization;
using BLL.gigade.Model.Custom;
using System.IO;
namespace Vendor.Controllers
{
    public class VendorProductComboController : Controller
    {
        //
        // GET: /VendorProductCombo/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private ParameterMgr paraMgr;
        private IProductComboImplMgr _combMgr;
        private IProductImplMgr _productMgr;
        private IProductTempImplMgr _productTempMgr;
        private IPriceMasterTempImplMgr _pMasterTempMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IProductSpecTempImplMgr _specTempMgr;
        private IProductPictureTempImplMgr _pPicTempMgr;
        private IProductTagImplMgr _productTagMgr;
        private IProductTagSetTempImplMgr _productTagSetTempMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetTempImplMgr _productNoticeSetTempMgr;
        private IProductCategoryImplMgr _procateMgr;
        //  private IProductCategorySetImplMgr _categorySetMgr;
        private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        private IProductItemImplMgr _productItemMgr;
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);

        private ISiteImplMgr _siteMgr;
        private VendorBrandMgr vbMgr;
        //  private ISiteConfigImplMgr siteConfigMgr;
        private IProductPictureImplMgr _productPicMgr;
        private IProductSpecImplMgr _specMgr;
        private IProductItemTempImplMgr _productItemTempMgr;
        string prodPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prodPath);
        string prod50Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod50Path);
        string prod150Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod150Path);
        string prod280Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod280Path);

        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.specPath);
        string spec100Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.spec100Path);
        string spec280Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.spec280Path);


        string descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descPath);
        string desc400Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.desc400Path);

        string tagPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_tagPath);
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);
        private int COMBO_TYPE = 2;
        private IProductComboTempImplMgr _combTempMgr;
        private int imgNameIdx = 7;
        #region View

        public ActionResult Index(string id)
        {
            ViewBag.ProductId = id;//獲取編輯時傳遞過來的id
            ViewBag.OldProductId = Request.QueryString["product_id"] ?? "";//獲取商品複製時傳遞過來的id
            ViewBag.hfAuth = Request.Cookies[FormsAuthentication.FormsCookieName] == null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value;
            ViewBag.hfAspSessID = Session.SessionID;
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                ViewBag.ProductId = Request.QueryString["id"].ToString();
            }
            if (string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                ViewBag.IsEdit = "false";
            }
            else
            {
                ViewBag.IsEdit = "true";
            }
            return View();
        }
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public ActionResult BaseInfo()
        {
            Parametersrc para = new Parametersrc();
            string[] paraList = new string[2] { "", "" };
            //加載運費方式
            para.ParameterType = "product_freight";
            paraList[0] = QueryParameter(para);
            //加載出貨方式
            para.ParameterType = "product_mode";
            paraList[1] = QueryParameter(para);
            return View(paraList);
        }

        public ActionResult Description()
        {
            return View();
        }

        public ActionResult Spec()
        {
            return View();
        }

        public ActionResult Price()
        {
            return View();
        }


        public ActionResult Category()
        {
            return View();
        }

        public ActionResult productStock()
        {
            return View();
        }

        public ActionResult ProductPic()
        {

            return View();
        }

        public ActionResult upLoad()
        {
            return View();
        }

        #endregion

        #region  獲取運費方式和出貨方式 +string QueryParameter(Parametersrc p)
        /// <summary>
        /// 獲取運費方式和出貨方式參數
        /// </summary>
        /// <param name="p">parameter model 對象</param>
        /// <returns>json數組</returns>
        public string QueryParameter(Parametersrc p)
        {
            string json = string.Empty;
            try
            {
                paraMgr = new ParameterMgr(connectionString);
                json = paraMgr.QueryBindData(p);
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

        #region 獲取新增和編輯時正在處理的數據 +HttpResponseBase QueryProduct()

        /// <summary>
        /// 獲取新增和編輯時正在處理的數據
        /// </summary>
        /// <returns>json數組</returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProduct()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))//編輯數據
                {
                    uint product_id = 0;
                    if (!uint.TryParse(Request.Form["ProductId"], out product_id)) //臨時數據編輯處理
                    {
                        _productTempMgr = new ProductTempMgr(connectionString);
                        ProductTemp productTemp = new ProductTemp();
                        if (!string.IsNullOrEmpty(Request.Form["childId"]) && Request.Form["childId"] == "true")//為組合商品添加臨時表裡面的子商品
                        {
                            productTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Form["ProductId"].ToString() }).FirstOrDefault();
                        }
                        else
                        {
                            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                            int write_Id = Convert.ToInt32(vendorModel.vendor_id);


                            productTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Writer_Id = write_Id, Product_Id = Request.Form["ProductId"].ToString(), Create_Channel = 2 }).FirstOrDefault();
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
                        _productMgr = new ProductMgr(connectionString);
                        Product product = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
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
                    _productTempMgr = new ProductTempMgr(connectionString);
                    BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                    int write_Id = Convert.ToInt32(vendorModel.vendor_id);
                    ProductTemp query = new ProductTemp { Writer_Id = write_Id, Combo_Type = COMBO_TYPE, Temp_Status = 11, Create_Channel = 2 };
                    //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))//複製商品
                    //{
                    //    query.Vendor_Product_Id = Request.Form["OldProductId"];
                    //}
                    ProductTemp proTemp = _productTempMgr.GetProTempByVendor(query).FirstOrDefault();
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

        #region 刪除臨時數據

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase DeleteTempPro()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int write_Id = Convert.ToInt32(vendorModel.vendor_id);
                string product_id = string.Empty;
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    product_id = Request.Form["OldProductId"].ToString();
                //}
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    product_id = Request.Form["ProductId"].ToString();
                }
                _productTempMgr = new ProductTempMgr(connectionString);
                //刪除服務器上對應的圖片
                DeletePicOnServer(true, true, true, product_id);
                if (_productTempMgr.DeleteVendorProductTemp(write_Id, COMBO_TYPE, product_id))
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
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

        #region 獲取詳細的文件地址 +string GetDetailFolder(string picName)

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

        #region 從product_temp product_spec_temp 和product_picture中獲取圖片路徑 刪除服務器上面的圖片 +void DeletePicOnServer(bool prod, bool spec, bool desc, string product_id)
        /// <summary>
        /// 從product_temp product_spec_temp 和product_picture中獲取圖片路徑 刪除服務器上面的圖片
        /// </summary>
        /// <param name="prod">是否獲取product_temp圖片路徑數據</param>
        /// <param name="spec">是否獲取product_spec_temp圖片路徑數據</param>
        /// <param name="desc">是否獲取product_picture_temp圖片路徑數據</param>
        /// <param name="product_id">商品id</param>
        public void DeletePicOnServer(bool prod, bool spec, bool desc, string product_id)
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int write_Id = Convert.ToInt32(vendorModel.vendor_id);

            _productTempMgr = new ProductTempMgr(connectionString);
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
            ArrayList ImgList = new ArrayList();

            ProductSpecTemp pSpec = new ProductSpecTemp();
            pSpec.Writer_Id = write_Id;
            pSpec.product_id = product_id;
            pSpec.spec_type = 1;

            //刪除對應的圖片
            //商品圖
            if (prod)
            {
                ProductTemp query = new ProductTemp { Writer_Id = write_Id, Combo_Type = COMBO_TYPE, Product_Id = product_id, Create_Channel = 2 };
                string fileName = _productTempMgr.GetProTempByVendor(query).FirstOrDefault().Product_Image;
                if (!string.IsNullOrEmpty(fileName))
                {
                    ImgList.Add(imgLocalPath + prodPath + GetDetailFolder(fileName) + fileName);
                    ImgList.Add(imgLocalPath + prod50Path + GetDetailFolder(fileName) + fileName);
                    ImgList.Add(imgLocalPath + prod150Path + GetDetailFolder(fileName) + fileName);
                    ImgList.Add(imgLocalPath + prod280Path + GetDetailFolder(fileName) + fileName);
                }

            }
            //規格圖
            if (spec)
            {
                List<ProductSpecTemp> pSList = _specTempMgr.Query(pSpec);
                foreach (var item in pSList)
                {
                    if (item.spec_image != "")
                    {
                        ImgList.Add(imgLocalPath + specPath + GetDetailFolder(item.spec_image) + item.spec_image);
                        ImgList.Add(imgLocalPath + spec100Path + GetDetailFolder(item.spec_image) + item.spec_image);
                        ImgList.Add(imgLocalPath + spec280Path + GetDetailFolder(item.spec_image) + item.spec_image);
                    }
                }
            }
            //商品說明圖
            if (desc)
            {
                ProductPictureTemp query = new ProductPictureTemp { writer_Id = write_Id, combo_type = COMBO_TYPE, product_id = product_id };
                List<ProductPictureTemp> pPList = _pPicTempMgr.Query(query);
                foreach (var item in pPList)
                {
                    ImgList.Add(imgLocalPath + descPath + GetDetailFolder(item.image_filename) + item.image_filename);
                    ImgList.Add(imgLocalPath + desc400Path + GetDetailFolder(item.image_filename) + item.image_filename);
                }
            }

            foreach (string item in ImgList)
            {
                //刪除服務器上對應的圖片
                if (System.IO.File.Exists(item))
                {
                    System.IO.File.Delete(item);
                }
            }
        }
        #endregion

        #region 組合規格發生變化時刪除相關數據（product_combo_temp,price_master_temp,item_price_temp...） +HttpResponseBase combSpecTempDelete()
        /// <summary>
        /// 組合規格發生變化時刪除相關數據（product_combo_temp,price_master_temp,item_price_temp...）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecTempDelete()
        {
            string resultStr = "{success:false}";
            try
            {
                _combTempMgr = new ProductComboTempMgr(connectionString);
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int write_Id = Convert.ToInt32(vendorModel.vendor_id);
                ProductComboTemp delete = new ProductComboTemp { Writer_Id = write_Id, Combo_Type = COMBO_TYPE };
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    delete.Parent_Id = Request.Form["ProductId"].ToString();
                }
                //複製商品時
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    delete.Parent_Id = Request.Form["OldProductId"].ToString();
                //}

                _combTempMgr.DeleteByVendor(delete);
                resultStr = "{success:true}";
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

        #region 基本資料保存 +HttpResponseBase SaveBaseInfo()
        [HttpPost]
        public HttpResponseBase SaveBaseInfo()
        {
            ProductTemp pTemp = new ProductTemp();
            _productTempMgr = new ProductTempMgr(connectionString);
            _productMgr = new ProductMgr(connectionString);
            string json = "{success:true}";

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int write_Id = Convert.ToInt32(vendorModel.vendor_id);

            if (Request.Params["product_id"] != "")
            {
                pTemp.Product_Id = Request.Params["product_id"];
            }
            //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
            //{
            //    pTemp.Product_Id = Request.Form["OldProductId"].ToString();
            //}
            //庫存
            if (!string.IsNullOrEmpty(Request.Params["ig_sh_InsertValue"]))
            {
                string[] Value = Request.Params["ig_sh_InsertValue"].Split(',');
                pTemp.Ignore_Stock = int.Parse(Value[0]);
                pTemp.Shortage = int.Parse(Value[1]);
                pTemp.stock_alarm = int.Parse(Value[2]);

            }
            ProductTemp query = new ProductTemp { Writer_Id = write_Id, Combo_Type = COMBO_TYPE, Product_Id = pTemp.Product_Id, Create_Channel = 2 };

            if (_productTempMgr.GetProTempByVendor(query).FirstOrDefault() != null)
            {
                pTemp = _productTempMgr.GetProTempByVendor(query).FirstOrDefault();
            }

            #region 獲取前台數據
            uint brand_id = 0;
            string product_name = "";
            uint product_sort = 0;
            string product_vendor_code = "";
            uint product_start = 0;
            uint product_end = 0;
            uint expect_time = 0;
            uint product_freight_set = 0;
            int tax_type = 0;
            uint combination = 0;
            uint product_mode = 0;
            string expect_msg = string.Empty;

            //庫存
            if (!string.IsNullOrEmpty(Request.Params["ig_sh_InsertValue"]))
            {
                string[] Value = Request.Params["ig_sh_InsertValue"].Split(',');
                pTemp.Ignore_Stock = int.Parse(Value[0]);
                pTemp.Shortage = int.Parse(Value[1]);
                pTemp.stock_alarm = int.Parse(Value[2]);
            }
            else
            {
                brand_id = uint.Parse(Request.Form["brand_id"]);
                product_name = Request.Form["product_name"];
                product_sort = uint.Parse(Request.Form["product_sort"]);
                product_vendor_code = Request.Form["product_vendor_code"];
                product_start = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_start"]).ToString());
                product_end = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_end"]).ToString());
                expect_time = uint.Parse(CommonFunction.GetPHPTime(Request.Form["expect_time"]).ToString());
                product_freight_set = uint.Parse(Request.Form["product_freight_set"]);
                tax_type = int.Parse(Request.Form["tax_type"]);
                combination = uint.Parse(Request.Form["combination"]);
                product_mode = uint.Parse(Request.Params["product_mode"]);
                expect_msg = Request.Form["expect_msg"] ?? "";
            }
            #endregion

            pTemp.Brand_Id = brand_id;
            pTemp.Product_Name = product_name;
            pTemp.Product_Sort = product_sort;
            pTemp.Product_Vendor_Code = product_vendor_code;
            pTemp.Product_Start = product_start;
            pTemp.Product_End = product_end;
            pTemp.Expect_Time = expect_time;
            pTemp.Product_Freight_Set = product_freight_set;
            pTemp.Product_Mode = product_mode;
            pTemp.Tax_Type = tax_type;
            pTemp.Combination = combination;
            pTemp.expect_msg = expect_msg;
            pTemp.Combo_Type = COMBO_TYPE;
            //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
            //{
            //    pTemp.Vendor_Product_Id = Request.Form["OldProductId"].ToString();
            //}

            pTemp.Writer_Id = write_Id;
            pTemp.Product_Status = 20;
            pTemp.Product_Ipfrom = Request.UserHostAddress;
            #region 表處理

            //新增數據處理
            if (string.IsNullOrEmpty(Request.Params["product_id"]) || pTemp == null)
            {
                try
                {
                    pTemp.Temp_Status = 11;
                    pTemp.Create_Channel = 2;
                    pTemp.Product_Createdate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                    pTemp.Product_Updatedate = pTemp.Product_Createdate;
                    string product_id = _productTempMgr.vendorBaseInfoSave(pTemp);
                    if (!string.IsNullOrEmpty(product_id))
                    {
                        ViewBag.ProductId = product_id;
                        json = "{success:true,msg:'" + product_id + "'}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }

                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false,msg:'" + Resources.VendorProduct.SAVE_FAIL + "'}";
                }
            }
            else//編輯數據處理  複製商品處理和新增商品處理
            {
                //更新
                try
                {
                    pTemp.Product_Updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());

                    if (!string.IsNullOrEmpty(Request.Params["ig_sh_InsertValue"]))
                    {
                        _productTempMgr.ProductTempUpdateByVendor(pTemp, "stock");
                    }
                    else
                    {
                        if (pTemp.Product_Mode != 2)
                        {
                            pTemp.Bag_Check_Money = 0;
                        }
                        else
                        {
                            pTemp.Bag_Check_Money = pTemp.Bag_Check_Money;
                        }
                        _productTempMgr.vendorBaseInfoUpdate(pTemp);
                    }
                    json = "{success:true,resurt:\"" + Resources.VendorProduct.SAVE_SUCCESS + "\"}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false,msg:'" + Resources.VendorProduct.SAVE_FAIL + "'}";
                }
            }


            #endregion

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 描述頁面

        #region 商品標籤 +HttpResponseBase GetProTag()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProTag()
        {
            string json = string.Empty;
            try
            {
                StringBuilder strJson = new StringBuilder();
                _productTagMgr = new ProductTagMgr(connectionString);
                //查找出全部標籤
                List<ProductTag> tags = _productTagMgr.Query(new ProductTag { tag_status = 1 });
                if (tags != null)
                {
                    _productTagSetTempMgr = new ProductTagSetTempMgr(connectionString);
                    //查找出此ProductId的
                    ProductTagSetTemp ptstQuery = new ProductTagSetTemp();

                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        string productId = Request.Form["ProductId"];
                        ptstQuery.product_id = productId;
                    }
                    //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    //{
                    //    ptstQuery.product_id = Request.Form["OldProductId"].ToString();
                    //}
                    BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                    int write_Id = Convert.ToInt32(vendorModel.vendor_id);

                    ptstQuery.Writer_Id = write_Id;
                    ptstQuery.Combo_Type = COMBO_TYPE;

                    List<ProductTagSetTemp> tagSets = _productTagSetTempMgr.QueryVendorTagSet(ptstQuery);

                    foreach (var item in tags)
                    {
                        strJson.AppendFormat("<input type='checkbox' id='tag_{0}' name='tags' value='{0}' ", item.tag_id);
                        if (tagSets.Exists(m => m.tag_id == item.tag_id))
                        {
                            strJson.Append("checked='true'");
                        }
                        strJson.AppendFormat("/><label for='tag_{0}'><img src='{1}' style='margin-right:5px' /></label>", item.tag_id, imgServerPath + tagPath + GetDetailFolder(item.tag_filename) + item.tag_filename);
                    }

                }
                json = strJson.ToString();
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

        #region 商品公告+HttpResponseBase GetProNotice()

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
                    _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connectionString);
                    ProductNoticeSetTemp queryProductNotice = new ProductNoticeSetTemp();
                    string productId = string.Empty;
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        productId = Request.Form["ProductId"].ToString();
                        queryProductNotice.product_id = productId;
                    }
                    //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    //{
                    //    queryProductNotice.product_id = Request.Form["OldProductId"].ToString();
                    //}
                    BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                    int write_Id = Convert.ToInt32(vendorModel.vendor_id);

                    queryProductNotice.Writer_Id = write_Id;
                    queryProductNotice.Combo_Type = COMBO_TYPE;
                    List<ProductNoticeSetTemp> noticeSets = _productNoticeSetTempMgr.QueryVendorProdNotice(queryProductNotice);
                    ProductTemp proTemp = new ProductTemp { Writer_Id = write_Id, Combo_Type = COMBO_TYPE, Product_Id = productId, Create_Channel = 2 };
                    _productTempMgr = new ProductTempMgr(connectionString);
                    _productTempMgr.GetProTempByVendor(proTemp);
                    bool check = (proTemp != null && !string.IsNullOrEmpty(proTemp.Page_Content_2)) ? false : true;

                    foreach (var item in notices)
                    {
                        strJson.AppendFormat("<input type='checkbox' id='notice_{0}' name='notices' value='{0}' ", item.notice_id);
                        if (check || noticeSets.Exists(m => m.notice_id == item.notice_id))
                        {
                            strJson.Append("checked='true'");
                        }
                        strJson.AppendFormat("/><label for='notice_{0}'>{1}</label>", item.notice_id, item.notice_name);
                    }
                }

                json = strJson.ToString();

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

        #region 保存商品描述 標籤 公告 +HttpResponseBase SaveDescription()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveDescription()
        {
            string json = string.Empty;

            try
            {
                string tags = Request.Form["Tags"] ?? "";
                string notices = Request.Form["Notice"] ?? "";
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                _productTempMgr = new ProductTempMgr(connectionString);
                ProductTemp proTemp = new ProductTemp();

                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writer_id = Convert.ToInt32(vendorModel.vendor_id);

                string Vendor_Product_Id = string.Empty;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    Vendor_Product_Id = Request.Form["ProductId"];
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    Vendor_Product_Id = Request.Form["OldProductId"];
                //}

                proTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Vendor_Product_Id, Combo_Type = COMBO_TYPE, Writer_Id = writer_id, Create_Channel = 2 }).FirstOrDefault();
                if (proTemp == null)
                {
                    proTemp = new ProductTemp();
                }

                proTemp.Page_Content_1 = Request.Form["page_content_1"] ?? "";
                proTemp.Page_Content_2 = Request.Form["page_content_2"] ?? "";
                proTemp.Page_Content_3 = Request.Form["page_content_3"] ?? "";
                proTemp.Product_Keywords = Request.Form["product_keywords"] ?? "";
                if (!string.IsNullOrEmpty(Request.Form["product_buy_limit"]))
                {
                    proTemp.Product_Buy_Limit = uint.Parse(Request.Form["product_buy_limit"]);
                }
                proTemp.Writer_Id = writer_id;
                proTemp.Combo_Type = COMBO_TYPE;
                List<ProductTagSetTemp> tagTemps = jsSer.Deserialize<List<ProductTagSetTemp>>(tags);
                foreach (ProductTagSetTemp item in tagTemps)
                {
                    item.Writer_Id = writer_id;
                    item.Combo_Type = COMBO_TYPE;
                    item.product_id = proTemp.Product_Id;
                }
                List<ProductNoticeSetTemp> noticeTemps = jsSer.Deserialize<List<ProductNoticeSetTemp>>(notices);
                foreach (ProductNoticeSetTemp item in noticeTemps)
                {
                    item.Writer_Id = writer_id;
                    item.Combo_Type = COMBO_TYPE;
                    item.product_id = proTemp.Product_Id;
                }


                if (_productTempMgr.VendorDescriptionInfoUpdate(proTemp, tagTemps, noticeTemps))
                {
                    json = "{success:true}";
                }

                else
                {
                    json = "{success:false}";
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
        #endregion

        #region  規格界面

        #region 保存供應商商品規格 +HttpResponseBase combSpecSave()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecSave()
        {
            string resultStr = "{success:false}";
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writer_id = Convert.ToInt32(vendorModel.vendor_id);

                string result = Request.Params["resultStr"];

                _combTempMgr = new ProductComboTempMgr(connectionString);
                List<ProductComboTemp> comList = JsonConvert.DeserializeObject<List<ProductComboTemp>>(result);
                string parent_id = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    parent_id = Request.Params["ProductId"];
                }

                //複製商品時
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    parent_id = Request.Form["OldProductId"];
                //}
                foreach (ProductComboTemp item in comList)
                {
                    item.Writer_Id = writer_id;
                    item.Parent_Id = parent_id;
                }

                if (_combTempMgr.SaveByVendor(comList))
                {
                    resultStr = "{success:true}";
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

        #region 根據id獲取供應商商品規格頁面數據 + HttpResponseBase combSpecQuery()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecQuery()
        {
            string json = "{success:false}";
            try
            {
                int pileId = 0;
                int.TryParse(Request.Params["pileId"] ?? "0", out pileId);
                uint parentId = 0;

                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writer_id = Convert.ToInt32(vendorModel.vendor_id);
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    if (uint.TryParse(Request.Params["ProductId"].ToString(), out parentId))
                    {
                        _combMgr = new ProductComboMgr(connectionString);
                        json = "{success:true,data:" + JsonConvert.SerializeObject(_combMgr.combQuery(new ProductComboCustom { Parent_Id = int.Parse(Request.Params["ProductId"]), Pile_Id = pileId })) + "}";

                    }
                    else
                    {
                        _combTempMgr = new ProductComboTempMgr(connectionString);
                        ProductComboCustomVendor query = new ProductComboCustomVendor { Writer_Id = writer_id, Pile_Id = pileId, create_channel = 2, temp_status = 12, Parent_Id = Request.Params["ProductId"] };
                        json = "{success:true,data:" + JsonConvert.SerializeObject(_combTempMgr.combQueryByVendor(query)) + "}";
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
            Response.Write(json);
            Response.End();
            return this.Response;
        }
        #endregion

        #region 判斷添加的子商品是否是該供應商之商品 +HttpResponseBase ISVendorProduct()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ISVendorProduct()
        {
            string resultStr = "{success:false}";
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];

                int writer_id = Convert.ToInt32(vendorModel.vendor_id);

                VendorBrand vb = new VendorBrand();
                vb.Vendor_Id = vendorModel.vendor_id;//todo:獲取該供應商下的所有品牌，暫時寫死
                vbMgr = new VendorBrandMgr(connectionString);
                List<VendorBrand> brandList = vbMgr.GetProductBrandList(vb);

                uint brand = 0;

                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))//編輯數據
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id)) //正式表數據
                    {
                        _productMgr = new ProductMgr(connectionString);
                        Product p = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                        if (p != null)
                        {
                            brand = p.Brand_Id;
                        }
                    }
                    else
                    {//臨時表數據
                        _productTempMgr = new ProductTempMgr(connectionString);
                        ProductTemp pTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Form["ProductId"], Writer_Id = writer_id, Create_Channel = 2, Temp_Status = 12 }).FirstOrDefault();
                        if (pTemp != null)
                        {
                            brand = pTemp.Brand_Id;
                        }
                    }

                    foreach (VendorBrand item in brandList)
                    {
                        if (brand == item.Brand_Id)
                        {
                            resultStr = "{success:true}";
                            break;
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
                resultStr = "{success:false}";
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
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
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                string ParentId = string.Empty;
                uint pid = 0;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    if (uint.TryParse(Request.Params["ProductId"].ToString(), out pid))
                    {
                        List<ProductCombo> resultList = null;
                        _combMgr = new ProductComboMgr(connectionString);
                        resultList = _combMgr.groupNumQuery(new ProductCombo { Parent_Id = int.Parse(Request.Params["ProductId"]) });
                        json = JsonConvert.SerializeObject(resultList);
                    }
                    else
                    {
                        ParentId = Request.Params["ProductId"];
                        List<ProductComboTemp> resultList = null;
                        int writer_id = Convert.ToInt32(vendorModel.vendor_id);
                        _combTempMgr = new ProductComboTempMgr(connectionString);
                        ProductComboTemp query = new ProductComboTemp { Writer_Id = writer_id, Parent_Id = ParentId };
                        resultList = _combTempMgr.groupNumQueryByVendor(query);
                        json = JsonConvert.SerializeObject(resultList);
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

            return json;
        }
        #endregion

        #endregion

        #region 保存供應商商品價格 +string ComboPriceSave()

        [HttpPost]
        [CustomHandleError]
        public string ComboPriceSave()
        {
            string json = "{success:true,msg:'" + Resources.VendorProduct.SAVE_SUCCESS + "'}";
            #region 參數

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writer_id = Convert.ToInt32(vendorModel.vendor_id);

            string product_name = Request.Params["product_name"];
            string price_type = Request.Params["price_type"];
            string product_price_list = Request.Params["product_price_list"];
            string price = Request.Params["price"];
            string cost = Request.Params["cost"];
            string max_price = Request.Params["max_price"];
            string event_price = Request.Params["event_price"];
            string event_cost = Request.Params["event_cost"];
            string max_event_price = Request.Params["max_event_price"];
            string event_start = Request.Params["event_start"];
            string event_end = Request.Params["event_end"];
            string bag_check_money = Request.Params["bag_check_money"] == "" ? "0" : Request.Params["bag_check_money"];
            string same_price = Request.Params["same_price"];
            string show_listprice = Request.Params["show_listprice"];
            string valid_start = Request.Params["valid_start"];
            string valid_end = Request.Params["valid_end"];

            #endregion

            string Vendor_productId = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                Vendor_productId = Request.Params["product_id"];
            }
            //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
            //{
            //    Vendor_productId = Request.Params["OldProductId"];
            //}

            ProductTemp pTemp = new ProductTemp();
            List<PriceMasterTemp> pMasterListT = new List<PriceMasterTemp>();

            List<List<ItemPrice>> ItemPList = new List<List<ItemPrice>>();
            PriceMasterTemp pMasterTemp = new PriceMasterTemp();

            #region 臨時表數據修改
            try
            {
                #region product_temp
                //product_temp
                pTemp.Product_Id = Vendor_productId;
                pTemp.Product_Price_List = uint.Parse(product_price_list);
                pTemp.Writer_Id = writer_id;
                pTemp.Combo_Type = COMBO_TYPE;
                pTemp.Price_type = int.Parse(price_type);
                pTemp.Bag_Check_Money = uint.Parse(bag_check_money);
                pTemp.show_listprice = uint.Parse(show_listprice);
                pTemp.Create_Channel = 2;

                #endregion

                #region Price_Master
                //Price_Master
                pMasterTemp.product_id = Vendor_productId;
                pMasterTemp.product_name = product_name;
                pMasterTemp.writer_Id = writer_id;
                pMasterTemp.combo_type = COMBO_TYPE;
                //默認站臺1:吉甲地,(按統一價格比例拆分)會員等級1：普通會員
                pMasterTemp.site_id = 1;
                pMasterTemp.user_level = 1;
                pMasterTemp.same_price = int.Parse(same_price);
                pMasterTemp.price = int.Parse(price);
                pMasterTemp.cost = int.Parse(cost);
                pMasterTemp.max_price = int.Parse(max_price);
                pMasterTemp.max_event_price = int.Parse(max_event_price);
                pMasterTemp.event_price = int.Parse(event_price);
                pMasterTemp.event_cost = int.Parse(event_cost);
                #endregion

                #region 時間 活動時間
                if (event_start != "")
                {
                    pMasterTemp.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(event_start));
                }
                if (event_end != "")
                {
                    pMasterTemp.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(event_end));
                }
                if (!string.IsNullOrEmpty(valid_start))
                {
                    pMasterTemp.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                }
                if (!string.IsNullOrEmpty(valid_end))
                {
                    pMasterTemp.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                }

                #endregion

                pMasterTemp.price_status = 1;

                pMasterListT.Add(pMasterTemp);

                //如果原價格為複製價格，則需刪除原複製價格並將child_id設為oldProductId
                if (!string.IsNullOrEmpty(Vendor_productId))
                {
                    pMasterListT.ForEach(m =>
                    {
                        if (string.IsNullOrEmpty(m.child_id) || m.child_id == "0")
                        {
                            m.child_id = Vendor_productId.ToString();
                        }
                    });
                }

                //查詢                 
                _pMasterTempMgr = new PriceMasterTempMgr(connectionString);
                PriceMasterProductCustomTemp queryReust = _pMasterTempMgr.QueryByVendor(new PriceMasterTemp() { writer_Id = writer_id, child_id = Vendor_productId.ToString(), product_id = Vendor_productId, combo_type = COMBO_TYPE });
                //檢查價格類型是否有變動，如果有則刪除原有價格數據
                if (queryReust != null)
                {
                    if (!price_type.Equals(queryReust.price_type.ToString()))
                    {
                        _combTempMgr = new ProductComboTempMgr(connectionString);
                        _combTempMgr.comboPriceDeleteByVendor(new ProductComboTemp { Writer_Id = writer_id, Combo_Type = COMBO_TYPE, Parent_Id = Vendor_productId });
                        queryReust = null;
                    }
                }

                if (queryReust == null)//插入
                {
                    if (price_type == "1")
                    {
                        _pMasterTempMgr.SaveByVendor(pMasterListT, null, null);
                    }
                }
                else//更新
                {
                    if (price_type == "1")
                    {
                        _pMasterTempMgr.UpdateByVendor(pMasterListT, null);
                    }
                }
                _productTempMgr = new ProductTempMgr(connectionString);
                _productTempMgr.PriceBonusInfoSaveByVendor(pTemp);

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + Resources.VendorProduct.SAVE_FAIL + "'}";
            }
            #endregion

            return json;

        }
        #endregion

        #region 查询站臺 +HttpResponseBase GetSite()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetSite()
        {
            string json = string.Empty;
            try
            {
                _siteMgr = new SiteMgr(connectionString);
                json = JsonConvert.SerializeObject(_siteMgr.Query(new Site()));
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

        #region 新增時 根據該roductid獲取整個價格頁面信息 +HttpResponseBase QueryPriceMasterProduct()
        //查詢價格
        /// <summary>
        ///  新增時 根據該roductid獲取整個價格頁面信息
        /// </summary>
        /// <returns> json數組 data為序列化的數組</returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryPriceMasterProduct()
        {
            string Vendor_Product_Id = string.Empty;
            //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
            //{
            //    Vendor_Product_Id = Request.Form["OldProductId"];
            //}
            if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
            {
                Vendor_Product_Id = Request.Form["ProductId"];
            }
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writer_id = Convert.ToInt32(vendorModel.vendor_id);

                _pMasterTempMgr = new PriceMasterTempMgr(connectionString);
                PriceMasterProductCustomTemp pmpCus = _pMasterTempMgr.QueryByVendor(new PriceMasterTemp() { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = Vendor_Product_Id, child_id = Vendor_Product_Id });
                json = "{success:true,data:" + JsonConvert.SerializeObject(pmpCus) + "}";

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

        #region 編輯時查詢組合商品下單一商品價格信息 + HttpResponseBase QuerySingleProPrice()
        public HttpResponseBase QuerySingleProPrice()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["product_id"], out product_id)) //正式表數據
                    {
                        _priceMasterMgr = new PriceMasterMgr(connectionString);
                        List<SingleProductPrice> singleList = _priceMasterMgr.SingleProductPriceQuery(uint.Parse(Request.Params["product_id"]), int.Parse(Request.Params["pile_id"]));
                        json = "{data:" + JsonConvert.SerializeObject(singleList) + "}";
                    }
                    else//臨時表數據處理
                    {
                        _pMasterTempMgr = new PriceMasterTempMgr(connectionString);
                        List<SingleProductPriceTemp> singleList = _pMasterTempMgr.SingleProductPriceQueryByVendor(Request.Params["product_id"], int.Parse(Request.Params["pile_id"]));
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

        #region 查询商品站台信息

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetPriceMaster()
        {
            string json = string.Empty;
            try
            {
                List<PriceMasterCustom> proSiteCustom = new List<PriceMasterCustom>();
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    string Vendor_PId = Request.Params["ProductId"];

                    BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                    int writer_id = Convert.ToInt32(vendorModel.vendor_id);

                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id)) //正式表數據
                    {
                        _priceMasterMgr = new PriceMasterMgr(connectionString);
                        proSiteCustom = _priceMasterMgr.Query(new PriceMaster { product_id = product_id, child_id = Convert.ToInt32(product_id) });
                    }
                    else
                    {
                        _pMasterTempMgr = new PriceMasterTempMgr(connectionString);
                        proSiteCustom = _pMasterTempMgr.QueryProdSiteByVendor(new PriceMasterTemp { product_id = Vendor_PId, combo_type = COMBO_TYPE, child_id = Vendor_PId, writer_Id = writer_id });
                    }
                    StringBuilder strJson = new StringBuilder("[");
                    if (proSiteCustom != null)
                    {
                        foreach (var item in proSiteCustom)
                        {
                            var vendorpid = item.product_id != 0 ? item.product_id.ToString() : item.vendor_product_id;
                            strJson.Append("{");
                            strJson.AppendFormat("price_master_id:{0},product_id:\"{1}\",site_id:{2},site_name:\"{3}\"", item.price_master_id, vendorpid, item.site_id, item.site_name);
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

        #region 修改建議售價、寄倉費+ HttpResponseBase UpdatePrice()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase UpdatePrice()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp pro = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Form["ProductId"].ToString(), Create_Channel = 2 }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Request.Form["product_price_list"]))
                    {
                        pro.Product_Price_List = uint.Parse(Request.Form["product_price_list"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["bag_check_money"]))
                    {
                        pro.Bag_Check_Money = uint.Parse(Request.Form["bag_check_money"]);
                    }
                    pro.show_listprice = Convert.ToUInt32((Request.Form["show_listprice"] ?? "") == "on" ? 1 : 0);
                    pro.Combo_Type = COMBO_TYPE;
                    bool result = _productTempMgr.UpdateAchieve(pro);

                    json = "{success:" + result.ToString().ToLower() + "}";
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

        #region 類別界面

        #region 獲取選擇過得類別
        [HttpPost]
        [CustomHandleError]
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

                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writer_id = Convert.ToInt32(vendorModel.vendor_id);

                _productTempMgr = new ProductTempMgr(connectionString);
                ProductTemp query = new ProductTemp { Writer_Id = writer_id, Combo_Type = COMBO_TYPE, Product_Id = vendor_pid };

                ProductTemp tempResult = _productTempMgr.GetVendorProTemp(query);
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
        #endregion

        #region 保存類別 +HttpResponseBase tempCategoryAdd()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase tempCategoryAdd()
        {


            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writd_Id = Convert.ToInt32(vendorModel.vendor_id);

            string resultStr = "{success:true}";
            string tempStr = Request.Params["result"];
            string cate_id = Request.Params["cate_id"];


            List<ProductCategorySetTemp> saveTempList = new List<ProductCategorySetTemp>();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductCategorySetCustomTemp> cateCustomList = js.Deserialize<List<ProductCategorySetCustomTemp>>(tempStr);
                if (string.IsNullOrEmpty(tempStr))
                {
                    cateCustomList = new List<ProductCategorySetCustomTemp>();
                }

                string vendor_pId = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    vendor_pId = Request.Params["ProductId"];

                    //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    //{
                    //    vendor_pId = Request.Form["OldProductId"];
                    //}
                    _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
                    _productTempMgr = new ProductTempMgr(connectionString);
                    if (string.IsNullOrEmpty(tempStr))
                    {
                        bool result = _categoryTempSetMgr.DeleteByVendor(new ProductCategorySetTemp { Writer_Id = writd_Id, Combo_Type = COMBO_TYPE, Product_Id = vendor_pId });
                    }
                    else
                    {
                        ProductCategorySetTemp saveTemp;
                        ProductTemp query = new ProductTemp { Writer_Id = writd_Id, Combo_Type = COMBO_TYPE, Product_Id = vendor_pId };
                        ProductTemp proTemp = _productTempMgr.GetVendorProTemp(query);
                        foreach (ProductCategorySetCustomTemp item in cateCustomList)
                        {
                            saveTemp = new ProductCategorySetTemp();
                            saveTemp.Writer_Id = writd_Id;
                            saveTemp.Product_Id = vendor_pId;
                            saveTemp.Category_Id = item.Category_Id;
                            saveTemp.Brand_Id = proTemp.Brand_Id;
                            saveTemp.Combo_Type = COMBO_TYPE;
                            saveTempList.Add(saveTemp);
                        }

                        if (!_categoryTempSetMgr.SaveByVendor(saveTempList))
                        {
                            resultStr = "{success:false}";
                        }
                    }

                    if (!_productTempMgr.CategoryInfoUpdateByVendor(new ProductTemp { Writer_Id = writd_Id, Cate_Id = cate_id, Combo_Type = COMBO_TYPE, Product_Id = vendor_pId }))
                    {
                        resultStr = "{success:false}";
                    }
                }
            }

            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{success:false}";
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        #endregion


        #region 得到前台分類 +HttpResponseBase GetCatagory()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetCatagory()
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
                string Ven_product_id = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    Ven_product_id = Request.Params["ProductId"];
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    Ven_product_id = Request.Form["OldProductId"];
                //}
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writd_Id = Convert.ToInt32(vendorModel.vendor_id);

                ProductCategorySetTemp query = new ProductCategorySetTemp { Writer_Id = writd_Id, Combo_Type = COMBO_TYPE, Product_Id = Ven_product_id };
                _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);

                TempresultList = (from c in _categoryTempSetMgr.QueryByVendor(query)
                                  select new
                                  {
                                      Id = c.Id,
                                      Product_Id = c.Product_Id,
                                      Category_Id = c.Category_Id,
                                      Brand_Id = c.Brand_Id
                                  }).ToList().ConvertAll<ProductCategorySetTemp>(m => new ProductCategorySetTemp()
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
                    if (cateListResult[0].children[i].id.ToString() == "5")
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
        #endregion

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
        #region 獲取category類別 +List<ProductCategoryCustom> getCate(List<ProductCategory> categoryList, uint fatherId)
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
        #endregion
        #endregion

        #region 庫存

        #region 查詢庫存信息+string QueryStock()
        [HttpPost]
        [CustomHandleError]
        public string QueryStock()
        {
            string json = string.Empty;
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writd_Id = Convert.ToInt32(vendorModel.vendor_id);

            ProductItemTemp query = new ProductItemTemp();
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                query.Product_Id = Request.Params["product_id"].ToString();
            }
            query.Writer_Id = writd_Id;

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

        #region  圖檔
        [HttpPost]
        public string QueryExplainPic()
        {
            string json = string.Empty;
            //查找臨時表記錄
            _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writd_Id = Convert.ToInt32(vendorModel.vendor_id);


            ProductPictureTemp temp = new ProductPictureTemp { writer_Id = writd_Id, combo_type = COMBO_TYPE };
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                temp.product_id = Request.Params["product_id"].ToString();
            }
            //if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))
            //{
            //    temp.product_id = Request.Params["OldProductId"].ToString();
            //}
            if (!string.IsNullOrEmpty(Request.Params["isEdit"]) && Request.Params["isEdit"].ToString() == "true")
            {
                List<ProductPictureTemp> picList = _pPicTempMgr.Query(temp);
                foreach (var item in picList)
                {
                    if (item.image_filename != "")
                    {
                        item.image_filename = imgServerPath + descPath + GetDetailFolder(item.image_filename) + item.image_filename;
                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(picList) + "}";
            }
            else
            {

            }
            json = json.Replace("image_filename", "img");
            return json;
        }

        [HttpPost]
        public HttpResponseBase DeletePic()
        {
            string json = "{success:true,msg:\"" + Resources.VendorProduct.DELETE_SUCCESS + "\"}";
            string deleteType = Request.Params["type"];
            ProductSpecTemp psTemp = new ProductSpecTemp();
            ProductSpec pSpec = new ProductSpec();
            List<ProductSpecTemp> psList = new List<ProductSpecTemp>();

            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writd_Id = Convert.ToInt32(vendorModel.vendor_id);


            string[] record = Request.Params["rec"].Split(',');
            string fileName = record[0].Split('/')[imgNameIdx];


            if (deleteType == "desc")
            {
                string imageName = imgLocalPath + descPath + GetDetailFolder(fileName) + fileName;
                string imageName400 = imgLocalPath + desc400Path + GetDetailFolder(fileName) + fileName;
                DeletePicFile(imageName);
                DeletePicFile(imageName400);
            }

            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                psTemp.Writer_Id = writd_Id;

                try
                {
                    _specTempMgr.Update(psList, "image");
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:true,msg:\"" + Resources.VendorProduct.DELETE_SPEC_FAIL + "\"}";
                }
            }
            else
            {
                pSpec.product_id = uint.Parse(Request.Params["product_id"]);

                try
                {
                    _specMgr.UpdateSingle(pSpec);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:true,msg:\"" + Resources.VendorProduct.DELETE_SPEC_FAIL + "\"}";
                }
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [HttpPost]
        public HttpResponseBase productPictrueTempSave()
        {

            string json = "{success:true}";
            ProductTemp pTemp = new ProductTemp();
            _productTempMgr = new ProductTempMgr(connectionString);
            _specTempMgr = new ProductSpecTempMgr(connectionString);

            _productPicMgr = new ProductPictureMgr(connectionString);

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writd_Id = Convert.ToInt32(vendorModel.vendor_id);


            _specMgr = new ProductSpecMgr(connectionString);
            if (!string.IsNullOrEmpty(Request.Params["image_InsertValue"])) pTemp.Product_Image = Request.Params["image_InsertValue"];
            if (!string.IsNullOrEmpty(Request.Params["productMedia"])) pTemp.product_media = Request.Params["productMedia"];
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                pTemp.Product_Id = Request.Params["product_id"].ToString();
            }
            pTemp.Writer_Id = writd_Id;
            pTemp.Combo_Type = COMBO_TYPE;

            ProductSpecTemp pSpec = new ProductSpecTemp();
            List<ProductSpecTemp> pSpecList = new List<ProductSpecTemp>();
            if (!string.IsNullOrEmpty(Request.Params["spec_InsertValue"]))
            {
                string[] Values = Request.Form["spec_InsertValue"].ToString().Split(';');
                for (int i = 0; i < Values.Length - 1; i++)
                {
                    pSpec = new ProductSpecTemp();
                    pSpec.Writer_Id = writd_Id;
                    string[] perValue = Values[i].Split(',');
                    if (!string.IsNullOrEmpty(perValue[0])) { pSpec.spec_image = perValue[0]; };
                    if (!string.IsNullOrEmpty(perValue[1])) { pSpec.spec_id = uint.Parse(perValue[1]); };
                    if (!string.IsNullOrEmpty(perValue[2])) { pSpec.spec_sort = uint.Parse(perValue[2]); };
                    if (!string.IsNullOrEmpty(perValue[3])) { pSpec.spec_status = uint.Parse(perValue[3]); };
                    pSpecList.Add(pSpec);
                }
            }

            List<ProductPictureTemp> picList = new List<ProductPictureTemp>();
            _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
            ProductPictureTemp pPic = new ProductPictureTemp();
            if (!string.IsNullOrEmpty(Request.Params["picture_InsertValue"]))
            {
                string[] Values = Request.Form["picture_InsertValue"].ToString().Split(';');
                for (int i = 0; i < Values.Length - 1; i++)
                {
                    pPic = new ProductPictureTemp();
                    string[] perValue = Values[i].Split(',');
                    pPic.combo_type = COMBO_TYPE;
                    pPic.writer_Id = writd_Id;
                    pPic.image_createdate = Convert.ToInt32(CommonFunction.GetPHPTime());
                    pPic.product_id  = pTemp.Product_Id;
                    if (!string.IsNullOrEmpty(perValue[0])) { pPic.image_filename = perValue[0]; };
                    if (!string.IsNullOrEmpty(perValue[1])) { pPic.image_sort = uint.Parse(perValue[1]); };
                    if (!string.IsNullOrEmpty(perValue[2])) { pPic.image_state = uint.Parse(perValue[2]); };
                    picList.Add(pPic);
                }
            }

            try
            {

                //保存至productTemp
                if (pTemp.Product_Image != "" || pTemp.product_media != "")
                {
                    _productTempMgr.ProductTempUpdateByVendor(pTemp, "pic");
                }
                //保存規格圖
                _specTempMgr.Update(pSpecList, "image");
                //保存說明圖
                //int oldProductId = 0;
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    oldProductId = int.Parse(Request.Form["OldProductId"]);
                //}
                ProductPictureTemp proPictureTemp = new ProductPictureTemp { writer_Id = pTemp.Writer_Id, combo_type = pTemp.Combo_Type, product_id = pTemp.Product_Id };
                _pPicTempMgr.Save(picList, proPictureTemp);
                //_pPicTempMgr.Save(picList, new ProductPictureTemp() { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = oldProductId }); edite by wangwei0216w 註釋掉_pPicTempMgr.Save 以解決複製后不能讀取圖片路勁到數據庫
                json = "{success:true,msg:\"" + Resources.VendorProduct.SAVE_SUCCESS + "\"}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + Resources.VendorProduct.EDIT_FAIL + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public void CreateFolder(string path, string[] Mappath)
        {
            FTP ftp = null;
            try
            {
                string fullPath = path;
                foreach (string s in Mappath)
                {
                    fullPath += s;
                    ftp = new FTP(path, ftpuser, ftppwd);
                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath, ftpuser, ftppwd);
                        ftp.MakeDirectory();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }

        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }

        [HttpPost]
        public string QuerySpecPic()
        {
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);

            string json = string.Empty;

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];


            //查找臨時表記錄
            ProductSpecTemp psTemp = new ProductSpecTemp();
            psTemp.Writer_Id = Convert.ToInt32(vendorModel.vendor_id);
            psTemp.spec_type = 1;
            //string str = "{success:true,items:"+JsonConvert.+"}";
            List<ProductSpecTemp> results = _specTempMgr.Query(psTemp); //JsonConvert.SerializeObject();
            foreach (var item in results)
            {
                if (item.spec_image != "")
                {
                    item.spec_image = imgServerPath + specPath + GetDetailFolder(item.spec_image) + item.spec_image;
                }

            }
            json = "{success:true,items:" + JsonConvert.SerializeObject(results) + "}";
            json = json.Replace("spec_image", "img");
            return json;
        }


        #endregion
        #region 點擊儲存保存數據至臨時表+HttpResponseBase SaveTemp()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveTemp()
        {
            string json = string.Empty;
            string product_id = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];

                int writerId = Convert.ToInt32(vendorModel.vendor_id);
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    product_id = Request.Form["ProductId"];
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    product_id = Request.Form["OldProductId"];
                //}
                ProductTemp proTemp = new ProductTemp();
                proTemp.Writer_Id = writerId;
                proTemp.Combo_Type = 2;
                proTemp.Product_Id = product_id;
                _productTempMgr = new ProductTempMgr(connectionString);
                if (_productTempMgr.SaveTemp(proTemp))
                {
                    json = "{success:true,msg:'" + Resources.VendorProduct.SAVE_SUCCESS + "'}";
                }
                else
                {
                    json = "{success:false,msg:'" + Resources.VendorProduct.SAVE_FAIL + "'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + Resources.VendorProduct.SAVE_FAIL + "'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
    }
}
