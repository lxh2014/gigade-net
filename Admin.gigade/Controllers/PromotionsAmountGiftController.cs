#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountGiftController.cs
* 摘 要：
* 滿額滿件送禮controller
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：2014/6/20 
* 修改歷史：
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：mengjuan0826j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
 *        v1.2修改日期：2015/3/16 
*             修改人員：shuangshuang0420j
*             修改内容：完善否專區時寫入字段
 *                      贈送購物金或抵用券時自動生成gift_id
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

namespace Admin.gigade.Controllers
{ 
    public class PromotionsAmountGiftController : Controller
    {
        //
        // GET: /PromotionsAmountGift/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsAmountGiftImplMgr _promoAmountGiftMgr;
        private IProductCategoryImplMgr _produCateMgr = new ProductCategoryMgr(mySqlConnectionString);
        private IParametersrcImplMgr _parasrcMgr;
        //private IProductCategoryImplMgr _produCateMgr;
        //private IProductCategorySetImplMgr _produCateSetMgr;
        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(mySqlConnectionString);
        private IPromoTicketImplMgr _ptMgr = new PromoTicketMgr(mySqlConnectionString);
        private ISiteImplMgr _siteMgr = new SiteMgr(mySqlConnectionString);

        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑

        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";


        //end 上傳圖片


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TryEatAndDiscount()
        {
            return View();
        }

        #region 獲滿額滿件送禮列表頁 + HttpResponseBase GetPromoAmountGiftList()
        /// <summary>
        /// 獲取滿額滿件列表頁
        /// </summary>
        /// <returns>json數組列表頁</returns>
        [CustomHandleError]
        public HttpResponseBase GetPromoAmountGiftList()
        {
            List<PromotionsAmountGiftQuery> store = new List<PromotionsAmountGiftQuery>();
            string json = string.Empty;
            try
            {
                PromotionsAmountGiftQuery query = new PromotionsAmountGiftQuery();

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
                #endregion

                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);

                int totalCount = 0;

                string type = "'G1','G2'";
                store = _promoAmountGiftMgr.Query(query, out totalCount, type);
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
                    if (!string.IsNullOrEmpty(item.payment_code))
                    {

                        string[] arryPayment = item.payment_code.Split(',');//將payment_code轉化為payment
                        for (int i = 0; i < arryPayment.Length; i++)
                        {
                            if (arryPayment[i] != "0")
                            {
                                _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                                string nameStr = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "payment", Used = 1, ParameterCode = arryPayment[i] }).FirstOrDefault().parameterName;
                                if (!string.IsNullOrEmpty(nameStr))
                                {
                                    item.payment += nameStr;
                                }

                                if (i != arryPayment.Length - 1)
                                {
                                    item.payment += ",";
                                }
                            }

                        }
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;


        }
        #endregion

        #region 保存第一步數據 + HttpResponseBase FirstSaveGift()
        /// <summary>
        /// 保存第一步數據
        /// </summary>
        /// <returns>json數組第一步保存產生的category_id，event_type和event_id</returns>
        [CustomHandleError]
        public HttpResponseBase FirstSaveGift()
        {
            string jsonStr = String.Empty;
            try
            {
                PromotionsAmountGiftQuery model = new PromotionsAmountGiftQuery();

                #region 獲取第一步保存model的數據
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    model.name = Request.Params["name"].ToString();
                }
                else
                {
                    model.name = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["desc"]))
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
                if (!string.IsNullOrEmpty(Request.Params["amount"]))
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"].ToString());
                }
                else
                {
                    model.amount = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["quantity"]))
                {
                    model.quantity = Convert.ToInt32(Request.Params["quantity"].ToString());
                }
                else
                {
                    model.quantity = 0;
                }
                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.created = DateTime.Now;
                model.status = 0;

                if (!string.IsNullOrEmpty(Request.Params["event_type_name"]))
                {
                    //獲取event_type和category_father_id
                    _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                    List<Parametersrc> fatherIdResult = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", parameterName = Request.Params["event_type_name"].ToString(), Used = 1 });
                    if (fatherIdResult.Count == 1)
                    {
                        model.event_type = fatherIdResult[0].ParameterCode;
                        model.category_father_id = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                    }
                }
                else
                {
                    model.event_type = string.Empty;
                    model.category_father_id = 0;
                }
                model.category_ipfrom = Request.UserHostAddress;
                #endregion

                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                int id = _promoAmountGiftMgr.Save(model);
                // DataTable dt = _promoAmountGiftMgr.SelectDt(query);
                PromotionsAmountGift pag = _promoAmountGiftMgr.GetModel(id);

                string event_id = CommonFunction.GetEventId(pag.event_type.ToString(), pag.id.ToString());
                if (pag != null)
                {
                    jsonStr = "{success:true,\"id\":\"" + pag.id + "\",\"cateID\":\"" + pag.category_id + "\",\"event_type\":\"" + pag.event_type + "\",\"event_id\":\"" + event_id + "\"}";
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

        #region 保存第二步數據 + HttpResponseBase SecondSaveGift()
        /// <summary>
        /// 保存第二步數據
        /// </summary>
        /// <returns>json數組 第二步編輯是否成功</returns>
        [CustomHandleError]
        public HttpResponseBase SecondSaveGift()
        {
            string jsonStr = String.Empty;
            try
            {
                string rowID = Request.Params["rowid"].ToString();
                string isEdit = Request.Params["isEdit"].ToString();
                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                PromotionsAmountGiftQuery model = new PromotionsAmountGiftQuery();
                PromotionsAmountGiftQuery OldModel = _promoAmountGiftMgr.Select(Convert.ToInt32(rowID));
                OldModel.event_id = CommonFunction.GetEventId(OldModel.event_type, OldModel.id.ToString());

                #region 獲取數據
                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    model.id = Convert.ToInt32(Request.Params["rowid"]);
                }
                else
                {
                    model.id = OldModel.id;
                }
                model.category_id = OldModel.category_id;
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    model.name = Request.Params["event_name"].ToString();
                }
                else
                {
                    model.name = OldModel.name;
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    model.event_desc = Request.Params["event_desc"].ToString();
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
                if (!string.IsNullOrEmpty(Request.Params["event_type"]))
                {
                    model.event_type = Request.Params["event_type"].ToString();
                }
                else
                {
                    model.event_type = OldModel.event_type;
                }
                if (Request.Params["amount"].ToString() != "" && Request.Params["amount"].ToString() != "0")
                {
                    if (!string.IsNullOrEmpty(Request.Params["amount"]))
                    {

                        model.amount = Convert.ToInt32(Request.Params["amount"].ToString());
                        model.quantity = 0;
                    }
                    else
                    {
                        model.amount = OldModel.amount;
                        model.quantity = 0;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["quantity"]))
                    {
                        model.quantity = Convert.ToInt32(Request.Params["quantity"].ToString());
                        model.amount = 0;
                    }
                    else
                    {
                        model.quantity = OldModel.quantity;
                        model.amount = 0;
                    }
                }

                if (model.id != 0)
                {
                    model.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
                }
                else
                {
                    model.event_id = CommonFunction.GetEventId(OldModel.event_type, OldModel.id.ToString());
                }
                #endregion

                #region 講圖片和鏈接保存至product_category中
                ////講圖片和鏈接保存至product_category中
                if (!string.IsNullOrEmpty(Request.Params["url_by"]))
                {
                    model.url_by = Convert.ToInt32(Request.Params["url_by"].ToString());
                    if (model.url_by == 1)//是專區
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
                            int newRand = rand.Next(1000, 9999);

                            FileManagement fileLoad = new FileManagement();

                            for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                            {
                                HttpPostedFileBase file = Request.Files[iFile];
                                string fileName = string.Empty;//當前文件名

                                string fileExtention = string.Empty;//當前文件的擴展名

                                fileName = Path.GetFileName(file.FileName);

                                // string returnName = imgServerPath;
                                bool result = false;
                                string NewFileName = string.Empty;

                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = model.event_id + newRand + fileExtention;

                                string ServerPath = string.Empty;
                                //判斷目錄是否存在，不存在則創建
                                //string[] mapPath = new string[1];
                                //mapPath[0] = promoPath.Substring(1, promoPath.Length - 2);
                                //string s = localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1);
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                                //  returnName += promoPath + NewFileName;
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

                                try
                                {
                                    //上傳
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)//上傳成功
                                    {
                                        model.banner_image = fileName;
                                    }
                                }
                                catch (Exception)
                                {
                                    model.banner_image = OldModel.banner_image;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            model.banner_image = OldModel.banner_image;
                        }

                        #endregion
                        if (!string.IsNullOrEmpty(Request.Params["banner_url"]))
                        {
                            model.category_link_url = Request.Params["banner_url"].ToString();
                        }
                        else
                        {
                            model.category_link_url = OldModel.category_link_url;
                        }

                        if (!string.IsNullOrEmpty(Request.Params["sbrand_id"].ToString()))
                        {
                            int brandTranId = 0;
                            if (int.TryParse(Request.Params["sbrand_id"].ToString(), out  brandTranId))
                            {
                                model.brand_id = Convert.ToInt32(Request.Params["sbrand_id"].ToString());
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


                        if (!string.IsNullOrEmpty(Request.Params["sclass_id"].ToString()))
                        {
                            int classTranId = 0;
                            if (int.TryParse(Request.Params["sclass_id"].ToString(), out classTranId))
                            {
                                model.class_id = Convert.ToInt32(Request.Params["sclass_id"].ToString());

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

                        if (!string.IsNullOrEmpty(Request.Params["allClass"]))
                        {
                            if (Request.Params["allClass"] == "1")
                            {
                                model.quanguan = 1;
                                model.class_id = 0;
                                model.brand_id = 0;
                                model.product_id = 999999;
                            }
                            else
                            {
                                model.quanguan = 0;
                            }
                        }
                    }
                    else//非專區
                    {
                        model.category_link_url = "";
                        model.banner_image = "";

                        if (!string.IsNullOrEmpty(Request.Params["nobrand_id"].ToString()))
                        {
                            int brandTranId = 0;
                            if (int.TryParse(Request.Params["nobrand_id"].ToString(), out  brandTranId))
                            {
                                model.brand_id = Convert.ToInt32(Request.Params["nobrand_id"].ToString());
                            }
                            else
                            {
                                model.brand_id = OldModel.brand_id;
                            }
                        }
                        else
                        {
                            model.brand_id = OldModel.brand_id;
                        }


                        if (!string.IsNullOrEmpty(Request.Params["noclass_id"].ToString()))
                        {
                            int classTranId = 0;
                            if (int.TryParse(Request.Params["noclass_id"].ToString(), out classTranId))
                            {
                                model.class_id = Convert.ToInt32(Request.Params["noclass_id"].ToString());

                            }
                            else
                            {
                                model.class_id = OldModel.class_id;
                            }
                        }
                        else
                        {
                            model.class_id = OldModel.class_id;
                        }

                        if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                        {
                            model.product_id = Convert.ToInt32(Request.Params["product_id"].ToString());
                        }
                        else
                        {
                            model.product_id = OldModel.product_id;
                        }

                        if (!string.IsNullOrEmpty(Request.Params["noallClass"]))
                        {
                            if (Request.Params["noallClass"] == "1" || Request.Params["noallClass"] == "true")
                            {
                                model.quanguan = 1;
                                model.class_id = 0;
                                model.brand_id = 0;
                                model.product_id = 999999;
                            }
                            else
                            {
                                model.quanguan = 0;
                            }
                        }
                    }
                }
                else
                {
                    model.url_by = OldModel.url_by;
                }
                #endregion

                #region 獲取數據
                if (Request.Params["group_id"].ToString() != "")
                {
                    int groupTryId = 0;
                    if (!string.IsNullOrEmpty(Request.Params["group_id"].ToString()) && int.TryParse(Request.Params["group_id"].ToString(), out groupTryId))
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
                    if (!string.IsNullOrEmpty(Request.Params["condition_id"]))//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    else
                    {
                        model.condition_id = OldModel.condition_id;
                    }
                    model.group_id = 0;
                }


                if (!string.IsNullOrEmpty(Request.Params["count_by"]))
                {
                    model.count_by = Convert.ToInt32(Request.Params["count_by"].ToString());
                }
                else
                {
                    model.count_by = OldModel.count_by;
                }
                if (!string.IsNullOrEmpty(Request.Params["numLimit"]))
                {
                    model.num_limit = Convert.ToInt32(Request.Params["numLimit"].ToString());
                }
                else
                {
                    model.num_limit = OldModel.num_limit;
                }
                if (!string.IsNullOrEmpty(Request.Params["IsRepeat"]))
                {
                    model.repeat = Convert.ToInt32(Request.Params["IsRepeat"].ToString()) == 1 ? true : false;
                }
                else
                {
                    model.repeat = OldModel.repeat;
                }
                if (!string.IsNullOrEmpty(Request.Params["frieghttype"].ToString()))
                {
                    int typeId = 0;
                    if (int.TryParse(Request.Params["frieghttype"].ToString(), out typeId))
                    {
                        model.type = Convert.ToInt32(Request.Params["frieghttype"].ToString());
                    }
                    else
                    {
                        model.type = OldModel.type;
                    }
                }
                else
                {
                    model.type = OldModel.type;
                }
                if (!string.IsNullOrEmpty(Request.Params["devicename"]))
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

                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    model.startdate = Convert.ToDateTime(Request.Params["start_time"].ToString());
                }
                else
                {
                    model.startdate = OldModel.startdate;
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    model.enddate = Convert.ToDateTime(Request.Params["end_time"].ToString());
                }
                else
                {
                    model.enddate = OldModel.enddate;
                }
                //int siteTranId = 0;
                //if (!string.IsNullOrEmpty(Request.Params["site"].ToString()) && int.TryParse(Request.Params["site"].ToString(), out siteTranId))//site
                //{
                //    model.site = Convert.ToInt32(Request.Params["site"].ToString());
                //}
                //else
                //{
                //    model.site = OldModel.site;
                //}
                if (!string.IsNullOrEmpty(Request.Params["site"].ToString()))//修改時傳的值為siteName
                {

                    Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
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

                #region 贈品處理
                if (!string.IsNullOrEmpty(Request.Params["gift_type"]))//gift_type when 1 then '商品' WHEN 2 then '機會' when 3  then '購物金' when 4 then '抵用券' 
                {
                    model.gift_type = Convert.ToInt32(Request.Params["gift_type"].ToString());
                    if (OldModel.gift_type == 2)
                    {
                        if (model.gift_type == 2)
                        {
                            if (OldModel.ticket_id != 0)
                            {
                                model.ticket_id = OldModel.ticket_id;
                            }
                            else
                            {
                                model.ticket_id = 0;
                            }
                            model.ticket_name = model.name;
                        }
                        else
                        {
                            if (_ptMgr.Delete(OldModel.ticket_id) > 0)
                            {
                                jsonStr = "{success:success}";
                                model.ticket_id = 0;
                                model.ticket_name = "";
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:'promo_ticket刪除出錯！'}";
                                this.Response.Clear();
                                this.Response.Write(jsonStr.ToString());
                                this.Response.End();
                                return this.Response;
                            }
                        }
                    }
                    if (model.gift_type == 1)
                    {
                        model.gift_id = Convert.ToInt32(Request.Params["gift_id"].ToString());
                        model.gift_product_number = Convert.ToInt32(Request.Params["gift_product_number"].ToString());
                    }
                    else if (model.gift_type == 3)
                    {
                        model.bonus_type = 1;
                    }
                    else if (model.gift_type == 4)
                    {
                        model.bonus_type = 2;
                    }



                    if (!string.IsNullOrEmpty(Request.Params["activeNow"]))
                    {
                        model.active_now = Convert.ToInt32(Request.Params["activeNow"].ToString());
                    }
                    else
                    {
                        model.active_now = OldModel.active_now;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                    {
                        model.use_start = Convert.ToDateTime(Request.Params["startdate"].ToString());
                    }
                    else
                    {

                        model.use_start = OldModel.use_start;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                    {
                        model.use_end = Convert.ToDateTime(Request.Params["enddate"].ToString());
                    }
                    else
                    {

                        model.use_end = OldModel.use_end;
                    }
                    try
                    {
                        TimeSpan ts1 = new TimeSpan(model.use_end.Ticks);
                        TimeSpan ts2 = new TimeSpan(model.use_start.Ticks);
                        TimeSpan ts3 = ts1.Subtract(ts2).Duration();
                        int isAddDay = ts3.Hours > 0 ? 1 : 0;
                        model.valid_interval = ts3.Days + isAddDay;
                    }
                    catch
                    {
                        model.valid_interval = OldModel.valid_interval;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bonusAmount"]))
                    {
                        model.deduct_welfare = Convert.ToUInt32(Request.Params["bonusAmount"].ToString());
                    }
                    else
                    {
                        model.deduct_welfare = OldModel.deduct_welfare;
                    }
                }
                else
                {
                    model.gift_type = OldModel.gift_type;
                    model.ticket_id = OldModel.ticket_id;
                    model.ticket_name = OldModel.ticket_name;
                }
                #endregion

                model.status = 1;


                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                if (string.IsNullOrEmpty(isEdit))
                {
                    model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.created = DateTime.Now;
                    model.muser = model.kuser;
                    model.modified = model.created;
                    if (_promoAmountGiftMgr.Update(model, OldModel.event_id) > 0)
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

                    if (_promoAmountGiftMgr.Update(model, OldModel.event_id) > 0)
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
                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                PromotionsAmountGiftQuery model = _promoAmountGiftMgr.Select(id);
                if (model.url_by == 1)
                {
                    model.category_link_url = _produCateMgr.GetModelById(model.category_id).category_link_url;
                }
                model.active = activeValue;
                model.id = id;
                model.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
                model.start = model.startdate;
                model.end = model.enddate;
                model.muser = currentUser;
                model.modified = DateTime.Now;

                if (_promoAmountGiftMgr.UpdateActive(model) > 0)
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

        #region 刪除列表頁數據+HttpResponseBase DeletePromoAmountGift()
        /// <summary>
        /// 刪除列表頁數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeletePromoAmountGift()
        {
            PromotionsAmountGift pag = new PromotionsAmountGift();
            string jsonStr = String.Empty;
            _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
                    {
                        if (rid != null)
                        {
                            pag = _promoAmountGiftMgr.GetModel(Convert.ToInt32(rid));
                            var eventid = CommonFunction.GetEventId(pag.event_type, pag.id.ToString());

                            if (!string.IsNullOrEmpty(rid))
                            {
                                if (_promoAmountGiftMgr.Delete(Convert.ToInt32(rid), eventid) > 0)
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
                    jsonStr = "{success:true}";
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

        #region 獲試吃紅利折抵列表頁 + HttpResponseBase GetTryEatDiscountList()
        /// <summary>
        /// 獲取滿額滿件列表頁
        /// </summary>
        /// <returns>json數組列表頁</returns>
        [CustomHandleError]
        public HttpResponseBase GetTryEatDiscountList()
        {
            List<PromotionsAmountGiftQuery> store = new List<PromotionsAmountGiftQuery>();
            string json = string.Empty;
            try
            {
                PromotionsAmountGiftQuery query = new PromotionsAmountGiftQuery();
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
                    query.selcon = Request.Params["serchcontent"];
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.start = DateTime.Parse(Request.Params["timestart"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.end = DateTime.Parse(Request.Params["timeend"]);
                }
                #endregion

                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);

                int totalCount = 0;
                string type = "'G3','G0','G4'";
                store = _promoAmountGiftMgr.Query(query, out totalCount, type);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                foreach (var item in store)
                {
                    if (!string.IsNullOrEmpty(item.delivery_category.ToString()))
                    {
                        string[] arrySite = item.delivery_category.Split(',');//將siteid轉化為site_name
                        item.delivery_category = "";
                        for (int i = 0; i < arrySite.Length; i++)
                        {
                            Site querySite = new Site();
                            querySite.Site_Id = Convert.ToUInt32(arrySite[i]);
                            item.delivery_category += _siteMgr.Query(querySite).FirstOrDefault().Site_Name;
                            if (i != arrySite.Length - 1)
                            {
                                item.delivery_category += ",";
                            }
                        }
                    }
                    if (item.banner_image != "")
                    {
                        item.banner_image = imgServerPath + promoPath + item.banner_image;
                    }
                    else
                    {
                        item.banner_image = defaultImg;
                    }
                    if (!string.IsNullOrEmpty(item.payment_code))
                    {
                        _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                        string[] arryPayment = item.payment_code.Split(',');//將payment_code轉化為payment
                        for (int i = 0; i < arryPayment.Length; i++)
                        {
                            if (arryPayment[i] != "0")
                            {
                                string nameStr = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "payment", Used = 1, ParameterCode = arryPayment[i] }).FirstOrDefault().parameterName;
                                if (!string.IsNullOrEmpty(nameStr))
                                {
                                    item.payment += nameStr;
                                }

                                if (i != arryPayment.Length - 1)
                                {
                                    item.payment += ",";
                                }
                            }
                        }
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 試吃/紅利折抵的保存和編輯
        #region 保存第一步數據 + HttpResponseBase TryEatAndDiscountFirstSaveGift()
        [CustomHandleError]
        public HttpResponseBase TryEatAndDiscountFirstSaveGift()
        {
            string jsonStr = String.Empty;
            try
            {
                PromotionsAmountGiftQuery model = new PromotionsAmountGiftQuery();

                #region 獲取第一步保存model的數據
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    model.name = Request.Params["name"].ToString();
                }
                else
                {
                    model.name = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["desc"]))
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
                if (!string.IsNullOrEmpty(Request.Params["bonus_state"]))//0：一般 1:試吃 2：紅利折抵
                {
                    switch (Request.Params["bonus_state"].ToString())
                    {
                        case "0":
                            model.bonus_state = 0;
                            model.dividend = 0;
                            break;
                        case "1":
                            model.bonus_state = 1;
                            model.dividend = 0;
                            if (!string.IsNullOrEmpty(Request.Params["freight_price"]))
                            {
                                model.freight_price = Convert.ToInt32(Request.Params["freight_price"].ToString());
                            }
                            else
                            {
                                model.freight_price = 0;
                            }
                            break;
                        case "2":
                            model.bonus_state = 2;
                            if (!string.IsNullOrEmpty(Request.Params["Points"]))
                            {
                                model.dividend = Convert.ToInt32(Request.Params["Points"]);
                                if (model.dividend == 4)
                                {
                                    if (!string.IsNullOrEmpty(Request.Params["point"]))//此處要做出修改
                                    {
                                        model.point = Convert.ToInt32(Request.Params["point"].ToString());
                                    }
                                    else
                                    {
                                        model.point = 0;
                                    }
                                    if (!string.IsNullOrEmpty(Request.Params["dollar"]))//此處要做出修改
                                    {
                                        model.dollar = Convert.ToInt32(Request.Params["dollar"].ToString());
                                    }
                                    else
                                    {
                                        model.dollar = 0;
                                    }
                                }
                                else if (model.dividend == 5)
                                {
                                    model.point = 0;
                                    if (!string.IsNullOrEmpty(Request.Params["dollar"]))//此處要做出修改
                                    {
                                        model.dollar = Convert.ToInt32(Request.Params["dollar"].ToString());
                                    }
                                    else
                                    {
                                        model.dollar = 0;
                                    }
                                }
                                else
                                {
                                    model.point = 0;
                                    model.dollar = 0;
                                }
                            }
                            break;
                    }
                }


                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.created = DateTime.Now;
                model.status = 0;

                if (!string.IsNullOrEmpty(Request.Params["event_type_name"]))
                {
                    //獲取event_type和category_father_id
                    _parasrcMgr = new ParameterMgr(mySqlConnectionString);
                    List<Parametersrc> fatherIdResult = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", parameterName = Request.Params["event_type_name"].ToString(), Used = 1 });
                    if (fatherIdResult.Count == 1)
                    {
                        model.event_type = fatherIdResult[0].ParameterCode;
                        model.category_father_id = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                    }
                }
                else
                {
                    model.event_type = string.Empty;
                    model.category_father_id = 0;
                }
                model.category_ipfrom = Request.UserHostAddress;
                #endregion

                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                PromotionsAmountGift query = new PromotionsAmountGift();
                int id = _promoAmountGiftMgr.TryEatAndDiscountSave(model);
                //DataTable dt = _promoAmountGiftMgr.SelectDt(query);
                PromotionsAmountGift pag = _promoAmountGiftMgr.GetModel(id);
                string event_id = CommonFunction.GetEventId(pag.event_type, pag.id.ToString());
                if (pag != null)
                {
                    jsonStr = "{success:true,\"id\":\"" + pag.id + "\",\"cateID\":\"" + pag.category_id + "\",\"event_type\":\"" + pag.event_type + "\",\"event_id\":\"" + event_id + "\"}";
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
        #region 保存第二步數據 + HttpResponseBase TryEatAndDiscountSecondSaveGift()
        [CustomHandleError]
        public HttpResponseBase TryEatAndDiscountSecondSaveGift()
        {
            string jsonStr = String.Empty;
            try
            {
                string rowID = Request.Params["rowid"].ToString();
                string isEdit = Request.Params["isEdit"].ToString();
                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                PromotionsAmountGiftQuery model = new PromotionsAmountGiftQuery();
                PromotionsAmountGiftQuery OldModel = _promoAmountGiftMgr.Select(Convert.ToInt32(rowID));
                OldModel.event_id = CommonFunction.GetEventId(OldModel.event_type, OldModel.id.ToString());

                #region 獲取數據
                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    model.id = Convert.ToInt32(Request.Params["rowid"]);
                }
                else
                {
                    model.id = OldModel.id;
                }
                model.category_id = OldModel.category_id;
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    model.name = Request.Params["event_name"].ToString();
                }
                else
                {
                    model.name = OldModel.name;
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    model.event_desc = Request.Params["event_desc"].ToString();
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
                if (!string.IsNullOrEmpty(Request.Params["event_type"]))
                {
                    model.event_type = Request.Params["event_type"].ToString();
                }
                else
                {
                    model.event_type = OldModel.event_type;
                }

                if (!string.IsNullOrEmpty(Request.Params["bonus_state"]))//0：一般 1:試吃 2：紅利折抵
                {
                    switch (Request.Params["bonus_state"].ToString())
                    {
                        case "0":
                            model.bonus_state = 0;
                            model.dividend = 0;
                            break;
                        case "1":
                            model.bonus_state = 1;
                            model.dividend = 0;
                            if (Request.Params["freight_price"].ToString() != "" && Request.Params["freight_price"].ToString() != "0")
                            {
                                model.freight_price = Convert.ToInt32(Request.Params["freight_price"].ToString());
                            }
                            else
                            {
                                model.freight_price = OldModel.freight_price;
                            }
                            break;
                        case "2":
                            model.bonus_state = 2;
                            if (!string.IsNullOrEmpty(Request.Params["Points"]))
                            {
                                model.dividend = Convert.ToInt32(Request.Params["Points"]);
                                if (model.dividend == 4)
                                {
                                    if (!string.IsNullOrEmpty(Request.Params["point"]))//此處要做出修改
                                    {
                                        model.point = Convert.ToInt32(Request.Params["point"].ToString());
                                    }
                                    else
                                    {
                                        model.point = 0;
                                    }
                                    if (!string.IsNullOrEmpty(Request.Params["dollar"]))//此處要做出修改
                                    {
                                        model.dollar = Convert.ToInt32(Request.Params["dollar"].ToString());
                                    }
                                    else
                                    {
                                        model.dollar = 0;
                                    }
                                }
                                else if (model.dividend == 5)
                                {
                                    model.point = 0;
                                    if (!string.IsNullOrEmpty(Request.Params["dollar"]))//此處要做出修改
                                    {
                                        model.dollar = Convert.ToInt32(Request.Params["dollar"].ToString());
                                    }
                                    else
                                    {
                                        model.dollar = 0;
                                    }
                                }
                                else
                                {
                                    model.point = 0;
                                    model.dollar = 0;
                                }
                            }
                            break;
                    }
                }
                if (model.id != 0)
                {
                    model.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
                }
                else
                {
                    model.event_id = CommonFunction.GetEventId(OldModel.event_type, OldModel.id.ToString());
                }
                #endregion

                #region 講圖片和鏈接保存至product_category中
                ////講圖片和鏈接保存至product_category中
                if (!string.IsNullOrEmpty(Request.Params["url_by"]))
                {
                    model.url_by = Convert.ToInt32(Request.Params["url_by"].ToString());
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
                            int newRand = rand.Next(1000, 9999);

                            FileManagement fileLoad = new FileManagement();

                            for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                            {
                                HttpPostedFileBase file = Request.Files[iFile];
                                string fileName = string.Empty;//當前文件名

                                string fileExtention = string.Empty;//當前文件的擴展名

                                fileName = Path.GetFileName(file.FileName);

                                // string returnName = imgServerPath;
                                bool result = false;
                                string NewFileName = string.Empty;

                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = model.event_id + newRand + fileExtention;

                                string ServerPath = string.Empty;
                                //判斷目錄是否存在，不存在則創建
                                //string[] mapPath = new string[1];
                                //mapPath[0] = promoPath.Substring(1, promoPath.Length - 2);
                                //string s = localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1);
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                                //  returnName += promoPath + NewFileName;
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

                                try
                                {
                                    //上傳
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)//上傳成功
                                    {
                                        model.banner_image = fileName;
                                    }
                                }
                                catch (Exception)
                                {
                                    model.banner_image = OldModel.banner_image;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            model.banner_image = OldModel.banner_image;
                        }

                        #endregion
                        if (!string.IsNullOrEmpty(Request.Params["banner_url"]))
                        {
                            model.category_link_url = Request.Params["banner_url"].ToString();
                        }
                        else
                        {
                            model.category_link_url = OldModel.category_link_url;
                        }
                    }
                    else
                    {
                        model.category_link_url = "";
                        model.banner_image = "";
                    }
                }
                else
                {
                    model.url_by = OldModel.url_by;
                }
                #endregion

                #region 獲取數據
                if (Request.Params["group_id"].ToString() != "")
                {
                    int groupTryId = 0;
                    if (!string.IsNullOrEmpty(Request.Params["group_id"].ToString()) && int.TryParse(Request.Params["group_id"].ToString(), out groupTryId))
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
                    if (!string.IsNullOrEmpty(Request.Params["condition_id"]))//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    else
                    {
                        model.condition_id = OldModel.condition_id;
                    }
                    model.group_id = 0;
                }

                if (!string.IsNullOrEmpty(Request.Params["sbrand_id"].ToString()))
                {
                    int brandTranId = 0;
                    if (int.TryParse(Request.Params["sbrand_id"].ToString(), out  brandTranId))
                    {
                        model.brand_id = Convert.ToInt32(Request.Params["sbrand_id"].ToString());
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



                if (!string.IsNullOrEmpty(Request.Params["sclass_id"].ToString()))
                {
                    int classTranId = 0;
                    if (int.TryParse(Request.Params["sclass_id"].ToString(), out classTranId))
                    {
                        model.class_id = Convert.ToInt32(Request.Params["sclass_id"].ToString());

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

                if (!string.IsNullOrEmpty(Request.Params["allClass"]))
                {
                    if (Request.Params["allClass"] == "1")
                    {
                        model.quanguan = 1;
                        model.class_id = 0;
                        model.brand_id = 0;
                        model.product_id = 999999;
                    }
                    else
                    {
                        model.quanguan = 0;
                    }
                }
                else
                {

                }
                if (!string.IsNullOrEmpty(Request.Params["count_by"]))
                {
                    model.count_by = Convert.ToInt32(Request.Params["count_by"].ToString());
                }
                else
                {
                    model.count_by = OldModel.count_by;
                }
                if (!string.IsNullOrEmpty(Request.Params["numLimit"]))
                {
                    model.num_limit = Convert.ToInt32(Request.Params["numLimit"].ToString());
                }
                else
                {
                    model.num_limit = OldModel.num_limit;
                }
                if (!string.IsNullOrEmpty(Request.Params["gift_mundane"]))
                {
                    model.gift_mundane = Convert.ToInt32(Request.Params["gift_mundane"].ToString());
                }
                else
                {
                    model.gift_mundane = OldModel.gift_mundane;
                }

                if (!string.IsNullOrEmpty(Request.Params["IsRepeat"]))
                {
                    model.repeat = Convert.ToInt32(Request.Params["IsRepeat"].ToString()) == 1 ? true : false;
                }
                else
                {
                    model.repeat = OldModel.repeat;
                }
                if (!string.IsNullOrEmpty(Request.Params["frieghttype"].ToString()))
                {
                    int typeId = 0;
                    if (int.TryParse(Request.Params["frieghttype"].ToString(), out typeId))
                    {
                        model.type = Convert.ToInt32(Request.Params["frieghttype"].ToString());
                    }
                    else
                    {
                        model.type = OldModel.type;
                    }
                }
                else
                {
                    model.type = OldModel.type;
                }
                if (!string.IsNullOrEmpty(Request.Params["devicename"]))
                {
                    model.device = Convert.ToInt32(Request.Params["devicename"].ToString());
                }
                else
                {
                    model.device = OldModel.device;
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"].ToString()))
                {
                    Regex reg = new Regex("^([0-9]+,)*([0-9]+)$");   //1,14
                    if (reg.IsMatch(Request.Params["payment"].ToString()))
                    {
                        model.payment_code = Request.Params["payment"].ToString();
                    }
                    else
                    {
                        model.payment_code = OldModel.payment_code;
                    }
                }
                else
                {
                    model.payment_code = OldModel.payment_code;
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    model.startdate = Convert.ToDateTime(Request.Params["start_time"].ToString());
                }
                else
                {
                    model.startdate = OldModel.startdate;
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    model.enddate = Convert.ToDateTime(Request.Params["end_time"].ToString());
                }
                else
                {
                    model.enddate = OldModel.enddate;
                }

                if (!string.IsNullOrEmpty(Request.Params["site"].ToString()))//修改時傳的值為siteName
                {

                    string site = Request.Params["site"].ToString();
                    Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
                    if (reg.IsMatch(site))
                    {
                        model.site = site;// 將站台改為多選 edit by shuangshuang0420j 20140925 10:08
                    }
                    else
                    {
                        if (site.LastIndexOf(',') == site.Length - 1)//最後一個字符為，時
                        {
                            model.site = site.Substring(0, site.Length - 1);

                        }
                        else
                        {
                            model.site = OldModel.site;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["delivery_category"].ToString()))//修改時傳的值為siteName
                {

                    Regex rega = new Regex("^([0-9]+,)*([0-9]+)$");
                    if (rega.IsMatch(Request.Params["delivery_category"].ToString()))
                    {
                        model.delivery_category = Request.Params["delivery_category"].ToString();// 將站台改為多選 edit by shuangshuang0420j 20140925 10:08
                    }
                    else
                    {
                        model.delivery_category = OldModel.delivery_category;
                    }
                }
                #endregion


                model.status = 1;


                _promoAmountGiftMgr = new PromotionsAmountGiftMgr(mySqlConnectionString);
                if (string.IsNullOrEmpty(isEdit))
                {
                    model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.created = DateTime.Now;
                    model.muser = model.kuser;
                    model.modified = model.created;
                    if (_promoAmountGiftMgr.TryEatAndDiscountUpdate(model, OldModel.event_id) > 0)
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

                    if (_promoAmountGiftMgr.TryEatAndDiscountUpdate(model, OldModel.event_id) > 0)
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
        #endregion
    }
}
