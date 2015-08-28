using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ShoppingSetupCartDao
    {
        private IDBAccess _dbAccess;
        public ShoppingSetupCartDao(string connectionString)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable QueryShopCart()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select cart_id,cart_name from shopping_setup_cart  ");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShoppingSetupCartDao-->QueryShopCart-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
