using BLL.gigade.Mgr;
using BLL.gigade.Model;
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
    public class BrandLogoSortController : Controller
    {
        //
        // GET: /BrandLogoSort/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        private BrandLogoSortMgr _BrandLSMgr;
        string brandPath = "/brand_master/a/";
        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseBase GetBLSList()
        {
            string json = string.Empty;
            try
            {
                BrandLogoSort query = new BrandLogoSort();
                List<BrandLogoSort> store = new List<BrandLogoSort>();
                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    uint n = 99999999;

                    if (uint.TryParse(Request.Params["category_id"].ToString().Trim(), out n))
                    {
                        query.category_id = Convert.ToUInt32(Request.Params["category_id"].ToString().Trim());
                    }
                    else
                    {
                        query.category_id = 99999999;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    uint n = 99999999;
                    if (uint.TryParse(Request.Params["brand_id"].ToString().Trim(), out n))
                    {
                        query.brand_id = Convert.ToUInt32(Request.Params["brand_id"].ToString().Trim());
                    }
                    else
                    {
                        query.brand_id = 99999999;
                    }
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                _BrandLSMgr = new BrandLogoSortMgr(mySqlConnectionString);
                store = _BrandLSMgr.GetBLSList(query, out totalCount);
                foreach (var item in store)
                {
                    if (!string.IsNullOrEmpty(item.brand_logo))
                    {
                        string folder1 = item.brand_logo.Substring(0, 2) + "/"; //圖片名前兩碼
                        string folder2 = item.brand_logo.Substring(2, 2) + "/"; //圖片名第三四碼
                        item.brand_logo = imgServerPath + brandPath + folder1 + folder2 + item.brand_logo;
                    }
                    else
                    {
                        item.brand_logo = defaultImg;
                    }

                }
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

        public HttpResponseBase SaveBLS()
        {
            string json = string.Empty;
            BrandLogoSort query = new BrandLogoSort();
            try
            {
                _BrandLSMgr = new BrandLogoSortMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["blo_id"]))
                {
                    query.blo_id = Convert.ToInt32(Request.Params["blo_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.brand_id = Convert.ToUInt32(Request.Params["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["old_brand_id"]))
                {
                    query.old_brand_id = Convert.ToInt32(Request.Params["old_brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["blo_sort"]))
                {
                    query.blo_sort = Convert.ToInt32(Request.Params["blo_sort"]);
                }
                query.blo_kuser = Convert.ToUInt32((Session["caller"] as Caller).user_id);
                query.blo_muser = query.blo_kuser;
                json = _BrandLSMgr.SaveBLS(query);
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

        public HttpResponseBase DeleteBLS()
        {
            string json = string.Empty;
            BrandLogoSort query = new BrandLogoSort();
            List<BrandLogoSort> list = new List<BrandLogoSort>();
            try
            {
                _BrandLSMgr = new BrandLogoSortMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("∑") != -1)
                    {
                        foreach (string id in rowIDs.Split('∑'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                query = new BrandLogoSort();
                                query.blo_id = Convert.ToInt32(id);
                                list.Add(query);
                            }
                        }
                    }
                }
                json = _BrandLSMgr.DeleteBLS(list);
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

        public HttpResponseBase GetCategoryStore()
        {
            string json = string.Empty;
            try
            {
                _BrandLSMgr = new BrandLogoSortMgr(mySqlConnectionString);
                DataTable _dt = _BrandLSMgr.CategoryStore();
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";
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

        public HttpResponseBase GetBrandStore()
        {
            string json = string.Empty;
            try
            {
                BrandLogoSort query = new BrandLogoSort();

                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                    _BrandLSMgr = new BrandLogoSortMgr(mySqlConnectionString);
                    DataTable _dt = _BrandLSMgr.BrandStore(query);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";
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

        public HttpResponseBase MaxSort()
        {
            string json = string.Empty;
            try
            {
                BrandLogoSort query = new BrandLogoSort();

                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                    _BrandLSMgr = new BrandLogoSortMgr(mySqlConnectionString);
                    int sort = _BrandLSMgr.MaxSort(query);
                    json = "{success:true,data:" + sort + "}";
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

    }
}
