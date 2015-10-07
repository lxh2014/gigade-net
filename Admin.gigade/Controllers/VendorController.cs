#region 文件信息
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：VendorController.cs 
 * 摘   要： 
 *      供應商管理
 * 当前版本：v1.1 
 *           v1.2
 *           添加資料修改記錄和修改記錄匯出功能 工作項378 edit by shuangshuang0420j  2015.06.16
 *           供查詢資料是否有異動使用            
 *            ■修改前後的需要有的欄位資料: 狀態、成本百分比、出貨天數、合約簽訂日期、銀行資料(銀行代碼,銀行名稱,銀行帳號,銀行戶名)、管理人員。  
 * 作   者： hongfei0416j
 * 完成日期：2014/10/7
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
using System.Data;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;
using gigadeExcel.Comment;


namespace Admin.gigade.Controllers
{
    public class VendorController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IVendorBrandSetImplMgr _IvendorBrandSet;
        private VendorBrandSetQuery query;
        private VendorBrandMgr _vendorBrand;
        private ISiteConfigImplMgr _siteConfigMgr;
        private IProductImplMgr _productMgr;
        //供應商查詢
        private IVendorImplMgr _vendorMgr;
        private ZipMgr zMgr;
        private IConfigImplMgr _configMgr;
        private ICallerImplMgr mgr;
        private IVendorLoginListImplMgr _Ivendorloginlist;
        private ParameterMgr paraMgr;
        private GroupCallerMgr gcMgr;
        private IFgroupImplMgr fgMgr;
        private IManageUserImplMgr muMgr;
        private ISiteConfigImplMgr siteConfigMgr;
        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制

        static string excelPath = ConfigurationManager.AppSettings["ImportCompareExcel"];//關於導入的excel文件的限制
        private InvoiceMasterRecordMgr _imrMgr;

        string vendorPath = ConfigurationManager.AppSettings["vendorPath"];
        string vendorOriginalPath = "/brand_story/a/";
        //string vendor400Path = "/brand_story/400x400/";   //在前台如果各种尺寸的图档没有的时候，前台会自动产生！！！
        string brandPath = "/brand_master/a/";
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.vendorPath);//圖片保存路徑
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();   //用於添密
        #region View
        public ActionResult VendorList()
        {
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            ViewBag.vendorCsvPath = excelPath;
            return View();
        }
        public ActionResult VendorDetails()
        {
            ViewBag.Vendor_id = Request.QueryString["Vendor_id"] ?? "";//獲取付款單號
            return View();
        }
        public ActionResult VendorBrandList()
        {
            return View();
        }
        public ActionResult VendorLoginList()
        {
            return View();
        }
        public ActionResult PictureMaintain()
        {
            VendorBrand query = new VendorBrand();
            _vendorBrand = new VendorBrandMgr(connectionString);

            ViewBag.BrandId = Request.Params["Brand_Id"];
            query.Brand_Id = Convert.ToUInt32(Request.Params["Brand_Id"]);
            VendorBrand model = _vendorBrand.GetProductBrand(query);
            if (model != null)
            {
                ViewBag.BrandName = model.Brand_Name;
                //Request.Params["Brand_Name"];
            }
            else
            {
                ViewBag.BrandName = "";
            }
            return View();
        }

        public ActionResult VendorBrandStoryList()
        {
            return View();
        }

        public ActionResult VendorChangeLog()
        {
            return View();
        }
        #endregion

        #region 供應商查詢
        #region 獲取供應商列表數據 HttpResponseBase GetVendorList()
        /// <summary>
        /// 獲取供應商列表數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendorList()
        {
            List<VendorQuery> stores = new List<VendorQuery>();
            string json = string.Empty;

            try
            {
                _vendorMgr = new VendorMgr(connectionString);
                #region 搜索條件
                VendorQuery query = new VendorQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量


                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    query.vendor_id = Convert.ToUInt32(Request.Params["relation_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["VendorId"]))
                {
                    query.vendor_id = Convert.ToUInt32(Request.Params["VendorId"].ToString());
                }
                string type = string.Empty;
                string con = string.Empty;

                if (!string.IsNullOrEmpty(Request.Params["dateType"]))
                {
                    type = Request.Params["dateType"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["dateCon"]))
                {
                    con = Request.Params["dateCon"].ToString();
                }
                int totalCount = 0;
                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(con) && type != "0")
                {//當查詢條件和查詢內容都有值時執行
                    switch (type)
                    {
                        case "0":
                            break;
                        case "1":
                            query.searchEmail = con.ToLower();
                            break;
                        case "2":
                            query.searchName = con;
                            break;
                        case "3":
                            query.vendor_name_full = con;
                            break;
                        case "4":
                            query.searchInvoice = con;
                            break;
                        case "5":
                            query.erp_id = con;
                            break;
                        case "6":
                            uint isTranUint = 0;
                            if (uint.TryParse(con, out isTranUint))
                            {
                                query.vendor_id = Convert.ToUInt32(con);
                            }
                            break;
                        case "7":
                            query.vendor_code = con;
                            break;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendortype"]))//供应商类型
                    {
                        query.vendor_type = Request.Params["vendortype"].ToString();
                        query.vendor_type = query.vendor_type.TrimEnd(',');
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                    {
                        query.create_dateOne = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateOne"]).ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                    {
                        query.create_dateTwo = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateTwo"]).ToString("yyyy-MM-dd 23:59:59"));
                    }
                    stores = _vendorMgr.Query(query, ref totalCount);
                }
                else
                { //當查詢條件和查詢內容都沒有值時執行
                    if (!string.IsNullOrEmpty(Request.Params["vendortype"]))//供应商类型
                    {
                        query.vendor_type = Request.Params["vendortype"].ToString();
                        query.vendor_type = query.vendor_type.TrimEnd(',');
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                    {
                        query.create_dateOne = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateOne"]).ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                    {
                        query.create_dateTwo = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateTwo"]).ToString("yyyy-MM-dd 23:59:59"));
                    }
                    stores = _vendorMgr.Query(query, ref totalCount);
                }
                #endregion
                #region 供應商類型參數
                List<Parametersrc> list = new List<Parametersrc>();
                paraMgr = new ParameterMgr(connectionString);
                string types = "vendor_type";
                list = paraMgr.GetElementType(types);
                #endregion


                foreach (var item in stores)
                {
                    item.vendor_name_full = Server.HtmlDecode(Server.HtmlDecode(item.vendor_name_full));
                    item.vendor_name_simple = Server.HtmlDecode(Server.HtmlDecode(item.vendor_name_simple));
                    string temp = string.Empty;

                    #region 供應商類型關聯參數表
                    if (!string.IsNullOrEmpty(item.vendor_type) && item.vendor_type != null)
                    {
                        string[] vendor_types = item.vendor_type.Split(',');
                        for (int i = 0; i < vendor_types.Length; i++)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                if (list[j].ParameterCode == vendor_types[i].ToString())
                                {
                                    item.vendor_type_name += list[j].parameterName + ",";
                                }
                            }

                        }
                        item.vendor_type_name = item.vendor_type_name.Substring(0, item.vendor_type_name.Length - 1);
                    }
                    #endregion
                    if (item.self_send_days == 0)
                    {
                        if (item.stuff_ware_days == 0)
                        {
                            if (item.dispatch_days == 0)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = "調度";
                            }
                        }
                        else
                        {
                            if (item.dispatch_days == 0)
                            {
                                temp = "寄倉";
                            }
                            else
                            {
                                temp = "寄倉,調度";
                            }
                        }
                    }
                    else
                    {
                        if (item.stuff_ware_days == 0)
                        {
                            if (item.dispatch_days == 0)
                            {
                                temp = "自出";
                            }
                            else
                            {
                                temp = "自出,調度";
                            }
                        }
                        else
                        {
                            if (item.dispatch_days == 0)
                            {
                                temp = "自出,寄倉";
                            }
                            else
                            {
                                temp = "自出,寄倉,調度";
                            }
                        }
                    }
                    item.vendor_mode = temp;

                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {

                        if (!string.IsNullOrEmpty(item.vendor_name_full))
                        {
                            item.vendor_name_full = item.vendor_name_full.Substring(0, 1) + "**";
                        }
                        //
                        if (!string.IsNullOrEmpty(item.vendor_name_simple))
                        {
                            item.vendor_name_simple = item.vendor_name_simple.Substring(0, 1) + "**";
                        }//vendor_company_address
                        if (item.vendor_company_address.ToString().Length > 3)
                        {
                            item.vendor_company_address = item.vendor_company_address.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.vendor_company_address = item.vendor_company_address + "***";
                        }
                        if (!string.IsNullOrEmpty(item.contact_email_1))
                        {
                            item.contact_email_1 = item.contact_email_1.Split('@')[0] + "@***";
                        }
                        if (!string.IsNullOrEmpty(item.contact_email_2))
                        {
                            item.contact_email_2 = item.contact_email_2.Split('@')[0] + "@***";
                        }
                        if (!string.IsNullOrEmpty(item.contact_email_3))
                        {
                            item.contact_email_3 = item.contact_email_3.Split('@')[0] + "@***";
                        }
                        if (!string.IsNullOrEmpty(item.contact_email_4))
                        {
                            item.contact_email_4 = item.contact_email_4.Split('@')[0] + "@***";
                        }
                        if (!string.IsNullOrEmpty(item.contact_email_5))
                        {
                            item.contact_email_5 = item.contact_email_5.Split('@')[0] + "@***";
                        }


                        if (item.contact_mobile_1.ToString().Length > 3)
                        {
                            item.contact_mobile_1 = item.contact_mobile_1.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_mobile_1 = item.contact_mobile_1 + "***";
                        }
                        if (item.contact_mobile_2.ToString().Length > 3)
                        {
                            item.contact_mobile_2 = item.contact_mobile_2.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_mobile_2 = item.contact_mobile_2 + "***";
                        }
                        if (item.contact_mobile_3.ToString().Length > 3)
                        {
                            item.contact_mobile_3 = item.contact_mobile_3.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_mobile_3 = item.contact_mobile_3 + "***";
                        }
                        if (item.contact_mobile_4.ToString().Length > 3)
                        {
                            item.contact_mobile_4 = item.contact_mobile_4.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_mobile_4 = item.contact_mobile_4 + "***";
                        }
                        if (item.contact_mobile_5.ToString().Length > 3)
                        {
                            item.contact_mobile_5 = item.contact_mobile_5.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_mobile_5 = item.contact_mobile_5 + "***";
                        }

                        if (item.contact_phone_1_1.ToString().Length > 3)
                        {
                            item.contact_phone_1_1 = item.contact_phone_1_1.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_1_1 = item.contact_phone_1_1 + "***";
                        }
                        if (item.contact_phone_1_2.ToString().Length > 3)
                        {
                            item.contact_phone_1_2 = item.contact_phone_1_2.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_1_2 = item.contact_phone_1_2 + "***";
                        }
                        if (item.contact_phone_1_3.ToString().Length > 3)
                        {
                            item.contact_phone_1_3 = item.contact_phone_1_3.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_1_3 = item.contact_phone_1_3 + "***";
                        }
                        if (item.contact_phone_1_4.ToString().Length > 3)
                        {
                            item.contact_phone_1_4 = item.contact_phone_1_4.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_1_4 = item.contact_phone_1_4 + "***";
                        }
                        if (item.contact_phone_1_5.ToString().Length > 3)
                        {
                            item.contact_phone_1_5 = item.contact_phone_1_5.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_1_5 = item.contact_phone_1_5 + "***";
                        }


                        if (item.contact_phone_2_1.ToString().Length > 3)
                        {
                            item.contact_phone_2_1 = item.contact_phone_2_1.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_2_1 = item.contact_phone_2_1 + "***";
                        }
                        if (item.contact_phone_2_2.ToString().Length > 3)
                        {
                            item.contact_phone_2_2 = item.contact_phone_2_2.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_2_2 = item.contact_phone_2_2 + "***";
                        }
                        if (item.contact_phone_2_3.ToString().Length > 3)
                        {
                            item.contact_phone_2_3 = item.contact_phone_2_3.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_2_3 = item.contact_phone_2_3 + "***";
                        }
                        if (item.contact_phone_2_4.ToString().Length > 3)
                        {
                            item.contact_phone_2_4 = item.contact_phone_2_4.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_2_4 = item.contact_phone_2_4 + "***";
                        }
                        if (item.contact_phone_2_5.ToString().Length > 3)
                        {
                            item.contact_phone_2_5 = item.contact_phone_2_5.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.contact_phone_2_5 = item.contact_phone_2_5 + "***";
                        }

                        if (!string.IsNullOrEmpty(item.contact_name_1))
                        {
                            item.contact_name_1 = item.contact_name_1.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item.contact_name_2))
                        {
                            item.contact_name_2 = item.contact_name_2.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item.contact_name_3))
                        {
                            item.contact_name_3 = item.contact_name_3.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item.contact_name_4))
                        {
                            item.contact_name_4 = item.contact_name_4.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item.contact_name_5))
                        {
                            item.contact_name_5 = item.contact_name_5.Substring(0, 1) + "**";
                        }

                        if (!string.IsNullOrEmpty(item.manage_name))
                        {
                            item.manage_name = item.manage_name.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item.manage_email))
                        {
                            item.manage_email = item.manage_email.Split('@')[0] + "@***";
                        }
                        if (!string.IsNullOrEmpty(item.vendor_email))
                        {
                            item.vendor_email = item.vendor_email.Split('@')[0] + "@***";
                        }
                        if (!string.IsNullOrEmpty(item.company_person))
                        {
                            item.company_person = item.company_person.Substring(0, 1) + "**";
                        }
                        if (item.company_address.ToString().Length > 3)
                        {
                            item.company_address = item.company_address.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.company_address = item.company_address + "***";
                        }
                        if (item.invoice_address.ToString().Length > 3)
                        {
                            item.invoice_address = item.invoice_address.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.invoice_address = item.invoice_address + "***";
                        }
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        public string GetShopName(uint brandId)
        {
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            uint id = brandId;
            return _IvendorBrandSet.GetShopName(id);
        }
        #endregion

        #region 修改密碼EditPass
        [CustomHandleError]
        public HttpResponseBase EditPass()
        {
            string jsonStr = "{success:false}";
            try
            {
                _vendorMgr = new VendorMgr(connectionString);
                string newPass = string.Empty;

                string vendorId = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["vendorId"]))
                {
                    vendorId = Request.Params["vendorId"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["newPass"]))
                {
                    BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                    newPass = hash.SHA256Encrypt(Request.Params["newPass"].ToString());

                }


                if (_vendorMgr.EditPass(vendorId, newPass) > 0)
                {
                    jsonStr = "{success:true}";
                }
                else
                {
                    jsonStr = "{success:false}";
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

        #region 保存數據 + HttpResponseBase SaveVendor()
        /// <summary>
        /// 保存供應商數據
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SaveVendor()
        {
            string jsonStr = String.Empty;
            try
            {
                VendorQuery venQuery = new VendorQuery();

                venQuery.user_type = 2;//變更者類型1.供應商2.管理員
                if (Request.Params["vendor_id"].ToString() == "")
                {
                    #region 獲取供應商數據
                    if (!string.IsNullOrEmpty(Request.Params["vendor_status"].ToString()))
                    {
                        venQuery.vendor_status = Convert.ToUInt32(Request.Params["vendor_status"].ToString());
                    }
                    else
                    {
                        venQuery.vendor_status = 1;
                    }

                    _vendorMgr = new VendorMgr(connectionString);
                    if (!string.IsNullOrEmpty(Request.Params["vendor_email"].ToString()))
                    {
                        try
                        {
                            if (_vendorMgr.IsExitEmail(Request.Params["vendor_email"].ToString().ToLower()) != 0)
                            {
                                jsonStr = "{success:true,msg:0}";//返回json數據
                                this.Response.Clear();
                                this.Response.Write(jsonStr);
                                this.Response.End();
                                return this.Response;
                            }
                            else
                            {
                                venQuery.vendor_email = Request.Params["vendor_email"].Trim().ToLower();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            jsonStr = "{success:false,msg:1}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr);
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    else
                    {
                        venQuery.vendor_email = "";
                    }

                    BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                    if (!string.IsNullOrEmpty(Request.Params["vendor_password"].ToString()))
                    {
                        venQuery.vendor_password = hash.SHA256Encrypt(Request.Params["vendor_password"].ToString());
                    }
                    else
                    {
                        //新增供應商時如果未設置密碼則將統一編號進行加密作為密碼進行保存
                        venQuery.vendor_password = hash.SHA256Encrypt(Request.Params["vendor_invoice"]);
                    }

                    if (!string.IsNullOrEmpty(Request.Params["vendor_name_full"].ToString()))
                    {
                        venQuery.vendor_name_full = Request.Params["vendor_name_full"].ToString().Trim();
                    }
                    else
                    {
                        venQuery.vendor_name_full = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendor_name_simple"].ToString()))
                    {
                        venQuery.vendor_name_simple = Request.Params["vendor_name_simple"].ToString().Trim();
                    }
                    else
                    {
                        venQuery.vendor_name_simple = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendor_invoice"].ToString()))
                    {
                        venQuery.vendor_invoice = Request.Params["vendor_invoice"].ToString();
                    }
                    else
                    {
                        venQuery.vendor_invoice = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_phone"].ToString()))
                    {
                        venQuery.company_phone = Request.Params["company_phone"].ToString();
                    }
                    else
                    {
                        venQuery.company_phone = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_fax"].ToString()))
                    {
                        venQuery.company_fax = Request.Params["company_fax"].ToString();
                    }
                    else
                    {
                        venQuery.company_fax = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_person"].ToString()))
                    {
                        venQuery.company_person = Request.Params["company_person"].ToString();
                    }
                    else
                    {
                        venQuery.company_person = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_zip"].ToString()))
                    {
                        venQuery.company_zip = Convert.ToUInt32(Request.Params["company_zip"].ToString());
                    }
                    else
                    {
                        venQuery.company_zip = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_address"].ToString()))
                    {
                        venQuery.company_address = Request.Params["company_address"].ToString();
                    }
                    else
                    {
                        venQuery.company_address = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["invoice_zip"].ToString()))
                    {
                        venQuery.invoice_zip = Convert.ToUInt32(Request.Params["invoice_zip"].ToString());
                    }
                    else
                    {
                        venQuery.invoice_zip = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["invoice_address"].ToString()))
                    {
                        venQuery.invoice_address = Request.Params["invoice_address"].ToString();
                    }
                    else
                    {
                        venQuery.invoice_address = "";
                    }

                    if (!string.IsNullOrEmpty(Request.Params["pm"].ToString()))
                    {
                        venQuery.product_manage = Convert.ToUInt32(Request.Params["pm"].ToString());
                    }
                    else
                    {
                        venQuery.product_manage = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["cost_percent"].ToString()))
                    {
                        venQuery.cost_percent = Convert.ToUInt32(Request.Params["cost_percent"].ToString());
                    }
                    else
                    {
                        venQuery.cost_percent = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["creditcard_1_percent"].ToString()))
                    {
                        venQuery.creditcard_1_percent = Convert.ToUInt32(Request.Params["creditcard_1_percent"].ToString());
                    }
                    else
                    {
                        venQuery.creditcard_1_percent = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["creditcard_3_percent"].ToString()))
                    {
                        venQuery.creditcard_3_percent = Request.Params["creditcard_3_percent"].ToString();
                    }
                    else
                    {
                        venQuery.creditcard_3_percent = "0";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["agreement_createdate"].ToString()))
                    {
                        venQuery.agreement_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["agreement_createdate"].ToString()));

                    }
                    else
                    {
                        venQuery.agreement_createdate = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["agreement_start"].ToString()))
                    {
                        venQuery.agreement_start = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["agreement_start"].ToString()));
                    }
                    else
                    {
                        venQuery.agreement_start = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["agreement_end"].ToString()))
                    {
                        venQuery.agreement_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["agreement_end"].ToString()));
                    }
                    else
                    {
                        venQuery.agreement_end = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["checkout_type"].ToString()))
                    {
                        venQuery.checkout_type = Convert.ToUInt32(Request.Params["checkout_type"].ToString());
                    }
                    else
                    {
                        venQuery.checkout_type = 1;
                    }
                    if (venQuery.checkout_type == 3)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["checkout_other"].ToString()))
                        {
                            venQuery.checkout_other = Request.Params["checkout_other"].ToString();
                        }
                        else
                        {
                            venQuery.checkout_other = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_code"].ToString()))
                    {
                        venQuery.bank_code = Request.Params["bank_code"].ToString();
                    }
                    else
                    {
                        venQuery.bank_code = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_name"].ToString()))
                    {
                        venQuery.bank_name = Request.Params["bank_name"].ToString();
                    }
                    else
                    {
                        venQuery.bank_name = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_number"].ToString()))
                    {
                        venQuery.bank_number = Request.Params["bank_number"].ToString();
                    }
                    else
                    {
                        venQuery.bank_number = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_account"].ToString()))
                    {
                        venQuery.bank_account = Request.Params["bank_account"].ToString();
                    }
                    else
                    {
                        venQuery.bank_account = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_number"].ToString()))
                    {
                        venQuery.bank_number = Request.Params["bank_number"].ToString();
                    }
                    else
                    {
                        venQuery.bank_number = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["freight_low_limit"].ToString()))
                    {
                        venQuery.freight_low_limit = Convert.ToUInt32(Request.Params["freight_low_limit"].ToString());
                    }
                    else
                    {
                        venQuery.freight_low_limit = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["freight_low_money"].ToString()))
                    {
                        venQuery.freight_low_money = Convert.ToUInt32(Request.Params["freight_low_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_low_money = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["freight_return_low_money"].ToString()))
                    {
                        venQuery.freight_return_low_money = Convert.ToUInt32(Request.Params["freight_return_low_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_return_low_money = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["freight_normal_limit"].ToString()))
                    {
                        venQuery.freight_normal_limit = Convert.ToUInt32(Request.Params["freight_normal_limit"].ToString());
                    }
                    else
                    {
                        venQuery.freight_normal_limit = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["freight_normal_money"].ToString()))
                    {
                        venQuery.freight_normal_money = Convert.ToUInt32(Request.Params["freight_normal_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_normal_money = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["freight_return_normal_money"].ToString()))
                    {
                        venQuery.freight_return_normal_money = Convert.ToUInt32(Request.Params["freight_return_normal_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_return_normal_money = 0;
                    }
                    //if (!string.IsNullOrEmpty(Request.Params["assist"].ToString()))
                    //{
                    //    venQuery.assist = Convert.ToUInt32(Request.Params["assist"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.assist = 0;
                    //}
                    //if (!string.IsNullOrEmpty(Request.Params["dispatch"].ToString()))
                    //{
                    //    venQuery.dispatch = Convert.ToUInt32(Request.Params["dispatch"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.dispatch = 0;
                    //}
                    //if (!string.IsNullOrEmpty(Request.Params["product_mode"].ToString()))
                    //{
                    //    venQuery.product_mode = Convert.ToUInt32(Request.Params["product_mode"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.product_mode = 1;
                    //}
                    if (!string.IsNullOrEmpty(Request.Params["procurement_days"].ToString()))
                    {
                        venQuery.procurement_days = Convert.ToInt32(Request.Params["procurement_days"].ToString());
                    }
                    else
                    {
                        venQuery.procurement_days = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["self_send_days"].ToString()))
                    {
                        venQuery.self_send_days = Convert.ToInt32(Request.Params["self_send_days"].ToString());
                    }
                    else
                    {
                        venQuery.self_send_days = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["stuff_ware_days"].ToString()))
                    {
                        venQuery.stuff_ware_days = Convert.ToInt32(Request.Params["stuff_ware_days"].ToString());
                    }
                    else
                    {
                        venQuery.stuff_ware_days = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dispatch_days"].ToString()))
                    {
                        venQuery.dispatch_days = Convert.ToInt32(Request.Params["dispatch_days"].ToString());
                    }
                    else
                    {
                        venQuery.dispatch_days = 0;
                    }

                    if (!string.IsNullOrEmpty(Request.Params["gigade_bunus_percent"].ToString()))
                    {
                        venQuery.gigade_bunus_percent = Convert.ToUInt32(Request.Params["gigade_bunus_percent"].ToString());
                    }
                    else
                    {
                        venQuery.gigade_bunus_percent = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["gigade_bunus_threshold"].ToString()))
                    {
                        venQuery.gigade_bunus_threshold = Convert.ToUInt32(Request.Params["gigade_bunus_threshold"].ToString());
                    }
                    else
                    {
                        venQuery.gigade_bunus_threshold = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendor_note"].ToString()))
                    {
                        venQuery.vendor_note = Request.Params["vendor_note"].ToString();
                    }
                    else
                    {
                        venQuery.vendor_note = "";
                    }

                    if (!string.IsNullOrEmpty(Request.Params["prod_cate"].ToString()))
                    {
                        venQuery.prod_cate = Request.Params["prod_cate"].ToString();
                    }
                    else
                    {
                        venQuery.prod_cate = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["buy_cate"].ToString()))
                    {
                        venQuery.buy_cate = Request.Params["buy_cate"].ToString();
                    }
                    else
                    {
                        venQuery.buy_cate = "";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["tax_type"].ToString()))
                    {
                        venQuery.tax_type = Request.Params["tax_type"].ToString();
                    }
                    else
                    {
                        venQuery.tax_type = "";
                    }
                    #endregion

                    #region   //對聯絡人的 信息處理


                    if (!string.IsNullOrEmpty(Request.Params["conactValues"].ToString()))
                    {
                        string contact = Request.Params["conactValues"].ToString();
                        string[] contactarr = contact.Split('|');
                        string[] contact1;
                        for (int i = 0; i < contactarr.Length - 1; i++)
                        {
                            contact1 = contactarr[i].Split(',');
                            if (i == 0)
                            {
                                venQuery.contact_type_1 = 4;
                                venQuery.contact_name_1 = contact1[1];
                                venQuery.contact_phone_1_1 = contact1[2];
                                venQuery.contact_phone_2_1 = contact1[3];
                                venQuery.contact_mobile_1 = contact1[4];
                                venQuery.contact_email_1 = contact1[5].ToLower();
                            }
                            else if (i == 1)
                            {
                                venQuery.contact_type_2 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_2 = contact1[1];
                                venQuery.contact_phone_1_2 = contact1[2];
                                venQuery.contact_phone_2_2 = contact1[3];
                                venQuery.contact_mobile_2 = contact1[4];
                                venQuery.contact_email_2 = contact1[5].ToLower().ToLower();
                            }
                            else if (i == 2)
                            {
                                venQuery.contact_type_3 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_3 = contact1[1];
                                venQuery.contact_phone_1_3 = contact1[2];
                                venQuery.contact_phone_2_3 = contact1[3];
                                venQuery.contact_mobile_3 = contact1[4];
                                venQuery.contact_email_3 = contact1[5].ToLower();
                            }
                            else if (i == 3)
                            {
                                venQuery.contact_type_4 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_4 = contact1[1];
                                venQuery.contact_phone_1_4 = contact1[2];
                                venQuery.contact_phone_2_4 = contact1[3];
                                venQuery.contact_mobile_4 = contact1[4];
                                venQuery.contact_email_4 = contact1[5].ToLower();
                            }
                            else if (i == 4)
                            {
                                venQuery.contact_type_5 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_5 = contact1[1];
                                venQuery.contact_phone_1_5 = contact1[2];
                                venQuery.contact_phone_2_5 = contact1[3];
                                venQuery.contact_mobile_5 = contact1[4];
                                venQuery.contact_email_5 = contact1[5].ToLower();
                            }

                        }


                    }
                    #endregion
                    venQuery.ip = Request.UserHostAddress;
                    venQuery.file_name = "VendorList.chtml";
                    venQuery.created = DateTime.Now;
                    venQuery.kuser_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    venQuery.export_flag = 1;
                    venQuery.kuser = (int)venQuery.kuser_id;
                    venQuery.kdate = venQuery.created;
                    //供應商類型
                    if (!string.IsNullOrEmpty(Request.Params["gigade_vendor_type"]))
                    {
                        venQuery.vendor_type = Request.Params["gigade_vendor_type"].ToString();
                    }
                    mgr = new CallerMgr(connectionString);
                    Caller caller = new Caller();
                    caller = mgr.GetUserById(Convert.ToInt32(venQuery.kuser_id));
                    venQuery.kuser_name = caller.user_username;

                    return VendorAdd(venQuery);
                }
                else
                {
                    venQuery.vendor_id = Convert.ToUInt32(Request.Params["vendor_id"].ToString());
                    venQuery.vendor_code = Request.Params["vendor_code"].ToString();
                    _vendorMgr = new VendorMgr(connectionString);
                    _configMgr = new ConfigMgr(connectionString);
                    Vendor oldven = _vendorMgr.GetSingle(venQuery);
                    StringBuilder update_log = new StringBuilder();
                    List<TableChangeLog> list = new List<TableChangeLog>();

                    #region 獲取供應商數據
                    uint isUint = 0;//判斷字符串是否能夠轉換為uint類型
                    if (uint.TryParse(Request.Params["vendor_status"].ToString(), out isUint))
                    {
                        venQuery.vendor_status = Convert.ToUInt32(Request.Params["vendor_status"].ToString());
                        if (oldven.vendor_status != venQuery.vendor_status)
                        {//1:啟用2：停用3：失格
                            // update_log.AppendFormat("vendor_status:{0}:{1}:供應商狀態#", oldven.vendor_status, venQuery.vendor_status);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "vendor_status";
                            item.old_value = oldven.vendor_status.ToString();
                            item.new_value = venQuery.vendor_status.ToString();
                            item.field_ch_name = "供應商狀態";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.vendor_status = oldven.vendor_status;
                    }
                    //供應商類型
                    if (!string.IsNullOrEmpty(Request.Params["gigade_vendor_type"]))
                    {
                        venQuery.vendor_type = Request.Params["gigade_vendor_type"].ToString();

                    }
                    int tmp = 0;
                    string[] strs = venQuery.vendor_type.Split(',');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (!int.TryParse(strs[i], out tmp))
                        {
                            venQuery.vendor_type = oldven.vendor_type;
                            break;
                        }
                    }
                    if (oldven.vendor_type != venQuery.vendor_type)
                    {
                        // update_log.AppendFormat("vendor_type:{0}:{1}:供應商類型#", oldven.vendor_type, venQuery.vendor_type);
                        TableChangeLog item = new TableChangeLog();
                        item.change_field = "vendor_type";
                        item.old_value = oldven.vendor_type;
                        item.new_value = venQuery.vendor_type;
                        item.field_ch_name = "供應商類型";
                        list.Add(item);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendor_email"].ToString()))
                    {
                        int total = _vendorMgr.IsExitEmail(Request.Params["vendor_email"].ToString().ToLower());
                        try
                        {
                            if (total > 1)
                            {
                                jsonStr = "{success:true,msg:0}";//返回json數據
                                this.Response.Clear();
                                this.Response.Write(jsonStr);
                                this.Response.End();
                                return this.Response;
                            }
                            else
                            {
                                if (total == 1)
                                {
                                    if (Request.Params["vendor_email"].ToString().ToLower() == oldven.vendor_email.ToLower())
                                    {
                                        venQuery.vendor_email = oldven.vendor_email.ToLower();
                                    }
                                    else
                                    {
                                        jsonStr = "{success:true,msg:0}";//返回json數據rue
                                        this.Response.Clear();
                                        this.Response.Write(jsonStr);
                                        this.Response.End();
                                        return this.Response;
                                    }
                                }
                                else if (total == 0)
                                {
                                    venQuery.vendor_email = Request.Params["vendor_email"].ToString().ToLower();
                                    if (oldven.vendor_email != venQuery.vendor_email)
                                    {
                                        // update_log.AppendFormat("vendor_email:{0}:{1}:公司Email#", oldven.vendor_email, venQuery.vendor_email);
                                        TableChangeLog item = new TableChangeLog();
                                        item.change_field = "vendor_email";
                                        item.old_value = oldven.vendor_email;
                                        item.new_value = venQuery.vendor_email;
                                        item.field_ch_name = "公司Email";
                                        list.Add(item);
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
                            jsonStr = "{success:false,msg:1}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr);
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    else
                    {
                        venQuery.vendor_email = oldven.vendor_email;
                    }
                    //if (!string.IsNullOrEmpty(Request.Params["vendor_password"].ToString()))
                    //{
                    //    BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                    //    venQuery.vendor_password = hash.SHA256Encrypt(Request.Params["vendor_password"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.vendor_password = oldven.vendor_password;
                    //}
                    if (!string.IsNullOrEmpty(Request.Params["vendor_name_full"].ToString()))
                    {
                        venQuery.vendor_name_full = Request.Params["vendor_name_full"].ToString().Trim();
                    }
                    else
                    {
                        venQuery.vendor_name_full = oldven.vendor_name_full;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendor_name_simple"].ToString()))
                    {
                        venQuery.vendor_name_simple = Request.Params["vendor_name_simple"].ToString().Trim();
                    }
                    else
                    {
                        venQuery.vendor_name_simple = oldven.vendor_name_simple;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["vendor_invoice"].ToString()))
                    {
                        venQuery.vendor_invoice = Request.Params["vendor_invoice"].ToString();
                        if (oldven.vendor_invoice != venQuery.vendor_invoice)
                        {
                            // update_log.AppendFormat("vendor_invoice:{0}:{1}:統一編號#", oldven.vendor_invoice, venQuery.vendor_invoice);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "vendor_invoice";
                            item.old_value = oldven.vendor_invoice;
                            item.new_value = venQuery.vendor_invoice;
                            item.field_ch_name = "統一編號";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.vendor_invoice = oldven.vendor_invoice;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_phone"].ToString()))
                    {
                        venQuery.company_phone = Request.Params["company_phone"].ToString();
                        if (oldven.company_phone != venQuery.company_phone)
                        {
                            //update_log.AppendFormat("company_phone:{0}:{1}:公司電話#", oldven.company_phone, venQuery.company_phone);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "company_phone";
                            item.old_value = oldven.company_phone;
                            item.new_value = venQuery.company_phone;
                            item.field_ch_name = "公司電話";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.company_phone = oldven.company_phone;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["company_fax"].ToString()))
                    {
                        venQuery.company_fax = Request.Params["company_fax"].ToString();
                        if (oldven.company_fax != venQuery.company_fax)
                        {
                            // update_log.AppendFormat("company_fax:{0}:{1}:公司傳真#", oldven.company_fax, venQuery.company_fax);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "company_fax";
                            item.old_value = oldven.company_fax;
                            item.new_value = venQuery.company_fax;
                            item.field_ch_name = "公司傳真";
                            list.Add(item);
                        }
                    }


                    if (!string.IsNullOrEmpty(Request.Params["company_person"].ToString()))
                    {
                        venQuery.company_person = Request.Params["company_person"].ToString();
                        if (oldven.company_person != venQuery.company_person)
                        {
                            //update_log.AppendFormat("company_person:{0}:{1}:公司負責人#", oldven.company_person, venQuery.company_person);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "company_person";
                            item.old_value = oldven.company_person;
                            item.new_value = venQuery.company_person;
                            item.field_ch_name = "公司負責人";
                            list.Add(item);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Params["company_zip"].ToString()))
                    {

                        venQuery.company_zip = Convert.ToUInt32(Request.Params["company_zip"].ToString());

                    }

                    venQuery.company_address = Request.Params["company_address"].ToString();
                    if (oldven.company_zip != venQuery.company_zip || oldven.company_address != venQuery.company_address)
                    {
                        //update_log.AppendFormat("company_address:{0}:{1}:公司地址#", oldven.company_zip + "&" + oldven.company_address, venQuery.company_zip + "&" + venQuery.company_address);
                        TableChangeLog item = new TableChangeLog();
                        item.change_field = "company_address";
                        item.old_value = oldven.company_zip + "&" + oldven.company_address;
                        item.new_value = venQuery.company_zip + "&" + venQuery.company_address;
                        item.field_ch_name = "公司地址";
                        list.Add(item);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["invoice_zip"].ToString()))
                    {
                        venQuery.invoice_zip = Convert.ToUInt32(Request.Params["invoice_zip"].ToString());
                    }

                    venQuery.invoice_address = Request.Params["invoice_address"].ToString();

                    if (oldven.invoice_zip != venQuery.invoice_zip || oldven.invoice_address != venQuery.invoice_address)
                    {
                        //update_log.AppendFormat("invoice_address:{0}:{1}:發票地址#", oldven.invoice_zip + "&" + oldven.invoice_address, venQuery.invoice_zip + "&" + venQuery.invoice_address);
                        TableChangeLog item = new TableChangeLog();
                        item.change_field = "invoice_address";
                        item.old_value = oldven.invoice_zip + "&" + oldven.invoice_address;
                        item.new_value = venQuery.invoice_zip + "&" + venQuery.invoice_address;
                        item.field_ch_name = "發票地址";
                        list.Add(item);
                    }
                    venQuery.erp_id = oldven.erp_id;
                    if (uint.TryParse(Request.Params["cost_percent"].ToString(), out isUint))
                    {
                        venQuery.cost_percent = Convert.ToUInt32(Request.Params["cost_percent"].ToString());
                        if (oldven.cost_percent != venQuery.cost_percent)
                        {
                            //update_log.AppendFormat("cost_percent:{0}:{1}:成本百分比#", oldven.cost_percent, venQuery.cost_percent);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "cost_percent";
                            item.old_value = oldven.cost_percent.ToString();
                            item.new_value = venQuery.cost_percent.ToString();
                            item.field_ch_name = "成本百分比";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.cost_percent = 0;
                    }


                    if (uint.TryParse(Request.Params["creditcard_1_percent"].ToString(), out isUint))
                    {
                        venQuery.creditcard_1_percent = Convert.ToUInt32(Request.Params["creditcard_1_percent"].ToString());
                    }
                    else
                    {
                        venQuery.creditcard_1_percent = 0;
                    }

                    if (!string.IsNullOrEmpty(Request.Params["creditcard_3_percent"].ToString()))
                    {
                        venQuery.creditcard_3_percent = Request.Params["creditcard_3_percent"].ToString();
                    }
                    else
                    {
                        venQuery.creditcard_3_percent = "0";
                    }
                    if (!string.IsNullOrEmpty(Request.Params["agreement_createdate"].ToString()))
                    {
                        venQuery.agreement_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["agreement_createdate"].ToString()));
                        if (oldven.agreement_createdate != venQuery.agreement_createdate)
                        {

                            //update_log.AppendFormat("agreement_createdate:{0}:{1}:合約簽訂日期#", CommonFunction.GetNetTime(oldven.agreement_createdate).ToShortDateString(), CommonFunction.GetNetTime(venQuery.agreement_createdate).ToShortDateString());
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "agreement_createdate";
                            item.old_value = CommonFunction.GetNetTime(oldven.agreement_createdate).ToShortDateString();
                            item.new_value = CommonFunction.GetNetTime(venQuery.agreement_createdate).ToShortDateString();
                            item.field_ch_name = "合約簽訂日期";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.agreement_createdate = oldven.agreement_createdate;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["agreement_start"].ToString()))
                    {
                        venQuery.agreement_start = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["agreement_start"].ToString()));
                        if (oldven.agreement_start != venQuery.agreement_start)
                        {
                            // update_log.AppendFormat("agreement_start:{0}:{1}:合約開始日#", CommonFunction.GetNetTime(oldven.agreement_start).ToShortDateString(), CommonFunction.GetNetTime(venQuery.agreement_start).ToShortDateString());
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "agreement_start";
                            item.old_value = CommonFunction.GetNetTime(oldven.agreement_start).ToShortDateString();
                            item.new_value = CommonFunction.GetNetTime(venQuery.agreement_start).ToShortDateString();
                            item.field_ch_name = "合約開始日";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.agreement_start = oldven.agreement_start;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["agreement_end"].ToString()))
                    {
                        venQuery.agreement_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["agreement_end"].ToString()));
                        if (oldven.agreement_end != venQuery.agreement_end)
                        {
                            // update_log.AppendFormat("agreement_end:{0}:{1}:合約結束日#", CommonFunction.GetNetTime(oldven.agreement_end).ToShortDateString(), CommonFunction.GetNetTime(venQuery.agreement_end).ToShortDateString());
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "agreement_end";
                            item.old_value = CommonFunction.GetNetTime(oldven.agreement_end).ToShortDateString();
                            item.new_value = CommonFunction.GetNetTime(venQuery.agreement_end).ToShortDateString();
                            item.field_ch_name = "合約結束日";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.agreement_end = oldven.agreement_end;
                    }
                    if (uint.TryParse(Request.Params["checkout_type"].ToString(), out isUint))
                    {
                        venQuery.checkout_type = Convert.ToUInt32(Request.Params["checkout_type"].ToString());
                    }
                    else
                    {
                        venQuery.checkout_type = oldven.checkout_type;
                    }
                    if (venQuery.checkout_type == 3)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["checkout_other"].ToString()))
                        {
                            venQuery.checkout_other = Request.Params["checkout_other"].ToString();
                        }
                        else
                        {
                            venQuery.checkout_other = oldven.checkout_other;
                        }
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_code"].ToString()))
                    {
                        venQuery.bank_code = Request.Params["bank_code"].ToString();
                        if (oldven.bank_code != venQuery.bank_code)
                        {
                            //update_log.AppendFormat("bank_code:{0}:{1}:銀行代碼#", oldven.bank_code, venQuery.bank_code);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "bank_code";
                            item.old_value = oldven.bank_code;
                            item.new_value = venQuery.bank_code;
                            item.field_ch_name = "銀行代碼";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.bank_code = oldven.bank_code;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_name"].ToString()))
                    {
                        venQuery.bank_name = Request.Params["bank_name"].ToString();
                        if (oldven.bank_name != venQuery.bank_name)
                        {
                            //update_log.AppendFormat("bank_name:{0}:{1}:銀行名稱#", oldven.bank_name, venQuery.bank_name);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "bank_name";
                            item.old_value = oldven.bank_name;
                            item.new_value = venQuery.bank_name;
                            item.field_ch_name = "銀行名稱";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.bank_name = oldven.bank_name;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_number"].ToString()))
                    {
                        venQuery.bank_number = Request.Params["bank_number"].ToString();
                        if (oldven.bank_number != venQuery.bank_number)
                        {
                            // update_log.AppendFormat("bank_number:{0}:{1}:銀行賬號#", oldven.bank_number, venQuery.bank_number);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "bank_number";
                            item.old_value = oldven.bank_number;
                            item.new_value = venQuery.bank_number;
                            item.field_ch_name = "銀行賬號";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.bank_number = oldven.bank_number;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_account"].ToString()))
                    {
                        venQuery.bank_account = Request.Params["bank_account"].ToString();
                        if (oldven.bank_account != venQuery.bank_account)
                        {
                            //update_log.AppendFormat("bank_account:{0}:{1}:銀行戶名#", oldven.bank_account, venQuery.bank_account);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "bank_account";
                            item.old_value = oldven.bank_account;
                            item.new_value = venQuery.bank_account;
                            item.field_ch_name = "銀行戶名";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.bank_account = oldven.bank_account;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_number"].ToString()))
                    {
                        venQuery.bank_number = Request.Params["bank_number"].ToString();
                    }
                    else
                    {
                        venQuery.bank_number = oldven.bank_number;
                    }

                    if (uint.TryParse(Request.Params["freight_low_limit"].ToString(), out isUint))
                    {
                        venQuery.freight_low_limit = Convert.ToUInt32(Request.Params["freight_low_limit"].ToString());
                    }
                    else
                    {
                        venQuery.freight_low_limit = 0;
                    }
                    if (uint.TryParse(Request.Params["freight_low_money"].ToString(), out isUint))
                    {
                        venQuery.freight_low_money = Convert.ToUInt32(Request.Params["freight_low_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_low_money = 0;
                    }
                    if (uint.TryParse(Request.Params["freight_return_low_money"].ToString(), out isUint))
                    {
                        venQuery.freight_return_low_money = Convert.ToUInt32(Request.Params["freight_return_low_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_return_low_money = 0;
                    }
                    if (uint.TryParse(Request.Params["freight_normal_limit"].ToString(), out isUint))
                    {
                        venQuery.freight_normal_limit = Convert.ToUInt32(Request.Params["freight_normal_limit"].ToString());
                    }
                    else
                    {
                        venQuery.freight_normal_limit = 0;
                    }
                    if (uint.TryParse(Request.Params["freight_normal_money"].ToString(), out isUint))
                    {
                        venQuery.freight_normal_money = Convert.ToUInt32(Request.Params["freight_normal_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_normal_money = 0;
                    }
                    if (uint.TryParse(Request.Params["freight_return_normal_money"].ToString(), out isUint))
                    {
                        venQuery.freight_return_normal_money = Convert.ToUInt32(Request.Params["freight_return_normal_money"].ToString());
                    }
                    else
                    {
                        venQuery.freight_return_normal_money = 0;
                    }
                    //if (uint.TryParse(Request.Params["assist"].ToString(), out isUint))
                    //{
                    //    venQuery.assist = Convert.ToUInt32(Request.Params["assist"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.assist = 0;
                    //}
                    //if (uint.TryParse(Request.Params["dispatch"].ToString(), out isUint))
                    //{
                    //    venQuery.dispatch = Convert.ToUInt32(Request.Params["dispatch"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.dispatch = 0;
                    //}
                    //if (uint.TryParse(Request.Params["product_mode"].ToString(), out isUint))
                    //{
                    //    venQuery.product_mode = Convert.ToUInt32(Request.Params["product_mode"].ToString());
                    //}
                    //else
                    //{
                    //    venQuery.product_mode = 0;
                    //}
                    //try
                    //{
                    //    //Regex email = new System.Text.RegularExpressions.Regex("(([a-z0-9]{1})([\\.a-z0-9_-]*)@([a-z0-9]+)(\\.([a-z0-9]+)){1,3})");
                    //    //Regex name = new System.Text.RegularExpressions.Regex("[\u4e00-\u9fa5]");
                    //    //if (email.IsMatch(Request.Params["pm"].ToString()))
                    //    //{
                    //    //    venQuery.product_manage = _configMgr.QueryByEmail(Request.Params["pm"].ToString());
                    //    //}
                    //    //else if (name.IsMatch(Request.Params["pm"].ToString()))
                    //    //{
                    //    //    venQuery.product_manage = _configMgr.QueryByName(Request.Params["pm"].ToString());
                    //    //}
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    //    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    //    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    //    log.Error(logMessage);
                    //    venQuery.product_manage = 0;
                    //}
                    if (!string.IsNullOrEmpty(Request.Params["procurement_days"].ToString()))
                    {
                        venQuery.procurement_days = Convert.ToInt32(Request.Params["procurement_days"].ToString());
                    }
                    else
                    {
                        venQuery.procurement_days = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["self_send_days"].ToString()))
                    {
                        venQuery.self_send_days = Convert.ToInt32(Request.Params["self_send_days"].ToString());
                        if (oldven.self_send_days != venQuery.self_send_days)
                        {
                            //update_log.AppendFormat("self_send_days:{0}:{1}:自出出貨天數#", oldven.self_send_days, venQuery.self_send_days);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "self_send_days";
                            item.old_value = oldven.self_send_days.ToString();
                            item.new_value = venQuery.self_send_days.ToString();
                            item.field_ch_name = "自出出貨天數";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.self_send_days = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["stuff_ware_days"].ToString()))
                    {
                        venQuery.stuff_ware_days = Convert.ToInt32(Request.Params["stuff_ware_days"].ToString());
                        if (oldven.stuff_ware_days != venQuery.stuff_ware_days)
                        {
                            // update_log.AppendFormat("stuff_ware_days:{0}:{1}:寄倉出貨天數#", oldven.stuff_ware_days, venQuery.stuff_ware_days);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "stuff_ware_days";
                            item.old_value = oldven.stuff_ware_days.ToString();
                            item.new_value = venQuery.stuff_ware_days.ToString();
                            item.field_ch_name = "寄倉出貨天數";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.stuff_ware_days = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dispatch_days"].ToString()))
                    {
                        venQuery.dispatch_days = Convert.ToInt32(Request.Params["dispatch_days"].ToString());
                        if (oldven.dispatch_days != venQuery.dispatch_days)
                        {
                            // update_log.AppendFormat("dispatch_days:{0}:{1}:調度出貨天數#", oldven.dispatch_days, venQuery.dispatch_days);
                            TableChangeLog item = new TableChangeLog();
                            item.change_field = "dispatch_days";
                            item.old_value = oldven.dispatch_days.ToString();
                            item.new_value = venQuery.dispatch_days.ToString();
                            item.field_ch_name = "調度出貨天數";
                            list.Add(item);
                        }
                    }
                    else
                    {
                        venQuery.dispatch_days = 0;
                    }
                    if (Request.Params["pm"].ToString() == "")
                    {
                        venQuery.product_manage = 0;
                    }
                    else
                    {
                        if (uint.TryParse(Request.Params["pm"].ToString(), out isUint))
                        {
                            venQuery.product_manage = Convert.ToUInt32(Request.Params["pm"].ToString());
                        }
                        else
                        {
                            venQuery.product_manage = oldven.product_manage;
                        }
                    }
                    if (oldven.product_manage != venQuery.product_manage)
                    {
                        // update_log.AppendFormat("product_manage:{0}:{1}:管理人員#", oldven.product_manage, venQuery.product_manage);
                        TableChangeLog item = new TableChangeLog();
                        item.change_field = "product_manage";
                        item.old_value = oldven.product_manage.ToString();
                        item.new_value = venQuery.product_manage.ToString();
                        item.field_ch_name = "管理人員";
                        list.Add(item);
                    }
                    if (uint.TryParse(Request.Params["gigade_bunus_percent"].ToString(), out isUint))
                    {
                        venQuery.gigade_bunus_percent = Convert.ToUInt32(Request.Params["gigade_bunus_percent"].ToString());
                    }
                    else
                    {
                        venQuery.gigade_bunus_percent = 0;
                    }
                    if (uint.TryParse(Request.Params["gigade_bunus_threshold"].ToString(), out isUint))
                    {
                        venQuery.gigade_bunus_threshold = Convert.ToUInt32(Request.Params["gigade_bunus_threshold"].ToString());
                    }
                    else
                    {
                        venQuery.gigade_bunus_threshold = 0;
                    }
                    venQuery.vendor_note = Request.Params["vendor_note"].ToString();
                    #endregion

                    #region 對聯絡人的 信息處理
                    string delcon = string.Empty;

                    if (!string.IsNullOrEmpty(Request.Params["delconnect"].ToString()))
                    {
                        delcon = Request.Params["delconnect"].ToString().TrimEnd(',');
                        var delArr = delcon.Split(',');
                        int re_1 = 0, re_2 = 0, re_3 = 0;
                        if (delArr.Length == 4)
                        {
                            //update_log.AppendFormat("contact_type_2:{0}:{1}:原第二聯絡人類型#", oldven.contact_type_2, 7);                           
                            //update_log.AppendFormat("contact_name_2:{0}:{1}:原第二聯絡人姓名#", oldven.contact_name_2, "刪除");                          
                            //update_log.AppendFormat("contact_phone_1_2:{0}:{1}:原第二聯絡人電話一#", oldven.contact_phone_1_2, "刪除");
                            //update_log.AppendFormat("contact_phone_2_2:{0}:{1}:原第二聯絡人電話二#", oldven.contact_phone_2_2, "刪除");
                            //update_log.AppendFormat("contact_mobile_2:{0}:{1}:原第二聯絡人手機號碼#", oldven.contact_mobile_2, "刪除");
                            //update_log.AppendFormat("contact_email_2:{0}:{1}:原第二聯絡人郵箱#", oldven.contact_email_2, "刪除");
                            //update_log.AppendFormat("contact_type_3:{0}:{1}:原第三聯絡人類型#", oldven.contact_type_3, 7);
                            //update_log.AppendFormat("contact_name_3:{0}:{1}:原第三聯絡人姓名#", oldven.contact_name_3, "刪除");
                            //update_log.AppendFormat("contact_phone_1_3:{0}:{1}:原第三聯絡人電話一#", oldven.contact_phone_1_3, "刪除");
                            //update_log.AppendFormat("contact_phone_2_3:{0}:{1}:原第三聯絡人電話二#", oldven.contact_phone_2_3, "刪除");
                            //update_log.AppendFormat("contact_mobile_3:{0}:{1}:原第三聯絡人手機號碼#", oldven.contact_mobile_3, "刪除");
                            //update_log.AppendFormat("contact_email_3:{0}:{1}:原第三聯絡人郵箱#", oldven.contact_email_3, "刪除");
                            //update_log.AppendFormat("contact_type_4:{0}:{1}:原第四聯絡人類型#", oldven.contact_type_4, 7);
                            //update_log.AppendFormat("contact_name_4:{0}:{1}:原第四聯絡人姓名#", oldven.contact_name_4, "刪除");
                            //update_log.AppendFormat("contact_phone_1_4:{0}:{1}:原第四聯絡人電話一#", oldven.contact_phone_1_4, "刪除");
                            //update_log.AppendFormat("contact_phone_2_4:{0}:{1}:原第四聯絡人電話二#", oldven.contact_phone_2_4, "刪除");
                            //update_log.AppendFormat("contact_mobile_4:{0}:{1}:原第四聯絡人手機號碼#", oldven.contact_mobile_4, "刪除");
                            //update_log.AppendFormat("contact_email_4:{0}:{1}:原第四聯絡人郵箱#", oldven.contact_email_4, "刪除");
                            //update_log.AppendFormat("contact_type_5:{0}:{1}:第五聯絡人類型#", oldven.contact_type_5, 7);
                            //update_log.AppendFormat("contact_name_5:{0}:{1}:第五聯絡人姓名#", oldven.contact_name_5, "刪除");
                            //update_log.AppendFormat("contact_phone_1_5:{0}:{1}:第五聯絡人電話一#", oldven.contact_phone_1_5, "刪除");
                            //update_log.AppendFormat("contact_phone_2_5:{0}:{1}:第五聯絡人電話二#", oldven.contact_phone_2_5, "刪除");
                            //update_log.AppendFormat("contact_mobile_5:{0}:{1}:第五聯絡人手機號碼#", oldven.contact_mobile_5, "刪除");
                            //update_log.AppendFormat("contact_email_5:{0}:{1}:第五聯絡人郵箱#", oldven.contact_email_5, "刪除");

                            list.Add(new TableChangeLog() { change_field = "contact_type_2", old_value = oldven.contact_type_2.ToString(), new_value = "7", field_ch_name = "原第二聯絡人類型" });
                            list.Add(new TableChangeLog() { change_field = "contact_name_2", old_value = oldven.contact_name_2, new_value = "刪除", field_ch_name = "原第二聯絡人姓名" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_1_2", old_value = oldven.contact_phone_1_2, new_value = "刪除", field_ch_name = "原第二聯絡人電話一" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_2_2", old_value = oldven.contact_phone_2_2, new_value = "刪除", field_ch_name = "原第二聯絡人電話二" });
                            list.Add(new TableChangeLog() { change_field = "contact_mobile_2", old_value = oldven.contact_mobile_2, new_value = "刪除", field_ch_name = "原第二聯絡人手機號碼" });
                            list.Add(new TableChangeLog() { change_field = "contact_email_2", old_value = oldven.contact_email_2, new_value = "刪除", field_ch_name = "原第二聯絡人郵箱" });
                            list.Add(new TableChangeLog() { change_field = "contact_type_3", old_value = oldven.contact_type_3.ToString(), new_value = "7", field_ch_name = "原第三聯絡人類型" });
                            list.Add(new TableChangeLog() { change_field = "contact_name_3", old_value = oldven.contact_name_3, new_value = "刪除", field_ch_name = "原第三聯絡人姓名" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_1_3", old_value = oldven.contact_phone_1_3, new_value = "刪除", field_ch_name = "原第三聯絡人電話一" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_2_3", old_value = oldven.contact_phone_2_3, new_value = "刪除", field_ch_name = "原第三聯絡人電話二" });
                            list.Add(new TableChangeLog() { change_field = "contact_mobile_3", old_value = oldven.contact_mobile_3, new_value = "刪除", field_ch_name = "原第三聯絡人手機號碼" });
                            list.Add(new TableChangeLog() { change_field = "contact_email_3", old_value = oldven.contact_email_3, new_value = "刪除", field_ch_name = "原第三聯絡人郵箱" });
                            list.Add(new TableChangeLog() { change_field = "contact_type_4", old_value = oldven.contact_type_4.ToString(), new_value = "7", field_ch_name = "原第四聯絡人類型" });
                            list.Add(new TableChangeLog() { change_field = "contact_name_4", old_value = oldven.contact_name_4, new_value = "刪除", field_ch_name = "原第四聯絡人姓名" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_1_4", old_value = oldven.contact_phone_1_4, new_value = "刪除", field_ch_name = "原第四聯絡人電話一" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_2_4", old_value = oldven.contact_phone_2_4, new_value = "刪除", field_ch_name = "原第四聯絡人電話二" });
                            list.Add(new TableChangeLog() { change_field = "contact_mobile_4", old_value = oldven.contact_mobile_4, new_value = "刪除", field_ch_name = "原第四聯絡人手機號碼" });
                            list.Add(new TableChangeLog() { change_field = "contact_email_4", old_value = oldven.contact_email_4, new_value = "刪除", field_ch_name = "原第四聯絡人郵箱" });
                            list.Add(new TableChangeLog() { change_field = "contact_type_5", old_value = oldven.contact_type_5.ToString(), new_value = "7", field_ch_name = "第五聯絡人類型" });
                            list.Add(new TableChangeLog() { change_field = "contact_name_5", old_value = oldven.contact_name_5, new_value = "刪除", field_ch_name = "第五聯絡人姓名" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_1_5", old_value = oldven.contact_phone_1_5, new_value = "刪除", field_ch_name = "第五聯絡人電話一" });
                            list.Add(new TableChangeLog() { change_field = "contact_phone_2_5", old_value = oldven.contact_phone_2_5, new_value = "刪除", field_ch_name = "第五聯絡人電話二" });
                            list.Add(new TableChangeLog() { change_field = "contact_mobile_5", old_value = oldven.contact_mobile_5, new_value = "刪除", field_ch_name = "第五聯絡人手機號碼" });
                            list.Add(new TableChangeLog() { change_field = "contact_email_5", old_value = oldven.contact_email_5, new_value = "刪除", field_ch_name = "第五聯絡人郵箱" });
                        }
                        else
                        {
                            for (int i = 0; i < delArr.Length; i++)
                            {
                                int del = Convert.ToInt32(delArr[i]);

                                if (del == 1 && re_1 == 0 && oldven.contact_type_2 != 0)
                                {
                                    re_1++;
                                    //update_log.AppendFormat("contact_type_2:{0}:{1}:原第二聯絡人類型#", oldven.contact_type_2, 7);
                                    //update_log.AppendFormat("contact_name_2:{0}:{1}:原第二聯絡人姓名#", oldven.contact_name_2, "刪除");
                                    //update_log.AppendFormat("contact_phone_1_2:{0}:{1}:原第二聯絡人電話一#", oldven.contact_phone_1_2, "刪除");
                                    //update_log.AppendFormat("contact_phone_2_2:{0}:{1}:原第二聯絡人電話二#", oldven.contact_phone_2_2, "刪除");
                                    //update_log.AppendFormat("contact_mobile_2:{0}:{1}:原第二聯絡人手機號碼#", oldven.contact_mobile_2, "刪除");
                                    //update_log.AppendFormat("contact_email_2:{0}:{1}:原第二聯絡人郵箱#", oldven.contact_email_2, "刪除");

                                    list.Add(new TableChangeLog() { change_field = "contact_type_2", old_value = oldven.contact_type_2.ToString(), new_value = "7", field_ch_name = "原第二聯絡人類型" });
                                    list.Add(new TableChangeLog() { change_field = "contact_name_2", old_value = oldven.contact_name_2, new_value = "刪除", field_ch_name = "原第二聯絡人姓名" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_2", old_value = oldven.contact_phone_1_2, new_value = "刪除", field_ch_name = "原第二聯絡人電話一" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_2", old_value = oldven.contact_phone_2_2, new_value = "刪除", field_ch_name = "原第二聯絡人電話二" });
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_2", old_value = oldven.contact_mobile_2, new_value = "刪除", field_ch_name = "原第二聯絡人手機號碼" });
                                    list.Add(new TableChangeLog() { change_field = "contact_email_2", old_value = oldven.contact_email_2, new_value = "刪除", field_ch_name = "原第二聯絡人郵箱" });

                                }
                                else if (((del == 2 && re_2 == 0) || re_1 == 1) && oldven.contact_type_3 != 0)
                                {
                                    if (re_1 == 1)
                                    {
                                        re_1++;
                                    }
                                    else
                                    {
                                        re_2++;
                                    }
                                    //update_log.AppendFormat("contact_type_3:{0}:{1}:原第三聯絡人類型#", oldven.contact_type_3, 7);
                                    //update_log.AppendFormat("contact_name_3:{0}:{1}:原第三聯絡人姓名#", oldven.contact_name_3, "刪除");
                                    //update_log.AppendFormat("contact_phone_1_3:{0}:{1}:原第三聯絡人電話一#", oldven.contact_phone_1_3, "刪除");
                                    //update_log.AppendFormat("contact_phone_2_3:{0}:{1}:原第三聯絡人電話二#", oldven.contact_phone_2_3, "刪除");
                                    //update_log.AppendFormat("contact_mobile_3:{0}:{1}:原第三聯絡人手機號碼#", oldven.contact_mobile_3, "刪除");
                                    //update_log.AppendFormat("contact_email_3:{0}:{1}:原第三聯絡人郵箱#", oldven.contact_email_3, "刪除");
                                    list.Add(new TableChangeLog() { change_field = "contact_type_3", old_value = oldven.contact_type_3.ToString(), new_value = "7", field_ch_name = "原第三聯絡人類型" });
                                    list.Add(new TableChangeLog() { change_field = "contact_name_3", old_value = oldven.contact_name_3, new_value = "刪除", field_ch_name = "原第三聯絡人姓名" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_3", old_value = oldven.contact_phone_1_3, new_value = "刪除", field_ch_name = "原第三聯絡人電話一" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_3", old_value = oldven.contact_phone_2_3, new_value = "刪除", field_ch_name = "原第三聯絡人電話二" });
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_3", old_value = oldven.contact_mobile_3, new_value = "刪除", field_ch_name = "原第三聯絡人手機號碼" });
                                    list.Add(new TableChangeLog() { change_field = "contact_email_3", old_value = oldven.contact_email_3, new_value = "刪除", field_ch_name = "原第三聯絡人郵箱" });
                                }
                                else if (((del == 3 && re_3 == 0) || re_1 == 2 || re_2 == 1) && oldven.contact_type_4 != 0)
                                {
                                    if (re_1 == 2)
                                    {
                                        re_1++;
                                    }
                                    else if (re_2 == 1)
                                    {
                                        re_2++;
                                    }
                                    else
                                    {
                                        re_3++;
                                    }
                                    //update_log.AppendFormat("contact_type_4:{0}:{1}:原第四聯絡人類型#", oldven.contact_type_4, 7);
                                    //update_log.AppendFormat("contact_name_4:{0}:{1}:原第四聯絡人姓名#", oldven.contact_name_4, "刪除");
                                    //update_log.AppendFormat("contact_phone_1_4:{0}:{1}:原第四聯絡人電話一#", oldven.contact_phone_1_4, "刪除");
                                    //update_log.AppendFormat("contact_phone_2_4:{0}:{1}:原第四聯絡人電話二#", oldven.contact_phone_2_4, "刪除");
                                    //update_log.AppendFormat("contact_mobile_4:{0}:{1}:原第四聯絡人手機號碼#", oldven.contact_mobile_4, "刪除");
                                    //update_log.AppendFormat("contact_email_4:{0}:{1}:原第四聯絡人郵箱#", oldven.contact_email_4, "刪除");
                                    list.Add(new TableChangeLog() { change_field = "contact_type_4", old_value = oldven.contact_type_4.ToString(), new_value = "7", field_ch_name = "原第四聯絡人類型" });
                                    list.Add(new TableChangeLog() { change_field = "contact_name_4", old_value = oldven.contact_name_4, new_value = "刪除", field_ch_name = "原第四聯絡人姓名" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_4", old_value = oldven.contact_phone_1_4, new_value = "刪除", field_ch_name = "原第四聯絡人電話一" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_4", old_value = oldven.contact_phone_2_4, new_value = "刪除", field_ch_name = "原第四聯絡人電話二" });
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_4", old_value = oldven.contact_mobile_4, new_value = "刪除", field_ch_name = "原第四聯絡人手機號碼" });
                                    list.Add(new TableChangeLog() { change_field = "contact_email_4", old_value = oldven.contact_email_4, new_value = "刪除", field_ch_name = "原第四聯絡人郵箱" });
                                }
                                else if ((del == 4 || re_1 == 3 || re_2 == 2 || re_3 == 1) && oldven.contact_type_5 != 0)
                                {


                                    //update_log.AppendFormat("contact_type_5:{0}:{1}:第五聯絡人類型#", oldven.contact_type_5, 7);
                                    //update_log.AppendFormat("contact_name_5:{0}:{1}:第五聯絡人姓名#", oldven.contact_name_5, "刪除");
                                    //update_log.AppendFormat("contact_phone_1_5:{0}:{1}:第五聯絡人電話一#", oldven.contact_phone_1_5, "刪除");
                                    //update_log.AppendFormat("contact_phone_2_5:{0}:{1}:第五聯絡人電話二#", oldven.contact_phone_2_5, "刪除");
                                    //update_log.AppendFormat("contact_mobile_5:{0}:{1}:第五聯絡人手機號碼#", oldven.contact_mobile_5, "刪除");
                                    //update_log.AppendFormat("contact_email_5:{0}:{1}:第五聯絡人郵箱#", oldven.contact_email_5, "刪除");
                                    list.Add(new TableChangeLog() { change_field = "contact_type_5", old_value = oldven.contact_type_5.ToString(), new_value = "7", field_ch_name = "第五聯絡人類型" });
                                    list.Add(new TableChangeLog() { change_field = "contact_name_5", old_value = oldven.contact_name_5, new_value = "刪除", field_ch_name = "第五聯絡人姓名" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_5", old_value = oldven.contact_phone_1_5, new_value = "刪除", field_ch_name = "第五聯絡人電話一" });
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_5", old_value = oldven.contact_phone_2_5, new_value = "刪除", field_ch_name = "第五聯絡人電話二" });
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_5", old_value = oldven.contact_mobile_5, new_value = "刪除", field_ch_name = "第五聯絡人手機號碼" });
                                    list.Add(new TableChangeLog() { change_field = "contact_email_5", old_value = oldven.contact_email_5, new_value = "刪除", field_ch_name = "第五聯絡人郵箱" });
                                }
                            }
                        }
                    }


                    string[] contactarr = null;
                    if (!string.IsNullOrEmpty(Request.Params["conactValues"].ToString()))
                    {

                        string contact = Request.Params["conactValues"].ToString();
                        contactarr = contact.Split('|');
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        string[] contact1 = null;
                        if (contactarr != null && contactarr.Length > i + 1)
                        {
                            contact1 = contactarr[i].Split(',');
                        }
                        if (i == 0)
                        {
                            if (contact1 != null)
                            {
                                venQuery.contact_type_1 = 4;
                                venQuery.contact_name_1 = contact1[1];
                                venQuery.contact_phone_1_1 = contact1[2];
                                venQuery.contact_phone_2_1 = contact1[3];
                                venQuery.contact_mobile_1 = contact1[4];
                                venQuery.contact_email_1 = contact1[5].ToLower();

                                if (oldven.contact_type_1 != venQuery.contact_type_1)
                                {
                                    // update_log.AppendFormat("contact_type_1:{0}:{1}:第一聯絡人類型#", oldven.contact_type_1, venQuery.contact_type_1);
                                    list.Add(new TableChangeLog() { change_field = "contact_type_1", old_value = oldven.contact_type_1.ToString(), new_value = venQuery.contact_type_1.ToString(), field_ch_name = "第一聯絡人類型" });
                                    if (oldven.contact_name_1 != venQuery.contact_name_1)
                                    {
                                        // update_log.AppendFormat("contact_name_1:{0}:{1}:第一聯絡人姓名#", oldven.contact_name_1, venQuery.contact_name_1);
                                        list.Add(new TableChangeLog() { change_field = "contact_name_1", old_value = oldven.contact_name_1, new_value = venQuery.contact_name_1, field_ch_name = "第一聯絡人姓名" });
                                    }
                                    if (oldven.contact_phone_1_1 != venQuery.contact_phone_1_1)
                                    {
                                        //update_log.AppendFormat("contact_phone_1_1:{0}:{1}:第一聯絡人電話一#", oldven.contact_phone_1_1, venQuery.contact_phone_1_1);
                                        list.Add(new TableChangeLog() { change_field = "contact_phone_1_1", old_value = oldven.contact_phone_1_1, new_value = venQuery.contact_phone_1_1, field_ch_name = "第一聯絡人電話一" });
                                    }
                                    if (oldven.contact_phone_2_1 != venQuery.contact_phone_2_1)
                                    {
                                        // update_log.AppendFormat("contact_phone_2_1:{0}:{1}:第一聯絡人電話二#", oldven.contact_phone_2_1, venQuery.contact_phone_2_1);
                                        list.Add(new TableChangeLog() { change_field = "contact_phone_2_1", old_value = oldven.contact_phone_2_1, new_value = venQuery.contact_phone_2_1, field_ch_name = "第一聯絡人電話二" });
                                    }
                                    if (oldven.contact_mobile_1 != venQuery.contact_mobile_1)
                                    {
                                        // update_log.AppendFormat("contact_mobile_1:{0}:{1}:第一聯絡人手機號碼#", oldven.contact_mobile_1, venQuery.contact_mobile_1);
                                        list.Add(new TableChangeLog() { change_field = "contact_mobile_1", old_value = oldven.contact_mobile_1, new_value = venQuery.contact_mobile_1, field_ch_name = "第一聯絡人手機號碼" });
                                    }
                                    if (oldven.contact_email_1 != venQuery.contact_email_1)
                                    {
                                        //update_log.AppendFormat("contact_email_1:{0}:{1}:第一聯絡人郵箱#", oldven.contact_email_1, venQuery.contact_email_1);
                                        list.Add(new TableChangeLog() { change_field = "contact_email_1", old_value = oldven.contact_email_1, new_value = venQuery.contact_email_1, field_ch_name = "第一聯絡人郵箱" });
                                    }
                                }
                            }
                        }
                        else if (i == 1)
                        {


                            if (contact1 != null)
                            {
                                venQuery.contact_type_2 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_2 = contact1[1];
                                venQuery.contact_phone_1_2 = contact1[2];
                                venQuery.contact_phone_2_2 = contact1[3];
                                venQuery.contact_mobile_2 = contact1[4];
                                venQuery.contact_email_2 = contact1[5].ToLower().ToLower();

                                if (oldven.contact_type_2 != venQuery.contact_type_2)
                                {
                                    // update_log.AppendFormat("contact_type_2:{0}:{1}:第二聯絡人類型#", oldven.contact_type_2, venQuery.contact_type_2);
                                    list.Add(new TableChangeLog() { change_field = "contact_type_2", old_value = oldven.contact_type_2.ToString(), new_value = venQuery.contact_type_2.ToString(), field_ch_name = "第二聯絡人類型" });
                                }
                                if (oldven.contact_name_2 != venQuery.contact_name_2)
                                {
                                    //update_log.AppendFormat("contact_name_2:{0}:{1}:第二聯絡人姓名#", oldven.contact_name_2, venQuery.contact_name_2);
                                    list.Add(new TableChangeLog() { change_field = "contact_name_2", old_value = oldven.contact_name_2, new_value = venQuery.contact_name_2, field_ch_name = "第二聯絡人姓名" });
                                }
                                if (oldven.contact_phone_1_2 != venQuery.contact_phone_1_2)
                                {
                                    //update_log.AppendFormat("contact_phone_1_2:{0}:{1}:第二聯絡人電話一#", oldven.contact_phone_1_2, venQuery.contact_phone_1_2);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_2", old_value = oldven.contact_phone_1_2, new_value = venQuery.contact_phone_1_2, field_ch_name = "第二聯絡人電話一" });
                                }
                                if (oldven.contact_phone_2_2 != venQuery.contact_phone_2_2)
                                {
                                    //update_log.AppendFormat("contact_phone_2_2:{0}:{1}:第二聯絡人電話二#", oldven.contact_phone_2_2, venQuery.contact_phone_2_2);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_2", old_value = oldven.contact_phone_2_2, new_value = venQuery.contact_phone_2_2, field_ch_name = "第二聯絡人電話二" });
                                }
                                if (oldven.contact_mobile_2 != venQuery.contact_mobile_2)
                                {
                                    // update_log.AppendFormat("contact_mobile_2:{0}:{1}:第二聯絡人手機號碼#", oldven.contact_mobile_2, venQuery.contact_mobile_2);
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_2", old_value = oldven.contact_mobile_2, new_value = venQuery.contact_mobile_2, field_ch_name = "第二聯絡人手機號碼" });
                                }
                                if (oldven.contact_email_2 != venQuery.contact_email_2)
                                {
                                    // update_log.AppendFormat("contact_email_2:{0}:{1}:第二聯絡人郵箱#", oldven.contact_email_2, venQuery.contact_email_2);
                                    list.Add(new TableChangeLog() { change_field = "contact_email_2", old_value = oldven.contact_email_2, new_value = venQuery.contact_email_2, field_ch_name = "第二聯絡人郵箱" });
                                }
                            }
                        }
                        else if (i == 2)
                        {


                            if (contact1 != null)
                            {
                                venQuery.contact_type_3 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_3 = contact1[1];
                                venQuery.contact_phone_1_3 = contact1[2];
                                venQuery.contact_phone_2_3 = contact1[3];
                                venQuery.contact_mobile_3 = contact1[4];
                                venQuery.contact_email_3 = contact1[5].ToLower();
                                if (oldven.contact_type_3 != venQuery.contact_type_3)
                                {
                                    //update_log.AppendFormat("contact_type_3:{0}:{1}:第三聯絡人類型#", oldven.contact_type_3, venQuery.contact_type_3);
                                    list.Add(new TableChangeLog() { change_field = "contact_type_3", old_value = oldven.contact_type_3.ToString(), new_value = venQuery.contact_type_3.ToString(), field_ch_name = "第三聯絡人類型" });
                                }
                                if (oldven.contact_name_3 != venQuery.contact_name_3)
                                {
                                    //update_log.AppendFormat("contact_name_3:{0}:{1}:第三聯絡人姓名#", oldven.contact_name_3, venQuery.contact_name_3);
                                    list.Add(new TableChangeLog() { change_field = "contact_name_3", old_value = oldven.contact_name_3, new_value = venQuery.contact_name_3, field_ch_name = "第三聯絡人姓名" });
                                }
                                if (oldven.contact_phone_1_3 != venQuery.contact_phone_1_3)
                                {
                                    // update_log.AppendFormat("contact_phone_1_3:{0}:{1}:第三聯絡人電話一#", oldven.contact_phone_1_3, venQuery.contact_phone_1_3);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_3", old_value = oldven.contact_phone_1_3, new_value = venQuery.contact_phone_1_3, field_ch_name = "第三聯絡人電話一" });
                                }
                                if (oldven.contact_phone_2_3 != venQuery.contact_phone_2_3)
                                {
                                    // update_log.AppendFormat("contact_phone_2_3:{0}:{1}:第三聯絡人電話二#", oldven.contact_phone_2_3, venQuery.contact_phone_2_3);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_3", old_value = oldven.contact_phone_2_3, new_value = venQuery.contact_phone_2_3, field_ch_name = "第三聯絡人電話二" });
                                }
                                if (oldven.contact_mobile_3 != venQuery.contact_mobile_3)
                                {
                                    //  update_log.AppendFormat("contact_mobile_3:{0}:{1}:第三聯絡人手機號碼#", oldven.contact_mobile_3, venQuery.contact_mobile_3);
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_3", old_value = oldven.contact_mobile_3, new_value = venQuery.contact_mobile_3, field_ch_name = "第三聯絡人手機號碼" });
                                }
                                if (oldven.contact_email_3 != venQuery.contact_email_3)
                                {
                                    // update_log.AppendFormat("contact_email_3:{0}:{1}:第三聯絡人郵箱#", oldven.contact_email_3, venQuery.contact_email_3);
                                    list.Add(new TableChangeLog() { change_field = "contact_email_3", old_value = oldven.contact_email_3, new_value = venQuery.contact_email_3, field_ch_name = "第三聯絡人郵箱" });
                                }
                            }
                        }
                        else if (i == 3)
                        {


                            if (contact1 != null)
                            {
                                venQuery.contact_type_4 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_4 = contact1[1];
                                venQuery.contact_phone_1_4 = contact1[2];
                                venQuery.contact_phone_2_4 = contact1[3];
                                venQuery.contact_mobile_4 = contact1[4];
                                venQuery.contact_email_4 = contact1[5].ToLower();
                                if (oldven.contact_type_4 != venQuery.contact_type_4)
                                {
                                    //update_log.AppendFormat("contact_type_4:{0}:{1}:第四聯絡人類型#", oldven.contact_type_4, venQuery.contact_type_4);
                                    list.Add(new TableChangeLog() { change_field = "contact_type_4", old_value = oldven.contact_type_4.ToString(), new_value = venQuery.contact_type_4.ToString(), field_ch_name = "原第四聯絡人類型" });
                                }
                                if (oldven.contact_name_4 != venQuery.contact_name_4)
                                {
                                    // update_log.AppendFormat("contact_name_4:{0}:{1}:第四聯絡人姓名#", oldven.contact_name_4, venQuery.contact_name_4);
                                    list.Add(new TableChangeLog() { change_field = "contact_name_4", old_value = oldven.contact_name_4, new_value = venQuery.contact_name_4, field_ch_name = "第四聯絡人姓名" });
                                }
                                if (oldven.contact_phone_1_4 != venQuery.contact_phone_1_4)
                                {
                                    //update_log.AppendFormat("contact_phone_1_4:{0}:{1}:第四聯絡人電話一#", oldven.contact_phone_1_4, venQuery.contact_phone_1_4);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_4", old_value = oldven.contact_phone_1_4, new_value = venQuery.contact_phone_1_4, field_ch_name = "第四聯絡人電話一" });
                                }
                                if (oldven.contact_phone_2_4 != venQuery.contact_phone_2_4)
                                {
                                    // update_log.AppendFormat("contact_phone_2_4:{0}:{1}:第四聯絡人電話二#", oldven.contact_phone_2_4, venQuery.contact_phone_2_4);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_4", old_value = oldven.contact_phone_2_4, new_value = venQuery.contact_phone_2_4, field_ch_name = "第四聯絡人電話二" });
                                }
                                if (oldven.contact_mobile_4 != venQuery.contact_mobile_4)
                                {
                                    // update_log.AppendFormat("contact_mobile_4:{0}:{1}:第四聯絡人手機號碼#", oldven.contact_mobile_4, venQuery.contact_mobile_4);
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_4", old_value = oldven.contact_mobile_4, new_value = venQuery.contact_mobile_4, field_ch_name = "第四聯絡人手機號碼" });
                                }
                                if (oldven.contact_email_4 != venQuery.contact_email_4)
                                {
                                    // update_log.AppendFormat("contact_email_4:{0}:{1}:第四聯絡人郵箱#", oldven.contact_email_4, venQuery.contact_email_4);
                                    list.Add(new TableChangeLog() { change_field = "contact_email_4", old_value = oldven.contact_email_4, new_value = venQuery.contact_email_4, field_ch_name = "第四聯絡人郵箱" });
                                }
                            }
                        }
                        else if (i == 4)
                        {


                            if (contact1 != null)
                            {
                                venQuery.contact_type_5 = Convert.ToUInt32(ContactType(contact1[0].ToString()));
                                venQuery.contact_name_5 = contact1[1];
                                venQuery.contact_phone_1_5 = contact1[2];
                                venQuery.contact_phone_2_5 = contact1[3];
                                venQuery.contact_mobile_5 = contact1[4];
                                venQuery.contact_email_5 = contact1[5].ToLower();
                                if (oldven.contact_type_5 != venQuery.contact_type_5)
                                {
                                    //update_log.AppendFormat("contact_type_5:{0}:{1}:第五聯絡人類型#", oldven.contact_type_5, venQuery.contact_type_5);
                                    list.Add(new TableChangeLog() { change_field = "contact_type_5", old_value = oldven.contact_type_5.ToString(), new_value = venQuery.contact_type_5.ToString(), field_ch_name = "第五聯絡人類型" });
                                }
                                if (oldven.contact_name_5 != venQuery.contact_name_5)
                                {
                                    // update_log.AppendFormat("contact_name_5:{0}:{1}:第五聯絡人姓名#", oldven.contact_name_5, venQuery.contact_name_5);
                                    list.Add(new TableChangeLog() { change_field = "contact_name_5", old_value = oldven.contact_name_5, new_value = venQuery.contact_name_5, field_ch_name = "第五聯絡人姓名" });
                                }
                                if (oldven.contact_phone_1_5 != venQuery.contact_phone_1_5)
                                {
                                    // update_log.AppendFormat("contact_phone_1_5:{0}:{1}:第五聯絡人電話一#", oldven.contact_phone_1_5, venQuery.contact_phone_1_5);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_1_5", old_value = oldven.contact_phone_1_5, new_value = venQuery.contact_phone_1_5, field_ch_name = "第五聯絡人電話一" });
                                }
                                if (oldven.contact_phone_2_5 != venQuery.contact_phone_2_5)
                                {
                                    // update_log.AppendFormat("contact_phone_2_5:{0}:{1}:第五聯絡人電話二#", oldven.contact_phone_2_5, venQuery.contact_phone_2_5);
                                    list.Add(new TableChangeLog() { change_field = "contact_phone_2_5", old_value = oldven.contact_phone_2_5, new_value = venQuery.contact_phone_2_5, field_ch_name = "第五聯絡人電話二" });
                                }
                                if (oldven.contact_mobile_5 != venQuery.contact_mobile_5)
                                {
                                    //update_log.AppendFormat("contact_mobile_5:{0}:{1}:第五聯絡人手機號碼#", oldven.contact_mobile_5, venQuery.contact_mobile_5);
                                    list.Add(new TableChangeLog() { change_field = "contact_mobile_5", old_value = oldven.contact_mobile_5, new_value = venQuery.contact_mobile_5, field_ch_name = "第五聯絡人手機號碼" });
                                }
                                if (oldven.contact_email_5 != venQuery.contact_email_5)
                                {
                                    //update_log.AppendFormat("contact_email_5:{0}:{1}:第五聯絡人郵箱#", oldven.contact_email_5, venQuery.contact_email_5);
                                    list.Add(new TableChangeLog() { change_field = "contact_email_5", old_value = oldven.contact_email_5, new_value = venQuery.contact_email_5, field_ch_name = "第五聯絡人郵箱" });
                                }
                            }
                        }
                    }

                    #endregion


                    venQuery.ip = Request.UserHostAddress;
                    venQuery.file_name = "VendorList.chtml";
                    venQuery.created = DateTime.Now;
                    venQuery.kuser_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    venQuery.export_flag = 1;
                    mgr = new CallerMgr(connectionString);
                    Caller caller = new Caller();
                    caller = mgr.GetUserById(Convert.ToInt32(venQuery.kuser_id));
                    venQuery.kuser_name = caller.user_username;
                    return VendorEdit(venQuery, list);
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

        /// <summary>
        /// 保存供應商數據
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase VendorAdd(VendorQuery venQuery)
        {
            string jsonStr = "{success:false}";
            try
            {
                _vendorMgr = new VendorMgr(connectionString);

                if (!string.IsNullOrEmpty(Request.Params["pm"]))
                {
                    venQuery.product_manage = Convert.ToUInt32(Request.Params["pm"]);
                }
                int result = _vendorMgr.Add(venQuery);
                if (result > 0)
                {

                    jsonStr = "{success:true}";

                }
                else if (result == -1)
                {
                    jsonStr = "{success:false,msg:-1}";
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

        /// <summary>
        /// 保存供應商數據
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase VendorEdit(VendorQuery venQuery, List<TableChangeLog> list)
        {
            string jsonStr = "{success:false}";
            _vendorMgr = new VendorMgr(connectionString);
            _productMgr = new ProductMgr(connectionString);
            try
            {
                if (_vendorMgr.Update(venQuery, list) > 0)
                {
                    //變更商品可販售狀態
                    if (_productMgr.UpdateSaleStatusByCondition(new Product { Vendor_Id = venQuery.vendor_id }))
                    {
                        jsonStr = "{success:true}";
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

            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        #region 聯絡人窗口屬性 int ContactType()
        /// <summary>
        /// 獲取管理人員 product_manager 8個人
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public int ContactType(string contacttype)
        {
            int type = 0;
            switch (contacttype)
            {
                case "負責人":
                    type = 1;
                    break;
                case "業務窗口":
                    type = 2;
                    break;
                case "圖/文窗口":
                    type = 3;
                    break;
                case "出貨負責窗口":
                    type = 4;
                    break;
                case "帳務連絡窗口":
                    type = 5;
                    break;
                case "客服窗口":
                    type = 6;
                    break;
            }
            return type;
        }
        #endregion


        public string ContactTypeToStr(string contacttype)
        {
            string typestr = string.Empty;

            switch (contacttype)
            {
                case "1":
                    typestr = "負責人";
                    break;
                case "2":
                    typestr = "業務窗口";
                    break;
                case "3":
                    typestr = "圖/文窗口";
                    break;
                case "4":
                    typestr = "出貨負責窗口";
                    break;
                case "5":
                    typestr = "帳務連絡窗口";
                    break;
                case "6":
                    typestr = "客服窗口";
                    break;
            }
            return typestr;
        }
        #endregion
        #endregion



        #region 供應商詳細信息匯出csv + HttpResponseBase VendorExportCsv()
        /// <summary>
        /// 匯出csv
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase VendorExportCsv()
        {
            string newCsvName = string.Empty;
            string json = string.Empty;
            _vendorMgr = new VendorMgr(connectionString);
            string sqlwhere = string.Empty;
            string type = string.Empty;
            string content = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["dateType"]))
            {
                type = Request.Params["dateType"];
            }
            if (!string.IsNullOrEmpty(Request.Params["dateCon"]))
            {
                content = Request.Params["dateCon"];
            }
            // string status = Request.Params["dateStatus"];
            string agreementStart = string.Empty;
            string agreementEnd = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
            {
                agreementStart = Convert.ToDateTime(Request.Params["dateOne"]).ToString("yyyy-MM-dd 00:00:00");
            }
            if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
            {
                agreementEnd = Convert.ToDateTime(Request.Params["dateTwo"]).ToString("yyyy-MM-dd 23:59:59");
            }
            //匯出不需要查詢狀態
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(type) && type != "0")
            {
                switch (type)
                {
                    case "1":
                        sqlwhere += " AND vendor_email LIKE '%" + content + "%'";
                        break;
                    case "2":
                        sqlwhere += " AND vendor_name_simple  LIKE '%" + content + "%'";
                        break;
                    case "3":
                        sqlwhere += " AND  vendor_name_full LIKE '%" + content + "%'";
                        break;
                    case "4":
                        sqlwhere += " AND vendor_invoice LIKE '%" + content + "%'";
                        break;
                    case "5":
                        sqlwhere += " AND vendor.erp_id LIKE '%" + content + "%'";
                        break;
                    case "6":
                        uint isTranUint = 0;
                        if (uint.TryParse(content, out isTranUint))
                        {
                            if (content != "0")
                            {
                                sqlwhere += " AND vendor_id = '" + content + "'";
                            }
                        }
                        break;
                    case "7":
                        sqlwhere += " AND vendor_code LIKE '%" + content + "%'";
                        break;
                    default:
                        sqlwhere += "";
                        break;

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Params["vendortype"]))//供应商类型
                {
                    string vondertype = Request.Params["vendortype"].ToString();
                    sqlwhere += " and (";
                    string[] checks = vondertype.Substring(0, vondertype.Length - 1).Split(',');
                    int num = 0;
                    for (int i = 0; i < checks.Length; i++)
                    {
                        if (num == 0)
                        {
                            sqlwhere += " vendor_type like '%" + checks[i] + "%'";
                            num++;
                        }
                        else
                        {
                            sqlwhere += " or vendor_type like '%" + checks[i] + "%'";
                            num++;
                        }
                    }
                    sqlwhere += " ) ";
                }
                if (!string.IsNullOrEmpty(agreementStart))
                {
                    sqlwhere += " AND agreement_createdate >=" + CommonFunction.GetPHPTime(agreementStart);
                }
                if (!string.IsNullOrEmpty(agreementEnd))
                {
                    sqlwhere += " AND agreement_createdate <=" + CommonFunction.GetPHPTime(agreementEnd);
                }
            }
            DataTable dt = new DataTable();
            dt = _vendorMgr.GetVendorDetail(sqlwhere);

            DataTable dtHZ = new DataTable();
            dtHZ.Columns.Add("供應商編號", typeof(String));
            dtHZ.Columns.Add("供應商編碼", typeof(String));
            dtHZ.Columns.Add("狀態", typeof(String));
            dtHZ.Columns.Add("供應商名稱", typeof(String));
            dtHZ.Columns.Add("供應商簡稱", typeof(String));
            dtHZ.Columns.Add("統一編號", typeof(String));
            dtHZ.Columns.Add("傳真", typeof(String));
            dtHZ.Columns.Add("負責人", typeof(String));
            dtHZ.Columns.Add("公司地址", typeof(String));
            dtHZ.Columns.Add("發票地址", typeof(String));
            dtHZ.Columns.Add("成本百分比", typeof(String));
            dtHZ.Columns.Add("信用卡一期百分比", typeof(String));
            dtHZ.Columns.Add("信用卡三期百分比", typeof(String));
            dtHZ.Columns.Add("合約簽定日", typeof(String));
            dtHZ.Columns.Add("合約期間", typeof(String));
            dtHZ.Columns.Add("結帳方式", typeof(String));
            dtHZ.Columns.Add("銀行代碼", typeof(String));
            dtHZ.Columns.Add("銀行名稱", typeof(String));
            dtHZ.Columns.Add("銀行帳號", typeof(String));
            dtHZ.Columns.Add("銀行戶名", typeof(String));
            dtHZ.Columns.Add("常溫運費", typeof(String));
            dtHZ.Columns.Add("常溫運費門檻", typeof(String));
            dtHZ.Columns.Add("常溫逆物流費", typeof(String));
            dtHZ.Columns.Add("低溫運費", typeof(String));
            dtHZ.Columns.Add("低溫運費門檻", typeof(String));
            dtHZ.Columns.Add("低溫逆物流費", typeof(String));
            dtHZ.Columns.Add("負責PM", typeof(String));

            dtHZ.Columns.Add("業績獎金門檻", typeof(String));
            dtHZ.Columns.Add("獎金百分比", typeof(String));
            dtHZ.Columns.Add("採購天數", typeof(String));
            dtHZ.Columns.Add("自出出貨天數", typeof(String));
            dtHZ.Columns.Add("寄倉出貨天數", typeof(String));
            dtHZ.Columns.Add("調度出貨天數", typeof(String));
            dtHZ.Columns.Add("調度倉模式", typeof(String));

            dtHZ.Columns.Add("出貨窗口聯絡人", typeof(String));
            dtHZ.Columns.Add("連絡電話", typeof(String));
            dtHZ.Columns.Add("聯絡手機", typeof(String));
            dtHZ.Columns.Add("聯絡Mail", typeof(String));

            dtHZ.Columns.Add("第二聯絡人類型", typeof(String));
            dtHZ.Columns.Add("第二聯絡人姓名", typeof(String));
            dtHZ.Columns.Add("第二連絡電話", typeof(String));
            dtHZ.Columns.Add("第二聯絡手機", typeof(String));
            dtHZ.Columns.Add("第二聯絡Mail", typeof(String));

            dtHZ.Columns.Add("第三聯絡人類型", typeof(String));
            dtHZ.Columns.Add("第三聯絡人姓名", typeof(String));
            dtHZ.Columns.Add("第三連絡電話", typeof(String));
            dtHZ.Columns.Add("第三聯絡手機", typeof(String));
            dtHZ.Columns.Add("第三聯絡Mail", typeof(String));

            dtHZ.Columns.Add("第四聯絡人類型", typeof(String));
            dtHZ.Columns.Add("第四聯絡人姓名", typeof(String));
            dtHZ.Columns.Add("第四連絡電話", typeof(String));
            dtHZ.Columns.Add("第四聯絡手機", typeof(String));
            dtHZ.Columns.Add("第四聯絡Mail", typeof(String));

            dtHZ.Columns.Add("第五聯絡人類型", typeof(String));
            dtHZ.Columns.Add("第五聯絡人姓名", typeof(String));
            dtHZ.Columns.Add("第五連絡電話", typeof(String));
            dtHZ.Columns.Add("第五聯絡手機", typeof(String));
            dtHZ.Columns.Add("第五聯絡Mail", typeof(String));


            dtHZ.Columns.Add("備註", typeof(String));
            try
            {
                //"電話"   "電子信箱",
                //string[] colname ={"供應商編號","供應商編碼","狀態","供應商名稱","供應商簡稱","統一編號","傳真","負責人",
                //               "公司地址","發票地址","成本百分比","信用卡一期百分比","信用卡三期百分比",
                //               "合約簽定日","合約期間","結帳方式","銀行代碼","銀行名稱","銀行帳號","銀行戶名",
                //               "常溫運費","常溫運費門檻","常溫逆物流費","低溫運費","低溫運費門檻","低溫逆物流費",
                //               "負責PM","出貨窗口聯絡人","連絡電話","聯絡手機","聯絡Mail","業績獎金門檻","獎金百分比","採購天數"
                //               ,"自出出貨天數","寄倉出貨天數","調度出貨天數","調度倉模式","備註"
                //              };                                      
                string filename = "供應商列表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                newCsvName = Server.MapPath(excelPath) + filename;
                foreach (DataRow dr in dt.Rows)
                {
                    dr["company_address"] = CommonFunction.ZipAddress(dr["company_zip"].ToString()) + " " + dr["company_address"].ToString();
                    dr["invoice_address"] = CommonFunction.ZipAddress(dr["invoice_zip"].ToString()) + " " + dr["invoice_address"].ToString();
                    if (string.IsNullOrEmpty(dr["user_username"].ToString()))
                    {
                        dr["user_username"] = "未設定";
                    }
                    string temp = string.Empty;
                    if (Convert.ToInt32(dr["self_send_days"]) == 0)
                    {
                        if (Convert.ToInt32(dr["stuff_ware_days"]) == 0)
                        {
                            if (Convert.ToInt32(dr["dispatch_days"]) == 0)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = "調度";
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(dr["dispatch_days"]) == 0)
                            {
                                temp = "寄倉";
                            }
                            else
                            {
                                temp = "寄倉，調度";
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(dr["stuff_ware_days"]) == 0)
                        {
                            if (Convert.ToInt32(dr["dispatch_days"]) == 0)
                            {
                                temp = "自出";
                            }
                            else
                            {
                                temp = "自出，調度";
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(dr["dispatch_days"]) == 0)
                            {
                                temp = "自出，寄倉";
                            }
                            else
                            {
                                temp = "自出，寄倉，調度";
                            }
                        }
                    }
                    dr["vendor_mode"] = temp;

                    dr["vendor_note"] = dr["vendor_note"].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                }
                foreach (DataRow dr_v in dt.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dr_v["vendor_id"].ToString();
                    dr[1] = dr_v["vendor_code"].ToString();
                    dr[2] = dr_v["case vendor_status when 1 then '啟用' when 2 then '停用' when 3 then '失格' end"].ToString();
                    dr[3] = dr_v["vendor_name_full"].ToString();
                    dr[4] = dr_v["vendor_name_simple"].ToString();
                    dr[5] = dr_v["vendor_invoice"].ToString();
                    dr[6] = dr_v["company_fax"].ToString();
                    dr[7] = dr_v["company_person"].ToString();
                    dr[8] = dr_v["company_address"].ToString();
                    dr[9] = dr_v["invoice_address"].ToString();
                    dr[10] = dr_v["cost_percent"].ToString();
                    dr[11] = dr_v["creditcard_1_percent"].ToString();
                    dr[12] = dr_v["creditcard_3_percent"].ToString();
                    dr[13] = dr_v["FROM_UNIXTIME(agreement_createdate,'%Y/%m/%d')"].ToString();
                    dr[14] = dr_v["agreement_duration"].ToString();
                    dr[15] = dr_v["checkout_type"].ToString();
                    dr[16] = dr_v["bank_code"].ToString();
                    dr[17] = dr_v["bank_name"].ToString();
                    dr[18] = dr_v["bank_number"].ToString();
                    dr[19] = dr_v["bank_account"].ToString();
                    dr[20] = dr_v["freight_normal_money"].ToString();
                    dr[21] = dr_v["freight_normal_limit"].ToString();
                    dr[22] = dr_v["freight_return_normal_money"].ToString();
                    dr[23] = dr_v["freight_low_money"].ToString();
                    dr[24] = dr_v["freight_low_limit"].ToString();
                    dr[25] = dr_v["freight_return_low_money"].ToString();
                    dr[26] = dr_v["user_username"].ToString();

                    dr[27] = dr_v["gigade_bunus_threshold"].ToString();
                    dr[28] = dr_v["gigade_bunus_percent"].ToString();
                    dr[29] = dr_v["procurement_days"].ToString();
                    dr[30] = dr_v["self_send_days"].ToString();
                    dr[31] = dr_v["stuff_ware_days"].ToString();
                    dr[32] = dr_v["dispatch_days"].ToString();
                    dr[33] = dr_v["vendor_mode"].ToString();


                    dr[34] = dr_v["contact_name_1"].ToString();
                    dr[35] = dr_v["contact_phone_1_1"].ToString();
                    dr[36] = dr_v["contact_mobile_1"].ToString();
                    dr[37] = dr_v["contact_email_1"].ToString();

                    //dr[31] = dr_v["contact_type_2"].ToString();
                    dr[38] = ContactTypeToStr(dr_v["contact_type_2"].ToString());
                    dr[39] = dr_v["contact_name_2"].ToString();
                    dr[40] = dr_v["contact_phone_1_2"].ToString();
                    dr[41] = dr_v["contact_mobile_2"].ToString();
                    dr[42] = dr_v["contact_email_2"].ToString();

                    dr[43] = ContactTypeToStr(dr_v["contact_type_3"].ToString());
                                        
                    dr[44] = dr_v["contact_name_3"].ToString();
                    dr[45] = dr_v["contact_phone_1_3"].ToString();
                    dr[46] = dr_v["contact_mobile_3"].ToString();
                    dr[47] = dr_v["contact_email_3"].ToString();
                    dr[48] = ContactTypeToStr(dr_v["contact_type_4"].ToString());
                    dr[49] = dr_v["contact_name_4"].ToString();
                    dr[50] = dr_v["contact_phone_1_4"].ToString();
                    dr[51] = dr_v["contact_mobile_4"].ToString();
                    dr[52] = dr_v["contact_email_4"].ToString();
                    dr[53] = ContactTypeToStr(dr_v["contact_type_5"].ToString());
                    dr[54] = dr_v["contact_name_5"].ToString();
                    dr[55] = dr_v["contact_phone_1_5"].ToString();
                    dr[56] = dr_v["contact_mobile_5"].ToString();
                    dr[57] = dr_v["contact_email_5"].ToString();


                    dr[58] = dr_v["vendor_note"].ToString();
                    dtHZ.Rows.Add(dr);
                }


                dt.Columns.Remove("company_zip");
                dt.Columns.Remove("invoice_zip");
                dt.Columns.Remove("product_manage");
                dt.Columns.Remove("company_phone");//電話
                dt.Columns.Remove("vendor_email");//郵箱
                if (System.IO.File.Exists(newCsvName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newCsvName, FileAttributes.Normal);
                    System.IO.File.Delete(newCsvName);
                }
                ExcelHelperXhf.ExportDataTabletoExcel(dtHZ, "", newCsvName);
                json = "true," + filename;
                //if (System.IO.File.Exists(newCsvName))
                //{
                //    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename));
                //    Response.ContentType = "application/ms-excel";
                //    //Response.WriteFile(newCsvName);
                //    Response.Flush();
                //    Response.End();

                //}
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false, ";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }

        #endregion

        #region 獲取供應商品牌列表數據 HttpResponseBase GetVendorBrandList()
        /// <summary>
        /// 獲取供應商品牌列表數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendorBrandList()
        {
            string jsonStr = string.Empty;
            List<VendorBrandSetQuery> stores = new List<VendorBrandSetQuery>();
            try
            {
                query = new VendorBrandSetQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
                int totalCount = 0;
                query.SearchType = Convert.ToInt32(Request.Params["serchs"]);
                query.SearchCondition = Request.Params["serchcontent"];
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    query.Brand_Id = Convert.ToUInt32(Request.Params["relation_id"].ToString());
                }
                stores = _IvendorBrandSet.GetVendorBrandList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                #region todo:自己优化的方法--代码较多，但速度快了很多
                string brandId = " ";
                if (totalCount != 0)
                {
                    foreach (var item in stores)
                    {
                        brandId += item.Brand_Id + ",";
                    }
                    brandId = brandId.Remove(brandId.LastIndexOf(','));
                    _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
                    DataTable dtShopName = _IvendorBrandSet.GetShop(brandId);
                    foreach (var item in stores)
                    {
                        DataRow[] rShopName = dtShopName.Select("Brand_Id='" + item.Brand_Id + "'");
                        string classIds = "";
                        string class_name = "";
                        if (rShopName.Length > 0)
                        {
                            foreach (var rows in rShopName)
                            {
                                classIds += rows["class_id"] + ",";
                                class_name += rows["class_name"] + "|";
                            }
                            item.classIds = classIds.Remove(classIds.LastIndexOf(','));
                            item.class_name = class_name.Remove(class_name.LastIndexOf('|'));
                        }

                        if (Convert.ToBoolean(Request.Params["isSecret"]))
                        {

                            if (!string.IsNullOrEmpty(item.vendor_name_full))
                            {
                                item.vendor_name_full = item.vendor_name_full.Substring(0, 1) + "**";
                            }

                            if (!string.IsNullOrEmpty(item.vendor_name_simple))
                            {
                                item.vendor_name_simple = item.vendor_name_simple.Substring(0, 1) + "**";
                            }
                        }

                        item.begin_time = CommonFunction.GetNetTime(item.Brand_Msg_Start_Time);
                        item.end_time = CommonFunction.GetNetTime(item.Brand_Msg_End_Time);
                        //顯示圖片的絕對路徑
                        if (!string.IsNullOrEmpty(item.Image_Name))
                        {
                            string folder1 = item.Image_Name.Substring(0, 2) + "/"; //圖片名前兩碼
                            string folder2 = item.Image_Name.Substring(2, 2) + "/"; //圖片名第三四碼
                            item.Image_Name = imgServerPath + brandPath + folder1 + folder2 + item.Image_Name;
                        }
                        else
                        {
                            item.Image_Name = defaultImg;
                        }
                        if (!string.IsNullOrEmpty(item.Resume_Image))
                        {
                            string folder3 = item.Resume_Image.Substring(0, 2) + "/"; //圖片名前兩碼
                            string folder4 = item.Resume_Image.Substring(2, 2) + "/"; //圖片名第三四碼
                            item.Resume_Image = imgServerPath + brandPath + folder3 + folder4 + item.Resume_Image;
                        }
                        else
                        {
                            item.Resume_Image = defaultImg;
                        }
                        if (!string.IsNullOrEmpty(item.Promotion_Banner_Image))
                        {
                            string folder5 = item.Promotion_Banner_Image.Substring(0, 2) + "/"; //圖片名前兩碼
                            string folder6 = item.Promotion_Banner_Image.Substring(2, 2) + "/"; //圖片名第三四碼
                            item.Promotion_Banner_Image = imgServerPath + brandPath + folder5 + folder6 + item.Promotion_Banner_Image;
                        }
                        else
                        {
                            item.Promotion_Banner_Image = defaultImg;
                        }
                    }
                }
                #endregion

                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Newtonsoft.Json.Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success：false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 獲取供應商登錄記錄列表數據 HttpResponseBase GetVendorLoginList()
        /// <summary>
        /// 獲取供應商登錄記錄列表數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendorLoginList()
        {
            List<VendorLoginQuery> stores = new List<VendorLoginQuery>();
            string jsonStr = string.Empty;
            try
            {
                VendorLoginQuery query = new VendorLoginQuery();

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量

                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                    uint vendorid = 0;
                    if (uint.TryParse(Request.Params["vendor_id"].ToString(), out vendorid))
                    {
                        query.vendor_id = vendorid;
                    }
                    else
                    {
                        query.vendor_code = Request.Params["vendor_id"].ToString();
                    }

                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.serchstart = Convert.ToDateTime(Request.Params["timestart"]);
                    query.serchstart = Convert.ToDateTime(query.serchstart.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.serchend = Convert.ToDateTime(Request.Params["timeend"]);
                    query.serchend = Convert.ToDateTime(query.serchend.ToString("yyyy-MM-dd 23:59:59"));
                }
                _Ivendorloginlist = new VendorLoginListMgr(connectionString);
                int totalCount = 0;
                stores = _Ivendorloginlist.Query(query, out totalCount);
                foreach (var item in stores)
                {

                    if (!string.IsNullOrEmpty(item.username))
                    {
                        item.username = item.username.Substring(0, 1) + "**";
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Newtonsoft.Json.Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success：false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存供應商品牌列表數據 HttpResponseBase SaveVendorBrand()
        public HttpResponseBase SaveVendorBrand()
        {
            List<VendorBrandSetQuery> stores = new List<VendorBrandSetQuery>();
            query = new VendorBrandSetQuery();
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            string json = string.Empty;
            string errorInfo = string.Empty;

            try
            {
                #region 上傳圖片

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
                string localPromoPath = imgLocalPath + brandPath;//圖片存儲地址
                FileManagement fileLoad = new FileManagement();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string fileName = string.Empty;//當前文件名
                    HttpPostedFileBase file = Request.Files[i];
                    fileName = Path.GetFileName(file.FileName);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        continue;
                    }

                    fileLoad = new FileManagement();
                    string oldFileName = string.Empty;  //舊文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    bool result = false;
                    string NewFileName = string.Empty;
                    string ServerPath = string.Empty;
                    string newRand = string.Empty;
                    string ErrorMsg = string.Empty;

                    newRand = hash.Md5Encrypt(fileLoad.NewFileName(fileName) + DateTime.Now.ToString(), "32");
                    fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                    NewFileName = newRand + fileExtention;

                    string folder1 = NewFileName.Substring(0, 2) + "/"; //圖片名前兩碼
                    string folder2 = NewFileName.Substring(2, 2) + "/"; //圖片名第三四碼

                    FTP f_cf = new FTP();
                    localPromoPath = imgLocalPath + brandPath + folder1 + folder2;  //圖片存儲地址
                    string s = localPromoPath.Substring(0, localPromoPath.Length - (brandPath + folder1 + folder2).Length + 1);
                    f_cf.MakeMultiDirectory(s, (brandPath + folder1 + folder2).Substring(1, (brandPath + folder1 + folder2).Length - 2).Split('/'), ftpuser, ftppwd);
                    ServerPath = Server.MapPath(imgLocalServerPath + brandPath + folder1 + folder2);
                    fileName = NewFileName;
                    NewFileName = localPromoPath + NewFileName;//絕對路徑
                    Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件

                    //上傳
                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    if (result)//上傳成功
                    {
                        switch (i)
                        {
                            case 0:
                                query.Image_Name = fileName;
                                break;
                            case 1:
                                query.Resume_Image = fileName;
                                break;
                            case 2:
                                query.Promotion_Banner_Image = fileName;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        errorInfo += "第" + (i + 1) + "張" + ErrorMsg + "</br>";
                    }
                }
                #endregion
                query.Brand_Id = UInt32.Parse(_IvendorBrandSet.GetBrandId().ToString());
                if (!string.IsNullOrEmpty(Request.Params["brandName"]))
                {
                    query.Brand_Name = Request.Params["brandName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["vendorid"]))
                {
                    query.vendor_id = uint.Parse(Request.Params["vendorid"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["vendorid"]))
                {
                    query.vendor_id = uint.Parse(Request.Params["vendorid"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["brandsort"]))
                {
                    query.Brand_Sort = uint.Parse(Request.Params["brandsort"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["brandstatus"]))
                {
                    query.Brand_Status = uint.Parse(Request.Params["brandstatus"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["imagestatus"]))
                {
                    query.Image_Status = uint.Parse(Request.Params["imagestatus"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["imagelinkmode"]))
                {
                    query.Image_Link_Mode = uint.Parse(Request.Params["imagelinkmode"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["imagelinkurl"]))
                {
                    query.Image_Link_Url = Request.Params["imagelinkurl"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["resumeimagelink"]))
                {
                    query.Resume_Image_Link = Request.Params["resumeimagelink"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["mediareportlinkurl"]))
                {
                    query.Media_Report_Link_Url = Request.Params["mediareportlinkurl"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["brandmsg"]))
                {
                    query.Brand_Msg = Request.Params["brandmsg"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["Brand_Msg_Start_Time"]))
                {
                    long start = CommonFunction.GetPHPTime(Request.Params["Brand_Msg_Start_Time"].ToString());
                    query.Brand_Msg_Start_Time = uint.Parse(start.ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["Brand_Msg_End_Time"]))
                {
                    long end = CommonFunction.GetPHPTime(Request.Params["Brand_Msg_End_Time"].ToString());
                    query.Brand_Msg_End_Time = uint.Parse(end.ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["shopclass"]))
                {
                    query.classIds = Request.Params["shopclass"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["cucumberbrand"]))
                {
                    query.Cucumber_Brand = uint.Parse(Request.Params["cucumberbrand"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["promotionbannerimagelink"]))
                {
                    query.Promotion_Banner_Image_Link = Request.Params["promotionbannerimagelink"].ToString();
                }
                query.Brand_Createdate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                query.Brand_Ipfrom = Request.UserHostAddress;
                _IvendorBrandSet.Save(query);
                json = "{success:true,msg:\"數據新增成功!\"}";
                if (!string.IsNullOrEmpty(errorInfo))
                {
                    json = "{success:true,msg:\"<div>數據保存成功!<br/>但<br/>" + errorInfo + "</div>\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"新增失敗,請稍后再試!\"}";
                if (!string.IsNullOrEmpty(errorInfo))
                {
                    json = "{success:false,msg:\"<div>新增失敗,請稍后再試!<br/>" + errorInfo + "</div>\"}";
                }
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 圖檔維護
        //擴展名、最小值、最大值,圖片存儲地址
        string extention, minValue, maxValue, localHealthPath;
        #region 更改圖片狀態和排序 +HttpResponseBase UpdateImage()
        public HttpResponseBase UpdateImage()
        {
            string json = string.Empty;
            VendorBrandSetQuery query = new VendorBrandSetQuery();
            VendorBrandSetQuery old = new VendorBrandSetQuery();
            string name = Request.Params["image_filename"];
            name = name.Substring(name.LastIndexOf('/') + 1, name.Length - 1 - name.LastIndexOf('/'));
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            old = _IvendorBrandSet.GetSingleImage(name);
            query.image_filename = name;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["image_sort"]))
                {
                    query.image_sort = Convert.ToUInt32(Request.Params["image_sort"]);
                }
                else
                {
                    query.image_sort = old.image_sort;
                }
                if (!string.IsNullOrEmpty(Request.Params["image_state"]))
                {
                    query.image_state = Convert.ToUInt32(Request.Params["image_state"]);
                }
                else
                {
                    query.image_state = old.image_state;
                }
                if (_IvendorBrandSet.UpdateImage(query) > 0)
                {
                    json = "{success:true}";//返回json數據
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;


        }
        #endregion
        #region 刪除圖片 +HttpResponseBase DeleteImage()
        public HttpResponseBase DeleteImage()
        {
            string jsonStr = String.Empty;
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            if (!String.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            string name = rid.Substring(rid.LastIndexOf('/') + 1, rid.Length - 1 - rid.LastIndexOf('/'));
                            if (_IvendorBrandSet.DeleteImage(name) > 0)
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
        #region 上傳圖片 +HttpResponseBase UploadPicture()
        /// <summary>
        /// 圖檔維護中上傳圖片
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UploadPicture()
        {
            string json = string.Empty;
            string errorInfo = string.Empty;
            VendorBrandSetQuery query = new VendorBrandSetQuery();
            query.Brand_Id = uint.Parse(Request.Params["brand_id"]);
            query.image_sort = 0;
            query.image_state = 1;
            query.image_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToString());
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            ImagePathConfig();
            string fileName = String.Empty, fileExtention = String.Empty, newFileName = String.Empty, oldFileName = String.Empty;
            //string localPromoPath = imgLocalPath + vendorPath;//圖片存儲地址
            string localPromoPath = imgLocalPath + vendorOriginalPath;//圖片存儲地址
            FileManagement fileLoad;//+++++++++++++++++
            for (int i = 0; i < Request.Files.Count; i++)
            {
                fileLoad = new FileManagement();//+++++++++++++++
                HttpPostedFileBase file = Request.Files[i];
                fileName = Path.GetFileName(file.FileName);
                string newRand = hash.Md5Encrypt(fileLoad.NewFileName(fileName) + DateTime.Now.ToString(), "32");
                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                newFileName = newRand + fileExtention;
                string folder1 = newFileName.Substring(0, 2) + "/";//圖片名前兩碼
                string folder2 = newFileName.Substring(2, 2) + "/";//圖片名第三四碼
                string ServerPath = string.Empty;

                FTP f_cf = new FTP();
                localPromoPath = imgLocalPath + vendorOriginalPath + folder1 + folder2;//圖片存儲地址
                string s = localPromoPath.Substring(0, localPromoPath.Length - (vendorOriginalPath + folder1 + folder2).Length + 1);
                f_cf.MakeMultiDirectory(s, (vendorOriginalPath + folder1 + folder2).Substring(1, (vendorOriginalPath + folder1 + folder2).Length - 2).Split('/'), ftpuser, ftppwd);
                ServerPath = Server.MapPath(imgLocalServerPath + vendorOriginalPath + folder1 + folder2);
                fileName = newFileName;
                newFileName = localPromoPath + newFileName;//絕對路徑

                string ErrorMsg = string.Empty;     //錯誤消息
                Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                try
                {
                    //bool result = fileLoad.UpLoadFile(file, ServerPath, newFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    bool result = fileLoad.UpLoadFile(file, ServerPath, newFileName, extention, 150, int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    if (result)//上傳成功
                    {
                        query.image_filename = fileName;
                        _IvendorBrandSet.SaveBrandStory(query);
                        //json = "{success:true,msg:\"圖片上傳成功!\"}";
                    }
                    else
                    {
                        errorInfo += "第" + (i + 1) + "張" + ErrorMsg + "<br/>";
                        //json = "{success:false,msg:\"<div>圖片上傳成功!<br/>但第" + countError + "張" + ErrorMsg + "</div>\"}";
                    }
                    json = "{success:true,msg:\"圖片上傳成功!\"}";
                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        json = "{success:true,msg:\"<div>操作完成!<br/>但<br/>" + errorInfo + "</div>\"}";
                    }
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false,msg:\"操作失敗,請稍后再試!\"}";
                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        json = "{success:false,msg:\"<div>操作失敗,請稍后再試!<br/>" + errorInfo + "</div>\"}";
                    }
                }
                fileName = String.Empty; fileExtention = String.Empty; newFileName = String.Empty; oldFileName = String.Empty;//++++++++++++++++++
                localPromoPath = imgLocalPath + vendorOriginalPath;//圖片存儲地址//++++++++++++++++++++
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase upLoadImg()
        {
            string json = string.Empty;
            string errorInfo = string.Empty;
            VendorBrandSetQuery query = new VendorBrandSetQuery();
            query.Brand_Id = uint.Parse(Request.Params["brand_id"]);
            query.image_sort = 0;
            query.image_state = 1;
            query.image_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToString());
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            ImagePathConfig();
            string fileName = String.Empty, fileExtention = String.Empty, newFileName = String.Empty, oldFileName = String.Empty;

            //string localPromoPath = imgLocalPath + vendorPath;//圖片存儲地址
            string localPromoPath = imgLocalPath + vendorOriginalPath;//圖片存儲地址
            FileManagement fileLoad;//+++++++++++++++++
            Boolean sort_repeat = false;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                fileLoad = new FileManagement();//+++++++++++++++
                HttpPostedFileBase file = Request.Files[i];
                fileName = Path.GetFileName(file.FileName);
                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                string[] arrary = fileName.Split('_');
                if (arrary.Length < 3)
                {
                    errorInfo += "[" + file.FileName + "] ";
                    errorInfo += Resources.Product.FILE_NAME_ERROR;
                    errorInfo += " 例：brand_1_tupian" + fileExtention;
                    continue;
                }
                string brandStr = fileName.Split('_')[0].ToString();
                string sortStr = fileName.Split('_')[1].ToString();
                string filenameStr = fileName.Split('_')[2].ToString();

                Regex reg = new Regex("^?[1-9][0-9]*$");
                if (!brandStr.Equals("brand") || !reg.IsMatch(sortStr))
                {
                    errorInfo += "[" + file.FileName + "] ";
                    errorInfo += Resources.Product.FILE_NAME_ERROR;
                    errorInfo += " 例：brand_1_tupian" + fileExtention;
                    continue;
                }
                query.image_sort = Convert.ToUInt32(sortStr);
                if (query.image_sort > 255)
                    query.image_sort = 0;  //大於數據庫存儲範圍時，默認為0；

                string newRand = hash.Md5Encrypt(fileLoad.NewFileName(fileName) + DateTime.Now.ToString(), "32");
                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                newFileName = newRand + fileExtention;
                string folder1 = newFileName.Substring(0, 2) + "/";//圖片名前兩碼
                string folder2 = newFileName.Substring(2, 2) + "/";//圖片名第三四碼
                string ServerPath = string.Empty;

                FTP f_cf = new FTP();
                localPromoPath = imgLocalPath + vendorOriginalPath + folder1 + folder2;//圖片存儲地址
                string s = localPromoPath.Substring(0, localPromoPath.Length - (vendorOriginalPath + folder1 + folder2).Length + 1);
                f_cf.MakeMultiDirectory(s, (vendorOriginalPath + folder1 + folder2).Substring(1, (vendorOriginalPath + folder1 + folder2).Length - 2).Split('/'), ftpuser, ftppwd);
                ServerPath = Server.MapPath(imgLocalServerPath + vendorOriginalPath + folder1 + folder2);
                fileName = newFileName;
                newFileName = localPromoPath + newFileName;//絕對路徑

                string ErrorMsg = string.Empty;     //錯誤消息
                //Resource.CoreMessage = new CoreResource("Vendor");//尋找product.resx中的資源文件
                try
                {
                    bool result = fileLoad.UpLoadFile(file, ServerPath, newFileName, extention, 150, int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    if (result)//上傳成功
                    {
                        //查詢image_sort是否存在重複
                        if (_IvendorBrandSet.GetSortIsRepeat(query))
                        {
                            sort_repeat = true;
                        }
                        query.image_filename = fileName;
                        _IvendorBrandSet.SaveBrandStory(query);
                        errorInfo += "[" + file.FileName + "] " + "上傳成功!";
                    }
                    else
                    {
                        errorInfo += "[" + file.FileName + "] " + ErrorMsg;
                    }

                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    errorInfo += "[" + file.FileName + "] 上傳失敗<br/>";
                    //json = "{success:false,msg:\"操作失敗,請稍后再試!\"}";
                    //if (!string.IsNullOrEmpty(errorInfo))
                    //{
                    //    json = "{success:false,msg:\"<div>操作失敗,請稍后再試!<br/>" + errorInfo + "</div>\"}";
                    //}
                }
                fileName = String.Empty; fileExtention = String.Empty; newFileName = String.Empty; oldFileName = String.Empty;//++++++++++++++++++
                localPromoPath = imgLocalPath + vendorOriginalPath;//圖片存儲地址//++++++++++++++++++++
            }
            if (sort_repeat)//
                json = "{success:true,sort_repeat:true,msg:\"<div>" + errorInfo + "</div>\"}";
            else
                json = "{success:true,sort_repeat:false,msg:\"<div>" + errorInfo + "</div>\"}";

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion
        #region 列表頁 +HttpResponseBase GetImageInfo()
        public HttpResponseBase GetImageInfo()
        {
            VendorBrandSetQuery query = new VendorBrandSetQuery();
            string brandid = Request.Params["brand_id"].ToString();
            query.Brand_Id = uint.Parse(brandid);
            List<VendorBrandSetQuery> stores = new List<VendorBrandSetQuery>();
            string json = string.Empty;
            try
            {
                _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
                int totalCount = 0;
                stores = _IvendorBrandSet.GetImageInfo(query);
                foreach (var item in stores)
                {
                    if (!string.IsNullOrEmpty(item.image_filename))
                    {
                        item.image_filename = imgServerPath + vendorOriginalPath + item.image_filename.Substring(0, 2) + "/" + item.image_filename.Substring(2, 2) + "/" + item.image_filename;
                    }
                }
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
        #region 獲取圖片配置 -ImagePathConfig()
        /// <summary>
        /// 獲取圖片配置
        /// </summary>  
        private void ImagePathConfig()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
            SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
            SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
            SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
            SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
            //擴展名、最小值、最大值
            extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
            minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
            localHealthPath = imgLocalPath + vendorPath;//圖片存儲地址
        }
        #endregion
        #endregion

        #region 獲取所有的供應商名稱 +HttpResponseBase GetVendor()
        /// <summary>
        /// 獲取所有的供應商名稱
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendor()
        {
            _vendorMgr = new VendorMgr(connectionString);
            List<Vendor> stores = new List<Vendor>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                stores = _vendorMgr.VendorQueryAll(new Vendor());
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

        #region 刪除本地上傳的圖片
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

        #region 創建ftp文件夾
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
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                        ftp.MakeDirectory();
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
        #endregion

        #region 獲取地址store + string QueryArea()+ string QueryCity()+string QueryZip()
        /// <summary>
        ///從後台獲取區域地址
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryArea()
        {
            zMgr = new ZipMgr(connectionString);
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
        /*****************chaojie1124j********************/
        public HttpResponseBase GetZip()
        {
            Zip zip = new Zip();
            List<Zip> zipList = new List<Zip>();
            int resultzip = 0;
            if (!string.IsNullOrEmpty(Request.Params["big_code"]))
            {
               zip.bigcode = Request.Params["big_code"];
               resultzip = 1;
            }
             if (!string.IsNullOrEmpty(Request.Params["c_midcode"]))
            {
                zip.middlecode=Request.Params["c_midcode"];
                resultzip = 1;
            }
             if (!string.IsNullOrEmpty(Request.Params["c_zipcode"]))
            {
                zip.zipcode =Request.Params["c_zipcode"];
                resultzip = 1;
            }
           
            string jsonStr = string.Empty;
            try
            {
                zMgr = new ZipMgr(connectionString);
                zipList = zMgr.GetZipList(zip);
                if (zipList.Count > 0&& resultzip>0)
                {
                    jsonStr = "{success:true,msg:\"" + zipList[0].big +"  "+ zipList[0].middle +"  "+ zipList[0].zipcode + "/" + zipList[0].small.Trim() + "\"}";
                }
                else
                {
                    jsonStr = "{success:true,msg:\"" + 100 + "\"}";
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
        /// <summary>
        ///從後台獲取區域地址
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryCity()
        {
            zMgr = new ZipMgr(connectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["topValue"]))
                {
                    json = zMgr.QueryMiddle(Request.Params["topValue"].ToString());
                }
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

        /// <summary>
        /// 獲取區
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryZip()
        {
            zMgr = new ZipMgr(connectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["topValue"]))
                {
                    json = zMgr.QuerySmall(Request.Form["topValue"].ToString(), Request.Form["topText"].ToString());
                }
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

        #region 獲取pm管理員store +string QueryPm()
        /// <summary>
        /// 獲取管理人員 product_manager 8個人
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryPm()
        {
            gcMgr = new GroupCallerMgr(connectionString);
            paraMgr = new ParameterMgr(connectionString);
            fgMgr = new FgroupMgr(connectionString);
            muMgr = new ManageUserMgr(connectionString);
            groupCaller gc = new groupCaller();
            Fgroup fg = new Fgroup();
            string json = string.Empty;

            try
            {
                List<Parametersrc> parstore = paraMgr.QueryUsed(new Parametersrc { ParameterType = "vendor_pm" }).ToList();

                if (parstore.Count != 0)
                {
                    fg.groupCode = parstore[0].ParameterCode;
                    //  fg.groupName = parstore[0].parameterName;//parameterName是可變的不可依此查詢 edit by shuangshuang0420j 2015.07.28 09:48
                    Fgroup pmfg = fgMgr.GetSingle(fg);
                    if (pmfg != null)
                    {
                        gc.groupId = pmfg.rowid;
                        string pm = gcMgr.QueryCallidById(gc);
                        StringBuilder stb = new StringBuilder("");
                        stb.Append("{");
                        stb.Append("success:true,item:[");
                        //stb.Append("{");
                        //stb.AppendFormat("\"userId\":\"{0}\",\"userName\":\"{1}\"", 0, "請選擇");
                        //stb.Append("}");
                        string nameStr = string.Empty;

                        string[] pmar = pm.Split(',').ToArray();
                        foreach (var item in pmar)
                        {
                            int total = 0;
                            ManageUserQuery mu = muMgr.GetManageUserList(new ManageUserQuery { user_email = item, search_status = "-1" }, out total).FirstOrDefault();
                            if (total == 1 && mu != null)
                            {
                                stb.Append("{");
                                stb.AppendFormat("\"userId\":\"{0}\",\"userName\":\"{1}\"", mu.user_id, mu.user_username);
                                stb.Append("}");
                            }
                        }
                        stb.Append("]}");
                        json = stb.ToString().Replace("}{", "},{");
                    }
                    //else
                    //{
                    //    json = "{success:false,error:1}";
                    //}
                }
                //else
                //{
                //    json = "{success:false,error:0}";
                //}
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
        //public string QueryPmOld()
        //{
        //    _configMgr = new ConfigMgr(connectionString);

        //    string json = string.Empty;
        //    try
        //    {
        //        paraMgr = new ParameterMgr(connectionString);
        //        List<Parametersrc> parstore = paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_manage_count" }).ToList();
        //        int count = 8;
        //        if (parstore != null)
        //        {
        //            count = Convert.ToInt32(parstore[0].ParameterCode);
        //        }
        //        List<ConfigQuery> store = _configMgr.Query("product_manage", count);//獲取8個管理人員
        //        if (store != null)
        //        {
        //            StringBuilder stb = new StringBuilder("");
        //            stb.Append("{");
        //            stb.Append("success:true,item:[");
        //            stb.Append("{");
        //            stb.AppendFormat("\"userId\":\"{0}\",\"userName\":\"{1}\"", 0, "請選擇");
        //            stb.Append("}");
        //            string nameStr = string.Empty;
        //            for (int i = 0; i < store.Count; i++)
        //            {
        //                nameStr += "'" + store[i].name + "'";
        //                if (i != store.Count - 1)
        //                {
        //                    nameStr += ",";
        //                }
        //            }
        //            List<ManageUser> model = _configMgr.getUserPm(nameStr);
        //            foreach (ManageUser item in model)
        //            {

        //                stb.Append("{");
        //                stb.AppendFormat("\"userId\":\"{0}\",\"userName\":\"{1}\"", item.user_id, item.name);
        //                stb.Append("}");
        //            }
        //            stb.Append("]}");
        //            json = stb.ToString().Replace("}{", "},{");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "[]";
        //    }

        //    return json;
        //}
        #endregion

        #region 獲取erp_cate store + string QueryProductCate()+string QueryBuyCate()
        /// <summary>
        ///從後台商品類型
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryProductCate()
        {
            string resultStr = "{success:false}";

            try
            {
                paraMgr = new ParameterMgr(connectionString);
                List<Parametersrc> store = paraMgr.QueryUsed(new Parametersrc { ParameterType = "erp_cate" }).Where(p => p.TopValue == "0").ToList();

                StringBuilder stb = new StringBuilder("");
                stb.Append("{");
                stb.Append("success:true,items:[");
                //stb.Append("{");
                //stb.AppendFormat("\"ParameterCode\":\"{0}\",\"parameterName\":\"{1}\"", 0, "請選擇商品類型");
                //stb.Append("}");
                if (store != null)
                {
                    foreach (Parametersrc item in store)
                    {
                        stb.Append("{");
                        stb.AppendFormat("\"ParameterCode\":\"{0}\",\"parameterName\":\"{1}\"", item.ParameterCode, item.parameterName);
                        stb.Append("}");
                    }
                }
                stb.Append("]}");
                resultStr = stb.ToString().Replace("}{", "},{");

                //resultStr = "{success:true,items:" + JsonConvert.SerializeObject(store) + "}";
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
            return resultStr;
        }

        /// <summary>
        /// 獲取購買類型
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryBuyCate()
        {
            string resultStr = "{success:false}";

            try
            {
                List<Parametersrc> store = new List<Parametersrc>();

                if (Request.Params["topValue"] != "0" && Request.Params["topValue"] != null)
                {
                    string topValue = Request.Params["topValue"] ?? "";
                    paraMgr = new ParameterMgr(connectionString);

                    store = paraMgr.QueryUsed(new Parametersrc { ParameterType = "erp_cate", TopValue = topValue });
                }
                StringBuilder stb = new StringBuilder("");
                stb.Append("{");
                stb.Append("success:true,items:[");
                //stb.Append("{");
                //stb.AppendFormat("\"ParameterCode\":\"{0}\",\"parameterName\":\"{1}\"", 0, "請選擇購買類型");
                //stb.Append("}");
                if (store != null)
                {
                    foreach (Parametersrc item in store)
                    {
                        stb.Append("{");
                        stb.AppendFormat("\"ParameterCode\":\"{0}\",\"parameterName\":\"{1}\"", item.ParameterCode, item.parameterName);
                        stb.Append("}");
                    }
                }
                stb.Append("]}");
                resultStr = stb.ToString().Replace("}{", "},{");

                //                resultStr = "{success:true,items:" + JsonConvert.SerializeObject(paraMgr.QueryUsed(new Parametersrc { ParameterType = "erp_cate", TopValue = topValue })) + "}";
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
            return resultStr;
        }
        #endregion

        #region 獲取聯繫人 store +string QueryContact()
        /// <summary>
        /// 獲取管理人員 product_manager 8個人
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public string QueryContact()
        {
            _vendorMgr = new VendorMgr(connectionString);

            string json = string.Empty;
            json = "[]";

            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["vendor_id"]))
                {
                    Vendor ven = new Vendor();
                    ven.vendor_id = Convert.ToUInt32(Request.QueryString["vendor_id"]);
                    json = _vendorMgr.QueryContanct(ven);
                }
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
        public HttpResponseBase QueryContactTable()
        {
            _vendorMgr = new VendorMgr(connectionString);
            DataTable _dt = new DataTable();
            _dt.Columns.Add("contact_type", typeof(string));
            _dt.Columns.Add("contact_name", typeof(string));
            _dt.Columns.Add("contact_phone1", typeof(string));
            _dt.Columns.Add("contact_phone2", typeof(string));
            _dt.Columns.Add("contact_mobile", typeof(string));
            _dt.Columns.Add("contact_email", typeof(string));
            string json = string.Empty;
            #region 字典保存出貨窗口
            Dictionary<uint, string> ContactType = new Dictionary<uint, string>();
            ContactType.Add(1, "負責人");
            ContactType.Add(2, "業務窗口");
            ContactType.Add(3, "圖/文窗口");
            ContactType.Add(4, "出貨窗口");
            ContactType.Add(5, "賬務窗口");
            ContactType.Add(6, "客服窗口");
            #endregion
            Vendor ven = new Vendor();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                   
                    ven.vendor_id = Convert.ToUInt32(Request.QueryString["vendor_id"]);
                    ven = _vendorMgr.GetSingle(ven);
                    
                }
                if (ContactType.Keys.Contains(ven.contact_type_1))
                {
                    DataRow row = _dt.NewRow();
                    row["contact_type"] = ContactType[ven.contact_type_1];
                    row["contact_name"] = ven.contact_name_1;
                    row["contact_phone1"] = ven.contact_phone_1_1;
                    row["contact_phone2"] = ven.contact_phone_2_1;
                    row["contact_mobile"] = ven.contact_mobile_1;
                    row["contact_email"] = ven.contact_email_1;
                    _dt.Rows.Add(row);
                }
                if (ContactType.Keys.Contains(ven.contact_type_2))
                {
                    DataRow row = _dt.NewRow();
                    row["contact_type"] = ContactType[ven.contact_type_2];
                    row["contact_name"] = ven.contact_name_2;
                    row["contact_phone1"] = ven.contact_phone_1_2;
                    row["contact_phone2"] = ven.contact_phone_2_2;
                    row["contact_mobile"] = ven.contact_mobile_2;
                    row["contact_email"] = ven.contact_email_2;
                    _dt.Rows.Add(row);
                }
                if (ContactType.Keys.Contains(ven.contact_type_3))
                {
                    DataRow row = _dt.NewRow();
                    row["contact_type"] = ContactType[ven.contact_type_3];
                    row["contact_name"] = ven.contact_name_3;
                    row["contact_phone1"] = ven.contact_phone_1_3;
                    row["contact_phone2"] = ven.contact_phone_2_3;
                    row["contact_mobile"] = ven.contact_mobile_3;
                    row["contact_email"] = ven.contact_email_3;
                    _dt.Rows.Add(row);
                }
                if (ContactType.Keys.Contains(ven.contact_type_4))
                {
                    DataRow row = _dt.NewRow();
                    row["contact_type"] = ContactType[ven.contact_type_4];
                    row["contact_name"] = ven.contact_name_4;
                    row["contact_phone1"] = ven.contact_phone_1_4;
                    row["contact_phone2"] = ven.contact_phone_2_4;
                    row["contact_mobile"] = ven.contact_mobile_4;
                    row["contact_email"] = ven.contact_email_4;
                    _dt.Rows.Add(row);
                }
                if (ContactType.Keys.Contains(ven.contact_type_5))
                {
                    DataRow row = _dt.NewRow();
                    row["contact_type"] = ContactType[ven.contact_type_5];
                    row["contact_name"] = ven.contact_name_5;
                    row["contact_phone1"] = ven.contact_phone_1_5;
                    row["contact_phone2"] = ven.contact_phone_2_5;
                    row["contact_mobile"] = ven.contact_mobile_5;
                    row["contact_email"] = ven.contact_email_5;
                    _dt.Rows.Add(row);
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 修改供應商品牌列表數據 HttpResponseBase UpdVendorBrand()
        /// <summary>
        /// 新增或修改供應商品牌信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdVendorBrand()
        {
            List<VendorBrandSetQuery> stores = new List<VendorBrandSetQuery>();
            query = new VendorBrandSetQuery();
            VendorBrandSetQuery oldquery = new VendorBrandSetQuery();
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            _productMgr = new ProductMgr(connectionString);
            string json = string.Empty;
            string errorInfo = string.Empty;

            try
            {
                #region 獲取數據
                query.Brand_Id = UInt32.Parse(Request.Params["Brand_Id"]);
                oldquery = _IvendorBrandSet.GetModelById(Int32.Parse(Request.Params["Brand_Id"]));
                try
                {
                    query.Brand_Name = Request.Params["brandName"].ToString();
                }
                catch (Exception)
                {
                    query.Brand_Name = oldquery.Brand_Name;
                }
                try
                {
                    query.vendor_id = uint.Parse(Request.Params["vendorid"].ToString());
                }
                catch (Exception)
                {
                    query.vendor_id = oldquery.vendor_id;
                }
                try
                {
                    query.Brand_Sort = uint.Parse(Request.Params["brandsort"].ToString());
                }
                catch (Exception)
                {
                    query.Brand_Sort = oldquery.Brand_Sort;
                }
                try
                {
                    query.Brand_Status = uint.Parse(Request.Params["brandstatus"].ToString());
                }
                catch (Exception)
                {
                    query.Brand_Status = oldquery.Brand_Status;
                }
                try
                {
                    query.Image_Status = uint.Parse(Request.Params["imagestatus"].ToString());
                }
                catch (Exception)
                {
                    query.Image_Status = oldquery.Image_Status;
                }
                try
                {
                    query.Image_Link_Mode = uint.Parse(Request.Params["imagelinkmode"].ToString());
                }
                catch (Exception)
                {
                    query.Image_Link_Mode = oldquery.Image_Link_Mode;
                }
                try
                {
                    query.Image_Link_Url = Request.Params["imagelinkurl"].ToString();
                }
                catch (Exception)
                {
                    query.Image_Link_Url = oldquery.Image_Link_Url;
                }
                try
                {
                    query.Resume_Image_Link = Request.Params["resumeimagelink"].ToString();
                }
                catch (Exception)
                {
                    query.Resume_Image_Link = oldquery.Resume_Image_Link;
                }
                try
                {
                    query.Media_Report_Link_Url = Request.Params["mediareportlinkurl"].ToString();
                }
                catch (Exception)
                {
                    query.Media_Report_Link_Url = oldquery.Media_Report_Link_Url;
                }
                try
                {
                    query.Brand_Msg = Request.Params["brandmsg"].ToString();
                }
                catch (Exception)
                {
                    query.Brand_Msg = oldquery.Brand_Msg;
                }
                long start = CommonFunction.GetPHPTime(Request.Params["begin_time"].ToString());
                long end = CommonFunction.GetPHPTime(Request.Params["end_time"].ToString());
                try
                {
                    query.Brand_Msg_Start_Time = uint.Parse(start.ToString());
                }
                catch (Exception)
                {
                    query.Brand_Msg_Start_Time = oldquery.Brand_Msg_Start_Time;
                }
                try
                {
                    query.Brand_Msg_End_Time = uint.Parse(end.ToString());
                }
                catch (Exception)
                {
                    query.Brand_Msg_End_Time = oldquery.Brand_Msg_End_Time;
                }
                try
                {
                    query.Brand_Createdate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                }
                catch (Exception)
                {
                    query.Brand_Createdate = oldquery.Brand_Createdate;
                }
                try
                {
                    query.classIds = Request["shopclass"].ToString();
                }
                catch (Exception)
                {
                    query.classIds = oldquery.classIds;
                }
                query.Brand_Ipfrom = Request.UserHostAddress;
                try
                {
                    query.Cucumber_Brand = uint.Parse(Request.Params["cucumberbrand"].ToString());
                }
                catch
                {
                    query.Cucumber_Brand = oldquery.Cucumber_Brand;
                }
                try
                {
                    query.Image_Name = Request.Params["imagename"].ToString();
                }
                catch (Exception)
                {
                    query.Image_Name = oldquery.Image_Name;
                }
                try
                {
                    query.Promotion_Banner_Image_Link = Request.Params["promotionbannerimagelink"].ToString();
                }
                catch (Exception)
                {
                    query.Promotion_Banner_Image_Link = oldquery.Promotion_Banner_Image_Link;
                }
                query.Image_Name = oldquery.Image_Name;
                query.Resume_Image = oldquery.Resume_Image;
                query.Promotion_Banner_Image = oldquery.Promotion_Banner_Image;
                #endregion
                #region 上傳圖片
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
                string localPromoPath = imgLocalPath + brandPath;//圖片存儲地址
                FileManagement fileLoad = new FileManagement();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string fileName = string.Empty;     //當前文件名
                    HttpPostedFileBase file = Request.Files[i];
                    fileName = Path.GetFileName(file.FileName);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        continue;
                    }

                    fileLoad = new FileManagement();
                    string oldFileName = string.Empty;  //舊文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    bool result = false;
                    string NewFileName = string.Empty;
                    string ServerPath = string.Empty;
                    string newRand = string.Empty;
                    string ErrorMsg = string.Empty;

                    newRand = hash.Md5Encrypt(fileLoad.NewFileName(fileName) + DateTime.Now.ToString(), "32");
                    fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                    NewFileName = newRand + fileExtention;

                    string folder1 = NewFileName.Substring(0, 2) + "/"; //圖片名前兩碼
                    string folder2 = NewFileName.Substring(2, 2) + "/"; //圖片名第三四碼

                    FTP f_cf = new FTP();
                    localPromoPath = imgLocalPath + brandPath + folder1 + folder2;  //圖片存儲地址
                    string s = localPromoPath.Substring(0, localPromoPath.Length - (brandPath + folder1 + folder2).Length + 1);
                    f_cf.MakeMultiDirectory(s, (brandPath + folder1 + folder2).Substring(1, (brandPath + folder1 + folder2).Length - 2).Split('/'), ftpuser, ftppwd);
                    ServerPath = Server.MapPath(imgLocalServerPath + brandPath + folder1 + folder2);
                    fileName = NewFileName;
                    NewFileName = localPromoPath + NewFileName;//絕對路徑
                    Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件

                    //上傳
                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    if (result)//上傳成功
                    {
                        //圖片上傳成功后記錄新圖片名稱
                        switch (i)
                        {
                            case 0:
                                query.Image_Name = fileName;
                                break;
                            case 1:
                                query.Resume_Image = fileName;
                                break;
                            case 2:
                                query.Promotion_Banner_Image = fileName;
                                break;
                            default:
                                break;
                        }

                        //圖片上傳成功后如果存在舊圖片則根據舊圖片名刪除舊圖片
                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            switch (i)
                            {
                                case 0:
                                    oldFileName = oldquery.Image_Name;
                                    break;
                                case 1:
                                    oldFileName = oldquery.Resume_Image;
                                    break;
                                case 2:
                                    oldFileName = oldquery.Promotion_Banner_Image;
                                    break;
                                default:
                                    break;
                            }
                            DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                        }
                    }
                    else
                    {
                        //圖片上傳失敗則圖片名稱為原有圖片名稱
                        switch (i)
                        {
                            case 0:
                                query.Image_Name = oldquery.Image_Name;//形象圖片
                                break;
                            case 1:
                                query.Resume_Image = oldquery.Resume_Image;//安心聲明圖片
                                break;
                            case 2:
                                query.Promotion_Banner_Image = oldquery.Promotion_Banner_Image;//促銷圖片
                                break;
                            default:
                                break;
                        }

                        errorInfo += "第" + (i + 1) + "張" + ErrorMsg + "<br/>";
                        //json = "{success:false,msg:\"第" + i + "張" + ErrorMsg + ",之後圖片未作處理" + "\"}";
                    }
                }
                #endregion
                //更新數據庫記錄
                if (_IvendorBrandSet.Update(query) > 0)
                {
                    if (_productMgr.UpdateSaleStatusByCondition(new Product { Vendor_Id = query.vendor_id, Brand_Id = query.Brand_Id }))
                    {
                        json = "{success:true,msg:\"數據保存成功!\"}";
                    }
                }
                if (!string.IsNullOrEmpty(errorInfo))
                {
                    json = "{success:true,msg:\"<div>數據保存成功!<br/>但<br/>" + errorInfo + "</div>\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"保存失敗,請稍后再試!\"}";
                if (!string.IsNullOrEmpty(errorInfo))
                {
                    json = "{success:false,msg:\"<div>保存失敗,請稍后再試!<br/>" + errorInfo + "</div>\"}";
                }
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取地址下拉列表
        public HttpResponseBase GetZipAddress()
        {
            List<Zip> store = new List<Zip>();
            string json = string.Empty;
            try
            {
                _imrMgr = new InvoiceMasterRecordMgr(connectionString);
                Zip zip = new Zip();
                store = _imrMgr.GetZipAddress(zip);
                zip = new Zip();
                zip.zipcode = "";
                zip.middle = "請選擇";
                store.Insert(0, zip);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 更改圖片狀態或者是排序 +HttpResponseBase UpdateSortByPicture()
        public HttpResponseBase UpdateSortByPicture()
        {
            string json = string.Empty;
            VendorBrandSetQuery query = new VendorBrandSetQuery();
            VendorBrandSetQuery old = new VendorBrandSetQuery();
            string name = Request.Params["picture"];
            name = name.Substring(name.LastIndexOf('/') + 1, name.Length - 1 - name.LastIndexOf('/'));
            _IvendorBrandSet = new VendorBrandSetMgr(connectionString);
            old = _IvendorBrandSet.GetSingleImage(name);
            query.image_filename = name;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["sort_id"]))
                {
                    query.image_sort = Convert.ToUInt32(Request.Params["sort_id"]);
                }
                else
                {
                    query.image_sort = old.image_sort;
                }
                if (!string.IsNullOrEmpty(Request.Params["this_type"]))
                {
                    query.image_state = Convert.ToUInt32(Request.Params["this_type"]);
                }
                else
                {
                    query.image_state = old.image_state;
                }
                if (_IvendorBrandSet.UpdateImage(query) > 0)
                {
                    json = "{success:true}";//返回json數據
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;


        }
        #endregion

        #region 獲取品牌故事文字列表 + HttpResponseBase GetVendorBrandStory()
        public HttpResponseBase GetVendorBrandStory()
        {
            string json = string.Empty;
            DataTable stores = new DataTable();
            try
            {
                VendorBrandQuery query = new VendorBrandQuery();
                _vendorBrand = new VendorBrandMgr(connectionString);
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量   
                // query.searchContent = Request.Params["searchContent"].Trim();

                if (!string.IsNullOrEmpty(Request.Params["searchContent"]))
                {
                    //支持空格，中英文逗號隔開
                    string content = Regex.Replace(Request.Params["searchContent"].Trim(), "(\\s+)|(，)|(\\,)", ",");
                    string[] contents = content.Split(',');
                    int pid = 0;
                    for (int i = 0; i < contents.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(contents[i].Trim()))
                        {
                            if (query.searchContent == "")
                            {
                                if (int.TryParse(contents[i], out pid))
                                {
                                    query.searchContent += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                                else
                                {
                                    query.searchContent += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                            }
                            else
                            {
                                if (int.TryParse(contents[i], out pid))
                                {
                                    query.searchContent += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                                else
                                {
                                    query.searchContent += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
                query.story_createname = Request.Params["createname"].Trim();
                query.story_createname = query.story_createname.Trim();
                if (!string.IsNullOrEmpty(Request.Params["searchVendor"]))
                {
                    query.Vendor_Id = uint.Parse(Request.Params["searchVendor"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brandid"]))
                {
                    query.Brand_Id = uint.Parse(Request.Params["brandid"]);
                }
                // query.searchState = Convert.ToInt32(Request.Params["searchState"]);
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["start_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_date"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["end_date"]);
                }
                if (Request.Params["searchState"] == "0")
                {
                    query.Brand_Story_Text = string.Empty;
                }
                else if (Request.Params["searchState"] == "1")
                {
                    query.Brand_Story_Text = "1";
                }
                else if (Request.Params["searchState"] == "2")
                {
                    query.Brand_Story_Text = "2";
                }
                #region 供應商狀態、品牌狀態
                if (!string.IsNullOrEmpty(Request.Params["vendorState"]))
                {
                    query.vendorState = Convert.ToInt32(Request.Params["vendorState"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brandState"]))
                {
                    query.Brand_Status = Convert.ToUInt32(Request.Params["brandState"]);
                }
                #endregion
                int totalCount = 0;
                stores = _vendorBrand.GetVendorBrandStory(query, out totalCount);

                foreach (DataRow dr in stores.Rows)
                {
                    dr["Brand_Story_Text"] = dr["Brand_Story_Text"].ToString().Replace("<br/>", "\n");
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Newtonsoft.Json.Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success：false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 添加品牌故事文字 + HttpResponseBase AddVendorBrandStory()
        public HttpResponseBase AddVendorBrandStory()
        {
            string json = string.Empty;
            int i = 0;
            try
            {
                VendorBrandQuery query = new VendorBrandQuery();
                _vendorBrand = new VendorBrandMgr(connectionString);
                query.Brand_Id = Convert.ToUInt32(Request.Params["brandid"]);
                query.Brand_Name = Request.Params["brandName"];
                query.Brand_Story_Text = Request.Params["brandStoryText"];
                query.Brand_Story_Text = query.Brand_Story_Text.Replace("\\", "\\\\");
                VendorBrand oldbrand = _vendorBrand.GetVendorBrand(query).FirstOrDefault();
                query.Story_Update = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                query.Story_Updatedate = DateTime.Now;
                if (oldbrand.Story_Created == 0)
                {
                    query.Story_Created = query.Story_Update;
                    query.Story_Createdate = query.Story_Updatedate;
                }
                else
                {
                    query.Story_Created = oldbrand.Story_Created;
                    query.Story_Createdate = oldbrand.Story_Createdate;
                }

                i = _vendorBrand.AddVendorBrandStory(query);
                if (i > 0)
                {
                    json = "{success:true,msg:'修改成功！'}";//返回json數據
                }
                else
                {
                    json = "{success:false,msg:'修改失敗！'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'異常！'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        public HttpResponseBase VendorBrandPreview()
        {
            _siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            List<SiteConfig> configList = _siteConfigMgr.Query();
            string DomainName = configList.Where(m => m.Name.Equals("DoMain_Name")).FirstOrDefault().Value;
            string json = string.Empty;
            try
            {
                VendorBrandQuery query = new VendorBrandQuery();
                _vendorBrand = new VendorBrandMgr(connectionString);
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.Brand_Id = Convert.ToUInt32(Request.Params["brand_id"]);
                    int classify = _vendorBrand.GetClassify(query);
                    string result = string.Empty;
                    switch (classify)
                    {
                        case 10:
                            result += "http://" + DomainName + "/food/brand_food.php?b_id=" + query.Brand_Id;//商品預覽
                            break;
                        case 20:
                            result += "http://" + DomainName + "/stuff/brand_stuff.php?b_id=" + query.Brand_Id;//商品預覽
                            break;
                        default:
                            result += "http://" + DomainName + "/brand.php?b_id=" + query.Brand_Id;//商品預覽
                            break;
                        //www.gigade100.com/newweb/food/brand_food.php?b_id=332
                    }
                    json = result.ToString().ToLower();
                }

            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "無預覽信息";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #region 品牌故事文字匯出
        public HttpResponseBase ExportFile()
        {
            int totalCount = 0;
            string json = "false";
            try
            {
                VendorBrandQuery query = new VendorBrandQuery();
                _vendorBrand = new VendorBrandMgr(connectionString);
                query.IsPage = false;
                query.isExport = true;
                // query.searchContent = Request.Params["searchContent"];
                if (!string.IsNullOrEmpty(Request.Params["searchContent"]))
                {
                    //支持空格，中英文逗號隔開
                    string content = Regex.Replace(Request.Params["searchContent"].Trim(), "(\\s+)|(，)|(\\,)", ",");
                    string[] contents = content.Split(',');
                    int pid = 0;
                    for (int i = 0; i < contents.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(contents[i].Trim()))
                        {
                            if (query.searchContent == "")
                            {
                                if (int.TryParse(contents[i], out pid))
                                {
                                    query.searchContent += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                                else
                                {
                                    query.searchContent += Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                            }
                            else
                            {
                                if (int.TryParse(contents[i], out pid))
                                {
                                    query.searchContent += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                                else
                                {
                                    query.searchContent += "," + Regex.Replace(contents[i].Trim(), "(\\s+)|(，)|(\\,)", ",");
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
                query.story_createname = Request.Params["createname"];
                if (!string.IsNullOrEmpty(Request.Params["searchVendor"]))
                {
                    if (Request.Params["searchVendor"] != "undefined")
                    {
                        query.Vendor_Id = uint.Parse(Request.Params["searchVendor"]);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["brandid"]))
                {
                    if (Request.Params["brandid"] != "undefined")
                    {
                        query.Brand_Id = uint.Parse(Request.Params["brandid"]);
                    }
                }
                // query.searchState = Convert.ToInt32(Request.Params["searchState"]);
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["start_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_date"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["end_date"]);
                }
                if (Request.Params["searchState"] == "0")
                {
                    query.Brand_Story_Text = string.Empty;
                }
                else if (Request.Params["searchState"] == "1")
                {
                    query.Brand_Story_Text = "1";
                }
                else if (Request.Params["searchState"] == "2")
                {
                    query.Brand_Story_Text = "2";
                }
                #region 供應商狀態、品牌狀態
                if (!string.IsNullOrEmpty(Request.Params["vendorState"]))
                {
                    query.vendorState = Convert.ToInt32(Request.Params["vendorState"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brandState"]))
                {
                    query.Brand_Status = Convert.ToUInt32(Request.Params["brandState"]);
                }
                #endregion

                query.IsPage = false;

                DataTable dtHZ = new DataTable();

                string newExcelName = string.Empty;
                dtHZ.Columns.Add("供應商編號", typeof(String));
                dtHZ.Columns.Add("供應商名稱", typeof(String));
                dtHZ.Columns.Add("品牌編號", typeof(String));
                dtHZ.Columns.Add("品牌名稱", typeof(String));
                dtHZ.Columns.Add("建立人", typeof(String));
                dtHZ.Columns.Add("建立時間", typeof(String));
                dtHZ.Columns.Add("修改人", typeof(String));
                dtHZ.Columns.Add("修改時間", typeof(String));

                DataTable _dt = _vendorBrand.GetVendorBrandStory(query, out totalCount);

                foreach (DataRow dr_v in _dt.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dr_v["vendor_id"].ToString();
                    dr[1] = dr_v["vendor_name_full"].ToString();
                    dr[2] = dr_v["brand_id"].ToString();
                    dr[3] = dr_v["brand_name"].ToString();
                    dr[4] = dr_v["story_createname"].ToString();
                    dr[5] = dr_v["story_createdate"].ToString();
                    dr[6] = dr_v["story_updatename"].ToString();
                    dr[7] = dr_v["story_updatedate"].ToString();

                    dtHZ.Rows.Add(dr);
                }
                string[] colname = new string[dtHZ.Columns.Count];
                string filename = "vendorbrand_story" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                newExcelName = Server.MapPath(excelPath_export) + filename;
                for (int i = 0; i < dtHZ.Columns.Count; i++)
                {
                    colname[i] = dtHZ.Columns[i].ColumnName;
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                ExcelHelperXhf.ExportDTtoExcel(dtHZ, "", newExcelName);
                //CsvHelper.ExportDataTableToCsv(_dt, newExcelName, columnName, true);
                json = "{success:true,ExcelName:\'" + filename + "\'}";

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 供應商類型store
        public HttpResponseBase GetVendorTypeStore()
        {
            paraMgr = new ParameterMgr(connectionString);
            List<Parametersrc> store = new List<Parametersrc>();
            string json = string.Empty;
            List<VendorQuery> query = new List<VendorQuery>();
            try
            {
                string type = "vendor_type";
                store = paraMgr.GetElementType(type);
                for (int i = 0; i < store.Count; i++)
                {
                    VendorQuery vendor = new VendorQuery();
                    vendor.vendor_type = store[i].ParameterCode.ToString();
                    vendor.vendor_type_name = store[i].parameterName.ToString();
                    query.Add(vendor);

                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(query, Formatting.Indented) + "}";
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "無預覽信息";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 供應商資料異動記錄
        /// <summary>
        /// 修改記錄
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase VendorChangeList()
        {
            string json = "{success:false,data:0}";
            try
            {
                TableChangeLogQuery query = new TableChangeLogQuery();
                query.change_table = "vendor";
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                if (!string.IsNullOrEmpty(Request.Params["ven_type"]))
                {
                    query.key_type = Convert.ToInt32(Request.Params["ven_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["d_type"]))
                {
                    query.d_type = Convert.ToInt32(Request.Params["d_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.key = Request.Params["search_con"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["date_one"]))
                {
                    query.date_one = Convert.ToDateTime(Request.Params["date_one"]);
                    query.date_one = Convert.ToDateTime(query.date_one.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["date_two"]))
                {
                    query.date_two = Convert.ToDateTime(Request.Params["date_two"]);
                    query.date_two = Convert.ToDateTime(query.date_two.ToString("yyyy-MM-dd 23:59:59"));

                }
                // query.IsPage = false;
                TableChangeLogMgr _tclMgr = new TableChangeLogMgr(connectionString);
                int totalCount = 0;
                DataTable store = _tclMgr.GetVendorChangeLog(query, out totalCount);
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

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 修改記錄詳情
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase VendorChangeDetailList()
        {
            string json = "{success:false,data:0}";
            try
            {
                TableChangeLogQuery query = new TableChangeLogQuery();
                query.change_table = "vendor";

                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                    query.pk_id = Convert.ToInt32(Request.Params["vendor_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["muser"]))
                {
                    query.create_user = Convert.ToInt32(Request.Params["muser"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["mdate"]))
                {
                    query.create_time = Convert.ToDateTime(Request.Params["mdate"]);
                }
                TableChangeLogMgr _tclMgr = new TableChangeLogMgr(connectionString);
                BLL.gigade.Model.Custom.TableChangeLogCustom store = _tclMgr.GetVendorChangeDetail(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 異動記錄匯出
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase VendorLogExport()
        {
            string newCsvName = string.Empty;
            string json = string.Empty;

            TableChangeLogQuery query = new TableChangeLogQuery();
            query.change_table = "vendor";


            if (!string.IsNullOrEmpty(Request.Params["ven_type"]))
            {
                query.key_type = Convert.ToInt32(Request.Params["ven_type"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["d_type"]))
            {
                query.d_type = Convert.ToInt32(Request.Params["d_type"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["search_con"]))
            {
                query.key = Request.Params["search_con"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["date_one"]))
            {
                query.date_one = Convert.ToDateTime(Request.Params["date_one"]);
                query.date_one = Convert.ToDateTime(query.date_one.ToString("yyyy-MM-dd 00:00:00"));
            }
            if (!string.IsNullOrEmpty(Request.Params["date_two"]))
            {
                query.date_two = Convert.ToDateTime(Request.Params["date_two"]);
                query.date_two = Convert.ToDateTime(query.date_two.ToString("yyyy-MM-dd 23:59:59"));
            }
            TableChangeLogMgr _tclMgr = new TableChangeLogMgr(connectionString);
            DataTable dt = _tclMgr.VendorLogExport(query);

            try
            {

                string filename = "供應商資料異動記錄_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                newCsvName = Server.MapPath(excelPath) + filename;

                if (System.IO.File.Exists(newCsvName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newCsvName, FileAttributes.Normal);
                    System.IO.File.Delete(newCsvName);
                }
                string[] colname = { "供應商編號", "供應商編碼", "供應商名稱", "建立人", "建立時間", "修改人", "修改時間", "變動欄位", "欄位中文名", "修改前值", "修改后值" };
                CsvHelper.ExportDataTableToCsv(dt, newCsvName, colname, true);
                json = "true," + filename + "," + excelPath;

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false, ";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }


        /// <summary>
        /// 根據供應商查看未失格商品的個數
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult GetOffGradeCount()
        {
            try
            {
                string vendor_id = Request.Params["vendor_id"].ToString();
                _vendorMgr = new VendorMgr(connectionString);
                int count = _vendorMgr.GetOffGradeCount(vendor_id);
                return Json(new { success = "true", count = count });
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


        /// <summary>
        /// 根據供應商解除失格
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UnGrade()
        {
            try
            {
                string vendor_id = Request.Params["vendor_id"].ToString();
                string active = Request.Params["active"].ToString();
                StringBuilder update_log = new StringBuilder();
                _vendorMgr = new VendorMgr(connectionString);
                //update_log.AppendFormat("vendor_status:{0}:{1}:供應商狀態", 3, active);
                List<TableChangeLog> list = new List<TableChangeLog>();
                list.Add(new TableChangeLog() { change_field = "vendor_status", old_value = "3", new_value = active, field_ch_name = "供應商狀態" });

                if (_vendorMgr.UnGrade(vendor_id, active, list) > 0)
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

        #region 刪除商品品牌中的促銷圖片


        public string GetDetailFolder(string picName)
        {
            string firthFolder = picName.Substring(0, 2) + "/";
            string secondFolder = picName.Substring(2, 2) + "/";

            return firthFolder + secondFolder;
        }
        [HttpPost]
        public HttpResponseBase DelPromoPicClick()
        {
            string json = "{success:true}";

            try
            {
                string picSrc = Request.Params["src"].ToString();
                var array = picSrc.Split('/');
                string picName = array.Last();
                int brandId = Convert.ToInt32(Request.Params["brand_id"].ToString());
                var path = imgLocalPath + brandPath + GetDetailFolder(picName) + picName;
                string type = Request.Params["type"].ToString();
                //刪除服務器上對應的圖片
                DeletePicFile(path);
                if (brandId != 0)
                {
                    _vendorBrand = new VendorBrandMgr(connectionString);
                    if (_vendorBrand.DelPromoPic(brandId,type) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
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
    }
}