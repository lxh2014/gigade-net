using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.IO;
namespace Admin.gigade.Ashx
{
    /// <summary>
    /// ProductItemMapExcel 的摘要说明
    /// </summary>
    public class ProductItemMapExcel : IHttpHandler
    {
        private IProductItemMapExcelImplMgr _excelMgr;
        private readonly string connectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public void ProcessRequest(HttpContext context)
        {
            _excelMgr = new ProductItemMapExcelMgr(connectionString);
            Resource.CoreMessage = new CoreResource("ProductItemMap");
            MemoryStream ms = _excelMgr.CreateExcelTable();
            context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddhhmmss")));
            context.Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}