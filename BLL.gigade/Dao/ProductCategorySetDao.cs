using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class ProductCategorySetDao : IProductCategorySetImplDao
    {
        private IDBAccess _access;
        public ProductCategorySetDao(string conStr)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, conStr);
          
        }

        public List<ProductCategorySet> Query(ProductCategorySet queryModel)
        {
            StringBuilder strSql = new StringBuilder("select product_id,category_id,brand_id from product_category_set where 1=1 ");
            if (queryModel.Product_Id != 0)
            {
                strSql.AppendFormat(" and product_id = {0} ", queryModel.Product_Id);
            }
            if (queryModel.Category_Id != 0)
            {
                strSql.AppendFormat(" and category_id = {0} ", queryModel.Category_Id);
            }
            return _access.getDataTableForObj<ProductCategorySet>(strSql.ToString());
        }

        public List<ProductCategorySetCustom> Query(ProductCategorySetCustom query)
        {
            //string sql = string.Format("select s.category_id,c.category_name from product_category_set s inner join product_category c on c.category_id = s.category_id where s.product_id = {0}", query.Product_Id);
            //return _access.getDataTableForObj<ProductCategorySetCustom>(sql);
            StringBuilder sb = new StringBuilder();
            string sql = "select s.category_id,c.category_name from product_category_set s inner join product_category c on c.category_id = s.category_id where 1=1 ";
            if (0 != query.Product_Id)
            {
                sb.AppendFormat(" and s.product_id = {0}", query.Product_Id);
            }
            if (0 != query.Category_Id)
            {
                sb.AppendFormat(" and s.category_id = {0}", query.Category_Id);
            }
            //sb.AppendFormat();
            //+" s.product_id = {0}", query.Product_Id);
            return _access.getDataTableForObj<ProductCategorySetCustom>(sql + sb.ToString());
        }
        public List<ProductCategorySetCustom> QueryByCategory(ProductCategorySetCustom query)
        {
            string sql = string.Format("select s.category_id,c.category_name from product_category_set s inner join product_category c on c.category_id = s.category_id where s.category_id = {0} ", query.Category_Id);
            return _access.getDataTableForObj<ProductCategorySetCustom>(sql);
        }

        public string Save(ProductCategorySet saveModel)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set (`product_id`,`category_id`,`brand_id`) ");
            stb.AppendFormat(" values ({0},{1},{2});", saveModel.Product_Id, saveModel.Category_Id, saveModel.Brand_Id);

            return stb.ToString();
        }

        public string SaveFromOtherPro(ProductCategorySet saveModel)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set (`product_id`,`category_id`,`brand_id`) select {0} as product_id,category_id,brand_id");
            stb.AppendFormat(" from product_category_set where product_id={0}", saveModel.Product_Id);
            return stb.ToString();
        }

        public string Delete(ProductCategorySet delModel, string deStr = "0")
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"set sql_safe_updates = 0;delete from product_category_set where product_id = {0}", delModel.Product_Id);
            if(deStr!="0")
            {
                sql.AppendFormat(" AND category_id in ({0})", deStr);
            }
            sql.Append("; set sql_safe_updates = 1;");
            return sql.ToString();
        }

        public string SaveNoPrid(ProductCategorySet save)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set (`product_id`,`category_id`,`brand_id`)  values ({0},");
            stb.AppendFormat("{0},{1});", save.Category_Id, save.Brand_Id);
            return stb.ToString();

        }
        public int Insert(ProductCategorySet saveModel)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set (`product_id`,`category_id`,`brand_id`) ");
            stb.AppendFormat(" values ({0},{1},{2});", saveModel.Product_Id, saveModel.Category_Id, saveModel.Brand_Id);
            return _access.execCommand(stb.ToString());

        }

        public string SaveProdCategorySet(ProductCategorySet saveModel)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set (`product_id`,`category_id`,`brand_id`) ");
            stb.AppendFormat(" values ({0},{1},{2});", saveModel.Product_Id, saveModel.Category_Id, saveModel.Brand_Id);
            return stb.ToString();
        }

        public string UpdateProdCategorySet(ProductCategorySet saveModel)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" set sql_safe_updates = 0;update product_category_set set product_id='{0}',brand_id='{1}' where category_id='{2}';set sql_safe_updates = 1;", saveModel.Product_Id, saveModel.Brand_Id, saveModel.Category_Id);
            return sb.ToString();

        }
        public int DeleteProductByModel(ProductCategorySet delModel)
        {
            string sql = string.Format("set sql_safe_updates = 0;delete from product_category_set where product_id = {0} and category_id = {1} and  brand_id={2};set sql_safe_updates = 1;", delModel.Product_Id, delModel.Category_Id, delModel.Brand_Id);
            try
            {
            return _access.execCommand(sql);
        }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetDao-->DeleteProductByModel-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string DeleteProductByModelStr(ProductCategorySet delModel)
        {
            string sql = string.Format("set sql_safe_updates = 0;delete from product_category_set where product_id = {0} and category_id = {1} and  brand_id={2};set sql_safe_updates = 1;", delModel.Product_Id, delModel.Category_Id, delModel.Brand_Id);
            try
            {
                return sql;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetDao-->DeleteProductByModelStr-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string DeleteProdCateSet(uint cateID)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("set sql_safe_updates = 0;delete from product_category_set where category_id='{0}';set sql_safe_updates = 1;", cateID);
            return sb.ToString();
        }

        public string DelProdCateSet(uint cateID)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("set sql_safe_updates = 0;delete from product_category_set where category_id='{0}';set sql_safe_updates = 1;", cateID);
            return sb.ToString();
        }
        public string DelProdCateSetByCPID(ProductCategorySet delModel)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" set sql_safe_updates = 0;delete from product_category_set where product_id='{0}' and category_id='{1}';set sql_safe_updates = 1;", delModel.Product_Id, delModel.Category_Id);
            return sb.ToString();
        }


        public DataTable QueryBrand(string webtype, int content_id)
        {
            StringBuilder sbsql = new StringBuilder();
            //sbsql.Append(" SELECT DISTINCT vb.brand_id, CONCAT(vb.brand_name,'[',tsc.parameterName,']') brand_name   from product_category_set pcs ");
            //sbsql.Append(" LEFT JOIN vendor_brand vb on vb.brand_id=pcs.brand_id ");
            //sbsql.Append(" LEFT JOIN t_parametersrc tsc on pcs.category_id=tsc.parameterProperty ");
            //sbsql.Append(" where category_id in (select parameterProperty from t_parametersrc where parameterType in ('site_id','page_id','area_id'))  ");



            sbsql.Append(@"SELECT c.category_id,vb.brand_id  ,CONCAT('【',c.category_name,'】',vb.brand_name) as 'brand_name'   from  product_category_set pcs
                INNER JOIN  vendor_brand vb on vb.brand_id=pcs.brand_id
                right JOIN   (select t2.category_id , t2.category_name from product_category t1,product_category t2 
                where t1.category_id = t2.category_father_id and (t1.category_id=(
                select a.category_id from product_category  a
                where a.category_id in(select parameterProperty from t_parametersrc where parameterType in ('site_id'))) or t1.category_father_id=(
                select a.category_id from product_category  a
                where a.category_id in(select parameterProperty from t_parametersrc where parameterType in ('site_id'))) )
                )c on c.category_id=pcs.category_id");


            if (!string.IsNullOrEmpty(webtype))
            {
                sbsql.AppendFormat("  and vb.brand_id not in (select brand_id from {0} ", webtype);
                if (content_id != 0)
                {
                    sbsql.AppendFormat(" where content_id !='{0}'  ", content_id);
                }
                sbsql.Append(")");
            }

            sbsql.Append(" group by vb.brand_id;");

            return _access.getDataTable(sbsql.ToString());
        }

        public DataTable QueryProduct(string category_id)
        {
            StringBuilder sbsql = new StringBuilder();
            sbsql.Append(@"SELECT c.category_id,p.product_id ,CONCAT('【',vb.brand_name,'】',p.product_name) as 'product_name'   from  product_category_set pcs
                INNER JOIN  product p on p.product_id=pcs.product_id
								INNER JOIN vendor_brand vb on vb.brand_id =p.brand_id
                right JOIN   (select t2.category_id , t2.category_name from product_category t1,product_category t2 
                where t1.category_id = t2.category_father_id and (t1.category_id=(
                select a.category_id from product_category  a
                where a.category_id in(select parameterProperty from t_parametersrc where parameterType in ('site_id'))) or t1.category_father_id=(
                select a.category_id from product_category  a
                where a.category_id in(select parameterProperty from t_parametersrc where parameterType in ('site_id'))) )
                )c on c.category_id=pcs.category_id
                where LENGTH(pcs.product_id)=5
	        GROUP BY p.product_id;");
            //GROUP BY vb.brand_id,p.product_id;");//對商品和品牌排序

            return _access.getDataTable(sbsql.ToString());
        }

        public DataTable GetCateByProds(string prods, string cateids)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("select distinct(pcs.category_id) from product_category_set pcs");
                sql.Append(" left join product_category pc on pc.category_id=pcs.category_id AND category_display = 1 where 1=1");
                if (!string.IsNullOrEmpty(prods))
                {
                    sql.AppendFormat("  and pcs.product_id in ({0})", prods);
                }
                if (!string.IsNullOrEmpty(cateids))
                {
                    sql.AppendFormat("  and pcs.category_id in ({0})", cateids);
                }
                sql.AppendFormat(" order by pcs.category_id");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetDao-->GetCateByProds-->" + ex.Message + sql.ToString(), ex);
            }
        }

        //add by wwei0216w 2015/2/24
        /// <summary>
        /// 根據商品id修改品牌id
        /// </summary>
        /// <param name="pcs">一個ProductCategorySet對象</param>
        /// <returns>將要執行的sql語句</returns>
        public string UpdateBrandId(ProductCategorySet pcs)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0;UPDATE product_category_set SET brand_id={0} WHERE product_id ={1};SET sql_safe_updates = 1;",pcs.Brand_Id,pcs.Product_Id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetDao-->UpdateBrandId" + ex.Message,ex);
            }
        }


        public List<ProductCategorySet> QueryMsg(ProductCategorySetQuery queryModel)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select product_id,category_id,brand_id from product_category_set where 1=1 ");
                if (!string.IsNullOrEmpty(queryModel.product_ids))
                {
                    strSql.AppendFormat(" and product_id in ({0}) ", queryModel.product_ids);
                }
                if (queryModel.Category_Id != 0)
                {
                    strSql.AppendFormat(" and category_id = {0} ", queryModel.Category_Id);
                }
                return _access.getDataTableForObj<ProductCategorySet>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetDao-->QueryMsg" + ex.Message, ex);
            }
        }

      
    }
}
