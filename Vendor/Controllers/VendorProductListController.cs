using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Vendor.CustomHandleError;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;
using Vendor.Controllers;

namespace Vendor.Controllers
{
    public class VendorProductListController : Controller
    {
        //
        // GET: /ProductList/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        private ISiteImplMgr _siteMgr;
        private IProductImplMgr _productMgr;
        private IPriceMasterImplMgr _pMaster;
        //private IFunctionImplMgr _functionMgr;
        private IProdVdReqImplMgr _prodVdReq;
        private ProductCategoryMgr _procateMgr;
        private ISiteConfigImplMgr _siteConfigMgr;
        private IProductTagImplMgr _productTagMgr;
        private ITableHistoryImplMgr _tableHistoryMgr;
        // private IProductStatusApplyImplMgr _applyMgr;
        private IProductTempImplMgr _productTempMgr;
        private IProductTagSetImplMgr _productTagSetMgr;
        private IProductTagSetTempImplMgr _productTagSetTempMgr;
        // private IProductStatusHistoryImplMgr _statusHistoryMgr;
        //private IProductStatusHistoryImplMgr _productStatusHistoryMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetImplMgr _productNoticeSetMgr;
        private IProductNoticeSetTempImplMgr _productNoticeSetTempMgr;

        private ITableHistoryItemImplMgr _tableHistoryItemMgr;
        private IProductItemImplMgr _productItemMgr;
        private IItemPriceImplMgr _itemPriceMgr;
        private IHistoryBatchImplMgr _historyBatchMgr;
        private VendorBrandMgr vbMgr;

        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);
        string tagPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_tagPath);

        string prod50Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod50Path);
        string noticePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_noticePath);
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";

        #region 視圖
        /// <summary>
        /// 商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductDetails()
        {
            return View();
        }
        public ActionResult PriceVerifyList()
        {
            return View();
        }
        //代審核
        public ActionResult ReplaceVerify()
        {
            return View();
        }
        //待審核列表
        public ActionResult VerifyList()
        {
            return View();
        }
        #endregion
        #region 供應商商品列表查詢+HttpResponseBase QueryVendorProductList()
        [CustomHandleError]
        public HttpResponseBase QueryVendorProductList()
        {
            string json = string.Empty;
            try
            {
                VenderProductList query = new VenderProductList();
                #region 查询条件填充
                query.IsPage = true;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"]);
                }
                query.price_check = true;//價格進階查詢
                #region 站台條件(暫設)
                query.site_id = 1;//站台預設為gigade
                query.user_level = 1;//一般會員
                query.user_id = 0;
                #endregion
                if (!string.IsNullOrEmpty(Request.Form["brand_id"]))    //品牌
                {
                    query.brand_id = uint.Parse(Request.Form["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["combination"])) //商品類型
                {
                    query.combination = int.Parse(Request.Form["combination"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_status"]))  //商品狀態
                {
                    query.product_status = int.Parse(Request.Form["product_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_freight_set"])) //運送方式
                {
                    query.freight = uint.Parse(Request.Form["product_freight_set"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_mode"]))    //出貨方式
                {
                    query.mode = uint.Parse(Request.Form["product_mode"]);
                }
                query.date_type = Request.Form["date_type"] ?? "";  //日期條件
                if (!string.IsNullOrEmpty(Request.Form["time_start"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                }
                if (!string.IsNullOrEmpty(Request.Form["time_end"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                }
                query.name_number = Request.Form["key"] ?? "";  //名稱/編號搜尋
                //品牌下拉選單只能看到該供應商品牌。
                query.vendor_id = Convert.ToUInt32((Session["vendor"] as BLL.gigade.Model.Vendor).vendor_id);
                #endregion

                _productMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                List<VenderProductListCustom> pros = _productMgr.GetVendorProduct(query, out totalCount);
                foreach (var item in pros)
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
                json = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(pros) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{succes:true,totalCount:0,item:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 供應商商品申請上架 +HttpResponseBase ProductUp()
        /// <summary>
        /// 供應商商品申請上架
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductUp()
        {
            string json = "{success:true}";
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            _prodVdReq = new ProdVdReqMgr(connectionString);
            _productMgr = new ProductMgr(connectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');
                    foreach (string str in pro_Ids.Distinct())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            uint product_id = uint.Parse(str);
                            Product pro = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                            if (pro != null && pro.Product_Status != 5)
                            {
                                ProdVdReqQuery prodQuery = new ProdVdReqQuery();
                                prodQuery.vendor_id = Convert.ToInt32(vendor.vendor_id);
                                prodQuery.product_id = Convert.ToInt32(product_id);
                                prodQuery.req_status = 1;
                                int totalCount = 0;
                                ProdVdReq prodRquery = _prodVdReq.QueryProdVdReqList(prodQuery, out totalCount).FirstOrDefault();


                                if (prodRquery == null)
                                {
                                    prodRquery = new ProdVdReq();
                                    prodRquery.vendor_id = Convert.ToInt32(vendor.vendor_id);
                                    prodRquery.product_id = Convert.ToInt32(product_id);
                                    prodRquery.req_status = 1;
                                    prodRquery.req_datatime = DateTime.Now;
                                    prodRquery.req_type = 1;
                                    if (_prodVdReq.Insert(prodRquery) < 0)
                                    {
                                        json = "{success:false,msg:0}";//返回json數據
                                    }
                                }
                                else
                                {
                                    prodRquery.vendor_id = Convert.ToInt32(vendor.vendor_id);
                                    prodRquery.product_id = Convert.ToInt32(product_id);
                                    prodRquery.req_status = 1;
                                    prodRquery.req_datatime = DateTime.Now;
                                    prodRquery.req_type = 1;
                                    if (_prodVdReq.Update(prodRquery) < 0)
                                    {
                                        json = "{success:false,msg:0}";//返回json數據
                                    }
                                }
                            }
                        }
                    }
                }
                //json = "{success:true}";
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
        #region 供應商商品申請下架 +HttpResponseBase ProductDown()
        /// <summary>
        /// 供應商商品申請下架
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductDown()
        {
            string json = "{success:true}";
            _prodVdReq = new ProdVdReqMgr(connectionString);
            _productMgr = new ProductMgr(connectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');
                    BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
                    uint product_end = 0;
                    string dtime = CommonFunction.DateTimeToString(Convert.ToDateTime(DateTime.Now.ToString()));
                    var explain = Request.Form["explain"] ?? "";
                    if (!string.IsNullOrEmpty(Request.Form["Product_End"]))
                    {
                        dtime = CommonFunction.DateTimeToString(Convert.ToDateTime(Request.Form["Product_End"].ToString()));
                        product_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Form["Product_End"]));
                    }

                    foreach (string str in pro_Ids.Distinct())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            uint product_id = uint.Parse(str);
                            Product pro = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                            if (product_end == 0)//說明是立即下架,就用原來的時間
                            {
                                product_end = pro.Product_End;
                            }
                            if (pro != null && pro.Product_Status == 5)// && pro.user_id == (Session["caller"] as Caller).user_id
                            {
                                ProdVdReqQuery prodQuery = new ProdVdReqQuery();
                                prodQuery.vendor_id = Convert.ToInt32(vendor.vendor_id);
                                prodQuery.product_id = Convert.ToInt32(product_id);
                                prodQuery.req_status = 1;
                                int totalCount = 0;
                                ProdVdReq prodRquery = _prodVdReq.QueryProdVdReqList(prodQuery, out totalCount).FirstOrDefault();



                                if (prodRquery == null)
                                {
                                    prodRquery = new ProdVdReq();
                                    prodRquery.vendor_id = Convert.ToInt32(vendor.vendor_id);
                                    prodRquery.product_id = Convert.ToInt32(product_id);
                                    prodRquery.req_status = 1;
                                    prodRquery.req_datatime = Convert.ToDateTime(dtime);
                                    prodRquery.req_type = 2;
                                    prodRquery.explain = explain + dtime;

                                    if (_prodVdReq.Insert(prodRquery) < 0)
                                    {
                                        json = "{success:false,msg:0}";//返回json數據
                                    }
                                }
                                else
                                {
                                    prodRquery.vendor_id = Convert.ToInt32(vendor.vendor_id);
                                    prodRquery.product_id = Convert.ToInt32(product_id);
                                    prodRquery.req_status = 1;
                                    prodRquery.req_datatime = Convert.ToDateTime(dtime);
                                    prodRquery.req_type = 2;
                                    prodRquery.explain = explain + dtime;
                                    if (_prodVdReq.Update(prodRquery) < 0)
                                    {
                                        json = "{success:false,msg:0}";//返回json數據
                                    }
                                }
                            }
                        }
                    }
                }
                //json = "{success:true}";
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

        #region 商品複製+HttpResponseBase ProductCopy()
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductCopy()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]) && !string.IsNullOrEmpty(Request.Form["Combination"]))
                {
                    string product_id = Request.Form["Product_Id"].ToString();
                    int writer_id = Convert.ToInt32((Session["vendor"] as BLL.gigade.Model.Vendor).vendor_id);
                    int combo_type = Convert.ToInt32(Request.Form["Combination"]);
                    string Product_Ipfrom = Request.UserHostAddress;//获取当前ip
                    combo_type = (combo_type == 0 || combo_type == 1) ? 1 : 2;
                    bool result = false;
                    _productTempMgr = new ProductTempMgr(connectionString);
                    string prod_id = "";
                    result = _productTempMgr.VendorCopyProduct(writer_id, combo_type, product_id, ref prod_id, Product_Ipfrom);
                    json = "{success:" + result.ToString().ToLower() + ",id:\"" + prod_id + "\"}";
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

        #region 商品詳細信息查詢
        [HttpPost]
        public HttpResponseBase ProductDetailsQuery()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    string product_id = Request.Form["ProductId"];
                    uint productid = 0;
                    _productMgr = new ProductMgr(connectionString);
                    ProductDetailsCustom product = new ProductDetailsCustom();
                    if (uint.TryParse(product_id, out productid))
                    {
                        product = _productMgr.ProductDetail(new Product { Product_Id = productid });
                    }
                    else
                    {
                        product = _productMgr.VendorProductDetail(new ProductTemp { Product_Id = product_id });
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

        #region 供應商商品刪除
        /// <summary>
        /// 供應商商品刪除
        /// </summary>
        /// <returns>響應結果</returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase VendorProductDelete()
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];

            _productTempMgr = new ProductTempMgr(connectionString);
            int writerId = (int)vendorModel.vendor_id;
            string json = "{success:false}";

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');
                    _productMgr = new ProductMgr(connectionString);
                    foreach (string product_id in pro_Ids.Distinct())
                    {
                        if (!string.IsNullOrEmpty(product_id))
                        {
                            ProductTemp product_temp = new ProductTemp();
                            _productTempMgr = new ProductTempMgr(connectionString);
                            product_temp = _productTempMgr.GetVendorProTemp(new ProductTemp { Product_Id = product_id, Writer_Id = writerId });
                            int combo_type = product_temp.Combo_Type;
                            bool isDelete = _productTempMgr.DeleteVendorProductTemp(writerId, combo_type, product_id == string.Empty ? "0" : product_id);
                            if (isDelete)
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
        #region 取消送審
        /// <summary>
        /// 取消送審
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase Product_Cancel()
        {
            string resultStr = "{success:false}";

            //初始化數據庫連接
            _productTempMgr = new ProductTempMgr(connectionString);

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');

                    foreach (string item in pro_Ids.Distinct())
                    {
                        ProductTemp productTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = item, Temp_Status = 12, Create_Channel = 2 }).FirstOrDefault();
                        if (productTemp != null)
                        {
                            productTemp.Product_Status = 0;
                            _productTempMgr.CancelVerify(productTemp);
                        }
                    }
                }
                resultStr = "{success:true}";
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

        #region 排序設定
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase sortSet()
        {
            string result = "{success:false}";

            string strJson = Request.Params["result"];
            _productMgr = new ProductMgr(connectionString);
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<Product> productList = js.Deserialize<List<Product>>(strJson);
            if (_productMgr.ExecUpdateSort(productList))
            {
                result = "{success:true}";
            }
            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 代申請審核
        /// <summary>
        /// 代申請審核商品查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase replaceVerifyQuery()
        {
            List<QueryandVerifyCustom> resultList = new List<QueryandVerifyCustom>();
            string result = "{success:false}";
            try
            {
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                int startPage = Convert.ToInt32(Request.Form["start"] ?? "0");
                int endPage = Convert.ToInt32(Request.Form["limit"] ?? "10");
                QueryVerifyCondition query = new QueryVerifyCondition();
                if (!string.IsNullOrEmpty(Request.Params["queryCondition"]))
                {
                    query = jsSer.Deserialize<QueryVerifyCondition>(Request.Params["queryCondition"]);
                    if (query.category_id == 2)         //Root表示全選
                    {
                        query.category_id = 0;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                }
                query.Start = startPage;
                query.Limit = endPage;
                _productMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                resultList = _productMgr.verifyWaitQuery(query, out totalCount);

                foreach (QueryandVerifyCustom item in resultList)
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

        #region 申請審核
        /// <summary>
        /// 申請審核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase VerifyApply()
        {
            string prodcutIdStr = Request.Params["prodcutIdStr"];

            List<QueryandVerifyCustom> resultList = new List<QueryandVerifyCustom>();
            string result = "{success:false}";

            //初始化數據庫連接
            _productTempMgr = new ProductTempMgr(connectionString);

            try
            {
                string[] productIds = prodcutIdStr.Split(',');
                foreach (string item in productIds.Distinct())
                {
                    ProductTemp productTemp = _productTempMgr.GetProTempByVendor(new ProductTemp { Product_Id = item, Temp_Status = 12, Create_Channel = 2 }).FirstOrDefault();
                    if (productTemp != null)
                    {
                        productTemp.Product_Status = 1;    //狀態 -> 申請審核
                        _productTempMgr.UpdateAchieve(productTemp);
                    }
                }
                result = "{success:true}";
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

        #region 價格審核列表查詢
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

        [HttpPost]
        public HttpResponseBase QueryPriceVerifyList()
        {
            string json = string.Empty;
            JavaScriptSerializer jsSer = new JavaScriptSerializer();
            int startPage = Convert.ToInt32(Request.Form["start"] ?? "0");
            int endPage = Convert.ToInt32(Request.Form["limit"] ?? "10");
            QueryVerifyCondition qvCon = new QueryVerifyCondition();
            if (!string.IsNullOrEmpty(Request.Params["queryCondition"]))
            {
                qvCon = jsSer.Deserialize<QueryVerifyCondition>(Request.Params["queryCondition"]);
            }
            qvCon.Start = startPage;
            qvCon.Limit = endPage;
            string time_start = string.Empty;
            string time_end = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["time_start"]))
            {
                if (qvCon.date_type == "apply_time")//time 為整形
                {
                    time_start = CommonFunction.GetPHPTime(Request.Params["time_start"]).ToString();
                    //time_end = CommonFunction.GetPHPTime(Request.Params["time_end"]).ToString();
                }
                else
                {
                    time_start = Request.Params["time_start"];
                    //time_end = Request.Params["time_end"];
                }
                qvCon.time_start = time_start;
            }
            if (!string.IsNullOrEmpty(Request.Params["time_end"]))
            {
                if (qvCon.date_type == "apply_time")//time 為整形
                {
                    time_start = CommonFunction.GetPHPTime(Request.Params["time_end"]).ToString();
                }
                else
                {
                    time_start = Request.Params["time_end"];
                }
                qvCon.time_end = time_end;
            }
            int total = 0;
            _productMgr = new ProductMgr(connectionString);
            try
            {
                List<QueryandVerifyCustom> qvCusList = _productMgr.QueryandVerify(qvCon, ref total);
                foreach (QueryandVerifyCustom item in qvCusList)
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
                json = "{success:true,total:" + total + ",data:" + JsonConvert.SerializeObject(qvCusList) + "}";
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
                            ptstQuery.Writer_Id = Convert.ToInt32((Session["vendor"] as BLL.gigade.Model.Vendor).vendor_id);
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

        #region 上次異動紀錄
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryLastModifyRecord()
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    StringBuilder html = new StringBuilder();
                    int productId = Convert.ToInt32(Request.Form["Product_Id"]);
                    bool isPro = (Request.Form["Type"] ?? "") == "product";
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _tableHistoryItemMgr = new TableHistoryItemMgr(connectionString);
                    TableHistory query = new TableHistory { pk_value = productId.ToString() };
                    if (isPro)
                    {
                        query.table_name = "product";
                    }
                    TableHistory lastRec = _tableHistoryMgr.QueryLastModifyByProductId(query);
                    if (lastRec != null)
                    {
                        List<TableHistory> histories = _tableHistoryMgr.Query(new TableHistory { batchno = lastRec.batchno });
                        if (histories != null && histories.Count > 0)
                        {
                            List<string> tbls = histories.GroupBy(m => m.table_name).Select(m => m.Key).ToList();
                            uint site = 0, level = 0, userid = 0;
                            string[] priceTable = { "price_master", "item_price" };
                            if (isPro)
                            {
                                tbls.RemoveAll(m => priceTable.Contains(m));
                            }
                            else
                            {
                                uint.TryParse(Request.Form["Site_id"], out site);
                                uint.TryParse(Request.Form["User_Level"], out level);
                                uint.TryParse(Request.Form["User_id"], out userid);
                                tbls.RemoveAll(m => !priceTable.Contains(m));
                            }
                            List<TableHistoryItem> items;
                            #region 初始化
                            StringBuilder pro = new StringBuilder();
                            StringBuilder spec = new StringBuilder();
                            StringBuilder category = new StringBuilder();
                            StringBuilder item = new StringBuilder();
                            StringBuilder master = new StringBuilder();
                            StringBuilder price = new StringBuilder();
                            NotificationController notification = new NotificationController();
                            #endregion
                            foreach (var tbl in tbls)
                            {
                                string tblName = tbl.ToString().ToLower();
                                bool isAdd = false;
                                #region 針對不同表的處理
                                switch (tblName)
                                {
                                    case "product":
                                        #region PRODUCT
                                        items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = lastRec.batchno, table_name = tblName });
                                        if (items != null && items.Count > 0)
                                        {
                                            StringBuilder column_1 = new StringBuilder("<tr><td style=\"border:1px solid #99bce8;\">欄位名稱</td>");
                                            StringBuilder column_2 = new StringBuilder("<tr><td style=\"border:1px solid #99bce8;\">修改前</td>");
                                            StringBuilder column_3 = new StringBuilder("<tr><td style=\"border:1px solid #99bce8;\">修改後</td>");
                                            Array cols = items.GroupBy(m => m.col_name).Select(m => m.Key).ToArray();
                                            foreach (var col in cols)
                                            {
                                                var tmp = items.FindAll(m => m.col_name == col.ToString());
                                                if (tmp.Count == 1 && string.IsNullOrEmpty(tmp.FirstOrDefault().old_value))
                                                { continue; }
                                                else
                                                {
                                                    tmp.Remove(tmp.Find(m => string.IsNullOrEmpty(m.old_value)));
                                                    var first = tmp.FirstOrDefault();
                                                    var last = tmp.LastOrDefault();
                                                    if (first == last)
                                                    {
                                                        notification.GetParamCon(last, true);
                                                    }
                                                    else
                                                    {
                                                        notification.GetParamCon(first, true);
                                                    }
                                                    notification.GetParamCon(last, false);
                                                    column_1.AppendFormat("<td style=\"border:1px solid #99bce8;\">{0}</td>", first.col_chsname);
                                                    column_2.AppendFormat("<td style=\"border:1px solid #99bce8;color:Red;\">{0}</td>", first == last ? last.old_value : first.old_value);//class=\"red\" 
                                                    column_3.AppendFormat("<td style=\"border:1px solid #99bce8;color:green;\">{0}</td>", last.col_value);//class=\"green\"
                                                    isAdd = true;
                                                }
                                            }
                                            if (isAdd)
                                            {
                                                pro.AppendFormat("<table style=\"width:180px;text-align:center;font-size: 13px;border:1px solid #99bce8;\">{0}</tr>{1}</tr>{2}</tr></table>", column_1, column_2, column_3);//class=\"tbptstyle\"
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "product_spec":
                                        #region SPEC
                                        StringBuilder spec_1 = new StringBuilder("<tr><td style=\"border:1px solid #99bce8;\">修改前</td>");
                                        StringBuilder spec_2 = new StringBuilder("<tr><td style=\"border:1px solid #99bce8;\">修改後</td>");
                                        Array specIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in specIds)
                                        {
                                            items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = lastRec.batchno, table_name = tblName, pk_value = id.ToString() });
                                            if (items.Count == 1 && string.IsNullOrEmpty(items.FirstOrDefault().old_value))
                                            { continue; }
                                            else
                                            {
                                                items.Remove(items.Find(m => string.IsNullOrEmpty(m.old_value)));
                                                var first = items.FirstOrDefault();
                                                var last = items.LastOrDefault();
                                                spec_1.AppendFormat("<td class=\"red\" style=\"border:1px solid #99bce8;color:Red;\">{0}</td>", first == last ? last.old_value : first.old_value);
                                                spec_2.AppendFormat("<td class=\"green\" style=\"border:1px solid #99bce8;color:green;\">{0}</td>", last.col_value);
                                                isAdd = true;
                                            }
                                        }
                                        if (isAdd)
                                        {
                                            spec.AppendFormat("<table style=\"width:180px;text-align:center;font-size: 13px;border:1px solid #99bce8;\">{0}</tr>{1}</tr></table>", spec_1, spec_2);//class=\"tbptstyle\"
                                        }
                                        #endregion
                                        break;
                                    case "product_category_set":
                                        #region CATEGORY
                                        items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = lastRec.batchno, table_name = tblName, pk_value = productId.ToString() });
                                        if (items.Count > 0)
                                        {
                                            var first = items.FirstOrDefault();
                                            var last = items.LastOrDefault();
                                            category.Append("<table style=\"width:180px;text-align:center;font-size: 13px;border:1px solid #99bce8;\"><tr><td style=\"border:1px solid #99bce8;\">修改前</td><td style=\"border:1px solid #99bce8;\">修改後</td></tr>");// class=\"tbptstyle\"
                                            category.AppendFormat("<tr><td class=\"red\" style=\"border:1px solid #99bce8;color:Red;\">{0}</td>", first == last ? last.old_value : first.old_value);
                                            category.AppendFormat("<td class=\"green\" style=\"border:1px solid #99bce8;color:green;\">{0}</td></td></table>", last.col_value);
                                        }
                                        #endregion
                                        break;
                                    case "product_item":
                                        #region ITEM
                                        ProductItem pItem;
                                        _productItemMgr = new ProductItemMgr(connectionString);
                                        Array itemIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in itemIds)
                                        {
                                            isAdd = false;
                                            pItem = _productItemMgr.Query(new ProductItem { Item_Id = uint.Parse(id.ToString()) }).FirstOrDefault();
                                            if (pItem != null)
                                            {
                                                string title = pItem.GetSpecName();
                                                string top = "<div style=\"float:left\"><table style=\"width:180px;text-align:center;font-size: 13px;border:1px solid #99bce8;\"><caption style=\"text-align:center;border:1px solid #99bce8;\">" + title + "</caption><tr><td style=\"border:1px solid #99bce8;\">欄位名稱</td><td style=\"border:1px solid #99bce8;\">修改前</td><td style=\"border:1px solid #99bce8;\">修改后</td></tr>";//class=\"tbstyle\"
                                                string bottom = "</table></div>";
                                                string strContent = "<tr><td style=\"border:1px solid #99bce8;\">{0}</td><td class=\"red\" style=\"border:1px solid #99bce8;color:Red;\">{1}</td><td class=\"green\" style=\"border:1px solid #99bce8;color:green;\">{2}</td></tr>";
                                                string content = notification.BuildContent(lastRec.batchno, tblName, id.ToString(), strContent, ref isAdd);
                                                if (isAdd)
                                                {
                                                    item.Append(top);
                                                    item.Append(content);
                                                    item.Append(bottom);
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "price_master":
                                        #region PRICE_MASTER
                                        PriceMaster pMaster;
                                        _pMaster = new PriceMasterMgr(connectionString);
                                        Array masterIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in masterIds)
                                        {
                                            isAdd = false;
                                            pMaster = _pMaster.Query(new PriceMaster { price_master_id = uint.Parse(id.ToString()) }).FirstOrDefault();
                                            if (pMaster != null && pMaster.site_id == site && pMaster.user_level == level && pMaster.user_id == userid)
                                            {
                                                string siteName = notification.QuerySiteName(pMaster.site_id.ToString());
                                                string userLevel = notification.QueryParaName(pMaster.user_level.ToString(), "UserLevel");
                                                string userMail = pMaster.user_id == 0 ? "" : notification.QueryMail(pMaster.user_id.ToString());
                                                string childName = string.Empty;
                                                if (pMaster.child_id != 0 && pMaster.product_id != pMaster.child_id)
                                                {
                                                    _productMgr = new ProductMgr(connectionString);
                                                    Product tmpPro = _productMgr.Query(new Product { Product_Id = Convert.ToUInt32(pMaster.child_id) }).FirstOrDefault();
                                                    if (tmpPro != null)
                                                    {
                                                        childName = tmpPro.Product_Name;
                                                    }
                                                }
                                                string title = siteName + " + " + userLevel + (string.IsNullOrEmpty(userMail) ? "" : (" + " + userMail))
                                                                + (string.IsNullOrEmpty(childName) ? "<br/>" : "<br/>子商品: " + childName);
                                                if (!title.Contains("子商品"))
                                                {
                                                    title += "<br/>";
                                                }
                                                string top = "<div style=\"float:left\"><table style=\"width:180px;text-align:center;font-size: 13px;border:1px solid #99bce8;\"><caption style=\"text-align:center;border:1px solid #99bce8;\">" + title + "</caption><tr><td style=\"border:1px solid #99bce8;\">欄位名稱</td><td style=\"border:1px solid #99bce8;\">修改前</td><td style=\"border:1px solid #99bce8;\">修改后</td></tr>";// class=\"tbstyle\" 
                                                string bottom = "</table></div>";
                                                string strContent = "<tr><td style=\"border:1px solid #99bce8;\">{0}</td><td class=\"red\" style=\"border:1px solid #99bce8;color:Red;\">{1}</td><td class=\"green\" style=\"border:1px solid #99bce8;color:green;\">{2}</td></tr>";
                                                string content = notification.BuildContent(lastRec.batchno, tblName, id.ToString(), strContent, ref isAdd);
                                                if (isAdd)
                                                {
                                                    master.Append(top);
                                                    master.Append(content);
                                                    master.Append(bottom);
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "item_price":
                                        #region ITEM_PRICE

                                        ItemPriceCustom itemPrice;
                                        PriceMaster tmpMaster;
                                        _itemPriceMgr = new ItemPriceMgr(connectionString);
                                        _pMaster = new PriceMasterMgr(connectionString);
                                        Array priceIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in priceIds)
                                        {
                                            isAdd = false;
                                            itemPrice = _itemPriceMgr.Query(new ItemPrice { item_price_id = uint.Parse(id.ToString()) }).FirstOrDefault();
                                            if (itemPrice != null)
                                            {
                                                tmpMaster = _pMaster.Query(new PriceMaster { price_master_id = itemPrice.price_master_id }).FirstOrDefault();
                                                if (tmpMaster != null && tmpMaster.site_id == site && tmpMaster.user_level == level && tmpMaster.user_id == userid)
                                                {
                                                    string siteName = notification.QuerySiteName(tmpMaster.site_id.ToString());
                                                    string userLevel = notification.QueryParaName(tmpMaster.user_level.ToString(), "UserLevel");
                                                    string userMail = tmpMaster.user_id == 0 ? "" : notification.QueryMail(tmpMaster.user_id.ToString());
                                                    string childName = string.Empty;
                                                    if (tmpMaster.child_id != 0 && tmpMaster.product_id != tmpMaster.child_id)
                                                    {
                                                        _productMgr = new ProductMgr(connectionString);
                                                        Product tmpPro = _productMgr.Query(new Product { Product_Id = Convert.ToUInt32(tmpMaster.child_id) }).FirstOrDefault();
                                                        if (tmpPro != null)
                                                        {
                                                            childName = tmpPro.Product_Name;
                                                        }
                                                    }
                                                    string strSpec = itemPrice.spec_name_1 + (string.IsNullOrEmpty(itemPrice.spec_name_2) ? "" : (" + " + itemPrice.spec_name_2));

                                                    string title = siteName + " + " + userLevel + (string.IsNullOrEmpty(userMail) ? "" : (" + " + userMail))
                                                        + (string.IsNullOrEmpty(childName) ? "<br/>" : "<br/>子商品: " + childName)
                                                        + "<br/>" + strSpec;
                                                    if (strSpec == "")
                                                    {
                                                        title += "<br/>";
                                                    }
                                                    string top = "<div style=\"float:left\"><table style=\"width:180px;text-align:center;font-size: 13px;border:1px solid #99bce8;\"><caption style=\"text-align:center;border:1px solid #99bce8;\">" + title + "</caption><tr><td style=\"border:1px solid #99bce8;\">欄位名稱</td><td style=\"border:1px solid #99bce8;\">修改前</td><td style=\"border:1px solid #99bce8;\">修改后</td></tr>";//class=\"tbstyle\"
                                                    string bottom = "</table></div>";
                                                    string strContent = "<tr><td style=\"border:1px solid #99bce8;\">{0}</td><td class=\"red\" style=\"border:1px solid #99bce8;color:Red;\">{1}</td><td class=\"green\" style=\"border:1px solid #99bce8;color:green;\">{2}</td></tr>";
                                                    string content = notification.BuildContent(lastRec.batchno, tblName, id.ToString(), strContent, ref isAdd);
                                                    if (isAdd)
                                                    {
                                                        price.Append(top);
                                                        price.Append(content);
                                                        price.Append(bottom);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }
                            #region 批次拼接
                            StringBuilder batchHtml = new StringBuilder();
                            if (pro.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td style=\"border:1px solid #99bce8;\">商品信息</td><td style=\"border:1px solid #99bce8;\">{0}</td></tr>", pro);
                            }
                            if (spec.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td style=\"border:1px solid #99bce8;\">規格信息</td><td style=\"border:1px solid #99bce8;\">{0}</td></tr>", spec);
                            }
                            if (category.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td style=\"border:1px solid #99bce8;\">前臺分類信息</td><td style=\"border:1px solid #99bce8;\">{0}</td></tr>", category);
                            }
                            if (item.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td style=\"border:1px solid #99bce8;\">商品細項信息</td><td style=\"border:1px solid #99bce8;\">{0}</td></tr>", item);
                            }
                            if (master.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td style=\"border:1px solid #99bce8;\">站臺商品信息</td><td style=\"border:1px solid #99bce8;\">{0}</td></tr>", master);
                            }
                            if (price.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td style=\"border:1px solid #99bce8;\">站臺價格信息</td><td style=\"border:1px solid #99bce8;\">{0}</td></tr>", price);
                            };
                            if (batchHtml.Length > 0)
                            {
                                _productMgr = new ProductMgr(connectionString);
                                Product product = _productMgr.Query(new Product { Product_Id = Convert.ToUInt32(productId) }).FirstOrDefault();
                                if (product != null)
                                {
                                    string brand = string.Empty;
                                    vbMgr = new VendorBrandMgr(connectionString);
                                    VendorBrand vendorBrand = vbMgr.GetProductBrand(new VendorBrand { Brand_Id = product.Brand_Id });
                                    if (vendorBrand != null)
                                    {
                                        brand = vendorBrand.Brand_Name;
                                    }
                                    _historyBatchMgr = new HistoryBatchMgr(connectionString);
                                    HistoryBatch batch = _historyBatchMgr.Query(new HistoryBatch { batchno = lastRec.batchno });
                                    html.Append("<html><head><style type=\"text/css\">table{ font-size: 13px;border:1px solid #99bce8}td{border:1px solid #99bce8} .tbstyle{width:180px;text-align:center;} .red{color:Red;}.green{color:green;} caption{text-align:center;border:1px solid #99bce8}</style></head><body>");
                                    html.AppendFormat("<table style=\"font-size: 13px;border:1px solid #99bce8;\"><tr><td colspan='2' style=\"border:1px solid #99bce8;\">商品編號：<b>{0}</b>   品牌：<b>{1}</b></td></tr>", productId, brand);
                                    html.AppendFormat("<tr><td colspan='2' style=\"border:1px solid #99bce8;\"><b>{0}</b>  (修改人:{1}", product.Product_Name, batch.kuser);
                                    html.AppendFormat(",修改時間:{0})</td></tr>", batch.kdate.ToString("yyyy/MM/dd HH:mm:ss"));
                                    html.Append(batchHtml);
                                    html.Append("</table>");
                                    html.Append("</body></html>");
                                }
                            }
                            #endregion
                        }
                        result = "{success:true,html:'" + HttpUtility.HtmlEncode(html.ToString()) + "'}";
                    }
                    else
                    {
                        result = "{success:true,html:''}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                result = "{success:true,html:''}";
            }
            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
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
                    uint productid = 0;
                    if (uint.TryParse(Request.Form["ProductId"].ToString(), out productid))
                    {
                        List<ProductStatusHistoryCustom> resultList = _mgr.HistoryQuery(new ProductStatusHistoryCustom { product_id = uint.Parse(Request.Params["ProductId"]) });
                        json = "{success:true,data:" + JsonConvert.SerializeObject(resultList) + "}";
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

        #region 查询站臺
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
                cateList = getCate(categoryList, 0);
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
        public List<ProductCategoryCustom> getCate(List<ProductCategory> categoryList, uint fatherId)
        {
            var cateList = (from c in categoryList
                            where c.category_father_id == fatherId
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
        #region 遞歸得到分類節點
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetFrontCateList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, uint.Parse(item.id.ToString()));
                item.children = childList;

                if (childList.Count() > 0)
                {
                    GetFrontCateList(categoryList, ref childList);
                }
            }
        }
        #endregion
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
                            queryProductNotice.Writer_Id = Convert.ToInt32((Session["vendor"] as BLL.gigade.Model.Vendor).vendor_id);
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
    }
}
