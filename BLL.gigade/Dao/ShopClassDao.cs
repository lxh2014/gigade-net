using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    class ShopClassDao : IShopClassImplDao
    {
        private IDBAccess _accessMySql;
        public ShopClassDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<Model.ShopClass> QueryAll(Model.ShopClass query)
        { 
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("select class_id,class_name,class_sort,class_status,class_content,class_createdate,class_updatedate,class_ipfrom from  shop_class ");
            return _accessMySql.getDataTableForObj<ShopClass>(sbSql.ToString());
        }
        public List<ShopClass> QueryStore(int classId = 0)
        {
            StringBuilder s = new StringBuilder();
            try
            {
                s.Append("select class_id,class_name from shop_class where 1=1 ");
                if (classId != 0)
                {
                    s.AppendFormat(" and class_id ='{0}'", classId);
                }
                return _accessMySql.getDataTableForObj<ShopClass>(s.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShopClassDao-->QueryStore-->" + ex.Message + s.ToString(), ex);
            }
        }


    }
}
