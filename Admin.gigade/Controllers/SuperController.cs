/*
* 文件名稱 :SuperController.cs
* 文件功能描述 :根據查詢語句，將查詢結果以Excel格式匯出
* 版權宣告 :
* 開發人員 : zhaozhi0623j
* 版本資訊 : 
* 日期 : 2015/09/11
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
  */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using gigadeExcel.Comment;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace Admin.gigade.Controllers
{
    public class SuperController : Controller
    {
        //
        // GET: /Super/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);//日志
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();

        private SuperMgr _superMgr = new SuperMgr(mySqlConnectionString);

        #region 界面視圖
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Super密碼驗證方法
        /// <summary>
        /// 驗證用戶輸入的密碼，返回Json
        /// </summary>
        /// <returns>Super密碼驗證</returns>
        public HttpResponseBase ConfirmSuperPwd()
        {
            string json = string.Empty;
            try
            {
                if (Request.Params["superPwd"] == DateTime.Now.ToString("yyyyMMdd"))
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;
        }
        #endregion

        #region Super Excel匯出方法
        /// <summary>
        /// 根據用戶輸入的查詢語句，匯出查詢結果
        /// </summary>
        /// <returns>Super Excel匯出</returns>
        public void SuperExportExcel()
        {
            SuperQuery query = new SuperQuery();
            int totalCount = 0;
            string json = string.Empty;
            DataTable _dt = new DataTable();

            string excelPath = String.Empty;

            try
            {
                try
                {
                    excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"].ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("找不到路徑" + excelPath + ex.Message);
                }

                //檢查Sql語句是否為查詢語句，不是則拋出錯誤
                //檢查Sql語句是否為空，為空則拋出錯誤
                if (!string.IsNullOrEmpty(Request.Params["superSql"]))
                {
                    query.superSql = Request.Params["superSql"];
                    query.superSql = query.superSql.Replace(",", ", ").Replace("\r", " ").Replace("\n", " ");
                    string[] sqlArray = query.superSql.Split(' ', ',', '(', ')');
                    if (sqlArray[0].ToLower() != "select")
                    {
                        Response.Write("Sql語句不是查詢語句，請輸入查詢語句 ");                      
                    }
                }
                else
                {
                    Response.Write("查詢語句為空！");                   
                }

                //檢查Sql語句是否錯誤
                try
                {
                    _dt = _superMgr.SuperExportExcel(query, out totalCount);
                }
                catch (Exception ex)
                {
                    Response.Write("Sql語句有錯誤" + ex.Message);
                    throw new Exception(ex.Message);                  
                }

                if (totalCount >= 60000)
                {
                    Response.Write("查詢到的數據多於60000條，請重新輸入查詢語句！");
                    
                }
                //返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径。
                //Exists确定给定路径是否引用磁盘上的现有目录。
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))//如果目錄不存在，添加目錄   ,在派生类中重写时，
                {
                    //CreateDirectory 按 path 的指定创建所有目录和子目录。
                    //Directory目錄 
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }


                if (_dt.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("報表_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(_dt, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);//Disposition:处置、部署
                    Response.BinaryWrite(ms.ToArray());//BinaryWrite(byte[] buffer)在派生类中重写时，将二进制字符的字符串写入 HTTP 输出流。
                    //要写入当前响应的二进制字符。 ToArray将流内容写入字节数组，而与 System.IO.MemoryStream.Position 属性无关。
                }
                else
                {
                    Response.Write("匯出數據不存在");
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
        }
        #endregion

    }
}

