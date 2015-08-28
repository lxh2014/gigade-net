using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System.Data;
namespace BLL.gigade.Dao
{
    public class ProductSpecTempDao : IProductSpecTempImplDao
    {
        private IDBAccess _access;
        public ProductSpecTempDao(string conStr)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, conStr);
        }

        public int Save(ProductSpecTemp saveTemp)
        {
            saveTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_spec_temp (`writer_id`,`spec_type`,`spec_name`,`spec_sort`,`spec_status`,`spec_id`,`spec_image`,`product_id`)");
            stb.AppendFormat(" values({0},{1},'{2}',{3},{4},{5},'{6}'", saveTemp.Writer_Id, saveTemp.spec_type, saveTemp.spec_name, saveTemp.spec_sort, saveTemp.spec_status, saveTemp.spec_id, saveTemp.spec_image);

            stb.AppendFormat(",'{0}')", saveTemp.product_id);
            return _access.execCommand(stb.ToString());
        }

        public int Delete(ProductSpecTemp delTemp)
        {
            StringBuilder sql = new StringBuilder("set sql_safe_updates = 0; delete from product_spec_temp where 1=1");
            if (delTemp.Writer_Id != 0)
            {
                sql.AppendFormat(" and writer_id = {0}", delTemp.Writer_Id);
            }
            if (delTemp.spec_id != 0)
            {
                sql.AppendFormat(" and spec_id = {0}", delTemp.spec_id);
            }
            sql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", delTemp.product_id );
            return _access.execCommand(sql.ToString());
        }

        public string Update(ProductSpecTemp pSpec, string updateType)
        {
            pSpec.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0; update product_spec_temp set ");
            strSql.AppendFormat(" spec_status={0} ", pSpec.spec_status);
            if (updateType == "image")
            {
                strSql.AppendFormat(",spec_image='{0}'", pSpec.spec_image);
            }
            if (!string.IsNullOrEmpty(pSpec.spec_name))
            {
                strSql.AppendFormat(" ,spec_name='{0}'", pSpec.spec_name);
            }
            if (pSpec.spec_sort != 0)
            {
                strSql.AppendFormat(" ,spec_sort={0}", pSpec.spec_sort);
            }
            strSql.AppendFormat(" where spec_id='{0}';set sql_safe_updates = 1;", pSpec.spec_id);
            return strSql.ToString();
        }

        public List<ProductSpecTemp> Query(ProductSpecTemp queryTemp)
        {
            StringBuilder sql = new StringBuilder("select spec_id,spec_type,spec_name,spec_image,spec_sort,spec_status from product_spec_temp where 1=1");
            if (queryTemp.Writer_Id != 0)
            {
                sql.AppendFormat(" and writer_id = {0}", queryTemp.Writer_Id);
            }
            if (queryTemp.spec_id != 0)
            {
                sql.AppendFormat(" and spec_id = {0}", queryTemp.spec_id);
            }
            if (queryTemp.spec_type != 0)
            {
                sql.AppendFormat(" and spec_type = {0}", queryTemp.spec_type);
            }
            sql.AppendFormat(" and product_id='{0}'", queryTemp.product_id );
            return _access.getDataTableForObj<ProductSpecTemp>(sql.ToString());
        }

        public string TempMoveSpec(ProductSpecTemp proSpecTemp)
        {
            StringBuilder stb = new StringBuilder("insert into product_spec(spec_id,product_id,spec_type,spec_name,spec_sort,spec_status,spec_image) select spec_id,{0} as product_id,");
            stb.AppendFormat("spec_type,spec_name,spec_sort,spec_status,spec_image from product_spec_temp where writer_id = {0}", proSpecTemp.Writer_Id);
            stb.AppendFormat(" and product_id='{0}'", proSpecTemp.product_id );
            return stb.ToString();
        }

        public string TempDelete(ProductSpecTemp proSpecTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_spec_temp ");
            strSql.AppendFormat("where writer_id = {0}", proSpecTemp.Writer_Id);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proSpecTemp.product_id );
            return strSql.ToString();
        }

        public string SaveFromSpec(ProductSpecTemp proSpecTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_spec_temp(writer_id,spec_id,product_id,spec_type,spec_name,spec_image,spec_sort,");
            strSql.AppendFormat("spec_status) select {0} as writer_id,spec_id,", proSpecTemp.Writer_Id);
            strSql.AppendFormat("product_id,spec_type,spec_name,spec_image,spec_sort,spec_status from product_spec where product_id='{0}'", proSpecTemp.product_id);
            strSql.AppendFormat(" and spec_id={0}", proSpecTemp.spec_id);
            return strSql.ToString();
        }

        public string UpdateCopySpecId(ProductSpecTemp proSpecTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_spec_temp set spec_id={0} ");
            strSql.AppendFormat("where writer_id={0} and product_id={1}", proSpecTemp.Writer_Id, proSpecTemp.product_id );
            strSql.AppendFormat(" and spec_id={0};set sql_safe_updates = 1;", proSpecTemp.spec_id);
            return strSql.ToString();
        }
        #region 供應商商品處理
        public int DeleteByVendor(ProductSpecTemp delTemp)
        {
            string sql = TempDeleteByVendor(delTemp);
            return _access.execCommand(sql.ToString());
        }

        public string TempDeleteByVendor(ProductSpecTemp proSpecTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_spec_temp where 1=1 ");
            if (proSpecTemp.Writer_Id != 0)
            {
                strSql.AppendFormat("and writer_id = {0}", proSpecTemp.Writer_Id);
            }

            if (proSpecTemp.spec_id != 0)
            {
                strSql.AppendFormat(" and spec_id = {0}", proSpecTemp.spec_id);
            }
            if (!string.IsNullOrEmpty(proSpecTemp.product_id))
            {
                strSql.AppendFormat("  and product_id ='{0}';set sql_safe_updates = 1;", proSpecTemp.product_id);
            }

            return strSql.ToString();
        }
        public int SaveByVendor(ProductSpecTemp saveTemp)
        {
            saveTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_spec_temp (`writer_id`,`spec_type`,`spec_name`,`spec_sort`,`spec_status`,`spec_id`,`spec_image`,`product_id`)");
            stb.AppendFormat(" values({0},{1},'{2}',{3},{4},{5},'{6}'", saveTemp.Writer_Id, saveTemp.spec_type, saveTemp.spec_name, saveTemp.spec_sort, saveTemp.spec_status, saveTemp.spec_id, saveTemp.spec_image);

            stb.AppendFormat(",'{0}')", saveTemp.product_id);
            return _access.execCommand(stb.ToString());
        }

        public string VendorSaveFromSpec(ProductSpecTemp proSpecTemp, string old_product_id)
        {   //20140909 供應商 複製 說明：查出最新的spec_id插入數據更新serial表中的spec_id
            //StringBuilder strspec = new StringBuilder("SELECT serial_value FROM serial where serial_id=77;");
            //int spec_id = Int32.Parse(_access.getDataTable(strspec.ToString()).Rows[0][0].ToString()) + 1;

            StringBuilder strSql = new StringBuilder("insert into product_spec_temp(writer_id,spec_id,product_id,spec_type,spec_name,spec_image,spec_sort,");
            strSql.AppendFormat("spec_status) select {0} as writer_id,", proSpecTemp.Writer_Id);
            strSql.Append(" {0} as spec_id, ");
            strSql.AppendFormat("  '{0}' as product_id,spec_type,spec_name,spec_image,spec_sort,spec_status  ", proSpecTemp.product_id);

            uint productid = 0;
            if (uint.TryParse(old_product_id, out productid))
            {
                strSql.AppendFormat(" from product_spec where product_id={0}", productid);
            }
            else
            {
                strSql.AppendFormat("from product_spec_temp where product_id='{0}'", old_product_id);
            }
            strSql.AppendFormat(" and spec_id={0} ;", proSpecTemp.spec_id);
            return strSql.ToString();
        }
   

        public string VendorTempMoveSpec(ProductSpecTemp proSpecTemp)
        {   //20140916 add jialei 核可商品用得到
            StringBuilder stb = new StringBuilder("insert into product_spec(spec_id,product_id,spec_type,spec_name,spec_sort,spec_status,spec_image) ");
            stb.Append("select spec_id,{0} as product_id,spec_type,spec_name,spec_sort,spec_status,spec_image from product_spec_temp where 1=1");
            if (proSpecTemp.Writer_Id != 0)
            {
                stb.AppendFormat(" and writer_id = {0}", proSpecTemp.Writer_Id);
            }
            stb.AppendFormat(" and product_id='{0}';", proSpecTemp.product_id);
            return stb.ToString();
        }
        public string VendorTempDelete(ProductSpecTemp proSpecTemp)
        {//20140916 add jialei 核可商品用得到
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_spec_temp where 1=1");
            if (proSpecTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id = {0}", proSpecTemp.Writer_Id);
            }
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proSpecTemp.product_id);
            return strSql.ToString();
        }
        public List<ProductSpecTemp> VendorQuery(ProductSpecTemp queryTemp)
        {
            StringBuilder sql = new StringBuilder("select spec_id,spec_type,spec_name,spec_image,spec_sort,spec_status ");
            uint pid = 0;
            if (uint.TryParse(queryTemp.product_id, out pid))
            {
                sql.AppendFormat("  from product_spec where  product_id='{0}'", queryTemp.product_id);
            }
            else
            {
                sql.AppendFormat("  from product_spec_temp where product_id='{0}'", queryTemp.product_id);
            }
            if (queryTemp.Writer_Id != 0)
            {
                sql.AppendFormat(" and writer_id = {0}", queryTemp.Writer_Id);
            }
            if (queryTemp.spec_id != 0)
            {
                sql.AppendFormat(" and spec_id = {0}", queryTemp.spec_id);
            }
            if (queryTemp.spec_type != 0)
            {
                sql.AppendFormat(" and spec_type = {0}", queryTemp.spec_type);
            }
            return _access.getDataTableForObj<ProductSpecTemp>(sql.ToString());
        }
        #endregion

    }
}
