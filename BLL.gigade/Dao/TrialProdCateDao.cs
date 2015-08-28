using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class TrialProdCateDao : ITrialProdCateImplDao
    {
        private IDBAccess _access;
        private string connStr;

        #region 有參構造函數
        /// <summary>
        /// 有參構造函數
        /// </summary>
        /// <param name="connectionstring">數據庫連接字符串</param>
        public TrialProdCateDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #endregion

        #region  獲取列表頁  +List<TrialProdCateQuery> Query(TrialProdCateQuery query, out int totalCount)
        /// <summary>
        /// 獲取滿額滿件送禮列表頁
        /// </summary>
        /// <param name="query">TrialProdCateQuery query對象 </param>
        /// <param name="totalCount">輸出總行數</param>
        /// <returns>List<Model.Query.TrialProdCateQuery>對象</returns>
        public List<TrialProdCateQuery> Query(TrialProdCateQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append(" select id,event_id,type,product_id,category_id,start_date,end_date from trial_prod_cate ");
                sql.Append(" order by id desc ");
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat("  limit {0},{1}", query.Start, query.Limit);
                }
                sql.AppendFormat(";");

                return _access.getDataTableForObj<TrialProdCateQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateDao-->Query-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        public List<TrialProdCateQuery> UadateTrialProd()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select DISTINCT event_type,event_id,event_name,aaa.product_id,start_date,end_date from  ");
                StringBuilder Giftsql = new StringBuilder();
                Giftsql.Append(" SELECT pag.id t_id,pag.event_type,pag.event_id,pag.name event_name,pcs.product_id ,pag.`start` as start_date,pag.`end` as end_date,pag.site,pag.device ");
                Giftsql.Append(" FROM product_category_set pcs  ");
                Giftsql.AppendFormat(" INNER JOIN promotions_amount_gift pag on pag.category_id=pcs.category_id and pag.`status`=1 and pag.event_type='{0}'  ", "G3");
                Giftsql.Append(" and pag.`start`<=NOW() and pag.`end`>=NOW() and pag.active=1 ");
                Giftsql.Append(" INNER JOIN product_category pc ON pcs.category_id = pc.category_id AND category_display = 1 ");

                StringBuilder Trialsql = new StringBuilder();
                Trialsql.Append("  SELECT  pat.id t_id,pat.event_type,pat.event_id ,pat.name event_name,pat.product_id,pat.start_date as start_date,pat.end_date as end_date,pat.site,pat.device ");
                Trialsql.Append(" from promotions_amount_trial pat  ");
                Trialsql.Append("  where pat.`status`=1 ");
                Trialsql.Append(" and pat.start_date<=NOW() and pat.end_date>=NOW() and pat.active=1 ");

                sql.AppendFormat("  ({0} union ({1})) aaa", Giftsql, Trialsql);
                sql.Append(" INNER JOIN product p ON p.product_id = aaa.product_id  ");
                sql.Append(" INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id    ");
                sql.Append(" INNER JOIN  price_master pm ON pm.product_id = p.product_id   ");
                sql.Append(" WHERE vb.brand_status = 1   ");
                sql.Append(" AND p.product_status = 5   ");
                sql.AppendFormat(" AND p.product_start < '{0}' ", CommonFunction.GetPHPTime());
                sql.AppendFormat(" AND p.product_end >'{0}'  ", CommonFunction.GetPHPTime());
                sql.Append(" AND p.combination > 0  ");
                sql.Append(" AND pm.price_status = 1  ");
                sql.Append(" AND pm.site_id = 1  ");
                sql.Append(" AND aaa.device in (0,1)  ");
                sql.Append("   AND  ( aaa.site =1 OR aaa.site LIKE '1,%' OR aaa.site LIKE '%,1,%' OR aaa.site LIKE '%,1') ;");
                return _access.getDataTableForObj<TrialProdCateQuery>(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateDao-->UadateTrialProd-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertTrialProd(TrialProdCateQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" insert into  trial_prod_cate (event_id,type,category_id,product_id,start_date,end_date) values ");
                sql.AppendFormat(" ('{0}','{1}','{2}','{3}'", query.event_id, query.type, query.category_id, query.product_id);
                sql.AppendFormat(" ,'{0}','{1}') ;", CommonFunction.DateTimeToString(query.start_date), CommonFunction.DateTimeToString(query.end_date));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateDao-->UadateTrialProd-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string DeleteTrialProd()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates=0;delete  from trial_prod_cate ;set sql_safe_updates=1; ");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateDao-->UadateTrialProd-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
