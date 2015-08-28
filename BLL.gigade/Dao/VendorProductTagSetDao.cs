using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class VendorProductTagSetDao : IVendorProductTagSetImplDao
    {
        private IDBAccess _dbAccess;
        public VendorProductTagSetDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.VendorProductTagSet> Query(Model.VendorProductTagSet vendorProductTagSet)
        {
            StringBuilder strSql = new StringBuilder("select product_id,tag_id from product_tag_set_temp where 1=1");
            if (!string.IsNullOrEmpty(vendorProductTagSet.product_id))
            {
                strSql.AppendFormat(" and product_id={0}", vendorProductTagSet.product_id);
            }
            return _dbAccess.getDataTableForObj<Model.VendorProductTagSet>(strSql.ToString());
        }

        public string Delete(Model.VendorProductTagSet vendorProductTagSet)
        {
            StringBuilder strSql = new StringBuilder("delete from product_tag_set_temp where ");
            strSql.AppendFormat(" product_id={0};", vendorProductTagSet.product_id);
            return strSql.ToString();
        }

        public string Save(Model.VendorProductTagSet vendorProductTagSet)
        {
            throw new NotImplementedException();
        }

        public string SaveFromOtherPro(Model.VendorProductTagSet vendorProductTagSet)
        {
            throw new NotImplementedException();
        }
    }
}
