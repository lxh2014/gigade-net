using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class TrialProdCateController : Controller
    {
        //
        // GET: /TrialProdCate/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ITrialProdCateImplMgr _trialProdMgr;
        private IProductCategoryImplMgr prodCateMgr;
        private IProductImplMgr _prodMgr;
        private IProductCategorySetImplMgr _pcsMgr;
        private IPromotionsAmountGiftImplMgr _giftMgr;
        private IPromotionsAmountTrialImplMgr _trialMgr;

        public ActionResult Index()
        {
            return View();
        }

        #region 獲取商品數據 + HttpResponseBase GetTrialProdCateList()
        /// <summary>
        /// 獲取商品數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetTrialProdCateList()
        {
            List<TrialProdCateQuery> store = new List<TrialProdCateQuery>();
            string json = string.Empty;
            try
            {
                TrialProdCateQuery query = new TrialProdCateQuery();
                #region 獲取query對象數據
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                #endregion
                _trialProdMgr = new TrialProdCateMgr(mySqlConnectionString);
                _prodMgr = new ProductMgr(mySqlConnectionString);
                prodCateMgr = new ProductCategoryMgr(mySqlConnectionString);//實例化對象mgr
                _giftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                _trialMgr = new PromotionsAmountTrialMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _trialProdMgr.Query(query, out totalCount);

                foreach (var item in store)
                {
                    item.product_name = _prodMgr.QueryClassify(Convert.ToUInt32(item.product_id)).Product_Name;
                    item.category_name = prodCateMgr.QueryAll(new ProductCategory { category_id = item.category_id }).FirstOrDefault().category_name;
                    if (item.event_id.StartsWith("T1") || item.event_id.StartsWith("T2"))
                    {
                        int id = Convert.ToInt16(item.event_id.Substring(2).ToString());
                        item.event_name = _trialMgr.GetModel(id).name;
                    }
                    if (item.event_id.StartsWith("G3"))
                    {
                        int id = Convert.ToInt16(item.event_id.Substring(2).ToString());
                        item.event_name = _giftMgr.GetModel(id).name;
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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



        #region 獲取商品數據 + HttpResponseBase UpdateTrialProdCate()
        /// <summary>
        /// 獲取商品數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateTrialProdCate()
        {
            List<TrialProdCateQuery> storeLi = new List<TrialProdCateQuery>();
            string json = string.Empty;
            try
            {
                _pcsMgr = new ProductCategorySetMgr(mySqlConnectionString);
                _trialProdMgr = new TrialProdCateMgr(mySqlConnectionString);
                //獲取新館類別

                prodCateMgr = new ProductCategoryMgr(mySqlConnectionString);//實例化對象mgr
                List<ProductCategory> category = prodCateMgr.QueryAll(new ProductCategory { category_display = 0 });//獲取所有的類別 顯示的隱藏的

                ProductCategory useCate = prodCateMgr.QueryAll(new ProductCategory { category_father_id = 754, category_name = "用品館" }).FirstOrDefault();
                string cateUseStr = string.Empty;//設定對象獲取用品館所有子類別
                GetAllCategory_id(category, useCate.category_id, ref cateUseStr);//設定對象獲取用品館所有子類別 顯示的

                ProductCategory eatCate = prodCateMgr.QueryAll(new ProductCategory { category_father_id = 754, category_name = "食品館" }).FirstOrDefault();
                string cateEatStr = string.Empty;//設定對象獲取食品館所有子類別
                GetAllCategory_id(category, eatCate.category_id, ref cateEatStr);//設定對象獲取食品館所有子類別 顯示的
                _prodMgr = new ProductMgr(mySqlConnectionString);

                //找到商品
                List<TrialProdCateQuery> store = _trialProdMgr.UadateTrialProd();

                Product prodModel = new Product();

                //找到商品新館所有類別
                foreach (var item in store)
                {

                    if (item.product_id != 0)
                    {
                        prodModel = _prodMgr.QueryClassify(Convert.ToUInt32(item.product_id));

                        DataTable dt = new DataTable();
                        if (prodModel != null)
                        {
                            if (prodModel.Prod_Classify == 10)
                            {
                                item.type = 1;
                                dt = _pcsMgr.GetCateByProds(item.product_id.ToString(), cateEatStr);

                                //找到大類類別
                                bool isTrue = false;
                                if (dt.Rows.Count != 0)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        ProductCategory pcModel = new ProductCategory();
                                        GetFatherCategory_id(category, ref isTrue, eatCate.category_id, Convert.ToUInt32(row["category_id"]), ref  pcModel);
                                        if (isTrue && pcModel != null)
                                        {
                                            if (item.category_id == 0)
                                            {
                                                item.category_id = pcModel.category_id;
                                                storeLi.Add(item);
                                            }
                                            else
                                            {
                                                if (pcModel.category_id != item.category_id)
                                                {
                                                    TrialProdCateQuery queryItem = new TrialProdCateQuery();
                                                    queryItem.product_id = item.product_id;
                                                    queryItem.event_id = item.event_id;
                                                    queryItem.type = item.type;
                                                    queryItem.start_date = item.start_date;
                                                    queryItem.end_date = item.end_date;
                                                    queryItem.category_id = pcModel.category_id;
                                                    List<TrialProdCateQuery> CFLi = storeLi.FindAll(p => p.category_id == queryItem.category_id && p.product_id == queryItem.product_id && p.type == queryItem.type && p.event_id == queryItem.event_id).ToList();
                                                    if (CFLi.Count == 0)
                                                    {
                                                        storeLi.Add(queryItem);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (prodModel.Prod_Classify == 20)
                            {
                                item.type = 2;
                                dt = _pcsMgr.GetCateByProds(item.product_id.ToString(), cateUseStr);
                                //找到大類類別
                                bool isTrue = false;
                                if (dt.Rows.Count != 0)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        ProductCategory pcModel = new ProductCategory();
                                        GetFatherCategory_id(category, ref isTrue, useCate.category_id, Convert.ToUInt32(row["category_id"]), ref  pcModel);
                                        if (isTrue && pcModel != null)
                                        {
                                            if (item.category_id == 0)
                                            {
                                                item.category_id = pcModel.category_id;
                                                storeLi.Add(item);
                                            }
                                            else
                                            {
                                                if (pcModel.category_id != item.category_id)
                                                {
                                                    TrialProdCateQuery queryItem = new TrialProdCateQuery();
                                                    queryItem.product_id = item.product_id;
                                                    queryItem.event_id = item.event_id;
                                                    queryItem.type = item.type;
                                                    queryItem.start_date = item.start_date;
                                                    queryItem.end_date = item.end_date;
                                                    queryItem.category_id = pcModel.category_id;
                                                    List<TrialProdCateQuery> CFLi = storeLi.FindAll(p => p.category_id == queryItem.category_id && p.product_id == queryItem.product_id && p.type == queryItem.type && p.event_id == queryItem.event_id).ToList();
                                                    if (CFLi.Count == 0)
                                                    {
                                                        storeLi.Add(queryItem);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }


                //插入數據到主表trial_prod_cate
                if (_trialProdMgr.InsertTrialProd(storeLi))
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
        #region 遞歸查詢子cateID
        /// <summary>
        /// 遞歸查詢子ID 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetAllCategory_id(List<ProductCategory> category, uint rowid, ref string id)
        {
            List<ProductCategory> query = category.FindAll(p => p.category_father_id == rowid).ToList();
            if (query.Count != 0)
            {

                foreach (var que in query)
                {
                    id += "," + que.category_id.ToString();
                    GetAllCategory_id(category, que.category_id, ref id);
                }
            }
            if (id.IndexOf(rowid.ToString()) < 0)
            {
                id += "," + rowid.ToString();
            }
            if (id.Substring(0, 1) == ",")
            {
                id = id.Remove(0, 1);
            }
        }
        #endregion


        #region 遞歸驗證該category_id的父節點是否等於fatherId
        /// <summary>
        /// 遞歸驗證求取食品館用品館類別
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetFatherCategory_id(List<ProductCategory> category, ref bool isTrue, uint fatherId, uint cate_id, ref  ProductCategory pcModel)
        {
            ProductCategory query = category.FindAll(p => p.category_id == cate_id).ToList().FirstOrDefault();//找到該cate_id
            if (query != null && query.category_father_id != 0)
            {
                if (query.category_father_id == fatherId)
                {
                    isTrue = true;
                    pcModel = query;
                    return;
                }
                else
                {
                    isTrue = false;
                    GetFatherCategory_id(category, ref isTrue, fatherId, query.category_father_id, ref pcModel);
                }
            }
            else
            {
                isTrue = false;
                return;
            }

        }
        #endregion
    }
}
