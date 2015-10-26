using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CategoryDao : ICategoryImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CategoryDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        public List<CategoryQuery> GetCategoryList(CategoryQuery store, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder search = new StringBuilder();
            List<VendorBrandSetQuery> list = new List<VendorBrandSetQuery>();
            StringBuilder strCount = new StringBuilder();
            DataTable _dt = new DataTable();

            try
            {
                strSql.Append(@"SELECT b.category_id,b.category_name,case when a.amount>0 THEN amount ELSE '0' END AS amo from ");
                strCount = strCount.Append("SELECT count(*) as search_total from  ");
                search.Append(" (select sum(od.single_money * buy_num) as amount,pcs.category_id from order_detail od inner join product_item pit using(item_id) INNER JOIN order_slave os USING (slave_id) INNER JOIN order_master om USING (order_id) inner join product p using (product_id) inner join product_category_set pcs using(product_id) where od.detail_status <> 90 ");//  
                //if (store.brand_status == 2 || store.brand_status == 0)
                //{
                //    search.Append(" AND od.detail_status <> 90 ");
                //}
                if (store.brand_status > 1)
                {
                    search.Append(" AND om.money_collect_date > 0 ");
                }
                if (store.seldate != 0)
                {
                    if (store.starttime != 0)
                    {
                        search.AppendFormat(" AND  om.order_createdate >= '{0}' ", store.starttime);
                    }
                    if (store.endtime != 0)
                    {
                        search.AppendFormat(" AND  om.order_createdate <= '{0}' ", store.endtime);
                    }
                }
                search.Append(" GROUP BY pcs.category_id ) a ");
                if (store.serchs != 0)
                {
                    search.AppendFormat(@"RIGHT JOIN (SELECT category_id , category_name FROM product_category WHERE category_id IN (SELECT category_id FROM product_category where (category_father_id='{0}' or category_id='{0}') and category_display = 1)) b ON a.category_id=b.category_id ", store.serchs);
                }
                else
                {
                    search.Append(" RIGHT JOIN (SELECT category_id , category_name FROM product_category WHERE category_id IN (SELECT category_id FROM product_category where category_father_id='5' or category_id='5')) b ON a.category_id=b.category_id ");
                }
                strSql.Append(search.ToString());
                totalCount = 0;
                if (store.IsPage)
                {
                    _dt = _dbAccess.getDataTable(strCount.ToString() + search.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    strSql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                return _dbAccess.getDataTableForObj<CategoryQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryDao-->GetCategoryList-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }
        public string GetSum(CategoryQuery store)
        {
            DataTable _dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (store.serchs == 0)
                {
                    store.serchs = 5;
                }
                sql.AppendFormat("select sum(od.single_money * buy_num) as amount from order_detail od inner join product_item pit using(item_id) INNER JOIN order_slave os USING (slave_id) INNER JOIN order_master om USING (order_id) inner join product p using (product_id) inner join product_category_set pcs using(product_id) where om.money_collect_date > 0 AND od.detail_status <> 90 AND category_id IN (SELECT category_id FROM product_category where category_father_id='{0}' or category_id='{0}') ", store.serchs);
                _dt = _dbAccess.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return _dt.Rows[0][0].ToString();
                }
                else
                    return "0";
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryDao-->GetSum-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
        }
        public List<CategoryQuery> GetCategory()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("SELECT category_id,category_father_id,category_name,category_sort,category_display,category_show_mode,category_image_in,category_image_out,category_link_mode,category_link_url,banner_image,banner_status,banner_link_mode,banner_link_url,banner_show_end,category_createdate,category_updatedate,category_ipfrom,short_description,`status` FROM product_category WHERE banner_status = 1");
                return _dbAccess.getDataTableForObj<CategoryQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryDao-->GetCategory-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }
        #region 根據父類別查詢子類別列表+List<CategoryQuery> GetProductCategoryList(CategoryQuery cq)
        /// <summary>
        /// 根據富類別查詢子類別列表
        /// </summary>
        /// <param name="cq"></param>
        /// <returns></returns>
        public List<CategoryQuery> GetProductCategoryList(CategoryQuery cq, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            StringBuilder strCount = new StringBuilder();
            StringBuilder strCondition = new StringBuilder();
            try
            {
                strSelect.AppendFormat("select pc1.category_id,pc1.category_father_id,pc1.category_name,pc1.category_sort,pc1.category_display,");
                strSelect.AppendFormat(" pc1.category_link_mode,pc1.category_link_url,pc1.banner_image,pc1.banner_status,pc1.banner_link_mode,");
                strSelect.AppendFormat("pc1.banner_link_url,FROM_UNIXTIME(pc1.banner_show_start) as startdate,FROM_UNIXTIME(pc1.banner_show_end) as enddate,");
                strSelect.AppendFormat("pc1.category_createdate,pc1.category_updatedate,pc1.category_ipfrom,pc1.status,pc2.category_name as category_father_name,pc1.short_description ");
                strCount.AppendFormat(" select count(pc1.category_id) as search_total ");
                strCondition.AppendFormat(" from product_category pc1 ");
                strCondition.AppendFormat(" left join product_category pc2 on pc1.category_father_id=pc2.category_id ");
                strCondition.AppendFormat(" where 1=1 ");
                if (cq.category_father_id == 0)
                {
                    strCondition.AppendFormat(" and pc1.category_father_id='{0}'", 2);
                }
                else
                {
                    strCondition.AppendFormat(" and pc1.category_father_id='{0}'", cq.category_father_id);
                }
                totalCount = 0;
                strSql.AppendFormat(strSelect.ToString() + strCondition.ToString());
                if (cq.IsPage)
                {
                    DataTable _dt = _dbAccess.getDataTable(strCount.ToString() + strCondition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    strSql.AppendFormat(" limit {0},{1}", cq.Start, cq.Limit);
                }
                return _dbAccess.getDataTableForObj<CategoryQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryDao-->GetProductCategoryList-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 保存類別信息+ int ProductCategorySave(CategoryQuery cq)
        /// <summary>
        /// 保存類別信息
        /// </summary>
        /// <param name="cq"></param>
        /// <returns></returns>
        public int ProductCategorySave(CategoryQuery cq)
        {
            cq.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                if (cq.category_id == 0)
                {
                    strSql.AppendFormat("insert into product_category (category_father_id,category_name,category_sort,category_display,category_link_mode,");
                    strSql.AppendFormat("category_link_url,banner_image,banner_status,banner_link_mode,banner_link_url,banner_show_start,banner_show_end,");
                    strSql.AppendFormat("category_createdate,category_updatedate,category_ipfrom,short_description,status)values('{0}','{1}',", cq.category_father_id, cq.category_name);
                    strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", cq.category_sort, cq.category_display, cq.category_link_mode, cq.category_link_url, cq.banner_image);
                    strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", cq.banner_status, cq.banner_link_mode, cq.banner_link_url, cq.banner_show_start, cq.banner_show_end);
                    strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}')", cq.category_createdate, cq.category_updatedate, cq.category_ipfrom, cq.short_description, cq.status);
                    return _dbAccess.execCommand(strSql.ToString());
                }
                else
                {
                    strSql.AppendFormat("update product_category set category_father_id='{0}',category_name='{1}',", cq.category_father_id, cq.category_name);
                    strSql.AppendFormat("category_sort='{0}',category_display='{1}',category_link_mode='{2}',", cq.category_sort, cq.category_display, cq.category_link_mode);
                    strSql.AppendFormat("category_link_url='{0}',banner_image='{1}',banner_status='{2}',", cq.category_link_url, cq.banner_image, cq.banner_status);
                    strSql.AppendFormat("banner_link_mode='{0}',banner_link_url='{1}',banner_show_start='{2}',", cq.banner_link_mode, cq.banner_link_url, cq.banner_show_start);
                    strSql.AppendFormat("banner_show_end='{0}',category_updatedate='{1}',", cq.banner_show_end, cq.category_updatedate);
                    strSql.AppendFormat("short_description='{0}' , ", cq.short_description);
                    strSql.AppendFormat("category_ipfrom='{0}' where category_id='{1}'", cq.category_ipfrom, cq.category_id);
                    return _dbAccess.execCommand(strSql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryDao-->ProductCategorySave-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢一條類別信息根據編號+CategoryQuery GetProductCategoryById(CategoryQuery cq)
        /// <summary>
        /// 查詢一條類別信息根據編號
        /// </summary>
        /// <param name="cq"></param>
        /// <returns></returns>
        public CategoryQuery GetProductCategoryById(CategoryQuery cq)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select category_id,category_father_id,category_name,category_sort,category_display,category_link_mode,category_link_url,banner_image,banner_status,banner_link_mode,banner_link_url,banner_show_start,banner_show_end,category_createdate,category_updatedate,category_ipfrom,short_description,status from product_category where category_id='{0}'", cq.category_id);
                return _dbAccess.getSinggleObj<CategoryQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("CategoryDao-->GetProductCategoryById-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 更改狀態+int UpdateActive(CategoryQuery model)
        /// <summary>
        /// 更改狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateActive(CategoryQuery model)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("update product_category set status='{0}' where category_id='{1}';", model.status, model.category_id);
                return _dbAccess.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("CategoryDao-->UpdateActive-->" + ex.Message + strSql, ex);
            }
        }
        #endregion

        public List<Model.ProductCategory> GetProductCategoryCSV(CategoryQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("select category_id,category_name,category_father_id from product_category  ");
                return _dbAccess.getDataTableForObj<Model.ProductCategory>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryDao-->GetProductCategoryCSV-->" + ex.Message + sql.ToString(), ex);
            }

        }

    }
}
