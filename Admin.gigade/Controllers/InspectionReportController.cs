using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.gigade.Model;
using System.Configuration;
using BLL.gigade.Common;
using System.IO;
using gigadeExcel.Comment;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Admin.gigade.Controllers
{
    public class InspectionReportController : Controller
    {
        //
        // GET: /InspectionReport/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private InspectionReportMgr _inspectionReport;
        private VendorBrandMgr _vendorBrand;
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp上傳圖片地址http://192.168.71.10:2121
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//顯示圖片路徑http://192.168.71.10:8765
        string InspectionReportPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.InspectionReportPath);//圖片保存路徑
        DataTable DTInspectionExcel = new DataTable();//由於會出匯入時的錯誤信息
        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        #region view
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InspectionReport()
        {
            return View();
        }
        #endregion
        #region 證書類型

        #region 證書類型列表
        public ActionResult CertificateCategory()
        {
            return View();
        }
        #endregion
        #region 證書類型列表頁獲取 +GetCertificateCategory()
        public HttpResponseBase GetCertificateCategory()
        {
            string json = string.Empty;
            int totalCount = 0;
            List<CertificateCategoryQuery> stores = new List<CertificateCategoryQuery>();
            CertificateCategoryQuery query = new CertificateCategoryQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                {
                    query.searchcon = Request.Params["searchcontent"].ToString().Trim();
                }
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                stores = _inspectionReport.GetCertificateCategoryList(query, out totalCount);
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 更改狀態
        public JsonResult UpdateActive()
        {
            try
            {
                CertificateCategoryQuery query = new CertificateCategoryQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.status = Convert.ToInt32(Request.Params["active"]);
                }
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                int res = _inspectionReport.UpdateActive(query);
                if (res > 0)
                {
                    return Json(new { success = "true", msg = "" });
                }
                else
                {
                    return Json(new { success = "false", msg = "" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false", msg = "" });
            }

        }
        #endregion
        #region 證書大類store
        public HttpResponseBase GetGroup()
        {
            List<CertificateCategoryQuery> store = new List<CertificateCategoryQuery>();
            CertificateCategoryQuery query = new CertificateCategoryQuery();
            string json = string.Empty;
            try
            {
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["ROWID"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["ROWID"]);
                }
                store = _inspectionReport.GetStore(query);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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

        // #region 證書小類store
        // public HttpResponseBase GetType2Group()
        // {
        //     List<CertificateCategoryQuery> store = new List<CertificateCategoryQuery>();
        //     CertificateCategoryQuery query = new CertificateCategoryQuery();
        //     string json = string.Empty;
        //     try
        //     {
        //         _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
        //         if (!string.IsNullOrEmpty(Request.Params["rowID"]))
        //         {
        //             query.rowID = Convert.ToInt32(Request.Params["rowID"]);
        //         }
        //         store = _inspectionReport.GetStore(query);
        //         json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
        //     }
        //     catch (Exception ex)
        //     {
        //         Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //         logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //         logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //         log.Error(logMessage);
        //         json = "{success:false}";
        //     }
        //     this.Response.Clear();
        //     this.Response.Write(json);
        //     this.Response.End();
        //     return this.Response;
        // }
        //#endregion
        #region 新增/編輯選擇下拉框帶出對應code
        public HttpResponseBase GetBigCode()
        {
            string json = string.Empty;
            List<CertificateCategoryQuery> list = new List<CertificateCategoryQuery>();
            try
            {
                CertificateCategoryQuery query = new CertificateCategoryQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["id"]);
                }
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                query.frowID = 0;
                list = _inspectionReport.GetCertificateCategoryInfo(query);
                json = "{success:true,msg:'" + list[0].certificate_categorycode + "',fid:" + list[0].rowID + "}";
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
        #region 列表刪除
        public HttpResponseBase DeleteCertificateCategory()
        {
            string json = string.Empty;
            CertificateCategoryQuery query = new CertificateCategoryQuery();
            string id = string.Empty;
            string fid = string.Empty;
            string frowIDs = string.Empty;
            _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
            try
            {

                if (!string.IsNullOrEmpty(Request.Params["ids"]))
                {
                    string rowIDs = (Request.Params["ids"]);
                    if (_inspectionReport.BeforeDelete(rowIDs))//判斷數據是否已被使用，如果使用則不能刪除
                    {
                        #region 刪除
                        if (!string.IsNullOrEmpty(Request.Params["rid"]))
                        {
                            //子類
                            id = Request.Params["rid"].ToString();
                        }
                        if (!string.IsNullOrEmpty(Request.Params["frid"]))
                        {
                            //父類
                            fid = Request.Params["frid"].ToString();
                        }
                        query.rowIDs = id.Substring(0, id.Length - 1);
                        string[] fids = fid.Split(',');
                        for (int i = 0; i < fids.Length - 1; i++)
                        {
                            if (!frowIDs.Contains(fids[i]))
                            {
                                frowIDs = frowIDs + fids[i] + ",";
                            }
                        }
                        #region 刪除子類
                        if (_inspectionReport.DeleteCertificateCategory(query) <= 0)
                        {
                            json = "{success:true,msg:0}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #endregion
                        #region 判斷父類對應的子類是否存在
                        string[] fidss = frowIDs.Split(',');
                        for (int j = 0; j < fidss.Length - 1; j++)
                        {
                            query.frowID = Convert.ToInt32(fidss[j]);
                            if (_inspectionReport.CheckOnly(query) <= 0)
                            {
                                query.frowIDs = query.frowIDs + fidss[j] + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(query.frowIDs))
                        {
                            query.frowIDs = query.frowIDs.Substring(0, query.frowIDs.Length - 1);
                        }
                        #endregion
                        #region 刪除不存在子類的父類
                        if (!string.IsNullOrEmpty(query.frowIDs) && query.frowIDs.Length > 0)
                        {
                            if (_inspectionReport.DeleteCCByTransaction(query))
                            {
                                json = "{success:true,msg:1}";
                            }
                            else
                            {
                                json = "{success:true,msg:0}";
                            }
                        }
                        #endregion
                        else
                        {
                            json = "{success:true,msg:1}";
                        }
                        #endregion
                    }
                    else
                    {
                        json = "{success:true,msg:-1}";
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
        #region 新增/編輯保存
        public HttpResponseBase InspectionReportSave()
        {
            string json = string.Empty;
            List<CertificateCategoryQuery> list = new List<CertificateCategoryQuery>();
            List<CertificateCategoryQuery> oldlist = new List<CertificateCategoryQuery>();
            List<CertificateCategoryQuery> oldbiglist = new List<CertificateCategoryQuery>();
            _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
            try
            {
                CertificateCategoryQuery query = new CertificateCategoryQuery();
                string fid = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["frowname"]))
                {
                    fid = Request.Params["frowname"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["rowID"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["frowID"]))
                {
                    query.frowID = Convert.ToInt32(Request.Params["frowID"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["f_code"]))
                {
                    query.certificate_categorycode = Request.Params["f_code"].ToString().ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["child_name"]))
                {
                    query.certificate_category_childname = Request.Params["child_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["child_code"]))
                {
                    query.certificate_category_childcode = Request.Params["child_code"].ToString().ToUpper();
                }
                uint unum = 0;
                if (uint.TryParse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString(), out unum))
                {
                    query.k_user = unum;
                }
                query.k_date = DateTime.Now;
                int isnum = 0;
                //查詢小類信息以供編輯時查重
                CertificateCategoryQuery oldquery = new CertificateCategoryQuery();
                oldquery.rowID = query.rowID;
                oldlist = _inspectionReport.GetCertificateCategoryInfo(oldquery);
                //查詢大類信息以供編輯時查重
                CertificateCategoryQuery oldbigquery = new CertificateCategoryQuery();
                oldbigquery.rowID = query.frowID;
                oldbiglist = _inspectionReport.GetCertificateCategoryInfo(oldbigquery);
                #region 編輯處理
                if (query.rowID != 0)
                {//編輯
                    #region 編輯時選擇大類非編輯大類
                    if (int.TryParse(fid, out isnum))//編輯選擇大類
                    {
                        bool res = true;
                        query.frowID = isnum;
                        query.certificate_categoryname = string.Empty;
                        if (oldbiglist.Count > 0 && oldbiglist[0].certificate_categorycode != query.certificate_categorycode)//檔大類code被編輯時
                        {
                            //檢查大類code是否重複
                            CertificateCategoryQuery bigquery = new CertificateCategoryQuery();
                            bigquery.certificate_categorycode = query.certificate_categorycode;
                            res = _inspectionReport.CheckCode(bigquery);
                            if (res)
                            {
                                json = "{success:true,type:1,msg:'證書-大類CODE不能重複!'}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                        }
                        
                    }
                    #endregion
                    #region 編輯時修改大類
                    else
                    {
                        query.certificate_categoryname = fid;
                        if (_inspectionReport.CheckCertificateCategoryName(query))
                        {
                            json = "{success:true,type:1,msg:'證書-大類名稱不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        //檢查大類code是否重複
                        if (oldbiglist.Count > 0 && oldbiglist[0].certificate_categorycode != query.certificate_categorycode)
                        {
                            CertificateCategoryQuery bigquery = new CertificateCategoryQuery();
                            bigquery.certificate_categorycode = query.certificate_categorycode;
                            if (_inspectionReport.CheckCode(bigquery))
                            {
                                json = "{success:true,type:1,msg:'證書-大類CODE不能重複!'}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                        }
                    }
                    #endregion
                    #region 編輯時檔小類名稱修改時
                    if (oldlist.Count > 0 && oldlist[0].certificate_categoryname != query.certificate_category_childname)//證書-小類被修改
                    {
                        if (_inspectionReport.CheckChildName(query))//檔小類名稱存在時
                        {
                            json = "{success:true,type:1,msg:'證書-小類名稱不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    #endregion
                    #region  編輯時檔小類code修改時
                    if (oldlist[0].certificate_categorycode != query.certificate_category_childcode)//檔小類code被修改時
                    {
                        CertificateCategoryQuery smallquery = new CertificateCategoryQuery();
                        smallquery.certificate_categorycode = query.certificate_category_childcode;
                        if (_inspectionReport.CheckCode(smallquery))
                        {
                            json = "{success:true,type:1,msg:'證書-小類CODE不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    #endregion
                    #region 判斷大類小類code是否一樣
                    if (query.certificate_category_childcode == query.certificate_categorycode)
                    {
                        json = "{success:true,type:1,msg:'證書-小類名稱不能重複!'}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    #endregion
                    #region 更新大類
                    //更新大類
                    int istrue = _inspectionReport.UpdateCertificateCategory(query);
                    if (istrue <= 0)
                    {
                        json = "{success:true,type:0,msg:'證書-大類修改失敗!'}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    #endregion
                    #region 更新小類
                    if (_inspectionReport.Update(query))
                    {
                        json = "{success:true,type:0,msg:'修改成功!'}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    else
                    {
                        json = "{success:true,type:0,msg:'修改失敗!'}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }

                }
                #endregion
                #endregion
                #region 新增處理
                else
                { //新增
                    bool res = true;
                    #region 大類是選擇出來的
                    if (int.TryParse(fid, out isnum))
                    {

                        query.frowID = isnum;
                        //檢查證書-大類code是否重複
                        if (oldbiglist.Count > 0 && oldbiglist[0].certificate_categorycode != query.certificate_categorycode)//大類code被修改
                        {
                            CertificateCategoryQuery bigquery = new CertificateCategoryQuery();
                            bigquery.certificate_categorycode = query.certificate_categorycode;
                            res = _inspectionReport.CheckCode(bigquery);
                            if (res)
                            {
                                json = "{success:true,type:1,msg:'證書-大類CODE不能重複!'}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                        }
                        //檢查小類名稱是否重複
                        res = _inspectionReport.CheckChildName(query);
                        if (res)
                        {
                            json = "{success:true,type:1,msg:'證書-小類名稱不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        //檢查小類code是否重複
                        CertificateCategoryQuery smallquery = new CertificateCategoryQuery();
                        smallquery.certificate_categorycode = query.certificate_category_childcode;
                        res = _inspectionReport.CheckCode(smallquery);
                        if (res)
                        {
                            json = "{success:true,type:1,msg:'證書-小類CODE不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        int num = _inspectionReport.UpdateCertificateCategory(query);//更新大類信息
                        if (num <= 0)
                        {
                            json = "{success:true,type:0,msg:'證書-大類新增失敗!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #region 新增小類
                        if (_inspectionReport.AddSave(query) > 0)//新增保存
                        {
                            json = "{success:true,type:0,msg:'新增成功!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        else
                        {
                            json = "{success:true,type:0,msg:'新增失敗!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #endregion
                    }
                    #endregion
                    #region 大類也是新增
                    else
                    {
                        query.certificate_categoryname = fid;
                        if (_inspectionReport.CheckCertificateCategoryName(query))
                        {
                            json = "{success:true,type:1,msg:'證書-大類名稱不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        CertificateCategoryQuery bigquery = new CertificateCategoryQuery();
                        bigquery.certificate_categorycode = query.certificate_categorycode;
                        if (_inspectionReport.CheckCode(bigquery))
                        {
                            json = "{success:true,type:1,msg:'證書-大類CODE不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        //檢查小類名稱是否重複
                        if (query.certificate_category_childcode == query.certificate_categorycode)
                        {
                            json = "{success:true,type:1,msg:'證書-小類名稱不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        res = _inspectionReport.CheckChildName(query);
                        if (res)
                        {
                            json = "{success:true,type:1,msg:'證書-小類名稱不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        //檢查小類code是否重複
                        CertificateCategoryQuery smallquery = new CertificateCategoryQuery();
                        smallquery.certificate_categorycode = query.certificate_category_childcode;
                        res = _inspectionReport.CheckCode(smallquery);
                        if (res)
                        {
                            json = "{success:true,type:1,msg:'證書-小類CODE不能重複!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #region 新增大類
                        query.frowID = 0;
                        query.frowID = _inspectionReport.GetNewCertificateCategoryId(query);
                        if (query.frowID <= 0)
                        {
                            json = "{success:true,type:0,msg:'證書-大類新增失敗!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #endregion
                        #region 新增小類
                        if (_inspectionReport.AddSave(query) > 0)//新增保存
                        {
                            json = "{success:true,type:0,msg:'新增成功!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        else
                        {
                            json = "{success:true,type:0,msg:'新增失敗!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #endregion
                    }
                    #endregion
                    
                    //#region 新增大類
                    //int num = _inspectionReport.UpdateCertificateCategory(query);//更新大類信息
                    //if (num <= 0)
                    //{
                    //    json = "{success:true,type:0,msg:'證書-大類新增失敗!'}";
                    //    this.Response.Clear();
                    //    this.Response.Write(json);
                    //    this.Response.End();
                    //    return this.Response;
                    //}
                    //#endregion
                   
                }
                #endregion
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
        #endregion

        #region 檢驗報告
        public HttpResponseBase InspectionReportList()
        {
            string json = string.Empty;
            try
            {
                InspectionReportQuery query = new InspectionReportQuery();
                List<InspectionReportQuery> store = new List<InspectionReportQuery>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["brand"]))
                {
                    query.brand_id = Convert.ToUInt32(Request.Params["brand"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["name_code"]))
                {
                    query.name_code = Request.Params["name_code"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_type1"]))
                {
                    query.certificate_type1 = (Request.Params["certificate_type1"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_type2"]))
                {
                    query.certificate_type2 = (Request.Params["certificate_type2"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time =Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time =Convert.ToDateTime( Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59")); 
                }

                if (!string.IsNullOrEmpty(Request.Params["last_day"]))
                {
                    query.last_day = Convert.ToInt32(Request.Params["last_day"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_date"]))
                {
                    query.search_date = Convert.ToInt32(Request.Params["search_date"]);
                }
                
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                store = _inspectionReport.InspectionReportList(query, out totalCount);
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
        public HttpResponseBase BrandStore()
        {
            string json = string.Empty;
            try
            {
                VendorBrandQuery query = new VendorBrandQuery();
                List<VendorBrand> store = new List<VendorBrand>();
                query.Brand_Status = 1;
                _vendorBrand = new BLL.gigade.Mgr.VendorBrandMgr(mySqlConnectionString);
                store = _vendorBrand.GetProductBrandList(query);
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
        public HttpResponseBase SaveInspectionRe()
        {
            string json = string.Empty;
            try
            {
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                InspectionReportQuery query = new InspectionReportQuery();
                InspectionReportQuery oldQuery = new InspectionReportQuery();
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.brand_id = Convert.ToUInt32(Request.Params["brand_id"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["certificate_type1"]))
                {
                    query.certificate_type1 = Request.Params["certificate_type1"];
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_type2"]))
                {
                    query.certificate_type2 = Request.Params["certificate_type2"];
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_expdate"]))
                {
                    query.certificate_expdate = Convert.ToDateTime(Request.Params["certificate_expdate"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_desc"]))
                {
                    query.certificate_desc = Request.Params["certificate_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_filename"]))
                {
                    query.certificate_filename = Request.Params["certificate_filename"];
                }
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["rowID"]);
                    oldQuery = _inspectionReport.oldQuery(query);
                }
                if (!string.IsNullOrEmpty(Request.Params["sort"]))
                {
                    query.sort = Convert.ToInt32(Request.Params["sort"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["old_sort"]))
                {
                    query.old_sort = Convert.ToInt32(Request.Params["old_sort"]);
                }
                query.k_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.m_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                #region 排序是否重複
                if (_inspectionReport.IsSortExist(query) || query.old_sort == query.sort)//sort不重複
                {
                    if (!_inspectionReport.IsExist(query))
                    {

                        HttpPostedFileBase picFile = Request.Files["certificate_filename"];
                        //檢查上傳的圖片名稱和拼出來的圖片名稱是否一樣，不一樣則不能上傳
                        //拼成圖片名稱

                        #region 上傳圖片
                        try
                        {
                            string path = Server.MapPath(xmlPath);
                            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                            SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                            SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_Min_Element");
                            SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                            SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
                            SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
                            //擴展名、最小值、最大值
                            string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                            string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                            string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
                            string localInspectionReportPath = imgLocalPath + InspectionReportPath;//圖片存儲地址
                            FileManagement fileLoad = new FileManagement();
                            if (Request.Files.Count > 0)
                            {
                                HttpPostedFileBase file = Request.Files[0];
                                string fileName = string.Empty;//當前文件名
                                string fileExtention = string.Empty;//當前文件的擴展名

                                //
                                //文件夾名稱
                                string[] dirPath = new string[2];
                                dirPath[0] = query.brand_id.ToString() + "/".ToString();
                                dirPath[1] = query.product_id.ToString() + "/".ToString();
                                //
                                fileName = fileLoad.NewFileName(file.FileName);
                                if (fileName != "")
                                {

                                    string[] mapPath = new string[2];
                                    mapPath[0] = _inspectionReport.GetType1Folder(query);
                                    mapPath[1] = _inspectionReport.GetType2Folder(query);
                                    string NewFileName = string.Empty;
                                    NewFileName = query.brand_id + "-" + query.product_id + "-" + mapPath[0].Replace('/', ' ').Replace(" ", "") + "-" + mapPath[1].Replace('/', ' ').Replace(" ", "");
                                    #region 判斷圖片名稱是否正確，正確則保存
                                    if (picFile.FileName.Substring(0, picFile.FileName.LastIndexOf(".")) == NewFileName)
                                    {
                                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                                        string ServerPath = string.Empty;
                                        //判斷目錄是否存在，不存在則創建
                                        FTP f_cf = new FTP();
                                        CreateFolder(localInspectionReportPath, dirPath);
                                        //壓縮圖片 源文件
                                        string sourcePath = Server.MapPath(imgLocalServerPath + InspectionReportPath + dirPath[0] + dirPath[1] + fileName + fileExtention);

                                        if (!Directory.Exists(Server.MapPath(imgLocalServerPath + InspectionReportPath + dirPath[0] + dirPath[1])))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(imgLocalServerPath + InspectionReportPath + dirPath[0] + dirPath[1]));
                                        }

                                        file.SaveAs(sourcePath);
                                        fileName = NewFileName + fileExtention;
                                        NewFileName = localInspectionReportPath + dirPath[0] + dirPath[1] + NewFileName + fileExtention;
                                        ServerPath = Server.MapPath(imgLocalServerPath + InspectionReportPath + dirPath[0] + dirPath[1]);
                                        string ErrorMsg = string.Empty;
                                        if (query.rowID != 0)
                                        {
                                            string oldFileName = oldQuery.certificate_filename;
                                            CommonFunction.DeletePicFile(ServerPath + oldFileName);
                                            FTP ftp = new FTP(localInspectionReportPath + dirPath[0] + dirPath[1], ftpuser, ftppwd);
                                            List<string> tem = ftp.GetFileList();
                                            if (tem.Contains(oldFileName))
                                            {
                                                FTP ftps = new FTP(localInspectionReportPath + dirPath[0] + dirPath[1] + oldFileName, ftpuser, ftppwd);
                                                ftps.DeleteFile(localInspectionReportPath + dirPath[0] + dirPath[1] + oldFileName);
                                            }
                                        }
                                        try
                                        {
                                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                                            bool result = fileLoad.ZIPUpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd, sourcePath);
                                            if (result)//上傳成功
                                            {
                                                query.certificate_filename = fileName;
                                                //刪除本地圖片
                                                CommonFunction.DeletePicFile(sourcePath);
                                                json = _inspectionReport.SaveInspectionRe(query);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                            log.Error(logMessage);
                                            query.certificate_filename = oldQuery.certificate_filename;
                                        }
                                        if (!string.IsNullOrEmpty(ErrorMsg))
                                        {
                                            string jsonPic = string.Empty;
                                            jsonPic = "{success:true,msg:\"" + ErrorMsg + "\"}";
                                            this.Response.Clear();
                                            this.Response.Write(jsonPic);
                                            this.Response.End();
                                            return this.Response;
                                        }
                                    }
                                    else
                                    {
                                        json = "{success:'false',msg:'3'}";//圖片名稱不對
                                    }
                                    #endregion
                                }
                                else
                                {
                                    query.certificate_filename = oldQuery.certificate_filename;
                                    json = _inspectionReport.SaveInspectionRe(query);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            query.certificate_filename = oldQuery.certificate_filename;
                        }
                        #endregion


                        //  picFile.SaveAs();
                    }
                    else
                    {
                        json = "{success:'false',msg:'2'}";//重複數據
                    }
                }
                else
                {
                    json = "{success:'false',msg:'4'}";//重複數據
                }



                #endregion
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
        public HttpResponseBase DeleteInspectionRe()
        {
            string json = string.Empty;
            InspectionReportQuery query = new InspectionReportQuery();
            List<InspectionReportQuery> list = new List<InspectionReportQuery>();
            try
            {
                _inspectionReport = new BLL.gigade.Mgr.InspectionReportMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("∑") != -1)
                    {
                        foreach (string id in rowIDs.Split('∑'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                query = new InspectionReportQuery();
                                string[] data = id.Split(',');
                                query.rowID = Convert.ToInt32(data[0]);
                                query.brand_id = Convert.ToUInt32(data[1]);
                                query.product_id = Convert.ToUInt32(data[2]);
                                string oldFileName = data[3];
                                list.Add(query);
                                #region 刪除圖片
                                string localInspectionReportPath = imgLocalPath + InspectionReportPath;//圖片存儲地址
                                string[] dirPath = new string[2];

                                dirPath[0] = query.brand_id.ToString() + "/".ToString();
                                dirPath[1] = query.product_id.ToString() + "/".ToString();
                                string ServerPath = Server.MapPath(imgLocalServerPath + InspectionReportPath + dirPath[0] + dirPath[1]);
                                CommonFunction.DeletePicFile(ServerPath + oldFileName);
                                FTP ftp = new FTP(localInspectionReportPath + dirPath[0] + dirPath[1], ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFileName))
                                {
                                    FTP ftps = new FTP(localInspectionReportPath + dirPath[0] + dirPath[1] + oldFileName, ftpuser, ftppwd);
                                    ftps.DeleteFile(localInspectionReportPath + dirPath[0] + dirPath[1] + oldFileName);
                                }
                                #endregion
                            }
                        }
                    }
                }

                json = _inspectionReport.DeleteInspectionRe(list);
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
        public HttpResponseBase GetBrandID()
        {
            string json = string.Empty;
            try
            {
                InspectionReportQuery query = new InspectionReportQuery();
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_name"]))
                {
                    query.brand_name = (Request.Params["brand_name"]).ToString().Trim();
                    query.brand_status = 1;
                }
                _inspectionReport = new BLL.gigade.Mgr.InspectionReportMgr(mySqlConnectionString);
                json = _inspectionReport.GetBrandID(query);
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
        #region 驗證用，隨時增刪
        public HttpResponseBase GetType1Group()
        {
            List<CertificateCategoryQuery> store = new List<CertificateCategoryQuery>();
            CertificateCategoryQuery query = new CertificateCategoryQuery();
            string json = string.Empty;
            try
            {
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["ROWID"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["ROWID"]);
                }
                store = _inspectionReport.GetType1Store(query);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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
        public HttpResponseBase GetType2Group()
        {
            List<CertificateCategoryQuery> store = new List<CertificateCategoryQuery>();
            CertificateCategoryQuery query = new CertificateCategoryQuery();
            string json = string.Empty;
            try
            {
                _inspectionReport = new InspectionReportMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["ROWID"]))
                {
                    query.rowID = Convert.ToInt32(Request.Params["ROWID"]);
                    store = _inspectionReport.GetType2Store(query);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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
        public void CreateFolder(string path, string[] Mappath)
        {
            FTP ftp = null;
            try
            {
                string fullPath = path.Substring(0, path.Length - 1);
                string nodeDir = fullPath.Substring(fullPath.LastIndexOf("/") + 1);
                //創建跟目錄
                ftp = new FTP(fullPath.Substring(0, fullPath.LastIndexOf("/") + 1), ftpuser, ftppwd);
                if (!ftp.DirectoryExist(nodeDir))
                {
                    ftp = new FTP(fullPath, ftpuser, ftppwd);
                    ftp.MakeDirectory();
                }
                foreach (string s in Mappath)
                {
                    ftp = new FTP(fullPath.Substring(0, fullPath.Length), ftpuser, ftppwd);
                    fullPath += "/" + s;

                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length), ftpuser, ftppwd);
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
        #endregion
        #region 匯入模板
        public void UpdownTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("商品編號", typeof(String));
                dtTemplateExcel.Columns.Add("證書大類", typeof(String));
                dtTemplateExcel.Columns.Add("證書小類", typeof(String));
                dtTemplateExcel.Columns.Add("有效期限", typeof(String));
                dtTemplateExcel.Columns.Add("說明", typeof(String));
                dtTemplateExcel.Columns.Add("檔案名稱", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "檢查報告匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtTemplateExcel, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
        #endregion

        #region 檢查報告匯入
        public HttpResponseBase ImportExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            InspectionReportQuery query = new InspectionReportQuery();
            CertificateCategoryQuery ccquery = new CertificateCategoryQuery();
            List<CertificateCategoryQuery> CClist = new List<CertificateCategoryQuery>();
            List<CertificateCategoryQuery> CClistTwo = new List<CertificateCategoryQuery>();
            _inspectionReport = new BLL.gigade.Mgr.InspectionReportMgr(mySqlConnectionString);
            try
            {
                if (Request.Files["ImportExcelFile"] != null && Request.Files["ImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportExcelFile"];
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();
                    uint brandID = 0;
                    int truenum = 0;
                    json = "{success:true,msg:0,total:0}";
                    if (dt.Rows.Count > 0)
                    {
                        #region 循環數據判斷是否符合要求
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (string.IsNullOrEmpty(dr[0].ToString()))
                            {
                                continue;
                            }
                            uint num = 0;
                            if (uint.TryParse(dr[0].ToString(), out num))
                            {
                                query.product_id = num;
                                if (_inspectionReport.GetProductById(query))//判斷商品id是否存在
                                {
                                    brandID= _inspectionReport.GetBrandId(query.product_id);
                                    ccquery.certificate_categorycode = dr[1].ToString();//大類code是否存在
                                    CClist = _inspectionReport.CheckBigCode(ccquery);
                                    if (CClist != null && CClist.Count > 0)
                                    {
                                        query.certificate_type1 = CClist[0].rowID.ToString();
                                        ccquery.frowID = CClist[0].rowID;
                                        ccquery.certificate_category_childcode = dr[2].ToString();
                                        CClistTwo = _inspectionReport.GetLsit(ccquery);
                                        if (CClistTwo != null && CClistTwo.Count > 0)//小類code是否存在
                                        {
                                            query.certificate_type2 = CClistTwo[0].rowID.ToString();
                                            DateTime dtime = DateTime.MinValue;
                                            if (DateTime.TryParse(dr[3].ToString(), out dtime))//判斷有校時間格式是否正確
                                            {
                                                query.certificate_expdate = dtime;
                                                query.certificate_desc = dr[4].ToString();
                                                query.certificate_filename=brandID.ToString()+"-"+query.product_id.ToString()+"-"+ccquery.certificate_categorycode+"-"+ccquery.certificate_category_childcode+".jpg";
                                                if (!string.IsNullOrEmpty(dr[5].ToString())&&query.certificate_filename==dr[5].ToString())//判斷檔案是否為空,對應的檔案名稱是否正確
                                                {
                                                    query.certificate_filename = dr[5].ToString();
                                                    query.k_user = (Session["caller"] as Caller).user_id;
                                                    query.m_user = (Session["caller"] as Caller).user_id;
                                                    query.k_date = DateTime.Now;
                                                    query.m_date = DateTime.Now;
                                                    if (!_inspectionReport.CheckInspectionReport(query))//判斷數據是否已存在
                                                    {
                                                        if (_inspectionReport.InsertInspectionReport(query))//新增數據
                                                        {
                                                            truenum++;//一共新增成功多少條數據
                                                        }
                                                    }
                                                    //
                                                    //else
                                                    //{
                                                    //    if (_inspectionReport.UpdateInspectionReport(query))//新增數據
                                                    //    {
                                                    //        truenum++;//一共新增成功多少條數據
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        json = "{success:false}";
                                                    //    }
                                                    //}
                                                    //
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        json = "{success:true,msg:1,total:" + truenum + "}";
                    }
                    else
                    {
                        json = "{success:true,msg:3:total:0}";
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

        public HttpResponseBase Export()
        {
            InspectionReportQuery  query= new InspectionReportQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["brand"]))
                {
                    query.brand_id = Convert.ToUInt32(Request.Params["brand"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["name_code"]))
                {
                    query.name_code = Request.Params["name_code"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_type1"]))
                {
                    query.certificate_type1 = (Request.Params["certificate_type1"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["certificate_type2"]))
                {
                    query.certificate_type2 = (Request.Params["certificate_type2"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(Request.Params["last_day"]))
                {
                    query.last_day = Convert.ToInt32(Request.Params["last_day"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_date"]))
                {
                    query.search_date = Convert.ToInt32(Request.Params["search_date"]);
                }
                DataTable _newDt = new DataTable();
                string newExcelName = string.Empty;
                _newDt.Columns.Add("商品編號", typeof(string));
                _newDt.Columns.Add("證書大類", typeof(string));
                _newDt.Columns.Add("證書小類", typeof(string));
                _newDt.Columns.Add("有效期限", typeof(string));
                _newDt.Columns.Add("說明", typeof(string));
                _newDt.Columns.Add("檔案名稱", typeof(string));
                _inspectionReport = new BLL.gigade.Mgr.InspectionReportMgr(mySqlConnectionString);
              DataTable  _dt = _inspectionReport.Export(query);
              foreach (DataRow row in _dt.Rows)
              {
                  DataRow newRow=_newDt.NewRow();
                  newRow[0] = row["product_id"].ToString();
                  newRow[1] = row["code1"].ToString();
                  newRow[2] = row["code2"].ToString();
                  newRow[3] = row["certificate_expdate"].ToString();
                  newRow[4] = row["certificate_desc"];
                  newRow[5] = row["certificate_filename"].ToString();
                  _newDt.Rows.Add(newRow);
              }
               string[]colname=new string[_newDt.Columns.Count];
               string filename = "InspectionReport" +DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
               newExcelName = Server.MapPath(excelPath_export) + filename;
               for (int i = 0; i < _newDt.Columns.Count; i++)
               {
                   colname[i] = _newDt.Columns[i].ColumnName;
               }
               if (System.IO.File.Exists(newExcelName))
               {
                   System.IO.File.Delete(newExcelName);
               }
               ExcelHelperXhf.ExportDTtoExcel(_newDt, "", newExcelName);
               json = "{success:true,ExcelName:\'" + filename + "\'}";
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
