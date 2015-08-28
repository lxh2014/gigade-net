using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
namespace BLL.gigade.Dao
{
    public class ProductStatusApplyDao : BLL.gigade.Dao.Impl.IProductStatusApplyImplDao
    {
        IDBAccess _access;
        public ProductStatusApplyDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public ProductStatusApply Query(ProductStatusApply query)
        {
            StringBuilder stb = new StringBuilder("select apply_id, prev_status,online_mode from product_status_apply where 1=1");
            if (query.product_id != 0)
            {
                stb.AppendFormat(" and product_id = {0}", query.product_id);
            }
            return _access.getSinggleObj<ProductStatusApply>(stb.ToString());
        }

        public string Save(ProductStatusApply apply)
        {
            StringBuilder strSql = new StringBuilder("insert into product_status_apply(`product_id`,`prev_status`,`apply_time`,`online_mode`)values(");
            strSql.AppendFormat("{0},{1},now(),{2})", apply.product_id, apply.prev_status, apply.online_mode);
            return strSql.ToString();
        }

        public string Delete(ProductStatusApply apply)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("set sql_safe_updates = 0; delete from product_status_apply where apply_id = {0};set sql_safe_updates = 1;", apply.apply_id);
            return strSql.ToString();
        }
    }
}
