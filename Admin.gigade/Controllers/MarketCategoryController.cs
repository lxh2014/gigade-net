using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class MarketCategoryController : Controller
    {
        //
        // GET: /MarketCategory/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IMarketProductMapImplMgr _IMarketProductMap;
        private IParametersrcImplMgr _ptersrc;
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private static IMarketCategoryImplMgr _marketCategoryMgr;
        private IProductCategoryImplMgr _procateMgr;
        private IProductCategoryBannerImplMgr _bannerMgr;
        //static string SavePachs = ConfigurationManager.AppSettings["SavePachs"].ToString();

        private OrderMasterShopComMgr om;
        #region 視圖
        /// <summary>
        /// 美安類別管理
        /// </summary>
        /// <returns></returns>
        public ActionResult MarketCategoryList()
        {
            return View();
        }
        /// <summary>
        /// 美安類別關係設定表視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult MarketProductMap()
        {
            return View();
        }

        /// <summary>
        /// 美安類別匯入
        /// </summary>
        /// <returns></returns>
        public ActionResult MarketCategoryImport()
        {
            return View();
        }

        public ActionResult MarketOrder()
        {
            return View();
        }
        #endregion

        #region 美安類別匯入
        public HttpResponseBase ImportMarketCategory()
        {
            int j = 0;
            string json = string.Empty;//json字符串

            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];//獲取文件流
                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement
                    string fileLastName = excelFile.FileName;
                    string newExcelName = Server.MapPath(excelPath) + "MarketCategory" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    excelFile.SaveAs(newExcelName);//上傳文件
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    dt = helper.SheetData();
                    DataRow[] dr = dt.Select("market_category <> ''"); //定义一个DataRow数组,读取ds里面所有行
                    int rowsnum = dt.Rows.Count;

                    if (rowsnum == 0)
                    {
                        json = "{success:false,msg:0}";//此表內沒有數據或數據有誤,請檢查后再次匯入!
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    else
                    {

                        _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
                        j = _marketCategoryMgr.MarketCategoryImport(dr);
                        if (j > 0)
                        {
                            json = "{success:true,msg:1}";//操作成功共匯入total條數據
                        }
                        else
                        {
                            json = "{success:false,msg:2}";//匯入失敗,請檢查是否重複匯入！
                        }

                    }
                }
                else//當直接點擊時會產生,
                {
                    json = "{success:true,msg:3}";//未找到該文件
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;

                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + ex.ToString() + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 美安類別關係管理

        #region 美安類別關係設定列表頁，保存，刪除
        /// <summary>
        /// 美安類別關係設定列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetMarketProductMapList()
        {
            string jsonStr = string.Empty;
            int tranInt = 0;
            try
            {
                MarketProductMapQuery query = new MarketProductMapQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search"]))
                {
                    if (int.TryParse(Request.Params["search"].ToString(), out tranInt))
                    {
                        query.product_number = Request.Params["search"].ToString();
                    }
                    else
                    {
                        query.product_name = Request.Params["search"].ToString();
                    }
                }
                int totalCount = 0;
                _IMarketProductMap = new MarketProductMapMgr(mySqlConnectionString);

                DataTable _dt = _IMarketProductMap.GetMarketProductMapList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 保存美安類別關係
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SavetMarketProductMap()
        {

            string json = string.Empty;
            try
            {
                MarketProductMapQuery query = new MarketProductMapQuery();
                if (!string.IsNullOrEmpty(Request.Params["map_id"]))
                {
                    query.map_id = Convert.ToInt32(Request.Params["map_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["comboFrontCage_hide"]))
                {
                    query.product_category_id = Convert.ToInt32(Request.Params["comboFrontCage_hide"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["comboMarket_hide"]))
                {
                    query.market_category_id = Convert.ToInt32(Request.Params["comboMarket_hide"]);
                }

                query.kuser = (Session["caller"] as Caller).user_id;
                query.muser = (Session["caller"] as Caller).user_id;
                query.created = DateTime.Now;
                query.modified = query.created;
                _IMarketProductMap = new MarketProductMapMgr(mySqlConnectionString);
                int result = 0;
                //新增修改都在Mgr裡面判斷
                //通過map_id 是否為零 判斷新增還是編輯
                result = _IMarketProductMap.SavetMarketProductMap(query);
                if (result > 0)
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 刪除美安類別關係
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteMarketProductMap()
        {
            //MarketProductMapQuery query = new MarketProductMapQuery();
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    Row_id = Row_id.TrimEnd(',');
                    //query.map_id_in = Row_id;
                }
                _IMarketProductMap = new MarketProductMapMgr(mySqlConnectionString);
                int result = _IMarketProductMap.DeleteMarketProductMap(Row_id);
                if (result > 0)
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:false,msg:\"" + result + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        #region 吉甲地美安樹形結構圖和美安類別樹形結構圖
        #region 吉甲地美安樹狀節點
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetProductCatagory()
        {
            //吉甲地美安
            string resultStr = "";
            try
            {
                List<Parametersrc> Market = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                Market = _ptersrc.GetAllKindType("market_category");//物流業者
               

                if (Market.Count > 0)
                {
                    ProductCategory category = new ProductCategory();
                    category.category_name = "新館";
                    List<ProductCategory> categoryStore = new List<ProductCategory>();//-------取新館的id用來獲取product_category_banner的子節點
                   // List<ProductCategory> categoryList = new List<ProductCategory>();
                    List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
                  
                    _procateMgr = new ProductCategoryMgr(mySqlConnectionString);
                   
                    categoryStore = _procateMgr.QueryAll(category);//
                    if (categoryStore.Count > 0)//取father_id;//在product_category表查詢新館的編號
                    {
                        category = categoryStore[0];
                    }
                    else 
                    {
                        category.category_id = 754;
                    }
                    ProductCategoryBannerQuery bannerquery = new ProductCategoryBannerQuery();//查詢裡面屬於新錧裡面美安的父元素和子元素
                    bannerquery.banner_cateid = uint.Parse(Market[0].ParameterCode);
                    bannerquery.IsPage = false;
                    _bannerMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
                    List<ProductCategoryBannerQuery> bannerList = new List<ProductCategoryBannerQuery>();
                    int total = 0;
                    bannerList = _bannerMgr.GetProCateBanList(bannerquery, out total);
                   // categoryList = _procateMgr.GetProductCate(new ProductCategory { });

                    //cateList = getCate(categoryList, "2", Market[0].ParameterCode.ToString());
                    cateList = getCate(bannerList,category.category_id.ToString());
                    GetCategoryList(bannerList, ref cateList);
                   // GetCategoryList(categoryList, ref cateList);
                    resultStr = JsonConvert.SerializeObject(cateList);
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
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(List<ProductCategoryBannerQuery> categoryList, ref List<ProductCategoryCustom> catelist)
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

        public List<ProductCategoryCustom> getCate(List<ProductCategoryBannerQuery> categoryList, string fatherId)//, string code = null
        {
             var cateList=new  List<ProductCategoryCustom>();
            //if (string.IsNullOrEmpty(code))
            //{
                 cateList = (from c in categoryList
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
              
            //}else
            //{
            //    cateList = (from c in categoryList
            //                where c.category_id.ToString() == code && c.category_father_id.ToString() == fatherId 
            //                    select new
            //                    {
            //                        id = c.category_id,
            //                        text = c.category_name
            //                    }).ToList().ConvertAll<ProductCategoryCustom>(m => new ProductCategoryCustom
            //                    {
            //                        id = m.id.ToString(),
            //                        text = m.text
            //                    });
                
            //}
            return cateList;
        }

        #endregion

        #region 美安樹狀節點

        public List<ProductCategoryCustom> getCate(List<MarketCategoryQuery> categoryList, int fatherId)
        {
            var cateList = (from c in categoryList
                            where c.market_category_father_id == fatherId
                            select new
                            {
                                id = c.market_category_id,
                                text = c.market_category_name,
                                // parameterCode = c.market_category_father_id//--------新增的參數，這個方便查詢子節點

                            }).ToList().ConvertAll<ProductCategoryCustom>(m => new ProductCategoryCustom
                            {
                                id = m.id.ToString(),
                                text = m.text,
                                // parameterCode = m.parameterCode.ToString()//---------新增的參數
                            });

            return cateList;
        }
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetFrontCatagory()
        {
            //Response.Cache.SetOmitVaryStar(true);
            List<MarketCategoryQuery> categoryList = new List<MarketCategoryQuery>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            string resultStr = "";
            try
            {
                _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
                MarketCategory category = new MarketCategory();
                category.IsPage = false;
                categoryList = _marketCategoryMgr.GetMarketCategoryList(category);//查詢美安的所有節點
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
        public void GetFrontCateList(List<MarketCategoryQuery> categoryList, ref List<ProductCategoryCustom> catelist)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, int.Parse(item.id));//item.id改成item.parameterCode
                item.children = childList;

                if (childList.Count() > 0)
                {
                    GetFrontCateList(categoryList, ref childList);
                }
            }
        }
        #endregion

        #endregion

        #endregion
        #endregion

        #region 美安類別管理
        #region 美安類別列表頁
        public HttpResponseBase GetMarketCategoryList()
        {
            string jsonStr = string.Empty;
            int tranInt = 0;
            try
            {
                MarketCategoryQuery query = new MarketCategoryQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "0");

                query.market_category_father_id = Convert.ToInt32(Request.Params["father_id"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["search"]))
                {
                    if (int.TryParse(Request.Params["search"].ToString(), out tranInt))
                    {
                        query.market_category_code = Request.Params["search"].ToString();
                    }
                    else
                    {
                        query.market_category_name = Request.Params["search"].ToString();
                    }
                }
                int totalCount = 0;
                _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);

                List<MarketCategoryQuery> store = _marketCategoryMgr.GetMarketCategoryList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 獲取上一層父類別編號 + HttpResponseBase GetLastFatherId()
        /// <summary>
        /// 獲取上一層父類別編號
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult GetLastFatherId()
        {
            string jsonStr = string.Empty;
            try
            {
                MarketCategoryQuery model = new MarketCategoryQuery();

                if (!string.IsNullOrEmpty(Request.Params["fid"]))
                {
                    model.market_category_id = Convert.ToUInt32(Request.Params["fid"].ToString());
                }
                _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
                MarketCategoryQuery store = _marketCategoryMgr.GetMarketCategoryList(model).FirstOrDefault();

                return Json(new { success = "true", result = store.market_category_father_id, fatherName = store.market_category_father_name });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }

        }
        #endregion
        #region 美安類別編輯
        public HttpResponseBase SaveMarketCategory()
        {

            string json = string.Empty;
            try
            {
                _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
                MarketCategory query = new MarketCategory();
                // MarketCategoryQuery model = new MarketCategoryQuery();
                if (!string.IsNullOrEmpty(Request.Params["market_category_id"]))
                {
                    query.market_category_id = Convert.ToUInt32(Request.Params["market_category_id"]);
                    //model = _marketCategoryMgr.GetMarketCategoryList(query).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(Request.Params["market_category_father_id"]))
                {
                    query.market_category_father_id = Convert.ToInt32(Request.Params["market_category_father_id"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["market_category_code"]))
                {
                    query.market_category_code = Request.Params["market_category_code"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Params["market_category_name"]))
                {
                    query.market_category_name = Request.Params["market_category_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["market_category_sort"]))
                {
                    query.market_category_sort = Convert.ToInt32(Request.Params["market_category_sort"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["market_category_status"]))
                {
                    query.market_category_status = Convert.ToInt32(Request.Params["market_category_status"]);
                }



                int result = 0;
                if (query.market_category_id == 0)//新增
                {
                    query.kuser = (Session["caller"] as Caller).user_id;
                    query.muser = query.kuser;
                    query.created = DateTime.Now;
                    query.modified = query.created;
                    result = _marketCategoryMgr.InsertMarketCategory(query);

                }
                else//修改 
                {
                    query.muser = (Session["caller"] as Caller).user_id;
                    query.modified = DateTime.Now;
                    result = _marketCategoryMgr.UpdateMarketCategory(query);
                }
                if (result > 0)
                {
                    json = "{success:true}";
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase DeleteMarketCategory()
        {
            string jsonStr = String.Empty;
            _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    bool isBool = true;
                    foreach (string item in Request.Params["rowID"].Split('|'))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            List<MarketCategoryQuery> store = _marketCategoryMgr.GetMarketCategoryList(new MarketCategory { market_category_father_id = Convert.ToInt32(item) });
                            if (store.Count > 0)
                            {
                                isBool = false;
                                break;
                            }
                        }
                    }
                    if (isBool)
                    {
                        if (_marketCategoryMgr.DeleteMarketCategory(Request.Params["rowID"].ToString()) > 0)
                        {
                            jsonStr = "{success:true,msg:0}";
                        }
                        else
                    {
                            jsonStr = "{success:false,msg:0}";
                    }
                    }
                    else
                    {
                        jsonStr = "{success:true,msg:1}";//妄圖刪除父類別，該斬！
                    }

                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase GetMarketCategoryCount()
        {
            string jsonStr = String.Empty;
            _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["code"]))
            {
                try
                {
                    List<MarketCategoryQuery> store = _marketCategoryMgr.GetMarketCategoryList(new MarketCategory { market_category_code = Request.Params["code"].ToString() });
                    jsonStr = "{success:true,count:" + store.Count + "}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 更改狀態

        public JsonResult UpdateActiveMarketCategory()
        {
            try
            {
                int id = Convert.ToInt32(Request.Params["id"] ?? "0");
                int activeValue = Convert.ToInt32(Request.Params["active"] ?? "0");
                _marketCategoryMgr = new MarketCategoryMgr(mySqlConnectionString);
                MarketCategory model = _marketCategoryMgr.GetMarketCategoryList(new MarketCategory { market_category_id = Convert.ToUInt32(id) }).FirstOrDefault();
                model.market_category_status = activeValue;
                model.muser = (Session["caller"] as Caller).user_id;
                model.modified = DateTime.Now;
                if (_marketCategoryMgr.UpdateMarketCategory(model) > 0)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }

        }
        #endregion
        #endregion

        #region 美安訂單匯出 
        public HttpResponseBase ExcelMarketOrder()
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            MarketOrderQuery q = new MarketOrderQuery();
            List<MarketOrderQuery> Mp = new List<MarketOrderQuery>();
            om = new OrderMasterShopComMgr(mySqlConnectionString);
            uint sum = 0;
            double sum2 = 0;
            double sum3 = 0;
            int sum4 = 0;
            string fileName = "";
            uint bonus = 0;
            ulong price = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["starttime"]))
                {//料位開始
                    q.starttime = DateTime.Parse(Request.Params["starttime"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["endtime"]))
                {
                    q.endtime = DateTime.Parse(Request.Params["endtime"]);
                }
                Mp = om.GetMarketOrderExcel(q);
                if (Mp.Count > 0)
                {
                    #region 表頭
                    dt.Columns.Add(new DataColumn("訂購日期"));
                    dt.Columns.Add(new DataColumn("訂單編號"));
                    dt.Columns.Add(new DataColumn("購買人"));
                    dt.Columns.Add(new DataColumn("Market Taiwan RID number"));
                    dt.Columns.Add(new DataColumn("商品編號"));
                    dt.Columns.Add(new DataColumn("購買商品"));
                    dt.Columns.Add(new DataColumn("數量"));
                    dt.Columns.Add(new DataColumn("網路銷售價"));
                    dt.Columns.Add(new DataColumn("折扣金額"));
                    dt.Columns.Add(new DataColumn("總計"));
                    #endregion
                    #region 給字段賦值
                    foreach (var item in Mp)
                    {
                        uint money = 0;
                        DataRow dr = dt.NewRow();
                        dr["訂購日期"] = CommonFunction.GetNetTime(item.Order_Createdate);
                        dr["訂單編號"] = item.Order_Id;
                        dr["購買人"] = item.Delivery_Name;
                        dr["Market Taiwan RID number"] = item.rid;
                        dr["商品編號"] = item.item_id;
                        dr["購買商品"] = item.product_name;
                        if (item.item_mode == 2)
                        {
                            dr["數量"] = item.buy_num * item.parent_num;
                            item.buy_num = item.buy_num * item.parent_num;
                        }
                        else
                        {
                            dr["數量"] = item.buy_num;
                        }
                        dr["網路銷售價"] = "NT$" + item.price.ToString("###,###");
                        price += item.price * item.buy_num ;
                        money= item.deduct_bonus + item.deduct_welfare + uint.Parse(item.deduct_happygo_money.ToString());
                        dr["折扣金額"] = money;
                        bonus += money;
                        if (item.buy_num > 1)
                        {
                            ulong a = (item.buy_num * item.price) - money;
                            dr["總計"] = "NT$" + a.ToString("###,###"); ;
                            sum += uint.Parse(a.ToString());
                        }
                        else
                        {
                            long a = item.price - money;
                            dr["總計"] = "NT$" + a.ToString("###,###"); ;
                            sum += uint.Parse(a.ToString());
                        }
                        dt.Rows.Add(dr);
                    }
                    DataRow dr1 = dt.NewRow();
                    dr1["網路銷售價"] = price.ToString("###,###");
                    dr1["折扣金額"] = bonus;
                    dr1["總計"] = sum.ToString("###,###");
                    dt.Rows.Add(dr1);
                    DataRow dr2 = dt.NewRow();
                    dr2["網路銷售價"] = "右欄填入佣金%";
                    dr2["折扣金額"] = "8.0%";
                    sum2 = sum * 0.08;
                    sum4 += int.Parse(Math.Round(sum2, MidpointRounding.AwayFromZero).ToString());
                    dr2["總計"] = Math.Round(sum2, MidpointRounding.AwayFromZero).ToString("###,###");
                    dt.Rows.Add(dr2);
                    DataRow dr3 = dt.NewRow();
                    dr3["網路銷售價"] = "營業稅";
                    dr3["折扣金額"] = "5%";
                    sum3 = sum4 * 0.05;
                    sum4 += int.Parse(Math.Round(sum3, MidpointRounding.AwayFromZero).ToString());
                    dr3["總計"] = Math.Round(sum3, MidpointRounding.AwayFromZero).ToString("###,###");
                    dt.Rows.Add(dr3);
                    DataRow dr4 = dt.NewRow();
                    dr4["網路銷售價"] = "應付佣金總數";
                    dr4["總計"] = sum4.ToString("###,###");
                    dt.Rows.Add(dr4);
                    #endregion
                }
                fileName = "美安訂單" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                String str = "美安訂單";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dt, str);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return this.Response;
        }
        #endregion
    }

}

