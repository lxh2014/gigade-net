using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class ProductCategoryDao : IProductCategoryImplDao
    {
        private IDBAccess _access;
        private string tempStr = "";
        public ProductCategoryDao(string connectionStr)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<ProductCategoryCustom> Query(int fatherId, int status = 1)
        {
            //category_code+
            tempStr = string.Format(@"select category_id as id,category_name as text from product_category where category_display = {0} and category_father_id = {1}", status, fatherId);
            return _access.getDataTableForObj<ProductCategoryCustom>(tempStr);
        }

        public List<Model.ProductCategory> QueryAll(Model.ProductCategory query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder("select category_id,category_name,category_father_id from product_category where 1=1");
            if (query.category_id != 0)
            {
                sql.AppendFormat(" and category_id = {0}", query.category_id);
            }
            if (!string.IsNullOrEmpty(query.category_name))
            {
                sql.AppendFormat(" and category_name = '{0}'", query.category_name);
            }
            if (query.category_father_id != 0)
            {
                sql.AppendFormat("  and category_father_id = {0}", query.category_father_id);
            }
            if (query.category_display != 0)//顯示與否
            {
                sql.AppendFormat(" and category_display = {0}", query.category_display);
            }
            if (query.status != 0)//啟用/禁用
            {
                sql.AppendFormat(" and status = {0}", query.status);
            }
            return _access.getDataTableForObj<Model.ProductCategory>(sql.ToString());
        }

        /// <summary>
        /// 獲取所有顯示的但不包括促銷的所有類別
        /// add by shuangshuang0420j 20141023 15:21
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Model.ProductCategory> GetProductCate(Model.ProductCategory query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder("select category_id,category_name,category_father_id from product_category where  category_display <> 0 ");
            sql.Append(" and category_id <> (select parameterProperty from t_parametersrc where parametercode='CXXM')");

            return _access.getDataTableForObj<Model.ProductCategory>(sql.ToString());
        }


        public List<ProductCategoryCustom> cateQuery(int fatherId)
        {
            tempStr = string.Format(@"select rowid, parametercode ,parametername as text from t_parametersrc where parametertype = 'product_cate' and topvalue = {0}", fatherId);
            return _access.getDataTableForObj<ProductCategoryCustom>(tempStr);
        }

        public uint GetCateID(string eventId)//css
        {
            StringBuilder sql = new StringBuilder(" select category_id from product_category where 1=1 ");
            sql.AppendFormat(" and event_id='{0}';", eventId);

            DataTable dt = _access.getDataTable(sql.ToString());

            return Convert.ToUInt32(dt.Rows[0][0].ToString());
        }

        public int Save(Model.ProductCategory model)//css
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" insert into product_category( category_father_id,category_name,category_sort,category_display,");
            sb.AppendFormat(" category_show_mode,category_image_in,category_image_out,category_link_mode,category_link_url,banner_image,banner_status,banner_link_mode,");
            sb.AppendFormat(" banner_link_url,banner_show_start,banner_show_end,category_createdate,category_updatedate,category_ipfrom)");
            sb.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}');select @@identity;",
                model.category_father_id, model.category_name, model.category_sort,
                model.category_display, model.category_show_mode, model.category_image_in
                , model.category_image_out, model.category_link_mode, model.category_link_url,
                model.banner_image, model.banner_status, model.banner_link_mode,
                model.banner_link_url, model.banner_show_start, model.banner_show_end,
                model.category_createdate, model.category_updatedate, model.category_ipfrom);
            return Int32.Parse(_access.getDataTable(sb.ToString()).Rows[0][0].ToString());

        }
        /// <summary>
        /// 用於返回事物所用到的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns>PromotiionsAmountFare、Promotionsamountdiscount</returns>
        public string SaveCategory(ProductCategory model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" insert into product_category( category_father_id,category_name,category_sort,category_display,");
            sb.AppendFormat(" category_show_mode,category_image_in,category_image_out,category_link_mode,category_link_url,banner_image,banner_status,banner_link_mode,");
            sb.AppendFormat(" banner_link_url,banner_show_start,banner_show_end,category_createdate,category_updatedate,category_ipfrom )");
            sb.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}');select @@identity;",
                model.category_father_id, model.category_name, model.category_sort,
                model.category_display, model.category_show_mode, model.category_image_in
                , model.category_image_out, model.category_link_mode, model.category_link_url,
                model.banner_image, model.banner_status, model.banner_link_mode,
                model.banner_link_url, model.banner_show_start, model.banner_show_end,
                model.category_createdate, model.category_updatedate, model.category_ipfrom);
            return sb.ToString();
        }

        public int Update(Model.ProductCategory model)//
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" update product_category set");
            sb.AppendFormat(" category_name='{0}',category_sort={1},category_display={2},", model.category_name, model.category_sort, model.category_display);
            sb.AppendFormat(" category_show_mode={0},category_image_in='{1}',category_image_out='{2}',", model.category_show_mode, model.category_image_in, model.category_image_out);
            sb.AppendFormat(" category_link_mode={0},category_link_url='{1}',banner_image='{2}',banner_status={3},banner_link_mode={4},", model.category_link_mode, model.category_link_url, model.banner_image, model.banner_status, model.banner_link_mode);
            sb.AppendFormat(" banner_link_url='{0}',banner_show_start={1},banner_show_end={2},", model.banner_link_url, model.banner_show_start, model.banner_show_end);
            sb.AppendFormat(" category_updatedate={0},category_ipfrom='{1}',category_father_id='{2}' where category_id={3};", model.category_updatedate, model.category_ipfrom, model.category_father_id, model.category_id);
            return _access.execCommand(sb.ToString());
        }

        public int Delete(Model.ProductCategory model)//
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" update product_category set category_display=0 where category_id='{0}'; ", model.category_id);
            return _access.execCommand(sb.ToString());
        }

        public ProductCategory GetModelById(uint id)
        {
            StringBuilder sql = new StringBuilder("select *  from product_category ");
            sql.AppendFormat("  where 1=1 and category_id={0};", id);
            return _access.getSinggleObj<ProductCategory>(sql.ToString());
        }

        public string UpdateProdCate(ProductCategory model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" update product_category set");
            sb.AppendFormat(" category_name='{0}',category_sort={1},category_display={2},", model.category_name, model.category_sort, model.category_display);
            sb.AppendFormat(" category_show_mode={0},category_image_in='{1}',category_image_out='{2}',", model.category_show_mode, model.category_image_in, model.category_image_out);
            sb.AppendFormat(" category_link_mode={0},category_link_url='{1}',banner_image='{2}',banner_status={3},banner_link_mode={4},", model.category_link_mode, model.category_link_url, model.banner_image, model.banner_status, model.banner_link_mode);
            sb.AppendFormat(" banner_link_url='{0}',banner_show_start={1},banner_show_end={2},", model.banner_link_url, model.banner_show_start, model.banner_show_end);
            sb.AppendFormat(" category_updatedate={0},category_ipfrom='{1}',category_father_id='{2}' where category_id={3};", model.category_updatedate, model.category_ipfrom, model.category_father_id, model.category_id);
            return sb.ToString();
        }
        public string Delete(uint cateID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" update product_category set category_display=0 where category_id='{0}' ;", cateID);
            return sb.ToString();
        }

        #region 活動商品列表 + List<ProdPromo> GetList
        public List<ProdPromoQuery> GetList(ProdPromo store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            sqlCount.AppendFormat("select count(*) as totalCount from prod_promo pp ");
            sql.AppendFormat("select pp.rid,pp.product_id,pp.event_id,pp.event_type,pp.event_desc,pp.`start`,pp.`end`,pp.page_url,pp.kdate,pp.`status`,pp.muser,mu.user_username from prod_promo pp LEFT JOIN manage_user mu on pp.muser=mu.user_id ");
            sqlWhere.AppendFormat(" where 1=1  ");
            if (!string.IsNullOrEmpty(store.SearchContent))
            {
                sqlWhere.AppendFormat(" and (pp.event_id like '%{0}%' or pp.product_id like '%{1}%' )  ", store.SearchContent, store.SearchContent);
            }
            if (store.IsPage)
            {
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }
            }
            sqlWhere.AppendFormat(" order by rid desc  limit {0},{1} ;", store.Start, store.Limit);
            try
            {
                return _access.getDataTableForObj<ProdPromoQuery>(sql.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryDao-->GetList-->" + sql.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 更改活動状态
        /// </summary>
        /// <param name="store">ProdPromo</param>
        /// <returns>受影响行数</returns>
        public int UpStatus(ProdPromoQuery store)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"update prod_promo set `status`='{0}',mdate='{1}',muser='{2}'  where rid in ({3})",store.status,Common.CommonFunction.DateTimeToString(store.mdate),store.muser,store.rids);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryDao-->UpStatus-->" + ex.Message + sbSql.ToString(), ex);
            }
        }


        public int UpdateUrl(ProdPromo store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update prod_promo set page_url='{0}' where rid={1};", store.page_url, store.rid);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryDao-->UpdateUrl-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetProductCategoryStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT category_id, CONCAT(category_id,'-',category_name) as category_name  FROM  product_category  WHERE  banner_status = 1;");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryDao-->GetProductCategoryStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetCagegoryIdsByIdAndFatherId(ProductCategory query)
        {
            StringBuilder sql = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                sql.AppendFormat(@"SELECT	category_id,category_name FROM product_category 
												WHERE category_display = 1 and (category_id={0} or  category_father_id={0})", query.category_id);               
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryDao-->GetCagegoryIdsByIdAndFatherId-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
