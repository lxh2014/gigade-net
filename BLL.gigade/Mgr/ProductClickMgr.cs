using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ProductClickMgr
    {
        private ProductClickDao pcDao; 
        public ProductClickMgr(string connectionString)
        {
            pcDao = new ProductClickDao(connectionString);
        }
        public DataTable GetProductClickList(ProductClickQuery query, out int totalCount)
        {
            try
            {
                return pcDao.GetProductClickList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductClickMgr-->GetProductClickList" + ex.Message, ex);
            }
        }
    }
}
