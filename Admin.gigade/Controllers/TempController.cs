using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Temp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class TempController : Controller
    {
        //
        // GET: /Temp/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OutToExcel()
        {
            string xmlPath = "../XML/SaleSum.xml";
            IProductOrderTempImplMgr _potMgr = new ProductOrderTempMgr(connectionString);
            ProductOrderTemp pot =new ProductOrderTemp();
            pot.brand_id="29, 98,41, 6, 53, 90";
            pot.create_time = Convert.ToDateTime("2014/10/1 00:00:01");
            MemoryStream ms = _potMgr.OutToExcel(Server.MapPath(xmlPath), pot);//根據查詢條件和xml路徑得到excel文件流
            if(ms==null)
            {
                return new EmptyResult();
            }
            return File(ms.ToArray(), "application/-excel", DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
        }
    }
}
