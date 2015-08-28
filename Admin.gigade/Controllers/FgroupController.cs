/*
* 文件名稱 :FgroupController.cs
* 文件功能描述 :群組管理
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using Admin.gigade.CustomError;
using System.IO;
using System.Data;
using gigadeExcel.Comment;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class FgroupController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private FgroupMgr fgMgr;
        private GroupCallerMgr gcMgr;
        private IParametersrcImplMgr paraMgr;
        private IVendorImplMgr venMgr;
        private string thispname = string.Empty;
        private string thiscname = string.Empty;
        Fgroup fg = new Fgroup();
        groupCaller gc = new groupCaller();
        //
        // GET: /Fgroup/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fgroup()
        {
            return View();
        }

        [CustomHandleError]
        public string QueryAll()
        {

            fgMgr = new FgroupMgr(mySqlConnectionString);

            string json = string.Empty;

            try
            {
                json = fgMgr.QueryAll();
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

        [CustomHandleError]
        public string QueryCallid()
        {

            fgMgr = new FgroupMgr(mySqlConnectionString);

            string json = string.Empty;

            try
            {
                json = fgMgr.QueryCallid();
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

        [CustomHandleError]
        public string QueryCallidById()
        {
            gcMgr = new GroupCallerMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["groupId"]))
                {
                    gc.groupId = Convert.ToInt32(Request.Form["groupId"]);
                }
                json = gcMgr.QueryCallidById(gc);
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

        [CustomHandleError]
        public HttpResponseBase Add()
        {
            fgMgr = new FgroupMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                fg.rowid = 0;
                fg.groupName = Request.Form["groupName"];
                fg.groupCode = Request.Form["groupCode"];
                fg.remark = Request.Form["remark"];
                fg.kuser = "";

                int num = fgMgr.Save(fg);

                if (num == -1)
                {
                    json = "{success:true,msg:\"群組名稱 或 群組編號已存在。\"}";
                }
                else if (num == 1)
                {
                    json = "{success:true,msg:\"新增成功\"}";
                }
                else
                {
                    json = "{success:true,msg:\"新增失败\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"新增失败\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [CustomHandleError]
        /// <summary>
        /// 人員管理
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AddCallid()//modify by mengjuan0826j 2015/6/8 處理供應商pm設定 增加條件設置：若pm存在于供應商，則不能執行刪除動作！
        {
            gcMgr = new GroupCallerMgr(mySqlConnectionString);
            venMgr = new VendorMgr(mySqlConnectionString);
            string json = string.Empty;
            bool delete = true;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["groupId"]))
                {
                    gc.groupId = Int32.Parse(Request.Form["groupId"]);
                }

                if (!string.IsNullOrEmpty(Request.Form["callid"]))
                {
                    string[] callid = Request.Form["callid"].IndexOf(",") != -1 ? Request.Form["callid"].Split(',') : new string[] { Request.Form["callid"] };

                    if (gc.groupId == GetVebdorPMGroup())//判定是否為供應商群組管理員
                    {
                        List<ManageUser> muStore = venMgr.GetVendorPM();//獲取供應商pm
                        if (muStore.Count != 0)
                        {
                            foreach (var item in muStore)//遍歷供應商pm，若該人員存在于供應商pm，則不能刪除，直接提示！
                            {
                                if (!callid.Contains(item.user_email))
                                {
                                    delete = false;
                                    json = "{success:false,msg:\"人員 " + item.user_username + " 為現有供應商PM，不能移除\"}";
                                    break;
                                }
                            }
                        }
                    }
                    if (delete)
                    {
                        gcMgr.Delete(gc);
                        foreach (string id in callid)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                gc.callid = id;
                                gcMgr.Save(gc);
                            }
                        }
                        json = "{success:true,msg:\"新增成功\"}";
                    }
                }
                else
                {
                    json = "{success:true,msg:\"新增失敗\"}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"新增失败\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [CustomHandleError]
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Edit()
        {
            fgMgr = new FgroupMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowid"]))
                {
                    fg.rowid = Convert.ToInt32(Request.Form["rowid"]);
                }

                if (!string.IsNullOrEmpty(Request.Form["groupName"]))
                {
                    fg.groupName = Request.Form["groupName"];
                }

                if (!string.IsNullOrEmpty(Request.Form["groupCode"]))
                {
                    fg.groupCode = Request.Form["groupCode"];
                }

                if (!string.IsNullOrEmpty(Request.Form["remark"]))
                {
                    fg.remark = Request.Form["remark"];
                }

                fg.kuser = "";
                int num = fgMgr.Save(fg);

                if (num == -1)
                {
                    json = "{success:true,msg:\"群組名稱 或 群組編號已存在。\"}";
                }
                else if (num == 2)
                {
                    json = "{success:true,msg:\"修改成功\"}";
                }
                else
                {
                    json = "{success:true,msg:\"修改失败\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"修改失败\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [CustomHandleError]
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Delete()
        {
            fgMgr = new FgroupMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("|") != -1)
                    {
                        foreach (string id in rowIDs.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                fg.rowid = Int32.Parse(id);
                                fgMgr.Delete(fg);
                            }
                        }
                        json = "{success:true,msg:\"删除成功\"}";
                    }
                    else
                    {
                        json = "{success:true,msg:\"删除失败\"}";
                    }
                }
                else
                {
                    json = "{success:true,msg:\"删除失败\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"删除失败\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();

            return this.Response;
        }


        public void ExportGroupLimit()
        {
            try
            {
                fgMgr = new FgroupMgr(mySqlConnectionString);
                List<string> NameList = new List<string>();
                List<DataTable> Elist = new List<DataTable>();
                List<bool> comName = new List<bool>();
                DataTable _dt = fgMgr.GetFgroupList();
                for (int i = 0; i < _dt.Rows.Count; i++)//循環各個群組
                {
                    DataTable _dtone = new DataTable();
                    _dtone.Columns.Add("群組人員", typeof(String));
                    _dtone.Columns.Add("功能模塊", typeof(String));
                    _dtone.Columns.Add("基本功能", typeof(String));
                    _dtone.Columns.Add("功能操作", typeof(String));
                    #region 獲取信息
                    //獲取某個群組下的所有用戶
                    DataTable _dtUsers = fgMgr.GetUsersByGroupId(Convert.ToInt32(_dt.Rows[i]["rowid"]));
                    //獲取某個群組下的所有權限
                    DataTable _dtAuthorty = fgMgr.GetAuthorityByGroupId(Convert.ToInt32(_dt.Rows[i]["rowid"]));
                    int usercount = _dtUsers.Rows.Count;
                    int authorty = _dtAuthorty.Rows.Count;
                    if (authorty >= usercount)//如果權限的行數大於群組中的個數
                    {
                        for (int z = 0; z < _dtAuthorty.Rows.Count; z++)
                        {
                            DataRow _tbRow = _dtone.NewRow();
                            try
                            {
                                _tbRow[0] = _dtUsers.Rows[z]["user_username"].ToString().Trim();
                            }
                            catch (Exception ex)
                            {
                                _tbRow[0] = "";
                            }
                            if (z == 0)
                            {
                                thispname = _dtAuthorty.Rows[0]["pname"].ToString();
                                thiscname = _dtAuthorty.Rows[0]["cname"].ToString();
                                //_tbRow[1] = _dtAuthorty.Rows[z]["pname"] + " " + (_dtAuthorty.Rows[z]["tpname"].ToString() == "" ? "X" : "O");
                                _tbRow[1] = _dtAuthorty.Rows[z]["pname"];
                                _tbRow[2] = _dtAuthorty.Rows[z]["cname"] + " " + (_dtAuthorty.Rows[z]["tcname"].ToString() == "" ? "X" : "O");
                            }
                            else
                            {
                                if (thispname == _dtAuthorty.Rows[z]["pname"].ToString())//如果相同
                                {
                                    _tbRow[1] = "";
                                }
                                else
                                {
                                    //_tbRow[1] = _dtAuthorty.Rows[z]["pname"] + " " + (_dtAuthorty.Rows[z]["tpname"].ToString() == "" ? "X" : "O");
                                    _tbRow[1] = _dtAuthorty.Rows[z]["pname"];
                                    thispname = _dtAuthorty.Rows[z]["pname"].ToString();
                                }
                                if (thiscname == _dtAuthorty.Rows[z]["cname"].ToString())
                                {
                                    _tbRow[2] = "";
                                }
                                else
                                {
                                    _tbRow[2] = _dtAuthorty.Rows[z]["cname"] + " " + (_dtAuthorty.Rows[z]["tcname"].ToString() == "" ? "X" : "O");
                                    thiscname = _dtAuthorty.Rows[z]["cname"].ToString();
                                }
                            }
                            if (_dtAuthorty.Rows[z]["tname"].ToString() == "")
                            {
                                _tbRow[3] = "";
                            }
                            else
                            {
                                if (_dtAuthorty.Rows[z]["ttname"].ToString() == "" || _dtAuthorty.Rows[z]["functionId"].ToString() == "")
                                {
                                    _tbRow[3] = _dtAuthorty.Rows[z]["tname"] + " " + "X";
                                }
                                else
                                {
                                    _tbRow[3] = _dtAuthorty.Rows[z]["tname"] + " " + "O";
                                }
                            }
                            _dtone.Rows.Add(_tbRow);

                        }
                    }
                    else
                    {
                        for (int j = 0; j < _dtUsers.Rows.Count; j++)
                        {
                            DataRow _tbRow = _dtone.NewRow();
                            _tbRow[0] = _dtUsers.Rows[j]["user_username"].ToString().Trim();
                            if (j == 0)
                            {
                                thispname = _dtAuthorty.Rows[0]["pname"].ToString();
                                thiscname = _dtAuthorty.Rows[0]["cname"].ToString();
                                //_tbRow[1] = _dtAuthorty.Rows[0]["pname"] + " " + (_dtAuthorty.Rows[j]["tpname"].ToString() == "" ? "X" : "O");
                                _tbRow[1] = _dtAuthorty.Rows[0]["pname"];
                                _tbRow[2] = _dtAuthorty.Rows[0]["cname"] + " " + (_dtAuthorty.Rows[j]["tcname"].ToString() == "" ? "X" : "O");
                                if (_dtAuthorty.Rows[0]["tname"].ToString() == "")
                                {
                                    _tbRow[3] = "";
                                }
                                else
                                {
                                    if (_dtAuthorty.Rows[0]["ttname"].ToString() == "" || _dtAuthorty.Rows[0]["functionId"].ToString() == "")
                                    {
                                        _tbRow[3] = _dtAuthorty.Rows[0]["tname"] + " " + "X";
                                    }
                                    else
                                    {
                                        _tbRow[3] = _dtAuthorty.Rows[0]["tname"] + " " + "O";
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (thispname == _dtAuthorty.Rows[j]["pname"].ToString())
                                    {
                                        _tbRow[1] = "";
                                    }
                                    else
                                    {
                                        // _tbRow[1] = _dtAuthorty.Rows[j]["pname"] + " " + (_dtAuthorty.Rows[j]["tpname"].ToString() == "" ? "X" : "O");
                                        _tbRow[1] = _dtAuthorty.Rows[j]["pname"];
                                        thispname = _dtAuthorty.Rows[j]["pname"].ToString();
                                    }
                                    if (thiscname == _dtAuthorty.Rows[j]["cname"].ToString())
                                    {
                                        _tbRow[2] = "";
                                    }
                                    else
                                    {
                                        _tbRow[2] = _dtAuthorty.Rows[j]["cname"] + " " + (_dtAuthorty.Rows[j]["tcname"].ToString() == "" ? "X" : "O");
                                        thiscname = _dtAuthorty.Rows[j]["cname"].ToString();
                                    }
                                    if (_dtAuthorty.Rows[j]["tname"].ToString() == "")
                                    {
                                        _tbRow[3] = "";
                                    }
                                    else
                                    {
                                        if (_dtAuthorty.Rows[j]["ttname"].ToString() == "" || _dtAuthorty.Rows[j]["functionId"].ToString() == "")
                                        {
                                            _tbRow[3] = _dtAuthorty.Rows[j]["tname"] + " " + "X";
                                        }
                                        else
                                        {
                                            _tbRow[3] = _dtAuthorty.Rows[j]["tname"] + " " + "O";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _tbRow[1] = "";
                                    _tbRow[2] = "";
                                    _tbRow[3] = "";
                                }
                            }
                            _dtone.Rows.Add(_tbRow);
                        }
                    }
                    #endregion
                    comName.Add(true);
                    Elist.Add(_dtone);
                    NameList.Add(_dt.Rows[i]["groupName"].ToString() + "群組");
                    thispname = "";
                    thiscname = "";
                }

                string fileName = "群組權限信息" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDTNoColumnsBySdy(Elist, NameList, comName);
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
        public int GetVebdorPMGroup()//add by mengjuan0826j 2015/6/8 處理供應商pm設定
        {
            paraMgr = new ParameterMgr(mySqlConnectionString);
            fgMgr = new FgroupMgr(mySqlConnectionString);
            List<Parametersrc> parstore = paraMgr.QueryUsed(new Parametersrc { ParameterType = "vendor_pm" }).ToList();//獲取vendor_pm 群組的設定  群組名稱和群組編碼
            int group_id = 0;
            if (parstore.Count != 0)
            {
                fg.groupCode = parstore[0].ParameterCode;
                fg.groupName = parstore[0].parameterName;
                Fgroup pmfg = fgMgr.GetSingle(fg);//根據群組名稱找到group_id
                if (pmfg != null)
                {
                    group_id = pmfg.rowid;
                }
            }
            return group_id;
        }
    }
}
