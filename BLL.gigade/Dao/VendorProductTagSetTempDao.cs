using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    class VendorProductTagSetTempDao : IVendorProductTagSetTempImplDao
    {
        private IDBAccess _dbAccess;
        public VendorProductTagSetTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<ProductTagSetTemp> Query(ProductTagSetTemp vendorProductTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("select writer_id,tag_id from product_tag_set_temp where 1=1");
            if (vendorProductTagSetTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", vendorProductTagSetTemp.Writer_Id);
            }
            if (vendorProductTagSetTemp.Combo_Type != 0)
            {
                strSql.AppendFormat(" and combo_type={0}", vendorProductTagSetTemp.Combo_Type);
            }
            strSql.AppendFormat(" and product_id{0}0", (!string.IsNullOrEmpty(vendorProductTagSetTemp.product_id))? "<>" : "=");
            return _dbAccess.getDataTableForObj<ProductTagSetTemp>(strSql.ToString());
        }

        public string Delete(ProductTagSetTemp vendorProductTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_tag_set_temp where ");
            strSql.AppendFormat(" writer_id={0} and combo_type = {1}", vendorProductTagSetTemp.Writer_Id, vendorProductTagSetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id{0}0;set sql_safe_updates=1;", (!string.IsNullOrEmpty(vendorProductTagSetTemp.product_id)) ? "<>" : "=");
            return strSql.ToString();
        }

        public string DeleteVendor(ProductTagSetTemp vendorProductTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_tag_set_temp where ");
            strSql.AppendFormat(" writer_id={0} and combo_type = {1}", vendorProductTagSetTemp.Writer_Id, vendorProductTagSetTemp.Combo_Type);
            if (!string.IsNullOrEmpty(vendorProductTagSetTemp.product_id))
            {
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", vendorProductTagSetTemp.product_id);

            }
            else
            {
                strSql.AppendFormat(" and product_id{0}0;set sql_safe_updates=1;", (!string.IsNullOrEmpty(vendorProductTagSetTemp.product_id)) ? "<>" : "=");
            }
            return strSql.ToString();
        }

        public string Save(ProductTagSetTemp vendorProductTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set_temp(`writer_id`,`product_id`,`tag_id`,`combo_type`)");
            strSql.AppendFormat("values({0},{1},{2},{3});", vendorProductTagSetTemp.Writer_Id, vendorProductTagSetTemp.product_id, vendorProductTagSetTemp.tag_id, vendorProductTagSetTemp.Combo_Type);
            return strSql.ToString();
        }

        public string MoveTag(ProductTagSetTemp vendorProductTagSetTemp)
        {
            throw new NotImplementedException();
        }

        public string SaveFromTag(ProductTagSetTemp vendorProductTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set_temp(writer_id,product_id,tag_id,combo_type) select ");
            strSql.AppendFormat("{0} as writer_id,product_id,tag_id,{1} as combo_type", vendorProductTagSetTemp.Writer_Id, vendorProductTagSetTemp.Combo_Type);
            strSql.AppendFormat(" from product_tag_set where product_id={0}", vendorProductTagSetTemp.product_id);
            return strSql.ToString();
        }

    }
}
