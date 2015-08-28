using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Dao
{
    public class ProductStatusHistoryDao : BLL.gigade.Dao.Impl.IProductStatusHistoryImplDao
    {
        IDBAccess _access;
        public ProductStatusHistoryDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public string Save(BLL.gigade.Model.ProductStatusHistory save)
        {
            try
            {
                save.Replace4MySQL();
                StringBuilder stb = new StringBuilder("set sql_safe_updates = 0; insert into product_status_history (`product_id`,`user_id`,`create_time`,`type`,`product_status`,`remark`)");
                stb.AppendFormat(" values ({0},{1},now(),{2},{3},'{4}');set sql_safe_updates = 1;", save.product_id, save.user_id, save.type, save.product_status, save.remark);
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryDao.SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string SaveNoProductId(BLL.gigade.Model.ProductStatusHistory save)
        {
            try
            {
                save.Replace4MySQL();
                StringBuilder stb = new StringBuilder("set sql_safe_updates = 0; insert into product_status_history (`product_id`,`user_id`,`create_time`,`type`,`product_status`,`remark`) values ({0}");
                stb.AppendFormat(",{0},now(),{1},{2},'{3}');set sql_safe_updates = 1;", save.user_id, save.type, save.product_status, save.remark);
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryDao.SaveNoProductId-->" + ex.Message, ex);
            }
        }

        public string Delete(BLL.gigade.Model.ProductStatusHistory history)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_status_history ");
                strSql.AppendFormat("where product_id={0};set sql_safe_updates = 1;", history.product_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryDao.Delete-->" + ex.Message, ex);
            }
        }

        public List<ProductStatusHistoryCustom> HistoryQuery(ProductStatusHistoryCustom query)
        {
            StringBuilder sql = new StringBuilder();
            if (query.create_channel == 2)//當product.create_channel為2時，鏈接vendor表 xiangwang0413w 2014/09/10
            {
                sql.AppendFormat(@"select h.product_id,h.create_time,h.remark,u.vendor_name_full as user_username,s1.parametername as type ,
                s2.parametername as product_status from product_status_history h 
                left join vendor u on h.user_id = u.vendor_id 
                left join (select parametercode,parametername from t_parametersrc where parametertype='verify_operate_type') s1 
                on  s1.parametercode = h.type 
                left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') s2 
                on s2.parametercode = h.product_status 
                inner join product p on h.product_id = p.product_id  
                where h.product_id = {0} order by h.create_time", query.product_id);
            }
            else
            {
                
                sql.Append("select h.product_id,h.create_time,h.remark,u.user_username,s1.parametername as type ,s2.parametername as product_status");
                sql.Append(" from product_status_history h");
                sql.Append(" left join manage_user u on h.user_id = u.user_id");
                sql.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='verify_operate_type') s1 on  s1.parametercode = h.type");
                sql.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') s2 on s2.parametercode = h.product_status");
                sql.Append(" inner join product p on h.product_id = p.product_id ");
                sql.AppendFormat(" where h.product_id = {0} order by h.create_time", query.product_id);
            }
            return _access.getDataTableForObj<ProductStatusHistoryCustom>(sql.ToString());

        }

        public int UpdateColumn(BLL.gigade.Model.ProductStatusHistory save)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                save.Replace4MySQL();
                sb.AppendFormat(@"SET SQL_SAFE_UPDATES = 0;UPDATE `product_status_history` SET `remark` = '{0}' WHERE product_id = {1};SET SQL_SAFE_UPDATES = 1;",save.remark,save.product_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryDao-->UpdateColumn" + ex.Message,ex);
            }
        }

    }
}
