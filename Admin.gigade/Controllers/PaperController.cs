using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using BLL.gigade.Common;
using System.Collections.Specialized;
using System.IO;
using gigadeExcel.Comment;
using System.Data;

namespace Admin.gigade.Controllers
{
    public class PaperController : Controller
    {
        //
        // GET: /Paper/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:2121"
        string PaperPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.paperPath);//圖片保存路徑

        private IPaperImplMgr _paperMgr;


        private IPaperClassImplMgr _paperClassMgr;
        private IPaperAnswerImplMgr _paperAnswerMgr;
        #region 視圖
        public ActionResult PaperList()
        {
            return View();
        }
        public ActionResult PaperClassList()
        {
            return View();
        }
        public ActionResult PaperAnswerList()
        {
            if (!string.IsNullOrEmpty(Request.Params["paper_id"]) && !string.IsNullOrEmpty(Request.Params["user_id"]))
            {
                ViewBag.paperid = Request.Params["paper_id"];
                ViewBag.userid = Request.Params["user_id"];
            }
            else
            {
                ViewBag.paperid = "";
                ViewBag.userid = "";

            }
            return View();
        }
        #endregion
        #region 問卷
        public HttpResponseBase GetPaperList()
        {
            string json = string.Empty;
            List<Paper> store = new List<Paper>();
            Paper p = new Paper();
            try
            {
                p.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                p.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _paperMgr = new PaperMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    p.paperID = int.Parse(Request.Params["paper_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    p.status = 1;
                }
                p.paperName = Request.Params["paper_name"];
                if (!string.IsNullOrEmpty(Request.Params["isPage"]))
                {
                    p.IsPage = false;
                }
                store = _paperMgr.GetPaperList(p, out totalCount);
                foreach (var item in store)
                {
                    if (!string.IsNullOrEmpty(item.paperBanner))
                    {
                        item.paperBanner = imgServerPath + PaperPath + item.paperBanner;
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
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
        public HttpResponseBase PaperEdit()
        {
            string json = string.Empty;
            Paper p = new Paper();
            _paperMgr = new PaperMgr(mySqlConnectionString);

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
            string localBannerPath = imgLocalPath + PaperPath;//圖片存儲地址

            FileManagement fileLoad = new FileManagement();

            try
            {
                List<Paper> store = new List<Paper>();
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    int totalCount = 0;
                    p.IsPage = false;
                    p.paperID = int.Parse(Request.Params["paper_id"]);
                    store = _paperMgr.GetPaperList(p, out totalCount);
                }
                string oldImg = string.Empty;
                foreach (var item in store)
                {
                    oldImg = item.paperBanner;
                }

                p = new Paper();
                p.paperName = Request.Params["paper_name"];
                p.paperMemo = Request.Params["paper_memo"];
                p.bannerUrl = Request.Params["banner_url"];
                if (!string.IsNullOrEmpty(Request.Params["paper_start"]))
                {
                    p.paperStart = DateTime.Parse(DateTime.Parse(Request.Params["paper_start"]).ToString("yyyy-MM-dd") + " 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["paper_end"]))
                {
                    p.paperEnd = DateTime.Parse(DateTime.Parse(Request.Params["paper_end"]).ToString("yyyy-MM-dd") + " 23:59:59");
                }
                p.event_ID = Request.Params["eventid"];
                if (!string.IsNullOrEmpty(Request.Params["isRepeatGift"]))
                {
                    p.isRepeatGift = int.Parse(Request.Params["isRepeatGift"]);
                }
                //if (!string.IsNullOrEmpty(Request.Params["isPromotion"]))
                //{
                //    p.isPromotion = int.Parse(Request.Params["isPromotion"]);
                //    if (p.isPromotion == 1)
                //    {
                //        p.promotionUrl = Request.Params["promotion_url"];
                //    }
                //}
                //if (!string.IsNullOrEmpty(Request.Params["isPromotion"]))
                //{
                //    p.isPromotion = int.Parse(Request.Params["isPromotion"]);
                //    if (p.isPromotion == 1)
                //    {
                //        p.promotionUrl = Request.Params["promotion_url"];
                //    }
                //}

                //if (!string.IsNullOrEmpty(Request.Params["isGiveBonus"]))
                //{
                //    p.isGiveBonus = int.Parse(Request.Params["isGiveBonus"]);
                //    if (p.isGiveBonus == 1)
                //    {
                //        if (!string.IsNullOrEmpty(Request.Params["bonus_num"]))
                //        {
                //            p.bonusNum = int.Parse(Request.Params["bonus_num"]);
                //        }
                //    }
                //}

                //if (!string.IsNullOrEmpty(Request.Params["isGiveProduct"]))
                //{
                //    p.isGiveProduct = int.Parse(Request.Params["isGiveProduct"]);
                //    if (p.isGiveProduct == 1)
                //    {
                //        if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                //        {
                //            p.productID = int.Parse(Request.Params["product_id"]);
                //        }
                //    }
                //}

                if (!string.IsNullOrEmpty(Request.Params["isRepeatWrite"]))
                {
                    p.isRepeatWrite = int.Parse(Request.Params["isRepeatWrite"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isNewMember"]))
                {
                    p.isNewMember = int.Parse(Request.Params["isNewMember"]);
                }
                p.creator = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                if (addlist.Length > 0)
                {
                    p.ipfrom = addlist[0].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]) && Request.Params["paper_banner"] == oldImg)
                {
                    p.paperBanner = oldImg;
                }
                else
                {
                    string ServerPath = string.Empty;
                    ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                    if (Request.Files.Count > 0)//單個圖片上傳
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string fileName = string.Empty;//當前文件名

                        string fileExtention = string.Empty;//當前文件的擴展名
                        //獲取圖片名稱
                        fileName = fileLoad.NewFileName(file.FileName);
                        if (fileName != "")
                        {
                            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                            fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                            string NewFileName = string.Empty;

                            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                            NewFileName = hash.Md5Encrypt(fileName, "32");
                            //判斷目錄是否存在，不存在則創建
                            FTP f_cf = new FTP();
                            f_cf.MakeMultiDirectory(localBannerPath.Substring(0, localBannerPath.Length - PaperPath.Length + 1), PaperPath.Substring(1, PaperPath.Length - 2).Split('/'), ftpuser, ftppwd);

                            fileName = NewFileName + fileExtention;
                            NewFileName = localBannerPath + NewFileName + fileExtention;//絕對路徑

                            string ErrorMsg = string.Empty;

                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                p.paperBanner = fileName;
                                //上傳新圖片成功后，再刪除舊的圖片
                                CommonFunction.DeletePicFile(ServerPath + oldImg);//刪除本地圖片
                                FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldImg))
                                {
                                    FTP ftps = new FTP(localBannerPath + oldImg, ftpuser, ftppwd);
                                    ftps.DeleteFile(localBannerPath + oldImg);//刪除ftp:71.159上的舊圖片
                                }
                            }
                            else
                            {
                                p.paperBanner = oldImg;
                            }
                        }
                        else
                        {
                            //上傳之前刪除已有的圖片
                            CommonFunction.DeletePicFile(ServerPath + oldImg);//刪除本地圖片
                            FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldImg))
                            {
                                FTP ftps = new FTP(localBannerPath + oldImg, ftpuser, ftppwd);
                                ftps.DeleteFile(localBannerPath + oldImg);//刪除ftp:71.159上的舊圖片
                            }
                            p.paperBanner = "";
                        }

                    }
                }

                #region 新增
                if (String.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    if (_paperMgr.Add(p) > 0)
                    {
                        json = "{success:true,msg:\"" + "新增成功！" + "\"}";
                    }

                }
                #endregion
                #region 編輯
                else
                {
                    p.paperID = int.Parse(Request.Params["paper_id"]);
                    p.modifier = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    if (_paperMgr.Update(p) > 0)
                    {
                        json = "{success:true,msg:\"" + "修改成功！" + "\"}";
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                json = "{success:false,msg:\"" + "異常" + "\"}";
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

        #region 修改狀態，啟用或者禁用
        public JsonResult UpdateState()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            _paperMgr = new PaperMgr(mySqlConnectionString);
            Paper p = new Paper();
            p.paperID = id;
            p.status = activeValue;
            p.modifier = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
            System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            if (addlist.Length > 0)
            {
                p.ipfrom = addlist[0].ToString();
            }
            if (_paperMgr.UpdateState(p) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
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
        #endregion
        #region 問卷題目列表
        public HttpResponseBase GetPaperClassList()
        {
            string json = string.Empty;
            List<PaperClass> store = new List<PaperClass>();
            PaperClass pc = new PaperClass();
            try
            {
                pc.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                pc.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _paperClassMgr = new PaperClassMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    pc.paperID = int.Parse(Request.Params["paper_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                {
                    pc.classID = int.Parse(Request.Params["class_id"]);
                }
                store = _paperClassMgr.GetPaperClassList(pc, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
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
        public HttpResponseBase PaperClassEdit()
        {
            string json = string.Empty;
            PaperClass pc = new PaperClass();
            _paperClassMgr = new PaperClassMgr(mySqlConnectionString);
            List<PaperClass> store = new List<PaperClass>();
            List<PaperClass> oldstore = new List<PaperClass>();
            bool success = false;
            try
            {
                PaperClass pcc = new PaperClass();
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    pc.paperID = int.Parse(Request.Params["paper_id"]);//新的問卷編號
                    pcc.paperID = int.Parse(Request.Params["paper_id"]);
                }
                int totalCount = 0;
                pcc.IsPage = false;
                store = _paperClassMgr.GetPaperClassList(pcc, out totalCount);//新的問卷編號查出來的數據
                pcc = new PaperClass();
                pcc.IsPage = false;
                if (!string.IsNullOrEmpty(Request.Params["old_paper_id"]))
                {
                    pcc.paperID = int.Parse(Request.Params["old_paper_id"]);//舊的問卷編號
                }

                string type = Request.Params["type"];
                //設定題目編號 規則 paperID+四位自增數字（0001）
                if (String.IsNullOrEmpty(Request.Params["class_id"]))
                {
                    if (store.Count > 0)
                    {
                        int classid = store.Max(e => e.classID);
                        pc.classID = classid + 1;

                    }
                    else
                    {
                        pc.classID = int.Parse(pc.paperID.ToString() + "0001");
                    }
                }
                else
                {
                    //修改問卷的話，題目編號也要相應變動
                    if (pc.paperID != pcc.paperID)
                    {
                        if (store.Count > 0)
                        {
                            int classid = store.Max(e => e.classID);
                            pc.classID = classid + 1;

                        }
                        else
                        {
                            pc.classID = int.Parse(pc.paperID.ToString() + "0001");
                        }

                    }
                    else
                    {
                        pc.classID = int.Parse(Request.Params["class_id"]);
                    }
                }
                pc.className = Request.Params["class_name"];
                pc.classType = Request.Params["class_type"];
                //pc.bannerUrl = Request.Params["banner_url"];

                if (!string.IsNullOrEmpty(Request.Params["project_num"]))
                {
                    pc.projectNum = int.Parse(Request.Params["project_num"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ismust"]))
                {
                    pc.isMust = int.Parse(Request.Params["ismust"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["pcontent"]))
                {
                    string content = Request.Params["pcontent"].ToString();
                    string[] contents = content.Split(';');
                    string[] c;
                    for (int i = 0; i < contents.Length - 1; i++)
                    {
                        c = contents[i].Split(',');
                        pc.classContent = c[0].ToString();
                        pc.orderNum = int.Parse(c[1].ToString());
                        pc.id = int.Parse(c[2].ToString());
                        #region 新增
                        if (String.IsNullOrEmpty(Request.Params["class_id"]))
                        {
                            if (_paperClassMgr.Add(pc) > 0)
                            {
                                success = true;
                            }
                            else
                            {
                                success = false;
                            }

                        }
                        #endregion
                        #region 編輯
                        else
                        {
                            if (pc.id != 0)
                            {
                                if (_paperClassMgr.Update(pc) > 0)
                                {
                                    success = true;
                                }
                                else
                                {
                                    success = false;
                                }
                            }
                            //編輯時新增
                            else
                            {
                                if (_paperClassMgr.Add(pc) > 0)
                                {
                                    success = true;
                                }
                                else
                                {
                                    success = false;
                                }
                            }

                        }
                        #endregion
                    }
                }
                //題目類型選擇的不是單選多選時
                else
                {
                    #region 新增
                    if (String.IsNullOrEmpty(Request.Params["class_id"]))
                    {
                        if (_paperClassMgr.Add(pc) > 0)
                        {
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }

                    }
                    #endregion
                    #region 編輯
                    else
                    {
                        if (!string.IsNullOrEmpty(Request.Params["id"]))
                        {
                            pc.id = int.Parse(Request.Params["id"]);
                        }
                        if (_paperClassMgr.Update(pc) > 0)
                        {
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                    #endregion

                }
                //如果選擇新的問卷,就重組舊問卷的題目編號
                if (Request.Params["paper_id"] != Request.Params["old_paper_id"])
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                    {
                        oldstore = _paperClassMgr.GetPaperClassList(pcc, out totalCount);//舊的問卷編號查出來的數據
                        int class_id = int.Parse(Request.Params["class_id"]);
                        foreach (var item in oldstore)
                        {
                            //一個問卷中，如果舊的題目編號小於當前的題目編號
                            if (item.classID > class_id)
                            {
                                item.classID = item.classID - 1;
                                _paperClassMgr.UpdateClassID(item);
                            }
                        }
                    }
                }
                if (success)
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
                json = "{success:false,msg:\"" + "異常" + "\"}";
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

        #region 修改問卷題目選項的狀態，啟用或者禁用
        public JsonResult UpdateClassState()
        {
            string ids = Request.Params["id"];
            //string rids = Request.Params["id"];
            ids = ids.TrimEnd(',');
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            _paperClassMgr = new PaperClassMgr(mySqlConnectionString);
            if (_paperClassMgr.UpdateState(ids, activeValue) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }

        #endregion
        public HttpResponseBase PaperClassDelete()
        {
            string json = string.Empty;
            PaperClass pc = new PaperClass();
            _paperClassMgr = new PaperClassMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    pc.id = int.Parse(Request.Params["id"]);
                }
                if (_paperClassMgr.Delete(pc) > 0)
                {
                    json = "{success:true}";
                }
            }
            catch (Exception ex)
            {
                json = "{success:false,msg:\"" + "異常" + "\"}";
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
        #region 問卷答案
        public HttpResponseBase GetPaperAnswerList()
        {
            string json = string.Empty;
            List<PaperAnswer> store = new List<PaperAnswer>();
            PaperAnswer pa = new PaperAnswer();
            try
            {
                pa.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                pa.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _paperAnswerMgr = new PaperAnswerMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    pa.paperID = int.Parse(Request.Params["paper_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    pa.userid = int.Parse(Request.Params["user_id"]);
                }
                store = _paperAnswerMgr.GetPaperAnswerList(pa, out totalCount);
                foreach (var item in store)
                {
                    item.userMail = item.userMail.Split('@')[0] + "@***";
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
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

        public void OutExcel()
        {
            try
            {
                PaperAnswer pa = new PaperAnswer();
                _paperAnswerMgr = new PaperAnswerMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    pa.paperID = int.Parse(Request.Params["paper_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    pa.userid = int.Parse(Request.Params["user_id"]);
                }
                DataTable dt = _paperAnswerMgr.Export(pa);
                //DataTable ndt = dt.DefaultView.ToTable(false, new string[] { "paperName", "userid", "userMail", "order_id", "className", "classType", "answerContent", "classContent" });
                DataTable ndt = dt.DefaultView.ToTable(false, new string[] { "paperName", "userid", "order_id", "className", "classType", "answerContent", "classContent" });

                DataTable content = new DataTable();
                content = ndt.Clone();
                content.Columns.Add("answerDate", typeof(string));
                content.Columns.Remove("classContent");
                //content.Columns["paperName"].ColumnName = "問卷名稱";
                //content.Columns["userid"].ColumnName = "用戶id";
                //content.Columns["userMail"].ColumnName = "用戶郵箱";
                //content.Columns["className"].ColumnName = "題目名稱";
                //content.Columns["classType"].ColumnName = "題目類型";
                //content.Columns["answerContent"].ColumnName = "答案";
                DataRow dr;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dr = content.NewRow();
                    if (i > 0)
                    {
                        if (dt.Rows[i]["paperID"].ToString() == dt.Rows[i - 1]["paperID"].ToString())
                        {
                            //dr["userid"] = ndt.Rows[i]["userid"];
                            //dr["userMail"] = ndt.Rows[i]["userMail"];
                            //content.Rows.Add(dr);
                            if (dt.Rows[i]["userid"].ToString() == dt.Rows[i - 1]["userid"].ToString() && dt.Rows[i]["answerDate"].ToString() == dt.Rows[i - 1]["answerDate"].ToString())
                            {
                                dr["className"] = dt.Rows[i]["className"];
                                //dr["classType"] = dt.Rows[i]["classType"];
                                switch (dt.Rows[i]["classType"].ToString())
                                {
                                    case "SC":
                                        dr["classType"] = "單選";
                                        dr["answerContent"] = dt.Rows[i]["classContent"];
                                        break;
                                    case "MC":
                                        dr["classType"] = "多選";
                                        dr["answerContent"] = dt.Rows[i]["classContent"];
                                        break;
                                    case "SL":
                                        dr["classType"] = "單行";
                                        dr["answerContent"] = dt.Rows[i]["answerContent"];
                                        break;
                                    case "ML":
                                        dr["classType"] = "多行";
                                        dr["answerContent"] = dt.Rows[i]["answerContent"];
                                        break;
                                }

                                if (!string.IsNullOrEmpty(dt.Rows[i]["answerDate"].ToString()))
                                {
                                    dr["answerDate"] = (DateTime.Parse(dt.Rows[i]["answerDate"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                content.Rows.Add(dr);
                            }
                            else
                            {
                                dr["userid"] = dt.Rows[i]["userid"];
                                //dr["userMail"] = dt.Rows[i]["userMail"];
                                dr["order_id"] = dt.Rows[i]["order_id"];
                                content.Rows.Add(dr);
                                dr = content.NewRow();
                                dr["className"] = dt.Rows[i]["className"];
                                //dr["classType"] = dt.Rows[i]["classType"];
                                switch (dt.Rows[i]["classType"].ToString())
                                {
                                    case "SC":
                                        dr["classType"] = "單選";
                                        dr["answerContent"] = dt.Rows[i]["classContent"];
                                        break;
                                    case "MC":
                                        dr["classType"] = "多選";
                                        dr["answerContent"] = dt.Rows[i]["classContent"];
                                        break;
                                    case "SL":
                                        dr["classType"] = "單行";
                                        dr["answerContent"] = dt.Rows[i]["answerContent"];
                                        break;
                                    case "ML":
                                        dr["classType"] = "多行";
                                        dr["answerContent"] = dt.Rows[i]["answerContent"];
                                        break;
                                }
                                if (!string.IsNullOrEmpty(dt.Rows[i]["answerDate"].ToString()))
                                {
                                    dr["answerDate"] = DateTime.Parse(dt.Rows[i]["answerDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                content.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            dr["paperName"] = dt.Rows[i]["paperName"];
                            content.Rows.Add(dr);
                            dr = content.NewRow();
                            dr["userid"] = dt.Rows[i]["userid"];
                            //dr["userMail"] = dt.Rows[i]["userMail"];
                            dr["order_id"] = dt.Rows[i]["order_id"];
                            content.Rows.Add(dr);
                            dr = content.NewRow();
                            dr["className"] = dt.Rows[i]["className"];
                            //dr["classType"] = dt.Rows[i]["classType"];
                            switch (dt.Rows[i]["classType"].ToString())
                            {
                                case "SC":
                                    dr["classType"] = "單選";
                                    dr["answerContent"] = dt.Rows[i]["classContent"];
                                    break;
                                case "MC":
                                    dr["classType"] = "多選";
                                    dr["answerContent"] = dt.Rows[i]["classContent"];
                                    break;
                                case "SL":
                                    dr["classType"] = "單行";
                                    dr["answerContent"] = dt.Rows[i]["answerContent"];
                                    break;
                                case "ML":
                                    dr["classType"] = "多行";
                                    dr["answerContent"] = dt.Rows[i]["answerContent"];
                                    break;
                            }
                            if (!string.IsNullOrEmpty(dt.Rows[i]["answerDate"].ToString()))
                            {
                                dr["answerDate"] = DateTime.Parse(dt.Rows[i]["answerDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            content.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr["paperName"] = dt.Rows[i]["paperName"];
                        content.Rows.Add(dr);
                        dr = content.NewRow();
                        dr["userid"] = dt.Rows[i]["userid"];
                        //dr["userMail"] = dt.Rows[i]["userMail"];
                        dr["order_id"] = dt.Rows[i]["order_id"];
                        content.Rows.Add(dr);
                        dr = content.NewRow();
                        dr["className"] = dt.Rows[i]["className"];
                        //dr["classType"] = dt.Rows[i]["classType"];
                        switch (dt.Rows[i]["classType"].ToString())
                        {
                            case "SC":
                                dr["classType"] = "單選";
                                dr["answerContent"] = dt.Rows[i]["classContent"];
                                break;
                            case "MC":
                                dr["classType"] = "多選";
                                dr["answerContent"] = dt.Rows[i]["classContent"];
                                break;
                            case "SL":
                                dr["classType"] = "單行";
                                dr["answerContent"] = dt.Rows[i]["answerContent"];
                                break;
                            case "ML":
                                dr["classType"] = "多行";
                                dr["answerContent"] = dt.Rows[i]["answerContent"];
                                break;
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["answerDate"].ToString()))
                        {
                            dr["answerDate"] = DateTime.Parse(dt.Rows[i]["answerDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        content.Rows.Add(dr);

                    }

                }
                content.Columns["paperName"].ColumnName = "問卷名稱";
                content.Columns["userid"].ColumnName = "用戶編號";
                //content.Columns["userMail"].ColumnName = "用戶郵箱";
                content.Columns["order_id"].ColumnName = "訂單編號";
                content.Columns["className"].ColumnName = "題目名稱";
                content.Columns["classType"].ColumnName = "題目類型";
                content.Columns["answerContent"].ColumnName = "答案";
                content.Columns["answerDate"].ColumnName = "作答時間";
                string fileName = "問卷答案_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(content, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

        }

        public void OutSinglePaperExcel()
        {
            try
            {
                PaperAnswer pa = new PaperAnswer();
                PaperClass pc = new PaperClass();
                string paper_name = Request.Params["paper_name"];
                _paperAnswerMgr = new PaperAnswerMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["paper_id"]))
                {
                    pa.paperID = int.Parse(Request.Params["paper_id"]);
                    pc.paperID = int.Parse(Request.Params["paper_id"]);
                }
                DataTable dtclass = _paperAnswerMgr.GetPaperClassID(pc);
                DataTable dtuser = _paperAnswerMgr.GetPaperAnswerUser(pa);
                DataTable dtanswer = _paperAnswerMgr.ExportSinglePaperAnswer(pa);
                DataTable dt = new DataTable();
                dt.Columns.Add("用戶編號", typeof(string));
                //dt.Columns.Add("用戶郵箱", typeof(string));
                dt.Columns.Add("訂單編號", typeof(string));
                dt.Columns.Add("作答時間", typeof(string));
                for (int i = 0; i < dtclass.Rows.Count; i++)
                {
                    dt.Columns.Add("題目" + (i + 1), typeof(string));
                }
                for (int i = 0; i < dtuser.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["用戶編號"] = dtuser.Rows[i]["userid"];
                    // dr["用戶郵箱"] = dtuser.Rows[i]["userMail"];
                    dr["訂單編號"] = dtuser.Rows[i]["order_id"];
                    dr["作答時間"] = DateTime.Parse(dtuser.Rows[i]["answerDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    for (int j = 0; j < dtclass.Rows.Count; j++)
                    {
                        DataRow[] drs = dtanswer.Select("userid=" + dtuser.Rows[i]["userid"] + " and answerDate='" + DateTime.Parse(dtuser.Rows[i]["answerDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "' and classID=" + dtclass.Rows[j]["classID"]);
                        string cname = "題目" + (j + 1);
                        if (drs.Count() == 0)
                        {
                            dr[cname] = "";
                        }
                        else if (drs.Count() == 1)
                        {
                            if (dtclass.Rows[j]["classType"].ToString() == "MC" || dtclass.Rows[j]["classType"].ToString() == "SC")
                            {
                                dr[cname] = drs[0]["classContent"];
                            }
                            else if (dtclass.Rows[j]["classType"].ToString() == "ML" || dtclass.Rows[j]["classType"].ToString() == "SL")
                            {
                                dr[cname] = drs[0]["answerContent"];
                            }

                        }
                        //多選題目,答案組合
                        else if (drs.Count() > 1)
                        {
                            for (int k = 0; k < drs.Count(); k++)
                            {
                                dr[cname] += "|" + drs[k]["classContent"];
                            }
                            dr[cname] = dr[cname].ToString().TrimStart('|');
                        }
                    }
                    dt.Rows.Add(dr);
                }
                if (dt.Rows.Count > 0)
                {
                    string fileName = paper_name + "_答案_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dt, paper_name);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else {
                    Response.Write("會出數據不存在");
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
    }
}
