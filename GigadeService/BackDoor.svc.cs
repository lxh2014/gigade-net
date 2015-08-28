using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System.Configuration;

namespace GigadeService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“BackDoor”。
    public class BackDoor : IBackDoor
    {
        private IProductItemImplDao proItem_Dao;
        private IProductImplDao pro_Dao;
        private string connString = ConfigurationManager.AppSettings["connString"];
        public string UpdateERP_ID()
        {
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
            } catch (Exception ex) {
                return string.Format("error:{0}", ex.Message);
            }
            return string.Format("total:{0},updated:{1}",total_count,resut);
        }
    }
}
