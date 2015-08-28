using BLL.gigade.Common;
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
    public class ProductCategoryBrandDao
    {
        private IDBAccess _access;
        public ProductCategoryBrandDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public List<ProductCategoryBrandQuery> GetCateBrandList(ProductCategoryBrandQuery cateBrandQuery, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlTotal = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlTotal.Append(" SELECT DISTINCT row_id,banner_cate_id, et.parameterName 'banner_catename',category_id,category_name,category_father_id,category_father_name,depth,pcb.brand_id,vb.brand_name,createdate  ");
                sql.Append(" from product_category_brand  pcb ");
                sql.Append("LEFT JOIN vendor_brand vb on vb.brand_id=pcb.brand_id");
                sql.Append(" LEFT JOIN (select parameterCode,parameterName,parameterType,parameterProperty from  t_parametersrc  where parameterType='banner_cate' and used=1 ) et on et.parameterCode=pcb.banner_cate_id");
                if (cateBrandQuery.brand_id != 0)
                {
                    if (string.IsNullOrEmpty(sqlCondition.ToString()))
                    {
                        sqlCondition.AppendFormat(" where  vb.brand_id='{0}' ", cateBrandQuery.brand_id);
                    }
                    else
                    {
                        sqlCondition.AppendFormat(" and  vb.brand_id='{0}' ", cateBrandQuery.brand_id);
                    }
                }
                if (!string.IsNullOrEmpty(cateBrandQuery.brand_name))
                {
                    if (string.IsNullOrEmpty(sqlCondition.ToString()))
                    {
                        sqlCondition.AppendFormat(" where  vb.brand_name  like N'%{0}%' ", cateBrandQuery.brand_name);
                    }
                    else
                    {
                        sqlCondition.AppendFormat(" and  vb.brand_name  like N'%{0}%' ", cateBrandQuery.brand_name);
                    }
                }
                if (cateBrandQuery.category_id != 0)
                {
                    if (string.IsNullOrEmpty(sqlCondition.ToString()))
                    {
                        sqlCondition.AppendFormat(" where  category_id='{0}' ", cateBrandQuery.category_id);
                    }
                    else
                    {
                        sqlCondition.AppendFormat(" and  category_id='{0}' ", cateBrandQuery.category_id);
                    }
                }
                if (!string.IsNullOrEmpty(cateBrandQuery.category_name))
                {
                    if (string.IsNullOrEmpty(sqlCondition.ToString()))
                    {
                        sqlCondition.AppendFormat(" where category_name  like N'%{0}%' ", cateBrandQuery.category_name);
                    }
                    else
                    {
                        sqlCondition.AppendFormat(" and  category_name  like N'%{0}%' ", cateBrandQuery.category_name);

                    }
                }
                if (cateBrandQuery.banner_cate_id != 0)
                {
                    if (string.IsNullOrEmpty(sqlCondition.ToString()))
                    {
                        sqlCondition.AppendFormat(" where  banner_cate_id='{0}' ", cateBrandQuery.banner_cate_id);
                    }
                    else
                    {
                        sqlCondition.AppendFormat(" and banner_cate_id='{0}' ", cateBrandQuery.banner_cate_id);
                    }
                }
                if (cateBrandQuery.IsPage)
                {
                    DataTable dt = _access.getDataTable(" SELECT count(row_id)  as totalCount" + sql.ToString() + sqlCondition.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sqlCondition.AppendFormat(@" LIMIT {0},{1} ", cateBrandQuery.Start, cateBrandQuery.Limit);
                }

                return _access.getDataTableForObj<ProductCategoryBrandQuery>(sqlTotal.ToString() + sql.ToString() + sqlCondition.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandDao-->GetCateBrandList" + ex.Message + sqlTotal.ToString() + sql.ToString() + sqlCondition.ToString(), ex);
            }
        }
        public List<ProductCategoryBrand> GetSaledProduct(uint XGCateID, int banner_cateid)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                strSql.Append("SELECT  distinct p.brand_id,pcs.category_id ");
                sqlCondi.Append(" FROM  (select product_id,brand_id from  product  where product_id>=10000 AND combination > 0 AND   product_status = 5 ");//商品狀態( 0.新建商品 1.申請審核 2.審核通過5.上架 6.下架)
                sqlCondi.AppendFormat(" AND product_start <= {0}", Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString())));
                sqlCondi.AppendFormat(" AND product_end >= {0}", Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString())));
                sqlCondi.Append(" )p  ");
                sqlCondi.Append(" INNER JOIN price_master pm ON p.product_id =pm.product_id AND pm.site_id=1 AND pm.price_status=1");//價格主檔狀態(1:上架 2:申請審核 3:申請駁回 4:下架)
                sqlCondi.Append(" INNER JOIN product_category_set pcs on pcs.product_id=p.product_id  ");
                if (banner_cateid != XGCateID)
                {
                    sqlCondi.AppendFormat(" INNER JOIN product_category_set pcs1 on pcs1.product_id=p.product_id  and  pcs1.category_id='{0}' ", banner_cateid);
                }
                sqlCondi.Append(" INNER JOIN (SELECT category_id,category_father_id from  product_category ");
                sqlCondi.Append("  WHERE category_display=1 ");
                sqlCondi.AppendFormat(" AND category_father_id>'{0}' ", XGCateID);
                sqlCondi.Append(" )  pc ON pc.category_id=pcs.category_id");
                // sqlCondi.Append(" AND vb.brand_status = 1");//品牌狀態(1.顯示 2.隱藏)
                // sqlCondi.Append(" AND  v.vendor_status = 1");//供應商狀態（0.禁用 1.啟用）
                strSql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<ProductCategoryBrand>(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandDao-->GetSaledProduct-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #region 根據專區獲取符合條件的商品
        /// <summary> 
        /// 根據專區獲取符合條件的商品 優惠專區 店配取貨 免運專區 產地直送
        /// </summary>
        /// <param name="banner_cateid">banner_cateid</param>
        /// <returns>返回List<ProductCategoryBrand></returns>      
        public List<ProductCategoryBrand> GetProductByCondi(uint XGCateId, int banner_cateid)
        {
            StringBuilder sqlSel = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sqlSel.Append("SELECT distinct p.brand_id,pcs.category_id ");
                sqlSel.Append(" FROM product_category_set pcs  ");
                sqlSel.Append(" INNER JOIN product_category pc ON pcs.category_id = pc.category_id AND category_display = 1  ");
                sqlSel.Append(" INNER JOIN (select product_id,brand_id,product_freight_set,product_mode from  product  where product_id>=10000 AND combination > 0 AND   product_status = 5");
                sqlSel.AppendFormat(" AND product_start <= {0}", Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString())));
                sqlSel.AppendFormat(" AND product_end >= {0}", Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString())));
                sqlSel.Append("  ) p  ON p.product_id = pcs.product_id ");
                sqlSel.Append(" INNER JOIN  price_master pm  ON pm.product_id = p.product_id   AND pm.price_status = 1 AND pm.site_id = 1 ");
                sqlCondi.AppendFormat(" WHERE pcs.category_id >'{0}' ", XGCateId);
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
                sqlSel.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<ProductCategoryBrand>(sqlSel.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandDao-->GetProductByCondi-->" + ex.Message + sqlSel.ToString(), ex);
            }
        }
        #endregion
        //public string InsertCateBrand(ProductCategoryBrand query, int banner_cate_id)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        sql.Append(" insert into  product_category_brand (banner_cate_id,brand_id,category_id,category_name,depth,createdate) values ");
        //        sql.AppendFormat(" ('{0}','{1}','{2}','{3}','{4}','{5}');", banner_cate_id, query.brand_id, query.category_id, query.category_name, query.depth, Common.CommonFunction.DateTimeToString(DateTime.Now));
        //        return sql.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ProductCategoryBrandDao-->InsertCateBrand-->" + ex.Message + sql.ToString(), ex);
        //    }
        //}
        public string InsertCateBrand(string[] value, int banner_cate_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" insert into  product_category_brand (banner_cate_id,brand_id,category_id,category_name,category_father_id,category_father_name,depth,createdate) values ");
                sql.AppendFormat(" ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", banner_cate_id, Convert.ToUInt32(value[4]), Convert.ToUInt32(value[0]), value[1], Convert.ToUInt32(value[2]), value[3], Convert.ToInt32(value[5]), CommonFunction.DateTimeToString(DateTime.Now));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandDao-->InsertCateBrand-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string DeleteCateBrand(int banner_cate_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates=0; ");
                sql.AppendFormat("   delete  from  product_category_brand where banner_cate_id='{0}' or banner_cate_id is NULL  ;", banner_cate_id);
                sql.Append("  set sql_safe_updates=1; ");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandDao-->DeleteCateBrand-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
