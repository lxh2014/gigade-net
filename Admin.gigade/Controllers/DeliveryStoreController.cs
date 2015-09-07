using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Configuration;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Newtonsoft.Json;
using BLL.gigade.Common;
using BLL.gigade.Model.Custom;
using System.IO;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class DeliveryStoreController : Controller
    {
        //
        // GET: /DeliveryStore/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string excelPath = ConfigurationManager.AppSettings["ImportDeliverySetExcel"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        private IDeliveryStoreImplMgr deliveryStoreMgr;
        private ISiteConfigImplMgr siteConfigMgr;
        private IShippingCarriorImplMgr _shippingCarriorMgr;
        private IProductDeliverySetImplMgr _prodDeliSetMgr;
        private DeliveryStorePlaceMgr DSPMgr;
        private IZipImplMgr IZImplMgr;
        /// <summary>
        /// 物流設定
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult TransportSet()
        {
            return View();
        }

        /// <summary>
        /// 批量物流設定
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult QuantityTransportSet()
        {
            return View();
        }


        //超級店家
        [CustomHandleError]
        public ActionResult Index()
        {
            return View();
        }

        #region 查詢BIG

        [HttpPost]
        [CustomHandleError]
        public string QueryBig()
        {
            IZipImplMgr zMgr = new ZipMgr(connectionString);
            string json = string.Empty;
            try
            {
                json = zMgr.QueryBig(Request.Form["topValue"] ?? "");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            return json;
        }
        #endregion

        #region 查詢物流業者

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryDelivery()
        {
            string json = string.Empty;
            try
            {
                IParametersrcImplMgr paraMgr = new ParameterMgr(connectionString);
                json = "{success:true,items:" + JsonConvert.SerializeObject(paraMgr.QueryUsed(new Parametersrc { ParameterType = "Deliver_Store", Used = 1 }).Where(m => m.Sort == 1)) + "}";
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

        #region 查詢店家

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryDeliveryStore()
        {
            List<DeliveryStoreQuery> stores = new List<DeliveryStoreQuery>();
            string json = string.Empty;
            try
            {
                DeliveryStore query = new DeliveryStore();
                query.Start = Convert.ToInt32(Request.Form["start"] ?? "0");
                query.status = 0;
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["Delivery"]))
                {
                    query.delivery_store_id = Convert.ToInt32(Request.Form["Delivery"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["Status"]))
                {
                    query.status = Convert.ToInt32(Request.Form["Status"]);
                }
                deliveryStoreMgr = new DeliveryStoreMgr(connectionString);
                int totalCount = 0;
                stores = deliveryStoreMgr.Query(query, out totalCount);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";
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
        #endregion

        #region 保存店家

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveDeliveryStore()
        {
            DeliveryStore store = new DeliveryStore();
            string json = "{success:false}";
            try
            {
                #region 填充實體

                if (!string.IsNullOrEmpty(Request.Form["delivery_store_id"]))
                {
                    store.delivery_store_id = Convert.ToInt32(Request.Form["delivery_store_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["BigText"]))
                {
                    store.big = Request.Form["BigText"];
                }
                if (!string.IsNullOrEmpty(Request.Form["bigcode"]))
                {
                    store.bigcode = Request.Form["bigcode"];
                }
                if (!string.IsNullOrEmpty(Request.Form["MiddleText"]))
                {
                    store.middle = Request.Form["MiddleText"];
                }
                if (!string.IsNullOrEmpty(Request.Form["middlecode"]))
                {
                    store.middlecode = Request.Form["middlecode"];
                }
                if (!string.IsNullOrEmpty(Request.Form["smallcode"]))
                {
                    string[] small = Request.Form["smallcode"].Split('/');
                    store.small = small[1];
                    store.smallcode = small[0];
                }
                if (!string.IsNullOrEmpty(Request.Form["store_id"]))
                {
                    store.store_id = Request.Form["store_id"];
                }
                if (!string.IsNullOrEmpty(Request.Form["store_name"]))
                {
                    store.store_name = Request.Form["store_name"];
                }
                if (!string.IsNullOrEmpty(Request.Form["address"]))
                {
                    store.address = Request.Form["address"];
                }
                if (!string.IsNullOrEmpty(Request.Form["phone"]))
                {
                    store.phone = Request.Form["phone"];
                }
                if (!string.IsNullOrEmpty(Request.Form["status"]))
                {
                    store.status = Convert.ToInt32(Request.Form["status"]);
                }
                #endregion

                deliveryStoreMgr = new DeliveryStoreMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Form["rowid"]))
                {
                    store.rowid = Convert.ToInt32(Request.Form["rowid"]);
                    if (deliveryStoreMgr.Update(store) > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    if (deliveryStoreMgr.Save(store) > 0)
                    {
                        json = "{success:true}";
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

        #region 刪除店家

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase DeleteDeliveryStore()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("|") != -1)
                    {
                        deliveryStoreMgr = new DeliveryStoreMgr(connectionString);
                        foreach (string id in rowIDs.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                deliveryStoreMgr.Delete(Convert.ToInt32(id));
                            }
                        }
                    }
                }
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

        #region 物流設定
        [CustomHandleError]
        public string QueryShippingCarriorAll()
        {
            string json = string.Empty;
            _shippingCarriorMgr = new ShippingCarriorMgr(connectionString);
            int totalCount = 0;
            List<ShippingCarriorCustom> shippingcariors = _shippingCarriorMgr.QueryAll(new ShippingCarriorCustom(), out totalCount);
            json = "{success:true,totalCount:" + totalCount + ",items:" + JsonConvert.SerializeObject(shippingcariors) + "}";
            return json;
        }
        /// <summary>
        /// 保存物流設定
        /// </summary>
        /// <returns></returns>
        /*[HttpPost]*/
        public HttpResponseBase SaveShippingCarrior(ShippingCarrior sc)
        {
            string json = string.Empty;
            try
            {
                _shippingCarriorMgr = new ShippingCarriorMgr(connectionString);
                int rowCount = 0;
                if (sc.Rid > 0)//修改
                {
                    rowCount = _shippingCarriorMgr.Update(sc);
                }
                else//新增
                {
                    rowCount = _shippingCarriorMgr.Save(sc);
                }
                if (rowCount > 0) json = "{success:true}";
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

        [HttpPost]
        public HttpResponseBase DeleteShippingCarrior(string rids)
        {
            string json = string.Empty;
            try
            {
                _shippingCarriorMgr = new ShippingCarriorMgr(connectionString);
                _shippingCarriorMgr.Delete(rids);
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

        [HttpPost]
        public ActionResult QuantityTransportSet(int flag = 1)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["transport"]) && Request.Files["file"] != null && Request.Files["file"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["file"];
                    string filePath = "";
                    if (UpLoadFile(excelFile, excelPath, ref filePath))
                    {
                        _prodDeliSetMgr = new ProductDeliverySetMgr(connectionString);
                        string[] transport = Request.Form["transport"].Split('|');
                        var prodDeliSet = new ProductDeliverySet { Freight_big_area = int.Parse(transport[0]), Freight_type = int.Parse(transport[1]) };

                        switch (flag)
                        {
                            case 1://上傳
                                string resultPath = "";
                                var results = _prodDeliSetMgr.Save(filePath, out resultPath, prodDeliSet);
                                string resultStr = JsonConvert.SerializeObject(results);
                                return Content("{success:true,results:" + resultStr + ",resultPath:\"" + Path.Combine(excelPath, Path.GetFileName(resultPath)) + "\"}");
                            case 2://刪除
                                if (_prodDeliSetMgr.Delete(filePath, prodDeliSet))
                                    return Content("{success:true}");
                                else
                                {
                                    return Content("{success:false,msg:'刪除失敗~'}");
                                }
                        }
                    }
                }
                else
                {
                    return Content("{success:false,msg:'上傳文檔格式錯誤~'}");
                }
                return Content("{success:false}");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Content("{success:false,msg:'程式出錯，請聯繫開發人員~'}");
            }
        }

        public bool UpLoadFile(HttpPostedFileBase file, string excelPath, ref string filePath)
        {
            Resource.CoreMessage = new CoreResource("Product");
            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            FileManagement fileManagement = new FileManagement();
            string newExcelName = fileManagement.NewFileName(file.FileName);
            string oldExcelName = file.FileName.Split('\\').LastOrDefault();

            #region Excel文件限制
            SiteConfig excellConfig = siteConfigMgr.GetConfigByName("Excel_Extension_Filter");
            string excelEx = string.IsNullOrEmpty(excellConfig.Value) ? excellConfig.DefaultValue : excellConfig.Value;

            SiteConfig minConfig = siteConfigMgr.GetConfigByName("Excel_Length_Min");
            int excelMin = Convert.ToInt32(DataCheck.IsNumeric(minConfig.Value) ? minConfig.Value : minConfig.DefaultValue);

            SiteConfig maxConfig = siteConfigMgr.GetConfigByName("Excel_Length_Max");
            int excelMax = Convert.ToInt32(DataCheck.IsNumeric(maxConfig.Value) ? maxConfig.Value : maxConfig.DefaultValue);
            filePath = Server.MapPath(excelPath) + "/" + newExcelName;
            #endregion
            return fileManagement.UpLoadFile(file, filePath, excelEx, excelMax, 0);//保存導入訂單excel文件
        }

        /// <summary>
        /// 下載物流配送模式
        /// </summary>
        /// <param name="delivSet"></param>
        /// <returns></returns>
        public ActionResult DownloadProductDeliverySet(ProductDeliverySet delivSet)
        {
            try
            {
                _prodDeliSetMgr = new ProductDeliverySetMgr(connectionString);
                MemoryStream ms = _prodDeliSetMgr.ExportProdDeliverySet(delivSet);
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
        /// <summary>
        /// 範本下載
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverySetTemplate()
        {
            try
            {
                List<ProdDeliverySetImport> list = new List<ProdDeliverySetImport>()
                {
                    new ProdDeliverySetImport
                    { 
                        ProductId = "商品編號",
                        BrandName = "品牌", 
                        ProductName = "商品名稱"
                    }
                };
                Dictionary<string, string> dics = new Dictionary<string, string>();
                dics.Add("ProductId", "ProductId");
                dics.Add("BrandName", "BrandName");
                dics.Add("ProductName", "ProductName");
                MemoryStream ms = gigadeExcel.Comment.ExcelHelperXhf.ExportExcel(list, dics);
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

        #region 獲取物流公司信息
        [CustomHandleError]
        [HttpPost]
        //add by wangwei0216w
        public JsonResult GetDelieveryStoreInfo()
        {
            IDeliveryStoreImplMgr dls = new DeliveryStoreMgr(connectionString);
            JsonResult result = null;
            int storeId = 0;
            int.TryParse(Request.Params["storeId"] ?? "0", out storeId);

            DeliveryStore store = new DeliveryStore();
            store.delivery_store_id = storeId;
            store.IsPage = false;
            int count = 0;
            try
            {
                result = Json(dls.Query(store, out count));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }

        //獲取配送區域信息
        public string GetProductDeliverySet()
        {
            IParametersrcImplMgr parame = new ParameterMgr(connectionString);

            string parameFun = "freight_big_area";
            try
            {
                return parame.Query(parameFun);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return "";
        }

        //獲取相應物流配送模式
        public JsonResult GetProductDeliverySetById(string rangeid)
        {
            IParametersrcImplMgr parame = new ParameterMgr(connectionString);
            Parametersrc p = new Parametersrc();
            JsonResult result = null;
            try
            {
                p.ParameterType = "freight_type";
                p.TopValue = rangeid;
                result = Json(parame.QueryForTopValue(p));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
        #endregion

        #region 物流營業所列表 add by yafeng0715j 20150825PM
        public ActionResult DeliveryStorePlace()
        {
            return View();
        }

        public HttpResponseBase GetDeliveryStorePlaceList()
        {
            List<DeliveryStorePlaceQuery> stores = new List<DeliveryStorePlaceQuery>();
            string json = string.Empty;
            try
            {
                DeliveryStorePlace model = new DeliveryStorePlace();
                model.Start = Convert.ToInt32(Request.Form["start"] ?? "0");
                model.Limit = Convert.ToInt32(Request.Form["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Form["dsp_name"]))
                {
                    model.dsp_name = Request.Form["dsp_name"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_big_code"]))
                {
                    model.dsp_big_code = Request.Form["dsp_big_code"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_deliver_store"]))
                {
                    model.dsp_deliver_store = Request.Form["dsp_deliver_store"];
                }
                DSPMgr = new DeliveryStorePlaceMgr(connectionString);
                int totalCount = 0;
                stores = DSPMgr.GetDeliveryStorePlaceList(model, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
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

        public HttpResponseBase GetTZipCodeList()
        {
            List<Zip> stores = new List<Zip>();
            string json = string.Empty;
            try
            {
                Zip zip = new Zip();
                zip.bigcode = "0";
                zip.big = "不分";
                IZImplMgr = new ZipMgr(connectionString);
                stores = IZImplMgr.GetZipList();
                if (string.IsNullOrEmpty(Request.Params["type"]))
                {
                    stores.Insert(0, zip);
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";
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

        public HttpResponseBase GetDspDeliverStoreList()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            string json = string.Empty;
            try
            {
                Parametersrc p = new Parametersrc();
                p.parameterName = "不分";
                p.ParameterCode = "0";
                IParametersrcImplMgr IPImplMgr = new ParameterMgr(connectionString);
                stores = IPImplMgr.ReturnParametersrcList();
                if (string.IsNullOrEmpty(Request.Params["type"]))
                {
                    stores.Insert(0, p);
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";
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

        public JsonResult DeliveryStorePlaceSave()
        {
            string json = string.Empty;
            try
            {
                DeliveryStorePlace model = new DeliveryStorePlace();
                DSPMgr = new DeliveryStorePlaceMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Form["dsp_name"]))
                {
                    model.dsp_name = Request.Form["dsp_name"];
                }
                model.modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                model.modify_time = DateTime.Now;
                model.dsp_status = 1;

                if (!string.IsNullOrEmpty(Request.Form["dsp_big_code"]))
                {
                    model.dsp_big_code = Request.Form["dsp_big_code"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_deliver_store"]))
                {
                    model.dsp_deliver_store = Request.Form["dsp_deliver_store"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_address"]))
                {
                    model.dsp_address = Request.Form["dsp_address"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_telephone"]))
                {
                    model.dsp_telephone = Request.Form["dsp_telephone"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_note"]))
                {
                    model.dsp_note = Request.Form["dsp_note"];
                }
                if (!string.IsNullOrEmpty(Request.Form["dsp_id"]))
                {
                    model.dsp_id = int.Parse(Request.Form["dsp_id"]);
                    if (DSPMgr.UpdateDeliveryStorePlace(model) > 0)
                    {
                        return Json(new { success = "true" });
                    }
                }
                else
                {
                    //if (DSPMgr.SelectDspName(model) != 0)
                    //{
                    //    return Json(new { success = "-1" });
                    //}
                    model.create_user = model.modify_user;
                    model.create_time = model.modify_time;
                    if (DSPMgr.InsertDeliveryStorePlace(model) > 0)
                    {
                        return Json(new { success = "true" });
                    }
                }

            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            return Json(new { success = "false" });
        }

        public JsonResult DeleteDeliveryStorePlaceByIds()
        {
            string json = string.Empty;
            try
            {
                DeliveryStorePlaceQuery query = new DeliveryStorePlaceQuery();
                DSPMgr = new DeliveryStorePlaceMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Form["rid"]))
                {
                    query.dsp_ids = Request.Form["rid"].TrimEnd(',');
                    if (DSPMgr.DeleteDeliveryStorePlace(query) > 0)
                    {
                        return Json(new { success = "true" });
                    }
                }
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            return Json(new { success = "false" });
        }
        #endregion
    }
}