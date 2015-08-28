using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace Admin.gigade.Controllers
{
    public class ProductTierController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IScheduleImplMgr _fstMgr;
        IScheduleItemImplMgr _scheduleItemMgr;
        /// <summary>
        /// 保存數據
        /// </summary>
        /// <param name="fst"></param>
        /// <returns></returns>
        public ActionResult TierSetSave(Schedule fst)
        {

            try
            {
                _fstMgr = new ScheduleMgr(connectionString);
                fst.create_user = (Session["caller"] as Caller).user_id;
                fst.create_date = DateTime.Now;
                bool result = _fstMgr.Save(fst);
                return Json(new { success = result });
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
        /// 頁面加載
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// 查詢排程
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public ActionResult GetTiers(ScheduleQuery sc)
        {
            try
            {
                int vendor_id = Request["vendorId"] == null ? 0:int.Parse(Request["vendorId"]);
                string product_id = Request["product_id"] == null ? "" : Request["product_id"];
                List<Schedule> list = new List<Schedule>();
                _fstMgr = new ScheduleMgr(connectionString);
                list = _fstMgr.Query(sc);
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd" };
                return Content(JsonConvert.SerializeObject(list, Formatting.None, iso));
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

        public ActionResult GetTiersByConditon(ScheduleQuery sc)
        {
            try
            {
                string vendor_id = Request["vendorId"] == null ? "0" : Request["vendorId"];
                string product_id = Request["product_id"] == null ? "" : Request["product_id"];
                IScheduleItemImplMgr _scheduleItemMgr = new ScheduleItemMgr(connectionString);
                List<ScheduleItemCustom> list = _scheduleItemMgr.QueryByCondition(new ScheduleItem { type = 1 });
                List<ScheduleItemCustom> resultList = new List<ScheduleItemCustom>();
                
                if (product_id != "")
                {
                    list = list.FindAll(m => m.key1 == 1);// || m.key2 == 1 || m.key3 == 1
                    if (list.Count > 0)
                    {
                        list = list.FindAll(m => m.value1 == vendor_id);//|| m.value2 == vendor_id || m.value3 == vendor_id
                    }
                }
                //foreach (ScheduleItemCustom sic in list)
                //{
                //    ScheduleItemCustom s = new ScheduleItemCustom();
                //    s.desc = sic.desc;
                //    s.schedule_Id = sic.schedule_Id;
                //    s.schedule_name = sic.schedule_name;
                //    resultList.Add(s);
                //}
                resultList.AddRange(list);
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd" };
                return Content(JsonConvert.SerializeObject(resultList, Formatting.None, iso));
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


        [CustomHandleError]
        [HttpPost]
        public ActionResult DeleteTires(string rowID)
        {
            try
            {
                int[] ids = (from i in rowID.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
                _fstMgr = new ScheduleMgr(connectionString);
                _fstMgr.Delete(ids);
                return Json(new { success = true });
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
        /// 獲取關係表數據by schedule_id
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public ActionResult GetRelationTiers(ScheduleRelation sr)
        {
            try
            {
                List<ScheduleRelation> list = new List<ScheduleRelation>();
                IScheduleRelationImplMgr _srMgr = new ScheduleRelationMgr(connectionString);
                list = _srMgr.Query(sr.schedule_id);
                return Content(JsonConvert.SerializeObject(list));
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
        /// 加載類型store
        /// </summary>
        [HttpPost]
        public ActionResult GetRelevantType()
         {
            try
            {
                string type = Request["type"];
                string xmlPath = Server.MapPath("../XML/ParameterSrc.xml");
                _fstMgr = new ScheduleMgr(connectionString);
                return Json(_fstMgr.GetRelevantInfo(xmlPath, type));
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

        [HttpPost]
        public ActionResult GetValueInfo()
        {
            try
            {
                int keyType =Request["keyType"]==null ? 0: int.Parse(Request["keyType"]);//key的值
                keyType = 1;
                _fstMgr = new ScheduleMgr(connectionString);
                return Json(_fstMgr.GetValue(keyType));
                
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

        public ActionResult SaveItem()
        {
            try
            {
                string scheduleStr = Request["relevants"];
                List<ScheduleItem> lists = JsonConvert.DeserializeObject<List<ScheduleItem>>(scheduleStr);
                _scheduleItemMgr = new ScheduleItemMgr(connectionString);
                return Json(_scheduleItemMgr.UpdateByBacth(lists));
           
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
        
       
        public ActionResult GetItemInfo()
        {
            try
            {
                int scheduleid = Request["scheduleid"] == null ? 0 : int.Parse(Request["scheduleid"]);//key的值
                _scheduleItemMgr = new ScheduleItemMgr(connectionString);
                List<ScheduleItemCustom> list = _scheduleItemMgr.Query(new ScheduleItem { schedule_Id = scheduleid });
                return Json(list);
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

        public ActionResult DeleteItem()
        {
            try
            {

                string item_type = Request["item_type"] == null ? "0" : Request["item_type"];
                string item_value = Request["item_value"] == null ? "0" : Request["item_value"];
                string rowIds = Request["rowID"];
                rowIds =rowIds.Remove(rowIds.Length-1, 1);
                string schedule_id = Request["schedule_id"];
                _scheduleItemMgr = new ScheduleItemMgr(connectionString);
                return Json(_scheduleItemMgr.Delete(Convert.ToInt32(schedule_id),rowIds,item_type,item_value));
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
        /// 加載類型relevantStore
        /// </summary>
        public ActionResult relevantStore()
        {
            try
            {
                int scheduleid = Request["schedule_id"] == null ? 0 : int.Parse(Request["schedule_id"]);//key的值
                string xmlPath = Server.MapPath("../XML/ParameterSrc.xml");
                _scheduleItemMgr = new ScheduleItemMgr(connectionString);
                List<ScheduleItemCustom> list = _scheduleItemMgr.Query(new ScheduleItem { schedule_Id=scheduleid});
                _fstMgr = new ScheduleMgr(connectionString);
                List<Parametersrc> typeList = _fstMgr.GetRelevantInfo(xmlPath, "ScheduleType");
                List<Parametersrc> keyList = _fstMgr.GetRelevantInfo(xmlPath, "Schedule_Key");
                foreach (ScheduleItemCustom i in list)
                {
                    i.keyStr = keyList.Find(m=>m.ParameterCode==i.key1.ToString()).parameterName;
                    i.tabType = typeList.Find(m=>m.ParameterCode==i.type.ToString()).parameterName;
                }
                return Json(list);
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
    }
}
