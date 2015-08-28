#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountTrialController.cs
* 摘 要：
* 試用controller
* 当前版本：v1.1
* 作 者： shuangshuang0420j
* 完成日期：2014/11/18 
* 修改歷史：
*        
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using BLL.gigade.Model.Custom;
using System.Text;

namespace Admin.gigade.Controllers
{ 
    public class PromotionsAmountTrialController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsAmountTrialImplMgr _promotionsAmountTrialMgr;
        private IParametersrcImplMgr _parasrcMgr;
        private ITrialRecordImplMgr _ITrialRecordMgr;
        private IUserConditionImplMgr _ucMgr;
        private IProductCategoryImplMgr prodCateMgr;
        private IProductImplMgr _productMgr;
        private ISiteConfigImplMgr siteConfigMgr;
        private ITrialPictureImplMgr _ITrialPictureMgr;
        private IProductImplMgr _prodMgr;
       
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑
        string elementPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.elementPath);
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        string prodPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prodPath);


        //end 上傳圖片

        #region 視圖
        /// <summary>
        /// 試用活動
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionsAmountTrialList()
        {
            return View();
        }
        /// <summary>
        /// 分享記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult ShareRecord()
        {
            ViewBag.trial_id = Request.Params["id"];
            return View();
        }
        /// <summary>
        /// 試用記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult TrialRecord()
        {
            ViewBag.trial_id = Request.Params["id"];
            return View();
        }
        public ActionResult upLoad()
        {
            return View();
        }
        #endregion

        #region 獲取試用列表頁 + HttpResponseBase GetPromotionsAmountTrialList()
        /// <summary>
        /// 獲取試用列表頁
        /// </summary>
        /// <returns>json數組列表頁</returns>
        [CustomHandleError]
        public HttpResponseBase GetPromotionsAmountTrialList()
        {

            List<PromotionsAmountTrialQuery> store = new List<PromotionsAmountTrialQuery>();
            string json = string.Empty;
            try
            {
                PromotionsAmountTrialQuery query = new PromotionsAmountTrialQuery();

                #region 獲取query對象數據
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["serchcontent"]))
                {
                    query.key = Request.Params["serchcontent"];
                }
                #endregion

                _promotionsAmountTrialMgr = new PromotionsAmountTrialMgr(mySqlConnectionString);
                _prodMgr = new ProductMgr(mySqlConnectionString);
                int totalCount = 0;

                _ITrialRecordMgr = new TrialRecordMgr(mySqlConnectionString);
                store = _promotionsAmountTrialMgr.Query(query, out totalCount);
                List<Parametersrc> ProductFreightStore = new List<Parametersrc>();
                List<Parametersrc> DeviceStore = new List<Parametersrc>();
                List<Parametersrc> EventTypeStore = new List<Parametersrc>();
                _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                ProductFreightStore = _parasrcMgr.GetElementType("product_freight");//---deliver_type
                DeviceStore = _parasrcMgr.GetElementType("device");//--device
                EventTypeStore = _parasrcMgr.GetElementType("event_type");//--event_type
                foreach (var item in store)
                {
                    for (int i = 0; i < ProductFreightStore.Count; i++)
                    {
                        if (int.Parse(ProductFreightStore[i].ParameterCode) == item.freight_type)
                        {
                            item.freight = ProductFreightStore[i].parameterName;
                        }
                    }
                    for (int i = 0; i < DeviceStore.Count; i++)
                    {
                        if (DeviceStore[i].ParameterCode == item.device.ToString())
                        {
                            item.device_name = DeviceStore[i].parameterName;
                        }
                    }
                    for (int i = 0; i < EventTypeStore.Count; i++)
                    {
                        if (EventTypeStore[i].ParameterCode == item.event_type)
                        {
                            item.eventtype = EventTypeStore[i].parameterName;
                        }
                    }
                    if (item.sale_productid != 0)
                    {
                        item.sale_product_name = _prodMgr.QueryClassify(Convert.ToUInt32(item.sale_productid)).Product_Name;
                    }
                    if (item.event_img_small != "")
                    {
                        item.event_img_small = imgServerPath + promoPath + item.event_img_small;
                    }
                    else
                    {
                        item.event_img_small = defaultImg;
                    }
                    if (item.event_img != "")
                    {
                        item.event_img = imgServerPath + promoPath + item.event_img;
                    }
                    else
                    {
                        item.event_img = defaultImg;
                    }
                    if (item.product_img != "")
                    {
                        if (item.product_img.ToString().StartsWith("T1") || item.product_img.ToString().StartsWith("T2"))
                        {
                            item.product_img = imgServerPath + promoPath + item.product_img;
                        }
                        else
                        {
                            item.product_img = imgServerPath + prodPath + GetDetailFolder(item.product_img) + item.product_img;
                        }
                    }
                    else
                    {
                        item.product_img = defaultImg;
                    }
                    DataTable _dtCount = _ITrialRecordMgr.GetSumCount(item);
                    if (_dtCount.Rows.Count != 0)
                    {
                        if (_dtCount.Rows[0][1].ToString() != "")
                        {
                            item.recordCount = Convert.ToInt32(_dtCount.Rows[0][1].ToString());
                        };
                        if (_dtCount.Rows[0][2].ToString() != "")
                        {
                            item.shareCount = Convert.ToInt32(_dtCount.Rows[0][2].ToString());
                        }
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
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;


        }
        #endregion
        /// <summary>
        /// 保存第一步數據
        /// </summary>
        /// <returns>json數組第一步保存產生的event_type和event_id</returns>
        [CustomHandleError]
        public HttpResponseBase SaveTrial()
        {
            string jsonStr = String.Empty;
            try
            {
                PromotionsAmountTrialQuery patmodel = new PromotionsAmountTrialQuery();

                _promotionsAmountTrialMgr = new PromotionsAmountTrialMgr(mySqlConnectionString);
                #region 試吃試用第一步數據
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {//活動名稱
                    patmodel.name = Request.Params["name"].ToString();
                } //活動類型
                int event_type = 0;
                if (!string.IsNullOrEmpty(Request.Params["event_type"]))
                {
                    if (int.TryParse(Request.Params["event_type"], out event_type))
                    {
                        if (event_type == 1)
                        {
                            patmodel.event_type = "T1";
                        }
                        else if (event_type == 2)
                        {
                            patmodel.event_type = "T2";
                        }
                    }
                }
                int paper_id = 0;
                if (int.TryParse(Request.Params["paper_id"].ToString(), out paper_id))
                {
                    patmodel.paper_id = paper_id;
                }
                else
                {
                    patmodel.paper_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    patmodel.start_date = Convert.ToDateTime(Request.Params["start_date"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["end_date"]))
                {
                    patmodel.end_date = Convert.ToDateTime(Request.Params["end_date"].ToString());
                }
                //活動描述
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    patmodel.event_desc = Request.Params["event_desc"].ToString();
                }
                patmodel.status = 0;//第一步保存為無效數據
                patmodel.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                patmodel.created = DateTime.Now;
                patmodel.modified = patmodel.created;
                patmodel.muser = patmodel.kuser;

                #endregion

                int id = _promotionsAmountTrialMgr.Save(patmodel);

                string event_id = CommonFunction.GetEventId(patmodel.event_type, id.ToString());
                if (id > 0)
                {
                    jsonStr = "{success:true,\"id\":\"" + id + "\",\"event_id\":\"" + event_id + "\"}";
                }
                else
                {
                    jsonStr = "{success:false }";
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


            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;

        }

        #region 試用第二步兼編輯
        /// <summary>
        /// 保存第二步數據即編輯第一步保存的數據
        /// </summary>
        /// <returns>json數組 第二步編輯是否成功</returns>
        [CustomHandleError]
        public HttpResponseBase UpdateTrial()
        {
            string jsonStr = String.Empty;
            int isTranInt = 0;
            try
            {
                string trial_id = Request.Params["trial_id"].ToString();
                string isEdit = Request.Params["isEdit"].ToString();
                _promotionsAmountTrialMgr = new PromotionsAmountTrialMgr(mySqlConnectionString);
                _ucMgr = new UserConditionMgr(mySqlConnectionString);
                PromotionsAmountTrialQuery model = new PromotionsAmountTrialQuery();
                PromotionsAmountTrialQuery OldModel = new PromotionsAmountTrialQuery();


                #region 第一面板數據
                if (!string.IsNullOrEmpty(Request.Params["trial_id"]))
                {
                    model.id = Convert.ToInt32(Request.Params["trial_id"]);
                    OldModel = _promotionsAmountTrialMgr.Select(model.id);
                }
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {//活動名稱
                    model.name = Request.Params["name"].ToString();
                }


                int event_type = 0;
                if (!string.IsNullOrEmpty(Request.Params["event_type"]))
                {
                    if (int.TryParse(Request.Params["event_type"], out event_type))
                    {
                        if (event_type == 1)
                        {
                            model.event_type = "T1";
                        }
                        else if (event_type == 2)
                        {
                            model.event_type = "T2";
                        }
                    }
                }
                model.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());

                int paper_id = 0;
                if (int.TryParse(Request.Params["paper_id"].ToString(), out paper_id))
                {
                    model.paper_id = paper_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["event_url"].ToString()))
                {
                    model.url = Request.Params["event_url"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    model.start_date = Convert.ToDateTime(Request.Params["start_date"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["end_date"]))
                {
                    model.end_date = Convert.ToDateTime(Request.Params["end_date"].ToString());
                }
                //活動描述
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    model.event_desc = Request.Params["event_desc"].ToString();
                }

                #endregion
                #region 第二面板數據
                if (!string.IsNullOrEmpty(Request.Params["group_id"].ToString()))
                {
                    if (int.TryParse(Request.Params["group_id"].ToString(), out isTranInt))
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    else
                    {
                        model.group_id = OldModel.group_id;
                    }
                    //當編輯活動時將會員條件改為會員群組則刪除原有的會員條件
                    if (OldModel.condition_id != 0)
                    {
                        UserCondition uc = new UserCondition();
                        uc.condition_id = OldModel.condition_id;

                        if (_ucMgr.Delete(uc) > 0)
                        {
                            jsonStr = "{success:true}";
                            model.condition_id = 0;
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:'user_condition刪除出錯！'}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr.ToString());
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    //當活動設置完會員條件又改為會員群組時刪除設置的會員條件
                    if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                    {
                        UserCondition uc = new UserCondition();
                        uc.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                        if (_ucMgr.Delete(uc) > 0)
                        {
                            jsonStr = "{success:true}";
                            model.condition_id = 0;
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:'user_condition刪除出錯！'}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr.ToString());
                            this.Response.End();
                            return this.Response;
                        }
                    }
                }
                else if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                {
                    model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    model.group_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["count_by"]))
                {
                    model.count_by = Convert.ToInt32(Request.Params["count_by"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["numLimit"]))
                {
                    model.num_limit = Convert.ToInt32(Request.Params["numLimit"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["gift_mundane"]))
                {
                    model.gift_mundane = Convert.ToInt32(Request.Params["gift_mundane"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["IsRepeat"]))
                {
                    model.repeat = Convert.ToInt32(Request.Params["IsRepeat"].ToString()) == 1 ? true : false;
                }

                if (!string.IsNullOrEmpty(Request.Params["freight_type"].ToString()))
                {
                    if (int.TryParse(Request.Params["freight_type"].ToString(), out isTranInt))
                    {
                        model.freight_type = Convert.ToInt32(Request.Params["freight_type"].ToString());
                    }
                    else
                    {
                        model.freight_type = OldModel.freight_type;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["device_name"]))
                {
                    model.device = Convert.ToInt32(Request.Params["device_name"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["site"].ToString()))//修改時傳的值為siteName
                {

                    Regex reg = new Regex("^([0-9]+,)*([0-9]+)$");
                    if (reg.IsMatch(Request.Params["site"].ToString()))
                    {
                        model.site = Request.Params["site"].ToString();// 將站台改為多選 edit by shuangshuang0420j 20140925 10:08
                    }
                    else
                    {
                        model.site = OldModel.site;
                    }
                }

                #endregion

                #region 第三面板數據

                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    model.product_id = Convert.ToInt32(Request.Params["product_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["product_name"]))
                {
                    model.product_name = Request.Params["product_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["sale_product_id"]))
                {
                    model.sale_productid = Convert.ToInt32(Request.Params["sale_product_id"].ToString());
                }
                model.category_id = Convert.ToUInt32(Request.Params["category_id"].ToString());
                model.brand_id = Convert.ToInt32(Request.Params["brand_id"].ToString());
                model.market_price = Convert.ToInt32(Request.Params["market_price"].ToString());
                model.show_number = Convert.ToInt32(Request.Params["show_number"].ToString());
                model.apply_limit = Convert.ToInt32(Request.Params["apply_limit"].ToString());
                model.apply_sum = Convert.ToInt32(Request.Params["apply_sum"].ToString());

                #region 上傳圖片
                try
                {
                    string path = Server.MapPath(xmlPath);
                    SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                    SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                    SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
                    SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                    SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
                    SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");

                    //擴展名、最小值、最大值
                    string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                    string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                    string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;

                    string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址


                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    bool result = false;
                    string NewFileName = string.Empty;//編譯后的文件名
                    string oldFile = "";
                    string ServerPath = string.Empty;
                    string ErrorMsg = string.Empty;


                    //判斷目錄是否存在，不存在則創建
                    CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                    FileManagement fileLoad = new FileManagement();

                    for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                    {
                        if (iFile == 0 && !string.IsNullOrEmpty(Request.Params["prod_file"]))
                        {
                            model.product_img = Request.Params["prod_file"].ToString().Substring(Request.Params["prod_file"].ToString().LastIndexOf("/") + 1);
                        }
                        else
                        {

                            HttpPostedFileBase file = Request.Files[iFile];

                            Random rand = new Random();
                            int newRand = rand.Next(1000, 9999);

                            fileName = Path.GetFileName(file.FileName);
                            if (fileName != "")
                            {

                                switch (iFile)
                                {
                                    case 1:
                                        oldFile = OldModel.event_img_small;
                                        break;
                                    case 2:
                                        oldFile = OldModel.event_img;
                                        break;
                                    case 0:
                                        oldFile = OldModel.product_img;
                                        break;
                                }

                                FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFile))
                                {
                                    FTP ftps = new FTP(localPromoPath + oldFile, ftpuser, ftppwd);
                                    ftps.DeleteFile(localPromoPath + oldFile);//刪除ftp:71.159上的舊圖片
                                    DeletePicFile(ServerPath + oldFile);//刪除本地圖片
                                }

                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = model.event_id + newRand + fileExtention;

                                fileName = NewFileName;
                                NewFileName = localPromoPath + NewFileName;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                try
                                {
                                    //上傳
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)//上傳成功
                                    {
                                        switch (iFile)
                                        {
                                            case 1:
                                                model.event_img_small = fileName;
                                                break;
                                            case 2:
                                                model.event_img = fileName;
                                                break;
                                            case 0:
                                                model.product_img = fileName;
                                                break;
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
                            else
                            {
                                switch (iFile)
                                {
                                    case 1:
                                        model.event_img_small = OldModel.event_img_small;
                                        break;
                                    case 2:
                                        model.event_img = OldModel.event_img;
                                        break;
                                    case 0:
                                        model.product_img = OldModel.product_img;
                                        break;
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
                }

                #endregion

                #endregion


                #region     更新表結構
                if (string.IsNullOrEmpty(isEdit))//新增數據
                {
                    model.active = 0;//默認不啟用
                    model.status = 1;//第二步保存為有效數據
                    model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.created = DateTime.Now;
                    model.muser = model.kuser;
                    model.modified = model.created;
                    if (_promotionsAmountTrialMgr.Update(model) > 0)
                    {

                        jsonStr = "{success:true,msg:0}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:0}";//返回json數據
                    }
                }
                else//編輯數據
                {
                    model.active = OldModel.active;
                    model.status = OldModel.status;
                    model.kuser = OldModel.kuser;
                    model.created = OldModel.created;
                    model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.modified = DateTime.Now;
                    if (_promotionsAmountTrialMgr.Update(model) > 0)
                    {

                        jsonStr = "{success:true,msg:0}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:0}";//返回json數據
                    }
                }
                #endregion
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
        #endregion
        #region 更改活動使用狀態 + HttpResponseBase UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string jsonStr = string.Empty;
            try
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
                _promotionsAmountTrialMgr = new PromotionsAmountTrialMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                PromotionsAmountTrialQuery model = _promotionsAmountTrialMgr.Select(id);

                model.active = activeValue;
                model.id = id;
                model.muser = currentUser;
                model.modified = DateTime.Now;

                if (_promotionsAmountTrialMgr.UpdateActive(model) > 0)
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

        #region 創建ftp文件夾 + void CreateFolder(string path, string[] Mappath)
        /// <summary>
        /// 創建文件夾
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="Mappath">文件名</param>
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
                        // ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                        ftp = new FTP(fullPath, ftpuser, ftppwd);
                        ftp.MakeDirectory();
                    }
                    fullPath += "/";

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

        #region 刪除本地上傳的圖片+void DeletePicFile(string imageName)
        /// <summary>
        /// 刪除本地上傳的圖片
        /// </summary>
        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }
        #endregion

        #region 分享記錄
        #region  列表頁+HttpResponseBase GetShareRecordList()
        public HttpResponseBase GetShareRecordList()
        {
            string json = string.Empty;
            TrialShareQuery query = new TrialShareQuery();
            //    List<TrialShareQuery> store = new List<TrialShareQuery>();
            int totalCount = 0;
            _ITrialRecordMgr = new TrialRecordMgr(mySqlConnectionString);
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["trial_id"]))
                {
                    query.trial_id = Convert.ToInt32(Request.Params["trial_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    query.share_id = Convert.ToInt32(Request.Params["relation_id"]);
                }
                DataTable _dt = _ITrialRecordMgr.GetShareList(query, out totalCount);
                //foreach (var item in store)
                //{
                //    item.gender = item.user_gender == 0 ? "小姐" : "先生";
                //}
                if (Convert.ToBoolean(Request.Params["isSecret"]))
                {
                    foreach (DataRow item in _dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(item["user_name"].ToString()))
                        {
                            item["user_name"] = item["user_name"].ToString().Substring(0, 1) + "**";
                        }
                         if (!string.IsNullOrEmpty(item["real_name"].ToString()))
                        {
                            item["real_name"] = item["real_name"].ToString().Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item["after_name"].ToString()))
                        {
                            item["after_name"] = item["after_name"].ToString().Substring(0, 1) + "**";
                        }
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";
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
        #region 新增編輯分享記錄信息+HttpResponseBase TrialRecordSave()
        public HttpResponseBase TrialRecordSave()
        {
            string json = string.Empty;
            string jsonStr = string.Empty;
            TrialShareQuery query = new TrialShareQuery();
            TrialPictureQuery trialP = new TrialPictureQuery();
            List<TrialPictureQuery> trialPList = new List<TrialPictureQuery>();
            _ITrialPictureMgr = new TrialPictureMgr(mySqlConnectionString);
            try
            {
                query.share_id = Convert.ToInt32(Request["share_id"]);
                #region 處理圖片
                try
                {
                    if (!string.IsNullOrEmpty(Request.Params["picInfo"]))
                    {

                        string[] picInfo = Request.Params["picInfo"].Split(';');
                        for (int i = 0; i < picInfo.Length - 1; i++)
                        {
                            string[] perValue = picInfo[i].Split(',');
                            trialP = new TrialPictureQuery();
                            if (!string.IsNullOrEmpty(perValue[0])) { trialP.share_id = query.share_id; }
                            if (!string.IsNullOrEmpty(perValue[1])) { trialP.image_filename = perValue[1]; }
                            if (!string.IsNullOrEmpty(perValue[2])) { trialP.image_sort = Convert.ToUInt32(perValue[2]); }
                            if (!string.IsNullOrEmpty(perValue[3])) { trialP.image_state = Convert.ToUInt32(perValue[3]); }
                            trialPList.Add(trialP);
                        }
                        _ITrialPictureMgr = new TrialPictureMgr(mySqlConnectionString);
                        _ITrialPictureMgr.SavePic(trialPList, trialP);
                    }
                    else
                    {
                        _ITrialPictureMgr.DeleteAllPic(trialP);
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
                #endregion
                _ITrialRecordMgr = new TrialRecordMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["trial_id"]))
                {
                    query.trial_id = Convert.ToInt32(Request.Params["trial_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["user_name"]))
                {
                    query.user_name = Request.Params["user_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["user_gender"]))
                {
                    query.user_gender = Convert.ToInt32(Request.Params["user_gender"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["niming"]))
                {
                    if (Request.Params["niming"] == "on")
                    {
                        query.is_show_name = 0;//匿名
                    }
                    else
                    {
                        query.is_show_name = 1;//不匿名
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["content"]))
                {
                    query.content = Request.Params["content"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.status = Convert.ToInt32(Request.Params["status"].ToString());
                }
                int j = _ITrialRecordMgr.TrialRecordSave(query);
                if (j > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{failure:true}";
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
        #endregion
        #region 查詢圖片
        [HttpPost]
        public string QueryPic()
        {
            string json = string.Empty;
            string serverElePath = imgServerPath + elementPath;
            List<TrialPictureQuery> picList = new List<TrialPictureQuery>();
            TrialPictureQuery query = new TrialPictureQuery();
            _ITrialPictureMgr = new TrialPictureMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["share_id"]))
            {
                query.share_id = Convert.ToInt32(Request.Params["share_id"]);
            }
            picList = _ITrialPictureMgr.QueryPic(query);
            foreach (var item in picList)
            {
                if (item.image_filename != "")
                {
                    //item.image_filename = serverElePath + GetDetailFolder(item.image_filename) + item.image_filename;
                    item.image_filename = serverElePath + item.image_filename;
                }
            }
            json = "{success:true,items:" + JsonConvert.SerializeObject(picList) + "}";
            return json;
        }
        public string GetDetailFolder(string picName)
        {
            string firthFolder = picName.Substring(0, 2) + "/";
            string secondFolder = picName.Substring(2, 2) + "/";

            return firthFolder + secondFolder;
        }
        #endregion
        #region 上傳圖片
        public ActionResult UpLoadImg()
        {
            string path = Server.MapPath(xmlPath);
            siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig extention_config = siteConfigMgr.GetConfigByName("PIC_Extention_Format");
            SiteConfig minValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
            SiteConfig maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
            SiteConfig admin_userName = siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
            SiteConfig admin_passwd = siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
            string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
            string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;

            string elementPath = ConfigurationManager.AppSettings["elementPath"];
            FileManagement fileLoad = new FileManagement();
            HttpPostedFileBase file = Request.Files["Filedata"];
            string fileName = string.Empty;
            string fileExtention = string.Empty;
            string[] Mappath = new string[2];
            fileName = fileLoad.NewFileName(file.FileName);
            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
            fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower();
            string localElePath = imgLocalPath + elementPath;
            string returnName = imgServerPath;////"http://192.168.71.159:8080"
            bool result = false;
            string NewFileName = string.Empty;
            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
            NewFileName = hash.Md5Encrypt(fileName, "32");
            string firstFolder = NewFileName.Substring(0, 2) + "/";
            string secondFolder = NewFileName.Substring(2, 2) + "/";
            string ServerPath = string.Empty;
            Mappath[0] = firstFolder;
            Mappath[1] = secondFolder;
            //CreateFolder(localElePath, Mappath);
            //   localElePath += firstFolder + secondFolder;
            //  elementPath += firstFolder + secondFolder;
            returnName += elementPath + NewFileName + fileExtention;
            NewFileName = localElePath + NewFileName + fileExtention;
            ServerPath = Server.MapPath(imgLocalServerPath + elementPath);
            string ErrorMsg = string.Empty;
            try
            {
                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
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
                ViewBag.Type = "pic";
            }
            else
            {
                ViewBag.fileName = "ERROR/" + ErrorMsg;
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = "ERROR/" + ErrorMsg;
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            return View("~/Views/PromotionsAmountTrial/upLoad.cshtml");
        }
        #endregion
        #endregion

        #region 試用記錄
        public HttpResponseBase GetTrialRecordList()
        {
            string json = string.Empty;
            TrialRecordQuery query = new TrialRecordQuery();
            List<TrialRecordQuery> store = new List<TrialRecordQuery>();
            int totalCount = 0;
            _ITrialRecordMgr = new TrialRecordMgr(mySqlConnectionString);
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["trial_id"]))
                {
                    query.trial_id = Convert.ToInt32(Request.Params["trial_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["luquStatus"]))
                {
                    query.status = Convert.ToInt32(Request.Params["luquStatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    query.record_id = Convert.ToInt32(Request.Params["relation_id"]);
                }
                //Request.Params["eventId"].ToString();
                store = _ITrialRecordMgr.GetTrialRecordList(query, out totalCount);

                if (Convert.ToBoolean(Request.Params["isSecret"]))
                {
                    foreach (var item in store)
                    {
                        if (!string.IsNullOrEmpty(item.user_name))
                        {
                            item.user_name = item.user_name.Substring(0, 1) + "**";
                        }
                        item.user_email = item.user_email.Split('@')[0] + "@***";
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
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult TrialRecordUpdate()
        {
            string jsonStr = string.Empty;
            try
            {
                _ITrialRecordMgr = new TrialRecordMgr(mySqlConnectionString);
                TrialRecordQuery model = new TrialRecordQuery();
                if (!string.IsNullOrEmpty(Request.Params["record_id"]))
                {
                    model.record_id = Convert.ToInt32(Request.Params["record_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["user_email"]))
                {
                    model.user_email = Request.Params["user_email"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    model.status = Convert.ToInt32(Request.Params["status"].ToString());
                }
                if (_ITrialRecordMgr.TrialRecordUpdate(model) > 0)
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

        #region 獲取元素詳情列表中類別中的所有商品，包含父節點及其下面的子節點
        public HttpResponseBase GetProductByCategorySet()
        {
            string resultStr = "{success:false}";
            string str = string.Empty;
            try
            {
                _productMgr = new ProductMgr(mySqlConnectionString);
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
                if (!string.IsNullOrEmpty(Request.Params["site_id"].ToString()))
                {
                    query.siteStr = Request.Params["site_id"].ToString();
                }
                if (uint.TryParse(Request.Params["brand_id"].ToString(), out isTryUint))
                {
                    query.Brand_Id = Convert.ToUInt32(Request.Params["brand_id"].ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                    {
                        VendorBrandSetMgr vbsMgr = new VendorBrandSetMgr(mySqlConnectionString);
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
                    if (uint.TryParse(Request.Params["keyCode"].ToString(), out isTryUint))
                    {
                        query.Product_Id = Convert.ToUInt32(Request.Params["keyCode"].ToString());
                    }
                    else
                    {
                        query.Product_Name = Request.Params["keyCode"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["category_id"].ToString()))
                {
                    if (uint.TryParse(Request.Params["category_id"].ToString(), out isTryUint))
                    {
                        //判斷是否是父節點，若是則獲取所有的category_id
                        prodCateMgr = new ProductCategoryMgr(mySqlConnectionString);
                        List<ProductCategory> category = prodCateMgr.QueryAll(new ProductCategory());
                        GetAllCategory_id(category, Convert.ToUInt32(Request.Params["category_id"].ToString()), ref str);
                        query.categoryArry = str;
                    }
                }
                int totalCount = 0;
                List<ProductDetailsCustom> prods = _productMgr.GetAllProList(query, out totalCount);
                foreach (var item in prods)
                {
                    if (!string.IsNullOrEmpty(item.product_image))
                    {
                        item.product_image = imgServerPath + prodPath + GetDetailFolder(item.product_image) + item.product_image;
                    }
                    else
                    {
                        item.product_image = imgServerPath + "/product/nopic_150.jpg";
                    }
                }
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

        #region 驗證郵箱--此功能已取消
        [HttpPost]
        //public string VerifyEmail()
        //{
        //    string json = string.Empty;
        //    _ITrialPictureMgr = new TrialPictureMgr(mySqlConnectionString);
        //    string email = Request.Params["email"];
        //    string result = _ITrialPictureMgr.VerifyEmail(email);
        //    if (result == "")
        //    {
        //        json = "{success:false}";
        //    }
        //    else
        //    {
        //        string[] perValue = result.Split(';');
        //        json = "{success:true,\"gender\":\"" + perValue[0] + "\",\"name\":\"" + perValue[1] + "\"}";
        //        //  json = result;
        //    }

        //    return json;
        //}
        #endregion
        #region 遞歸查詢子cateID
        /// <summary>
        /// 遞歸查詢子ID //edit by hjiajun1211w 2014/08/08 父商品查詢
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
        public HttpResponseBase VerifyMaxCount()
        {
            TrialRecordQuery query = new TrialRecordQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.status = Convert.ToInt32(Request.Params["status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["trial_id"]))
                {
                    query.trial_id = Convert.ToInt32(Request.Params["trial_id"]);
                }
                _ITrialRecordMgr = new TrialRecordMgr(mySqlConnectionString);
                if (_ITrialRecordMgr.VerifyMaxCount(query))
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{failure:true}";
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

    }
}
