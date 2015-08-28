using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ShoppingSetupCartMgr
    {
        private ShoppingSetupCartDao _sscDao;

        public ShoppingSetupCartMgr(string connectionString)
        {
            _sscDao = new ShoppingSetupCartDao(connectionString);
        }

        public string QueryShopCart()
        {

            try
            {
                DataTable _dt = _sscDao.QueryShopCart();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,data:["));
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in _dt.Rows)
                    {
                        stb.Append("{");
                        stb.Append(string.Format("\"cart_id\":\"{0}\",\"cart_name\":\"{1}\"", item["cart_id"].ToString(), item["cart_name"].ToString()));
                        stb.Append("}");
                    }
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ShoppingSetupCartMgr-->QueryShopCart-->" + ex.Message, ex);
            }
        }
    }
}
