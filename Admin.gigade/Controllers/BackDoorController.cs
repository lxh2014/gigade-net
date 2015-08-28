using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;


namespace Admin.gigade.Controllers
{
    public class BackDoorController : Controller
    {
        //
        // GET: /BackDoor/
        private IProductItemImplDao proItem_Dao;
        private IProductImplDao pro_Dao;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ERP_ID()
        {
            return View();
        }

        public string UpdateERP_ID() {
            string connString = Request.Params["connString"];
            int resut = 0;
            int total_count = 0;
            try
            {
                proItem_Dao = new ProductItemDao(connString);
                pro_Dao = new ProductDao(connString);

                List<Product> proList = pro_Dao.Query(new Product());
                total_count = proList.Count;
                foreach (Product p in proList)
                {
                    try
                    {
                        proItem_Dao.UpdateErpId(p.Product_Id.ToString());
                        resut++;
                    } catch
                    {
                    }
                }
            } catch (Exception ex)
            {
                return string.Format("error:{0}", ex.Message);
            }
            return string.Format("total:{0},updated:{1}", total_count, resut);
        }

    }
}
