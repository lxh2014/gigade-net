using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ProductStockImportDao : IProductStockImportImplDao
    {
        IDBAccess _access;
        public ProductStockImportDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        //public DataTable GetType(string item_id)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.AppendLine(@"SELECT pi.item_id,ps.spec_name as  spec_name1,pss.spec_name as  spec_name2,p.product_mode,p.prepaid ");
        //    sql.AppendLine(@"from product_item pi LEFT JOIN product_spec ps ON ps.spec_id=pi.spec_id_1 LEFT JOIN product_spec pss ON  pss.spec_id=pi.spec_id_2");
        //    sql.AppendFormat(@"LEFT JOIN product p on  pi.product_id=p.product_id where pi.item_id='{0}'",item_id);
        //    try 
        //    {
        //        return _access.getDataTable(sql.ToString());
        //    }
        //     catch (Exception ex)
        //    {
        //        throw new Exception("ProductStockImportDao.GetType-->" + ex.Message + sql.ToString(), ex);
        //    }
        //}
        public int UpdateStock(ProductItem pi)
        {
            string sql = string.Empty;
            if (pi.Item_Id != 0)
            {
                sql = string.Format("update product_item set item_alarm='{0}',remark='{1}' ", pi.Item_Alarm,pi.Remark);
                //判斷是否更新庫存值
                if (pi.Item_Stock!=999999999)
                {
                    sql += string.Format(",item_stock='{0}' ", pi.Item_Stock);
                }
                sql += string.Format(" where item_id='{0}'", pi.Item_Id);
                try
                {
                    return _access.execCommand(sql);
                }
                catch (Exception ex)
                {
                    throw new Exception("ProductStockImportDao.UpdateStock-->" + ex.Message + sql, ex);
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
