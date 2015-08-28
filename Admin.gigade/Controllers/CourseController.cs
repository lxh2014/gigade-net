using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.Web;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    public class CourseController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);
        //
        // GET: /Courses/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult upLoad()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CurriculDetailView(int course_id)
        {
            ViewBag.CourseId = course_id;
            return View();
        }

        /// <summary>
        /// 初進頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult CurriculuManage()
        {

            List<Schedule> list = new List<Schedule>();
            try
            {
                IScheduleImplMgr _FstMgr = new ScheduleMgr(connectionString);
                //list = _FstMgr.Query();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            return View();
        }

        /// <summary>
        /// 獲取課程data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurricul(Course c)
        {
            try
            {
                int count = 0;
                c.Start = int.Parse(Request.Form["start"] ?? "0");
                if (Request["limit"] != null)
                {
                    c.Limit = Convert.ToInt32(Request["limit"]);
                }
                ICourseImplMgr _courseMgr = new CourseMgr(connectionString);
                bool isPage = Convert.ToBoolean(Request["IsPage"]);

                if (isPage)
                {
                    var course = _courseMgr.Query(c, out count);
                    IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                    return Content("{succes:true,totalCount:" + count + ",item:" + JsonConvert.SerializeObject(course, Formatting.None, iso) + "}");
                }
                else
                {
                    var course = _courseMgr.Query(c);
                    return Json(course);
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }

        }

        /// <summary>
        /// 獲取課程詳情 data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurriculDetail(int course_id = 0)//add by wwei0216w 2015/5/11 修改課程F12調試時顯示的錯誤信息
        {
            try
            {
                ICourseDetailImplMgr _courseDetailMgr = new CourseDetailMgr(connectionString);
                var course = _courseDetailMgr.Query(new CourseDetail { Course_Id = course_id });
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                return Content(JsonConvert.SerializeObject(course, Formatting.None, iso));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }


        /// <summary>
        /// 獲取Ticket data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurriculTicket(int course_detail_id)
        {
            try
            {
                string xmlPath = Server.MapPath("../XML/ParameterSrc.xml");

                ICourseTicketImplMgr _courseTicketMgr = new CourseTicketMgr(connectionString);
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                return Content(JsonConvert.SerializeObject(_courseTicketMgr.Query(course_detail_id, xmlPath), Formatting.None, iso));

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }


        /// <summary>
        /// 保存課程數據
        /// </summary>
        /// <param name="fst"></param>
        /// <returns></returns>
        public ActionResult CurriculSave(string courseStr, string courseDetailStrs, string imgStore)
        {
            try
            {
                Course course = JsonConvert.DeserializeObject<Course>(courseStr);
                List<CourseDetail> courseDetails = JsonConvert.DeserializeObject<List<CourseDetail>>(courseDetailStrs);
                List<CoursePicture> imgList = JsonConvert.DeserializeObject<List<CoursePicture>>(imgStore);
                imgServerPath = imgServerPath + "/course/a/";
                foreach(CoursePicture item in imgList)
                {
                    item.picture_name = item.picture_name.Replace(imgServerPath, " ");
                }
                ICourseImplMgr _courseMgr = new CourseMgr(connectionString);
                return Json(new { success = _courseMgr.ExecuteAll(course, courseDetails, imgList) });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = false });
            }
        }

       
        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveCourseImg()
        {
            string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
            try
            {
                ViewBag.status = false;
                string path = Server.MapPath(xmlPath); ///<add key="SiteConfig" value="/Config.xml"/>
                ISiteConfigImplMgr siteConfigMgr;
                siteConfigMgr = new SiteConfigMgr(path);
                SiteConfig extention_config = siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                SiteConfig minValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
                SiteConfig maxValue_config = siteConfigMgr.GetConfigByName("PIC_640_Length_Max");
                string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
                HttpPostedFileBase file = Request.Files["Filedata"];
                //int courseCheck = file.FileName.LastIndexOf("course_");
                string nameType = "course";
                string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);
                string coursePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.coursePath);
                string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//<add key="imglocalpath" value=" ftp://192.168.18.166:2121/img"/> FTP路徑
                string coursePicturePath = imgLocalPath + coursePath;//課程圖片保存路徑
                string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];
                string[] Mappath = new string[2];
                FileManagement fileLoad = new FileManagement();
                string fileName = string.Empty;
                string fileExtention = string.Empty;
                ViewBag.moreFileOneTime = true;//圖片狀態判斷
                int courseNameCheck = file.FileName.LastIndexOf("course_");//獲取圖片前綴名稱
                string errorMsg = "ERROR/";
                //if (courseNameCheck==-1)
                //{
                //    errorMsg += "[" + file.FileName + "] ";
                //    errorMsg += Resources.Product.FILE_NAME_ERROR;
                //    ViewBag.fileName = errorMsg;
                //    //return View("~/Views/Product/upLoad.cshtml");
                //    return View("~/Views/Course/upLoad.cshtml");
                //}

                fileName = nameType + fileLoad.NewFileName(file.FileName);
                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.'));


                string NewFileName = string.Empty;
                string returnName = imgServerPath;
                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                NewFileName = hash.Md5Encrypt(fileName, "32");
                string firstFolder = NewFileName.Substring(0, 2) + "/";
                string secondFolder = NewFileName.Substring(2, 2) + "/";
                string ServerPath = string.Empty;

                if (nameType == "course")
                {
                    Mappath[0] = firstFolder;
                    Mappath[1] = secondFolder;
                    CreateFolder(coursePicturePath, Mappath);
                    coursePicturePath += firstFolder + secondFolder;

                    coursePath += firstFolder + secondFolder;

                    returnName += coursePath + NewFileName + fileExtention;

                    NewFileName = coursePicturePath + NewFileName + fileExtention;
                    ServerPath = Server.MapPath(imgLocalServerPath + coursePath);

                }
                string ErrorMsg = string.Empty;
                Resource.CoreMessage = new CoreResource("Product");

                //上傳圖片
                bool result = false;
                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
               
                if (string.IsNullOrEmpty(ErrorMsg))
                {
                    ViewBag.fileName = returnName;
                    ViewBag.Type = nameType;
                    ViewBag.status = true;
                    //獲取文件長度 add by zhuoqin0830w 2015/01/29
                    string[] strFile = file.FileName.Split('_');
                    //判斷文件名的長度是否大於 1 如果大於 1 則再次進行判斷是否為數字 如果不是則進行強制轉換  
                    int image_sort = 0;
                    int.TryParse(strFile.Length > 1 ? strFile[1] : "0", out image_sort);
                    ViewBag.image_sort = image_sort;
                }
                else
                { 
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = "ERROR/" + ErrorMsg;
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            return View("~/Views/Course/upLoad.cshtml");
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


        /// <summary>
        /// 獲取課程圖檔
        /// </summary>
        /// <returns></returns>

        public ActionResult GetCourseImg(int course_id = 0)
        {
            List<CoursePicture> list = new List<CoursePicture>();
            try
            {
                ICoursePictureImplMgr _coursePictureMgr = new CoursePictureMgr(connectionString);
                CoursePicture cp = new CoursePicture();
                cp.course_id = course_id;
   
                imgServerPath = imgServerPath + "/course/a/";
                list = _coursePictureMgr.Query(cp);
                for (int i = 0; i < list.Count;i++ )
                {
                    list[i].id =Convert.ToUInt32(i);
                    list[i].picture_name = list[i].picture_name.Replace(" ", imgServerPath);
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            return Json(new { images = list });
        }



    }
}
