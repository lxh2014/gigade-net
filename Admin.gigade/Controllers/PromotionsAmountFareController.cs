
#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountFareController.cs      
* 摘 要：                                                                               
* 滿額滿件免運列表顯示和後台方法處理
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/6/20 
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：規範代碼結構，完善異常拋出，添加注釋
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
using System.Configuration;
using System.Text.RegularExpressions;

 
namespace Admin.gigade.Controllers
{
    public class PromotionsAmountFareController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsAmountFareImplMgr _promoAmountFareMgr;
        private IParametersrcImplMgr _parasrcMgr;
        private IUserConditionImplMgr _ucMgr;
        //private ISiteImplMgr _siteMgr;
        private IProductCategoryImplMgr _produCateMgr = new ProductCategoryMgr(mySqlConnectionString);
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        // string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑

        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";

        //
        // GET: /PromotionsAmountFare/
        #region 獲滿額滿件視圖頁
        /// <summary>
        /// 滿額滿件免運的視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 獲滿額滿件列表頁 + HttpResponseBase GetPromoAmountFareList()
        /// <summary>
        /// 獲取滿額滿件列表頁
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetPromoAmountFareList()
        {

            List<PromotionsAmountFareQuery> store = new List<PromotionsAmountFareQuery>();
            string json = string.Empty;
            try
            {
                PromotionsAmountFareQuery query = new PromotionsAmountFareQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                else
                {
                    query.Limit = 25;
                }

                if (!string.IsNullOrEmpty(Request.Params["expiredSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["expiredSel"]);
                }
                _promoAmountFareMgr = new PromotionsAmountFareMgr(mySqlConnectionString);
                _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                int totalCount = 0;//獲取所有數據的行數
                store = _promoAmountFareMgr.Query(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                foreach (var item in store)
                {
                    if (item.banner_image != "")
                    {
                        item.banner_image = imgServerPath + promoPath + item.banner_image;
                    }
                    else
                    {
                        item.banner_image = defaultImg;
                    }

                }
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

        #region 第一步保存滿額滿件免運數據 + HttpResponseBase FirstSaveFare()
        /// <summary>
        /// 第一步保存滿額滿件免運數據
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase FirstSaveFare()
        {
            string jsonStr = String.Empty;
            try
            {
                PromotionsAmountFareQuery model = new PromotionsAmountFareQuery();
                if (!string.IsNullOrEmpty(Request.Params["name"].ToString()))
                {
                    model.name = Request.Params["name"].ToString();
                }
                else
                {
                    model.name = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["desc"].ToString()))
                {
                    model.event_desc = Request.Params["desc"].ToString();
                }
                else
                {
                    model.event_desc = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["vendor_coverage"]))
                {
                    model.vendor_coverage = Convert.ToInt32(Request.Params["vendor_coverage"].ToString());
                }
                else
                {
                    model.vendor_coverage = 0;
                }

                if (!string.IsNullOrEmpty(Request.Params["amount"].ToString()))
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"].ToString());
                }
                else
                {
                    model.amount = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["quantity"].ToString()))
                {
                    model.quantity = Convert.ToInt32(Request.Params["quantity"].ToString());
                }
                else
                {
                    model.quantity = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["fare_percent"].ToString()))
                {
                    model.fare_percent = Convert.ToInt32(Request.Params["fare_percent"].ToString());
                }
                else
                {
                    model.fare_percent = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["off_times"].ToString()))
                {
                    model.off_times = Convert.ToInt32(Request.Params["off_times"].ToString());
                }
                else
                {
                    model.off_times = 0;
                }




                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.created = DateTime.Now;
                model.status = 0;

                //獲取event_type和category_father_id
                _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                List<Parametersrc> fatherIdResult = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", parameterName = Request.Params["event_type"].ToString(), Used = 1 });
                if (fatherIdResult.Count == 1)
                {
                    model.event_type = fatherIdResult[0].ParameterCode;
                    model.category_father_id = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                }

                model.category_ipfrom = Request.UserHostAddress;//獲取當前ip

                _promoAmountFareMgr = new PromotionsAmountFareMgr(mySqlConnectionString);
                PromotionsAmountFare query = new PromotionsAmountFare();
                query.id = _promoAmountFareMgr.Save(model);
                System.Data.DataTable dt = _promoAmountFareMgr.Select(query);

                if (dt.Rows.Count > 0)
                {
                    jsonStr = "{success:true,\"id\":\"" + dt.Rows[0]["id"] + "\",\"cateID\":\"" + dt.Rows[0]["category_id"] + "\",\"event_type\":\"" + dt.Rows[0]["event_type"] + "\"}";
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
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存第二步數據 + HttpResponseBase SecondSaveFare()
        /// <summary>
        /// 保存第二步數據
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SecondSaveFare()
        {
            string jsonStr = String.Empty;
            int stringTranInt = 0;//使用tryParse使string轉換為int時保存轉換的值
            try
            {
                string rowID = Request.Params["rowid"].ToString() ?? "0";
                string isEdit = Request.Params["isEdit"].ToString() ?? "";
                _promoAmountFareMgr = new PromotionsAmountFareMgr(mySqlConnectionString);
                PromotionsAmountFareQuery model = new PromotionsAmountFareQuery();

                PromotionsAmountFareQuery OldModel = _promoAmountFareMgr.Select(Convert.ToInt32(rowID));

                #region 獲取數據
                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    model.id = Convert.ToInt32(Request.Params["rowid"]);
                }
                else
                {
                    model.id = 0;
                }
                model.category_id = OldModel.category_id;
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    model.name = Request.Params["name"].ToString();
                }
                else
                {
                    model.name = OldModel.name;
                }
                if (!string.IsNullOrEmpty(Request.Params["desc"]))
                {
                    model.event_desc = Request.Params["desc"].ToString();
                }

                else
                {
                    model.event_desc = OldModel.event_desc;
                }
                if (!string.IsNullOrEmpty(Request.Params["vendor_coverage"]))
                {
                    model.vendor_coverage = Convert.ToInt32(Request.Params["vendor_coverage"].ToString());
                }
                else
                {
                    model.vendor_coverage = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["sclass_id"]))
                {
                    if (int.TryParse(Request.Params["sclass_id"].ToString(), out stringTranInt))
                    {
                        model.class_id = Convert.ToInt32(Request.Params["sclass_id"].ToString() == "" ? "0" : Request.Params["sclass_id"].ToString());
                    }
                    else
                    {
                        model.class_id = OldModel.class_id;
                    }
                }
                else
                {
                    model.class_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["sbrand_id"]))
                {
                    if (int.TryParse(Request.Params["sbrand_id"].ToString(), out stringTranInt))
                    {
                        model.brand_id = Convert.ToInt32(Request.Params["sbrand_id"].ToString() == "" ? "0" : Request.Params["sbrand_id"].ToString());

                    }
                    else
                    {
                        model.brand_id = OldModel.brand_id;
                    }
                }
                else
                {
                    model.brand_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["allClass"]))
                {
                    model.allClass = Convert.ToInt32(Request.Params["allClass"].ToString() == "1" ? "1" : "0");
                    if (1 == model.allClass)
                    {
                        model.product_id = 999999;
                    }

                }
                else
                {
                    model.allClass = 0;
                }

                if (!string.IsNullOrEmpty(Request.Params["event_type"]))
                {
                    //獲取event_type和category_father_id
                    _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                    List<Parametersrc> fatherIdResult = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", parameterName = Request.Params["event_type"].ToString(), Used = 1 });
                    if (fatherIdResult.Count == 1)
                    {
                        model.event_type = fatherIdResult[0].ParameterCode;
                        model.category_father_id = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["amount"]))
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"].ToString());
                }
                else
                {
                    model.amount = OldModel.amount;
                }
                if (!string.IsNullOrEmpty(Request.Params["quantity"]))
                {
                    model.quantity = Convert.ToInt32(Request.Params["quantity"].ToString());
                }
                else
                {
                    model.quantity = OldModel.quantity;
                }
                if (!string.IsNullOrEmpty(Request.Params["fare_percent"]))
                {
                    model.fare_percent = Convert.ToInt32(Request.Params["fare_percent"].ToString());
                }
                else
                {
                    model.fare_percent = OldModel.fare_percent;
                }
                if (!string.IsNullOrEmpty(Request.Params["off_times"]))
                {
                    model.off_times = Convert.ToInt32(Request.Params["off_times"].ToString());
                }
                else
                {
                    model.off_times = OldModel.off_times;
                }
                if (model.id != 0)
                {
                    model.event_id = BLL.gigade.Common.CommonFunction.GetEventId(model.event_type, model.id.ToString());
                }

                if (Request.Params["group_id"].ToString() != "")
                {//會員群組不為空時，要清空已存在的會員條件
                    if (!string.IsNullOrEmpty(Request.Params["group_id"].ToString()) && int.TryParse(Request.Params["group_id"].ToString(), out stringTranInt))//group_id
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    else
                    {
                        model.group_id = OldModel.group_id;
                    }
                    if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                    {
                        UserCondition uc = new UserCondition();
                        uc.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                        _ucMgr = new UserConditionMgr(mySqlConnectionString);
                        if (_ucMgr.Delete(uc) > 0)
                        {
                            jsonStr = "{success:true}";
                            model.condition_id = 0;
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:3}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr.ToString());
                            this.Response.End();
                            return this.Response;
                        }
                    }

                }
                else if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                {
                    try//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.condition_id = OldModel.condition_id;
                    }
                    model.group_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["type"]) && int.TryParse(Request.Params["type"].ToString(), out stringTranInt))
                {
                    model.type = Convert.ToInt32(Request.Params["type"].ToString());
                }
                else
                {
                    model.type = OldModel.type;
                }
                if (!string.IsNullOrEmpty(Request.Params["deliver"].ToString()) && int.TryParse(Request.Params["deliver"].ToString(), out stringTranInt))
                {
                    model.delivery_store = Convert.ToInt32(Request.Params["deliver"].ToString());
                }
                else
                {
                    model.delivery_store = OldModel.delivery_store;
                }
                if (!string.IsNullOrEmpty(Request.Params["devicename"].ToString()))
                {
                    model.device = Convert.ToInt32(Request.Params["devicename"].ToString());
                }
                else
                {
                    model.device = OldModel.device;
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"].ToString()))
                {
                    Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
                    if (reg.IsMatch(Request.Params["payment"].ToString()))
                    {
                        model.payment_code = Request.Params["payment"].ToString();
                    }
                    else
                    {
                        model.payment_code = OldModel.payment_code;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"].ToString()))
                {
                    model.start = Convert.ToDateTime(Request.Params["start_time"].ToString());
                }
                else
                {
                    model.start = OldModel.start_time;
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"].ToString()))
                {
                    model.end = Convert.ToDateTime(Request.Params["end_time"].ToString());
                }
                else
                {
                    model.end = OldModel.end_time;
                }
                if (!string.IsNullOrEmpty(Request.Params["site"].ToString()))
                {
                    Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
                    if (reg.IsMatch(Request.Params["site"].ToString()))
                    {
                        model.site = Request.Params["site"].ToString();
                    }
                    else
                    {
                        model.site = OldModel.site;
                    }
                }

                model.status = 1;
                if (!string.IsNullOrEmpty(Request.Params["url_by"].ToString()))
                {
                    model.url_by = Convert.ToInt32(Request.Params["url_by"].ToString());
                }
                else
                {
                    model.url_by = OldModel.url_by;
                }

                #endregion
                ////講圖片和鏈接保存至product_category中

                if (model.url_by == 1)
                {
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

                        Random rand = new Random();
                        int newRand = rand.Next(1000, 9999);//獲取隨機數重命名圖片


                        if (Request.Files.Count > 0)
                        {

                            HttpPostedFileBase file = Request.Files[0];
                            string fileName = string.Empty;//當前文件名

                            string fileExtention = string.Empty;//當前文件的擴展名

                            fileName = Path.GetFileName(file.FileName);
                            if (fileName != "")
                            {
                                // string returnName = imgServerPath;
                                bool result = false;
                                string NewFileName = string.Empty;

                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = model.event_id + newRand + fileExtention;

                                string ServerPath = string.Empty;
                                //判斷目錄是否存在，不存在則創建
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));
                                fileName = NewFileName;
                                NewFileName = localPromoPath + NewFileName;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                string ErrorMsg = string.Empty;
                                //上傳之前刪除已有的圖片
                                string oldFileName = OldModel.banner_image;
                                FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFileName))
                                {
                                    FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                    ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                    DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                }
                                FileManagement fileLoad = new FileManagement();
                                //上傳
                                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                if (result)//上傳成功
                                {
                                    model.banner_image = fileName;
                                }

                            }
                            else
                            {
                                model.banner_image = OldModel.banner_image;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->圖片上傳失敗！", ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                        jsonStr = "{success:false,msg:0}";
                        model.banner_image = OldModel.banner_image;
                    }

                    #endregion
                    if (!string.IsNullOrEmpty(Request.Params["banner_url"].ToString()))
                    {
                        model.category_link_url = Request.Params["banner_url"].ToString();
                    }
                    else
                    {
                        model.category_link_url = OldModel.category_link_url;
                    }

                }
                else//url_by=0
                {
                    //刪除上傳的圖片
                    string oldFileName = OldModel.banner_image;
                    FTP ftp = new FTP(imgLocalPath + promoPath, ftpuser, ftppwd);
                    List<string> tem = ftp.GetFileList();
                    if (tem.Contains(oldFileName))
                    {
                        FTP ftps = new FTP(imgLocalPath + promoPath + oldFileName, ftpuser, ftppwd);
                        ftps.DeleteFile(imgLocalPath + promoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                        DeletePicFile(Server.MapPath(imgLocalServerPath + promoPath) + oldFileName);//刪除本地圖片
                    }
                    model.category_link_url = "";
                    model.banner_image = "";
                }

                _promoAmountFareMgr = new PromotionsAmountFareMgr(mySqlConnectionString);
                if (string.IsNullOrEmpty(isEdit))
                {
                    model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.created = DateTime.Now;
                    model.muser = model.kuser;
                    model.modified = model.created;
                    if (_promoAmountFareMgr.ReSave(model) > 0)
                    {
                        jsonStr = "{success:true,msg:0}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:0}";//返回json數據
                    }
                }
                else
                {
                    model.kuser = OldModel.kuser;
                    model.created = OldModel.created;
                    model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.modified = DateTime.Now;
                    if (_promoAmountFareMgr.Update(model, OldModel.event_id) > 0)
                    {
                        jsonStr = "{success:true,msg:0}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:0}";//返回json數據
                    }
                }
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

        #region 刪除 滿件滿額免運 +HttpResponseBase DeletePromoAmountFare()
        /// <summary>
        ///  刪除 滿件滿額免運
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeletePromoAmountFare()
        {
            string jsonStr = String.Empty;
            _promoAmountFareMgr = new PromotionsAmountFareMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            if (_promoAmountFareMgr.Delete(Convert.ToInt32(rid)) > 0)
                            {
                                jsonStr = "{success:true}";
                            }
                            else
                            {
                                jsonStr = "{success:false}";
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
                    jsonStr = "{success:false}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion



        #region 刪除本地上傳的圖片+ void DeletePicFile(string imageName)
        /// <summary>
        /// 刪除本地上傳的圖片
        /// </summary>

        public void DeletePicFile(string imageName)
        {
            try
            {
                if (System.IO.File.Exists(imageName))
                {
                    System.IO.File.Delete(imageName);
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

        #region 創建ftp文件夾 +void CreateFolder(string path, string[] Mappath)
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

        #region 更改活動使用狀態+JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
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

                int id = Convert.ToInt32(Request.Params["id"] ?? "0");
                _promoAmountFareMgr = new PromotionsAmountFareMgr(mySqlConnectionString);
                PromotionsAmountFareQuery model = _promoAmountFareMgr.Select(id);
                if (model.url_by == 1)
                {
                    model.category_link_url = _produCateMgr.GetModelById(model.category_id).category_link_url;
                }
                model.active = Convert.ToBoolean(activeValue);
                model.start = model.start_time;
                model.end = model.end_time;
                model.muser = currentUser;
                model.modified = DateTime.Now;
                if (_promoAmountFareMgr.UpdateActive(model) > 0)
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


    }
}
