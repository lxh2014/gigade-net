using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using System.Configuration;
using Newtonsoft.Json;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Collections;
using System.Web.Security;
using System.Net.Mail;
using BLL.gigade.Dao;
using System.Data;
using System.Text.RegularExpressions;
using BLL.gigade.Model.Temp;

using gigadeExcel.Comment;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private static DataTable DTExcel = new DataTable();
        private IProductImplMgr _productMgr;
        private IProductTempImplMgr _productTempMgr;
        private IProductCategoryImplMgr _procateMgr;
        private IProductCategorySetImplMgr _categorySetMgr;
        private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        private IProductSpecImplMgr _specMgr;
        private IProductSpecTempImplMgr _specTempMgr;
        private IProductItemTempImplMgr _productItemTempMgr;
        private IProductItemImplMgr _productItemMgr;
        private IProductPictureTempImplMgr _pPicTempMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IPriceMasterTsImplMgr _priceMasterTsMgr;
        private IItemPriceImplMgr _itemPriceMgr;
        private IItemPriceTsImplMgr _itemPriceTsMgr;
        private IProductTagImplMgr _productTagMgr;
        private IProductTagSetImplMgr _productTagSetMgr;
        private IProductTagSetTempImplMgr _productTagSetTempMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetImplMgr _productNoticeSetMgr;
        private IProductNoticeSetTempImplMgr _productNoticeSetTempMgr;
        private IProductPictureImplMgr _productPicMgr;
        private ITableHistoryImplMgr _tableHistoryMgr;
        private IUsersImplMgr _usersMgr;
        //private VendorBrandMgr vbMgr;
        private ParameterMgr paraMgr;
        private ISiteConfigImplMgr siteConfigMgr;
        private ISiteImplMgr _siteMgr;
        private ISerialImplMgr _serialMgr;
        private IPriceMasterTempImplMgr _priceMasterTempMgr;
        private IPriceUpdateApplyImplMgr _priceUpdateApplyMgr;
        private IPriceUpdateApplyHistoryImplMgr _priceUpdateApplyHistoryMgr;
        private IFunctionImplMgr _functionMgr;

        private IProductDeliverySetImplMgr _productDeliverySetMgr;
        private IProductDeliverySetTempImplMgr _productDeliverySetTempMgr;

        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];

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

        string descMobilePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descMobilePath);
        string desc400MobilePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.desc400MobilePath);

        string tagPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_tagPath);

        string prodMobile640 = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_mobile640); //add by wwei0216w 2015/4/1 添加原因:手機圖片要放在640*640路徑下

        //string noticePath = ConfigurationManager.AppSettings["prod_noticePath"];
        //string notice400Path = ConfigurationManager.AppSettings["prod_notice400Path"];
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];
        private int COMBO_TYPE = 1;

        private int defaultImgLength = 5;
        private int imgNameIdx = 7;    //按‘/’分割第 n 个为图片名称
        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private static List<ProductStockQuery> productStockStores = null;
        private IProductStockImportImplMgr _pStockMgr;
        private IProductSpecImplMgr _pSpecStatusMgr;
        private VendorBrandMgr vbMgr;
        #region View

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult upLoad()
        {
            return View();
        }
        public ActionResult ProductSave(string id)
        {
            ViewBag.ProductId = id;
            ViewBag.OldProductId = Request.QueryString["product_id"] ?? "";
            ViewBag.hfAuth = Request.Cookies[FormsAuthentication.FormsCookieName] == null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value;
            ViewBag.hfAspSessID = Session.SessionID;
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

        /// <summary>
        /// 添加物流模式設定
        /// </summary>
        /// <returns></returns>
        public ActionResult transportSet()
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

        public ActionResult CourseDetailItem()
        {
            return View();
        }

        public ActionResult Price()
        {
            return View();
        }

        public ActionResult Description()
        {
            return View();
        }

        /// <summary>
        /// 類別商品維護
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryProductMaintain()
        {
            return View();
        }
        /// <summary>
        /// 匯入商品庫存頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductStockImportIndex()
        {
            return View();
        }
        /// <summary>
        /// 商品類別批次匯入
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductCategoryEnter()
        {
            return View();
        }
        /// <summary>
        /// 商品詳情頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductDetailList()
        {
            return View();
        }
        #endregion

        #region  商品基本信息

        [HttpPost]
        public HttpResponseBase SaveBaseInfo(int course_id = 0)
        {
            string json = "{success:true}";
            int transportDays = -1;
            try
            {
                string prod_name = Request.Form["prod_name"] ?? "";
                string prod_sz = Request.Form["prod_sz"] ?? "";
                if (!Product.CheckProdName(prod_name) || !Product.CheckProdName(prod_sz))
                {
                    json = "{success:false,msg:'" + Resources.Product.FORBIDDEN_CHARACTER + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }

                ///add by wwei0216w 2015/8/24
                ///根據product_mode查找供應商對應的自出,寄倉,調度欄位,如果為0則不予保存
                uint brand_id = uint.Parse(Request.Form["brand_id"]??"0");
                uint product_mode = uint.Parse(Request.Form["product_mode"]??"0");///獲得product_mode
                string msg = "寄倉";
                IVendorImplMgr _vendorMgr = new VendorMgr(connectionString);
                List<Vendor> vendorList = _vendorMgr.GetArrayDaysInfo(brand_id);
                if(vendorList.Count > 0)
                {
                    switch (product_mode)
                    { 
                        case 1:
                            transportDays = vendorList.FirstOrDefault<Vendor>().self_send_days;
                            msg = "自出";
                            break;
                        case 2:
                            transportDays = vendorList.FirstOrDefault<Vendor>().stuff_ware_days;
                            msg = "寄倉";
                            break;
                        case 3:
                            msg = "調度";
                            transportDays = vendorList.FirstOrDefault<Vendor>().dispatch_days;
                            break;
                        default:
                            break;
                    }
                }

                if (transportDays==0)
                {
                    json = "{success:false,msg:'" + msg + Resources.Product.TRANSPORT_DAYS + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }


                ProductTemp pTemp = new ProductTemp();
                _productTempMgr = new ProductTempMgr(connectionString);
                _productMgr = new ProductMgr(connectionString);

                Caller _caller = (Session["caller"] as Caller);
                Product Query = new Product();
                Product p = new Product();
                //查詢product表。
                if (Request.Params["product_id"] != "")
                {
                    Query.Product_Id = uint.Parse(Request.Params["product_id"]);
                    p = _productMgr.Query(Query)[0];
                }

                
                uint product_sort = uint.Parse(Request.Form["product_sort"]);
                uint product_start = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_start"]).ToString());
                uint product_end = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_end"]).ToString());
                uint expect_time = uint.Parse(CommonFunction.GetPHPTime(Request.Form["expect_time"]).ToString());
                uint product_freight_set = uint.Parse(Request.Form["product_freight_set"]);


                string product_vendor_code = Request.Form["product_vendor_code"];
                int tax_type = int.Parse(Request.Form["tax_type"]);
                string expect_msg = Request.Form["expect_msg"] ?? "";
                //商品新增欄位 add by  xiangwang0413w 2014/09/15
                int show_in_deliver = int.Parse(Request.Form["show_in_deliver"]);
                int prepaid = int.Parse(Request.Form["prepaid"]);
                int process_type = int.Parse(Request.Form["process_type"]);
                int product_type = int.Parse(Request.Form["product_type"]);

                //商品新增欄位  add by zhuoqin0830w  2015/03/17
                int deliver_days = int.Parse(Request.Form["deliver_days"]);
                int min_purchase_amount = int.Parse(Request.Form["min_purchase_amount"]);
                double safe_stock_amount = double.Parse(Request.Form["safe_stock_amount"]);
                int extra_days = 0;

                int purchase_in_advance = Convert.ToInt32(Request.Form["purchase_in_advance"]);
                uint purchase_in_advance_start = uint.Parse(Request.Form["purchase_in_advance_start"]);
                uint purchase_in_advance_end = uint.Parse(Request.Form["purchase_in_advance_end"]);

                uint recommedde_jundge = uint.Parse(Request.Form["recommedde_jundge"]);//是否選擇了推薦商品屬性 1 表示推薦
                string recommededcheckall = string.Empty;
                uint recommedde_expend_day = 0;
                if (recommedde_jundge == 1)
                {
                    if (!string.IsNullOrEmpty(Request.Params["recommededcheckall"]))
                    {
                        recommededcheckall = Request.Params["recommededcheckall"].ToString().TrimEnd(',');//選擇的所有的月數
                    }
                    recommedde_expend_day = uint.Parse(Request.Form["recommedde_expend_day"]);
                }
                pTemp.Brand_Id = brand_id;
                pTemp.Prod_Name = prod_name;
                pTemp.Prod_Sz = prod_sz;
                pTemp.Product_Name = pTemp.GetProductName();


                pTemp.Product_Sort = product_sort;
                pTemp.Product_Vendor_Code = product_vendor_code;
                pTemp.Product_Start = product_start;
                pTemp.Product_End = product_end;
                pTemp.Expect_Time = expect_time;
                pTemp.Product_Freight_Set = product_freight_set;
                pTemp.Product_Mode = product_mode;
                pTemp.Tax_Type = tax_type;
                pTemp.expect_msg = expect_msg;
                pTemp.Create_Channel = 1;// 1:後台管理者(manage_user) edit by xiagnwang0413w 2014/08/09
                //商品新增欄位 add by  xiangwang0413w 2014/09/15
                pTemp.Show_In_Deliver = show_in_deliver;
                pTemp.Prepaid = prepaid;
                pTemp.Process_Type = process_type;
                pTemp.Product_Type = product_type;

                //商品新增欄位  add by zhuoqin0830w  2015/03/17
                pTemp.Deliver_Days = deliver_days;
                pTemp.Min_Purchase_Amount = min_purchase_amount;
                pTemp.Safe_Stock_Amount = safe_stock_amount;
                pTemp.Extra_Days = extra_days;

                //add 2015/07/30
                pTemp.purchase_in_advance = purchase_in_advance;
                pTemp.purchase_in_advance_start = purchase_in_advance_start;
                pTemp.purchase_in_advance_end = purchase_in_advance_end;


                //add dongya 2015/08/17
                pTemp.recommedde_jundge = recommedde_jundge;//推薦商品 1表示推薦 0表示不推薦
                pTemp.months = recommededcheckall;//以1,3,這樣的形式顯示
                pTemp.expend_day = recommedde_expend_day;
                
                if (Request.Params["product_id"] != "")
                {
                    p.Brand_Id = brand_id;
                    p.Prod_Name = prod_name;
                    p.Prod_Sz = prod_sz;
                    p.Product_Name = p.GetProductName();

                    p.Product_Sort = product_sort;
                    p.Product_Vendor_Code = product_vendor_code;
                    p.Product_Start = product_start;
                    p.Product_End = product_end;
                    p.Expect_Time = expect_time;
                    p.Product_Freight_Set = product_freight_set;
                    p.Product_Mode = product_mode;
                    p.Tax_Type = tax_type;
                    p.expect_msg = expect_msg;
                    p.Show_In_Deliver = show_in_deliver;
                    p.Prepaid = prepaid;
                    p.Process_Type = process_type;
                    p.Product_Type = product_type;

                    //商品新增欄位  add by zhuoqin0830w  2015/03/17
                    p.Deliver_Days = deliver_days;
                    p.Min_Purchase_Amount = min_purchase_amount;
                    p.Safe_Stock_Amount = safe_stock_amount;
                    p.Extra_Days = extra_days;
                    p.off_grade = int.Parse(Request.Form["off-grade"]);
                    p.purchase_in_advance = purchase_in_advance;
                    p.purchase_in_advance_start = purchase_in_advance_start;
                    p.purchase_in_advance_end = purchase_in_advance_end;
                    p.recommedde_jundge = recommedde_jundge;//表示是否推薦
                    p.months = recommededcheckall;
                    p.expend_day = recommedde_expend_day;
                }

                if (string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    //查找臨時表是否存在數據，存在：更新，不存在插入
                    pTemp.Writer_Id = _caller.user_id;
                    pTemp.Product_Status = 0;
                    pTemp.Combo_Type = COMBO_TYPE;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }

                    ProductTemp pTempList = _productTempMgr.GetProTemp(new ProductTemp { Writer_Id = pTemp.Writer_Id, Combo_Type = COMBO_TYPE, Product_Id = pTemp.Product_Id });
                    if (pTempList == null)
                    {
                        //插入
                        int result = 0;
                        result = _productTempMgr.baseInfoSave(pTemp);
                        if (result >0)//裡面加入一個新的sql
                        {
                            json = "{success:true}";
                        }
                        else
                        {
                            json = "{success:false}";
                        }
                    }
                    else
                    {
                        //更新
                        if (pTemp.Product_Mode != 2)
                        {
                            pTemp.Bag_Check_Money = 0;
                        }
                        else
                        {
                            pTemp.Bag_Check_Money = pTempList.Bag_Check_Money;
                        }
                        _productTempMgr.baseInfoUpdate(pTemp);
                    }

                    #region 商品為課程
                    if (course_id != 0) //edit by xiangwang0413w 2015/03/10
                    {
                        ICourseProductTempImplMgr _courseProductTempMgr = new CourseProductTempMgr(connectionString);
                        var courProdTemp = new CourseProductTemp { Writer_Id = _caller.user_id, Course_Id = course_id, Product_Id = 0 };
                        var courseResult = false;
                        if (_courseProductTempMgr.Query(courProdTemp) == null)
                        {
                            courseResult = _courseProductTempMgr.Save(courProdTemp);
                        }
                        else
                        {
                            courseResult = _courseProductTempMgr.Update(courProdTemp);
                        }

                        json = "{success:" + courseResult.ToString().ToLower() + "}";
                    }
                    #endregion

                }
                else
                {
                    //更新正式表
                    p.Product_Id = uint.Parse(Request.Params["product_id"]);
                    if (p.Product_Mode != 2)
                    {
                        p.Bag_Check_Money = 0;
                    }

                    _functionMgr = new FunctionMgr(connectionString);

                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid };
                    batch.batchno = Request.Params["batch"] ?? "";
                    batch.kuser = _caller.user_email;

                    _productMgr = new ProductMgr(connectionString);
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);//add by wwei0216w 2015/2/24 品牌名稱變更后,product_category_set表所對應的品牌名稱也需要更新
                    ArrayList aList = new ArrayList();
                    aList.Add(_productMgr.Update(p, _caller.user_id));
                    aList.Add(_categorySetMgr.UpdateBrandId(new ProductCategorySet { Product_Id = p.Product_Id, Brand_Id = p.Brand_Id })); //add by wwei0216w 2015/2/24 品牌名稱變更后,product_category_set表所對應的品牌名稱也需要更新
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    if (_tableHistoryMgr.SaveHistory<Product>(p, batch, aList))
                    {
                        //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                        //if (p.Combination == 1)
                        //{
                        //    _productItemMgr = new ProductItemMgr(connectionString);
                        //    ProductItem pro_Item = new ProductItem() { Product_Id = p.Product_Id, Export_flag = 2 };
                        //    _productItemMgr.UpdateExportFlag(pro_Item);
                        //}


                        #region add by zhuoqin0830w  2015/06/25  判斷修改的商品是否是失格商品  1為失格 0為正常
                        if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                        {

                            _productMgr.UpdateOff_Grade(p.Product_Id, p.off_grade);
                        }
                        #endregion

                        //add by wwei0216 2015/1/9 刪除不符合條件的物品匹配模式
                        if (!p.CheckdStoreFreight())
                        {
                            _productDeliverySetMgr = new ProductDeliverySetMgr(connectionString);
                            _productDeliverySetMgr.Delete(
                                new ProductDeliverySet { Freight_big_area = 1, Freight_type = 12 }, p.Product_Id);
                        }

                        #region ScheduleRelation
                        if (!string.IsNullOrEmpty(Request.Form["schedule_id"]))
                        {
                            IScheduleRelationImplMgr _srMgr = new ScheduleRelationMgr(connectionString);
                            _srMgr.Save(new ScheduleRelation { relation_table = "product", relation_id = (int)p.Product_Id, schedule_id = int.Parse(Request.Form["schedule_id"]) });
                        }
                        #endregion

                        #region 推薦商品屬性插入/修改recommended_product_attribute表中做記錄 add by dongya 2015/09/30 ----目前只針對單一商品
                        RecommendedProductAttributeMgr rProductAttributeMgr = new RecommendedProductAttributeMgr(connectionString);
                        RecommendedProductAttribute rPA = new RecommendedProductAttribute();
                        rPA.product_id = Convert.ToUInt32(Query.Product_Id);
                        rPA.time_start = pTemp.Recommended_time_start;
                        rPA.time_end = pTemp.Recommended_time_end;
                        rPA.expend_day = pTemp.expend_day;
                        rPA.months = pTemp.months;
                        rPA.combo_type = 1;
                        //首先判斷表中是否對該product_id設置為推薦
                        int productId = Convert.ToInt32(rPA.product_id);
                        if (rProductAttributeMgr.GetMsgByProductId(productId) > 0)//如果大於0,表示推薦表中存在數據
                        {
                            if (pTemp.recommedde_jundge == 1)//==1表示推薦 
                            {
                                rProductAttributeMgr.Update(rPA);
                            }
                            else if (pTemp.recommedde_jundge == 0)//==0表示不推薦 
                            {
                                rProductAttributeMgr.Delete(productId);
                            }
                        }
                        else
                        {
                            if (pTemp.recommedde_jundge == 1)//==1表示推薦 
                            {
                                rProductAttributeMgr.Save(rPA);
                            }
                        }
                        #endregion

                        json = "{success:true,msg:'" + Resources.Product.SAVE_SUCCESS + "'}";
                    }
                    else
                    {
                        json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                    }
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

        #region  庫存
        [HttpPost]
        [CustomHandleError]
        public string QueryStock()
        {
            string json = string.Empty;
            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                //查找臨時表是否有記錄
                _productItemTempMgr = new ProductItemTempMgr(connectionString);
                int writeId = (Session["caller"] as Caller).user_id;
                ProductItemTemp query = new ProductItemTemp { Writer_Id = writeId };
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    query.Product_Id = Request.Form["OldProductId"];
                }
                json = _productItemTempMgr.QueryStock(query);
            }
            else
            {
                //從正式表讀取數據
                _productItemMgr = new ProductItemMgr(connectionString);
                ProductItem pItem = new ProductItem();
                pItem.Product_Id = uint.Parse(Request.Params["product_id"]);
                _productItemMgr.Query(pItem);
                json = _productItemMgr.QueryStock(pItem);
            }
            return json;
        }
        //單一商品庫存保存
        [HttpPost]
        public HttpResponseBase StockSave()
        {
            ProductItemTemp piTemp = new ProductItemTemp();
            _productItemMgr = new ProductItemMgr(connectionString);
            ProductItem pItem = new ProductItem();
            string[] Value = Request.Params["ig_sh_InsertValue"].Split(',');

            string json = "{success:true}";

            _productItemTempMgr = new ProductItemTempMgr(connectionString);
            int writeId = (Session["caller"] as Caller).user_id;
            _tableHistoryMgr = new TableHistoryMgr(connectionString);

            List<ProductItemTemp> piTempList = new List<ProductItemTemp>();
            List<ProductItem> pItemList = new List<ProductItem>();

            //edit by wwei0216w 2015/6/11 修改原因:之前代碼在判斷時會做出誤判,因為spilt用(,)進行分割,導致字符串中本來是用(,)時,也被分割
            if (!string.IsNullOrEmpty(Request.Form["InsertValue"]))
            {
                List<ProductItem> list = JsonConvert.DeserializeObject<List<ProductItem>>(Request.Form["InsertValue"].ToString());
                foreach (var p_item in list)
                {
                    piTemp = new ProductItemTemp();
                    pItem = new ProductItem();
                    piTemp.Writer_Id = writeId;
                    if (!string.IsNullOrEmpty(p_item.Item_Id.ToString())) { piTemp.Item_Id = p_item.Item_Id; pItem.Item_Id = p_item.Item_Id; };
                    if (Request.Params["product_id"] != "")
                    {
                        pItem.Product_Id = uint.Parse(Request.Params["product_id"]);
                        pItem = _productItemMgr.Query(pItem)[0];
                    }
                    if (!string.IsNullOrEmpty(p_item.Spec_Id_1.ToString())) { piTemp.Spec_Id_1 = p_item.Spec_Id_1; pItem.Spec_Id_1 = p_item.Spec_Id_1; };
                    if (!string.IsNullOrEmpty(p_item.Spec_Id_2.ToString())) { piTemp.Spec_Id_2 = p_item.Spec_Id_2; pItem.Spec_Id_2 = p_item.Spec_Id_2; };
                    if (!string.IsNullOrEmpty(p_item.Item_Stock.ToString())) { piTemp.Item_Stock = p_item.Item_Stock; pItem.Item_Stock = p_item.Item_Stock; };
                    if (!string.IsNullOrEmpty(p_item.Item_Alarm.ToString())) { piTemp.Item_Alarm = p_item.Item_Alarm; pItem.Item_Alarm = p_item.Item_Alarm; };
                    if (!string.IsNullOrEmpty(p_item.Barcode.ToString())) { piTemp.Barcode = p_item.Barcode; pItem.Barcode = p_item.Barcode; };
                    if (!string.IsNullOrEmpty(p_item.Item_Code.ToString())) { piTemp.Item_Code = p_item.Item_Code; pItem.Item_Code = p_item.Item_Code; }
                    if (!string.IsNullOrEmpty(p_item.Erp_Id.ToString())) { piTemp.Erp_Id = p_item.Erp_Id; pItem.Erp_Id = p_item.Erp_Id; }
                    // add by zhuoqin0830w 2014/02/05 增加備註
                    if (!string.IsNullOrEmpty(p_item.Remark.ToString())) { piTemp.Remark = p_item.Remark; pItem.Remark = p_item.Remark; }
                    // add by zhuoqin0830w 2014/03/20 增加運達天數
                    if (!string.IsNullOrEmpty(p_item.Arrive_Days.ToString())) { piTemp.Arrive_Days = p_item.Arrive_Days; pItem.Arrive_Days = p_item.Arrive_Days; }
                    piTempList.Add(piTemp);
                    pItemList.Add(pItem);
                }

                //string[] Values = Request.Form["InsertValue"].ToString().Split(';');
                //for (int i = 0; i < Values.Length - 1; i++)
                //{
                //    piTemp = new ProductItemTemp();
                //    pItem = new ProductItem();
                //    piTemp.Writer_Id = writeId;
                //    string[] perValue = Values[i].Split(',');
                //    //查詢product_item數據
                //    if (!string.IsNullOrEmpty(perValue[5])) { piTemp.Item_Id = uint.Parse(perValue[5]); pItem.Item_Id = uint.Parse(perValue[5]); };
                //    if (Request.Params["product_id"] != "")
                //    {
                //        pItem.Product_Id = uint.Parse(Request.Params["product_id"]);
                //        pItem = _productItemMgr.Query(pItem)[0];
                //    }
                //    if (!string.IsNullOrEmpty(perValue[0])) { piTemp.Spec_Id_1 = uint.Parse(perValue[0]); pItem.Spec_Id_1 = uint.Parse(perValue[0]); };
                //    if (!string.IsNullOrEmpty(perValue[1])) { piTemp.Spec_Id_2 = uint.Parse(perValue[1]); pItem.Spec_Id_2 = uint.Parse(perValue[1]); };
                //    if (!string.IsNullOrEmpty(perValue[2])) { piTemp.Item_Stock = int.Parse(perValue[2]); pItem.Item_Stock = int.Parse(perValue[2]); };
                //    if (!string.IsNullOrEmpty(perValue[3])) { piTemp.Item_Alarm = uint.Parse(perValue[3]); pItem.Item_Alarm = uint.Parse(perValue[3]); };
                //    if (!string.IsNullOrEmpty(perValue[4])) { piTemp.Barcode = perValue[4]; pItem.Barcode = perValue[4]; };
                //    if (!string.IsNullOrEmpty(perValue[6])) { piTemp.Item_Code = perValue[6]; pItem.Item_Code = perValue[6]; }
                //    if (!string.IsNullOrEmpty(perValue[7])) { piTemp.Erp_Id = perValue[7]; pItem.Erp_Id = perValue[7]; }
                //    // add by zhuoqin0830w 2014/02/05 增加備註
                //    if (!string.IsNullOrEmpty(perValue[8])) { piTemp.Remark = perValue[8]; pItem.Remark = perValue[8]; }
                //    // add by zhuoqin0830w 2014/03/20 增加運達天數
                //    if (!string.IsNullOrEmpty(perValue[9])) { piTemp.Arrive_Days = int.Parse(perValue[9]); pItem.Arrive_Days = int.Parse(perValue[9]); }
                //    piTempList.Add(piTemp);
                //    pItemList.Add(pItem);
                //}
            }
            //判斷單一商品是新增還是修改
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {//修改單一商品時執行
                Product p = new Product();
                _functionMgr = new FunctionMgr(connectionString);
                string function = Request.Params["function"] ?? "";
                Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                int functionid = fun == null ? 0 : fun.RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid };
                batch.batchno = Request.Params["batch"] ?? "";
                batch.kuser = (Session["caller"] as Caller).user_email;
                //更新product
                _productMgr = new ProductMgr(connectionString);
                Product Query = new Product();
                //查詢product表。
                Query.Product_Id = uint.Parse(Request.Params["product_id"]);
                p = _productMgr.Query(Query)[0];
                p.Shortage = int.Parse(Value[1]);
                p.Ignore_Stock = int.Parse(Value[0]);
                json = "{success:true,msg:'" + Resources.Product.SAVE_SUCCESS + "'}";
                try
                {
                    foreach (var item in pItemList)
                    {
                        ArrayList arrList = new ArrayList();
                        arrList.Add(_productItemMgr.Update(item));
                        _tableHistoryMgr.SaveHistory<ProductItem>(item, batch, arrList);
                    }
                    ArrayList proList = new ArrayList();
                    proList.Add(_productMgr.Update(p));
                    _tableHistoryMgr.SaveHistory<Product>(p, batch, proList);
                    //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                    //if (p.Combination == 1)
                    //{
                    //    _productItemMgr = new ProductItemMgr(connectionString);
                    //    ProductItem pro_Item = new ProductItem() { Product_Id = p.Product_Id, Export_flag = 2 };
                    //    _productItemMgr.UpdateExportFlag(pro_Item);
                    //}
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
            else
            {//新增單一商品時
                try
                {
                    //更新product_temp
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp pTemp = new ProductTemp();

                    pTemp.Ignore_Stock = int.Parse(Value[0]);
                    pTemp.Shortage = int.Parse(Value[1]);
                    pTemp.Writer_Id = writeId;
                    pTemp.Combo_Type = COMBO_TYPE;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }

                    _productTempMgr.ProductTempUpdate(pTemp, "stock");

                    piTempList.ForEach(m => { m.Product_Id = pTemp.Product_Id; });


                    _productItemTempMgr.UpdateStockAlarm(piTempList);
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

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region  圖檔

        [HttpPost]
        public string QueryExplainPic()
        {
            int apporexplain = Convert.ToInt32(Request["apporexplain"]);
            string json = string.Empty;
            SetPath(apporexplain);
            string serverDescPath = imgServerPath + descPath;
            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                //查找臨時表記錄
                _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
                int writer_Id = (Session["caller"] as Caller).user_id;
                ProductPictureTemp query = new ProductPictureTemp { writer_Id = writer_Id, combo_type = COMBO_TYPE };
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    query.product_id = Request.Form["OldProductId"];
                }

                List<ProductPictureTemp> picList = _pPicTempMgr.Query(query, apporexplain);
                //if (apporexplain == 1) { serverDescPath = imgServerPath + descPath; }
                //else { serverDescPath = imgServerPath + descMobilePath; }
                foreach (var item in picList)
                {
                    if (item.image_filename != "")
                    {
                        if (item.pic_type == 2) ///edti by wwei0216w 當是手機圖片時,將查找的物理路徑改變為手機的路徑
                        {
                            serverDescPath = imgServerPath + "/product_picture/mobile/";
                        }

                        item.image_filename = serverDescPath + GetDetailFolder(item.image_filename) + item.image_filename;
                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(picList) + "}";
                json = json.Replace("image_filename", "img");


            }
            else
            {//查詢正式表                
                _productPicMgr = new ProductPictureMgr(connectionString);
                int product_id = int.Parse(Request.Params["product_id"]);
                List<ProductPicture> pList = _productPicMgr.Query(product_id, apporexplain);
                //if (apporexplain == 1) { serverDescPath = imgServerPath + descPath; }
                //else { serverDescPath = imgServerPath + descMobilePath; }
                foreach (var item in pList)
                {
                    if (item.image_filename != "")
                    {
                        if (item.pic_type == 2)  ///edti by wwei0216w 當是手機圖片時,將查找的物理路徑改變為手機的路徑
                        {
                            serverDescPath = imgServerPath + "/product_picture/mobile/";
                        }
                        item.image_filename = serverDescPath + GetDetailFolder(item.image_filename) + item.image_filename;
                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(pList) + "}";
                json = json.Replace("image_filename", "img");
            }
            return json;
        }
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
        public HttpResponseBase DeletePic()
        {
            string json = "{success:true,msg:\"" + Resources.Product.DELETE_SUCCESS + "\",path:\"" + default50Path + "\"}";
            try
            {
                string deleteType = Request.Params["type"];
                int apporexplain = Convert.ToInt32(Request["apporexplain"]);//判斷是從前臺的APP傳來還是從explain傳來
                ProductSpecTemp psTemp = new ProductSpecTemp();
                ProductSpec pSpec = new ProductSpec();
                List<ProductSpecTemp> psList = new List<ProductSpecTemp>();
                List<ProductSpec> pspList = new List<ProductSpec>();
                _specTempMgr = new ProductSpecTempMgr(connectionString);
                _specMgr = new ProductSpecMgr(connectionString);

                string[] records = Request.Params["rec"].Split('|');
                foreach (var item in records)
                {
                    string[] record = item.Split(',');
                    if (record[0].Split('/').Length == defaultImgLength)   //默认图片无法删除
                    {
                        json = "{success:false,msg:\"" + Resources.Product.DEFAULT_CANNOT_DELETE + "\"}";
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
                        pspList.Add(pSpec);
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
                        SetPath(apporexplain);
                        string imageName = imgLocalPath + descPath + GetDetailFolder(fileName) + fileName;
                        string imageName400 = imgLocalPath + desc400Path + GetDetailFolder(fileName) + fileName;
                        DeletePicFile(imageName);
                        DeletePicFile(imageName400);
                    }
                }

                //string[] record = Request.Params["rec"].Split(',');
                //if (record[0].Split('/').Length == defaultImgLength)   //默认图片无法删除
                //{
                //    json = "{success:false,msg:\"" + Resources.Product.DEFAULT_CANNOT_DELETE + "\"}";
                //    this.Response.Clear();
                //    this.Response.Write(json);
                //    this.Response.End();
                //    return this.Response;
                //}
                //string fileName = record[0].Split('/')[imgNameIdx];
                //if (deleteType == "spec")
                //{
                //    psTemp.spec_image = string.Empty;
                //    psTemp.spec_id = uint.Parse(record[1]);
                //    psTemp.spec_sort = uint.Parse(record[2]);
                //    psTemp.spec_status = uint.Parse(record[3]);
                //    pSpec.spec_image = string.Empty;
                //    pSpec.spec_id = uint.Parse(record[1]);
                //    pSpec.spec_sort = uint.Parse(record[2]);
                //    pSpec.spec_status = uint.Parse(record[3]);
                //    psList.Add(psTemp);
                //    string imageName = imgLocalPath + specPath + GetDetailFolder(fileName) + fileName;
                //    string image100 = imgLocalPath + spec100Path + GetDetailFolder(fileName) + fileName;
                //    string image280 = imgLocalPath + spec280Path + GetDetailFolder(fileName) + fileName;
                //    //刪除服務器上對應的圖片
                //    DeletePicFile(imageName);
                //    DeletePicFile(image100);
                //    DeletePicFile(image280);
                //}
                //else if (deleteType == "desc")
                //{
                //    //if (apporexplain == 1)
                //    //{
                //    SetPath(apporexplain);
                //    string imageName = imgLocalPath + descPath + GetDetailFolder(fileName) + fileName;
                //    string imageName400 = imgLocalPath + desc400Path + GetDetailFolder(fileName) + fileName;
                //    DeletePicFile(imageName);
                //    DeletePicFile(imageName400);
                //    //}
                //    //else if(apporexplain == 2)
                //    //{
                //    //    string imageName = imgLocalPath + descMobilePath + GetDetailFolder(fileName) + fileName;
                //    //    string imageName400 = imgLocalPath + desc400MobilePath + GetDetailFolder(fileName) + fileName;
                //    //    DeletePicFile(imageName);
                //    //    DeletePicFile(imageName400);
                //    //}

                //}

                if (string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    psTemp.Writer_Id = (Session["caller"] as Caller).user_id;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        psTemp.product_id = Request.Form["OldProductId"];
                    }

                    _specTempMgr.Update(psList, "image");

                }
                else
                {


                    uint productId = uint.Parse(Request.Params["product_id"]);
                    foreach (var item in pspList)
                    {
                        item.product_id = productId;
                        _specMgr.UpdateSingle(item);
                    }
                    //pSpec.product_id = uint.Parse(Request.Params["product_id"]);
                    //_specMgr.UpdateSingle(pSpec);

                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Product.DELETE_SPEC_FAIL + "\"}";
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
            try
            {
                ProductTemp pTemp = new ProductTemp();
                _productTempMgr = new ProductTempMgr(connectionString);
                _specTempMgr = new ProductSpecTempMgr(connectionString);
                _productPicMgr = new ProductPictureMgr(connectionString);
                _specMgr = new ProductSpecMgr(connectionString);

                if (string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["image_InsertValue"])) pTemp.Product_Image = Request.Params["image_InsertValue"];
                    if (!string.IsNullOrEmpty(Request.Params["image_MobileValue"])) pTemp.Mobile_Image = Request.Params["image_MobileValue"];//如果手機說明圖有值,將值賦予Moibile_Image edit by wwei0216w 2015/3/18 
                    if (!string.IsNullOrEmpty(Request.Params["specify_Product_alt"])) pTemp.Product_alt = Request.Params["specify_Product_alt"];//add by wwei0216w 2015/4/9
                    if (!string.IsNullOrEmpty(Request.Params["productMedia"])) pTemp.product_media = Request.Params["productMedia"];
                    pTemp.Writer_Id = (Session["caller"] as Caller).user_id;
                    pTemp.Combo_Type = COMBO_TYPE;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }

                    //關於規格圖的代碼部份
                    ProductSpecTemp pSpec = new ProductSpecTemp();
                    List<ProductSpecTemp> pSpecList = new List<ProductSpecTemp>();
                    if (!string.IsNullOrEmpty(Request.Params["spec_InsertValue"]))
                    {
                        string[] Values = Request.Form["spec_InsertValue"].ToString().Split(';');
                        for (int i = 0; i < Values.Length - 1; i++)
                        {
                            pSpec = new ProductSpecTemp();
                            pSpec.Writer_Id = (Session["caller"] as Caller).user_id;
                            pSpec.product_id = pTemp.Product_Id;
                            string[] perValue = Values[i].Split(',');
                            if (!string.IsNullOrEmpty(perValue[0])) { pSpec.spec_image = perValue[0]; };
                            if (!string.IsNullOrEmpty(perValue[1])) { pSpec.spec_id = uint.Parse(perValue[1]); };
                            if (!string.IsNullOrEmpty(perValue[2])) { pSpec.spec_sort = uint.Parse(perValue[2]); };
                            if (!string.IsNullOrEmpty(perValue[3])) { pSpec.spec_status = uint.Parse(perValue[3]); };
                            pSpecList.Add(pSpec);
                        }
                    }

                    //關於說明圖的代碼部份
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
                            pPic.writer_Id = (Session["caller"] as Caller).user_id;
                            pPic.product_id = pTemp.Product_Id;
                            if (!string.IsNullOrEmpty(perValue[0])) { pPic.image_filename = perValue[0]; };
                            if (!string.IsNullOrEmpty(perValue[1])) { pPic.image_sort = uint.Parse(perValue[1]); };
                            if (!string.IsNullOrEmpty(perValue[2])) { pPic.image_state = uint.Parse(perValue[2]); };
                            picList.Add(pPic);
                        }
                    }

                    //關於手機APP的代碼部份
                    List<ProductPictureTemp> picAppList = new List<ProductPictureTemp>();
                    //IProductPictureTempImplAppMgr ppt = new ProductPictureAppTempImplMgr(connectionString);
                    ProductPictureTemp pa = new ProductPictureTemp();
                    if (!string.IsNullOrEmpty(Request.Params["mobilePic_InsertValue"]))
                    {
                        string[] Values = Request.Form["mobilePic_InsertValue"].ToString().Split(';');
                        for (int i = 0; i < Values.Length - 1; i++)
                        {
                            pa = new ProductPictureTemp();
                            string[] AppValue = Values[i].Split(',');
                            pa.combo_type = COMBO_TYPE;
                            pa.writer_Id = (Session["caller"] as Caller).user_id;
                            pa.product_id = pTemp.Product_Id;
                            if (!string.IsNullOrEmpty(AppValue[0])) { pa.image_filename = AppValue[0]; };
                            if (!string.IsNullOrEmpty(AppValue[1])) { pa.image_sort = uint.Parse(AppValue[1]); };
                            if (!string.IsNullOrEmpty(AppValue[2])) { pa.image_state = uint.Parse(AppValue[2]); };
                            picAppList.Add(pa);
                        }
                    }
                    int type = 1;//1：商品說明圖,2：手機App說明圖

                    int writer_id = (Session["caller"] as Caller).user_id;
                    //保存至productTemp
                    if (pTemp.Product_Image != "" || pTemp.product_media != "" || pTemp.Mobile_Image != "" || pTemp.Product_alt != "") // edit by wwei0216w 2015/3/18 添加pTemp.Moibile_Image的判斷
                    {
                        _productTempMgr.ProductTempUpdate(pTemp, "pic");
                    }
                    //保存規格圖
                    _specTempMgr.Update(pSpecList, "image");
                    //保存說明圖
                    ProductPictureTemp proPictureTemp = new ProductPictureTemp { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = pTemp.Product_Id };
                    _pPicTempMgr.Save(picList, proPictureTemp, type);

                    type = 2;

                    _pPicTempMgr.Save(picAppList, proPictureTemp, type);
                    ////保存手機圖 add by wwei 0216w 2014/11/11
                    //ProductPictureAppTemp proAppTemp = new ProductPictureAppTemp { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = pTemp.Product_Id };
                    //ppt.Save(picAppList, proAppTemp);
                }
                else//更新正式表
                {
                    Product p = new Product();
                    uint productId = uint.Parse(Request.Params["product_id"]);
                    if (!string.IsNullOrEmpty(Request.Params["image_InsertValue"])) p.Product_Image = Request.Params["image_InsertValue"];//如果商品說明圖有值,將值賦予Product_Image
                    if (!string.IsNullOrEmpty(Request.Params["image_MobileValue"])) p.Mobile_Image = Request.Params["image_MobileValue"];//如果手機說明圖有值,將值賦予Moibile_Image edit by wwei0216w 2015/3/18 
                    if (!string.IsNullOrEmpty(Request.Params["productMedia"])) p.product_media = Request.Params["productMedia"];
                    if (!string.IsNullOrEmpty(Request.Params["specify_Product_alt"])) p.Product_alt = Request.Params["specify_Product_alt"];//add by wwei0216w 2015/4/9
                    p.Product_Id = productId;
                    _productMgr = new ProductMgr(connectionString);

                    ProductSpec pSpec = new ProductSpec();
                    List<ProductSpec> pSpecList = new List<ProductSpec>();
                    if (!string.IsNullOrEmpty(Request.Params["spec_InsertValue"]))
                    {
                        string[] Values = Request.Form["spec_InsertValue"].ToString().Split(';');
                        for (int i = 0; i < Values.Length - 1; i++)
                        {
                            pSpec = new ProductSpec();
                            pSpec.product_id = productId;
                            string[] perValue = Values[i].Split(',');
                            if (!string.IsNullOrEmpty(perValue[0])) { pSpec.spec_image = perValue[0]; };
                            if (!string.IsNullOrEmpty(perValue[1])) { pSpec.spec_id = uint.Parse(perValue[1]); };
                            if (!string.IsNullOrEmpty(perValue[2])) { pSpec.spec_sort = uint.Parse(perValue[2]); };
                            if (!string.IsNullOrEmpty(perValue[3])) { pSpec.spec_status = uint.Parse(perValue[3]); };
                            pSpecList.Add(pSpec);
                        }
                    }

                    List<ProductPicture> picList = new List<ProductPicture>();
                    _productPicMgr = new ProductPictureMgr(connectionString);
                    ProductPicture pPic = new ProductPicture();

                    if (!string.IsNullOrEmpty(Request.Params["picture_InsertValue"]))
                    {
                        string[] Values = Request.Form["picture_InsertValue"].ToString().Split(';');
                        for (int i = 0; i < Values.Length - 1; i++)
                        {
                            pPic = new ProductPicture();
                            string[] perValue = Values[i].Split(',');
                            pPic.product_id = int.Parse(Request.Params["product_id"]);
                            if (!string.IsNullOrEmpty(perValue[0])) { pPic.image_filename = perValue[0]; };
                            if (!string.IsNullOrEmpty(perValue[1])) { pPic.image_sort = uint.Parse(perValue[1]); };
                            if (!string.IsNullOrEmpty(perValue[2])) { pPic.image_state = uint.Parse(perValue[2]); };
                            picList.Add(pPic);
                        }
                    }

                    //手機app圖檔
                    List<ProductPicture> appList = new List<ProductPicture>();
                    _productPicMgr = new ProductPictureMgr(connectionString);
                    ProductPicture apppPic = new ProductPicture();

                    if (!string.IsNullOrEmpty(Request.Params["mobilePic_InsertValue"]))
                    {
                        string[] Values = Request.Form["mobilePic_InsertValue"].ToString().Split(';');
                        for (int i = 0; i < Values.Length - 1; i++)
                        {
                            apppPic = new ProductPicture();
                            string[] perValue = Values[i].Split(',');
                            apppPic.product_id = int.Parse(Request.Params["product_id"]);
                            if (!string.IsNullOrEmpty(perValue[0])) { apppPic.image_filename = perValue[0]; };
                            if (!string.IsNullOrEmpty(perValue[1])) { apppPic.image_sort = uint.Parse(perValue[1]); };
                            if (!string.IsNullOrEmpty(perValue[2])) { apppPic.image_state = uint.Parse(perValue[2]); };
                            appList.Add(apppPic);
                        }
                    }
                    _productMgr.Update_Product_Spec_Picture(p, pSpecList, picList, appList);

                    //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                    //if (p.Combination == 1)
                    //{
                    //    _productItemMgr = new ProductItemMgr(connectionString);
                    //    ProductItem pro_Item = new ProductItem() { Product_Id = p.Product_Id, Export_flag = 2 };
                    //    _productItemMgr.UpdateExportFlag(pro_Item);
                    //}

                    json = "{success:true,msg:\"" + Resources.Product.SAVE_SUCCESS + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Product.SAVE_FAIL + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        //判斷類型
        public void SetPath(int type)
        {
            switch (type)
            {
                case 1:
                    descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descPath);
                    break;
                case 2:
                    descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descMobilePath);
                    break;
                default:
                    break;
            }

        }

        [HttpPost]
        public ActionResult upLoadImg()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["Filedata"];
                int type = Request["appOrexplain"] == null ? 0 : Convert.ToInt32(Request["appOrexplain"]);
                string nameType = Request.Params["nameType"];  // 將 nametype 提前 使其判斷傳入的圖片是否為商品主圖 edit by zhuoqin0830w 2015/01/29
                int prodCheck = file.FileName.LastIndexOf("prod_");  // 將 proCheck 提前 使其判斷批量上傳的圖片中是否存在商品主圖   edit by zhuoqin0830w 2015/01/30
                if (nameType == "spec")
                {
                    type = 5;
                }
                if (prodCheck == 0 && type == 0) //批量上傳時type才會 ==0,其他單獨上傳時是不需要根據prod處理圖片的
                {
                    type = 3;
                }
                int mobileCheck = file.FileName.LastIndexOf("mobile_");//批量上傳時type才會 ==0,其他單獨上傳時是不需要根據prod處理圖片的
                if (mobileCheck == 0 && type == 0)
                {
                    type = 4;
                }
                string path = Server.MapPath(xmlPath);
                siteConfigMgr = new SiteConfigMgr(path);
                SiteConfig extention_config = siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                SiteConfig minValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
                SiteConfig maxValue_config = null;

                //判斷 批量上傳 或 單個上傳 的圖片是否為 商品主圖 或 手機商品圖  edit by zhuoqin0830w 2015/03/24
                switch (nameType)
                {
                    //如果  nameType == prod  則表示 是 單個上傳 商品主圖
                    case "prod":
                        maxValue_config = siteConfigMgr.GetConfigByName("PIC_280_Length_Max");
                        break;
                    //如果  nameType == mobile  則表示 是 單個上傳 手機商品圖 
                    case "mobile":
                        maxValue_config = siteConfigMgr.GetConfigByName("PIC_640_Length_Max");
                        break;
                    //如果  nameType == null  則表示 是 批量上傳
                    case null:
                        //如果  prodCheck == 0  則表示 是 批量上傳 中包含 商品主圖 
                        if (prodCheck == 0)
                        {
                            maxValue_config = siteConfigMgr.GetConfigByName("PIC_280_Length_Max");
                        }
                        //如果  mobileCheck == 0  則表示 是 批量上傳 中包含 手機商品圖 
                        else if (mobileCheck == 0)
                        {
                            maxValue_config = siteConfigMgr.GetConfigByName("PIC_640_Length_Max");
                        }
                        else
                        {
                            maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                        }
                        break;
                    default:
                        maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                        break;
                }

                string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;


                if ((nameType == "mobile" && type == 4) || (mobileCheck == 0 && type == 4)) //add by wwei0216w 2015/4/1 添加原因:手機圖片要放在640*640路徑下
                {
                    prodPath = prodMobile640;
                }
                string localProdPath = imgLocalPath + prodPath;

                //string localProd50Path = imgLocalPath + prod50Path;
                //string localProd150Path = imgLocalPath + prod150Path;
                //string localProd280Path = imgLocalPath + prod280Path;

                string localSpecPath = imgLocalPath + specPath;
                //string localSpec100Path = imgLocalPath + spec100Path;
                //string localSpec280Path = imgLocalPath + spec280Path;

                string[] Mappath = new string[2];

                FileManagement fileLoad = new FileManagement();

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
                    //int prodCheck = file.FileName.LastIndexOf("prod_");
                    int specCheck = file.FileName.LastIndexOf("spec_");
                    int descCheck = file.FileName.LastIndexOf("desc_");
                    int appCheck = file.FileName.LastIndexOf("app_");
                    string errorMsg = "ERROR/";
                    if (prodCheck == -1 && specCheck == -1 && descCheck == -1 && appCheck == -1 && mobileCheck == -1)
                    {
                        errorMsg += "[" + file.FileName + "] ";
                        errorMsg += Resources.Product.FILE_NAME_ERROR;
                        ViewBag.fileName = errorMsg;
                        return View("~/Views/Product/upLoad.cshtml");
                    }
                    else
                    {
                        nameType = file.FileName.Split('_')[0];
                        if (nameType == "app")
                        {
                            type = 2;
                        }
                        else if (nameType == "desc")
                        {
                            type = 1;
                        }
                        else if (nameType == "spec")
                        {
                            type = 5;
                        }
                        fileName = nameType + fileLoad.NewFileName(file.FileName);
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                    }
                    if (specCheck == 0)
                    {
                        Caller _caller = (Session["caller"] as Caller);
                        string spec = file.FileName.Split('_')[1].Split('.')[0];
                        bool checkStatus = true;
                        if (!string.IsNullOrEmpty(Request.Params["product_id"].Split(';')[0].ToString()))
                        {
                            //product_spec
                            uint pid = uint.Parse(Request.Params["product_id"].Split(';')[0].ToString());

                            _specMgr = new ProductSpecMgr(connectionString);
                            List<ProductSpec> pSpecList = _specMgr.Query(new ProductSpec { product_id = pid, spec_type = 1 });
                            foreach (var item in pSpecList)
                            {
                                if (item.spec_name == spec)
                                {
                                    checkStatus = false;
                                    ViewBag.spec_id = item.spec_id;
                                }
                            }
                        }
                        else
                        {
                            //product_spec_temp
                            _specTempMgr = new ProductSpecTempMgr(connectionString);
                            List<ProductSpecTemp> pSpecTempList = _specTempMgr.Query(new ProductSpecTemp { Writer_Id = _caller.user_id, spec_type = 1 });
                            foreach (var item in pSpecTempList)
                            {
                                if (item.spec_name == spec)
                                {
                                    checkStatus = false;
                                    ViewBag.spec_id = item.spec_id;
                                }
                            }
                        }
                        if (checkStatus)//表示沒有要上傳圖片規格相同的規格一
                        {
                            errorMsg += "[" + file.FileName + "] " + Resources.Product.SPEC_NOT_FIND;
                            ViewBag.fileName = errorMsg;
                            return View("~/Views/Product/upLoad.cshtml");
                        }
                    }
                    #endregion
                }
                SetPath(type);//設定圖片路徑
                string localDescPath = imgLocalPath + descPath;
                //string localDesc400Path = imgLocalPath + desc400Path;
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
                else if (nameType == "desc" || nameType == "app")
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
                Resource.CoreMessage = new CoreResource("Product");

                //上傳圖片

                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                #region //
                //上傳對應大小圖片
                //压缩图片至其它规格

                //Data:2014/06/26
                //author：Castle
                //在前台如果各种尺寸的图档没有的时候，前台会自动产生！！！
                //if (result)
                //{
                //    FTP ftp = null;
                //    string newFileName = NewFileName.Substring(NewFileName.LastIndexOf("/"));

                //    GigadeService.TransImageClient transImg = new GigadeService.TransImageClient();
                //    if (nameType == "spec")
                //    {
                //        string sourceImgPath = Server.MapPath(imgLocalServerPath + specPath + NewFileName.Substring(NewFileName.LastIndexOf("/")));
                //        ErrorMsg = transImg.Trans(sourceImgPath, Server.MapPath(imgLocalServerPath + spec100Path + firstFolder + secondFolder), newFileName, 100, 100, admin_userName.Value, admin_passwd.Value);
                //        if (string.IsNullOrWhiteSpace(ErrorMsg))
                //        {
                //            file.SaveAs(Server.MapPath(imgLocalServerPath + spec100Path + firstFolder + secondFolder + newFileName));
                //            ftp = new FTP(localSpec100Path, ftpuser, ftppwd);
                //            ftp.UploadFile(Server.MapPath(imgLocalServerPath + spec100Path + firstFolder + secondFolder + newFileName));
                //        }

                //        if (!Directory.Exists(Server.MapPath(imgLocalServerPath + spec280Path + firstFolder + secondFolder)))
                //            Directory.CreateDirectory(Server.MapPath(imgLocalServerPath + spec280Path + firstFolder + secondFolder));
                //        ErrorMsg = transImg.Trans(sourceImgPath, Server.MapPath(imgLocalServerPath + spec280Path + firstFolder + secondFolder), newFileName, 280, 280, admin_userName.Value, admin_passwd.Value);
                //        if (string.IsNullOrWhiteSpace(ErrorMsg))
                //        {
                //            file.SaveAs(Server.MapPath(imgLocalServerPath + spec280Path + firstFolder + secondFolder + newFileName));
                //            ftp = new FTP(localSpec100Path, ftpuser, ftppwd);
                //            ftp.UploadFile(Server.MapPath(imgLocalServerPath + spec280Path + firstFolder + secondFolder + newFileName));
                //        }
                //    }
                //    else if (nameType == "desc")
                //    {
                //        string sourceImgPath = Server.MapPath(imgLocalServerPath + descPath + NewFileName.Substring(NewFileName.LastIndexOf("/")));
                //        ErrorMsg = transImg.Trans(sourceImgPath, Server.MapPath(imgLocalServerPath + desc400Path + firstFolder + secondFolder), newFileName, 400, 400, admin_userName.Value, admin_passwd.Value);
                //        if (string.IsNullOrWhiteSpace(ErrorMsg))
                //        {
                //            file.SaveAs(Server.MapPath(imgLocalServerPath + desc400Path + firstFolder + secondFolder + newFileName));
                //            ftp = new FTP(localSpec100Path, ftpuser, ftppwd);
                //            ftp.UploadFile(Server.MapPath(imgLocalServerPath + desc400Path + firstFolder + secondFolder + newFileName));
                //        }
                //    }
                //    else
                //    {
                //        //string sourceImgPath = Server.MapPath(imgLocalServerPath + prodPath + NewFileName.Substring(NewFileName.LastIndexOf("/")));
                //        //ErrorMsg = transImg.Trans(sourceImgPath, Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder), newFileName, 50, 50, admin_userName.Value, admin_passwd.Value);
                //        //if (string.IsNullOrWhiteSpace(ErrorMsg))
                //        //{
                //        //    file.SaveAs(Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder + newFileName));
                //        //    ftp = new FTP(localProd50Path, ftpuser, ftppwd);
                //        //    ftp.UploadFile(Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder + newFileName));
                //        //}

                //        //ErrorMsg = transImg.Trans(sourceImgPath, Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder), newFileName, 150, 150, admin_userName.Value, admin_passwd.Value);
                //        //if (string.IsNullOrWhiteSpace(ErrorMsg))
                //        //{
                //        //    file.SaveAs(Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder + newFileName));
                //        //    ftp = new FTP(localProd150Path, ftpuser, ftppwd);
                //        //    ftp.UploadFile(Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder + newFileName));
                //        //}

                //        ////if (!Directory.Exists(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder)))
                //        ////    Directory.CreateDirectory(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder));
                //        ////ImageClass iC280 = new ImageClass(sourceImgPath);
                //        //////iC280.ImageMagick(sourceImgPath, Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))), 280, 280, ref error);
                //        ////iC150.MakeThumbnail(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))), 280, 280, ref error);
                //        //ErrorMsg = transImg.Trans(sourceImgPath, Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder), newFileName, 280, 280, admin_userName.Value, admin_passwd.Value);
                //        //if (string.IsNullOrWhiteSpace(ErrorMsg))
                //        //{
                //        //    file.SaveAs(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder + newFileName));
                //        //    ftp = new FTP(localProd280Path, ftpuser, ftppwd);
                //        //    ftp.UploadFile(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder + newFileName));
                //        //}
                //    }
                //}
                #endregion
                if (string.IsNullOrEmpty(ErrorMsg))
                {
                    ViewBag.fileName = returnName;
                    ViewBag.Type = type;

                    //獲取文件長度 add by zhuoqin0830w 2015/01/29
                    string[] strFile = file.FileName.Split('_');
                    //判斷文件名的長度是否大於 1 如果大於 1 則再次進行判斷是否為數字 如果不是則進行強制轉換  
                    int image_sort = 0;
                    int.TryParse(strFile.Length > 1 ? strFile[1] : "0", out image_sort);
                    ViewBag.image_sort = image_sort;
                }
                else
                {
                    // 判斷 批量上傳中 是否存在 商品圖 或 手機商品圖  edit by zhuoqin0830w 2015/03/24
                    if (prodCheck == 0 || mobileCheck == 0)
                    { ViewBag.fileName = "ERROR/" + "[" + file.FileName + "] " + ErrorMsg; }
                    else { ViewBag.fileName = "ERROR/" + "[" + file.FileName + "] " + ErrorMsg; }

                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = "ERROR/" + ErrorMsg;
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            return View("~/Views/Product/upLoad.cshtml");
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
                string fullPath = path.Substring(0, path.Length - 1);
                string nodeDir = fullPath.Substring(fullPath.LastIndexOf("/") + 1);
                //創建跟目錄
                ftp = new FTP(fullPath.Substring(0, fullPath.LastIndexOf("/") + 1), ftpuser, ftppwd);
                if (!ftp.DirectoryExist(nodeDir))
                {
                    ftp = new FTP(fullPath, ftpuser, ftppwd);
                    ftp.MakeDirectory();
                }
                foreach (string s in Mappath)
                {
                    ftp = new FTP(fullPath.Substring(0, fullPath.Length), ftpuser, ftppwd);
                    fullPath += "/" + s;

                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length), ftpuser, ftppwd);
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
            string serverSpecPath = imgServerPath + specPath;
            string serverSpec100Path = imgServerPath + spec100Path;
            string serverSpec280Path = imgServerPath + spec280Path;
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);
            string json = string.Empty;
            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                //查找臨時表記錄
                ProductSpecTemp psTemp = new ProductSpecTemp();
                psTemp.Writer_Id = (Session["caller"] as Caller).user_id;
                psTemp.spec_type = 1;
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    psTemp.product_id = Request.Form["OldProductId"];
                }
                //string str = "{success:true,items:"+JsonConvert.+"}";
                List<ProductSpecTemp> results = _specTempMgr.Query(psTemp); //JsonConvert.SerializeObject();
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
            }
            else
            {
                //查找正式表
                ProductSpec pSpec = new ProductSpec();
                pSpec.product_id = uint.Parse(Request.Params["product_id"]);
                pSpec.spec_type = 1;
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
            }
            return json;
        }
        public void DeletePicOnServer(bool prod, bool spec, bool desc, uint spec_id, string product_id, int type = 1)
        {
            int writerId = (Session["caller"] as Caller).user_id;
            _productTempMgr = new ProductTempMgr(connectionString);
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
            ArrayList ImgList = new ArrayList();
            ProductSpecTemp pSpec = new ProductSpecTemp();
            pSpec.Writer_Id = writerId;
            pSpec.spec_type = 1;
            if (spec_id != 0)
            {
                pSpec.spec_id = spec_id;
            }

            //刪除對應的圖片
            //商品圖
            if (prod)
            {
                ProductTemp query = new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE, Product_Id = product_id };
                string fileName = _productTempMgr.GetProTemp(query).Product_Image;
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
                ProductPictureTemp query = new ProductPictureTemp { writer_Id = writerId, combo_type = COMBO_TYPE, product_id = product_id };
                List<ProductPictureTemp> pPList = _pPicTempMgr.Query(query, type);
                SetPath(type);
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

        #region 查詢商品信息

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProduct()
        {
            string json = "{success:true,data:[]}";
            try
            {
                Product product = null;
                Caller caller = Session["caller"] as Caller;
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id))
                    {
                        _productMgr = new ProductMgr(connectionString);
                        product = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                        if (product.Product_alt == "")
                        {
                            product.Product_alt = product.Product_Name;
                        }//add by wwei0216w 2015/4/15 //如果商品說明為空則將商品名稱賦予product_alt
                    }
                }
                else//查詢temp表
                {
                    _productTempMgr = new ProductTempMgr(connectionString);
                    int writerId = caller.user_id;
                    ProductTemp query = new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    product = _productTempMgr.GetProTemp(query);
                    if (product.Product_alt == "")
                    {
                        product.Product_alt = product.Product_Name;
                    }//add by wwei0216w 2015/4/15 //如果商品說明為空則將商品名稱賦予product_alt
                }
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.Product_Image))
                    {
                        product.Product_Image = imgServerPath + prodPath + GetDetailFolder(product.Product_Image) + product.Product_Image;
                    }
                    else
                    {
                        product.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                        //product.Product_Image = imgServerPath + "/Content/img/點擊上傳.jpg";
                    }
                    if (!string.IsNullOrEmpty(product.Mobile_Image)) //edit by wwei0216w 2015/3/18 添加關於手機說明圖的操作
                    {
                        prodPath = prodMobile640; //add by wwei0216w 2015/4/1 添加原因:手機圖片要放在640*640路徑下
                        product.Mobile_Image = imgServerPath + prodPath + GetDetailFolder(product.Mobile_Image) + product.Mobile_Image;
                    }
                    else
                    {
                        product.Mobile_Image = imgServerPath + "/product/nopic_150.jpg";
                        //product.Mobile_Image = imgServerPath + "/Content/img/點擊上傳圖片.jpg";
                    }
                    #region 庫存是否可編輯
                    //edit by xiangwang 0413w 2014/10/09
                    if (Request.UrlReferrer.AbsolutePath == "/Product/productStock")
                    {
                        IFgroupImplMgr _fgroupMgr = new FgroupMgr(connectionString);
                        product.IsEdit = _fgroupMgr.QueryStockPrerogative(caller.user_email, Server.MapPath(xmlPath));

                        //if (caller.user_email == "eric.liu@gigade.com.tw" || caller.user_email == "miguel@gigade.com.tw")
                        //{
                        //    product.IsEdit = true;
                        //}



                    }
                    #endregion
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(product) + "}";
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
                int writerId = (Session["caller"] as Caller).user_id;
                string product_id = "0";
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    product_id = Request.Form["OldProductId"];
                }
                _productTempMgr = new ProductTempMgr(connectionString);
                //刪除服務器上對應的圖片
                int type = 1;
                DeletePicOnServer(true, true, true, 0, product_id, type);
                type = 2;
                DeletePicOnServer(true, true, true, 0, product_id, type);
                if (_productTempMgr.DeleteTemp(writerId, COMBO_TYPE, product_id))
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

        #region 臨時表數據匯入正式表

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase Temp2Pro()
        {
            string json = string.Empty;
            try
            {
                int writerId = (Session["caller"] as Caller).user_id;
                string product_id = "0";
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    product_id = Request.Form["OldProductId"];
                }
                _productMgr = new ProductMgr(connectionString);
                int productId = _productMgr.TempMove2Pro(writerId, COMBO_TYPE, product_id);
                if (productId != -1)
                {
                    json = "{success:true,productId:" + productId + ",msg:'" + Resources.Product.SAVE_SUCCESS + "'}";
                }
                else
                {
                    json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
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

        #region /************************    規格   *************************/
        public ActionResult SpecIndex()
        {
            return View();
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

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase specTempSave()
        {
            string resultStr = "{success:true}";
            bool result = true;
            try
            {

                Caller _caller = (Session["caller"] as Caller);
                string specType = Request.Params["specType"];
                string spec1Name = Request.Params["spec1Name"];
                string spec1Result = Request.Params["spec1Result"];
                string spec2Name = "";
                string spec2Result;

                _specMgr = new ProductSpecMgr(connectionString);
                _specTempMgr = new ProductSpecTempMgr(connectionString);
                _productTempMgr = new ProductTempMgr(connectionString);
                _productItemMgr = new ProductItemMgr(connectionString);
                _productItemTempMgr = new ProductItemTempMgr(connectionString);
                _serialMgr = new SerialMgr(connectionString);

                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    uint proId = uint.Parse(Request.Params["ProductId"]);

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid };
                    batch.batchno = Request.Params["batch"] ?? "";
                    batch.kuser = (Session["caller"] as Caller).user_email;

                    #region 正式表修改
                    List<ProductSpec> spec1List = null;
                    List<ProductSpec> spec2List = null;
                    List<ProductSpec> specUpdateList = new List<ProductSpec>();
                    List<ProductSpec> specAddList = new List<ProductSpec>();
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    spec1List = jss.Deserialize<List<ProductSpec>>(spec1Result);
                    if (spec1List != null)
                    {
                        //規格一處理
                        spec1List.ForEach(m =>
                        {
                            m.product_id = proId;
                            m.spec_type = 1;
                            if (m.spec_id != 0)
                            {
                                specUpdateList.Add(m);
                            }
                            else
                            {
                                specAddList.Add(m);
                            }
                        });
                    }

                    //規格二處理
                    if (specType.Equals("2"))
                    {
                        spec2Name = Request.Params["spec2Name"];
                        spec2Result = Request.Params["spec2Result"];
                        spec2List = jss.Deserialize<List<ProductSpec>>(spec2Result);

                        spec2List.ForEach(m =>
                        {
                            m.product_id = proId;
                            m.spec_type = 2;
                            if (m.spec_id != 0)
                            {
                                specUpdateList.Add(m);
                            }
                            else
                            {
                                specAddList.Add(m);
                            }
                        });
                    }

                    Product proModel = new Product();
                    _productMgr = new ProductMgr(connectionString);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    proModel = _productMgr.Query(new Product { Product_Id = proId }).FirstOrDefault();
                    proModel.Spec_Title_1 = spec1Name;
                    proModel.Spec_Title_2 = spec2Name;

                    if (specUpdateList.Count() > 0)
                    {
                        for (int i = 0, j = specUpdateList.Count(); i < j; i++)
                        {
                            ArrayList sqls = new ArrayList();
                            sqls.Add(_specMgr.Update(specUpdateList[i]));
                            if (i == 0)
                            {
                                sqls.Add(_productMgr.Update(proModel));
                            }
                            if (!_tableHistoryMgr.SaveHistory<ProductSpec>(specUpdateList[i], batch, sqls))
                            {
                                result = false;
                            }
                        }
                    }

                    if (specAddList.Count() > 0)
                    {

                        List<ProductItem> saveItemList = new List<ProductItem>();
                        List<ProductSpec> spec1ExistList = spec1List.Where(m => m.spec_id != 0).ToList();     //規格一中原本就存在的規格
                        specAddList.ForEach(p => p.spec_id = uint.Parse(_serialMgr.NextSerial(18).ToString()));
                        ProductItem saveTemp;
                        List<PriceMaster> list_price = new List<PriceMaster>();
                        PriceMasterMgr pm = new PriceMasterMgr(connectionString);
                        list_price = pm.GetPriceMasterInfoByID2(proId.ToString());//獲取同一個ID下其他產品的價格信息
                        PriceMaster _p = list_price.FirstOrDefault();//add by wangwei0216w 2014/9/19 獲取同一個ID下其他產品的信息
                        //List<ProductItem> pro_list = new List<ProductItem>();
                        //ProductItemMgr pig = new ProductItemMgr(connectionString);
                        //pro_list = pig.GetProductItemByID(Convert.ToInt32(proId));
                        //ProductItem _product = pro_list.FirstOrDefault();//獲取同一個ID下其他產品的信息
                        foreach (var m in specAddList)
                        {
                            if (specType.Equals("1"))
                            {
                                if (m.spec_type == 1)
                                {
                                    saveTemp = new ProductItem();
                                    if (_p.same_price == 1)             //add by wangwei 0216w 2014/9/19 如果同價 就將價格賦予新增規格
                                    {
                                        //saveTemp.Item_Cost = Convert.ToUInt32(_p.cost);
                                        //saveTemp.Item_Money = Convert.ToUInt32(_p.price);
                                    }
                                    else
                                    {
                                        resultStr = "{success:true,Msg:true}";
                                    }
                                    saveTemp.Spec_Id_1 = m.spec_id;
                                    saveTemp.Product_Id = proId;
                                    // saveTemp.Item_Stock = 10;
                                    saveTemp.Item_Alarm = 1;
                                    saveItemList.Add(saveTemp);
                                }
                                else
                                {
                                    saveTemp = new ProductItem();
                                    if (_p.same_price == 1)             //add by wangwei 0216w 2014/9/19 如果同價 就將價格賦予新增規格
                                    {
                                        //saveTemp.Item_Cost = Convert.ToUInt32(_p.cost);
                                        //saveTemp.Item_Money = Convert.ToUInt32(_p.price);
                                    }
                                    else
                                    {
                                        resultStr = "{success:true,Msg:true}";
                                    }
                                    saveTemp.Spec_Id_2 = m.spec_id;
                                    saveTemp.Product_Id = proId;
                                    //saveTemp.Item_Stock = 10;
                                    saveTemp.Item_Alarm = 1;
                                    saveItemList.Add(saveTemp);
                                }
                            }
                            else
                            {
                                if (m.spec_type == 1)
                                {
                                    foreach (ProductSpec item in spec2List)
                                    {
                                        saveTemp = new ProductItem();
                                        if (_p.same_price == 1)             //add by wangwei 0216w 2014/9/19 如果同價 就將價格賦予新增規格
                                        {
                                            //saveTemp.Item_Cost = Convert.ToUInt32(_p.cost);
                                            //saveTemp.Item_Money = Convert.ToUInt32(_p.price);
                                        }
                                        else
                                        {
                                            resultStr = "{success:true,Msg:true}";
                                        }
                                        saveTemp.Spec_Id_1 = m.spec_id;
                                        saveTemp.Spec_Id_2 = item.spec_id;
                                        //saveTemp.Item_Stock = 10;
                                        saveTemp.Item_Alarm = 1;
                                        saveTemp.Product_Id = proId;
                                        saveItemList.Add(saveTemp);
                                    }
                                }
                                else
                                {
                                    foreach (ProductSpec item2 in spec1ExistList)
                                    {
                                        saveTemp = new ProductItem();
                                        if (_p.same_price == 1)             //add by wangwei 0216w 2014/9/19 如果同價 就將價格賦予新增規格
                                        {
                                            //saveTemp.Item_Cost = Convert.ToUInt32(_p.cost);
                                            //saveTemp.Item_Money = Convert.ToUInt32(_p.price);
                                        }
                                        else
                                        {
                                            resultStr = "{success:true,Msg:true}";
                                        }
                                        saveTemp.Spec_Id_1 = item2.spec_id;
                                        saveTemp.Spec_Id_2 = m.spec_id;
                                        //saveTemp.Item_Stock = 10;
                                        saveTemp.Item_Alarm = 1;
                                        saveTemp.Product_Id = proId;
                                        saveItemList.Add(saveTemp);
                                    }

                                }
                            }
                        }

                        _specMgr.Save(specAddList);
                        _productItemMgr.Save(saveItemList);
                        ProductItem pro;
                        if (proModel.Product_Status != 0) //add by wangwei0216w 2014/9/18 作用:將Export_flag的值設置為1
                        {
                            pro = new ProductItem() { Product_Id = proModel.Product_Id, Export_flag = 1 };
                            _productItemMgr.UpdateExportFlag(pro);
                        }
                        if (result) //如果之前保存規格,插入臨時表登操作有誤,則不執行插入itemprice的操作
                        {
                            List<ProductItem> item_list = _productItemMgr.GetProductNewItem_ID(Convert.ToInt32(proModel.Product_Id));   //得到product_item的新增ID
                            ItemPrice i = new ItemPrice();
                            ItemPriceMgr ipm = new ItemPriceMgr(connectionString);
                            ArrayList liststr = new ArrayList();
                            foreach (PriceMaster p in list_price)
                            {
                                foreach (ProductItem pi in item_list)
                                {
                                    if (p.same_price == 1)
                                    {
                                        i.price_master_id = p.price_master_id;
                                        i.item_id = pi.Item_Id;
                                        i.item_money = Convert.ToUInt32(p.price);
                                        i.item_cost = Convert.ToUInt32(p.cost);
                                        i.event_cost = Convert.ToUInt32(p.event_cost);
                                        i.event_money = Convert.ToUInt32(p.event_price);
                                    }
                                    else
                                    {
                                        i.price_master_id = p.price_master_id;
                                        i.item_id = pi.Item_Id;
                                        i.item_money = 0;
                                        i.item_cost = 0;
                                        i.event_money = 0;
                                        i.event_cost = 0;
                                        resultStr = "{success:true,Msg:true}";
                                    }
                                    liststr.Add(ipm.Save(i));
                                }
                            }
                            ipm.AddItemPricBySpec(liststr, connectionString);
                        }
                    }
                    //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                    //if (proModel.Combination == 1)
                    //{
                    //    _productItemMgr = new ProductItemMgr(connectionString);
                    //    ProductItem pro_Item = new ProductItem() { Product_Id = proModel.Product_Id, Export_flag = 2 };
                    //    _productItemMgr.UpdateExportFlag(pro_Item);
                    //}

                    #endregion
                }
                else
                {
                    #region 臨時表修改
                    _productTempMgr = new ProductTempMgr(connectionString); //add by xiangwang 2014.09.26 可修改庫存預設值為99

                    string product_id = "0";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        product_id = Request.Form["OldProductId"];
                    }

                    //add by xiangwang 2014.09.26 可修改庫存預設值為99
                    ProductTemp query = _productTempMgr.GetProTemp(new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE, Product_Id = product_id });


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
                            //   specid = _serialMgr.NextSerial(18);
                            //item.spec_id = uint.Parse(specid.ToString());
                            item.Writer_Id = _caller.user_id;
                            item.product_id = product_id;
                            item.spec_type = 1;
                            item.spec_image = "";
                            specAllList.Add(item);
                        }

                        if (specType.Equals("2"))
                        {
                            spec2Name = Request.Params["spec2Name"];
                            spec2Result = Request.Params["spec2Result"];
                            spec2List = jss.Deserialize<List<ProductSpecTemp>>(spec2Result);

                            foreach (ProductSpecTemp item in spec2List)
                            {
                                // specid = _serialMgr.NextSerial(18);
                                //item.spec_id = uint.Parse(specid.ToString());
                                item.Writer_Id = _caller.user_id;
                                item.product_id = product_id;
                                item.spec_type = 2;
                                item.spec_image = "";
                                specAllList.Add(item);
                            }
                        }

                        List<ProductSpecTemp> tempList = _specTempMgr.Query(new ProductSpecTemp { Writer_Id = _caller.user_id, product_id = product_id });
                        if (tempList == null || tempList.Count() <= 0)
                        {
                            #region 保存

                            specAllList.ForEach(p => p.spec_id = uint.Parse(_serialMgr.NextSerial(18).ToString()));

                            bool saveSpecResult = _specTempMgr.Save(specAllList);

                            if (saveSpecResult)
                            {
                                _productItemTempMgr.Delete(new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = product_id });
                                #region 保存ProductItemTemp

                                List<ProductSpecTemp> specAllResultList = _specTempMgr.Query(new ProductSpecTemp { Writer_Id = _caller.user_id, product_id = product_id });
                                List<ProductSpecTemp> spec1ResultList = specAllResultList.Where(m => m.spec_type == 1).ToList();
                                List<ProductSpecTemp> spec2ResultList = specAllResultList.Where(m => m.spec_type == 2).ToList();

                                List<ProductItemTemp> saveItemList = new List<ProductItemTemp>();

                                if (specType.Equals("1"))
                                {
                                    foreach (ProductSpecTemp specTemp1 in spec1ResultList)
                                    {
                                        ProductItemTemp itemTemp = new ProductItemTemp();
                                        itemTemp.Writer_Id = _caller.user_id;
                                        itemTemp.Product_Id = product_id;
                                        itemTemp.Spec_Id_1 = specTemp1.spec_id;
                                        //itemTemp.Item_Stock = 10;
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
                                            itemTemp.Writer_Id = _caller.user_id;
                                            itemTemp.Product_Id = product_id;
                                            itemTemp.Spec_Id_1 = specTemp1.spec_id;
                                            itemTemp.Spec_Id_2 = specTemp2.spec_id;
                                            //itemTemp.Item_Stock = 10;
                                            itemTemp.Item_Alarm = 1;
                                            itemTemp.Item_Code = "";
                                            itemTemp.Barcode = "";
                                            saveItemList.Add(itemTemp);
                                        }
                                    }
                                }

                                //add by xiangwang 2014.09.26 可修改庫存預設值為99
                                //saveItemList.ForEach(m => m.SetDefaultItemStock(query));

                                bool saveItemResult = _productItemTempMgr.Save(saveItemList);

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
                            string[] specs = strSpecInit.Split(',');

                            List<ProductSpecTemp> addList = specAllList.Where(p => p.spec_id == 0).ToList();
                            if (addList.Count() > 0)
                            {
                                addList.ForEach(p => p.spec_id = uint.Parse(_serialMgr.NextSerial(18).ToString()));

                                List<ProductSpecTemp> specAllResultList = _specTempMgr.Query(new ProductSpecTemp { Writer_Id = _caller.user_id, product_id = product_id });
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
                                            saveTemp.Writer_Id = _caller.user_id;
                                            saveTemp.Spec_Id_1 = item.spec_id;
                                            saveTemp.Product_Id = product_id;
                                            //saveTemp.Item_Stock = 10;
                                            saveTemp.Item_Alarm = 1;
                                            saveItemList.Add(saveTemp);
                                        }
                                        else
                                        {
                                            ProductItemTemp saveTemp = new ProductItemTemp();
                                            saveTemp.Writer_Id = _caller.user_id;
                                            saveTemp.Spec_Id_2 = item.spec_id;
                                            saveTemp.Product_Id = product_id;
                                            // saveTemp.Item_Stock = 10;
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
                                                saveTemp.Writer_Id = _caller.user_id;
                                                saveTemp.Spec_Id_1 = item.spec_id;
                                                saveTemp.Spec_Id_2 = item1.spec_id;
                                                //saveTemp.Item_Stock = 10;
                                                saveTemp.Item_Alarm = 1;
                                                saveTemp.Product_Id = product_id;
                                                saveItemList.Add(saveTemp);
                                            }

                                            foreach (ProductSpecTemp item1 in addList.Where(p => p.spec_type == 2).ToList())
                                            {
                                                ProductItemTemp saveTemp = new ProductItemTemp();
                                                saveTemp.Writer_Id = _caller.user_id;
                                                saveTemp.Spec_Id_1 = item.spec_id;
                                                saveTemp.Spec_Id_2 = item1.spec_id;
                                                //saveTemp.Item_Stock = 10;
                                                saveTemp.Item_Alarm = 1;
                                                saveTemp.Product_Id = product_id;
                                                saveItemList.Add(saveTemp);
                                            }
                                        }
                                        else
                                        {
                                            foreach (ProductSpecTemp item2 in spec1ResultList)
                                            {
                                                ProductItemTemp saveTemp = new ProductItemTemp();
                                                saveTemp.Writer_Id = _caller.user_id;
                                                saveTemp.Spec_Id_1 = item2.spec_id;
                                                saveTemp.Spec_Id_2 = item.spec_id;
                                                saveTemp.Product_Id = product_id;
                                                // saveTemp.Item_Stock = 10;
                                                saveTemp.Item_Alarm = 1;
                                                saveItemList.Add(saveTemp);
                                            }

                                        }
                                    }
                                }
                                _specTempMgr.Save(addList);

                                //add by xiangwang 2014.09.26 可修改庫存預設值為99
                                //saveItemList.ForEach(m => m.SetDefaultItemStock(query));

                                _productItemTempMgr.Save(saveItemList);

                            }

                            if (specs.Length > 0)
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

                                        ProductItemTemp delTemp = new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = product_id };
                                        uint spectype = _specTempMgr.Query(new ProductSpecTemp { spec_id = uint.Parse(initSpecId), product_id = product_id })[0].spec_type;
                                        if (spectype == 1)
                                        {
                                            delTemp.Spec_Id_1 = uint.Parse(initSpecId);
                                        }
                                        else if (spectype == 2)
                                        {
                                            delTemp.Spec_Id_2 = uint.Parse(initSpecId);
                                        }
                                        if (!_productItemTempMgr.Delete(delTemp))
                                        {
                                            result = false;
                                        }
                                        if (!_specTempMgr.Delete(new ProductSpecTemp { spec_id = uint.Parse(initSpecId), Writer_Id = _caller.user_id, product_id = product_id }))
                                        {
                                            result = false;
                                        }
                                        DeletePicOnServer(false, true, false, uint.Parse(initSpecId), product_id);
                                    }

                                }
                                if (!_specTempMgr.Update(updateList, "spec"))
                                {
                                    result = false;
                                }
                            }

                            #endregion
                        }

                        #region 更新Product

                        ProductTemp proTemp = new ProductTemp();
                        proTemp.Writer_Id = _caller.user_id;
                        proTemp.Product_Spec = uint.Parse(specType);
                        proTemp.Spec_Title_1 = spec1Name;
                        proTemp.Spec_Title_2 = spec2Name;
                        proTemp.Combo_Type = COMBO_TYPE;
                        proTemp.Product_Id = product_id;
                        bool saveProductResult = _productTempMgr.SpecInfoSave(proTemp);
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
                        List<ProductItemTemp> saveList = new List<ProductItemTemp>();
                        //如果原數據有規格
                        if (query.Product_Spec != 0)
                        {
                            //刪除服務器上對應的圖片
                            DeletePicOnServer(false, true, false, 0, product_id);

                            _productItemTempMgr.Delete(new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = product_id });

                            _specTempMgr.Delete(new ProductSpecTemp { Writer_Id = _caller.user_id, product_id = product_id });

                            _productTempMgr.SpecInfoSave(new ProductTemp { Product_Spec = 0, Spec_Title_1 = "", Spec_Title_2 = "", Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE, Product_Id = product_id });

                            saveList = new List<ProductItemTemp>();
                            saveList.Add(new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = product_id/*, Item_Stock = 10*/, Item_Alarm = 1 });
                        }
                        else
                        {
                            List<ProductItemTemp> itemQuery = _productItemTempMgr.Query(new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = product_id });
                            if (itemQuery.Count() <= 0)
                            {
                                saveList = new List<ProductItemTemp>();
                                saveList.Add(new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = product_id, /*Item_Stock = 10,*/ Item_Alarm = 1 });
                                // _productItemTempMgr.Save(saveList);
                            }
                        }

                        //add by xiangwang 2014.09.26 可修改庫存預設值為99
                        //saveList.ForEach(m => m.SetDefaultItemStock(query));
                        _productItemTempMgr.Save(saveList);

                        #endregion
                    }
                    #endregion

                    #region 調度或自出商品,商品庫存預設為99
                    var proditemTemp = new ProductItemTemp { Product_Id = product_id, Writer_Id = _caller.user_id };
                    proditemTemp.SetDefaultItemStock(query);
                    _productItemTempMgr.UpdateItemStock(proditemTemp);
                    #endregion
                }
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
                resultStr = "{success:false,Msg:false}";
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
            Caller _caller = (Session["caller"] as Caller);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    //product_spec
                    uint pid = uint.Parse(Request.Form["ProductId"]);
                    _specMgr = new ProductSpecMgr(connectionString);
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specMgr.Query(new ProductSpec { product_id = pid, spec_type = 1 })) + "}";
                }
                else
                {
                    //product_spec_temp
                    _specTempMgr = new ProductSpecTempMgr(connectionString);
                    ProductSpecTemp query = new ProductSpecTemp { Writer_Id = _caller.user_id, spec_type = 1 };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.product_id = Request.Form["OldProductId"];
                    }
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specTempMgr.Query(query)) + "}";
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
            Caller _caller = (Session["caller"] as Caller);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    //product_spec
                    uint pid = uint.Parse(Request.Form["ProductId"]);
                    _specMgr = new ProductSpecMgr(connectionString);
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specMgr.Query(new ProductSpec { product_id = pid, spec_type = 2 })) + "}";

                }
                else
                {
                    //product_spec_temp
                    _specTempMgr = new ProductSpecTempMgr(connectionString);
                    ProductSpecTemp query = new ProductSpecTemp { Writer_Id = _caller.user_id, spec_type = 2 };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.product_id = Request.Form["OldProductId"];
                    }
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_specTempMgr.Query(query)) + "}";

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
            Caller _caller = (Session["caller"] as Caller);
            try
            {

                ProductSpecTemp proSpecTemp = new ProductSpecTemp { Writer_Id = _caller.user_id };

                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    proSpecTemp.product_id = Request.Form["OldProductId"];
                }
                DeletePicOnServer(false, true, false, 0, proSpecTemp.product_id);
                _specTempMgr.Delete(proSpecTemp);
                _productItemTempMgr.Delete(new ProductItemTemp { Writer_Id = _caller.user_id, Product_Id = proSpecTemp.product_id });

                //刪除CourseDetailItemTemp hxw 2015/03/11
                ICourseDetailItemTempImplMgr _courDetaItemTempMgr = new CourseDetailItemTempMgr(connectionString);
                _courDetaItemTempMgr.Delete(_caller.user_id);

                //更新Product規格為無規格
                ProductTemp proTemp = new ProductTemp { Product_Spec = 0, Spec_Title_1 = "", Spec_Title_2 = "", Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                proTemp.Product_Id = proSpecTemp.product_id;
                bool saveProductResult = _productTempMgr.SpecInfoSave(proTemp);


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

        #region 課程設定
        public ActionResult CourseDetailItemQuery(int productId = 0, bool detail = false)//edit by wwei0216w productId 一個初始值,防止報錯
        {
            if (detail)//詳情頁查詢
            {
                ICourseDetailItemImplMgr _courseDetailItemMgr = new CourseDetailItemMgr(connectionString);
                return Json(_courseDetailItemMgr.QueryName(productId));
            }


            if (productId == 0)//查詢臨時表
            {
                ICourseDetailItemTempImplMgr _courseDetailItemTempMgr = new CourseDetailItemTempMgr(connectionString);
                return Json(_courseDetailItemTempMgr.Query((Session["caller"] as Caller).user_id, productId));
            }
            else//查詢正式表
            {
                ICourseDetailItemImplMgr _courseDetailItemMgr = new CourseDetailItemMgr(connectionString);
                return Json(_courseDetailItemMgr.Query(productId));
            }
        }


        public ActionResult CourseDetailItemSave(string itemStr, string delItemStr, uint productId)
        {
            bool result = true;
            var courseDetailItems = JsonConvert.DeserializeObject<List<CourseDetailItemTemp>>(itemStr);
            var delList = JsonConvert.DeserializeObject<List<CourseDetailItemTemp>>(delItemStr);
            if (productId == 0)//操作臨時表
            {
                courseDetailItems.ForEach(c => c.Writer_Id = (Session["caller"] as Caller).user_id);
                ICourseDetailItemTempImplMgr _courseDetailItemTempMgr = new CourseDetailItemTempMgr(connectionString);
                result = _courseDetailItemTempMgr.Save(courseDetailItems, delList);
            }
            else//正式表
            {
                ICourseDetailItemImplMgr _courseDetailItemMgr = new CourseDetailItemMgr(connectionString);
                var list = courseDetailItems.Select(c => (CourseDetailItem)c).ToList();
                var deleteList = delList.Select(c => (CourseDetailItem)c).ToList();
                result = _courseDetailItemMgr.Save(list, deleteList);
            }
            return Json(new { success = result, Msg = "" });
        }

        #endregion

        #region /************************  新類別   *************************/
        public ActionResult NewCategoryIndex()
        {
            return View();
        }
        #endregion


        #region /************************    類別   *************************/
        public ActionResult CategoryIndex()
        {
            return View();
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetSelectedCage()
        {//獲取報表列表內容
            string resultStr = "{success:false}";
            try
            {
                string strCateId = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    #region 從正式表獲取
                    uint pid = uint.Parse(Request.Params["ProductId"]);
                    _productMgr = new ProductMgr(connectionString);
                    Product result = _productMgr.Query(new Product { Product_Id = pid }).FirstOrDefault();
                    if (result != null)
                    {
                        strCateId = result.Cate_Id;
                    }
                    //resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_productMgr.Query(new Product { Product_Id = pid }).FirstOrDefault()) + "}";
                    #endregion
                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    ProductTemp query = new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp tempResult = _productTempMgr.GetProTemp(query);
                    if (tempResult != null)
                    {
                        strCateId = tempResult.Cate_Id;
                    }
                    // resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_productTempMgr.GetProTemp(query)) + "}";
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
        #region 获取选中的前台分类 (無應用)
        /// <summary>
        /// 获取选中的前台分类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetFrontCate()
        {
            string resultStr = "{success:false}";

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    #region 從正式表獲取
                    uint pid = uint.Parse(Request.Params["ProductId"]);
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_categorySetMgr.Query(new ProductCategorySetCustom { Product_Id = pid })) + "}";
                    #endregion
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

        [HttpPost]
        [CustomHandleError]
        public ActionResult GetProdClassify(uint productId = 0, uint oldProductId = 0, int coboType = 1)
        {
            Product product;
            if (productId == 0)//
            {
                string tempPoroductId = oldProductId == 0 ? "0" : oldProductId.ToString();
                _productTempMgr = new ProductTempMgr(connectionString);
                product = _productTempMgr.GetProTemp(new ProductTemp { Writer_Id = (Session["caller"] as Caller).user_id, Combo_Type = coboType, Product_Id = tempPoroductId });
            }
            else
            {
                _productMgr = new ProductMgr(connectionString);
                product = _productMgr.Query(new Product { Product_Id = productId }).FirstOrDefault();
            }
            return Json(new { product.Prod_Classify });
        }

        #region 保存
        [HttpPost]
        [CustomHandleError]
        public ActionResult tempCategoryAdd(int categoryType, int coboType = 1)
        {
            COMBO_TYPE = coboType;
            string resultStr = "{success:true}";
            string oldresult = Request["oldresult"];//add by wwei0216w 2014/12/26 需要刪除的選中項

            if (string.IsNullOrEmpty(oldresult))
            {
                resultStr = "{success:false}";
                return Content(resultStr);
            }
            try
            {
                Caller _caller = (Session["caller"] as Caller);
                string tempStr = "";
                string cate_id = "";
                if (!string.IsNullOrEmpty(Request.Params["result"]))
                {
                    tempStr = Request.Params["result"];
                }

                if (!string.IsNullOrEmpty(Request.Params["cate_id"]))
                {
                    cate_id = Request.Params["cate_id"];
                }

                List<ProductCategorySetTemp> saveTempList = new List<ProductCategorySetTemp>();

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductCategorySetCustom> cateCustomList = js.Deserialize<List<ProductCategorySetCustom>>(tempStr);
                string deStr = "";
                //if (categoryType == 2)//新類別
                //{


                //    if (cateCustomList.Count() <= 0)
                //    {
                //        resultStr = "{success:false,msg:'新類別必選'}";
                //        return Content(resultStr);
                //    }
                //}

                #region 將要刪除的id 拼成 '1,2,3'這種形式
                List<ProductCategorySetCustom> deleteCategorySet = js.Deserialize<List<ProductCategorySetCustom>>(oldresult);

                if (deleteCategorySet.Count() > 0)
                {
                    foreach (var item in deleteCategorySet)
                    {
                        deStr += item.Category_Id + ",";
                    }
                    deStr = deStr.Remove(deStr.Length - 1);
                }
                #endregion



                if (string.IsNullOrEmpty(tempStr))
                {
                    cateCustomList = new List<ProductCategorySetCustom>();
                }


                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    #region 修改正式表
                    uint pid = uint.Parse(Request.Params["ProductId"]);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);
                    _productMgr = new ProductMgr(connectionString);


                    Product pro = new Product();
                    pro = _productMgr.Query(new Product { Product_Id = pid }).FirstOrDefault();
                    if (pro != null)
                    {
                        _functionMgr = new FunctionMgr(connectionString);
                        string function = Request.Params["function"] ?? "";
                        Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                        int functionid = fun == null ? 0 : fun.RowId;
                        HistoryBatch batch = new HistoryBatch { functionid = functionid };
                        batch.batchno = Request.Params["batch"] ?? "";
                        batch.kuser = (Session["caller"] as Caller).user_email;



                        ArrayList sqls = new ArrayList();
                        if (deStr != "")
                        {
                            sqls.Add(_categorySetMgr.Delete(new ProductCategorySet { Product_Id = pid }, deStr));
                        }
                        if (categoryType == 1)
                        {
                            pro.Cate_Id = cate_id;
                        }
                        else if (categoryType == 2)  //edit by wwei0216w 如果是新類別 就不進行報表修改
                        {
                            pro.Prod_Classify = Convert.ToInt32(Request["prodClassify"]); //獲得對應館別ID
                            _tableHistoryMgr.SaveHistory<Product>(pro, batch, null);
                        }

                        sqls.Add(_productMgr.Update(pro, _caller.user_id));
                        foreach (ProductCategorySetCustom item in cateCustomList)
                        {
                            item.Product_Id = pid;
                            item.Brand_Id = pro.Brand_Id;
                            sqls.Add(_categorySetMgr.Save(item));
                        }

                        if (!_tableHistoryMgr.SaveHistory<ProductCategorySetCustom>(cateCustomList, batch, sqls))
                        {

                            throw new Exception("there is no History be saved");
                        }
                        //else
                        //{ //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                        //    if (pro.Combination == 1)
                        //    {
                        //        _productItemMgr = new ProductItemMgr(connectionString);
                        //        ProductItem pro_Item = new ProductItem() { Product_Id = pro.Product_Id, Export_flag = 2 };
                        //        _productItemMgr.UpdateExportFlag(pro_Item);
                        //    }
                        //}
                    }
                    else
                    {
                        throw new Exception("None Product has being found");
                    }
                    #endregion
                }
                else
                {
                    #region 修改臨時表
                    string product_id = "0";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        product_id = Request.Form["OldProductId"];
                    }
                    _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
                    _productTempMgr = new ProductTempMgr(connectionString);

                    bool result = _categoryTempSetMgr.Delete(new ProductCategorySetTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE, Product_Id = product_id }, deStr);

                    ProductTemp query = new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE, Product_Id = product_id };
                    ProductTemp proTemp = _productTempMgr.GetProTemp(query);
                    ProductCategorySetTemp saveTemp;

                    if (proTemp == null)
                    {
                        resultStr = "{success:false}";
                    }
                    else
                    {
                        foreach (ProductCategorySetCustom item in cateCustomList)
                        {
                            saveTemp = new ProductCategorySetTemp();
                            saveTemp.Writer_Id = _caller.user_id;
                            saveTemp.Product_Id = product_id;
                            saveTemp.Category_Id = item.Category_Id;
                            saveTemp.Brand_Id = proTemp.Brand_Id;
                            saveTemp.Combo_Type = COMBO_TYPE;
                            saveTempList.Add(saveTemp);
                        }

                        if (!_categoryTempSetMgr.Save(saveTempList))
                        {
                            resultStr = "{success:false}";
                        }
                    }

                    proTemp.Combo_Type = COMBO_TYPE;
                    proTemp.Product_Id = product_id;
                    if (categoryType == 1)
                    {
                        proTemp.Cate_Id = cate_id;
                    }
                    else if (categoryType == 2)  //edit by wwei0216w 如果是新類別 就不進行報表修改
                    {
                        proTemp.Prod_Classify = Convert.ToInt32(Request["prodClassify"]); //獲得對應館別ID
                    }

                    if (!_productTempMgr.CategoryInfoUpdate(proTemp))
                    {
                        resultStr = "{success:false}";
                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{success:false}";
                return Content(resultStr);
            }

            return Content(resultStr);
            ////this.Response.Clear();
            ////this.Response.Write(resultStr);
            ////this.Response.End();
            ////return this.Response;
        }

        #endregion

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

        #region 得到所有前台分類
        [CustomHandleError]
        //[OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetCatagory(string id = "true", int coboType = 1)
        {
            COMBO_TYPE = coboType;
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            int categoryType = 2; //用來區別查詢所有節點還是查詢新館的節點 //edit by wwei0216w 2014/12/25
            //從後臺獲取查詢所有節點還是查詢新館的節點
            categoryType = Request["categoryType"] == null ? 2 : Convert.ToInt32(Request["categoryType"]);
            string resultStr = "";
            try
            {
                _procateMgr = new ProductCategoryMgr(connectionString);
                categoryList = _procateMgr.QueryAll(new ProductCategory { });
                //查找出新館的信息 新館的id為754 
                cateList = getCate(categoryList, 0);
                if (categoryType == 2) //如果等於新類別
                {
                    cateList = cateList.FindAll(m => m.id == "2");//找到ROOT節點,新類別只需要新館與其父,子節點 add by wwei0216 2015/1/15
                }
                List<ProductCategorySet> resultList = new List<ProductCategorySet>();
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);
                    resultList = _categorySetMgr.Query(new ProductCategorySet { Product_Id = uint.Parse(Request.Params["ProductId"]) });
                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
                    ProductCategorySetTemp query = new ProductCategorySetTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))//edit by xiaohui 2014/09/24
                    {
                        query.Product_Id = Request.Params["OldProductId"];
                    }
                    resultList = (from c in _categoryTempSetMgr.Query(query)
                                  select new
                                  {
                                      Id = c.Id,
                                      Product_Id = c.Product_Id,
                                      Category_Id = c.Category_Id,
                                      Brand_Id = c.Brand_Id
                                  }).ToList().ConvertAll<ProductCategorySet>(m => new ProductCategorySet
                                  {
                                      Id = m.Id,
                                      Product_Id = uint.Parse(m.Product_Id),
                                      Category_Id = m.Category_Id,
                                      Brand_Id = m.Brand_Id
                                  });
                }
                GetCategoryList(categoryList, ref cateList, resultList);
                switch (categoryType)
                {
                    case 0:
                        resultStr = JsonConvert.SerializeObject(cateList);
                        break;
                    case 1: //等於754      
                        cateList.FindAll(m => m.id == "2")[0].children.RemoveAll(m => m.id == "754");//移除新館節點
                        resultStr = JsonConvert.SerializeObject(cateList);
                        break;
                    case 2:
                        cateList.FindAll(m => m.id == "2")[0].children.RemoveAll(m => m.id != "754");//移除不是新館節點
                        resultStr = JsonConvert.SerializeObject(cateList);
                        break;
                }
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
        //public HttpResponseBase GetCatagory(string id = "true")
        //{   //Edit 20140912 jialei by 用於商品管理供應商申請審核
        //    List<ProductCategory> categoryList = new List<ProductCategory>();
        //    List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
        //    string resultStr = "";
        //    try
        //    {
        //        _procateMgr = new ProductCategoryMgr(connectionString);
        //        categoryList = _procateMgr.QueryAll(new ProductCategory { });
        //        cateList = getCate(categoryList, 0);
        //        List<ProductCategorySetTemp> TempresultList = new List<ProductCategorySetTemp>();
        //        List<ProductCategorySet> resultList = new List<ProductCategorySet>();
        //        ProductCategorySetTemp query = new ProductCategorySetTemp();
        //        Caller _caller = (Session["caller"] as Caller);
        //        if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
        //        {
        //            uint product_id = 0;
        //            if (uint.TryParse(Request.Form["ProductId"], out product_id))
        //            {
        //                _categorySetMgr = new ProductCategorySetMgr(connectionString);
        //                resultList = _categorySetMgr.Query(new ProductCategorySet { Product_Id = uint.Parse(Request.Params["ProductId"]) });
        //            }
        //            else
        //            {
        //                query.Product_Id = Request.Params["ProductId"];
        //                query.Writer_Id = _caller.user_id;
        //                query.Combo_Type = COMBO_TYPE;
        //                _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
        //                TempresultList = (from c in _categoryTempSetMgr.QueryByVendor(query)
        //                                  select new
        //                                  {
        //                                      Id = c.Id,
        //                                      Product_Id = c.Product_Id,
        //                                      Category_Id = c.Category_Id,
        //                                      Brand_Id = c.Brand_Id
        //                                  }).ToList().ConvertAll<ProductCategorySetTemp>(m => new ProductCategorySetTemp
        //                                  {
        //                                      Id = m.Id,
        //                                      Product_Id = m.Product_Id,
        //                                      Category_Id = m.Category_Id,
        //                                      Brand_Id = m.Brand_Id
        //                                  });
        //                VendorGetCategoryList(categoryList, ref cateList, TempresultList, resultList);
        //                List<ProductCategoryCustom> cateListResult = new List<ProductCategoryCustom>();
        //                cateListResult = getCate(categoryList, 0);
        //                cateListResult[0].children = getCate(categoryList, cateListResult[0].id);
        //                int cateLen = cateListResult[0].children.Count;
        //                int i = 0;
        //                while (cateLen > 0)
        //                {
        //                    if (cateListResult[0].children[i].id == 5)
        //                    {
        //                        i++;
        //                    }
        //                    else
        //                    {
        //                        cateListResult[0].children.Remove(cateListResult[0].children[i]);
        //                    }
        //                    cateLen--;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
        //            query = new ProductCategorySetTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
        //            if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
        //            {
        //                query.Product_Id = Request.Form["OldProductId"];
        //            }
        //            resultList = (from c in _categoryTempSetMgr.Query(query)
        //                          select new
        //                          {
        //                              Id = c.Id,
        //                              Product_Id = c.Product_Id,
        //                              Category_Id = c.Category_Id,
        //                              Brand_Id = c.Brand_Id
        //                          }).ToList().ConvertAll<ProductCategorySet>(m => new ProductCategorySet
        //                          {
        //                              Id = m.Id,
        //                              Product_Id = uint.Parse(m.Product_Id),
        //                              Category_Id = m.Category_Id,
        //                              Brand_Id = m.Brand_Id
        //                          });
        //        }
        //        GetCategoryList(categoryList, ref cateList, resultList);
        //        resultStr = JsonConvert.SerializeObject(cateList);
        //        resultStr = resultStr.Replace("Checked", "checked");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(resultStr);
        //    this.Response.End();
        //    return this.Response;
        //}
        #endregion

        #region 遞歸得到分類節點
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, List<ProductCategorySet> resultList)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, uint.Parse(item.id.ToString()));
                item.children = childList;

                ProductCategorySet resultTemp = resultList.Where(m => m.Category_Id.ToString() == item.id).FirstOrDefault();
                if (resultTemp != null)
                {
                    item.Checked = true;
                }

                if (childList.Count() > 0)
                {
                    GetCategoryList(categoryList, ref childList, resultList);
                }
            }

        }
        public void VendorGetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, List<ProductCategorySetTemp> TempresultList, List<ProductCategorySet> resultList)
        {   //Add 20140912 jialei by 用於商品管理供應商申請審核
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, uint.Parse(item.id.ToString()));
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
                    VendorGetCategoryList(categoryList, ref childList, TempresultList, resultList);
                }
            }
        }

        #endregion

        #endregion


        #region /************************    抽獎   *************************/

        public ActionResult FortuneIndex()
        {
            return View();
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase fortuneSave()
        {
            string resultStr = "{success:true}";
            Caller _caller = (Session["caller"] as Caller);

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    #region 保存正式表

                    _productMgr = new ProductMgr(connectionString);
                    Product pro = _productMgr.Query(new Product { Product_Id = uint.Parse(Request.Form["ProductId"]) }).FirstOrDefault();
                    pro.Fortune_Quota = uint.Parse(Request.Params["Fortune_Quota"]);
                    pro.Fortune_Freight = uint.Parse(Request.Params["Fortune_Freight"]);


                    if (!_productMgr.ExecUpdate(pro))
                    {
                        resultStr = "{success:false}";
                    }
                    //else
                    //{//若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                    //    if (pro.Combination == 1)
                    //    {
                    //        _productItemMgr = new ProductItemMgr(connectionString);
                    //        ProductItem pro_Item = new ProductItem() { Product_Id = pro.Product_Id, Export_flag = 2 };
                    //        _productItemMgr.UpdateExportFlag(pro_Item);
                    //    }
                    //}
                    #endregion

                }
                else
                {
                    #region 保存臨時表
                    ProductTemp pTemp = new ProductTemp();
                    pTemp.Fortune_Quota = uint.Parse(Request.Params["Fortune_Quota"]);
                    pTemp.Fortune_Freight = uint.Parse(Request.Params["Fortune_Freight"]);
                    pTemp.Writer_Id = _caller.user_id;
                    pTemp.Combo_Type = COMBO_TYPE;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }
                    _productTempMgr = new ProductTempMgr(connectionString);
                    bool saveFortuneTempResult = _productTempMgr.FortuneInfoSave(pTemp);
                    if (!saveFortuneTempResult)
                    {
                        resultStr = "{success:false}";
                    }
                    #endregion
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

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase fortuneQuery()
        {
            string resultStr = "{success:false}";
            Caller _caller = (Session["caller"] as Caller);
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    //product
                    _productMgr = new ProductMgr(connectionString);
                    Product pro = null;
                    pro = _productMgr.Query(new Product { Product_Id = uint.Parse(Request.Params["ProductId"]) }).FirstOrDefault();
                    if (pro != null)
                    {
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(pro) + "}";
                    }

                }
                else
                {
                    //product_temp
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp pTemp = null;
                    ProductTemp query = new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    pTemp = _productTempMgr.GetProTemp(query);
                    if (pTemp != null)
                    {
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(pTemp) + "}";
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


        #region 價格
        #region 商品細項

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProItems()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]) && !string.IsNullOrEmpty(Request.Form["SiteId"]))
                {
                    //item_price
                }
                else if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    //product_item
                    _productItemMgr = new ProductItemMgr(connectionString);
                    List<ProductItem> proItem = _productItemMgr.QueryPrice(new ProductItem { Product_Id = Convert.ToUInt32(Request.Form["ProductId"]) });
                    json = JsonConvert.SerializeObject(proItem);
                }
                else
                {
                    //product_item_temp
                    int writerId = (Session["caller"] as Caller).user_id;
                    ProductItemTemp query = new ProductItemTemp { Writer_Id = writerId };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    _productItemTempMgr = new ProductItemTempMgr(connectionString);

                    List<ProductItemTemp> proItemTemp = _productItemTempMgr.Query(query);
                    json = JsonConvert.SerializeObject(proItemTemp);
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

        #region 保存商品細項價格、新增站臺價格

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveItemPrice()
        {
            string json = string.Empty;
            string msg = string.Empty;
            try
            {
                if (!PriceMaster.CheckProdName(Request.Form["product_name"]))
                {
                    json = "{success:false,msg:'" + Resources.Product.FORBIDDEN_CHARACTER + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                string items = Request.Form["Items"];
                float default_bonus_percent = 0;
                float.TryParse(Request.Form["default_bonus_percent"] ?? "1", out default_bonus_percent);
                float bonus_percent = 0;
                float.TryParse(Request.Form["bonus_percent"] ?? "1", out bonus_percent);
                int same_price = (Request.Form["same_price"] ?? "") == "on" ? 1 : 0;
                int accumulated_bonus = (Request.Form["accumulated_bonus"] ?? "") == "on" ? 1 : 0;
                string start = Request.Form["event_product_start"] ?? Request.Form["event_start"];
                string end = Request.Form["event_product_end"] ?? Request.Form["event_end"];
                string bonus_start = Request.Form["bonus_percent_start"];
                string bonus_end = Request.Form["bonus_percent_end"];
                string valid_start = Request.Form["valid_start"];
                string valid_end = Request.Form["valid_end"];
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]) && !string.IsNullOrEmpty(Request.Form["site_name"]))
                {
                    #region price_master,item_price  新增站台价格

                    List<ItemPrice> itemPrices = jsSer.Deserialize<List<ItemPrice>>(items);

                    PriceMaster priceMaster = new PriceMaster { bonus_percent = bonus_percent, default_bonus_percent = default_bonus_percent };
                    if (!string.IsNullOrEmpty(start))
                    {
                        priceMaster.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(start));
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        priceMaster.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(end));
                    }
                    priceMaster.product_name = PriceMaster.Product_Name_FM(Request.Form["product_name"]);
                    priceMaster.site_id = uint.Parse(Request.Form["site_name"]);
                    priceMaster.product_id = uint.Parse(Request.Form["ProductId"]);
                    priceMaster.user_level = uint.Parse(Request.Form["user_level"] ?? "1");
                    priceMaster.same_price = same_price;
                    priceMaster.accumulated_bonus = Convert.ToUInt32(accumulated_bonus);
                    priceMaster.price_status = 2;//申請審核
                    priceMaster.price = Convert.ToInt32(itemPrices.Min(m => m.item_money));
                    priceMaster.event_price = Convert.ToInt32(itemPrices.Min(m => m.event_money));
                    priceMaster.cost = Convert.ToInt32(itemPrices.Min(m => m.item_cost));
                    priceMaster.event_cost = Convert.ToInt32(itemPrices.Min(m => m.event_cost));
                    if (same_price == 0)
                    {
                        priceMaster.max_price = Convert.ToInt32(itemPrices.Max(m => m.item_money));
                        priceMaster.max_event_price = Convert.ToInt32(itemPrices.Max(m => m.event_money));
                    }
                    if (!string.IsNullOrEmpty(bonus_start))
                    {
                        priceMaster.bonus_percent_start = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_start));
                    }
                    if (!string.IsNullOrEmpty(bonus_start))
                    {
                        priceMaster.bonus_percent_end = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_end));
                    }
                    if (!string.IsNullOrEmpty(valid_start))
                    {
                        priceMaster.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                    }
                    if (!string.IsNullOrEmpty(valid_end))
                    {
                        priceMaster.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                    }

                    _usersMgr = new UsersMgr(connectionString);
                    System.Data.DataTable dt_User = _usersMgr.Query(Request.Form["user_id"] ?? "");
                    if (dt_User != null && dt_User.Rows.Count > 0)
                    {
                        priceMaster.user_id = Convert.ToUInt32(dt_User.Rows[0]["user_id"]);
                    }

                    Resource.CoreMessage = new CoreResource("Product");
                    _priceMasterMgr = new PriceMasterMgr(connectionString);
                    _priceMasterTsMgr = new PriceMasterTsMgr("");
                    int priceMasterId = _priceMasterMgr.Save(priceMaster, itemPrices, null, ref msg);
                    if (priceMasterId != -1)
                    {
                        //價格修改 申請審核
                        PriceUpdateApply priceUpdateApply = new PriceUpdateApply { price_master_id = Convert.ToUInt32(priceMasterId) };
                        priceUpdateApply.apply_user = Convert.ToUInt32((Session["caller"] as Caller).user_id);

                        //價格審核記錄
                        PriceUpdateApplyHistory applyHistroy = new PriceUpdateApplyHistory();
                        applyHistroy.user_id = Convert.ToInt32(priceUpdateApply.apply_user);
                        //applyHistroy.price_status = 1;
                        //applyHistroy.type = 3;
                        applyHistroy.price_status = 1; //edit by wwei0216w 2014/12/16 價格修改時 price_status為 2申請審核
                        applyHistroy.type = 1;//edit by wwei0216w 所作操作為 1:申請審核的操作 


                        _priceUpdateApplyMgr = new PriceUpdateApplyMgr(connectionString);
                        _priceUpdateApplyHistoryMgr = new PriceUpdateApplyHistoryMgr(connectionString);

                        int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                        if (apply_id != -1)
                        {
                            priceMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                            priceMaster.apply_id = Convert.ToUInt32(apply_id);
                            applyHistroy.apply_id = apply_id;
                            ArrayList excuteSql = new ArrayList();
                            excuteSql.Add(_priceMasterMgr.Update(priceMaster));
                            excuteSql.Add(_priceMasterTsMgr.UpdateTs(priceMaster));//edit by xiangwang0413w 2014/07/22 更新price_master_ts表後用時更新price_master_ts表，以便價格審核
                            excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));

                            _functionMgr = new FunctionMgr(connectionString);
                            string function = Request.Params["function"] ?? "";
                            Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                            int functionid = fun == null ? 0 : fun.RowId;
                            HistoryBatch batch = new HistoryBatch { functionid = functionid };
                            batch.batchno = Request.Params["batch"] ?? "";
                            batch.kuser = (Session["caller"] as Caller).user_email;

                            _tableHistoryMgr = new TableHistoryMgr(connectionString);
                            if (_tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, excuteSql))
                            {
                                json = "{success:true}";
                            }
                            else
                            {
                                json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                            }
                        }
                        else
                        {
                            json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:'" + msg + "'}";
                    }
                    #endregion
                }
                else
                {
                    #region product_item_temp 修改临时表数据

                    ProductTemp proTemp = new ProductTemp { Bonus_Percent = bonus_percent, Default_Bonus_Percent = default_bonus_percent };
                    List<ProductItemTemp> proItemTemps = jsSer.Deserialize<List<ProductItemTemp>>(items);
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        proTemp.Product_Id = Request.Form["OldProductId"];
                        proItemTemps.ForEach(m => m.Product_Id = proTemp.Product_Id);
                    }

                    if (!string.IsNullOrEmpty(start))
                    {
                        proTemp.Bonus_Percent_Start = Convert.ToUInt32(CommonFunction.GetPHPTime(start));
                        proItemTemps.ForEach(m => m.Event_Product_Start = proTemp.Bonus_Percent_Start);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        proTemp.Bonus_Percent_End = Convert.ToUInt32(CommonFunction.GetPHPTime(end));
                        proItemTemps.ForEach(m => m.Event_Product_End = proTemp.Bonus_Percent_End);
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
                    proTemp.Writer_Id = (Session["caller"] as Caller).user_id;
                    proTemp.Combo_Type = COMBO_TYPE;
                    proItemTemps.ForEach(m => m.Writer_Id = proTemp.Writer_Id);

                    _productTempMgr = new ProductTempMgr(connectionString);
                    #region PriceMasterTemp
                    PriceMasterTemp priceMasterTemp = new PriceMasterTemp { price_status = 1, default_bonus_percent = default_bonus_percent, bonus_percent = bonus_percent, same_price = same_price };
                    priceMasterTemp.accumulated_bonus = Convert.ToUInt32(accumulated_bonus);
                    priceMasterTemp.product_id = proTemp.Product_Id;
                    priceMasterTemp.combo_type = COMBO_TYPE;
                    priceMasterTemp.product_name = PriceMaster.Product_Name_FM(Request.Form["product_name"] ?? "");
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
                    if (!string.IsNullOrEmpty(bonus_start))
                    {
                        priceMasterTemp.bonus_percent_start = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_start));
                    }
                    if (!string.IsNullOrEmpty(bonus_start))
                    {
                        priceMasterTemp.bonus_percent_end = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_end));
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

                    _productItemTempMgr = new ProductItemTempMgr(connectionString);
                    _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                    if (_productItemTempMgr.UpdateCostMoney(proItemTemps) && _productTempMgr.PriceBonusInfoSave(proTemp) > 0)
                    {
                        bool result = false;
                        PriceMasterTemp query = new PriceMasterTemp { writer_Id = priceMasterTemp.writer_Id, product_id = proTemp.Product_Id, combo_type = COMBO_TYPE };
                        if (_priceMasterTempMgr.Query(query) == null)//插入
                        {
                            result = _priceMasterTempMgr.Save(new List<PriceMasterTemp> { priceMasterTemp }, null, null);
                        }
                        else//更新
                        {
                            result = _priceMasterTempMgr.Update(new List<PriceMasterTemp> { priceMasterTemp }, null);
                        }
                        json = "{success:" + result.ToString().ToLower() + "}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                    #endregion
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

        #region 修改站臺價格

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase UpdateItemPrice()
        {
            string json = string.Empty;

            try
            {
                if (!PriceMaster.CheckProdName(Request.Form["product_name"]))
                {
                    json = "{success:false,msg:'" + Resources.Product.FORBIDDEN_CHARACTER + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                uint priceMasterId = uint.Parse(Request.Form["price_master_id"] ?? "0");
                float default_bonus_percent = float.Parse(Request.Form["default_bonus_percent"] ?? "1");
                float bonus_percent = float.Parse(Request.Form["bonus_percent"] ?? "1");
                int same_price = (Request.Form["same_price"] ?? "") == "on" ? 1 : 0;
                int accumulated_bonus = (Request.Form["accumulated_bonus"] ?? "") == "on" ? 1 : 0;
                string start = Request.Form["event_start"] != null ? Request.Form["event_start"] : "0";//edit by zhuoqin0830w 2015/01/14 判斷時間是否為null
                string end = Request.Form["event_end"] != null ? Request.Form["event_end"] : "0";//edit by zhuoqin0830w 2015/01/14 判斷時間是否為null
                string bonus_start = Request.Form["bonus_percent_start"] ?? "";
                string bonus_end = Request.Form["bonus_percent_end"] ?? "";
                string valid_start = Request.Form["valid_start"] ?? "0";//edit by zhuoqin0830w 2015/01/28 判斷時間是否為null
                string valid_end = Request.Form["valid_end"] ?? "0";//edit by zhuoqin0830w 2015/01/28 判斷時間是否為null
                string items = Request.Form["Items"];
                List<ItemPrice> newPrices = jsSer.Deserialize<List<ItemPrice>>(items);

                _priceMasterMgr = new PriceMasterMgr(connectionString);
                _priceMasterTsMgr = new PriceMasterTsMgr("");
                PriceMaster priceMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = priceMasterId }).FirstOrDefault();
                if (priceMaster != null)
                {
                    #region 處理PriceMaster
                    priceMaster.user_id = 0;
                    priceMaster.product_name = PriceMaster.Product_Name_FM(Request.Form["productFormat"] ?? "");
                    priceMaster.same_price = same_price;
                    priceMaster.accumulated_bonus = Convert.ToUInt32(accumulated_bonus);
                    priceMaster.user_level = uint.Parse(Request.Form["user_level"] ?? "1");
                    if (!string.IsNullOrEmpty(start))
                    {
                        priceMaster.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(start));
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        priceMaster.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(end));
                    }
                    if (!string.IsNullOrEmpty(bonus_start))
                    {
                        priceMaster.bonus_percent_start = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_start));
                    }
                    if (!string.IsNullOrEmpty(bonus_end))
                    {
                        priceMaster.bonus_percent_end = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_end));
                    }
                    if (!string.IsNullOrEmpty(valid_start))
                    {
                        priceMaster.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                    }
                    if (!string.IsNullOrEmpty(valid_end))
                    {
                        priceMaster.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                    }


                    if (!string.IsNullOrEmpty(Request.Form["user_id"]))
                    {
                        _usersMgr = new UsersMgr(connectionString);
                        System.Data.DataTable dt_User = _usersMgr.Query(Request.Form["user_id"]);
                        if (dt_User != null && dt_User.Rows.Count > 0)
                        {
                            priceMaster.user_id = Convert.ToUInt32(dt_User.Rows[0]["user_id"]);
                        }
                    }
                    priceMaster.price = Convert.ToInt32(newPrices.Min(m => m.item_money));
                    priceMaster.event_price = Convert.ToInt32(newPrices.Min(m => m.event_money));
                    if (same_price == 0)
                    {
                        priceMaster.max_price = Convert.ToInt32(newPrices.Max(m => m.item_money));
                        priceMaster.max_event_price = Convert.ToInt32(newPrices.Max(m => m.event_money));
                    }
                    priceMaster.cost = Convert.ToInt32(newPrices.Min(m => m.item_cost));
                    priceMaster.event_cost = Convert.ToInt32(newPrices.Min(m => m.event_cost));
                    priceMaster.bonus_percent = bonus_percent;
                    priceMaster.default_bonus_percent = default_bonus_percent;
                    priceMaster.price_status = 2;//申請審核
                    #endregion

                    bool isExist = false;
                    List<PriceMasterCustom> masterList = _priceMasterMgr.Query(new PriceMaster { site_id = priceMaster.site_id, user_id = priceMaster.user_id, user_level = priceMaster.user_level, product_id = priceMaster.product_id });
                    List<PriceMasterCustom> resultList = masterList.Where(p => p.price_master_id != priceMaster.price_master_id).ToList();
                    if (resultList != null && resultList.Count() > 0)
                    {
                        if (priceMaster.user_id != 0 || (priceMaster.user_id == 0 && resultList.Where(p => p.user_id == 0).Count() > 0))
                        {
                            json = "{success:false,msg:'" + Resources.Product.SITE_EXIST + "'}";
                            isExist = true;
                        }
                    }
                    if (!isExist)
                    {
                        ArrayList excuteSql = new ArrayList();
                        Product product = null;
                        if (priceMaster.site_id == 1 && priceMaster.user_level == 1 && priceMaster.user_id == 0)
                        {
                            #region 處理Product

                            _productMgr = new ProductMgr(connectionString);
                            product = _productMgr.Query(new Product { Product_Id = priceMaster.product_id }).FirstOrDefault();
                            if (product != null)
                            {
                                product.Default_Bonus_Percent = default_bonus_percent;
                                product.Bonus_Percent = bonus_percent;
                            }
                            excuteSql.Add(_productMgr.Update(product, 0));
                            #endregion

                            #region 處理ProductItem

                            //_productItemMgr = new ProductItemMgr(connectionString);
                            //List<ProductItem> productItems = _productItemMgr.Query(new ProductItem { Product_Id = priceMaster.product_id });
                            //if (productItems != null)
                            //{
                            //    if (!string.IsNullOrEmpty(start))
                            //    {
                            //        productItems.ForEach(m => m.Event_Product_Start = Convert.ToUInt32(CommonFunction.GetPHPTime(start)));
                            //    }
                            //    if (!string.IsNullOrEmpty(end))
                            //    {
                            //        productItems.ForEach(m => m.Event_Product_End = Convert.ToUInt32(CommonFunction.GetPHPTime(end)));
                            //    }
                            //    newPrices.ForEach(m =>{
                            //        ProductItem pi = productItems.Find(n => n.Item_Id == m.item_id);
                            //        pi.Item_Money = m.item_money;
                            //        pi.Item_Cost = m.item_cost;
                            //        pi.Event_Item_Money = m.event_money;
                            //        pi.Event_Item_Cost = m.event_cost;
                            //    }); 
                            //    productItems.ForEach(m => excuteSql.Add(_productItemMgr.Update(m)));
                            //}
                            #endregion
                        }
                        //價格修改 申請審核
                        PriceUpdateApply priceUpdateApply = new PriceUpdateApply { price_master_id = priceMasterId };
                        priceUpdateApply.apply_user = Convert.ToUInt32((Session["caller"] as Caller).user_id);

                        //價格審核記錄
                        PriceUpdateApplyHistory applyHistroy = new PriceUpdateApplyHistory();
                        applyHistroy.user_id = Convert.ToInt32(priceUpdateApply.apply_user);
                        //applyHistroy.price_status = 1;
                        //applyHistroy.type = 3;
                        applyHistroy.price_status = 1; //edit by wwei0216w 2014/12/16 價格修改時 price_status為 2申請審核
                        applyHistroy.type = 1;//edit by wwei0216w 所作操作為 1:申請審核的操作 

                        _priceUpdateApplyMgr = new PriceUpdateApplyMgr(connectionString);
                        _priceUpdateApplyHistoryMgr = new PriceUpdateApplyHistoryMgr(connectionString);
                        _tableHistoryMgr = new TableHistoryMgr(connectionString);

                        bool result = true;
                        int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                        if (apply_id != -1)
                        {
                            priceMaster.apply_id = Convert.ToUInt32(apply_id);
                            applyHistroy.apply_id = apply_id;
                            //excuteSql.Add(_priceMasterMgr.Update(priceMaster)); 
                            excuteSql.Add(_priceMasterTsMgr.UpdateTs(priceMaster)); //修改站台價格，不再更新price_master表，改為更新price_master_ts表  edit by xiangwang0413w 2014/07/21
                            excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));

                            //item_price_id==0 新增規格，做新增動作
                            _itemPriceMgr = new ItemPriceMgr(connectionString);
                            newPrices.FindAll(m => m.item_price_id == 0).ForEach(m => excuteSql.Add(_itemPriceMgr.Save(m)));

                            _functionMgr = new FunctionMgr(connectionString);
                            string function = Request.Params["function"] ?? "";
                            Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                            int functionid = fun == null ? 0 : fun.RowId;
                            HistoryBatch batch = new HistoryBatch { functionid = functionid };
                            batch.batchno = Request.Params["batch"] ?? "";
                            batch.kuser = (Session["caller"] as Caller).user_email;

                            if (_tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, excuteSql))
                            {
                                //_itemPriceMgr = new ItemPriceMgr("");
                                _itemPriceTsMgr = new ItemPriceTsMgr("");
                                foreach (var item in newPrices.FindAll(m => m.item_price_id != 0))
                                {
                                    item.apply_id = (uint)apply_id;//细项与主项使用相同的apply_id
                                    excuteSql = new ArrayList();
                                    //excuteSql.Add(_itemPriceMgr.Update(item));
                                    excuteSql.Add(_itemPriceTsMgr.UpdateTs(item));//修改站台價格，不再更新item_price表，改為更新item_price_ts表 edit by xiangwang0413w 2014/07/21
                                    if (!_tableHistoryMgr.SaveHistory<ItemPrice>(item, batch, excuteSql))
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                        //if (result&&product != null && product.Combination == 1)
                        //{
                        //    _productItemMgr = new ProductItemMgr(connectionString);
                        //    ProductItem pro_Item = new ProductItem() { Product_Id = product.Product_Id, Export_flag = 2 };
                        //    _productItemMgr.UpdateExportFlag(pro_Item);
                        //}
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
                json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 修改建議售價、寄倉費

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase UpdatePrice()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    _productMgr = new ProductMgr(connectionString);
                    Product pro = _productMgr.Query(new Product { Product_Id = uint.Parse(Request.Form["ProductId"]) }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Request.Form["product_price_list"]))
                    {
                        pro.Product_Price_List = uint.Parse(Request.Form["product_price_list"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["bag_check_money"]))
                    {
                        pro.Bag_Check_Money = uint.Parse(Request.Form["bag_check_money"]);
                    }
                    pro.show_listprice = Convert.ToUInt32((Request.Form["show_listprice"] ?? "") == "on" ? 1 : 0);

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid };
                    batch.batchno = Request.Params["batch"] ?? "";
                    batch.kuser = (Session["caller"] as Caller).user_email;

                    ArrayList sqls = new ArrayList();
                    sqls.Add(_productMgr.Update(pro));
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    bool result = _tableHistoryMgr.SaveHistory<Product>(pro, batch, sqls);

                    //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                    //if (result&&pro.Combination == 1)
                    //{
                    //    _productItemMgr = new ProductItemMgr(connectionString);
                    //    ProductItem pro_Item = new ProductItem() { Product_Id = pro.Product_Id, Export_flag = 2 };
                    //    _productItemMgr.UpdateExportFlag(pro_Item);
                    //}

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

        #region 查询商品站台信息

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetPriceMaster()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint productId = uint.Parse(Request.Form["ProductId"]);

                    _priceMasterMgr = new PriceMasterMgr(connectionString);
                    List<PriceMasterCustom> proSiteCustom = _priceMasterMgr.Query(new PriceMaster { product_id = productId });
                    StringBuilder strJson = new StringBuilder("[");
                    if (proSiteCustom != null)
                    {
                        foreach (var item in proSiteCustom)
                        {
                            strJson.Append("{");
                            strJson.AppendFormat("price_master_id:{0},product_id:{1},site_id:{2},site_name:\"{3}\"", item.price_master_id, item.product_id, item.site_id, item.site_name);
                            strJson.AppendFormat(",product_name:\"{0}\",product_name_format:\"{1}\",bonus_percent:{2},default_bonus_percent:{3}", item.product_name, PriceMaster.Product_Name_Op(item.product_name), item.bonus_percent, item.default_bonus_percent);
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
                    int writer_id = (Session["caller"] as Caller).user_id;
                    PriceMasterTemp query = new PriceMasterTemp { writer_Id = writer_id, combo_type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.product_id = Request.Form["OldProductId"];
                    }
                    _priceMasterTempMgr = new PriceMasterTempMgr(connectionString);
                    json = JsonConvert.SerializeObject(_priceMasterTempMgr.Query(query));
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

        #region 查詢站臺價格明細

        [HttpPost]
        [CustomHandleError]
        public JsonResult GetItemPrice()
        {
            List<ItemPriceCustom> itemPrices = new List<ItemPriceCustom>();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["PriceMasterId"]))
                {
                    _itemPriceMgr = new ItemPriceMgr(connectionString);
                    uint priceMasterId = uint.Parse(Request.Form["PriceMasterId"]);
                    itemPrices = _itemPriceMgr.Query(new ItemPrice { price_master_id = priceMasterId });

                    List<ItemPriceCustom> newAdds = _itemPriceMgr.QueryNewAdd(new ItemPrice { price_master_id = priceMasterId });
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

        #region 站臺價格狀態更改

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase UpdatePriceStatus()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["priceMasterId"]))
                {
                    uint priceMasterId = uint.Parse(Request.Form["priceMasterId"]);
                    _priceMasterMgr = new PriceMasterMgr(connectionString);
                    PriceMaster priceMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = priceMasterId }).FirstOrDefault();
                    if (priceMaster != null)
                    {
                        string status = Request.Form["updateStatus"];
                        if (status == "up")//上架
                        {
                            priceMaster.price_status = 1;

                        }
                        else if (status == "down")//下架
                        {
                            priceMaster.price_status = 4;
                        }


                        _functionMgr = new FunctionMgr(connectionString);
                        string function = Request.Params["function"] ?? "";
                        Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                        int functionid = fun == null ? 0 : fun.RowId;
                        HistoryBatch batch = new HistoryBatch { functionid = functionid };
                        batch.batchno = Request.Params["batch"] ?? "";
                        batch.kuser = (Session["caller"] as Caller).user_email;

                        ArrayList sqls = new ArrayList();
                        sqls.Add(_priceMasterMgr.Update(priceMaster));
                        _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        bool result = _tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, sqls);
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion

        #region 商品描述

        #region 商品標籤

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
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        uint productId = uint.Parse(Request.Form["ProductId"]);
                        _productTagSetMgr = new ProductTagSetMgr(connectionString);
                        List<ProductTagSet> tagSets = _productTagSetMgr.Query(new ProductTagSet { product_id = productId });
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
                    else
                    {
                        int writerId = (Session["caller"] as Caller).user_id;
                        ProductTagSetTemp query = new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                        if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                        {
                            query.product_id = Request.Form["OldProductId"];
                        }
                        _productTagSetTempMgr = new ProductTagSetTempMgr(connectionString);
                        List<ProductTagSetTemp> tagSetTemps = _productTagSetTempMgr.Query(query);
                        foreach (var item in tags)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='tag_{0}' name='tags' value='{0}' ", item.tag_id);
                            if (tagSetTemps.Exists(m => m.tag_id == item.tag_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='tag_{0}'><img src='{1}' style='margin-right:5px' /></label>", item.tag_id, imgServerPath + tagPath + GetDetailFolder(item.tag_filename) + item.tag_filename);
                        }
                    }
                    json = strJson.ToString();
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
                        uint productId = uint.Parse(Request.Form["ProductId"]);
                        _productNoticeSetMgr = new ProductNoticeSetMgr(connectionString);
                        List<ProductNoticeSet> noticeSets = _productNoticeSetMgr.Query(new ProductNoticeSet { product_id = productId });
                        foreach (var item in notices)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='notice_{0}' name='notices' value='{0}' ", item.notice_id);
                            if (noticeSets.Exists(m => m.notice_id == item.notice_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='notice_{0}'>{1}</label>", item.notice_id, item.notice_name);
                        }
                    }
                    else
                    {
                        int writerId = (Session["caller"] as Caller).user_id;
                        ProductNoticeSetTemp query = new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                        if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                        {
                            query.product_id = Request.Form["OldProductId"];
                        }
                        _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connectionString);
                        _productTempMgr = new ProductTempMgr(connectionString);
                        List<ProductNoticeSetTemp> noticeSetTemps = _productNoticeSetTempMgr.Query(query);
                        ProductTemp proTemp = _productTempMgr.GetProTemp(new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE });
                        bool check = (proTemp != null && !string.IsNullOrEmpty(proTemp.Page_Content_2)) ? false : true;
                        foreach (var item in notices)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='notice_{0}' name='notices' value='{0}' ", item.notice_id);
                            if (check || noticeSetTemps.Exists(m => m.notice_id == item.notice_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='notice_{0}'>{1}</label>", item.notice_id, item.notice_name);
                        }
                    }
                    json = strJson.ToString();
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

        #region 保存商品描述 標籤 公告

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

                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = uint.Parse(Request.Form["ProductId"]);
                    _productMgr = new ProductMgr(connectionString);
                    Product product = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                    if (product != null)
                    {
                        _functionMgr = new FunctionMgr(connectionString);
                        string function = Request.Params["function"] ?? "";
                        Function fun = _functionMgr.QueryFunction(function, "/Product/ProductSave");
                        int functionid = fun == null ? 0 : fun.RowId;
                        HistoryBatch batch = new HistoryBatch { functionid = functionid };
                        batch.batchno = Request.Params["batch"] ?? "";
                        batch.kuser = (Session["caller"] as Caller).user_email;

                        product.Page_Content_1 = Request.Form["page_content_1"] ?? "";
                        product.Page_Content_2 = Request.Form["page_content_2"] ?? "";
                        product.Page_Content_3 = Request.Form["page_content_3"] ?? "";
                        product.Product_Keywords = Request.Form["product_keywords"] ?? "";
                        if (!string.IsNullOrEmpty(Request.Form["product_buy_limit"]))
                        {
                            product.Product_Buy_Limit = uint.Parse(Request.Form["product_buy_limit"]);
                        }
                        ArrayList sqls = new ArrayList();
                        sqls.Add(_productMgr.Update(product, 0));
                        //TAG
                        List<ProductTagSet> tagSets = jsSer.Deserialize<List<ProductTagSet>>(tags);
                        tagSets.ForEach(m => m.product_id = product_id);
                        _productTagSetMgr = new ProductTagSetMgr("");
                        sqls.Add(_productTagSetMgr.Delete(new ProductTagSet { product_id = product_id }));
                        tagSets.ForEach(m => sqls.Add(_productTagSetMgr.Save(m)));
                        //NOTICE
                        List<ProductNoticeSet> noticeSets = jsSer.Deserialize<List<ProductNoticeSet>>(notices);
                        noticeSets.ForEach(m => m.product_id = product_id);
                        _productNoticeSetMgr = new ProductNoticeSetMgr("");
                        sqls.Add(_productNoticeSetMgr.Delete(new ProductNoticeSet { product_id = product_id }));
                        noticeSets.ForEach(m => sqls.Add(_productNoticeSetMgr.Save(m)));

                        _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        if (_tableHistoryMgr.SaveHistory<Product>(product, batch, sqls))
                        {
                            //若為單一商品,則把product_item.export_flag改為2 edit by xiangwang0413w 2014/06/30
                            //if (product.Combination == 1)
                            //{
                            //    _productItemMgr = new ProductItemMgr(connectionString);
                            //    ProductItem pro_Item = new ProductItem() { Product_Id = product.Product_Id, Export_flag = 2 };
                            //    _productItemMgr.UpdateExportFlag(pro_Item);
                            //}
                            json = "{success:true}";
                        }
                        else
                        {
                            json = "{success:false}";
                        }
                    }
                }
                else
                {
                    int writer_id = (Session["caller"] as Caller).user_id;
                    ProductTemp proTemp = new ProductTemp();
                    proTemp.Page_Content_1 = Request.Form["page_content_1"] ?? "";
                    proTemp.Page_Content_2 = Request.Form["page_content_2"] ?? "";
                    proTemp.Page_Content_3 = Request.Form["page_content_3"] ?? "";
                    proTemp.Product_Keywords = Request.Form["product_keywords"] ?? "";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        proTemp.Product_Id = Request.Form["OldProductId"];
                    }
                    if (!string.IsNullOrEmpty(Request.Form["product_buy_limit"]))
                    {
                        proTemp.Product_Buy_Limit = uint.Parse(Request.Form["product_buy_limit"]);
                    }
                    proTemp.Writer_Id = writer_id;
                    proTemp.Combo_Type = COMBO_TYPE;
                    List<ProductTagSetTemp> tagTemps = jsSer.Deserialize<List<ProductTagSetTemp>>(tags);
                    //tagTemps.ForEach(m => m.Writer_Id = writer_id);
                    foreach (ProductTagSetTemp item in tagTemps)
                    {
                        item.Writer_Id = writer_id;
                        item.Combo_Type = COMBO_TYPE;
                        item.product_id = proTemp.Product_Id;
                    }
                    List<ProductNoticeSetTemp> noticeTemps = jsSer.Deserialize<List<ProductNoticeSetTemp>>(notices);
                    //noticeTemps.ForEach(m => m.Writer_Id = writer_id);
                    foreach (ProductNoticeSetTemp item in noticeTemps)
                    {
                        item.Writer_Id = writer_id;
                        item.Combo_Type = COMBO_TYPE;
                        item.product_id = proTemp.Product_Id;
                    }
                    _productTempMgr = new ProductTempMgr(connectionString);
                    if (_productTempMgr.DescriptionInfoSave(proTemp, tagTemps, noticeTemps))
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
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

        #region 類別商品維護
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetProductCatagory(string id = "true")
        {
            string resultStr = "";
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            //BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            _procateMgr = new ProductCategoryMgr(connectionString);

            try
            {
                categoryList = _procateMgr.GetProductCate(new ProductCategory { });
                cateList = getCate(categoryList, "0");

                //調試resultlist是否為空
                GetCategoryList(categoryList, ref cateList);
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

        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, item.id.ToString());
                item.children = childList;
                if (childList.Count() > 0)
                {
                    GetCategoryList(categoryList, ref childList);
                }
            }
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

        #region 獲取所有商品 +HttpResponseBase GetAllProList()

        //[HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetAllProList()
        {
            string json = string.Empty;
            try
            {
                _productMgr = new ProductMgr(connectionString);
                ProductQuery query = new ProductQuery();
                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"].ToString());
                }
                else
                {
                    query.Limit = 500;
                }

                uint isTryUint = 0;
                if (!string.IsNullOrEmpty(Request.Params["keyCode"].ToString()))
                {

                    string keyCode = Request.Params["keyCode"].ToString();
                    keyCode = keyCode.Replace("，", ",");
                    keyCode = Regex.Replace(keyCode, @"\s+", ",");

                    if (keyCode.Substring(keyCode.Length - 1).Equals(","))
                    {
                        keyCode = keyCode.Substring(0, keyCode.Length - 1);
                    }
                    string[] ProductID = keyCode.Split(',');
                    try
                    {
                        for (int i = 0; i < ProductID.Length; i++)
                        {
                            int ID = int.Parse(ProductID[i]);
                        }
                        query.Product_Id = 0;
                        query.Product_Id_In = keyCode;
                    }
                    catch (Exception)
                    {
                        query.Product_Name = Request.Params["keyCode"].ToString();
                    }
                    //if (uint.TryParse(Request.Params["keyCode"].ToString(), out isTryUint))
                    //{
                    //    query.Product_Id = Convert.ToUInt32(Request.Params["keyCode"].ToString());
                    //}

                    //else
                    //{
                    //    query.Product_Name = Request.Params["keyCode"].ToString();
                    //}
                }
                if (uint.TryParse(Request.Params["status"].ToString(), out isTryUint))
                {
                    query.Product_Status = Convert.ToUInt32(Request.Params["status"].ToString());
                }
                if (uint.TryParse(Request.Params["brand_id"].ToString(), out isTryUint))
                {
                    query.Brand_Id = Convert.ToUInt32(Request.Params["brand_id"].ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                    {
                        VendorBrandSetMgr vbsMgr = new VendorBrandSetMgr(connectionString);
                        VendorBrandSet vbs = new VendorBrandSet();
                        vbs.class_id = Convert.ToUInt32(Request.Params["class_id"]);
                        List<VendorBrandSet> vbsList = vbsMgr.Query(vbs);
                        foreach (VendorBrandSet item in vbsList)
                        {
                            query.brandArry += item.brand_id;
                            query.brandArry += ",";
                        }
                        query.brandArry = query.brandArry.Substring(0, query.brandArry.Length - 1);
                    }

                }
                if (uint.TryParse(Request.Params["comboFrontCage_hide"].ToString(), out isTryUint))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["comboFrontCage_hide"].ToString());

                }
                int totalCount = 0;
                List<ProductDetailsCustom> prods = _productMgr.GetAllProList(query, out totalCount);
                json = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(prods) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{succes:false,totalCount:0,item:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取categoryset中的所有商品 +HttpResponseBase GetProductByCategorySet()
        public HttpResponseBase GetProductByCategorySet()
        {
            string resultStr = "{success:false}";
            try
            {
                _productMgr = new ProductMgr(connectionString);
                ProductQuery query = new ProductQuery();
                query.isjoincate = true;
                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"].ToString());
                }
                else
                {
                    query.Limit = 500;
                }
                uint isTryUint = 0;
                if (uint.TryParse(Request.Params["status"].ToString(), out isTryUint))
                {
                    query.Product_Status = Convert.ToUInt32(Request.Params["status"].ToString());
                }
                if (uint.TryParse(Request.Params["brand_id"].ToString(), out isTryUint))
                {
                    query.Brand_Id = Convert.ToUInt32(Request.Params["brand_id"].ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                    {
                        VendorBrandSetMgr vbsMgr = new VendorBrandSetMgr(connectionString);
                        VendorBrandSet vbs = new VendorBrandSet();
                        vbs.class_id = Convert.ToUInt32(Request.Form["class_id"]);
                        List<VendorBrandSet> vbsList = vbsMgr.Query(vbs);
                        foreach (VendorBrandSet item in vbsList)
                        {
                            query.brandArry += item.brand_id;
                            query.brandArry += ",";
                        }
                        query.brandArry = query.brandArry.Substring(0, query.brandArry.Length - 1);
                    }

                }
                if (!string.IsNullOrEmpty(Request.Params["keyCode"].ToString()))
                {

                    string keyCode = Request.Params["keyCode"].ToString();
                    keyCode = keyCode.Replace("，", ",");
                    keyCode = Regex.Replace(keyCode, @"\s+", ",");
                    if (keyCode.Substring(keyCode.Length - 1).Equals(","))
                    {
                        keyCode = keyCode.Substring(0, keyCode.Length - 1);
                    }
                    string[] ProductID = keyCode.Split(',');
                    try
                    {
                        for (int i = 0; i < ProductID.Length; i++)
                        {
                            int ID = int.Parse(ProductID[i]);
                        }
                        query.Product_Id = 0;
                        query.Product_Id_In = keyCode;
                    }
                    catch (Exception)
                    {
                        query.Product_Name = Request.Params["keyCode"].ToString();
                    }
                }
                //if (!string.IsNullOrEmpty(Request.Params["keyCode"].ToString()))
                //{
                //    if (uint.TryParse(Request.Params["keyCode"].ToString(), out isTryUint))
                //    {
                //        query.Product_Id = Convert.ToUInt32(Request.Params["keyCode"].ToString());
                //    }
                //    else
                //    {
                //        query.Product_Name = Request.Params["keyCode"].ToString();
                //    }
                //}

                if (string.IsNullOrEmpty(Request.Params["category_id"].ToString()))
                {
                    if (uint.TryParse(Request.Params["comboFrontCage_hide"].ToString(), out isTryUint))
                    {
                        query.category_id = Convert.ToUInt32(Request.Params["comboFrontCage_hide"].ToString());

                    }
                }
                else
                {
                    if (uint.TryParse(Request.Params["category_id"].ToString(), out isTryUint))
                    {
                        query.category_id = Convert.ToUInt32(Request.Params["category_id"].ToString());
                    }
                }
                int totalCount = 0;
                List<ProductDetailsCustom> prods = _productMgr.GetAllProList(query, out totalCount);
                resultStr = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(prods) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{succes:false,totalCount:0,item:[]}";
            }


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存商品到類別中去 +HttpResponseBase SaveProductCategorySet()
        public HttpResponseBase SaveProductCategorySet()
        {
            string[] bids = Request.Params["brandids"].Split('|');
            string categoryid = Request.Params["categoryid"];

            string[] pids = Request.Params["productids"].Split('|');
            string proIds = string.Empty;//保存已經存在于categorset中的product
            string resultStr = "{success:false}";
            try
            {
                for (int i = 0; i < bids.Length; i++)
                {
                    if (bids[i].ToString() != "" && pids[i].ToString() != "")
                    {
                        ProductCategorySetMgr _categorySetMgr = new ProductCategorySetMgr(connectionString);
                        ProductCategorySet pcs = new ProductCategorySet();

                        pcs.Brand_Id = Convert.ToUInt32(bids[i]);
                        pcs.Category_Id = Convert.ToUInt32(categoryid);
                        pcs.Product_Id = Convert.ToUInt32(pids[i]);
                        List<ProductCategorySet> queryList = _categorySetMgr.Query(pcs);
                        if (queryList.Count == 0)//該類別下不存在該商品時才新增
                        {
                            _categorySetMgr.Insert(pcs);
                        }
                        else
                        {
                            proIds += queryList[0].Product_Id;
                            if (i != bids.Length - 1)
                            {
                                proIds += ",";

                            }
                        }

                    }
                }

                resultStr = "{success:true,\"proIds\":\"" + proIds + "\"}";
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

        #region 刪除數據 +HttpResponseBase DeleteProductFromCategorySet()
        public HttpResponseBase DeleteProductFromCategorySet()
        {
            string bids = Request.Params["brandids"];
            string cids = Request.Params["categoryid"];
            string pids = Request.Params["productids"];
            string resultStr = "{success:false}";
            try
            {
                ProductCategorySetMgr _categorySetMgr = new ProductCategorySetMgr(connectionString);
                if (_categorySetMgr.DeleteProductByModelArry(bids, cids, pids))
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


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion

        #region 庫存匯入
        /// <summary>
        /// 上傳文件，匯入到datatable里
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UploadExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            productStockStores = new List<ProductStockQuery>();
            try
            {
                if (Request.Files["ImportExcelFile"] != null && Request.Files["ImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportExcelFile"];
                    //FileManagement fileManagement = new FileManagement();
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();
                    if (dt.Rows.Count > 0)
                    {
                        ProductStockQuery ps;
                        _pStockMgr = new ProductStockImportMgr(connectionString);
                        foreach (DataRow dr in dt.Rows)
                        {
                            ps = new ProductStockQuery();
                            ps.item_id = dr["細項編號"].ToString();
                            ps.product_id = dr["商品編號"].ToString();
                            ps.item_stock = dr["庫存"].ToString();
                            ps.item_alarm = dr["警告值"].ToString();
                            ps.product_name = dr["商品名稱"].ToString();
                            ps.spec_name1 = dr["規格1"].ToString();
                            ps.spec_name2 = dr["規格2"].ToString();
                            ps.product_mode = dr["出貨方式"].ToString();
                            ps.prepaid = dr["已買斷商品"].ToString();
                            ps.spec_status = dr["規格1顯示狀態"].ToString();
                            ps.spec_status2 = dr["規格2顯示狀態"].ToString();
                            //if (ps.spec_status == "顯示")
                            //{
                            //    ps.spec_status = "1";
                            //}
                            //else if (ps.spec_status == "隱藏")
                            //{
                            //    ps.spec_status = "0";
                            //}
                            //else {
                            //    ps.spec_status = string.Empty;
                            //}
                            //if (ps.spec_status2 == "顯示")
                            //{
                            //    ps.spec_status2 = "1";
                            //}
                            //else if (ps.spec_status2 == "隱藏")
                            //{
                            //    ps.spec_status2 = "0";
                            //}
                            //else
                            //{
                            //    ps.spec_status2 = string.Empty;
                            //}
                            ps.remark = dr["備註"].ToString();

                            if (dr[11].ToString() == "寄倉" || dr[12].ToString() == "是")
                            {
                                ps.type = 1;
                            }
                            productStockStores.Add(ps);
                        }
                    }
                    json = "{success:true,data:" + JsonConvert.SerializeObject(productStockStores, Formatting.Indented) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 匯入的數據進行顯示
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetProductStockList()
        {
            string json = string.Empty;
            try
            {
                json = "{success:true,data:" + JsonConvert.SerializeObject(productStockStores, Formatting.Indented) + "}";
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 對表中的數據進行更新
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateStock()
        {
            string json = string.Empty;
            ProductItem pi;
            try
            {
                // json = "{success:true,data:" + JsonConvert.SerializeObject(stores, Formatting.Indented) + "}";
                _pStockMgr = new ProductStockImportMgr(connectionString);
                // _pSpecStatusMgr = new ProductSpecMgr(connectionString);
                int i = 0;
                int j = 0;
                foreach (var item in productStockStores)
                {
                    pi = new ProductItem();
                    if (!string.IsNullOrEmpty(item.item_id))
                    {
                        pi.Item_Id = uint.Parse(item.item_id.Trim());
                    }
                    if (!string.IsNullOrEmpty(item.item_alarm))
                    {
                        pi.Item_Alarm = uint.Parse(item.item_alarm.Trim());
                    }
                    //非寄倉、非買斷商品更新庫存和警告值 +備註
                    if (item.type == 0)
                    {
                        if (!string.IsNullOrEmpty(item.item_stock))
                        {
                            pi.Item_Stock = int.Parse(item.item_stock.Trim());
                        }
                        if (!string.IsNullOrEmpty(item.remark))
                        {
                            pi.Remark = item.remark;
                        }
                        i += _pStockMgr.UpdateStock(pi);
                    }
                    //寄倉、買斷商品只更新警告值,+備註
                    else
                    {
                        pi.Item_Stock = 999999999;
                        if (!string.IsNullOrEmpty(item.remark))
                        {
                            pi.Remark = item.remark;
                        }
                        j += _pStockMgr.UpdateStock(pi);
                    }
                    //if (item.spec_status == "顯示")
                    //{
                    //    item.spec_status = "1";
                    //}
                    //else if (item.spec_status == "隱藏")
                    //{
                    //    item.spec_status = "0";
                    //}
                    //else
                    //{
                    //    item.spec_status = string.Empty;
                    //}
                    //if (item.spec_status2 == "顯示")
                    //{
                    //    item.spec_status2 = "1";
                    //}
                    //else if (item.spec_status2 == "隱藏")
                    //{
                    //    item.spec_status2 = "0";
                    //}
                    //else
                    //{
                    //    item.spec_status2 = string.Empty;
                    //}
                    //_pSpecStatusMgr.Updspecstatus(item);
                }
                json = "{success:true,msg:\"" + "匯入成功！<br/>非寄倉、非買斷商品更新" + i + "條,<br/>寄倉、買斷商品更新" + j + "條。\"}";
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + "匯入失敗！" + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 物流配送模式
        /// <summary>
        /// 查詢物流配送模式
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="comboType">1為單一商品,2為組合商品</param>
        /// <param name="oldProductId">舊的商品id</param>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult GetProductDeliverySet(int productId, int comboType, int oldProductId = 0)
        {
            IEnumerable<ProductDeliverySet> deliverySets;
            if (productId == 0)//臨時表
            {
                ProductDeliverySetTempMgr _productDeliverySetTempMgr = new ProductDeliverySetTempMgr(connectionString);
                deliverySets = _productDeliverySetTempMgr.QueryByProductId(new ProductDeliverySetTemp { Product_id = productId, Combo_Type = comboType, Writer_Id = ((Caller)Session["caller"]).user_id });
            }
            else//正式表
            {
                _productDeliverySetMgr = new ProductDeliverySetMgr(connectionString);
                deliverySets = _productDeliverySetMgr.QueryByProductId(productId);
            }
            return Json(deliverySets.Select(d => d.Freight_big_area + "|" + d.Freight_type));
        }
        /// <summary>
        /// 保存物流設定
        /// </summary>
        /// <param name="proDeliverySets"></param>
        /// <param name="comboType">1為單一商品,2為組合商品</param>
        /// <returns></returns>
        [CustomHandleError]
        [HttpPost]
        public ActionResult ProductDeliverySetSave(string proDeliverySets, int comboType, int oldProductId = 0)
        {
            string json = string.Empty;
            try
            {
                List<ProductDeliverySet> lists = JsonConvert.DeserializeObject<List<ProductDeliverySet>>(proDeliverySets);
                Caller _caller = Session["caller"] as Caller;

                #region 驗證是否可以選擇本島店配
                //在做商品保存和修改時，驗證：
                //商品運送方式=常溫 && 出貨方式=寄倉 &&combination=單一商品時
                //才可以點本島宅配！
                //add by xiangwang0413w 2014/12/17
                if (lists.Find(p => p.Freight_type == 12) != null)
                {
                    Product prod;
                    if (lists[0].Product_id == 0)
                    {
                        _productTempMgr = new ProductTempMgr(connectionString);
                        prod = _productTempMgr.GetProTemp(new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = comboType, Product_Id = oldProductId.ToString() });
                    }
                    else
                    {
                        _productMgr = new ProductMgr(connectionString);
                        prod = _productMgr.Query(new Product { Product_Id = (uint)lists[0].Product_id }).First();
                    }

                    if (!prod.CheckdStoreFreight())
                    {
                        return Json(new { success = false, msg = Resources.Product.SHOP_TRANSPORT_NOT });
                    }
                }
                #endregion

                bool result = false;
                if (lists[0].Product_id == 0)//新增臨時表
                {
                    _productDeliverySetTempMgr = new ProductDeliverySetTempMgr(connectionString);
                    List<ProductDeliverySetTemp> tempProDeliSets = new List<ProductDeliverySetTemp>();

                    lists.ForEach(m =>
                    {
                        tempProDeliSets.Add(new ProductDeliverySetTemp
                        {
                            Product_id = oldProductId,
                            Freight_big_area = m.Freight_big_area,
                            Freight_type = m.Freight_type,
                            Writer_Id = _caller.user_id,
                            Combo_Type = comboType
                        });
                    });
                    result = _productDeliverySetTempMgr.Save(tempProDeliSets, tempProDeliSets[0].Product_id, tempProDeliSets[0].Combo_Type, tempProDeliSets[0].Writer_Id);
                }
                else//修改正式表
                {
                    _productDeliverySetMgr = new ProductDeliverySetMgr(connectionString);
                    result = _productDeliverySetMgr.Save(lists, lists[0].Product_id);
                }
                json = "{success:" + result.ToString().ToLower() + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
            }
            return Content(json);
        }
        #endregion

        #region 批次上傳商品類別
        public HttpResponseBase productcategoryinto()
        {
            string json = string.Empty;
            DTExcel.Clear();
            DTExcel.Columns.Clear();
            string newName = string.Empty;
            DTExcel.Columns.Add("產品ID", typeof(String));
            DTExcel.Columns.Add("類別ID", typeof(String));
            DTExcel.Columns.Add("不能匯入的原因", typeof(String));
            int result = 0;
            int count = 0;//總匯入數
            int errorcount = 0;
            int chongfucount = 0;
            StringBuilder strsql = new StringBuilder();
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];
                    //FileManagement fileManagement = new FileManagement();
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    dt = CsvHelper.ReadCsvToDataTable(newName, true);
                    _productMgr = new ProductMgr(connectionString);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                int a = Convert.ToInt32(dr[0]);//產品ID
                                int b = Convert.ToInt32(dr[1]);//類別ID
                                if (_productMgr.Yesornoexist(a, b) > 0)//大於1表示該數據庫內存在該值
                                {
                                    DataRow drtwo = DTExcel.NewRow();
                                    drtwo[0] = dr[0].ToString();
                                    drtwo[1] = dr[1].ToString();
                                    drtwo[2] = "數據庫中該行數據已存在";
                                    DTExcel.Rows.Add(drtwo);
                                    chongfucount++;
                                    continue;
                                    //someproduct_id = someproduct_id + a + ','; 
                                    //someproduct_categroy_id = someproduct_categroy_id + b + ',';
                                }
                                else//當數據不存在時進行添加數據
                                {
                                    int product_id = _productMgr.Yesornoexistproduct(a);
                                    int category_id = _productMgr.Yesornoexistproductcategory(b);
                                    if (product_id > 0 && category_id > 0)
                                    {
                                        count++;
                                        strsql.AppendFormat(@"insert into product_category_set(product_id,category_id)values('{0}','{1}');", a, b);
                                        continue;
                                    }
                                    else if (product_id == 0 && category_id == 0)
                                    {
                                        DataRow drtwo = DTExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = "類別ID和產品ID在類別表和產品表中均不存在";
                                        DTExcel.Rows.Add(drtwo);
                                        errorcount++;
                                        continue;
                                    }
                                    else if (product_id > 0 && category_id == 0)
                                    {
                                        DataRow drtwo = DTExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = "類別ID在類別表中不存在";
                                        DTExcel.Rows.Add(drtwo);
                                        errorcount++;
                                        continue;
                                    }
                                    else if (product_id == 0 && category_id > 0)
                                    {
                                        DataRow drtwo = DTExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = "產品ID在產品表中不存在";
                                        DTExcel.Rows.Add(drtwo);
                                        errorcount++;
                                        continue;
                                    }

                                }
                            }
                            catch
                            {
                                DataRow drtwo = DTExcel.NewRow();
                                drtwo[0] = dr[0].ToString();
                                drtwo[1] = dr[1].ToString();
                                drtwo[2] = "數據異常";
                                DTExcel.Rows.Add(drtwo);
                                errorcount++;
                                continue;
                            }
                        }
                        if (strsql.ToString().Trim() != "")
                        {
                            result = _productMgr.Updateproductcategoryset(strsql.ToString());
                            if (result > 0)
                            {
                                json = "{success:true,total:" + count + ",error:" + errorcount + ",repeat:" + chongfucount + "}";
                            }
                            else
                            {
                                json = "{success:false}";
                            }
                        }
                        else
                        {
                            json = "{success:true,total:" + 0 + ",error:" + errorcount + ",repeat:" + chongfucount + "}";
                        }
                    }
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + 0 + ",repeat:" + 0 + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                newName = string.Empty;
                DTExcel.Clear();
                DTExcel.Columns.Clear();
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 批次上傳商品類別匯出信息
        public void Updownmessage()
        {
            string json = string.Empty;
            try
            {
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(DTExcel, "目前不符合的數據.csv");
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "目前不符合的數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
        #endregion
        /// <summary>
        /// 獲得排序
        /// </summary>
        /// <param name="brand_id"></param>
        /// <returns></returns>
        public ActionResult GetProductSortNum(uint brand_id)
        {
            _productMgr = new ProductMgr(connectionString);
            return Content(_productMgr.GetProductSort(brand_id).ToString());
        }

        #region 商品詳情文字
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetProductList()
        {
            ProductQuery query = new ProductQuery();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _productMgr = new ProductMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                    query.Vendor_Id = uint.Parse(Request.Params["vendor_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.Brand_Id = uint.Parse(Request.Params["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_status"]))
                {
                    query.Product_Status = uint.Parse(Request.Params["product_status"]);
                }
                else
                {
                    query.Product_Status = 999;//未選擇商品狀態
                }
                if (!string.IsNullOrEmpty(Request.Params["creater"]))
                {
                    query.create_username = Request.Params["creater"];
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.time_start = Convert.ToDateTime(Request.Params["timestart"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.time_end = Convert.ToDateTime(Request.Params["timeend"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_content"]))
                {
                    //支持空格，中英文逗號隔開
                    string content = Regex.Replace(Request.Params["search_content"].Trim(), "(\\s+)|(，)|(\\,)", ",");
                    string[] contents = content.Split(',');
                    int pid = 0;
                    for (int i = 0; i < contents.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(contents[i].Trim()))
                        {
                            if (query.pids == "" && query.Product_Name == "")
                            {
                                if (int.TryParse(contents[i], out pid))
                                {
                                    query.pids += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                                else
                                {
                                    query.Product_Name += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                            }
                            else
                            {
                                if (int.TryParse(contents[i], out pid))
                                {
                                    query.pids += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                                else
                                {
                                    query.Product_Name += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
                string status = Request.Params["status"];
                //判斷是不是已編輯
                if (status == "0")
                {
                    query.product_detail_text = string.Empty;
                }
                else if (status == "1")
                {
                    query.product_detail_text = "1";
                }
                else if (status == "2")
                {
                    query.product_detail_text = "2";
                }
                if (!string.IsNullOrEmpty(Request.Params["combination"]))
                {
                    // Convert.ToUInt32(Request.Params["combination"]) != 0
                    query.Combination = uint.Parse(Request.Params["combination"]);
                }
                #region 供應商狀態、品牌狀態
                if (!string.IsNullOrEmpty(Request.Params["vendorState"]))
                {
                    query.vendor_status = Convert.ToInt32(Request.Params["vendorState"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brandState"]))
                {
                    query.brand_status = Convert.ToInt32(Request.Params["brandState"]);
                }
                #endregion
                DataTable dt = _productMgr.GetProductList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 更新商品詳情文字
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateProductDetail()
        {
            Product p = new Product();
            Product old_p = new Product();
            string json = string.Empty;
            try
            {
                _productMgr = new ProductMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    p.Product_Id = uint.Parse(Request.Params["product_id"]);
                    old_p = _productMgr.Query(new Product { Product_Id = p.Product_Id }).FirstOrDefault();
                }
                p.product_detail_text = Request.Params["product_detail_text"];
                if (!string.IsNullOrEmpty(p.product_detail_text))
                {
                    p.product_detail_text = p.product_detail_text.Replace("\\", "\\\\");
                    p.product_detail_text = p.product_detail_text.Replace("\n", "<br/>");
                }
                Caller _caller = (Session["caller"] as Caller);
                if (old_p != null)
                {
                    if (old_p.detail_createdate == DateTime.MinValue)//新增
                    {
                        p.detail_createdate = DateTime.Now;
                        p.detail_created = _caller.user_id;
                        p.detail_update = p.detail_created;
                        p.detail_updatedate = p.detail_createdate;
                    }
                    else
                    {
                        p.detail_created = old_p.detail_created;
                        p.detail_createdate = old_p.detail_createdate;
                        p.detail_update = _caller.user_id;
                        p.detail_updatedate = DateTime.Now;
                    }
                }

                int i = _productMgr.UpdateProductDeatail(p);
                if (i > 0)
                {
                    json = "{success:true,msg:'修改成功！'}";//返回json數據
                }
                else
                {
                    json = "{success:false,msg:'修改失敗！'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'異常！'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase GetVendor()
        {
            Vendor v = new Vendor();
            string json = string.Empty;
            try
            {
                _productMgr = new ProductMgr(connectionString);
                DataTable dt = _productMgr.GetVendor(v);
                json = "{success:true,data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 獲得品牌
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendorBrand()
        {
            VendorBrand vb = new VendorBrand();
            List<VendorBrand> store = new List<VendorBrand>();
            string json = string.Empty;
            try
            {
                vbMgr = new VendorBrandMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                    vb.Vendor_Id = uint.Parse(Request.Params["vendor_id"]);
                }
                store = vbMgr.GetProductBrandList(vb);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase GetProductDetialText()
        {
            ProductQuery query = new ProductQuery();
            string json = string.Empty;
            DataTable result = new DataTable();
            string notFind = string.Empty;
            try
            {
                _productMgr = new ProductMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Params["product_IDS"]))
                {
                    result = _productMgr.GetProductDetialText(query, Request.Params["product_IDS"], out notFind);
                    if (result != null)
                    {
                        for (int i = 0; i < result.Rows.Count; i++)
                        {
                            result.Rows[i][0] = result.Rows[i][0].ToString().Replace("<br/>", "\n");
                        }
                        json = "{success:true,data:" + JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented) + "}";

                    }
                    else
                    {
                        json = "{success:false,data:" + JsonConvert.SerializeObject(notFind, Newtonsoft.Json.Formatting.Indented) + "}";
                    }
                }

            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase GetProductDetialSafe()
        {
            string json = string.Empty;
            try
            {
                //string path = Server.MapPath("../Template/ProductDetail/product_detail_safe.txt");
                FileStream fs = new FileStream(Server.MapPath("../Template/ProductDetail/product_detail_safe.txt"), FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string strTemp = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                json = "{success:true,data:" + JsonConvert.SerializeObject(strTemp, Newtonsoft.Json.Formatting.Indented) + "}";
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 匯出
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase ProductDetailExport()
        {
            string json = "false";
            ProductQuery query = new ProductQuery();
            _productMgr = new ProductMgr(connectionString);
            if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
            {
                query.Vendor_Id = uint.Parse(Request.Params["vendor_id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
            {
                query.Brand_Id = uint.Parse(Request.Params["brand_id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["product_status"]))
            {
                query.Product_Status = uint.Parse(Request.Params["product_status"]);
            }
            else
            {
                query.Product_Status = 999;//未選擇商品狀態
            }
            if (!string.IsNullOrEmpty(Request.Params["creater"]))
            {
                query.create_username = Request.Params["creater"];
            }
            if (!string.IsNullOrEmpty(Request.Params["timestart"]))
            {
                query.time_start = Convert.ToDateTime(Request.Params["timestart"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["timeend"]))
            {
                query.time_end = Convert.ToDateTime(Request.Params["timeend"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["search_content"]))
            {
                //支持空格，中英文逗號隔開
                string content = Regex.Replace(Request.Params["search_content"].Trim(), "(\\s+)|(，)|(\\,)", ",");
                string[] contents = content.Split(',');
                int pid = 0;
                for (int i = 0; i < contents.Length; i++)
                {
                    if (!string.IsNullOrEmpty(contents[i].Trim()))
                    {
                        if (query.pids == "" && query.Product_Name == "")
                        {
                            if (int.TryParse(contents[i], out pid))
                            {
                                query.pids += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                            }
                            else
                            {
                                query.Product_Name += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                            }
                        }
                        else
                        {
                            if (int.TryParse(contents[i], out pid))
                            {
                                query.pids += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                            }
                            else
                            {
                                query.Product_Name += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
            }
            string status = Request.Params["status"];
            //判斷是不是已編輯
            if (status == "0")
            {
                query.product_detail_text = string.Empty;
            }
            else if (status == "1")
            {
                query.product_detail_text = "1";
            }
            else if (status == "2")
            {
                query.product_detail_text = "2";
            }
            if (!string.IsNullOrEmpty(Request.Params["combination"]))
            {
                query.Combination = uint.Parse(Request.Params["combination"]);
            }
            #region 供應商狀態、品牌狀態
            if (!string.IsNullOrEmpty(Request.Params["vendorState"]))
            {
                query.vendor_status = Convert.ToInt32(Request.Params["vendorState"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["brandState"]))
            {
                query.brand_status = Convert.ToInt32(Request.Params["brandState"]);
            }
            #endregion
            query.IsPage = false;

            DataTable dtHZ = new DataTable();

            string newExcelName = string.Empty;
            dtHZ.Columns.Add("供應商編號", typeof(String));
            dtHZ.Columns.Add("供應商名稱", typeof(String));
            dtHZ.Columns.Add("品牌名稱", typeof(String));
            dtHZ.Columns.Add("商品編號", typeof(String));
            dtHZ.Columns.Add("商品名稱", typeof(String));
            dtHZ.Columns.Add("建立人", typeof(String));
            dtHZ.Columns.Add("建立時間", typeof(String));
            dtHZ.Columns.Add("修改人", typeof(String));
            dtHZ.Columns.Add("修改時間", typeof(String));
            try
            {
                int totalCount = 0;
                DataTable dt = _productMgr.GetProductList(query, out totalCount);

                foreach (DataRow dr_v in dt.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dr_v["vendor_id"].ToString();
                    dr[1] = dr_v["vendor_name_full"].ToString();
                    dr[2] = dr_v["brand_name"].ToString();
                    dr[3] = dr_v["product_id"].ToString();
                    dr[4] = dr_v["product_name"].ToString();
                    dr[5] = dr_v["create_username"].ToString();
                    dr[6] = dr_v["detail_createdate"].ToString();
                    dr[7] = dr_v["update_username"].ToString();
                    dr[8] = dr_v["detail_updatedate"].ToString();

                    dtHZ.Rows.Add(dr);
                }
                string[] colname = new string[dtHZ.Columns.Count];
                string filename = "product_detail" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls"; ;
                newExcelName = Server.MapPath(excelPath_export) + filename;
                for (int i = 0; i < dtHZ.Columns.Count; i++)
                {
                    colname[i] = dtHZ.Columns[i].ColumnName;
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }

                ExcelHelperXhf.ExportDTtoExcel(dtHZ, "", newExcelName);
                //CsvHelper.ExportDataTableToCsv(dtHZ, newExcelName, colname, true);
                json = "{success:true,ExcelName:\'" + filename + "\'}";
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
