#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsMaintainController.cs 
 * 摘   要： 
 *      促銷項目維護
 * 当前版本：v1.1 
 * 作   者： hongfei0416j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：hongfei0416j
 *      v1.1修改内容：合并代碼，添加注釋
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using BLL.gigade.Model;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Custom;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using Newtonsoft.Json;
using System.Configuration;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;
using Newtonsoft.Json.Converters;



namespace Admin.gigade.Controllers
{
    public class PromotionsMaintainController : Controller
    {

        // GET: /PromotionsMaintain/
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IProductCategoryImplMgr _procateMgr;
        // private IProductCategorySetImplMgr _categorySetMgr;
        //private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        //  private int COMBO_TYPE = 1;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EventIndex()
        {
            return View();
        }
        #region 得到所有前台分類 +HttpResponseBase GetCatagory(string id = "true")
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetCatagory(string id = "true")
        {
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            List<ProductCategorySet> resultList = new List<ProductCategorySet>();

            _procateMgr = new ProductCategoryMgr(connectionString);
            ParameterMgr parameterMgr = new ParameterMgr(connectionString);

            string resultStr = "";
            try
            {
                uint rootId = 0;
                categoryList = _procateMgr.QueryAll(new ProductCategory { });
                List<Parametersrc> fatherIdResult = parameterMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", Used = 1, ParameterCode = "CXXM" });
                rootId = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                cateList = getCate(categoryList, rootId);
                GetCategoryList(categoryList, ref cateList, resultList);
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
        #endregion

        #region 遞歸得到分類節點 +void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, List<ProductCategorySet> resultList)
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

                if (childList.Count() > 0)
                {
                    GetCategoryList(categoryList, ref childList, resultList);
                }
            }

        }
        #endregion

        #region 獲取類別 List<ProductCategoryCustom> getCate(List<ProductCategory> categoryList, uint fatherId)
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
                                text = m.text,

                            });

            return cateList;
        }
        #endregion

        #region 獲取類別類型 +string getCateType()
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

        #region 獲取參數 +string QueryParameter(Parametersrc p)
        public string QueryParameter(Parametersrc p)
        {
            string json = string.Empty;
            try
            {
                ParameterMgr paraMgr = new ParameterMgr(connectionString);
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

        #region 獲得品牌 +string QueryBrand()
        //[HttpPost]
        [CustomHandleError]
        public string QueryBrand()
        {
            string json = string.Empty;
            try
            {
                VendorBrandMgr vbMgr = new VendorBrandMgr(connectionString);
                json = vbMgr.QueryBrand(new VendorBrand { Brand_Status = 1 });
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

        #region 獲得品牌 +string QueryClassBrand()
        //[HttpPost]
        [CustomHandleError]
        public string QueryClassBrand()
        {
            string json = string.Empty;
            try
            {
                uint cid = 0;
                VendorBrand vb = new VendorBrand();
                VendorBrandMgr vbMgr = new VendorBrandMgr(connectionString);
                //Edit By Castle
                //Date:2014/07/01
                //Discription:Brand_status是给前台用的后台不做限制
                vb.Brand_Status = 0;
                //vb.Brand_Status = 1;

                if (!string.IsNullOrEmpty(Request.Form["topValue"]))
                {
                    cid = Convert.ToUInt32(Request.Form["topValue"]);
                }

                json = vbMgr.QueryClassBrand(vb, cid);

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

        #region 商品列表查詢 +HttpResponseBase QueryProList()

        //[HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProList()
        {
            string json = string.Empty;
            string _classid = String.Empty;
            PromotionsMaintainDao pmDao = new PromotionsMaintainDao(connectionString);
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                #region 查询条件填充
                query.IsPage = true;
                query.Start = int.Parse(Request.Params["start"] ?? "0");
                query.Limit = int.Parse(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["ProCatid"]))//活動的category_id
                {
                    query.cate_id = Request.Form["ProCatid"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["key"]))//變動1支持商品編號的批次查詢
                {
                    query.product_name = Request.Params["key"].Replace('，', ',').Replace('|', ',').Replace(' ', ','); //在這裡product_id用,分割拼接的字符串用product_name 存放
                }

                if (!string.IsNullOrEmpty(Request.Params["site_id"]))//支持多站台查詢
                {
                    query.site_ids = Request.Params["site_id"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Params["comboFrontCage_hide"]))//支持類別查詢商品
                {
                    query.category_id = Convert.ToUInt32(Request.Params["comboFrontCage_hide"].ToString());
                }

                #endregion
                query.combination = 1;//只顯示單一商品
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))//支持品牌查詢
                {
                    query.brand_id = uint.Parse(Request.Params["brand_id"]);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))//支持館別查詢
                    {
                        VendorBrandSetMgr vbsMgr = new VendorBrandSetMgr(connectionString);
                        VendorBrandSet vbs = new VendorBrandSet();
                        vbs.class_id = Convert.ToUInt32(Request.Params["class_id"]);
                        List<VendorBrandSet> vbsList = vbsMgr.Query(vbs);
                        foreach (VendorBrandSet item in vbsList)
                        {
                            query.brand_ids += item.brand_id + ",";
                        }
                        query.brand_ids = query.brand_ids.TrimEnd(',');
                    }
                }
                List<QueryandVerifyCustom> pros = pmDao.GetProList(query, out totalCount);
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

        #region 保存商品到活動中去 +HttpResponseBase SaveProductCategorySet()
        public HttpResponseBase SaveProductCategorySet()
        {
            string[] bids = Request.Params["brandids"].Split('|');
            string categoryid = Request.Params["categoryid"];

            string[] pids = Request.Params["productids"].Split('|');

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
                        _categorySetMgr.Insert(pcs);
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
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取活動中的商品 +HttpResponseBase GetProductByCategorySet()
        public HttpResponseBase GetProductByCategorySet()
        {
            string resultStr = "{success:false}";
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                if (!string.IsNullOrEmpty(Request.Params["producateid"]))//活動類別編號
                {
                    query.cate_id = Request.Params["producateid"];
                }
                if (!string.IsNullOrEmpty(Request.Params["key"]))//變動3支持商品編號批次搜索
                {
                    query.product_name = Request.Params["key"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');
                }
                //站台改成多個站台的site_id字符串 edit by shuangshuang0420j 20140925 13:40
                if (!string.IsNullOrEmpty(Request.Params["site_id"]))
                {
                    query.site_ids = Request.Params["site_id"];
                }
                int totalCount = 0;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                PromotionsMaintainDao pmDao = new PromotionsMaintainDao(connectionString);
                List<QueryandVerifyCustom> tempPros = pmDao.GetEventList(query, out totalCount);
                resultStr = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(tempPros) + "}";
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

        #region 刪除全館數據 +HttpResponseBase DeleteAllClassProductByModel()
        /// <summary>
        /// 刪除全館數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteAllClassProductByModel()
        {
            ProductCategorySetMgr _categorySetMgr = new ProductCategorySetMgr(connectionString);
            string categoryid = Request.Params["categoryid"];
            string resultStr = "{success:false}";
            ProductCategorySet pcs = new ProductCategorySet();
            pcs.Brand_Id = 0;
            pcs.Category_Id = Convert.ToUInt32(categoryid);
            pcs.Product_Id = 999999;
            try
            {
                _categorySetMgr.DeleteProductByModel(pcs);
                resultStr = "{success:true}";
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
            string[] bids = Request.Params["brandids"].Split('|');
            string categoryid = Request.Params["categoryid"];
            string[] pids = Request.Params["productids"].Split('|');
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
                        try
                        {
                            _categorySetMgr.DeleteProductByModel(pcs);
                        }
                        catch (Exception)
                        {


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
            }


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取館別 +HttpResponseBase GetShopClass()
        public HttpResponseBase GetShopClass()
        {
            List<ShopClass> stores = new List<ShopClass>();
            string json = string.Empty;
            try
            {

                IShopClassImplMgr _shopClassMgr = new ShopClassMgr(connectionString);

                stores = _shopClassMgr.QueryStore();
                ShopClass Dmodel = new ShopClass();
                Dmodel.class_name = "不分";
                stores.Insert(0, Dmodel);
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據


            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,data:[]}";
            }


            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        #region 根據ProductItemid獲取ProductId + uint GetProductIdByItemId(uint id)
        public uint GetProductIdByItemId(uint id)
        {
            uint ret = 0;
            try
            {
                IProductItemImplMgr _productItemMgr = new ProductItemMgr(connectionString);
                List<ProductItem> proItemList = _productItemMgr.Query(new ProductItem { Item_Id = id });
                if (proItemList.Count > 0)
                {
                    ret = proItemList[0].Product_Id;
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return ret;
        }
        #endregion


        #region 活動商品列表
        #region 列表頁
        public HttpResponseBase GetList()
        {
            string json = string.Empty;
            List<ProdPromoQuery> store = new List<ProdPromoQuery>();
            ProdPromo query = new ProdPromo();
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                {
                    query.SearchContent = Request.Params["searchcontent"].ToString();
                }
                _procateMgr = new ProductCategoryMgr(connectionString);
                store = _procateMgr.GetList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 修改活動状态
        public JsonResult UpStatus()
        {
            string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
            string muser = string.Empty;
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            if (!string.IsNullOrEmpty(Request.Params["muser"]))
            {
                muser = (Request.Params["muser"]);
            }
            if (currentUser == muser && activeValue == 1)
            {
                return Json(new { success = "stop" });
            }
            string musers = string.Empty;
            string rids=string.Empty;
            string rid = Request.Params["id"];
            if (!string.IsNullOrEmpty(Request.Params["musers"]))
            {
                musers = Request.Params["musers"];
                string[] muser1 = musers.TrimEnd(',').Split(',');
                List<int> num = new List<int>();
                for (int i = 0; i < muser1.Length; i++)
                {
                    if (muser1[i] != currentUser || activeValue==0)
                    {
                        num.Add(i);
                    }
                }
                if(num.Count==0)
                {
                    return Json(new { success = "stop" });
                }
                string[] rids1 = rid.TrimEnd(',').Split(',');
                for (int i = 0; i < num.Count; i++)
                {
                    rids += rids1[num[i]] + ",";
                }
            }
            _procateMgr = new ProductCategoryMgr(connectionString);
            ProdPromoQuery store = new ProdPromoQuery();
            store.rids = rids.TrimEnd(',') == "" ? rid.TrimEnd(',') : rids.TrimEnd(',');
            store.muser = currentUser;
            store.mdate = DateTime.Now;
            store.status = activeValue;
            if (_procateMgr.UpStatus(store) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion
        #region 批次更改活動狀態
        public HttpResponseBase UpdateUrl()
        {
            string json = string.Empty;
            ProdPromo query = new ProdPromo();
            _procateMgr = new ProductCategoryMgr(connectionString);
            if (!string.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.Params["pageUrl"]))
                    {
                        query.page_url = Request.Params["pageUrl"];
                    }
                    foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
                    {

                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.rid = Convert.ToInt32(rid);
                            if (_procateMgr.UpdateUrl(query) >= 0)
                            {
                                json = "{success:true}";
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
