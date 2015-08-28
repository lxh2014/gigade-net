using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class PromotionAmountDiscountsController : Controller
    {
         
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPromotionsAmountDiscountImplMgr _promotionsAmountDiscount;
        //
        // GET: /PromotionAmountDiscounts/

        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseBase GetList()
        {
            string json = string.Empty;
            int totalCount=0;
            try
            {
                PromotionsAmountDiscountCustom query = new PromotionsAmountDiscountCustom();
                List<PromotionsAmountDiscountCustom> store = new List<PromotionsAmountDiscountCustom>();
               query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
               query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

               _promotionsAmountDiscount = new PromotionsAmountDiscountMgr(mySqlConnectionString);
               if (!string.IsNullOrEmpty(Request.Params["searchStore"]))
               {
                   query.searchStore = Convert.ToInt32(Request.Params["searchStore"]);
               }
               store=_promotionsAmountDiscount.GetList(query,out totalCount);
               //foreach (var item in store)
               //{
               //    if (item.group_name == "")
               //    {
               //        item.group_name = "不分";
               //    }
               //}
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase Save()
        {
            string json = string.Empty;
            try
            {
                PromotionsAmountDiscountCustom query = new PromotionsAmountDiscountCustom();
                _promotionsAmountDiscount = new PromotionsAmountDiscountMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    query.name = Request.Params["name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id =Convert.ToInt32( Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["discount"]))
                {
                    query.discount = Convert.ToInt32(Request.Params["discount"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["amount"]))
                {
                    query.amount = Convert.ToInt32(Request.Params["amount"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["quantity"]))
                {
                    query.quantity = Convert.ToInt32(Request.Params["quantity"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["site"]))
                {
                    query.site = (Request.Params["site"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start"]))
                {
                    query.start =Convert.ToDateTime(Request.Params["start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end"]))
                {
                    query.end =Convert.ToDateTime( Request.Params["end"]);
                }
                if (string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.status = 0;
                    query.created = DateTime.Now;
                    query.modified = query.created;
                    query.kuser = (Session["caller"] as Caller).user_id.ToString();
                    query.muser = query.kuser;
                    //新增
                    if (_promotionsAmountDiscount.Save(query) > 0)
                    {

                        json = "{success:'true'}";
                    }
                    else
                    {
                        json = "{success:'false'}";
                    }
                }
                else
                {
                    //編輯
                    query.id = Convert.ToInt32(Request.Params["id"]);
                    query.modified = DateTime.Now;
                    query.muser = (Session["caller"] as Caller).user_id.ToString();
                    if (_promotionsAmountDiscount.Save(query) > 0)
                    {
                        json = "{success:'true'}";
                    }
                    else
                    {
                        json = "{success:'false'}";
                    }
                }

            }
            catch(Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:'false'}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase Delete()
        {
            string json = string.Empty;
            PromotionsAmountDiscountCustom query = null;
            _promotionsAmountDiscount = new PromotionsAmountDiscountMgr(mySqlConnectionString);
            List<PromotionsAmountDiscountCustom> list = new List<PromotionsAmountDiscountCustom>();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("|") != -1)
                    {
                        foreach (string id in rowIDs.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                query = new PromotionsAmountDiscountCustom();
                                query.id = Convert.ToInt32(id);
                                list.Add(query);
                            }
                        }
                    }
                    if (_promotionsAmountDiscount.Delete(list))
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{failure:true}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{failure:true}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public JsonResult UpPADActive()
        {
            string json = string.Empty;
            try
            {
                PromotionsAmountDiscountCustom query = new PromotionsAmountDiscountCustom();
                string currentUser = (Session["caller"] as Caller).user_id.ToString();
                if (!string.IsNullOrEmpty(Request.Params["muser"]))
                {
                    query.muser = (Request.Params["muser"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    if (Convert.ToInt32(Request.Params["active"]) == 1)
                    {
                        query.active = true;
                    }
                    else
                    {
                        query.active = false;
                    }
                }
                if (currentUser != query.muser || query.active!=true)
                {
                    if (!string.IsNullOrEmpty(Request.Params["id"]))
                    {
                        query.id = Convert.ToInt32(Request.Params["id"]);
                    }
                    query.modified = DateTime.Now;
                    query.muser = currentUser;
                    _promotionsAmountDiscount = new PromotionsAmountDiscountMgr(mySqlConnectionString);
                    if (_promotionsAmountDiscount.UpdatePromoAmountDisActive(query))
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
                }
                else
                {
                    return Json(new { success="msg"});
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

    }
}
