
#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsDiscountController.cs      
* 摘 要：                                                                               
* 幾件幾元、幾件幾折列表顯示和後台方法處理
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/6/20 
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：規範代碼結構，完善異常拋出，添加注釋
 *        v1.2修改日期：2015/3/16 
*             修改人員：shuangshuang0420j
*             修改内容：完善否專區時寫入字段
 *                      
*/

#endregion
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Mgr.Impl;
using Admin.gigade.CustomError;
using BLL.gigade.Model;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;
using BLL.gigade.Common;
using System.IO;
using BLL.gigade.Model.Custom;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Admin.gigade.Controllers
{
    public class PromotionsDiscountController : Controller
    {
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private IVipUserGroupImplMgr _vipGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
        private IPromoDiscountImplMgr _promoDiscountMgr = new PromoDiscountMgr(mySqlConnectionString);
        private ISiteImplMgr _siteMgr = new SiteMgr(mySqlConnectionString);
        private IPromotionsAmountDiscountImplMgr _promoAmountDiscountMgr = new PromotionsAmountDiscountMgr(mySqlConnectionString);
        private IProductCategoryImplMgr _produCateMgr = new ProductCategoryMgr(mySqlConnectionString);
        private IParametersrcImplMgr _parasrcMgr = new ParameterMgr(mySqlConnectionString);
        private IProductCategorySetImplMgr _produCateSetMgr = new ProductCategorySetMgr(mySqlConnectionString);
        private IProdPromoImplMgr _prodPromoMgr = new ProdPromoMgr(mySqlConnectionString);
        private IPromoAllImplMgr _promAllMgr = new PromoAllMgr(mySqlConnectionString);
        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(mySqlConnectionString);
        private IProductImplMgr _prodMgr;
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        //string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑

        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        private ISerialImplMgr _seriMgr = new SerialMgr(mySqlConnectionString);

        //end 上傳圖片

        #region 視圖
        public ActionResult Index()
        {
            return View();
        }
        //幾件幾元 
        public ActionResult FewPieceFewYuan()
        {
            return View();
        }
        //幾件幾折
        public ActionResult FewPieceFewDiscount()
        {
            return View();
        }
        #endregion

        #region 獲取幾件幾元列表頁ok+ HttpResponseBase DiscountList()
        /// <summary>
        /// 獲取幾件幾元列表頁
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase DiscountList()
        {
            List<PromotionAmountDiscountQuery> store = new List<PromotionAmountDiscountQuery>();
            string json = string.Empty;
            try
            {
                PromotionAmountDiscountQuery query = new PromotionAmountDiscountQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Form["limit"]))
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
                query.event_type = "M2";
                query.url_by = -1;
                int totalCount = 0;
                store = _promoAmountDiscountMgr.Query(query, out totalCount, query.event_type);
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

        #region 獲取幾件幾折列表頁 + HttpResponseBase DiscountZheList()
        /// <summary>
        /// 獲取幾件幾折列表頁
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase DiscountZheList()
        {
            List<PromotionAmountDiscountQuery> store = new List<PromotionAmountDiscountQuery>();
            string json = string.Empty;
            try
            {
                PromotionAmountDiscountQuery query = new PromotionAmountDiscountQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["expiredSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["expiredSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isurl"]))
                {
                    query.url_by = Convert.ToInt32(Request.Params["isurl"]);
                }
                else
                {
                    query.url_by = -1;
                }

                query.event_type = "M1";
                int totalCount = 0;
                store = _promoAmountDiscountMgr.Query(query, out totalCount, query.event_type);
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

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
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

        #region 獲得活動id和類別idok + HttpResponseBase SavePromotionsAmountDiscount()
        /// <summary>
        /// 獲得活動id和類別id
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SavePromotionsAmountDiscount()
        {
            string jsonStr = String.Empty;
            try
            {
                PromotionsAmountDiscount model = new PromotionsAmountDiscount();
                if (!string.IsNullOrEmpty(Request.Params["name"].ToString()))
                {
                    model.name = Request.Params["name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["desc"].ToString()))
                {
                    model.event_desc = Request.Params["desc"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["e_type"].ToString()))
                {
                    model.event_type = Request.Params["e_type"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["devicename"].ToString()))
                {
                    model.device = Convert.ToInt32(Request.Params["devicename"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["start_date"].ToString()))
                {
                    model.start = Convert.ToDateTime(Request.Params["start_date"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["end_date"].ToString()))
                {
                    model.end = Convert.ToDateTime(Request.Params["end_date"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["vendor_coverage"].ToString()))
                {
                    model.vendor_coverage = Convert.ToInt32(Request.Params["vendor_coverage"].ToString());
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
                        model.site = site.TrimEnd(',');
                    }
                }

                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.created = DateTime.Now;
                model.muser = model.kuser;
                model.modified = model.created;
                model.active = false;//不啟用
                model.status = 0;

                #region 保存第一步到product_category 獲取prodduct_amount_discount的category_id


                ProductCategory pmodel = new ProductCategory();

                pmodel.category_name = model.name;

                //獲取category_father_id
                List<Parametersrc> fatherIdResult = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", Used = 1, ParameterCode = model.event_type });
                if (fatherIdResult.Count != 0)
                {
                    pmodel.category_father_id = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                }
                else
                {
                    pmodel.category_father_id = 0;
                }
                pmodel.category_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_updatedate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());// Request.UserHostAddress;
                pmodel.category_display = Convert.ToUInt32(model.status);
                model.category_id = Convert.ToUInt32(_produCateMgr.Save(pmodel));

                #endregion

                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 12;
                serial.Serial_Value = Convert.ToUInt32(model.category_id);
                if (_seriMgr.Update(serial) > 0)
                {
                    System.Data.DataTable query = _promoAmountDiscountMgr.Save(model);

                    if (query != null)
                    {

                        jsonStr = "{success:true,id:" + query.Rows[0]["id"] + ",cateID:" + query.Rows[0]["category_id"] + "}";
                    }
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

        #region 保存活動的其他欄位 + HttpResponseBase SavePromotionsAmountDiscount()
        /// <summary>
        /// 獲得活動id和類別id,并補全數據
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SavePromotionsAmountDiscountTwo()
        {
            try
            {
                string jsonStr = string.Empty;
                PromotionsAmountDiscount model = new PromotionsAmountDiscount();
                ProductCategory pmodel = new ProductCategory();
                int isTryInt = 0;
                #region 獲取新增時promotionamountdiscount和productcategory model值
                if (!string.IsNullOrEmpty(Request.Params["pid"].ToString()))
                {
                    model.id = Convert.ToInt32(Request.Params["pid"].ToString());
                }

                PromotionsAmountDiscountCustom oldermodel = _promoAmountDiscountMgr.GetModelById(model.id);
                model.category_id = oldermodel.category_id;
                pmodel.category_id = Convert.ToUInt32(oldermodel.category_id);

                ProductCategory olderpcmodel = _produCateMgr.GetModelById(pmodel.category_id);

                PromotionsAmountDiscountCustom pacdModel = new PromotionsAmountDiscountCustom();
                pacdModel.id = model.id;
                pacdModel.category_id = model.category_id;

                if (!string.IsNullOrEmpty(Request.Params["name"].ToString()))
                {
                    pacdModel.name = Request.Params["name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["desc"].ToString()))
                {
                    pacdModel.event_desc = Request.Params["desc"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["e_type"].ToString()))
                {
                    pacdModel.event_type = Request.Params["e_type"].ToString();
                }
                pacdModel.event_id = CommonFunction.GetEventId(pacdModel.event_type.ToString(), pacdModel.id.ToString());
                if (!string.IsNullOrEmpty(Request.Params["url_by"].ToString()))
                {
                    pacdModel.url_by = Convert.ToInt32(Request.Params["url_by"].ToString());
                }
                else
                {
                    pacdModel.url_by = oldermodel.url_by;
                }
                if (pacdModel.url_by == 1)
                {

                    #region 上傳圖片

                    try
                    {
                        if (Request.Files.Count != 0)
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
                            //生成隨機數，用於圖片的重命名
                            Random rand = new Random();
                            int newRand = rand.Next(1000, 9999);



                            //獲取上傳的圖片
                            HttpPostedFileBase file = Request.Files[0];
                            string fileName = string.Empty;//當前文件名
                            string fileExtention = string.Empty;//當前文件的擴展名

                            fileName = Path.GetFileName(file.FileName);
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                bool result = false;
                                string NewFileName = string.Empty;

                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = pacdModel.event_id + newRand + fileExtention;//圖片重命名為event_id+4位隨機數+擴展名

                                string ServerPath = string.Empty;
                                //判斷目錄是否存在，不存在則創建
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                                //  returnName += promoPath + NewFileName;
                                fileName = NewFileName;
                                NewFileName = localPromoPath + NewFileName;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                string ErrorMsg = string.Empty;

                                //上傳之前刪除已有的圖片
                                string oldFileName = olderpcmodel.banner_image;
                                FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFileName))
                                {
                                    //FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                    //ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                    //DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                }
                                try
                                {
                                    //上傳
                                    FileManagement fileLoad = new FileManagement();
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)//上傳成功
                                    {
                                        pacdModel.banner_image = fileName;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->fileLoad.UpLoadFile()", ex.Source, ex.Message);
                                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    log.Error(logMessage);
                                    jsonStr = "{success:false,msg:'圖片上傳失敗'}";

                                    pacdModel.banner_image = olderpcmodel.banner_image;
                                }
                            }
                            else
                            {
                                pacdModel.banner_image = olderpcmodel.banner_image;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->圖片上傳失敗！", ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                        jsonStr = "{success:false,msg:'圖片上傳失敗'}";

                        pacdModel.banner_image = olderpcmodel.banner_image;
                    }


                    #endregion

                    if (!string.IsNullOrEmpty(Request.Params["banner_url"].ToString()))//存連接地址
                    {
                        pacdModel.category_link_url = Request.Params["banner_url"].ToString();
                    }
                    else
                    {
                        pacdModel.category_link_url = olderpcmodel.category_link_url;
                    }

                    if (!string.IsNullOrEmpty(Request.Params["sbrand_id"].ToString()))
                    {
                        int brandTranId = 0;
                        if (int.TryParse(Request.Params["sbrand_id"].ToString(), out brandTranId))
                        {
                            pacdModel.brand_id = Convert.ToInt32(Request.Params["sbrand_id"].ToString());
                        }
                        else
                        {
                            pacdModel.brand_id = oldermodel.brand_id;
                        }
                    }
                    else
                    {
                        pacdModel.brand_id = 0;
                    }


                    if (!string.IsNullOrEmpty(Request.Params["sclass_id"].ToString()))
                    {
                        int classTranId = 0;
                        if (int.TryParse(Request.Params["sclass_id"].ToString(), out classTranId))
                        {
                            pacdModel.class_id = Convert.ToInt32(Request.Params["sclass_id"].ToString());

                        }
                        else
                        {
                            pacdModel.class_id = oldermodel.class_id;
                        }
                    }
                    else
                    {
                        pacdModel.class_id = 0;
                    }
                    if (Request.Params["allClass"].ToString() == "1")//是否全館
                    {
                        string allClass = Request.Params["allClass"];
                        pacdModel.brand_id = 0;
                        pacdModel.class_id = 0;
                        pacdModel.product_id = 999999;


                    }
                }
                else
                {
                    //刪除上傳的圖片
                    //string oldFileName = olderpcmodel.banner_image;
                    //FTP ftp = new FTP(imgLocalPath + promoPath, ftpuser, ftppwd);
                    //List<string> tem = ftp.GetFileList();
                    //if (tem.Contains(oldFileName))
                    //{
                    //FTP ftps = new FTP(imgLocalPath + promoPath + oldFileName, ftpuser, ftppwd);
                    //ftps.DeleteFile(imgLocalPath + promoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                    // DeletePicFile(Server.MapPath(imgLocalServerPath + promoPath) + oldFileName);//刪除本地圖片
                    // }

                    pacdModel.banner_image = "";
                    pacdModel.category_link_url = "";
                    if (!string.IsNullOrEmpty(Request.Params["noclass_id"]) && int.TryParse(Request.Params["noclass_id"].ToString(), out isTryInt))
                    {
                        pacdModel.class_id = Convert.ToInt32(Request.Params["noclass_id"]);
                    }
                    else
                    {
                        pacdModel.class_id = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["nobrand_id"]) && int.TryParse(Request.Params["nobrand_id"].ToString(), out isTryInt))
                    {
                        pacdModel.brand_id = Convert.ToInt32(Request.Params["nobrand_id"]);
                    }
                    else
                    {
                        pacdModel.brand_id =0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                    {
                        pacdModel.product_id = Convert.ToInt32(Request.Params["product_id"]);
                    }
                    else
                    {
                        pacdModel.product_id = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["no_allClass"]))
                    {
                        if (Request.Params["no_allClass"].ToString() == "1")//是否全館
                        {
                            string allClass = Request.Params["no_allClass"];
                            pacdModel.brand_id = 0;
                            pacdModel.class_id = 0;
                            pacdModel.product_id = 999999;


                        }
                    }
                    if (!string.IsNullOrEmpty(Request.Params["no_amount"]))
                    {
                        pacdModel.amount = Convert.ToInt32(Request.Params["no_amount"]);
                    }
                    else
                    {
                        pacdModel.amount = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["no_discount"]))
                    {
                        pacdModel.discount = Convert.ToInt32(Request.Params["no_discount"]);
                    }
                    else
                    {
                        pacdModel.discount = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["no_quantity"]))
                    {
                        pacdModel.quantity = Convert.ToInt32(Request.Params["no_quantity"]);
                    }
                    else
                    {
                        pacdModel.quantity = 0;
                    }
                }
                if (Request.Params["group_id"].ToString() != "")
                {
                    int groupTryId = 0;
                    if (!string.IsNullOrEmpty(Request.Params["group_id"].ToString()) && int.TryParse(Request.Params["group_id"].ToString(), out groupTryId))
                    {
                        pacdModel.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());

                    }
                    else
                    {
                        pacdModel.group_id = oldermodel.group_id;
                    }
                    if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                    {
                        UserCondition uc = new UserCondition();
                        uc.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                        if (_ucMgr.Delete(uc) > 0)
                        {

                            jsonStr = "{success:true}";
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:'user_condition delete failure！'}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr.ToString());
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    pacdModel.condition_id = 0;
                }
                else
                {
                    if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                    {
                        if (!string.IsNullOrEmpty(Request.Params["condition_id"].ToString()))//condition_id
                        {
                            pacdModel.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                        }
                        else
                        {
                            pacdModel.condition_id = oldermodel.condition_id;
                        }
                        pacdModel.group_id = 0;
                    }
                }


                if (!string.IsNullOrEmpty(Request.Params["devicename"].ToString()))//device
                {
                    pacdModel.device = Convert.ToInt32(Request.Params["devicename"].ToString());
                }
                else
                {
                    pacdModel.device = oldermodel.device;
                }
                if (!string.IsNullOrEmpty(Request.Params["start_date"].ToString()))//start
                {
                    pacdModel.start = Convert.ToDateTime(Request.Params["start_date"].ToString());
                }
                else
                {
                    pacdModel.start = oldermodel.date_state;
                }
                if (!string.IsNullOrEmpty(Request.Params["end_date"].ToString()))//end
                {
                    pacdModel.end = Convert.ToDateTime(Request.Params["end_date"].ToString());
                }
                else
                {
                    pacdModel.end = oldermodel.date_end;
                }

                if (!string.IsNullOrEmpty(Request.Params["site"].ToString()))//修改時傳的值為siteName
                {
                    string site = Request.Params["site"].ToString();
                    Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
                    if (reg.IsMatch(site))
                    {
                        pacdModel.site = site;// 將站台改為多選 edit by shuangshuang0420j 20140925 10:08
                    }
                    else
                    {
                        if (site.LastIndexOf(',') == site.Length - 1)//最後一個字符為，時
                        {
                            pacdModel.site = site.Substring(0, site.Length - 1);

                        }

                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["vendor_coverage"].ToString()))//vendor_coverage
                {
                    pacdModel.vendor_coverage = Convert.ToInt32(Request.Params["vendor_coverage"].ToString());
                }
                else
                {
                    pacdModel.vendor_coverage = oldermodel.vendor_coverage;
                }
                //獲取pacdModel.category_father_id
                List<Parametersrc> fatherIdResult = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "event_type", Used = 1, ParameterCode = pacdModel.event_type });
                if (fatherIdResult.Count > 0)
                {
                    pacdModel.category_father_id = Convert.ToUInt32(fatherIdResult[0].ParameterProperty);
                }
                pacdModel.active = false;
                pacdModel.category_updatedate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pacdModel.status = 1;
                pacdModel.category_ipfrom = Request.UserHostAddress;
                #endregion
                if (Request.Params["type"].ToString() == "Add")
                {
                    pacdModel.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    pacdModel.created = DateTime.Now;
                    pacdModel.muser = pacdModel.kuser;
                    pacdModel.modified = pacdModel.created;
                    return AddPromotionsAmountDiscount(pacdModel);
                }
                else
                {

                    pacdModel.kuser = oldermodel.kuser;
                    pacdModel.created = oldermodel.created;
                    pacdModel.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    pacdModel.modified = DateTime.Now;
                    return UpdatePromotionsAmountDiscount(pacdModel, CommonFunction.GetEventId(oldermodel.event_type.ToString(), oldermodel.id.ToString()));
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                this.Response.Clear();
                this.Response.Write("{success:false,msg:0}");
                this.Response.End();
                return this.Response;
            }

        }
        #endregion

        #region 新增活動的其他欄位 + HttpResponseBase AddPromotionsAmountDiscount()
        /// <summary>
        /// PromotionsAmountDiscountCustom model 對象
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase AddPromotionsAmountDiscount(PromotionsAmountDiscountCustom model)
        {
            string jsonStr = String.Empty;
            try
            {

                if (_promoAmountDiscountMgr.ReSaveDiscount(model))
                {
                    jsonStr = "{success:true}";//返回json數據
                }
                else
                {
                    jsonStr = "{success:false}";//返回json數據
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

        #region 更新活動的其他欄位 + HttpResponseBase UpdatePromotionsAmountDiscount()
        /// <summary>
        /// PromotionsAmountDiscountCustom model 對象
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase UpdatePromotionsAmountDiscount(PromotionsAmountDiscountCustom model, string oldEventId)
        {
            string jsonStr = String.Empty;
            try
            {

                if (_promoAmountDiscountMgr.ReUpdateDiscount(model, oldEventId))
                {
                    jsonStr = "{success:true}";//返回json數據
                }
                else
                {
                    jsonStr = "{success:false}";//返回json數據
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

        #region 獲取幾件幾元的活動條件 + HttpResponseBase PromoDiscountList()
        /// <summary>
        /// 獲取幾件幾元的活動條件列表
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase PromoDiscountList()
        {
            /********************************************************/
            List<PromoDiscount> stores = new List<PromoDiscount>();

            string json = string.Empty;
            try
            {
                PromoDiscount query = new PromoDiscount();
                string pid = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProType"].ToString()) && !string.IsNullOrEmpty(Request.Params["Proid"].ToString()))
                {
                    pid = CommonFunction.GetEventId(Request.Params["ProType"].ToString(), Request.Params["Proid"].ToString());

                }
                int totalCount = 0;
                query.event_id = pid;
                stores = _promoDiscountMgr.GetPromoDiscount(query);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據

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

        #region 保存幾件幾元或者幾件幾折的活動條件 + HttpResponseBase SavePromoDis()
        /// <summary>
        /// 保存幾件幾元或者幾件幾折的活動條件
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SavePromoDis()
        {
            PromoDiscount model = new PromoDiscount();
            if (!string.IsNullOrEmpty(Request.Params["eventType"].ToString()) && !string.IsNullOrEmpty(Request.Params["id"].ToString()))
            {

                model.event_id = CommonFunction.GetEventId(Request.Params["eventType"].ToString(), Request.Params["id"].ToString());
            }
            else
            {
                model.event_id = string.Empty;
            }
            if (!string.IsNullOrEmpty(Request.Params["jianshu"].ToString()))
            {
                model.quantity = Convert.ToInt32(Request.Params["jianshu"].ToString());
            }
            else
            {
                model.quantity = 0;
            }
            if (Request.Params["price"] != null && !string.IsNullOrEmpty(Request.Params["price"].ToString()))
            {
                model.special_price = Convert.ToInt32(Request.Params["price"].ToString());
            }
            else
            {
                model.special_price = 0;
            }
            if (Request.Params["discount"] != null && !string.IsNullOrEmpty(Request.Params["discount"].ToString()))
            {
                model.discount = Convert.ToInt32(Request.Params["discount"].ToString());
            }
            else
            {
                model.discount = 0;
            }

            model.status = 1;

            if (string.IsNullOrEmpty(Request.Params["rid"]))//新增
            {
                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.kdate = DateTime.Now;
                model.mdate = DateTime.Now;

                return InsertPromoDis(model);
            }
            else
            {
                model.rid = int.Parse(Request.Params["rid"]);
                model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();//修改人
                model.kdate = DateTime.Now;
                model.mdate = DateTime.Now;
                return UpdatePromoDis(model);

            }

        }

        protected HttpResponseBase InsertPromoDis(PromoDiscount model)
        {

            string json = string.Empty;
            int maxQuantity, maxPrice, minDiscount, minQuantity, minPrice, maxDiscount;
            try
            {
                DataTable dt = _promoDiscountMgr.GetLimitByEventId(model.event_id, 0);

                if (dt != null)
                {

                    try
                    {
                        maxQuantity = Convert.ToInt32(dt.Rows[0]["maxQ"].ToString());
                    }
                    catch (Exception)
                    {

                        maxQuantity = 0;
                    }
                    try
                    {
                        maxPrice = Convert.ToInt32(dt.Rows[0]["maxS"].ToString());
                    }
                    catch
                    {
                        maxPrice = 0;
                    }
                    try
                    {
                        minDiscount = int.Parse(dt.Rows[0]["minD"].ToString());
                    }
                    catch (Exception)
                    {
                        minDiscount = 100;
                    }

                    try
                    {
                        minQuantity = Convert.ToInt32(dt.Rows[0]["minQ"].ToString());
                    }
                    catch (Exception)
                    {
                        minQuantity = 0;

                    }
                    try
                    {
                        minPrice = Convert.ToInt32(dt.Rows[0]["minS"].ToString());
                    }
                    catch
                    {
                        minPrice = 0;
                    }
                    try
                    {
                        maxDiscount = int.Parse(dt.Rows[0]["maxD"].ToString());
                    }
                    catch (Exception)
                    {
                        maxDiscount = 100;
                    }
                    if ((model.quantity < minQuantity || minQuantity == 0) && model.discount == 100 && model.quantity != 0 && (dt.Rows.Count == 1 && dt.Rows[0]["minQ"].ToString() == ""))
                    {
                        if (_promoDiscountMgr.Save(model) > 0)
                        {
                            json = "{success:true,msg:0}";//返回json數據
                        }
                        else
                        {
                            json = "{success:false,msg:0}";//返回json數據
                        }
                    }
                    else
                    {


                        if ((model.quantity > maxQuantity && (model.special_price > maxPrice || model.discount < minDiscount))
                            || (model.quantity < minQuantity && model.quantity != 0 &&
                            ((model.special_price < minPrice && model.special_price != 0) || (model.discount > maxDiscount && model.discount != 0))))
                        {
                            if (_promoDiscountMgr.Save(model) > 0)
                            {
                                json = "{success:true,msg:0}";//返回json數據
                            }
                            else
                            {
                                json = "{success:false,msg:0}";//返回json數據
                            }
                        }
                        else
                        {
                            json = "{success:false,msg:1}";//返回json數據
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
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }

        protected HttpResponseBase UpdatePromoDis(PromoDiscount model)
        {
            string json = string.Empty;
            int maxQuantity, maxPrice, minDiscount, minQuantity, minPrice, maxDiscount;
            try
            {
                DataTable dt = _promoDiscountMgr.GetLimitByEventId(model.event_id, model.rid);
                if (dt != null)
                {

                    try
                    {
                        maxQuantity = Convert.ToInt32(dt.Rows[0]["maxQ"].ToString());
                    }
                    catch (Exception)
                    {

                        maxQuantity = 0;
                    }
                    try
                    {
                        maxPrice = Convert.ToInt32(dt.Rows[0]["maxS"].ToString());
                    }
                    catch
                    {
                        maxPrice = 0;
                    }
                    try
                    {
                        minDiscount = int.Parse(dt.Rows[0]["minD"].ToString());
                    }
                    catch (Exception)
                    {
                        minDiscount = 100;
                    }

                    try
                    {
                        minQuantity = Convert.ToInt32(dt.Rows[0]["minQ"].ToString());
                    }
                    catch (Exception)
                    {
                        minQuantity = 0;

                    }
                    try
                    {
                        minPrice = Convert.ToInt32(dt.Rows[0]["minS"].ToString());
                    }
                    catch
                    {
                        minPrice = 0;
                    }
                    try
                    {
                        maxDiscount = int.Parse(dt.Rows[0]["maxD"].ToString());
                    }
                    catch (Exception)
                    {
                        maxDiscount = 100;
                    }
                    if ((model.quantity < minQuantity || minQuantity == 0) && model.discount == 100 && model.quantity != 0 && (dt.Rows.Count == 1 && dt.Rows[0]["minQ"].ToString() == "100"))
                    {
                        if (_promoDiscountMgr.Update(model) > 0)
                        {
                            json = "{success:true,msg:0}";//返回json數據
                        }
                        else
                        {
                            json = "{success:false,msg:0}";//返回json數據
                        }
                    }
                    else
                    {

                        if ((model.quantity > maxQuantity && (model.special_price > maxPrice || model.discount < minDiscount))
                                 || (model.quantity < minQuantity && model.quantity != 0 &&
                                ((model.special_price < minPrice && model.special_price != 0) || (model.discount > maxDiscount && model.discount != 0))))
                        {
                            if (_promoDiscountMgr.Update(model) > 0)
                            {
                                json = "{success:true,msg:0}";//返回json數據
                            }
                            //listUser是准备转换的对象
                            else
                            {
                                json = "{success:false,msg:0}";//返回json數據
                            }
                        }
                        else
                        {

                            json = "{success:false,msg:1}";//返回json數據
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
                json = "{success:false,msg:0}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除幾件幾元或幾件幾折的活動條件 + HttpResponseBase DeletePromoDis()
        /// <summary>
        /// 刪除幾件幾元或幾件幾折的活動條件
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase DeletePromoDis()
        {
            string jsonStr = String.Empty;

            if (!String.IsNullOrEmpty(Request.Params["rid_del"]))
            {
                try
                {
                    string rid = Request.Params["rid_del"].ToString();
                    PromoDiscount query = new PromoDiscount();
                    if (!String.IsNullOrEmpty(rid))
                    {
                        query.rid = Convert.ToInt32(rid);
                        _promoDiscountMgr.DeleteByRid(Convert.ToInt32(rid));

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

        #region 獲取會員群組 +HttpResponseBase GetVipGroup()
        /// <summary>
        /// 獲取會員分組
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVipGroup()
        {
            List<VipUserGroup> stores = new List<VipUserGroup>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                stores = _vipGroupMgr.GetAllUserGroup();
                VipUserGroup Dmodel = new VipUserGroup();
                Dmodel.group_id = 0;
                Dmodel.group_name = "不分";
                stores.Insert(0, Dmodel);//使會員群組的第一行為不分
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據

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

        #region 獲取Site +HttpResponseBase GetSite()
        /// <summary>
        /// 獲取site
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetSite()
        {

            List<Site> stores = new List<Site>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                Site query = new Site();
                stores = _siteMgr.Query(query);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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

        #region 移除幾件幾元或幾件幾折 +HttpResponseBase Deletezhe()
        /// <summary>
        /// 移除幾件幾元或幾件幾折
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Deletezhe()
        {
            string jsonStr = String.Empty;
            PromotionsAmountDiscount query = new PromotionsAmountDiscount();
            ProductCategory pmodel = new ProductCategory();
            ProductCategorySet pmsmodel = new ProductCategorySet();
            ProductCategorySet querypcs = new ProductCategorySet();
            PromoDiscount querypd = new PromoDiscount();
            PromoAll pamodel = new PromoAll();
            ProdPromo ppmodel = new ProdPromo();

            querypcs.Category_Id = pmodel.category_id;
            List<ProductCategorySet> smodel = _produCateSetMgr.Query(querypcs);

            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))//批次移除
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {

                            //刪除peomotion_amount_discount 
                            //根據id 獲取整個model 
                            query = _promoAmountDiscountMgr.GetModelById(Convert.ToInt32(rid));

                            var eventid = CommonFunction.GetEventId(query.event_type, query.id.ToString());

                            if (_promoAmountDiscountMgr.Delete(query, eventid) > 0)
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

        #region 更改活動使用狀態 + JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            try
            {
                int id = Convert.ToInt32(Request.Params["id"] ?? "0");
                int activeValue = Convert.ToInt32(Request.Params["active"] ?? "0");
                string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                string kuser = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["muser"]))
                {
                    kuser = (Request.Params["muser"]);
                }
                if(currentUser==kuser &&activeValue==1)
                {
                    return Json(new {success="stop" });
                }
                PromotionsAmountDiscountCustom model = _promoAmountDiscountMgr.GetModelById(id);
                if (model.url_by == 1)
                {
                    model.category_link_url = _produCateMgr.GetModelById(model.category_id).category_link_url;
                }
                //model.active = activeValue == 1 ? true : false;
                model.active = Convert.ToBoolean(activeValue);
                model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.modified = DateTime.Now;
                model.event_id = CommonFunction.GetEventId(model.event_type.ToString(), model.id.ToString());
                model.start = model.date_state;
                model.end = model.date_end;
                if (_promoAmountDiscountMgr.UpdateActive(model))
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

        #region 刪除本地上傳的圖片 + void DeletePicFile(string imageName)
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

        #region 創建ftp文件夾 + void CreateFolder(string path, string[] Mappath)
        /// <summary>
        /// 創建文件夾
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="Mappath">文件名數組</param>
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



        #region 根據商品編號獲取商品名稱
        public HttpResponseBase GetProdName()
        {
            string json = string.Empty;
            try
            {
                _prodMgr = new ProductMgr(mySqlConnectionString);
                int isTranUint = 0;

                int pid = 0;
                int cid = 0;
                int bid = 0;
                if (!string.IsNullOrEmpty(Request.Params["product_id"]) && int.TryParse(Request.Params["product_id"], out isTranUint))
                {
                    pid = Convert.ToInt32(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["class_id"]) && int.TryParse(Request.Params["class_id"], out isTranUint))
                {
                    cid = Convert.ToInt32(Request.Params["class_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]) && int.TryParse(Request.Params["brand_id"], out isTranUint))
                {
                    bid = Convert.ToInt32(Request.Params["brand_id"]);
                }
                string prodName = _prodMgr.GetNameForID(pid, cid, bid);
                if (!string.IsNullOrEmpty(prodName))
                {
                    json = "{success:true,\"prod_name\":\"" + prodName + "\"}";//返回json數據
                }
                else
                {
                    json = "{success:false,\"prod_name\":\"\"}";//返回json數據
                }



            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,\"prod_name\":\"\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }

        #endregion
    }
}









