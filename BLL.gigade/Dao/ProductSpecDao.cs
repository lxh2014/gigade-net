using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Dao
{
    class ProductSpecDao : IProductSpecImplDao
    {
        private IDBAccess _dbAccess;
        private string tempStr = "";
        public ProductSpecDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public int Save(ProductSpec saveTemp) 
        {
            saveTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_spec (`spec_type`,`spec_name`,`spec_sort`,`spec_status`,`spec_id`,`spec_image`,`product_id`)");
            stb.AppendFormat(" values({0},'{1}',{2},{3},{4},'{5}',{6})", saveTemp.spec_type, saveTemp.spec_name, saveTemp.spec_sort, saveTemp.spec_status, saveTemp.spec_id, saveTemp.spec_image, saveTemp.product_id);
            return _dbAccess.execCommand(stb.ToString());
        }

        public List<ProductSpec> query(int product_id, string type)
        {
            tempStr = string.Format("select distinct s.spec_id,s.spec_name from  product_item i  left join product_spec s on s.spec_id = i.{1} where i.product_id={0} and spec_status=1", product_id, type);
            return _dbAccess.getDataTableForObj<ProductSpec>(tempStr);
        }

        public ProductSpec query(int spec_id)
        {
            tempStr = string.Format("select spec_type,spec_name,spec_sort,spec_status,spec_id,spec_image,product_id from product_spec where spec_id = {0}", spec_id);
            return _dbAccess.getSinggleObj<ProductSpec>(tempStr);
        }

        public List<ProductSpec> Query(ProductSpec query)
        {
            StringBuilder stb = new StringBuilder("select spec_id,product_id,spec_type,spec_name,spec_sort,spec_status,spec_image from product_spec where 1=1 ");
            if (query.product_id != 0)
            {
                stb.AppendFormat(" and product_id = {0}", query.product_id);
            }
            if (query.spec_type != 0)
            {
                stb.AppendFormat(" and spec_type = {0}", query.spec_type);
            }

            return _dbAccess.getDataTableForObj<ProductSpec>(stb.ToString());
        }

        public string Update(ProductSpec uSpec)
        {
            uSpec.Replace4MySQL();
            StringBuilder stb = new StringBuilder("update product_spec");
            stb.AppendFormat(" set spec_name='{0}',spec_sort={1},spec_status={2},spec_image='{3}'", uSpec.spec_name, uSpec.spec_sort, uSpec.spec_status, uSpec.spec_image);
            stb.AppendFormat(" where spec_id = {0};", uSpec.spec_id);
            return stb.ToString();
        }

        public int UpdateSingle(ProductSpec uSpec)
        {
            uSpec.Replace4MySQL();
            StringBuilder stb = new StringBuilder("update product_spec");
            stb.AppendFormat(" set spec_image='{0}'", uSpec.spec_image);
            stb.AppendFormat(" where spec_id = {0};", uSpec.spec_id);
            return _dbAccess.execCommand(stb.ToString());
        }

        //public string Update(ProductSpec pSpec)
        //{
        //    StringBuilder strSql = new StringBuilder("update product_spec set ");
        //    strSql.AppendFormat(" spec_status={0} ,spec_image='{1}'", pSpec.spec_status, pSpec.spec_image);
        //    if (pSpec.spec_sort != 0)
        //    {
        //        strSql.AppendFormat(" ,spec_sort={0}", pSpec.spec_sort);
        //    }
        //    strSql.AppendFormat("  where product_id={0} and spec_name='{1}'", pSpec.product_id, pSpec.spec_name);
        //    return strSql.ToString();
        //}

        public string Delete(uint product_Id)
        {
            StringBuilder strSql = new StringBuilder("delete from product_spec where product_id=" + product_Id);
            return strSql.ToString();
        }

        public string SaveFromSpec(ProductSpec proSpec)
        {
            StringBuilder strSql = new StringBuilder(" insert into product_spec(spec_id,product_id,spec_type,spec_name,spec_image,spec_sort,spec_status) select {0},{1},");
            strSql.AppendFormat(" spec_type,spec_name,spec_image,spec_sort,spec_status from product_spec where product_id={0}", proSpec.product_id);
            strSql.AppendFormat(" and spec_id={0};", proSpec.spec_id);
            return strSql.ToString();
        }

        public string UpdateCopySpecId(ProductSpec proSpec)
        {
            StringBuilder strSql = new StringBuilder(" set sql_safe_updates = 0;update product_spec set spec_id={0}");
            strSql.AppendFormat(" where spec_id={0};set sql_safe_updates = 1;", proSpec.spec_id);
            return strSql.ToString();
        }
        public int Updspecstatus(ProductStockQuery m)
        {
            StringBuilder stb = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                sql.AppendFormat("SELECT spec_id_1,spec_id_2 from product_item where item_id ='{0}'", m.item_id);
                dt = _dbAccess.getDataTable(sql.ToString());

                if (!string.IsNullOrEmpty(m.spec_status))
                {
                    if (dt.Rows[0]["spec_id_1"].ToString() != "0" && dt.Rows[0]["spec_id_1"] != null)
                    {
                        stb.AppendFormat("set sql_safe_updates = 0;update product_spec set spec_status='{1}' where spec_id='{0}';set sql_safe_updates = 1; ", dt.Rows[0]["spec_id_1"], m.spec_status);
                    }
                }
                if (!string.IsNullOrEmpty(m.spec_status2))
                {
                    if (dt.Rows[0]["spec_id_2"].ToString() != "0" && dt.Rows[0]["spec_id_2"] != null)
                    {
                        stb.AppendFormat("set sql_safe_updates = 0;update product_spec set spec_status='{1}' where spec_id='{0}';set sql_safe_updates = 1; ", dt.Rows[0]["spec_id_2"], m.spec_status2);
                    }
                }
                if (stb.ToString().Length > 0)
                {
                    return _dbAccess.execCommand(stb.ToString());
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecDao.Updspecstatus-->" + ex.Message + stb + ";" + sql, ex);
            }
        }
    }
}
