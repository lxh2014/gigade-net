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
using System.Xml.Linq;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class ProductListController : Controller
    {
        //
        // GET: /ProductList/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//add by hufeng0813w xml的文件
        private ISiteImplMgr _siteMgr;
        private VendorBrandMgr vbMgr;
        private ProductCategoryMgr _procateMgr;
        private IProductImplMgr _prodMgr;
        private IProductTempImplMgr _productTempMgr;
        private IPriceMasterImplMgr _pMaster;
        private IPriceMasterTsImplMgr _pMasterTsMgr;
        private IHistoryBatchImplMgr _historyBatchMgr;
        private ITableHistoryImplMgr _tableHistoryMgr;
        private ITableHistoryItemImplMgr _tableHistoryItemMgr;
        private IPriceUpdateApplyHistoryImplMgr _pHMgr;
        private IProductStatusApplyImplMgr _applyMgr;
        private IProductStatusHistoryImplMgr _statusHistoryMgr;
        private IProductImplMgr _productMgr;
        private IProductTagImplMgr _productTagMgr;
        private IProductTagSetImplMgr _productTagSetMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetImplMgr _productNoticeSetMgr;
        private IProductStatusHistoryImplMgr _productStatusHistoryMgr;
        private IFunctionImplMgr _functionMgr;
        //private IProductSpecImplMgr _productSpecMgr;
        private IProductItemImplMgr _productItemMgr;
        private IItemPriceImplMgr _itemPriceMgr;
        private IItemPriceTsImplMgr _itemPriceTsMgr;
        private ISiteConfigImplMgr _siteConfigMgr;

        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);

        string prod50Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod50Path);
        string tagPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_tagPath);

        string noticePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_noticePath);
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["product_id"]))
            {
                ViewBag.product_id = Request.QueryString["product_id"].ToString();
            }
            return View();
        }

        public ActionResult ProductDetails()
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

        public ActionResult PriceVerifyList()
        {
            return View();
        }

        #region  Action
        #region 匯出商品對照
        public HttpResponseBase ExportProductItemMap()
        {
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                #region 查询条件填充

                if (!string.IsNullOrEmpty(Request["brand_id"]))
                {
                    query.brand_id = uint.Parse(Request["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request["comboProCate_hide"]))
                {
                    query.cate_id = Request["comboProCate_hide"].Trim();
                }
                if (!string.IsNullOrEmpty(Request["comboFrontCage_hide"]))
                {
                    query.category_id = uint.Parse(Request["comboFrontCage_hide"]);
                    query.category_id = query.category_id == 2 ? 0 : query.category_id;
                }
                if (!string.IsNullOrEmpty(Request["combination"]))
                {
                    query.combination = int.Parse(Request["combination"]);
                }
                if (!string.IsNullOrEmpty(Request["product_status"]))
                {
                    query.product_status = int.Parse(Request["product_status"]);
                }
                if (!string.IsNullOrEmpty(Request["product_freight_set"]))
                {
                    query.freight = uint.Parse(Request["product_freight_set"]);
                }
                if (!string.IsNullOrEmpty(Request["product_mode"]))
                {
                    query.mode = uint.Parse(Request["product_mode"]);
                }
                if (!string.IsNullOrEmpty(Request["tax_type"]))
                {
                    query.tax_type = uint.Parse(Request["tax_type"]);
                }
                query.date_type = Request["date_type"] ?? "";
                if (!string.IsNullOrEmpty(Request["time_start"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                }
                if (!string.IsNullOrEmpty(Request["time_end"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                }
                query.name_number = Request["key"] ?? "";

                //add by zhuoqin0830w 2015/07/22  失格商品匯出
                query.off_grade = int.Parse(Request["off_grade"]);

                //添加預購商品 guodong1130w 2015/9/16添加
                query.purchase_in_advance = int.Parse(Request["purchase_in_advance"]);

                #region 站台條件(暫設)
                query.site_id = 1;
                query.user_level = 1;
                query.user_id = 0;
                #endregion

                #endregion

                IProductItemMapImplMgr _proItemMapMgr = new ProductItemMapMgr(connectionString);
                Resource.CoreMessage = new CoreResource("ProductItemMap");
                MemoryStream ms = _proItemMapMgr.ExportProductItemMap(query);

                this.Response.Clear();
                this.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddhhmmss")));
                this.Response.BinaryWrite(ms.ToArray());
                ms.Close();
                ms.Dispose();
                this.Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                this.Response.Write(Resource.CoreMessage.GetResource("EXPORT_ERROR"));
                this.Response.End();
            }
            return this.Response;
        }
        #endregion

        //edit by xiangwang0413w 2014/10/20
        /// <summary>
        /// 庫存資料匯出(單一商品)
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>excel文件</returns>
        public ActionResult ExportProduct(QueryVerifyCondition query, int exportFlag, string cols)
        {
            try
            {
                #region 查詢條件
                query.category_id = query.category_id == 2 ? 0 : query.category_id;
                query.date_type = query.date_type ?? "";
                if (!string.IsNullOrEmpty(query.time_start))
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(query.time_start).ToString("yyyy/MM/dd 00:00:00")).ToString();
                else query.time_start = "";
                if (!string.IsNullOrEmpty(query.time_end))
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(query.time_end).ToString("yyyy/MM/dd 23:59:59")).ToString();
                else query.time_end = "";
                #endregion

                _productMgr = new ProductMgr(connectionString);
                string xmlPath = string.Empty;
                switch (exportFlag)
                {
                    case 1://庫存資料匯出(單一商品)
                        xmlPath = "../XML/ProductStock.xml";
                        break;
                    case 2: //商品資料匯出
                        xmlPath = "../XML/ProductInfo.xml";
                        break;
                    case 3://商品價格匯出
                        xmlPath = "../XML/ProductPrice.xml";
                        break;
                    case 4:
                        xmlPath = "../XML/ItemPrice.xml";
                        break;
                    case 5://預購商品匯出  （沒有這個XML只是使用公用方法以防報錯）guodong1130w 2015/9/21
                        xmlPath = "../XML/ProductInfo.xml";
                        break;
                }

                MemoryStream ms = _productMgr.ExportProductToExcel(query, exportFlag, cols, Server.MapPath(xmlPath));//根據查詢條件和xml路徑得到excel文件流
                return File(ms.ToArray(), "application/-excel", DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Content("export errer");
            }
        }

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
                // json = "[{\"Site_Id\":\"1\",\"Site_Name\":\"吉甲地\"},{\"Site_Id\":\"2\",\"Site_Name\":\"天貓\"}]";


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
            List<Parametersrc> Paramelist = new List<Parametersrc>(); //保存全部數據項
            List<Parametersrc> ParameListByParent = new List<Parametersrc>();//保存父項Id
            string resultStr = "";
            try
            {
                //_procateMgr = new ProductCategoryMgr(connectionString);
                //cateList = _procateMgr.cateQuery(0);
                //GetCategoryList(ref cateList);
                //resultStr = JsonConvert.SerializeObject(cateList);
                ParameterMgr pm = new ParameterMgr(connectionString);
                Paramelist = pm.QueryUsed(new Parametersrc() { ParameterType = "product_cate" });
                ParameListByParent = Paramelist.FindAll(p => p.TopValue == "0");
                foreach (var p in ParameListByParent)
                {
                    cateList.Add(new ProductCategoryCustom { rowid = p.Rowid, parameterCode = p.ParameterCode, text = p.parameterName });
                }

                GetCategoryList(ref cateList, Paramelist);
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
        public void GetCategoryList(ref List<ProductCategoryCustom> catelist, List<Parametersrc> ParameteList)
        {
            List<Parametersrc> ParameListByChild = new List<Parametersrc>();
            foreach (ProductCategoryCustom item in catelist)
            {
                item.id = item.parameterCode;
                //List<ProductCategoryCustom> childList = _procateMgr.cateQuery(int.Parse(item.parameterCode));
                List<ProductCategoryCustom> childList = new List<ProductCategoryCustom>();
                ParameListByChild = ParameteList.FindAll(p => p.TopValue == item.parameterCode);
                foreach (var p in ParameListByChild)
                {
                    childList.Add(new ProductCategoryCustom { rowid = p.Rowid, parameterCode = p.ParameterCode, text = p.parameterName });
                }
                item.children = childList;

                if (childList.Count() > 0)
                {
                    GetCategoryList(ref childList, ParameListByChild);
                }
            }
        }
        //edit by wangwei0216w 2014/10/13 提高品類種類查詢時速度
        #endregion


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

        #region 查詢前台分類

        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetFrontCatagory()
        {
            //Response.Cache.SetOmitVaryStar(true);
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
            _prodMgr = new ProductMgr(connectionString);
            try
            {
                List<QueryandVerifyCustom> qvCusList = _prodMgr.QueryandVerify(qvCon, ref total);
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

                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                resultList = _prodMgr.verifyWaitQuery(query, out totalCount);

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


        /// <summary>
        /// 申請審核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase verifyApply()
        {
            List<QueryandVerifyCustom> resultList = new List<QueryandVerifyCustom>();
            string result = "{success:false}";
            bool resl = true;
            try
            {
                string prodcutIdStr = Request.Params["prodcutIdStr"];
                string[] productIds = prodcutIdStr.Split(',');
                string method = Request.Params["method"];
                Caller _caller = (Session["caller"] as Caller);
                _prodMgr = new ProductMgr(connectionString);
                _applyMgr = new ProductStatusApplyMgr(connectionString);
                _statusHistoryMgr = new ProductStatusHistoryMgr(connectionString);
                _tableHistoryMgr = new TableHistoryMgr(connectionString);
                _functionMgr = new FunctionMgr(connectionString);
                string function = Request.Params["function"] ?? "";
                Function fun = _functionMgr.QueryFunction(function, "/ProductList") ?? _functionMgr.QueryFunction(function, "/ProductList/ReplaceVerify");
                int functionid = fun == null ? 0 : fun.RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                string msg = "";
                foreach (string item in productIds.Distinct())
                {
                    ArrayList sqls = new ArrayList();
                    Product update = _prodMgr.Query(new Product { Product_Id = uint.Parse(item) }).FirstOrDefault();
                    //選擇自動上架時間時更改商品上架時間為選定時間
                    if (method.Equals("3"))
                    {
                        update.Product_Start = uint.Parse(BLL.gigade.Common.CommonFunction.GetPHPTime(Request.Params["product_start"]).ToString());
                        method = "1";
                    }
                    //若當前商品狀態不是新建商品或下架,則跳過申請
                    if (update.Product_Status != 0 && update.Product_Status != 6 && update.Product_Status!=7)
                    {
                        break;
                    }
                    //判斷商品是否失格 則 直接取消申請  add by  zhuoqin0830w 20105/07/01
                    if (update.off_grade == 1)
                    {
                        msg += "【" + update.Product_Id + "】商品是失格商品，不可申請審核！</br>";
                        break;
                    }
                    ProductStatusApply apply = new ProductStatusApply();
                    apply.product_id = uint.Parse(item);
                    apply.prev_status = update.Product_Status;
                    apply.online_mode = uint.Parse(method);
                    sqls.Add(_applyMgr.Save(apply));

                    ProductStatusHistory history = new ProductStatusHistory();
                    history.product_id = uint.Parse(item);
                    history.user_id = uint.Parse(_caller.user_id.ToString());
                    history.type = 1;               //操作類型        ???????????????????                      
                    history.product_status = 1;     //操作後狀態
                    //edit by zhuoqin0830w  2015/06/30  添加備註欄位
                    history.remark = Request.Form["Remark"];
                    sqls.Add(_statusHistoryMgr.Save(history));

                    batch.batchno = batchNo + update.Product_Id;
                    update.Product_Status = 1;    //狀態 -> 申請審核
                    sqls.Add(_prodMgr.Update(update, _caller.user_id));
                    if (!_tableHistoryMgr.SaveHistory<Product>(update, batch, sqls))
                    {
                        resl = false;
                    }
                    //若當前商品為單一商品並且商品狀態為新建商品,則將product_item.export_flag改為1
                    if (resl && update.Combination == 1 && apply.prev_status == 0)
                    {
                        _productItemMgr = new ProductItemMgr(connectionString);
                        ProductItem proItem = new ProductItem() { Product_Id = update.Product_Id, Export_flag = 1 };
                        _productItemMgr.UpdateExportFlag(proItem);
                    }
                }
                result = "{success:" + resl.ToString().ToLower() + ",'msg':'" + msg + "'}";
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

        #region 待審核商品列表

        /// <summary>
        /// 待審核商品列表查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase waitVerifyQuery()
        {
            List<QueryandVerifyCustom> resultList = new List<QueryandVerifyCustom>();
            string result = "{success:false}";
            try
            {
                uint brand_id, category_id;
                int combination, prev_status;
                uint.TryParse(Request.Params["brand_id"] ?? "0", out brand_id);
                uint.TryParse(Request.Params["category_id"] ?? "0", out category_id);
                int.TryParse(Request.Params["combination"] ?? "0", out combination);
                int.TryParse(Request.Params["prev_status"] ?? "0", out prev_status);

                QueryVerifyCondition query = new QueryVerifyCondition();

                query.product_status = 1;                       //待審核商品列表只查詢商品狀態為審請審核的

                query.brand_id = brand_id;
                if (!string.IsNullOrEmpty(Request.Params["cate_id"]))
                {
                    query.cate_id = Request.Params["cate_id"].Trim();
                }
                if (category_id != 2)    //ROOT 表全选
                {
                    query.category_id = category_id;
                }

                query.combination = combination;
                query.prev_status = prev_status;
                query.date_type = Request.Params["date_type"];
                query.name_number = Request.Params["key"];
                query.Start = Convert.ToInt32(Request.Form["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Form["limit"] ?? "25");

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

                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                resultList = _prodMgr.verifyWaitQuery(query, out totalCount);

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

        /// <summary>
        /// 待審核商品核可
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase vaiteVerifyPass()
        {
            string resultStr = "{success:false}";
            bool result = true;
            try
            {
                Caller _caller = (Session["caller"] as Caller);
                _applyMgr = new ProductStatusApplyMgr(connectionString);
                _statusHistoryMgr = new ProductStatusHistoryMgr(connectionString);
                _prodMgr = new ProductMgr(connectionString);
                _pMaster = new PriceMasterMgr(connectionString);
                _tableHistoryMgr = new TableHistoryMgr(connectionString);
                string productIds = Request.Params["prodcutIdStr"];
                string[] products = productIds.Split(',');

                _functionMgr = new FunctionMgr(connectionString);
                string function = Request.Params["function"] ?? "";
                Function fun = _functionMgr.QueryFunction(function, "/ProductList/VerifyList");
                int functionid = fun == null ? 0 : fun.RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                foreach (string item in products)
                {
                    Product product = _prodMgr.Query(new Product { Product_Id = uint.Parse(item) }).FirstOrDefault();

                    ArrayList sqls = new ArrayList();
                    if (_applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) }) != null)
                    {
                        batch.batchno = batchNo + product.Product_Id;
                        //更改商品价格之状态
                        PriceMaster pmQuery = new PriceMaster();
                        if (product.Combination != 0 && product.Combination != 1)   //组合商品
                        {
                            pmQuery.child_id = int.Parse(item);
                        }
                        else
                        {
                            pmQuery.child_id = 0;
                        }
                        pmQuery.product_id = uint.Parse(item);
                        pmQuery.price_status = 2;       //只更改价格状态为申请审核的商品价格        
                        List<PriceMaster> pmResultList = _pMaster.PriceMasterQuery(pmQuery);
                        if (pmResultList != null && pmResultList.Count() > 0)
                        {
                            _pHMgr = new PriceUpdateApplyHistoryMgr(connectionString);
                            List<PriceUpdateApplyHistory> pHList = new List<PriceUpdateApplyHistory>();
                            foreach (var pm in pmResultList)
                            {
                                ArrayList priceUpdateSqls = new ArrayList();
                                pm.price_status = 1;      //价格状态为上架
                                pm.apply_id = 0;
                                priceUpdateSqls.Add(_pMaster.Update(pm));
                                if (!_tableHistoryMgr.SaveHistory<PriceMaster>(pm, batch, priceUpdateSqls))
                                {
                                    result = false;
                                    break;
                                }

                                //价格异动记录（price_update_apply_history）                            
                                PriceUpdateApplyHistory pH = new PriceUpdateApplyHistory();
                                pH.apply_id = int.Parse(pm.apply_id.ToString());
                                pH.user_id = (Session["caller"] as Caller).user_id;
                                pH.price_status = 1;
                                pH.type = 1;
                                pHList.Add(pH);
                            }
                            if (!_pHMgr.Save(pHList))
                            {
                                result = false;
                                break;
                            }
                        }

                        //更改商品之状态
                        ProductStatusApply queryApply = _applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) });
                        uint online_mode = queryApply.online_mode;
                        //申請狀態為審核後立即上架時將上架時間改為當前時間，商品狀態改為上架
                        if (online_mode == 2)
                        {
                            product.Product_Status = 5;
                            product.Product_Start = uint.Parse(BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToLongTimeString()).ToString());
                        }
                        else
                        {
                            product.Product_Status = 2;
                            //product.Product_Start = online_mode;
                        }
                        sqls.Add(_prodMgr.Update(product, _caller.user_id));

                        ProductStatusHistory save = new ProductStatusHistory();
                        save.product_id = product.Product_Id;
                        save.user_id = uint.Parse(_caller.user_id.ToString());
                        save.type = 2;           //操作類型(核可)
                        save.product_status = int.Parse(product.Product_Status.ToString());

                        sqls.Add(_statusHistoryMgr.Save(save));         //保存历史记录

                        sqls.Add(_applyMgr.Delete(queryApply));         //刪除審核申請表中的數據

                        if (!_tableHistoryMgr.SaveHistory<Product>(product, batch, sqls))
                        {
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                resultStr = "{success:" + result.ToString().ToLower() + "}";
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

        /// <summary>
        /// 待審核商品駁回
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase vaiteVerifyBack()
        {
            string resultStr = "{success:false}";
            bool result = true;
            try
            {
                string productIds = Request.Params["productIds"];
                string backReason = Request.Params["backReason"];

                Caller _caller = (Session["caller"] as Caller);
                _applyMgr = new ProductStatusApplyMgr(connectionString);
                _statusHistoryMgr = new ProductStatusHistoryMgr(connectionString);
                _prodMgr = new ProductMgr(connectionString);
                _tableHistoryMgr = new TableHistoryMgr(connectionString);
                string[] products = productIds.Split(',');

                _functionMgr = new FunctionMgr(connectionString);
                string function = Request.Params["function"] ?? "";
                Function fun = _functionMgr.QueryFunction(function, "/ProductList/VerifyList");
                int functionid = fun == null ? 0 : fun.RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                foreach (string item in products)
                {
                    Product product = _prodMgr.Query(new Product { Product_Id = uint.Parse(item) }).FirstOrDefault();
                    ArrayList sqls = new ArrayList();
                    if (_applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) }) != null)
                    {
                        ProductStatusApply queryApply = _applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) });
                        uint prev_status = queryApply.prev_status;
                        product.Product_Status = prev_status;
                        sqls.Add(_prodMgr.Update(product, _caller.user_id));

                        ProductStatusHistory save = new ProductStatusHistory();
                        save.product_id = product.Product_Id;
                        save.user_id = uint.Parse(_caller.user_id.ToString());
                        save.type = 3;           //操作類型(駁回)
                        save.product_status = int.Parse(product.Product_Status.ToString());
                        save.remark = backReason;
                        sqls.Add(_statusHistoryMgr.Save(save));

                        batch.batchno = batchNo + product.Product_Id;

                        sqls.Add(_applyMgr.Delete(queryApply));
                        if (!_tableHistoryMgr.SaveHistory<Product>(product, batch, sqls))
                        {
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }

                resultStr = "{success:" + result.ToString().ToLower() + "}";

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

        #region 審核價格

        [HttpPost]
        public HttpResponseBase PriceVerify()
        {
            string json = string.Empty;
            try
            {
                _pMaster = new PriceMasterMgr(connectionString);
                _pMasterTsMgr = new PriceMasterTsMgr(connectionString);
                _itemPriceMgr = new ItemPriceMgr("");
                _itemPriceTsMgr = new ItemPriceTsMgr("");
                _prodMgr = new ProductMgr(connectionString);
                _tableHistoryMgr = new TableHistoryMgr(connectionString);
                _pHMgr = new PriceUpdateApplyHistoryMgr(connectionString);

                List<PriceMaster> priceMasters = JsonConvert.DeserializeObject<List<PriceMaster>>(Request.Params["priceMasters"]);

                List<PriceUpdateApplyHistory> pHList = new List<PriceUpdateApplyHistory>();

                _functionMgr = new FunctionMgr(connectionString);
                string function = Request.Params["function"] ?? "";
                Function fun = _functionMgr.QueryFunction(function, "/ProductList/PriceVerifyList");
                int functionid = fun == null ? 0 : fun.RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                int operationType = int.Parse(Request.Params["type"]);
                //edit by xiangwang0413w 2014/08/12 批量審核價格
                foreach (var item in priceMasters)
                {
                    //加上price_master 表中同一個product_id的所有價格 add by hufeng0813w 2014/06/12 Reason 各自定價父商品和子商品的價格狀態要同時更新
                    List<PriceMaster> ListpM = _pMasterTsMgr.QueryByApplyId(new PriceMaster { product_id = item.product_id, apply_id = item.apply_id });

                    //價格審核詳情
                    PriceUpdateApplyHistory pH = new PriceUpdateApplyHistory();
                    pH.user_id = (Session["caller"] as Caller).user_id;
                    pH.type = operationType;
                    pH.apply_id = (int)ListpM[0].apply_id;
                    foreach (var pM in ListpM)
                    {
                        ArrayList aList = new ArrayList();
                        uint applyId = pM.apply_id;
                        batch.batchno = batchNo + pM.product_id;
                        ItemPrice ip = new ItemPrice { price_master_id = pM.price_master_id, apply_id = pM.apply_id };
                        if (operationType == 1)//核可
                        {
                            pM.apply_id = 0;
                            pM.price_status = 1;

                            pH.price_status = 1; // 價格狀態為 1 :上架
                            pH.type = 2; //操作動作為2：核可
                            pHList.Add(pH);

                            aList.Add(_pMaster.Update(pM));//核可更新price_master表
                            if (pM.product_id != pM.child_id)
                            {
                                aList.Add(_itemPriceMgr.UpdateFromTs(ip));  //將Item_price_ts相對應數據導入Item_price表
                                aList.Add(_itemPriceTsMgr.DeleteTs(ip)); //更新成功後，刪除item_price_ts相應數據
                            }
                        }
                        else//駁回
                        {
                            pM.price_status = 3;
                            pH.price_status = 3; //加個狀態為 申請駁回
                            pH.type = 3; //操作動作為 3：駁回
                            pH.remark = Request.Params["reason"];
                            pHList.Add(pH);
                            if (pM.product_id != pM.child_id)
                            {
                                aList.Add(_itemPriceTsMgr.DeleteTs(ip)); //更新成功後，刪除item_price_ts相應數據
                            }
                        }
                        aList.Add(_pMasterTsMgr.DeleteTs(new PriceMaster { price_master_id = pM.price_master_id, apply_id = applyId }));//審核完成後刪除price_master_ts表
                        _tableHistoryMgr.SaveHistory<PriceMaster>(pM, batch, aList);
                    }
                }

                if (operationType == 1)//核可
                {
                    string[] prodList = (from p in priceMasters select p.product_id.ToString()).ToArray();

                    //將 獲取 時間 代碼提前  是後面獲得的 時間 相同
                    uint TimeNow = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());

                    for (int i = 0; i < prodList.Length; i++)
                    {
                        ArrayList pList = new ArrayList();
                        //查詢product
                        Product product = _prodMgr.Query(new Product() { Product_Id = uint.Parse(prodList[i]) })[0];
                        if (product.Product_Start < TimeNow && product.Product_End > TimeNow)
                        {
                            product.Product_Start = TimeNow;
                        }
                        //p.Product_Status = 5; //價格審核不應該更改商品狀態 edit by xiangwang0413w 20140826
                        pList.Add(_prodMgr.Update(product));
                        batch.batchno = batchNo + product.Product_Id;
                        _tableHistoryMgr.SaveHistory<Product>(product, batch, pList);
                    }

                }
                //價格審核記錄
                _pHMgr.Save(pHList);
                json = "{success:true}";
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

        #region 商品詳細信息查詢
        [HttpPost]
        public HttpResponseBase ProductDetailsQuery()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = uint.Parse(Request.Form["ProductId"]);
                    _productMgr = new ProductMgr(connectionString);
                    ProductDetailsCustom product = _productMgr.ProductDetail(new Product { Product_Id = product_id });
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
                json = "[]";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        #region 商品詳細信息單一商品之組合商品ID查詢
        [HttpPost]
        public HttpResponseBase ParentListQuery()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = uint.Parse(Request.Form["ProductId"]);
                    IProductComboImplMgr _comboMgr = new ProductComboMgr(connectionString);
                    string parent_list = _comboMgr.GetParentList(Convert.ToInt32(product_id));
                    json = "{success:true,data:" + JsonConvert.SerializeObject(parent_list) + "}";
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



        #region 商品列表查詢

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProList()
        {
            string json = string.Empty;
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                #region 查询条件填充

                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"]);
                }
                query.price_check = (Request.Form["price_check"] ?? "") == "true";
                if (!string.IsNullOrEmpty(Request.Form["brand_id"]))
                {
                    query.brand_id = uint.Parse(Request.Form["brand_id"]);
                }
                //庫存類型
                if (!string.IsNullOrEmpty(Request.Form["stockStatus"]))
                {
                    query.StockStatus = int.Parse(Request.Form["stockStatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["comboProCate_hide"]))
                {
                    query.cate_id = Request.Form["comboProCate_hide"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Form["comboFrontCage_hide"]))
                {
                    query.category_id = uint.Parse(Request.Form["comboFrontCage_hide"]);
                    query.category_id = query.category_id == 2 ? 0 : query.category_id;
                }
                if (!string.IsNullOrEmpty(Request.Form["combination"]))
                {
                    query.combination = int.Parse(Request.Form["combination"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["outofstock_time_days"]))//add by dongya 2015/10/22
                {
                    query.outofstock_days_stopselling = int.Parse(Request.Form["outofstock_time_days"]);
                }
                
                if (!string.IsNullOrEmpty(Request.Form["product_status"]))
                {
                    query.product_status = int.Parse(Request.Form["product_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_type"]))//add 2015/06/01
                {
                    query.product_type = int.Parse(Request.Form["product_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_freight_set"]))
                {
                    query.freight = uint.Parse(Request.Form["product_freight_set"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_mode"]))
                {
                    query.mode = uint.Parse(Request.Form["product_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["tax_type"]))
                {
                    query.tax_type = uint.Parse(Request.Form["tax_type"]);
                }
                query.date_type = Request.Form["date_type"] ?? "";
                if (!string.IsNullOrEmpty(Request.Form["time_start"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                }
                if (!string.IsNullOrEmpty(Request.Form["time_end"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                }
                query.name_number = Request.Form["key"] ?? "";
                if (query.price_check)
                {
                    if (!string.IsNullOrEmpty(Request.Form["site_id"]))
                    {
                        query.site_id = uint.Parse(Request.Form["site_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["user_level"]))
                    {
                        query.user_level = uint.Parse(Request.Form["user_level"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["price_status"]))
                    {
                        query.price_status = uint.Parse(Request.Form["price_status"]);
                    }
                }
                //增加查詢值  按鈕選中  add by zhuoqin0830w  2015/02/10
                if (!string.IsNullOrEmpty(Request.Form["priceCondition"]))
                {
                    query.priceCondition = int.Parse(Request.Form["priceCondition"]);
                }
                //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
                if (!string.IsNullOrEmpty(Request.Form["productPrepaid"]))
                {
                    query.Prepaid = int.Parse(Request.Form["productPrepaid"]);
                }
                //add by zhuoqin0830w  2015/06/30  失格商品篩選
                if (!string.IsNullOrEmpty(Request.Form["off_grade"]))
                {
                    query.off_grade = int.Parse(Request.Form["off_grade"]);
                }
                //add by guodong1130w 2015/09/16 預購商品篩選
                if (!string.IsNullOrEmpty(Request.Form["purchase_in_advance"]))
                {
                    query.purchase_in_advance = int.Parse(Request.Form["purchase_in_advance"]);
                }
                #endregion

                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                List<QueryandVerifyCustom> pros = _prodMgr.QueryByProSite(query, out totalCount);
                foreach (var item in pros)
                {
                    if ((item.product_image != "" && item.product_image!="-1"))
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

        #region 單一商品旗下6碼子商品id查詢

        [HttpPost]
        [CustomHandleError]
        public ActionResult QueryItemId()///add by wwei0216w 2015/8/12
        {
            string itemIdStr = "";
            string requestProduct_id = Request["product_id"];
            int product_id = requestProduct_id == null ? 0 : Convert.ToInt32(requestProduct_id);
            List<ProductItem> productItem_list = new List<ProductItem>();
            try
            {
                IProductItemImplMgr _productItemMgr = new ProductItemMgr(connectionString);
                productItem_list = _productItemMgr.GetProductItemByID(product_id);
                if (productItem_list.Count == 0)  return Json(new { itemIds = itemIdStr});
                foreach (ProductItem pi in productItem_list)
                {
                    itemIdStr += pi.Item_Id + ",";
                }
                itemIdStr = itemIdStr.Remove(itemIdStr.ToString().Length - 1, 1);
                return Json(new { itemIds = itemIdStr});
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }
        #endregion


        #region 商品列表查詢For批量增加站臺價格
        /// <summary>
        /// add by jiajun 2014/08/21
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProListForAddSta()
        {
            string json = string.Empty;
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                #region 查询条件填充

                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["brand_id"]))
                {
                    query.brand_id = uint.Parse(Request.Form["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["comboProCate_hide"]))
                {
                    query.cate_id = Request.Form["comboProCate_hide"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Form["comboFrontCage_hide"]))
                {
                    query.category_id = uint.Parse(Request.Form["comboFrontCage_hide"]);
                    query.category_id = query.category_id == 2 ? 0 : query.category_id;
                }
                if (!string.IsNullOrEmpty(Request.Form["combination"]))
                {
                    query.combination = int.Parse(Request.Form["combination"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_status"]))
                {
                    query.product_status = int.Parse(Request.Form["product_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_freight_set"]))
                {
                    query.freight = uint.Parse(Request.Form["product_freight_set"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_mode"]))
                {
                    query.mode = uint.Parse(Request.Form["product_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["tax_type"]))
                {
                    query.tax_type = uint.Parse(Request.Form["tax_type"]);
                }
                query.date_type = Request.Form["date_type"] ?? "";
                if (!string.IsNullOrEmpty(Request.Form["time_start"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                }
                if (!string.IsNullOrEmpty(Request.Form["time_end"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                }
                query.name_number = Request.Form["key"] ?? "";
                if (query.price_check)
                {
                    if (!string.IsNullOrEmpty(Request.Form["site_id"]))
                    {
                        query.site_id = uint.Parse(Request.Form["site_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["user_level"]))
                    {
                        query.user_level = uint.Parse(Request.Form["user_level"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["price_status"]))
                    {
                        query.price_status = uint.Parse(Request.Form["price_status"]);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["price_type"]))
                {
                    query.price_type = int.Parse(Request.Form["price_type"]);
                }
                #endregion

                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                List<QueryandVerifyCustom> pros = _prodMgr.QueryForStation(query, out totalCount);
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

        #region 商品下架

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductDown(int type = 1)
        {
            string json = "{success:true}";
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');
                    uint product_end =0 ;
                    if (!string.IsNullOrEmpty(Request.Form["Product_End"]))
                    {
                        product_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Form["Product_End"]));
                    }
                    //立即下架不改變下架時間
                    //else
                    //{
                    //    product_end = Convert.ToUInt32(CommonFunction.GetPHPTime());
                    //}

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/ProductList");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                    string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                    _prodMgr = new ProductMgr(connectionString);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _productStatusHistoryMgr = new ProductStatusHistoryMgr("");
                    ProductStatusHistory proStatusHistory = new ProductStatusHistory { product_status = 6, type = 4, user_id = Convert.ToUInt32((Session["caller"] as Caller).user_id) };
                    ArrayList sqls;
                    foreach (string str in pro_Ids.Distinct())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            uint product_id = uint.Parse(str);
                            Product pro = _prodMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                            if (type == 1 && product_end == 0)//說明是立即下架,就用原來的時間
                            {
                                product_end = pro.Product_End;
                            }
                            //edit by zhuoqin0830w  2015/06/26  添加備註欄位
                            string remark = "";
                            if (pro != null && pro.Product_Status == 5)// && pro.user_id == (Session["caller"] as Caller).user_id
                            {
                                switch (type)
                                {
                                    case 1:
                                        pro.Product_Status = 6;//下架
                                        remark = Request.Form["UnShelve"]; //edit by zhuoqin0830w  2015/06/26  添加備註欄位
                                        pro.Product_End = product_end;  //將 pro.Product_End = product_end 代碼提前  避免下架不販售的時候沒有下架時間  edit by zhuoqin0830w  2015/07/15
                                        break;
                                    case 2:
                                        pro.Product_Status = 99;//下架不販售
                                        remark = Request.Form["Remark"]; //edit by zhuoqin0830w  2015/06/26  添加備註欄位
                                        IProductExtImplMgr _prodExtMgr = new ProductExtMgr(connectionString);
                                        _prodExtMgr.UpdatePendDel(product_id, true);
                                        break;
                                }

                                sqls = new ArrayList();
                                sqls.Add(_prodMgr.Update(pro));
                                batch.batchno = batchNo + pro.Product_Id;
                                proStatusHistory.product_id = product_id;
                                //edit by zhuoqin0830w  2015/06/26  添加備註欄位
                                proStatusHistory.remark = remark;
                                sqls.Add(_productStatusHistoryMgr.Save(proStatusHistory));
                                if (!_tableHistoryMgr.SaveHistory<Product>(pro, batch, sqls))
                                {
                                    json = "{success:false}";
                                }
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

        #region 商品上架

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductUp()
        {
            string json = "{success:true}";
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/ProductList");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                    string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                    _prodMgr = new ProductMgr(connectionString);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _productStatusHistoryMgr = new ProductStatusHistoryMgr("");
                    ProductStatusHistory proStatusHistory = new ProductStatusHistory { product_status = 5, type = 6, user_id = Convert.ToUInt32((Session["caller"] as Caller).user_id) };
                    ArrayList sqls;
                    foreach (string str in pro_Ids.Distinct())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            uint product_id = uint.Parse(str);
                            Product pro = _prodMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                            //上架商品不能為失格商品  eidt by  zhuoqin0830w 20105/07/01  pro.off_grade != 1
                            string msg = "";
                            if (pro.off_grade == 1)
                            {
                                msg += "【" + pro.Product_Id + "】商品是失格商品，不可上架！</br>";
                                json = "{success:false,'msg':'" + msg + "'}";
                                break;
                            }
                            if (pro != null && pro.Product_Status == 2)//&& pro.user_id == (Session["caller"] as Caller).user_id 
                            {
                                pro.Product_Status = 5;//上架
                                pro.Product_Start = Convert.ToUInt32(CommonFunction.GetPHPTime());
                                sqls = new ArrayList();
                                sqls.Add(_prodMgr.Update(pro));

                                batch.batchno = batchNo + pro.Product_Id;

                                proStatusHistory.product_id = product_id;
                                //edit by zhuoqin0830w  2015/06/26  添加備註欄位
                                proStatusHistory.remark = Request.Form["Remark"];
                                sqls.Add(_productStatusHistoryMgr.Save(proStatusHistory));
                                if (!_tableHistoryMgr.SaveHistory<Product>(pro, batch, sqls))
                                {
                                    json = "{success:false}";
                                }
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

        #region 商品刪除

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductDelete()
        {
            string json = json = "{success:false}";
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');
                    _prodMgr = new ProductMgr(connectionString);
                    foreach (string str in pro_Ids.Distinct())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            uint product_id = uint.Parse(str);
                            Product pro = _prodMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                            if (pro != null && pro.Product_Status == 0)
                            {
                                if (_prodMgr.Delete(product_id))
                                {
                                    json = "{success:true}";
                                }
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

        #region --取消送審
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase Product_Cancel()
        {
            string resultStr = "{success:false}";
            bool result = true;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]))
                {
                    string[] pro_Ids = Request.Form["Product_Id"].Split('|');
                    _prodMgr = new ProductMgr(connectionString);

                    Caller _caller = (Session["caller"] as Caller);
                    _applyMgr = new ProductStatusApplyMgr(connectionString);
                    _statusHistoryMgr = new ProductStatusHistoryMgr(connectionString);
                    _prodMgr = new ProductMgr(connectionString);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/ProductList/VerifyList");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid, kuser = (Session["caller"] as Caller).user_email };
                    string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";

                    foreach (string item in pro_Ids.Distinct())
                    {
                        Product product = _prodMgr.Query(new Product { Product_Id = uint.Parse(item) }).FirstOrDefault();
                        ArrayList sqls = new ArrayList();
                        if (_applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) }) != null)
                        {
                            ProductStatusApply queryApply = _applyMgr.Query(new ProductStatusApply { product_id = uint.Parse(item) });
                            uint prev_status = queryApply.prev_status;
                            product.Product_Status = prev_status;
                            sqls.Add(_prodMgr.Update(product, _caller.user_id));

                            ProductStatusHistory save = new ProductStatusHistory();
                            save.product_id = product.Product_Id;
                            save.user_id = uint.Parse(_caller.user_id.ToString());
                            save.type = 8;           //操作類型(取消送審)
                            save.product_status = int.Parse(product.Product_Status.ToString());
                            save.remark = "錯誤送審";
                            sqls.Add(_statusHistoryMgr.Save(save));

                            batch.batchno = batchNo + product.Product_Id;

                            sqls.Add(_applyMgr.Delete(queryApply));
                            if (!_tableHistoryMgr.SaveHistory<Product>(product, batch, sqls))
                            {
                                result = false;
                                break;
                            }
                        }
                        else
                        {
                            result = false;
                            break;
                        }
                    }
                }
                resultStr = "{success:" + result.ToString().ToLower() + "}";

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

        #region 商品複製

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ProductCopy()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Product_Id"]) && !string.IsNullOrEmpty(Request.Form["Combination"]))
                {
                    string product_id = Request.Form["Product_Id"];
                    int writer_id = (Session["caller"] as Caller).user_id;
                    int combo_type = Convert.ToInt32(Request.Form["Combination"]);
                    combo_type = (combo_type == 0 || combo_type == 1) ? 1 : 2;
                    bool result = false;
                    _productTempMgr = new ProductTempMgr(connectionString);
                    result = _productTempMgr.CopyProduct(writer_id, combo_type, product_id);
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
                    int prod_classify = Convert.ToInt32(Request.Form["Prod_Classify"]);
                    string result = "";
                    if (type == "0")
                    {
                        switch (prod_classify)
                        {
                            case 10:
                                result += "http://" + DomainName + "/food/product_food.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");//商品預覽
                                break;
                            case 20:
                                result += "http://" + DomainName + "/stuff/product_stuff.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");//商品預覽
                                break;
                            default:
                                result += "http://" + DomainName + "/product.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");//商品預覽
                                break;
                        }
                    }
                    if (type == "1")
                    {
                        //商品預覽+價格頁面
                        //根據館別信息判斷進入哪一個連接  eidt  by zhuoqin0830w 2015/03/05
                        switch (prod_classify)
                        {
                            case 10:
                                result += "http://" + DomainName + "/food/product_food.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "&sid=" + site_id + "&ulv=" + user_level + "&uid=" + user_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");
                                result += "|";
                                result += "http://" + DomainName + "/food/product_food.php?pid=" + product_id + "&sid=" + site_id + "&code=" + hash.Md5Encrypt(product_id + "&sid=" + site_id, "32");//商品隱賣連結: + "&ulv=" + user_level
                                break;
                            case 20:
                                result += "http://" + DomainName + "/stuff/product_stuff.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "&sid=" + site_id + "&ulv=" + user_level + "&uid=" + user_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");
                                result += "|";
                                result += "http://" + DomainName + "/stuff/product_stuff.php?pid=" + product_id + "&sid=" + site_id + "&code=" + hash.Md5Encrypt(product_id + "&sid=" + site_id, "32");//商品隱賣連結: + "&ulv=" + user_level
                                break;
                            default:
                                result += "http://" + DomainName + "/product.php?pid=" + product_id + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "&sid=" + site_id + "&ulv=" + user_level + "&uid=" + user_id + "&view=" + DateTime.Now.ToString("yyyyMMdd");
                                result += "|";
                                result += "http://" + DomainName + "/product.php?pid=" + product_id + "&sid=" + site_id + "&code=" + hash.Md5Encrypt(product_id + "&sid=" + site_id, "32");//商品隱賣連結: + "&ulv=" + user_level
                                break;
                        }
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
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        uint productId = uint.Parse(Request.Form["ProductId"]);
                        _productTagSetMgr = new ProductTagSetMgr(connectionString);
                        List<ProductTagSet> tagSets = _productTagSetMgr.Query(new ProductTagSet { product_id = productId });
                        foreach (var item in tags)
                        {
                            if (tagSets.Exists(m => m.tag_id == item.tag_id))
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
                            if (noticeSets.Exists(m => m.notice_id == item.notice_id))
                            {
                                strJson.AppendFormat("{0}", item.notice_name);
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
                    List<ProductStatusHistoryCustom> resultList = _mgr.HistoryQuery(new ProductStatusHistoryCustom { product_id = uint.Parse(Request.Params["ProductId"]), create_channel = int.Parse(Request.Form["Create_Channel"]) });
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

        #region 上次異動紀律

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

        /// <summary>
        /// 排序設定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase sortSet()
        {
            string result = "{success:false}";

            string strJson = Request.Params["result"];
            _prodMgr = new ProductMgr(connectionString);
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<Product> productList = js.Deserialize<List<Product>>(strJson);
            if (_prodMgr.ExecUpdateSort(productList))
            {
                result = "{success:true}";
            }

            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }
        #endregion
    }
}