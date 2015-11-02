using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class XmlManageController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string parentPath = "~/XML/";///定義公共的xml所存放的文件夾
        IHandleXmlImplMgr _xmlInfo = new HandleXmlMgr();                        

        public ActionResult Index()
        {
            return View();
        }


        public string XmlFile(string fileName = "")
        {
            if (fileName == "") return "";
            string path = parentPath + fileName;
            path = Server.MapPath(path);
            return _xmlInfo.GetStrCopy(path);
        }

        [HttpPost]
        public ActionResult XmlList(string fileName = "")
        {
            try
            {
                if (fileName == "") return new EmptyResult();
                string path = parentPath + fileName;
                path = Server.MapPath(path);
                string resultStr = _xmlInfo.GetXmlInfo(path);
                return Content(resultStr);
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult GetFileName()
        {
            try
            {

                ///獲得要搜索的文件名
                string xmlName = Request["fileName"];

                ///獲取查找路徑
                string path = Server.MapPath(parentPath);

                ///獲得xml文件名
                List<XmlModelCustom> list = _xmlInfo.GetXmlName(path);

                ///如果存在搜索條件
                if (xmlName!=null && xmlName!="") {
                    /// 查詢符合條件的集合
                    List<XmlModelCustom> resultList = list.FindAll(m => m.fileName.Contains(xmlName));
                    return Json(new { item = resultList });
                }
                return Json(new { item = list});
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



        public ActionResult UpdateNode()
        {
            try
            {
                XmlModelCustom xmc = new XmlModelCustom();
                int type = Request["type"] == null ? 0 : Convert.ToInt32(Request["type"]);
                xmc.rowId = Request["rowId"] == null ? 0 : Convert.ToInt32(Request["rowId"]);
                xmc.fileName = Request["fileName"];
                xmc.name = Request["name"].Trim();
                xmc.code = Request["code"].Trim();
                xmc.brotherName = Request["brotherName"].Trim();
                xmc.attributes = Request["attributes"].Trim();
                xmc.childName = Request["childName"].Trim(); 
                IHandleXmlImplMgr _xmlInfo = new HandleXmlMgr();
                string path = parentPath + xmc.fileName;
                path = Server.MapPath(path);
                bool flag=false;
                ///如果為4 刪除文件,則直接調用刪除文件方法
                if (type == 4)
                {
                    flag = _xmlInfo.CreateFile(path, xmc.name);
                    return Json(new { success = flag });
                }
                flag = _xmlInfo.SaveOrUpdate(path, xmc, type);
                return Json(new { success = flag });
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

        public ActionResult DeleteFile()
        {
            try
            {
                string fileName = Request["fileName"];
                string fullPath = parentPath + fileName;
                bool flag = _xmlInfo.DeleteFile(Server.MapPath(fullPath));
                return Json(new { success = flag });
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
    }
}
