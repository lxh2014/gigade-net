#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductCategoryBannerDao.cs
* 摘 要：
* 專區商品類別設置
* 当前版本：v1.0
* 作 者： shuangshuang0420j
* 完成日期：2014/12/30 
* 修改歷史：
*         
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;
namespace BLL.gigade.Dao
{
    public class ProductCategoryBannerDao : IProductCagegoryBannerImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ProductCategoryBannerDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public List<ProductCategoryBannerQuery> QueryAll(ProductCategoryBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.AppendFormat("select category_id,category_name,category_father_id  ");
                sqlfrom.AppendFormat(" from product_category_banner ");
                return _access.getDataTableForObj<ProductCategoryBannerQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->QueryAll-->" + ex.Message + sql.ToString() + sqlfrom.ToString(), ex);

            }
        }


        #region product_category_banner列表頁 +GetProCateBanList()
        public List<ProductCategoryBannerQuery> GetProCateBanList(ProductCategoryBannerQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sqlcount.Append("select count( pcb.row_id) as totalCount ");
                sql.Append("select pcb.row_id,pcb.banner_cateid,  et.parameterName 'banner_catename', pcb.category_id,pcb.category_father_id,pcb.category_name,pcb.category_sort,");
                sql.Append("pcb.category_display,pcb.category_link_mode,pcb.createdate,pcb.updatedate,pcb.create_ipfrom,pcb.`status`,pc.category_name as category_father_name , pce.category_name as banner_catename    ");
                sqlfrom.Append(" from product_category_banner pcb ");
                sqlfrom.Append(" left join product_category pc on pcb.category_father_id=pc.category_id  ");
                sqlfrom.Append(" left join  product_category pce on pcb.banner_cateid=pce.category_id");
                sqlfrom.Append(" LEFT JOIN (select parameterCode,parameterName,parameterType,parameterProperty from  t_parametersrc  where parameterType='banner_cate' ) et on et.parameterCode=pcb.banner_cateid");

                if (query.banner_cateid != 0)
                {
                    sqlWhere.AppendFormat(" where pcb.banner_cateid={0} ", query.banner_cateid);
                }
                sqlWhere.Append("  order by pcb.row_id desc ");
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<ProductCategoryBannerQuery>(sql.ToString() + sqlfrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->GetProCateBanList-->" + ex.Message + sql.ToString() + sqlfrom.ToString(), ex);

            }
        }
        #endregion
        /// <summary>
        /// 驗證product_id的合法性
        /// </summary>
        /// <param name="prodIDs"></param>
        /// <returns></returns>
        public string GetProdsByCategorys(string banner_cateid)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            string pids = string.Empty;
            try
            {
                strSql.Append("SELECT  distinct(p.product_id) ");

                sqlCondi.Append(" FROM product p ");
                sqlCondi.Append(" INNER JOIN vendor_brand vb ON p.brand_id = vb.brand_id ");
                sqlCondi.Append(" INNER JOIN vendor v ON  v.vendor_id=vb.vendor_id");
                sqlCondi.Append(" INNER JOIN price_master pm ON p.product_id =pm.product_id AND pm.site_id=1 AND pm.price_status=1");
                sqlCondi.Append(" INNER JOIN product_category_set pcs on pcs.product_id=p.product_id  ");
                if (!string.IsNullOrEmpty(banner_cateid))
                {
                    sqlCondi.AppendFormat(" INNER JOIN (select category_id from   product_category p_c where  p_c.category_id in ({0}) AND p_c.category_display =1  ) pc on pc.category_id=pcs.category_id", banner_cateid);
                }
                else
                {
                    sqlCondi.AppendFormat(" INNER JOIN product_category pc on pc.category_id=pcs.category_id ");

                }
                sqlCondi.Append(" WHERE p.product_id>=10000");
                sqlCondi.Append(" AND p.combination > 0");

                //sqlCondi.Append(" AND pc.category_display =1");//商品狀態( 0.新建商品 1.申請審核 2.審核通過5.上架 6.下架)
                sqlCondi.Append(" AND  p.product_status = 5");//商品狀態( 0.新建商品 1.申請審核 2.審核通過5.上架 6.下架)
                sqlCondi.Append(" AND vb.brand_status = 1");//品牌狀態(1.顯示 2.隱藏)
                sqlCondi.Append(" AND  v.vendor_status = 1");//供應商狀態（0.禁用 1.啟用）
                //sqlCondi.Append(" AND p.shortage = 1 ");//(1=暫停販售，但前台還是要顯示)
                sqlCondi.Append(" AND pm.price_status = 1");//價格主檔狀態(1:上架 2:申請審核 3:申請駁回 4:下架)
                sqlCondi.AppendFormat(" AND p.product_start <= {0}", Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString())));
                sqlCondi.AppendFormat(" AND p.product_end >= {0}", Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString())));
                strSql.Append(sqlCondi.ToString());
                DataTable dt = _access.getDataTable(strSql.ToString());
                string prods = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    prods += dt.Rows[i]["product_id"].ToString();
                    if (i != dt.Rows.Count - 1)
                    {
                        prods += ",";
                    }
                }

                return prods;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->isSaleProd-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #region 專區類別刪除
        public int DeleteProCateBan(int rowId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from product_category_banner where row_id={0};set sql_safe_updates=1;", rowId);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->DeleteProCateBan-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        public List<ProductCategory> GetXGCate()
        {
            StringBuilder sqlXG = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            try
            {
                sqlXG.Append(" select parameterProperty from t_parametersrc where parametercode='XG';");
                uint XGCateid = Convert.ToUInt32(_access.getDataTable(sqlXG.ToString()).Rows[0]["parameterProperty"].ToString());
                sql.Append("select category_id,category_name,category_father_id from product_category where  category_display =1");
                sql.AppendFormat(" and category_id in  ({0})", XGCateid);
                return _access.getDataTableForObj<Model.ProductCategory>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->GetXGCate-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #region 獲取類別信息+List<ProductCategory> GetProductCategoryInfo(string categoryIds)
        /// <summary>
        /// 獲取類別信息
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <returns></returns>
        public List<ProductCategory> GetProductCategoryInfo(string categoryIds)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("select category_id,category_father_id,category_name,category_sort,category_display,category_link_mode");
                sb.Append(",category_createdate,category_updatedate,category_ipfrom,status ");
                sb.AppendFormat("from product_category   where category_id in ({0})", categoryIds);
                sb.Append(" and category_id not in ( select parameterCode from  t_parametersrc  where parameterType='banner_cate')");

                return _access.getDataTableForObj<ProductCategory>(sb.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerDao-->GetProductCategoryInfo-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        public int UpdateState(ProductCategoryBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;update product_category_banner set status={0},updatedate={1} where row_id={2};set sql_safe_updates=1; ", query.status, CommonFunction.GetPHPTime(), query.row_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->UpdateState-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string Save(string[] values, ProductCategoryBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" insert into product_category_banner(category_id,category_name, category_father_id,category_sort,category_display,");
            sql.AppendFormat(" category_link_mode,createdate,updatedate,create_ipfrom,status,banner_cateid)");
            sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}',", Convert.ToUInt32(values[0]), values[1], Convert.ToUInt32(values[2]), Convert.ToUInt32(values[3]), Convert.ToUInt32(values[4]));
            sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');", Convert.ToUInt32(values[5]), query.createdate, query.updatedate, query.create_ipfrom, Convert.ToInt32(values[6]), query.banner_cateid);
            return sql.ToString();
        }
        public DataTable GetDatable(int id)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append("select category_id,category_father_id,category_name from product_category where  ");
                sbSql.AppendFormat("  category_id = {0}", id);
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerDao-->GetDatable-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        public DataTable GetMyDatable()
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append("select pc1.category_id,pc1.category_father_id from product_category pc1, product_category pc2 where pc1.category_id=pc2.category_father_id ");
                DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerDao-->GetDatable-->" + ex.Message + sbSql.ToString(), ex);
            }



        }
        public string DeleteByBanCateId(ProductCategoryBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            string str = string.Empty;
            sql.AppendFormat("set sql_safe_updates=0;delete from product_category_banner where banner_cateid={0};set sql_safe_updates=1; ", query.banner_cateid);
            return sql.ToString();
        }

        #region 根據專區獲取符合條件的商品
        public string GetProductByCateId(string category_ids, uint banner_cateid)
        {
            StringBuilder sqlSel = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();

            try
            {
                sqlSel.Append("SELECT distinct(p.product_id)");
                sqlSel.Append(" FROM product_category_set pcs");
                // sqlSel.Append(" INNER JOIN product_category pc ON pcs.category_id = pc.category_id AND category_display = 1 ");
                if (!string.IsNullOrEmpty(category_ids))
                {
                    sqlCondi.AppendFormat(" INNER JOIN (select category_id from   product_category p_c where  p_c.category_id in ({0}) AND category_display = 1 ) pc on pc.category_id=pcs.category_id", category_ids);
                }
                else
                {
                    sqlCondi.AppendFormat(" INNER JOIN product_category pc on pc.category_id=pcs.category_id AND category_display = 1 ");
                }
                sqlSel.Append(" INNER JOIN product p  ON p.product_id = pcs.product_id");
                sqlSel.Append(" INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id ");
                sqlSel.Append(" INNER  JOIN vendor v ON vb.vendor_id = v.vendor_id");
                sqlSel.Append(" INNER JOIN  price_master pm  ON pm.product_id = p.product_id ");
                sqlCondi.Append(" WHERE vb.brand_status = 1");
                sqlCondi.Append("  AND p.product_id>=10000");

                sqlCondi.Append(" AND p.combination > 0");
                sqlCondi.Append(" AND p.product_status = 5");
                sqlCondi.Append(" AND v.vendor_status = 1 AND pm.price_status = 1 AND pm.site_id = 1");
                sqlCondi.AppendFormat(" AND p.product_start <={0} AND p.product_end >= {0}", CommonFunction.GetPHPTime());
                switch (banner_cateid)
                {
                    case 999998://優惠專區
                        sqlCondi.AppendFormat(" AND pm.event_start <={0} AND pm.event_end >={0}", CommonFunction.GetPHPTime());
                        break;
                    case 999997://店配取貨
                        sqlSel.Append(" INNER JOIN product_delivery_set pds on pds.product_id=p.product_id  AND  pds.freight_type = 12 ");//物流配送方式為本島店配
                        break;
                    case 999996://免運專區freeshipping條件(3.常溫免運 4.冷凍免運 6.冷藏免運)
                        sqlCondi.Append("  AND p.product_freight_set in (3,4,6)");
                        break;
                    case 999995://產地直送
                        sqlCondi.Append("   AND p.product_mode=1");
                        break;

                }

                sqlCondi.Append(" GROUP BY p.product_id");
                sqlSel.Append(sqlCondi.ToString());
                DataTable _dt = _access.getDataTable(sqlSel.ToString());
                string prods = string.Empty;
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    prods += _dt.Rows[i]["product_id"].ToString();
                    if (i != _dt.Rows.Count - 1)
                    {
                        prods += ",";
                    }
                }
                return prods;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerDao-->GetProductByCateId-->" + ex.Message + sqlSel.ToString(), ex);
            }
        }
        #endregion


    }
}
