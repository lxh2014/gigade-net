
#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductController.cs      
* 摘 要：                                                                               
* 單一商品新增
* 当前版本：v1.0                                                                
* 作 者： shuangshuang0420j                                           
* 完成日期：
* 
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using System.Configuration;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using Vendor.CustomHandleError;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Collections;
using System.Web.Security;
using System.Net.Mail;
using Newtonsoft.Json;



namespace Vendor.Controllers
{
    [HandleError]
    public class VendorProductController : Controller
    {
        //
        // GET: /Product/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private ParameterMgr paraMgr;
        private IProductTempImplMgr _productTempMgr;
        private IProductImplMgr _product;
        private IProductImplMgr _productMgr;
        private IProductPictureTempImplMgr _pPicTempMgr;
        private IProductPictureImplMgr _pPicMgr;
        private IProductSpecTempImplMgr _specTempMgr;
        private IProductTagImplMgr _productTagMgr;
        private IProductCategoryImplMgr _procateMgr;
        private IProductCategorySetImplMgr _categorySetMgr;
        private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetTempImplMgr _productNoticeSetTempMgr;
        private IProductTagSetTempImplMgr _productTagSetTempMgr;
        private IProductSpecImplMgr _specMgr;
        private ISerialImplMgr _serialMgr;
        private IProductItemTempImplMgr _productItemTempMgr;
        private IProductItemImplMgr _productItemMgr;
        private IPriceMasterTempImplMgr _priceMasterTempMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IItemPriceTempImplMgr _itemPriceTempMgr;
        private ISiteConfigImplMgr siteConfigMgr;

        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        //string noticePath = ConfigurationManager.AppSettings["prod_noticePath"];
        //string notice400Path = ConfigurationManager.AppSettings["prod_notice400Path"];
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];
        private int COMBO_TYPE = 1;//單一商品
        private int defaultImgLength = 5;
        private int imgNameIdx = 7;    //按‘/’分割第 n 个为图片名称

        #region 圖片屬性


        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);

        string default50Path = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";

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


        #endregion

        #region 視圖
        public ActionResult ProductSave(string id)
        {
            ViewBag.ProductId = id;//獲取編輯時傳遞過來的id
            ViewBag.OldProductId = Request.QueryString["product_id"] ?? "";//獲取商品複製時傳遞過來的id
            ViewBag.hfAuth = Request.Cookies[FormsAuthentication.FormsCookieName] == null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value;
            ViewBag.hfAspSessID = Session.SessionID;
            //獲取複製商品后新產生的product_id
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                ViewBag.ProductId = Request.QueryString["id"] ?? null;
            }
            if (string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                ViewBag.isEdit = "false";
            }
            else
            {
                ViewBag.isEdit = "true";
            }
            return View();
        }
        public ActionResult baseInfo()
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
        public ActionResult ProductCategory()
        {
            return View();
        }
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
        public ActionResult SpecIndex()
        {
            return View();
        }
        public ActionResult PriceIndex()
        {
            return View();
        }
        public ActionResult ProductPic()
        {
            return View();
        }
        public ActionResult productStock()
        {
            return View();
        }
        public ActionResult upLoad()
        {
            return View();
        }

        #endregion

        #region 查詢商品信息+HttpResponseBase QueryProduct()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProduct()
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            string json = string.Empty;
            string prodTempID = string.Empty;
            int writerId = (int)vendorModel.vendor_id;
            try
            {
                _productTempMgr = new ProductTempMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))//非新增時
                {
                    prodTempID = Request.Form["ProductId"].ToString();
                }
                ProductTemp queryProdTemp = new ProductTemp();
                queryProdTemp.Product_Id = prodTempID;
                queryProdTemp.Writer_Id = writerId;
                if (string.IsNullOrEmpty(queryProdTemp.Product_Id))
                {
                    queryProdTemp.Temp_Status = 11;
                }
                queryProdTemp.Create_Channel = 2;
                if (!string.IsNullOrEmpty(prodTempID))
                {
                    ProductTemp productT = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Params["ProductId"] }).FirstOrDefault();
                    if (productT != null)
                    {
                        queryProdTemp.Combo_Type = productT.Combo_Type;
                    }
                    else
                    {
                        queryProdTemp.Combo_Type = COMBO_TYPE;
                    }
                }
                else
                {
                    queryProdTemp.Combo_Type = COMBO_TYPE;
                }
                uint pid = 0;
                if (uint.TryParse(Request.Params["ProductId"], out pid))
                {
                    #region 正式表
                    Product product = new Product();
                    product.Product_Id = Convert.ToUInt32(Request.Params["ProductId"]);
                    _product = new ProductMgr(connectionString);
                    Product prod = _product.Query(product).FirstOrDefault();
                    if (prod != null)
                    {
                        if (!string.IsNullOrEmpty(prod.Product_Image))
                        {
                            prod.Product_Image = imgServerPath + prodPath + GetDetailFolder(prod.Product_Image) + prod.Product_Image;
                        }
                        else
                        {
                            prod.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                        }
                    }
                    json = "{success:true,data:" + JsonConvert.SerializeObject(prod) + "}";
                    #endregion
                }
                else
                {
                    #region 供應商 臨時表
                    ProductTemp prodTemp = _productTempMgr.GetProTempByVendor(queryProdTemp).FirstOrDefault();
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
                    #endregion
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

        #region 點擊儲存保存數據至臨時表+HttpResponseBase SaveTemp()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveTemp()
        {
            string json = string.Empty;
            string product_id = string.Empty;
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            try
            {
                int writerId = (int)vendorModel.vendor_id;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    product_id = Request.Form["ProductId"];
                }
                ProductTemp proTemp = new ProductTemp();
                proTemp.Writer_Id = writerId;
                proTemp.Combo_Type = COMBO_TYPE;
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

        #region  保存商品基本信息+HttpResponseBase SaveBaseInfo()

        [HttpPost]
        public HttpResponseBase SaveBaseInfo()
        {
            ProductTemp pTemp = new ProductTemp();//儲存前台輸入的數據
            _productTempMgr = new ProductTempMgr(connectionString);
            string json = "{success:true}";
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];

            uint brand_id = uint.Parse(Request.Form["brand_id"]);
            uint product_sort = uint.Parse(Request.Form["product_sort"]);
            uint product_start = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_start"]).ToString());
            uint product_end = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_end"]).ToString());
            uint expect_time = uint.Parse(CommonFunction.GetPHPTime(Request.Form["expect_time"]).ToString());
            uint product_freight_set = uint.Parse(Request.Form["product_freight_set"]);
            uint product_mode = uint.Parse(Request.Form["product_mode"]);
            string product_name = Request.Form["product_name"];
            string product_vendor_code = Request.Form["product_vendor_code"];
            int tax_type = int.Parse(Request.Form["tax_type"]);
            string expect_msg = Request.Form["expect_msg"] ?? "";

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
            pTemp.expect_msg = expect_msg;
            pTemp.Combo_Type = COMBO_TYPE;
            pTemp.Product_Ipfrom = Request.UserHostAddress;//获取当前ip

            if (string.IsNullOrEmpty(Request.Params["product_id"]))//新增
            {
                //查找臨時表是否存在數據，存在：更新，不存在插入
                pTemp.Writer_Id = (int)vendorModel.vendor_id;
                pTemp.Product_Status = 20;
                pTemp.Combo_Type = COMBO_TYPE;
                pTemp.Temp_Status = 11;
                pTemp.Create_Channel = 2;
                pTemp.Product_Createdate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                pTemp.Product_Updatedate = pTemp.Product_Createdate;
                ProductTemp pTempList = null;
                if (pTempList == null)
                {
                    //插入
                    string result = string.Empty;
                    try
                    {
                        result = _productTempMgr.vendorBaseInfoSave(pTemp);
                        if (result != null)
                        {
                            json = "{success:true,rid:'" + result + "'}";
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
                        json = "{success:false,msg:'" + Resources.VendorProduct.ADD_FAIL + "'}";
                    }
                }
                else
                {
                    pTemp.Temp_Status = 11;
                    //更新
                    if (pTemp.Product_Mode != 2)
                    {
                        pTemp.Bag_Check_Money = 0;
                    }
                    else
                    {
                        pTemp.Bag_Check_Money = pTempList.Bag_Check_Money;
                    }
                    try
                    {
                        _productTempMgr.vendorBaseInfoUpdate(pTemp);
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
            }
            else//編輯臨時表數據
            {
                ProductTemp prodTemp = new ProductTemp();//儲存查到的數據
                //product_id存在時表修改查詢product表。
                prodTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Params["product_id"].ToString(), Writer_Id = (int)vendorModel.vendor_id, Create_Channel = 2 }).FirstOrDefault();
                prodTemp.Brand_Id = brand_id;
                prodTemp.Product_Name = product_name;
                prodTemp.Product_Sort = product_sort;
                prodTemp.Product_Vendor_Code = product_vendor_code;
                prodTemp.Product_Start = product_start;
                prodTemp.Product_End = product_end;
                prodTemp.Expect_Time = expect_time;
                prodTemp.Product_Freight_Set = product_freight_set;
                prodTemp.Product_Mode = product_mode;
                if (product_mode != 2)
                {
                    prodTemp.Bag_Check_Money = 0;
                }
                prodTemp.Tax_Type = tax_type;
                prodTemp.expect_msg = expect_msg;
                prodTemp.Writer_Id = (int)vendorModel.vendor_id;
                prodTemp.Product_Status =20;
                prodTemp.Combo_Type = COMBO_TYPE;
                //prodTemp.Temp_Status = 12;//修改时商品状态都为供应商新建
                prodTemp.Create_Channel = 2;
                prodTemp.Product_Ipfrom = Request.UserHostAddress;//获取当前ip
                prodTemp.Product_Updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                int result = 0;
                try
                {
                    result = _productTempMgr.vendorBaseInfoUpdate(prodTemp);
                    if (result == 1)
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
                    json = "{success:false,msg:'" + Resources.VendorProduct.EDIT_FAIL + "'}";
                }
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除臨時數據+HttpResponseBase DeleteTempPro()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase DeleteTempPro()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];

                int writerId = (int)vendorModel.vendor_id;// (Session["caller"] as Caller).user_id;
                string product_id = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    product_id = Request.Params["ProductId"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    product_id = Request.Form["OldProductId"].ToString();
                //}

                _productTempMgr = new ProductTempMgr(connectionString);
                //刪除服務器上對應的圖片
                DeletePicOnServer(true, true, true, 0, product_id);
                if (_productTempMgr.DeleteVendorProductTemp(writerId, COMBO_TYPE, product_id == string.Empty ? "0" : product_id))
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
        /// <summary>
        /// 刪除服務器上的圖片
        /// </summary>
        /// <param name="prod">商品圖是否刪除</param>
        /// <param name="spec">規格圖是否是否刪除</param>
        /// <param name="desc">說明圖是否刪除</param>
        /// <param name="spec_id">規格id</param>
        /// <param name="product_id">商品id</param>
        public void DeletePicOnServer(bool prod, bool spec, bool desc, uint spec_id, string product_id)
        {
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerId = (int)vendorModel.vendor_id;
                _productTempMgr = new ProductTempMgr(connectionString);
                _specTempMgr = new ProductSpecTempMgr(connectionString);
                _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
                ArrayList ImgList = new ArrayList();


                //刪除對應的圖片
                //商品圖
                if (prod)
                {
                    ProductTemp query = new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE, Create_Channel = 2, Product_Id = product_id };
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
                    ProductSpecTemp pSpec = new ProductSpecTemp();
                    pSpec.Writer_Id = writerId;
                    pSpec.spec_type = 1;
                    if (spec_id != 0)
                    {
                        pSpec.spec_id = spec_id;
                    }
                    if (!string.IsNullOrEmpty(product_id))
                    {
                        pSpec.product_id = product_id;
                    }
                    List<ProductSpecTemp> pSList = _specTempMgr.VendorQuery(pSpec);
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
                    ProductPictureTemp query = new ProductPictureTemp { writer_Id = writerId, combo_type = COMBO_TYPE, product_id = product_id };
                    List<ProductPictureTemp> pPList = _pPicTempMgr.VendorQuery(query);
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
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion



        #region /******************供應商商品描述*********************/

        #region 商品標籤+HttpResponseBase GetProTag()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProTag()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                StringBuilder strJson = new StringBuilder("");
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
                    ptstQuery.Writer_Id = (int)vendorModel.vendor_id;
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
                //獲取當前登入的供應商
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                StringBuilder strJson = new StringBuilder("");
                _productNoticeMgr = new ProductNoticeMgr(connectionString);
                List<ProductNotice> notices = _productNoticeMgr.Query(new ProductNotice { notice_status = 1 });
                if (notices != null)
                {
                    _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connectionString);
                    ProductNoticeSetTemp queryProductNotice = new ProductNoticeSetTemp();
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        string productId = Request.Form["ProductId"].ToString();
                        queryProductNotice.product_id = productId;
                    }
                    //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    //{
                    //    queryProductNotice.product_id = Request.Form["OldProductId"].ToString();
                    //}
                    queryProductNotice.Writer_Id = (int)vendorModel.vendor_id;
                    queryProductNotice.Combo_Type = COMBO_TYPE;
                    List<ProductNoticeSetTemp> noticeSets = _productNoticeSetTempMgr.QueryVendorProdNotice(queryProductNotice);
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp proTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Writer_Id = (int)vendorModel.vendor_id, Combo_Type = COMBO_TYPE, Product_Id = queryProductNotice.product_id, Create_Channel = 2 }).FirstOrDefault();
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

        #region 保存商品描述 標籤 公告

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveDescription()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writeId = (int)vendorModel.vendor_id;
                string tags = Request.Form["Tags"] ?? "";
                string notices = Request.Form["Notice"] ?? "";
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                _productTempMgr = new ProductTempMgr(connectionString);
                string product_id = string.Empty;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    product_id = Request.Form["ProductId"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    product_id = Request.Form["OldProductId"].ToString();
                //}
                if (!string.IsNullOrEmpty(product_id))
                {
                    ProductTemp oldProdTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = product_id, Combo_Type = COMBO_TYPE, Create_Channel = 2, Writer_Id = writeId }).FirstOrDefault();
                    if (oldProdTemp != null)
                    {
                        oldProdTemp.Page_Content_1 = Request.Form["page_content_1"] ?? "";
                        oldProdTemp.Page_Content_2 = Request.Form["page_content_2"] ?? "";
                        oldProdTemp.Page_Content_3 = Request.Form["page_content_3"] ?? "";
                        oldProdTemp.Product_Keywords = Request.Form["product_keywords"] ?? "";
                        if (!string.IsNullOrEmpty(Request.Form["product_buy_limit"]))
                        {
                            oldProdTemp.Product_Buy_Limit = uint.Parse(Request.Form["product_buy_limit"]);
                        }
                        oldProdTemp.Writer_Id = writeId;
                        oldProdTemp.Combo_Type = COMBO_TYPE;

                        List<ProductTagSetTemp> tagTemps = jsSer.Deserialize<List<ProductTagSetTemp>>(tags);
                        foreach (ProductTagSetTemp item in tagTemps)
                        {
                            item.Writer_Id = writeId;
                            item.Combo_Type = COMBO_TYPE;
                            item.product_id = oldProdTemp.Product_Id;
                        }
                        List<ProductNoticeSetTemp> noticeTemps = jsSer.Deserialize<List<ProductNoticeSetTemp>>(notices);
                        //noticeTemps.ForEach(m => m.Writer_Id = writer_id);
                        foreach (ProductNoticeSetTemp item in noticeTemps)
                        {
                            item.Writer_Id = writeId;
                            item.Combo_Type = COMBO_TYPE;
                            item.product_id = oldProdTemp.Product_Id;
                        }
                        _productTempMgr = new ProductTempMgr(connectionString);
                        if (_productTempMgr.VendorDescriptionInfoUpdate(oldProdTemp, tagTemps, noticeTemps))
                        {
                            json = "{success:true}";
                        }
                        else
                        {
                            json = "{success:false}";
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion

        #region /************************    規格   *************************/

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

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase specTempSave()
        {
            string resultStr = "{success:true}";
            bool result = true;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;

                string specType = Request.Params["specType"];
                string spec1Name = Request.Params["spec1Name"];
                string spec1Result = Request.Params["spec1Result"];
                string spec2Name = Request.Params["spec2Name"];
                string spec2Result = Request.Params["spec2Result"];

                _specTempMgr = new ProductSpecTempMgr(connectionString);
                _productTempMgr = new ProductTempMgr(connectionString);
                _productItemTempMgr = new ProductItemTempMgr(connectionString);
                _serialMgr = new SerialMgr(connectionString);
                #region 臨時表修改
                string proId = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    proId = Request.Params["ProductId"].ToString();

                }
                if (!string.IsNullOrEmpty(proId))
                {
                    if (!specType.Equals("0") && !string.IsNullOrEmpty(specType))
                    {
                        #region 有規格
                        List<ProductSpecTemp> spec1List;
                        List<ProductSpecTemp> spec2List;
                        List<ProductSpecTemp> specAllList = new List<ProductSpecTemp>();

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        spec1List = jss.Deserialize<List<ProductSpecTemp>>(spec1Result);

                        foreach (ProductSpecTemp item in spec1List)
                        {
                            item.Writer_Id = writerID;// item.Writer_Id = _caller.user_id;
                            item.product_id = proId;
                            item.spec_type = 1;
                            item.spec_image = "";
                            specAllList.Add(item);
                        }

                        if (specType.Equals("2"))
                        {

                            spec2List = jss.Deserialize<List<ProductSpecTemp>>(spec2Result);

                            foreach (ProductSpecTemp item in spec2List)
                            {
                                item.Writer_Id = writerID;// item.Writer_Id = _caller.user_id;
                                item.product_id = proId;
                                item.spec_type = 2;
                                item.spec_image = "";
                                specAllList.Add(item);
                            }
                        }

                        List<ProductSpecTemp> tempList = _specTempMgr.VendorQuery(new ProductSpecTemp { Writer_Id = writerID, product_id = proId });

                        if (tempList == null || tempList.Count() <= 0)
                        {
                            #region 保存

                            specAllList.ForEach(p => p.spec_id = uint.Parse(_serialMgr.NextSerial(18).ToString()));

                            bool saveSpecResult = _specTempMgr.SaveByVendor(specAllList);

                            if (saveSpecResult)
                            {
                                _productItemTempMgr.DeleteByVendor(new ProductItemTemp { Writer_Id = writerID, Product_Id = proId });
                                #region 保存ProductItemTemp

                                List<ProductSpecTemp> specAllResultList = _specTempMgr.VendorQuery(new ProductSpecTemp { Writer_Id = writerID, product_id = proId });
                                List<ProductSpecTemp> spec1ResultList = specAllResultList.Where(m => m.spec_type == 1).ToList();
                                List<ProductSpecTemp> spec2ResultList = specAllResultList.Where(m => m.spec_type == 2).ToList();

                                List<ProductItemTemp> saveItemList = new List<ProductItemTemp>();

                                if (specType.Equals("1"))
                                {
                                    foreach (ProductSpecTemp specTemp1 in spec1ResultList)
                                    {
                                        ProductItemTemp itemTemp = new ProductItemTemp();
                                        itemTemp.Writer_Id = writerID;
                                        itemTemp.Product_Id = proId;
                                        itemTemp.Spec_Id_1 = specTemp1.spec_id;
                                        itemTemp.Item_Stock = 10;
                                        itemTemp.Item_Alarm = 1;
                                        saveItemList.Add(itemTemp);

                                    }
                                }
                                else if (specType.Equals("2"))
                                {
                                    foreach (ProductSpecTemp specTemp1 in spec1ResultList)
                                    {
                                        foreach (ProductSpecTemp specTemp2 in spec2ResultList)
                                        {
                                            ProductItemTemp itemTemp = new ProductItemTemp();
                                            itemTemp.Writer_Id = writerID;
                                            itemTemp.Product_Id = proId;
                                            itemTemp.Spec_Id_1 = specTemp1.spec_id;
                                            itemTemp.Spec_Id_2 = specTemp2.spec_id;
                                            itemTemp.Item_Stock = 10;
                                            itemTemp.Item_Alarm = 1;
                                            itemTemp.Item_Code = "";
                                            itemTemp.Barcode = "";
                                            saveItemList.Add(itemTemp);
                                        }
                                    }
                                }

                                bool saveItemResult = _productItemTempMgr.SaveByVendor(saveItemList);

                                if (!saveItemResult)
                                {
                                    result = false;
                                }

                                #endregion

                            }
                            else
                            {
                                result = false;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 更新

                            string strSpecInit = Request.Params["specInit"];

                            string[] specs = strSpecInit.Trim().Split(',');
                            List<ProductSpecTemp> addList = specAllList.Where(p => p.spec_id == 0).ToList();
                            if (addList.Count() > 0)
                            {
                                addList.ForEach(p => p.spec_id = uint.Parse(_serialMgr.NextSerial(18).ToString()));

                                List<ProductSpecTemp> specAllResultList = _specTempMgr.VendorQuery(new ProductSpecTemp { Writer_Id = writerID, product_id = proId });
                                List<ProductSpecTemp> spec1ResultList = specAllResultList.Where(m => m.spec_type == 1).ToList();
                                List<ProductSpecTemp> spec2ResultList = specAllResultList.Where(m => m.spec_type == 2).ToList();
                                List<ProductItemTemp> saveItemList = new List<ProductItemTemp>();
                                foreach (ProductSpecTemp item in addList)
                                {
                                    if (specType.Equals("1"))
                                    {
                                        if (item.spec_type == 1)
                                        {
                                            ProductItemTemp saveTemp = new ProductItemTemp();
                                            saveTemp.Writer_Id = writerID;
                                            saveTemp.Spec_Id_1 = item.spec_id;
                                            saveTemp.Product_Id = proId;
                                            saveTemp.Item_Stock = 10;
                                            saveTemp.Item_Alarm = 1;
                                            saveItemList.Add(saveTemp);
                                        }
                                        else
                                        {
                                            ProductItemTemp saveTemp = new ProductItemTemp();
                                            saveTemp.Writer_Id = writerID;
                                            saveTemp.Spec_Id_2 = item.spec_id;
                                            saveTemp.Product_Id = proId;
                                            saveTemp.Item_Stock = 10;
                                            saveTemp.Item_Alarm = 1;
                                            saveItemList.Add(saveTemp);
                                        }
                                    }
                                    else
                                    {
                                        if (item.spec_type == 1)
                                        {
                                            foreach (ProductSpecTemp item1 in spec2ResultList)
                                            {
                                                ProductItemTemp saveTemp = new ProductItemTemp();
                                                saveTemp.Writer_Id = writerID;
                                                saveTemp.Spec_Id_1 = item.spec_id;
                                                saveTemp.Spec_Id_2 = item1.spec_id;
                                                saveTemp.Item_Stock = 10;
                                                saveTemp.Item_Alarm = 1;
                                                saveTemp.Product_Id = proId;
                                                saveItemList.Add(saveTemp);
                                            }

                                            foreach (ProductSpecTemp item1 in addList.Where(p => p.spec_type == 2).ToList())
                                            {
                                                ProductItemTemp saveTemp = new ProductItemTemp();
                                                saveTemp.Writer_Id = writerID;
                                                saveTemp.Spec_Id_1 = item.spec_id;
                                                saveTemp.Spec_Id_2 = item1.spec_id;
                                                saveTemp.Item_Stock = 10;
                                                saveTemp.Item_Alarm = 1;
                                                saveTemp.Product_Id = proId;
                                                saveItemList.Add(saveTemp);
                                            }
                                        }
                                        else
                                        {
                                            foreach (ProductSpecTemp item2 in spec1ResultList)
                                            {
                                                ProductItemTemp saveTemp = new ProductItemTemp();
                                                saveTemp.Writer_Id = writerID;
                                                saveTemp.Spec_Id_1 = item2.spec_id;
                                                saveTemp.Spec_Id_2 = item.spec_id;
                                                saveTemp.Product_Id = proId;
                                                saveTemp.Item_Stock = 10;
                                                saveTemp.Item_Alarm = 1;
                                                saveItemList.Add(saveTemp);
                                            }

                                        }
                                    }
                                }
                                _specTempMgr.SaveByVendor(addList);
                                _productItemTempMgr.SaveByVendor(saveItemList);

                            }

                            if (specs.Length > 0 && !string.IsNullOrEmpty(strSpecInit.Trim()))
                            {
                                List<ProductSpecTemp> updateList = new List<ProductSpecTemp>();
                                foreach (string initSpecId in specs)
                                {
                                    ProductSpecTemp nowItem = specAllList.Where(p => p.spec_id == uint.Parse(initSpecId)).FirstOrDefault();
                                    if (nowItem != null)
                                    {
                                        updateList.Add(nowItem);
                                    }
                                    else
                                    {

                                        ProductItemTemp delTemp = new ProductItemTemp { Writer_Id = writerID, Product_Id = proId };
                                        uint spectype = _specTempMgr.VendorQuery(new ProductSpecTemp { spec_id = uint.Parse(initSpecId), product_id = proId })[0].spec_type;
                                        if (spectype == 1)
                                        {
                                            delTemp.Spec_Id_1 = uint.Parse(initSpecId);
                                        }
                                        else if (spectype == 2)
                                        {
                                            delTemp.Spec_Id_2 = uint.Parse(initSpecId);
                                        }
                                        if (!_productItemTempMgr.DeleteByVendor(delTemp))
                                        {
                                            result = false;
                                        }
                                        if (!_specTempMgr.DeleteByVendor(new ProductSpecTemp { spec_id = uint.Parse(initSpecId), Writer_Id = writerID, product_id = proId }))
                                        {
                                            result = false;
                                        }
                                        DeletePicOnServer(false, true, false, uint.Parse(initSpecId), Convert.ToString(proId));
                                    }

                                }
                                if (!_specTempMgr.Update(updateList, "spec"))
                                {
                                    result = false;
                                }
                            }

                            #endregion
                        }

                        #region 更新ProductTemp

                        ProductTemp proTemp = new ProductTemp();
                        proTemp.Writer_Id = writerID;
                        proTemp.Product_Spec = uint.Parse(specType);
                        proTemp.Spec_Title_1 = spec1Name;
                        proTemp.Spec_Title_2 = spec2Name;
                        proTemp.Combo_Type = COMBO_TYPE;
                        proTemp.Product_Id = proId;
                        bool saveProductResult = _productTempMgr.vendorSpecInfoSave(proTemp) > 0 ? true : false;
                        if (!saveProductResult)
                        {
                            result = false;
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region 無規格

                        ProductTemp query = _productTempMgr.GetVendorProTemp(new ProductTemp { Writer_Id = writerID, Combo_Type = COMBO_TYPE, Product_Id = proId });
                        //如果原數據有規格
                        if (query.Product_Spec != 0)
                        {
                            //刪除服務器上對應的圖片
                            DeletePicOnServer(false, true, false, 0, proId);

                            _productItemTempMgr.DeleteByVendor(new ProductItemTemp { Writer_Id = writerID, Product_Id = proId });

                            _specTempMgr.DeleteByVendor(new ProductSpecTemp { Writer_Id = writerID, product_id = proId });


                            _productTempMgr.vendorSpecInfoSave(new ProductTemp { Product_Spec = 0, Spec_Title_1 = "", Spec_Title_2 = "", Writer_Id = writerID, Combo_Type = COMBO_TYPE, Product_Id = proId });


                            List<ProductItemTemp> saveList = new List<ProductItemTemp>();
                            saveList.Add(new ProductItemTemp { Writer_Id = writerID, Product_Id = proId, Item_Stock = 10, Item_Alarm = 1 });
                            _productItemTempMgr.SaveByVendor(saveList);
                        }
                        else
                        {
                            List<ProductItemTemp> itemQuery = _productItemTempMgr.QueryByVendor(new ProductItemTemp { Writer_Id = writerID, Product_Id = proId });
                            if (itemQuery.Count() <= 0)
                            {
                                List<ProductItemTemp> saveList = new List<ProductItemTemp>();
                                saveList.Add(new ProductItemTemp { Writer_Id = writerID, Product_Id = proId, Item_Stock = 10, Item_Alarm = 1 });
                                _productItemTempMgr.SaveByVendor(saveList);
                            }
                        }


                        #endregion
                    }
                }
                else
                {
                    result = false;
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", "VendorProductCountroller.cs", "specTempSave()", Resources.VendorProduct.NOTFOUNTKEY);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    resultStr = "{success:false}";
                }
                #endregion

            }
            catch (Exception ex)
            {
                result = false;
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            if (!result)
            {
                resultStr = "{success:false}";
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase spec1TempQuery()
        {
            string resultStr = "{success:false}";

            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;
                uint pid = 0;
                //product_spec_temp
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    if (uint.TryParse(Request.Form["ProductId"].ToString(), out pid))
                    {//查看商品列表中正式表中的詳細資料
                        pid = uint.Parse(Request.Form["ProductId"]);
                        _specMgr = new ProductSpecMgr(connectionString);
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specMgr.Query(new ProductSpec { product_id = pid, spec_type = 1 })) + "}";
                    }
                    else
                    {
                        _specTempMgr = new ProductSpecTempMgr(connectionString);
                        ProductSpecTemp query = new ProductSpecTemp { Writer_Id = writerID, spec_type = 1 };
                        query.product_id = Request.Form["ProductId"].ToString();
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specTempMgr.VendorQuery(query)) + "}";
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

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase spec2TempQuery()
        {
            string resultStr = "{success:false}";

            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;
                uint pid = 0;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    if (uint.TryParse(Request.Form["ProductId"].ToString(), out pid))
                    {
                        //product_spec
                        pid = uint.Parse(Request.Form["ProductId"]);
                        _specMgr = new ProductSpecMgr(connectionString);
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specMgr.Query(new ProductSpec { product_id = pid, spec_type = 2 })) + "}";

                    }
                    else
                    {
                        //product_spec_temp
                        _specTempMgr = new ProductSpecTempMgr(connectionString);
                        ProductSpecTemp query = new ProductSpecTemp { Writer_Id = writerID, spec_type = 2 };
                        query.product_id = Request.Form["ProductId"].ToString();
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specTempMgr.VendorQuery(query)) + "}";
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

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase specTempDelete()
        {
            string resultStr = "{success:true}";
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _productItemTempMgr = new ProductItemTempMgr(connectionString);
            _productTempMgr = new ProductTempMgr(connectionString);

            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;

                ProductSpecTemp proSpecTemp = new ProductSpecTemp { Writer_Id = writerID };
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    proSpecTemp.product_id = Request.Form["ProductId"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    proSpecTemp.product_id = Request.Form["OldProductId"].ToString();
                //}
                DeletePicOnServer(false, true, false, 0, proSpecTemp.product_id);
                _specTempMgr.DeleteByVendor(proSpecTemp);
                _productItemTempMgr.DeleteByVendor(new ProductItemTemp { Writer_Id = writerID, Product_Id = proSpecTemp.product_id });

                //更新ProductTemp規格為無規格
                ProductTemp proTemp = new ProductTemp { Product_Spec = 0, Spec_Title_1 = "", Spec_Title_2 = "", Writer_Id = writerID, Combo_Type = COMBO_TYPE };
                proTemp.Product_Id = proSpecTemp.product_id;
                bool saveProductResult = _productTempMgr.vendorSpecInfoSave(proTemp) > 0 ? true : false;


                if (!saveProductResult)
                {
                    resultStr = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                resultStr = "{success:false}";
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

        #region /******************  價格   **************************/


        #region 商品細項價格+HttpResponseBase GetProItems()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProItems()
        {
            string json = string.Empty;
            uint productID = 0;

            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    if (uint.TryParse(Request.Form["ProductId"].ToString(), out productID))
                    {//商品列表中正式表商品的詳細資料
                        //product_item
                        _productItemMgr = new ProductItemMgr(connectionString);
                        List<ProductItem> proItem = _productItemMgr.QueryPrice(new ProductItem { Product_Id = Convert.ToUInt32(Request.Form["ProductId"]) });
                        json = JsonConvert.SerializeObject(proItem);
                    }
                    else
                    {
                        //product_item
                        ProductItemTemp query = new ProductItemTemp();
                        query.Product_Id = Request.Form["ProductId"].ToString();
                        query.Writer_Id = writerID;
                        _productItemTempMgr = new ProductItemTempMgr(connectionString);
                        List<ProductItemTemp> proItemTemp = _productItemTempMgr.QueryByVendor(query);
                        json = JsonConvert.SerializeObject(proItemTemp);
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

        #region 查询供應商新建商品站台信息+ HttpResponseBase GetPriceMaster()

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetPriceMaster()
        {
            string json = string.Empty;

            bool isEdit = false;

            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;
                uint pid = 0;

                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    if (uint.TryParse(Request.Form["ProductId"].ToString(), out pid))
                    {
                        _priceMasterMgr = new PriceMasterMgr(connectionString);
                        List<PriceMasterCustom> proSiteCustom = _priceMasterMgr.Query(new PriceMaster { product_id = pid });
                        StringBuilder strJson = new StringBuilder("[");
                        if (proSiteCustom != null)
                        {
                            foreach (var item in proSiteCustom)
                            {
                                strJson.Append("{");
                                strJson.AppendFormat("price_master_id:{0},product_id:{1},site_id:{2},site_name:\"{3}\"", item.price_master_id, item.product_id, item.site_id, item.site_name);
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
                        _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);

                        PriceMasterTemp query = new PriceMasterTemp();
                        query.writer_Id = writerID;
                        query.combo_type = COMBO_TYPE;
                        query.product_id = Request.Form["ProductId"].ToString();

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
                                    strJson.AppendFormat("price_master_id:{0},product_id:\"{1}\",site_id:{2},site_name:\"{3}\"", item.price_master_id, item.vendor_product_id, item.site_id, item.site_name);
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

        #region 查詢站臺價格明細+JsonResult GetItemPrice()

        [HttpPost]
        [CustomHandleError]
        public JsonResult GetItemPrice()
        {
            List<ItemPriceCustom> itemPrices = new List<ItemPriceCustom>();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["PriceMasterId"]))
                {
                    _itemPriceTempMgr = new ItemPriceTempMgr(connectionString);
                    uint priceMasterId = uint.Parse(Request.Form["PriceMasterId"]);
                    itemPrices = _itemPriceTempMgr.QueryByVendor(new ItemPrice { price_master_id = priceMasterId });

                    List<ItemPriceCustom> newAdds = _itemPriceTempMgr.QueryNewAdd(new ItemPrice { price_master_id = priceMasterId });
                    //说明：当itempricetemp中没有该条数据时，则从product_item_temp 中获取数据信息 edit by shuangshuang0420j 20140909 15:02

                    if (newAdds != null && newAdds.Count > 0)
                    {
                        newAdds.ForEach(m => m.price_master_id = priceMasterId);
                        itemPrices.AddRange(newAdds);
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
            return Json(itemPrices);
        }
        #endregion

        #region 保存商品細項價格、新增站臺價格+HttpResponseBase SaveItemPrice()
        /// <summary>
        /// 将商品价格信息保存至product_temp\price_master_temp\product_item_temp 表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveItemPrice()
        {
            string json = string.Empty;
            string msg = string.Empty;

            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;
                //获取前台输入的信息
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                string items = Request.Form["Items"];
                int same_price = (Request.Form["same_price"] ?? "") == "on" ? 1 : 0;
                string start = Request.Form["event_product_start"] ?? Request.Form["event_start"];
                string end = Request.Form["event_product_end"] ?? Request.Form["event_end"];
                string valid_start = Request.Form["valid_start"];
                string valid_end = Request.Form["valid_end"];

                #region product_item_temp 获取product_item_temp和product_temp的数据

                ProductTemp proTemp = new ProductTemp();
                List<ProductItemTemp> proItemTemps = jsSer.Deserialize<List<ProductItemTemp>>(items);

                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    proTemp.Product_Id = Request.Form["ProductId"].ToString();
                    proItemTemps.ForEach(m => m.Product_Id = proTemp.Product_Id);
                }
                //if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                //{
                //    proTemp.Product_Id = Request.Form["OldProductId"].ToString();
                //    proItemTemps.ForEach(m => m.Product_Id = proTemp.Product_Id);
                //}

                if (!string.IsNullOrEmpty(start))
                {
                    proItemTemps.ForEach(m => m.Event_Product_Start = Convert.ToUInt32(CommonFunction.GetPHPTime(start)));
                }
                if (!string.IsNullOrEmpty(end))
                {
                    proItemTemps.ForEach(m => m.Event_Product_End = Convert.ToUInt32(CommonFunction.GetPHPTime(end)));
                }
                if (!string.IsNullOrEmpty(Request.Form["product_price_list"]))
                {
                    uint product_price_list = 0;
                    uint.TryParse(Request.Form["product_price_list"] ?? "0", out product_price_list);
                    proTemp.Product_Price_List = product_price_list;
                }
                if (!string.IsNullOrEmpty(Request.Form["bag_check_money"]))
                {
                    proTemp.Bag_Check_Money = uint.Parse(Request.Form["bag_check_money"]);
                }
                proTemp.show_listprice = Convert.ToUInt32((Request.Form["show_listprice"] ?? "") == "on" ? 1 : 0);
                proTemp.Writer_Id = writerID;
                proTemp.Combo_Type = COMBO_TYPE;
                proItemTemps.ForEach(m => m.Writer_Id = proTemp.Writer_Id);
                proTemp.Create_Channel = 2;
                _productTempMgr = new ProductTempMgr(connectionString);

                #region PriceMasterTemp 获取PriceMasterTemp的数据


                PriceMasterTemp priceMasterTemp = new PriceMasterTemp { price_status = 1, same_price = same_price };
                priceMasterTemp.product_id = proTemp.Product_Id;
                priceMasterTemp.combo_type = COMBO_TYPE;
                priceMasterTemp.product_name = Request.Form["product_name"] ?? "";
                priceMasterTemp.writer_Id = proTemp.Writer_Id;
                priceMasterTemp.site_id = 1;//默認站臺1:吉甲地
                priceMasterTemp.user_level = 1;
                priceMasterTemp.price = Convert.ToInt32(proItemTemps.Min(m => m.Item_Money));
                priceMasterTemp.event_price = Convert.ToInt32(proItemTemps.Min(m => m.Event_Item_Money));
                priceMasterTemp.cost = Convert.ToInt32(proItemTemps.Min(m => m.Item_Cost));
                priceMasterTemp.event_cost = Convert.ToInt32(proItemTemps.Min(m => m.Event_Item_Cost));
                if (same_price == 0)
                {
                    priceMasterTemp.max_price = Convert.ToInt32(proItemTemps.Max(m => m.Item_Money));
                    priceMasterTemp.max_event_price = Convert.ToInt32(proItemTemps.Max(m => m.Event_Item_Money));
                }
                if (!string.IsNullOrEmpty(start))
                {
                    priceMasterTemp.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(start));
                }
                if (!string.IsNullOrEmpty(end))
                {
                    priceMasterTemp.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(end));
                }
                if (!string.IsNullOrEmpty(valid_start))
                {
                    priceMasterTemp.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                }
                if (!string.IsNullOrEmpty(valid_end))
                {
                    priceMasterTemp.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                }
                #endregion
                //执行
                _productItemTempMgr = new ProductItemTempMgr(connectionString);
                _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                if (_productItemTempMgr.UpdateCostMoney(proItemTemps) && _productTempMgr.PriceBonusInfoSaveByVendor(proTemp) > 0)//供應商商品無購物金
                {
                    bool result = false;
                    PriceMasterTemp query = new PriceMasterTemp { writer_Id = priceMasterTemp.writer_Id, product_id = proTemp.Product_Id, combo_type = COMBO_TYPE };
                    if (_priceMasterTempMgr.QueryByVendor(query) == null)//插入
                    {
                        result = _priceMasterTempMgr.Save(new List<PriceMasterTemp> { priceMasterTemp }, null, null);
                    }
                    else//更新
                    {
                        result = _priceMasterTempMgr.UpdateByVendor(new List<PriceMasterTemp> { priceMasterTemp }, null);
                    }
                    json = "{success:" + result.ToString().ToLower() + "}";
                }
                else
                {
                    json = "{success:false}";
                }
                #endregion

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

        #region 修改站臺價格

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase UpdateItemPrice()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writerID = (int)vendorModel.vendor_id;

                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                uint priceMasterId = uint.Parse(Request.Form["price_master_id"] ?? "0");
                int same_price = (Request.Form["same_price"] ?? "") == "on" ? 1 : 0;
                string start = Request.Form["event_start"] ?? "";
                string end = Request.Form["event_end"] ?? "";
                string valid_start = Request.Form["valid_start"];
                string valid_end = Request.Form["valid_end"];
                string items = Request.Form["Items"];
                List<ItemPrice> newPrices = jsSer.Deserialize<List<ItemPrice>>(items);


                _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                PriceMasterCustom PriceMasterTemp = _priceMasterTempMgr.QueryProdSiteByVendor(new PriceMasterTemp { price_master_id = priceMasterId }).FirstOrDefault();
                if (PriceMasterTemp != null)
                {
                    #region 處理PriceMasterTemp
                    PriceMasterTemp.product_name = Request.Form["product_name"] ?? "";
                    PriceMasterTemp.same_price = same_price;
                    PriceMasterTemp.user_level = 1;//会员等级默认为 1：普通会员
                    PriceMasterTemp.user_id = 0;
                    PriceMasterTemp.site_id = 1;//默认站台为吉甲地
                    if (!string.IsNullOrEmpty(start))
                    {
                        PriceMasterTemp.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(start));
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        PriceMasterTemp.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(end));
                    }

                    if (!string.IsNullOrEmpty(valid_start))
                    {
                        PriceMasterTemp.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                    }
                    if (!string.IsNullOrEmpty(valid_end))
                    {
                        PriceMasterTemp.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                    }

                    PriceMasterTemp.price = Convert.ToInt32(newPrices.Min(m => m.item_money));
                    PriceMasterTemp.event_price = Convert.ToInt32(newPrices.Min(m => m.event_money));
                    if (same_price == 0)
                    {
                        PriceMasterTemp.max_price = Convert.ToInt32(newPrices.Max(m => m.item_money));
                        PriceMasterTemp.max_event_price = Convert.ToInt32(newPrices.Max(m => m.event_money));
                    }
                    PriceMasterTemp.cost = Convert.ToInt32(newPrices.Min(m => m.item_cost));
                    PriceMasterTemp.event_cost = Convert.ToInt32(newPrices.Min(m => m.event_cost));
                    #endregion

                    bool isExist = false;
                    List<PriceMasterCustom> masterList = _priceMasterTempMgr.QueryProdSiteByVendor(new PriceMasterTemp { site_id = PriceMasterTemp.site_id, user_id = PriceMasterTemp.user_id, user_level = PriceMasterTemp.user_level, product_id = Convert.ToString(PriceMasterTemp.vendor_product_id) });
                    List<PriceMasterCustom> resultList = masterList.Where(p => p.price_master_id != PriceMasterTemp.price_master_id).ToList();
                    if (resultList != null && resultList.Count() > 0)
                    {
                        if (PriceMasterTemp.user_id != 0 || (PriceMasterTemp.user_id == 0 && resultList.Where(p => p.user_id == 0).Count() > 0))
                        {
                            json = "{success:false,msg:'" + Resources.VendorProduct.SITE_EXIST + "'}";
                            isExist = true;
                        }
                    }
                    if (!isExist)
                    {
                        ArrayList excuteSql = new ArrayList();
                        // ProductTemp productTemp = null;
                        if (PriceMasterTemp.site_id == 1 && PriceMasterTemp.user_level == 1 && PriceMasterTemp.user_id == 0)
                        {
                            //没有购物金。所以不处理product_temp
                            //#region 處理productTemp
                            //_productTempMgr = new ProductTempMgr(connectionString);
                            //productTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { product_id = PriceMasterTemp.vendor_product_id, Create_Channel = 2 }).FirstOrDefault();

                            //excuteSql.Add(_productTempMgr.Update(productTemp));
                            //#endregion

                            #region 處理ProductItem
                            _productItemTempMgr = new ProductItemTempMgr(connectionString);
                            List<ProductItemTemp> productItems = _productItemTempMgr.QueryByVendor(new ProductItemTemp { Product_Id = Convert.ToString(PriceMasterTemp.vendor_product_id), Writer_Id = writerID });
                            if (productItems != null)
                            {
                                if (!string.IsNullOrEmpty(start))
                                {
                                    productItems.ForEach(m => m.Event_Product_Start = Convert.ToUInt32(CommonFunction.GetPHPTime(start)));
                                }
                                if (!string.IsNullOrEmpty(end))
                                {
                                    productItems.ForEach(m => m.Event_Product_End = Convert.ToUInt32(CommonFunction.GetPHPTime(end)));
                                }
                                newPrices.ForEach(m => productItems.Find(n => n.Item_Id == m.item_id).Item_Money = m.item_money);
                                newPrices.ForEach(m => productItems.Find(n => n.Item_Id == m.item_id).Item_Cost = m.item_cost);
                                newPrices.ForEach(m => productItems.Find(n => n.Item_Id == m.item_id).Event_Item_Money = m.event_money);
                                newPrices.ForEach(m => productItems.Find(n => n.Item_Id == m.item_id).Event_Item_Cost = m.event_cost);

                                productItems.ForEach(m => excuteSql.Add(_productItemTempMgr.UpdateByVendor(m)));
                            }
                            #endregion
                        }


                        bool result = true;
                        //处理PriceMasterTemp
                        excuteSql.Add(_priceMasterTempMgr.UpdateTs(PriceMasterTemp));
                        _itemPriceTempMgr = new ItemPriceTempMgr(connectionString);
                        if (!_itemPriceTempMgr.UpdateByVendor(excuteSql))//执行sql语句
                        {
                            result = false;
                        }

                        json = "{success:" + result.ToString().ToLower() + "}";
                    }
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


        #endregion

        #region /******************  類別   **************************/

        #region /******************類別樹*********************/

        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetCatagory(string id = "true")
        {
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            string resultStr = "";
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
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
                    if (uint.TryParse(Request.Params["ProductId"], out product_id)) //正式表數據
                    {
                        _categorySetMgr = new ProductCategorySetMgr(connectionString);
                        resultList = _categorySetMgr.Query(new ProductCategorySet { Product_Id = uint.Parse(Request.Params["ProductId"]) });
                        TempresultList = null;
                    }
                    else
                    {
                        resultList = null;
                        query.Product_Id = Request.Params["ProductId"];
                        _productTempMgr = new ProductTempMgr(connectionString);
                        ProductTemp productT = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = Request.Params["ProductId"] }).FirstOrDefault();
                        if (productT != null)
                        {
                            query.Combo_Type = productT.Combo_Type;
                        }
                        else
                        {
                            query.Combo_Type = COMBO_TYPE;
                        }
                        query.Writer_Id = (int)vendorModel.vendor_id;

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
                    }
                }

                //調試resultlist是否為空
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
                {//正式數據不為null時對類表賦值
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

        #region 獲取選擇過得類別 +HttpResponseBase GetSelectedCage()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetSelectedCage()
        {
            string resultStr = "{success:false}";

            try
            {
                string strCateId = string.Empty;
                uint vendor_pid = 0;
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                int writd_Id = (int)vendorModel.vendor_id;

                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    if (uint.TryParse(Request.Params["ProductId"].ToString(), out vendor_pid))
                    {
                        #region 從正式表獲取
                        uint pid = uint.Parse(Request.Params["ProductId"]);
                        _productMgr = new ProductMgr(connectionString);
                        Product result = _productMgr.Query(new Product { Product_Id = pid }).FirstOrDefault();
                        if (result != null)
                        {
                            strCateId = result.Cate_Id;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 從臨時表獲取
                        _productTempMgr = new ProductTempMgr(connectionString);
                        ProductTemp query = new ProductTemp { Writer_Id = writd_Id, Combo_Type = COMBO_TYPE, Product_Id = Request.Params["ProductId"].ToString() };
                        ProductTemp tempResult = _productTempMgr.GetVendorProTemp(query);
                        if (tempResult != null)
                        {
                            strCateId = tempResult.Cate_Id;
                        }
                        #endregion
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
            int writd_Id = (int)vendorModel.vendor_id;
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

        #endregion

        #region /*********圖檔**************************************/
        //[HttpPost]
        public string QueryExplainPic()
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writerID = (int)vendorModel.vendor_id;
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

        [HttpPost]
        public HttpResponseBase DeletePic()
        {
            string json = "{success:true,msg:\"" + Resources.VendorProduct.DELETE_SUCCESS + "\",path:\"" + default50Path + "\"}";
            string deleteType = Request.Params["type"];
            ProductSpecTemp psTemp = new ProductSpecTemp();
            ProductSpec pSpec = new ProductSpec();
            List<ProductSpecTemp> psList = new List<ProductSpecTemp>();
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];//獲取當前登入的供應商

            string[] record = Request.Params["rec"].Split(',');
            if (record[0].Split('/').Length == defaultImgLength)   //默认图片无法删除
            {
                json = "{success:false,msg:\"" + Resources.VendorProduct.DEFAULT_CANNOT_DELETE + "\"}";
                this.Response.Clear();
                this.Response.Write(json);
                this.Response.End();
                return this.Response;
            }
            string fileName = record[0].Split('/')[imgNameIdx];
            if (deleteType == "spec")
            {
                psTemp.spec_image = string.Empty;
                psTemp.spec_id = uint.Parse(record[1]);
                psTemp.spec_sort = uint.Parse(record[2]);
                psTemp.spec_status = uint.Parse(record[3]);

                pSpec.spec_image = string.Empty;
                pSpec.spec_id = uint.Parse(record[1]);
                pSpec.spec_sort = uint.Parse(record[2]);
                pSpec.spec_status = uint.Parse(record[3]);

                psList.Add(psTemp);

                string imageName = imgLocalPath + specPath + GetDetailFolder(fileName) + fileName;
                string image100 = imgLocalPath + spec100Path + GetDetailFolder(fileName) + fileName;
                string image280 = imgLocalPath + spec280Path + GetDetailFolder(fileName) + fileName;

                //刪除服務器上對應的圖片
                DeletePicFile(imageName);
                DeletePicFile(image100);
                DeletePicFile(image280);
            }
            else if (deleteType == "desc")
            {

                string imageName = imgLocalPath + descPath + GetDetailFolder(fileName) + fileName;
                string imageName400 = imgLocalPath + desc400Path + GetDetailFolder(fileName) + fileName;
                DeletePicFile(imageName);
                DeletePicFile(imageName400);
            }
            psTemp.Writer_Id = (int)vendorModel.vendor_id;
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                psTemp.product_id = Request.Params["product_id"];
            }
            //if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))
            //{
            //    psTemp.product_id = Request.Params["OldProductId"];
            //}
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
                json = "{success:false,msg:\"" + Resources.VendorProduct.DELETE_SPEC_FAIL + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [HttpPost]
        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }

        [HttpPost]
        public HttpResponseBase productPictrueTempSave()
        {
            string json = "{success:true}";
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writerID = (int)vendorModel.vendor_id;
            ProductTemp pTemp = new ProductTemp();
            _productTempMgr = new ProductTempMgr(connectionString);
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            pTemp.Writer_Id = writerID;
            pTemp.Combo_Type = COMBO_TYPE;
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                pTemp.Product_Id = Request.Params["product_id"];
            }
            //if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))
            //{
            //    pTemp.Product_Id = Request.Params["OldProductId"];
            //}
            if (!string.IsNullOrEmpty(Request.Params["image_InsertValue"])) pTemp.Product_Image = Request.Params["image_InsertValue"];
            if (!string.IsNullOrEmpty(Request.Params["productMedia"])) pTemp.product_media = Request.Params["productMedia"];

            ProductSpecTemp pSpec = new ProductSpecTemp();
            List<ProductSpecTemp> pSpecList = new List<ProductSpecTemp>();
            if (!string.IsNullOrEmpty(Request.Params["spec_InsertValue"]))
            {
                string[] Values = Request.Form["spec_InsertValue"].ToString().Split(';');
                for (int i = 0; i < Values.Length - 1; i++)
                {
                    pSpec = new ProductSpecTemp();
                    pSpec.Writer_Id = writerID;
                    pSpec.product_id = pTemp.Product_Id;
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
                    pPic.writer_Id = writerID;
                    pPic.product_id = pTemp.Product_Id;
                    pPic.image_createdate = Convert.ToInt32(CommonFunction.GetPHPTime());
                    if (!string.IsNullOrEmpty(perValue[0])) { pPic.image_filename = perValue[0]; };
                    if (!string.IsNullOrEmpty(perValue[1])) { pPic.image_sort = uint.Parse(perValue[1]); };
                    if (!string.IsNullOrEmpty(perValue[2])) { pPic.image_state = uint.Parse(perValue[2]); };
                    picList.Add(pPic);
                }
            }
            try
            {
                // int writer_id = writerID; // (Session["caller"] as Caller).user_id;
                //保存至productTemp
                if (pTemp.Product_Image != "" || pTemp.product_media != "")
                {
                    _productTempMgr.ProductTempUpdateByVendor(pTemp, "pic");
                }
                //保存規格圖
                _specTempMgr.Update(pSpecList, "image");
                //保存說明圖
                ProductPictureTemp proPictureTemp = new ProductPictureTemp { writer_Id = writerID, combo_type = COMBO_TYPE, product_id = pTemp.Product_Id };
                _pPicTempMgr.Save(picList, proPictureTemp);
                json = "{success:true,msg:\"" + Resources.VendorProduct.SAVE_SUCCESS + "\"}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + Resources.VendorProduct.SAVE_FAIL + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [HttpPost]
        public ActionResult upLoadImg()
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];//獲取當前登入的供應商
            int writerID = (int)vendorModel.vendor_id;

            string path = Server.MapPath(xmlPath);
            siteConfigMgr = new SiteConfigMgr(path);
            ViewBag.moreFileOneTime = false;
            SiteConfig extention_config = siteConfigMgr.GetConfigByName("PIC_Extention_Format");
            SiteConfig minValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
            SiteConfig maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
            SiteConfig admin_userName = siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
            SiteConfig admin_passwd = siteConfigMgr.GetConfigByName("ADMIN_PASSWD");

            string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
            string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;

            string localProdPath = imgLocalPath + prodPath;
            //string localProd50Path = imgLocalPath + prod50Path;
            //string localProd150Path = imgLocalPath + prod150Path;
            //string localProd280Path = imgLocalPath + prod280Path;

            string localSpecPath = imgLocalPath + specPath;
            //string localSpec100Path = imgLocalPath + spec100Path;
            //string localSpec280Path = imgLocalPath + spec280Path;


            string localDescPath = imgLocalPath + descPath;
            //string localDesc400Path = imgLocalPath + desc400Path;
            string[] Mappath = new string[2];

            FileManagement fileLoad = new FileManagement();

            HttpPostedFileBase file = Request.Files["Filedata"];
            string nameType = Request.Params["nameType"];
            string fileName = string.Empty;
            string fileExtention = string.Empty;
            ViewBag.spec_id = -1;
            if (nameType != null)
            {
                fileName = nameType + fileLoad.NewFileName(file.FileName);
                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower();

            }
            else
            {
                #region 批次上傳圖片操作
                //此處由批次上傳進入.
                //判斷文件名格式是否正確
                ViewBag.moreFileOneTime = true;
                int prodCheck = file.FileName.LastIndexOf("prod_");
                int specCheck = file.FileName.LastIndexOf("spec_");
                int descCheck = file.FileName.LastIndexOf("desc_");
                string errorMsg = "ERROR/";
                if (prodCheck == -1 && specCheck == -1 && descCheck == -1)
                {
                    errorMsg += "[" + file.FileName + "] ";
                    errorMsg += Resources.VendorProduct.FILE_NAME_ERROR;

                    ViewBag.fileName = errorMsg;
                    return View("~/Views/VendorProduct/upLoad.cshtml");
                }
                else
                {
                    nameType = file.FileName.Split('_')[0];
                    fileName = nameType + fileLoad.NewFileName(file.FileName);
                    fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                    fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                }
                if (specCheck == 0)
                {
                    //Caller _caller = (Session["caller"] as Caller);
                    ProductSpecTemp proSpecTemp = new ProductSpecTemp();
                    string spec = file.FileName.Split('_')[1].Split('.')[0];
                    bool checkStatus = true;
                    if (!string.IsNullOrEmpty(Request.Params["product_id"].Split(';')[0].ToString()))
                    {
                        //product_spec
                        proSpecTemp.product_id = Request.Params["product_id"].Split(';')[0].ToString();
                    }
                    //product_spec_temp
                    _specTempMgr = new ProductSpecTempMgr(connectionString);

                    List<ProductSpecTemp> pSpecTempList = _specTempMgr.VendorQuery(new ProductSpecTemp { Writer_Id = writerID, spec_type = 1 });
                    foreach (var item in pSpecTempList)
                    {
                        if (item.spec_name == spec)
                        {
                            checkStatus = false;
                            ViewBag.spec_id = item.spec_id;
                        }
                    }
                    if (checkStatus)//表示沒有要上傳圖片規格相同的規格一
                    {
                        errorMsg += "[" + file.FileName + "] " + Resources.VendorProduct.SPEC_NOT_FIND;
                        ViewBag.fileName = errorMsg;
                        return View("~/Views/VendorProduct/upLoad.cshtml");
                    }
                }
                #endregion
            }

            string returnName = imgServerPath;


            bool result = false;
            string NewFileName = string.Empty;


            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
            NewFileName = hash.Md5Encrypt(fileName, "32");

            string firstFolder = NewFileName.Substring(0, 2) + "/";
            string secondFolder = NewFileName.Substring(2, 2) + "/";
            string ServerPath = string.Empty;

            if (nameType == "spec")
            {
                Mappath[0] = firstFolder;
                Mappath[1] = secondFolder;

                CreateFolder(localSpecPath, Mappath);
                //CreateFolder(localSpec100Path, Mappath);
                //CreateFolder(localSpec280Path, Mappath);

                localSpecPath += firstFolder + secondFolder;
                //localSpec100Path += firstFolder + secondFolder;
                //localSpec280Path += firstFolder + secondFolder;
                specPath += firstFolder + secondFolder;

                returnName += specPath + NewFileName + fileExtention;
                //localSpec100Path += NewFileName + fileExtention;
                //localSpec280Path += NewFileName + fileExtention;
                NewFileName = localSpecPath + NewFileName + fileExtention;
                ServerPath = Server.MapPath(imgLocalServerPath + specPath);

            }
            else if (nameType == "desc")
            {

                Mappath[0] = firstFolder;
                Mappath[1] = secondFolder;

                CreateFolder(localDescPath, Mappath);
                //CreateFolder(localDesc400Path, Mappath);

                localDescPath += firstFolder + secondFolder;
                //localDesc400Path += firstFolder + secondFolder;
                descPath += firstFolder + secondFolder;

                //localDesc400Path += NewFileName + fileExtention;
                returnName += descPath + NewFileName + fileExtention;
                NewFileName = localDescPath + NewFileName + fileExtention;
                ServerPath = Server.MapPath(imgLocalServerPath + descPath);
            }
            else
            {
                Mappath[0] = firstFolder;
                Mappath[1] = secondFolder;
                //Data:2014/06/26
                //author：Castle
                //在前台如果各种尺寸的图档没有的时候，前台会自动产生！！！
                CreateFolder(localProdPath, Mappath);
                //CreateFolder(localProd50Path, Mappath);
                //CreateFolder(localProd150Path, Mappath);
                //CreateFolder(localProd280Path, Mappath);

                localProdPath += firstFolder + secondFolder;
                //localProd50Path += firstFolder + secondFolder;
                //localProd150Path += firstFolder + secondFolder;
                //localProd280Path += firstFolder + secondFolder;
                prodPath += firstFolder + secondFolder;
                //localProd50Path += NewFileName + fileExtention;
                //localProd150Path += NewFileName + fileExtention;
                //localProd280Path += NewFileName + fileExtention;
                returnName += prodPath + NewFileName + fileExtention;

                NewFileName = localProdPath + NewFileName + fileExtention;
                ServerPath = Server.MapPath(imgLocalServerPath + prodPath);
            }
            string ErrorMsg = string.Empty;
            Resource.CoreMessage = new CoreResource("VendorProduct");
            try
            {
                //上傳圖片

                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                //上傳對應大小圖片
                //压缩图片至其它规格
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            if (string.IsNullOrEmpty(ErrorMsg))
            {
                ViewBag.fileName = returnName;
            }
            else
            {
                ViewBag.fileName = "ERROR/" + ErrorMsg;
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = "ERROR/" + ErrorMsg;
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            return View("~/Views/VendorProduct/upLoad.cshtml");
        }

        /// <summary>
        /// 生成和上传图片
        /// </summary>
        /// <param name="serverPath">服务器路径</param>
        /// <param name="newFileName">新名称</param>
        /// <param name="width">新图片宽度</param>
        /// <param name="height">新图片高度</param>
        public void MakeUpLoadImg(HttpPostedFileBase file, string ftpPath, string serverPath, string sourceImgPath, string newFileName, int width, int height)
        {
            string serverFilePath = Server.MapPath(serverPath + newFileName);
            string error = string.Empty;
            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);
            ImageClass iC = new ImageClass(sourceImgPath);
            iC.MakeThumbnail(serverFilePath, width, height, ref error);
            if (error != string.Empty)
            {
                file.SaveAs(serverFilePath);
            }
            FTP ftp = new FTP(ftpPath, ftpuser, ftppwd);
            ftp.UploadFile(serverFilePath);
        }

        public void CreateFolder(string path, string[] Mappath)
        {
            FTP ftp = null;
            try
            {
                string fullPath = path;
                foreach (string s in Mappath)
                {
                    ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                    fullPath += s;

                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                        ftp.MakeDirectory();
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
        }

        [HttpPost]
        public string QuerySpecPic()
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];//獲取當前登入的供應商

            string serverSpecPath = imgServerPath + specPath;
            string serverSpec100Path = imgServerPath + spec100Path;
            string serverSpec280Path = imgServerPath + spec280Path;


            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);
            string json = string.Empty;
            ProductSpecTemp psTemp = new ProductSpecTemp();
            psTemp.Writer_Id = (int)vendorModel.vendor_id;
            psTemp.spec_type = 1;
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                psTemp.product_id = Request.Params["product_id"];
            }

            uint pid = 0;
            if (uint.TryParse(Request.Params["product_id"], out pid))
            {
                #region 正式表
                ProductSpec pSpec = new ProductSpec();
                pSpec.spec_type = 1;
                pSpec.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                List<ProductSpec> spList = _specMgr.Query(pSpec);
                foreach (var item in spList)
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
                json = "{success:true,items:" + JsonConvert.SerializeObject(spList) + "}";
                json = json.Replace("spec_image", "img");
                #endregion
            }
            else
            {
                #region 供應商 臨時表
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
                #endregion
            }

            return json;
        }


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
        #endregion

        #region /*********庫存**************************************/
        #region 查詢庫存信息+string QueryStock()
        [HttpPost]
        [CustomHandleError]
        public string QueryStock()
        {

            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            string json = string.Empty;
            int writerID = (int)vendorModel.vendor_id;
            uint pid = 0;
            try
            {
                ProductItemTemp query = new ProductItemTemp();
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    if (uint.TryParse(Request.Params["product_id"].ToString(), out pid))
                    {//查詢商品列表中正式表的詳細資料
                        _productItemMgr = new ProductItemMgr(connectionString);
                        ProductItem pItem = new ProductItem();
                        pItem.Product_Id = uint.Parse(Request.Params["product_id"]);
                        _productItemMgr.Query(pItem);
                        json = _productItemMgr.QueryStock(pItem);

                    }
                    else
                    {
                        query.Product_Id = Request.Params["product_id"].ToString();

                        query.Writer_Id = writerID;

                        //查找臨時表是否有記錄
                        _productItemTempMgr = new ProductItemTempMgr(connectionString);

                        json = _productItemTempMgr.QueryStockByVendor(query);
                    }

                }
                else
                {
                    json = "{success:false,msg:'" + Resources.VendorProduct.NOTFOUNTKEY + "'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + ex.Message + "'}";
            }
            return json;
        }
        #endregion

        #region 單一商品庫存保存+HttpResponseBase StockSave()
        [HttpPost]
        public HttpResponseBase StockSave()
        {

            string[] Value = Request.Params["ig_sh_InsertValue"].Split(',');
            string json = "{success:true}";
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            int writeId = (int)vendorModel.vendor_id;

            _productItemTempMgr = new ProductItemTempMgr(connectionString);

            List<ProductItemTemp> piTempList = new List<ProductItemTemp>();
            ProductItemTemp piTemp = new ProductItemTemp();
            if (!string.IsNullOrEmpty(Request.Form["InsertValue"]))
            {
                string[] Values = Request.Form["InsertValue"].ToString().Split(';');
                for (int i = 0; i < Values.Length - 1; i++)
                {
                    piTemp = new ProductItemTemp();
                    piTemp.Writer_Id = writeId;
                    string[] perValue = Values[i].Split(',');

                    //查詢product_item數據
                    if (!string.IsNullOrEmpty(perValue[5]))
                    {
                        piTemp.Item_Id = uint.Parse(perValue[5]);
                    }
                    if (!string.IsNullOrEmpty(perValue[0])) { piTemp.Spec_Id_1 = uint.Parse(perValue[0]); }
                    if (!string.IsNullOrEmpty(perValue[1])) { piTemp.Spec_Id_2 = uint.Parse(perValue[1]); }
                    if (!string.IsNullOrEmpty(perValue[2])) { piTemp.Item_Stock = int.Parse(perValue[2]); }
                    if (!string.IsNullOrEmpty(perValue[3])) { piTemp.Item_Alarm = uint.Parse(perValue[3]); }
                    if (!string.IsNullOrEmpty(perValue[4])) { piTemp.Barcode = perValue[4]; };
                    if (!string.IsNullOrEmpty(perValue[6])) { piTemp.Item_Code = perValue[6]; }//與吉甲地商品後台保持一致並未存入數據庫並且此欄位已被價格面板的廠商商品編號一欄佔用
                    if (!string.IsNullOrEmpty(perValue[7])) { piTemp.Erp_Id = perValue[7]; }

                    piTempList.Add(piTemp);
                }
            }
            //執行數據修改
            try
            {
                //更新product_temp
                _productTempMgr = new ProductTempMgr(connectionString);
                ProductTemp pTemp = new ProductTemp();

                if (!string.IsNullOrEmpty(Request.Params["product_id"].ToString()))
                {
                    pTemp.Product_Id = Request.Params["product_id"].ToString();
                }

                pTemp.Ignore_Stock = int.Parse(Value[0]);
                pTemp.Shortage = int.Parse(Value[1]);
                pTemp.Writer_Id = writeId;
                pTemp.Combo_Type = COMBO_TYPE;


                _productTempMgr.ProductTempUpdateByVendor(pTemp, "stock");

                piTempList.ForEach(m => { m.Product_Id = pTemp.Product_Id; });
                if (_productItemTempMgr.UpdateStockAlarmByVendor(piTempList))
                {

                    json = "{success:true,msg:'" + Resources.VendorProduct.SAVE_SUCCESS + "'}";
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
        #endregion

    }
}

