using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();

        //
        // GET: /EdmNew/
        #region view
        public ActionResult Index()
        {
            return View();
        }
        //電子報
        public ActionResult EdmContentNew()
        {
            return View();
        }
        #endregion

        #region 電子報類型

        #region 電子報類型列表頁

        #endregion

        #region 電子報類型新增編輯

        #endregion

        #endregion

        #region 電子報範本

        #region  電子報範本列表頁

        #endregion

        #region  電子報範本新增編輯

        #endregion


        #endregion

        #region 電子報

        #region 電子報列表

        #endregion

        #region 電子報新增編輯

        #endregion

        #endregion

        #region 擋信名單管理

        #endregion

    }
}
