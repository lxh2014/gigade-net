using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class VipUserGroupController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private VipUserGroupMgr _vipUserGroup;
        private GroupCommitteContactMgr _gccMgr;
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        //
        // GET: /VipUserGroup/
        //企業會員管理
        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseBase GetZipStore()
        {
            List<ZipQuery> store = new List<ZipQuery>();
            string json = string.Empty;
            _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
            try
            {
                store = _vipUserGroup.GetZipStore();
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


        public HttpResponseBase InsertVipUserGroup()
        {
            string json = string.Empty;

            try
            {
                VipUserGroupQuery query = new VipUserGroupQuery();
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    query.group_name = Request.Params["group_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["eng_name"]))
                {
                    query.eng_name = Request.Params["eng_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["tax_id"]))
                {
                    query.tax_id = Request.Params["tax_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_code"]))
                {
                    query.group_code = Request.Params["group_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["check_iden"]))
                {
                    query.check_iden = Convert.ToInt32(Request.Params["check_iden"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_capital"]))
                {
                    query.group_capital = Convert.ToInt32(Request.Params["group_capital"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_emp_number"]))
                {
                    query.group_emp_number = Convert.ToInt32(Request.Params["group_emp_number"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_emp_age"]))
                {
                    query.group_emp_age = Request.Params["group_emp_age"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_emp_gender"]))
                {
                    query.group_emp_gender = Convert.ToInt32(Request.Params["group_emp_gender"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_benefit_type"]))
                {
                    query.group_benefit_type = Request.Params["group_benefit_type"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_benefit_desc"]))
                {
                    query.group_benefit_desc = Request.Params["group_benefit_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_subsidiary"]))
                {
                    query.group_subsidiary = Convert.ToInt32(Request.Params["group_subsidiary"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_hq_name"]))
                {
                    query.group_hq_name = Request.Params["group_hq_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_hq_code"]))
                {
                    query.group_hq_code = Request.Params["group_hq_code"];
                }
                _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                json = _vipUserGroup.InsertVipUserGroup(query);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase SaveVipUserGroup()
        {
            string json = string.Empty;
            try
            {
                VipUserGroupQuery query = new VipUserGroupQuery();

                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    query.group_name = Request.Params["group_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["eng_name"]))
                {
                    query.eng_name = Request.Params["eng_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["tax_id"]))
                {
                    query.tax_id = Request.Params["tax_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_code"]))
                {
                    query.group_code = Request.Params["group_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_capital"]))
                {
                    query.group_capital = Convert.ToInt32(Request.Params["group_capital"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["check_iden"]))
                {
                    query.check_iden = Convert.ToInt32(Request.Params["check_iden"]);
                }
                
                if (!string.IsNullOrEmpty(Request.Params["group_emp_number"]))
                {
                    query.group_emp_number = Convert.ToInt32(Request.Params["group_emp_number"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_emp_age"]))
                {
                    query.group_emp_age = Request.Params["group_emp_age"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_emp_gender"]))
                {
                    query.group_emp_gender = Convert.ToInt32(Request.Params["group_emp_gender"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_benefit_type"]))
                {
                    query.group_benefit_type = Request.Params["group_benefit_type"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_benefit_desc"]))
                {
                    query.group_benefit_desc = Request.Params["group_benefit_desc"].Replace("\\", "\\\\"); ;
                }
                if (!string.IsNullOrEmpty(Request.Params["group_subsidiary"]))
                {
                    query.group_subsidiary = Convert.ToInt32(Request.Params["group_subsidiary"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_hq_name"]))
                {
                    query.group_hq_name = Request.Params["group_hq_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_hq_code"]))
                {
                    query.group_hq_code = Request.Params["group_hq_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_name"]))
                {
                    query.group_committe_name = Request.Params["group_committe_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_code"]))
                {
                    query.group_committe_code = Request.Params["group_committe_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_chairman"]))
                {
                    query.group_committe_chairman = Request.Params["group_committe_chairman"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_phone"]))
                {
                    query.group_committe_phone = (Request.Params["group_committe_phone"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_mail"]))
                {
                    query.group_committe_mail = (Request.Params["group_committe_mail"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_promotion"]))
                {
                    query.group_committe_promotion = (Request.Params["group_committe_promotion"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_other"]))
                {
                    query.group_committe_other = (Request.Params["group_committe_other"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_desc"]))
                {
                    query.group_committe_desc = (Request.Params[@"group_committe_desc"]).Replace("\\", "\\\\");
                }
                DataTable _dt=new DataTable();
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["employee_list"];
                    query.file_name= excelFile.FileName;
                    FileManagement fileManagement = new FileManagement();
                    string newExcelName = Server.MapPath(excelPath) + "vip_user_group" + fileManagement.NewFileName(excelFile.FileName);
                    excelFile.SaveAs(newExcelName);
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    _dt = helper.SheetData();
               }

                _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                json = _vipUserGroup.SaveVipUserGroup(query, _dt);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase SaveComAddress()
        {
            string json = string.Empty;
            DeliveryAddress query = new DeliveryAddress();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["da_id"]))
                {
                    query.da_id = Convert.ToInt32(Request.Params["da_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    query.user_id = Convert.ToInt32(Request.Params["user_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["da_title"]))
                {
                    query.da_title = Request.Params["da_title"];
                }
                if (!string.IsNullOrEmpty(Request.Params["da_dist"]))
                {
                    query.da_dist = Request.Params["da_dist"];
                }
                if (!string.IsNullOrEmpty(Request.Params["da_address"]))
                {
                    query.da_address = Request.Params["da_address"];
                }

                _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                json = _vipUserGroup.SaveComAddress(query);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase GetComAddress()
        {
            List<DeliveryAddress> store = new List<DeliveryAddress>();
            DeliveryAddress query = new DeliveryAddress();
            string  json = "{success:true,data:[]}";
            try
            {
                _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    query.user_id = Convert.ToInt32(Request.Params["user_id"]);
                    store = _vipUserGroup.GetComAddress(query);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";
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

        public HttpResponseBase GetVipUserGList()
        {
            string json = string.Empty;
            try
            {
                VipUserGroupQuery query = new VipUserGroupQuery();
                List<VipUserGroupQuery> store = new List<VipUserGroupQuery>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["tax_name"]))
                {
                    query.tax_name = Request.Params["tax_name"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["relation_id"]);
                }
                if (Request.Params["isSecret"] == "true")
                {
                    query.isSecret = true;
                }
                else
                {
                    query.isSecret = false;
                }
                _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                store = _vipUserGroup.GetVipUserGList(query, out totalCount);
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

        public HttpResponseBase DeleteDeliveryAddress()
        {
            string json = string.Empty;
            DeliveryAddress query = new DeliveryAddress();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["da_id"])) 
                {
                    query.da_id = Convert.ToInt32(Request.Params["da_id"]);
                    _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                    json = _vipUserGroup.DeleteDeliveryAddress(query);
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

        public JsonResult UpVUGStatus()
        {

            {
                try
                {
                    VipUserGroupQuery query = new VipUserGroupQuery();
                    if (!string.IsNullOrEmpty(Request.Params["group_id"].ToString()))
                    {
                        query.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                    }
                    query.group_status = Convert.ToInt32(Request.Params["active"] ?? "0");
                    _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                    query.m_user = (Session["caller"] as Caller).user_id;
                    query.m_date = DateTime.Now;
                    int result=_vipUserGroup.UpVUGStatus(query) ;
                    if (result > 0)
                    {
                        return Json(new { success = "true" });
                    }
                    else if (result == 0)//此企業用戶正在參加活動不可變為禁用
                    {
                        return Json(new { success = "no" });
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
                    return Json(new { success = "no" });
                }

            }
        }

        public void DownLoad()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("員工編號", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add();
                string fileName = "企業用戶管理匯入範本_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        //public HttpResponseBase ExistEmail()
        //{
        //    string json = "{success:true}";
        //    try
        //    {
        //        VipUserGroupQuery query = new VipUserGroupQuery();
        //        if (!string.IsNullOrEmpty(Request.Params["group_committe_mail"]))
        //        {
        //            query.group_committe_mail = Request.Params["group_committe_mail"];
        //            _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
        //            if (_vipUserGroup.ExistEmail(query) > 0)
        //            {
        //                json = "{success:true}";//emai不可用
        //            }
        //            else
        //            {
        //                json = "{success:false}";//email可用
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:true}";//emai不可用
        //    }

        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}

        public HttpResponseBase ExisGroupCode()
        {
            string json = "{success:true}";
            try
            {
                VipUserGroupQuery query = new VipUserGroupQuery();
                if (!string.IsNullOrEmpty(Request.Params["group_code"]))
                {
                    query.group_code = Request.Params["group_code"];
                    _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                    if (_vipUserGroup.ExisGroupCode(query) > 0)
                    {
                        json = "{success:true}";//tax_id不可用
                    }
                    else
                    {
                        json = "{success:false}";//tax_id可用
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true}";//emai不可用
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase ExisTaxId()
        {
            string json = "{success:true}";
            try
            {
                VipUserGroupQuery query = new VipUserGroupQuery();
                if (!string.IsNullOrEmpty(Request.Params["tax_id"]))
                {
                      if(!string.IsNullOrEmpty(Request.Params["group_id"]))
                      {
                          query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                      }
                    query.tax_id = Request.Params["tax_id"];
                    _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                    if (_vipUserGroup.ExisTaxId(query) > 0)
                    {
                        json = "{success:true}";//tax_id不可用
                    }
                    else
                    {
                        json = "{success:false}";//tax_id可用
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true}";//emai不可用
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        

        public HttpResponseBase GetGroupCommitteContact()
        {
            List<GroupCommitteContact> store = new List<GroupCommitteContact>();
            string json = string.Empty;
            GroupCommitteContact query = new GroupCommitteContact();
            _gccMgr = new GroupCommitteContactMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                    store = _gccMgr.GetGroupCommitteContact(query);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";
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
        public HttpResponseBase SaveGCC()
        {
            string json = string.Empty;
            GroupCommitteContact query = new GroupCommitteContact();
            _gccMgr = new GroupCommitteContactMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_chairman"]))
                {
                    query.gcc_chairman = Request.Params["group_committe_chairman"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_phone"]))
                {
                    query.gcc_phone = Request.Params["group_committe_phone"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_committe_mail"]))
                {
                    query.gcc_mail = Request.Params["group_committe_mail"];
                }
                query.k_user = (Session["caller"] as Caller).user_id;
                query.k_date = DateTime.Now;
                query.m_user = (Session["caller"] as Caller).user_id;
                query.m_date = DateTime.Now;
                int i = _gccMgr.SaveGCC(query);
                if (i > 0)
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
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase UpdateGCC()
        {
            string json = string.Empty;
            GroupCommitteContact query = new GroupCommitteContact();
            _gccMgr = new GroupCommitteContactMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["gcc_id"]))
                {
                    query.gcc_id = Convert.ToInt32(Request.Params["gcc_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["gcc_chairman"]))
                {
                    query.gcc_chairman = Request.Params["gcc_chairman"];
                }
                if (!string.IsNullOrEmpty(Request.Params["gcc_phone"]))
                {
                    query.gcc_phone = Request.Params["gcc_phone"];
                }
                if (!string.IsNullOrEmpty(Request.Params["gcc_mail"]))
                {
                    query.gcc_mail = Request.Params["gcc_mail"];
                }
                query.m_user = (Session["caller"] as Caller).user_id;
                query.m_date = DateTime.Now;
                int i = _gccMgr.UpdateGCC(query);
                if (i > 0)
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
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase DeleteGCC()
        {
            string json = string.Empty;
            GroupCommitteContact query = new GroupCommitteContact();
            _gccMgr = new GroupCommitteContactMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["gcc_id"]))
                {
                    query.gcc_id = Convert.ToInt32(Request.Params["gcc_id"]);
                }
                int i = _gccMgr.DeleteGCC(query);
                if (i > 0)
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
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
    }
}
