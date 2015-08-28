#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsMaintainDao.cs 
 * 摘   要： 
 *      促銷項目維護
 * 当前版本：v1.1 
 * 作   者： hongfei0416j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：hongfei0416j
 *      v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class PromotionsMaintainDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public PromotionsMaintainDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr; 
        }
        #region 獲取商品數據 +List<Model.Custom.QueryandVerifyCustom> QueryByProSite(Model.Query.QueryVerifyCondition query, out int totalCount, uint selectCategoryID)
        /// <summary>
        /// 獲取商品數據
        /// </summary>
        /// <param name="query">QueryVerifyCondition類型參數</param>
        /// <param name="totalCount">取得的數量</param>
        /// <param name="selectCategoryID">CategoryID</param>
        /// <returns>返回查詢結果</returns>
        public List<Model.Custom.QueryandVerifyCustom> QueryByProSite(Model.Query.QueryVerifyCondition query, out int totalCount, uint selectCategoryID)
        {
            string sqlEx = String.Empty;
            try
            {
                query.Replace4MySQL();
                StringBuilder strCols = new StringBuilder("select  DISTINCT a.product_id,b.brand_name,a.brand_id,f.price_master_id,a.product_image,a.product_name,a.prod_sz,c.parametername as combination,d.parametername as product_spec,");
                strCols.Append("a.product_price_list,e.parametername as product_status,a.product_status as product_status_id,a.combination as combination_id,a.user_id ");

                StringBuilder strTbls = new StringBuilder("from product a left join vendor_brand b on a.brand_id=b.brand_id ");
                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode ");
                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_spec') d on a.product_spec=d.parametercode ");
                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') e on a.product_status=e.parametercode ");
                strTbls.Append("left join product_category_set j on a.product_id=j.product_id ");

                StringBuilder strCondition = new StringBuilder("where 1=1 and a.shortage=0 and a.product_status=5 ");
                strCondition.AppendFormat(" and a.product_id not in (select product_id from product_category_set  where product_category_set.category_id='{0}'  ) ", query.category_id);
                strCondition.Append(" and LENGTH(a.product_id)=5  ");

                if (query.brand_id != 0)
                {
                    strCondition.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }

                if (query.combination != 0)
                {
                    strCondition.AppendFormat(" and a.combination={0}", query.combination);
                }
                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    strCondition.AppendFormat(" and j.category_id='{0}'", query.cate_id);
                }
                if (selectCategoryID != 0)
                {
                    strCondition.AppendFormat(" and j.category_id={0}", selectCategoryID);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "a", strCondition);
                }

                strCols.Append(",h.parametername as user_level,i.parametername as price_status,f.price,f.cost,f.event_price,f.event_start,f.event_end ");

                strTbls.Append("left join price_master f on a.product_id=f.product_id and(f.product_id=f.child_id or f.child_id=0) ");
                if (query.site_id != 0)
                {
                    strCondition.AppendFormat(" and f.site_id='{0}'  ", query.site_id);
                }
                else if (!string.IsNullOrEmpty(query.site_ids))
                {
                    strCondition.AppendFormat(" and f.site_id in ({0})  ", query.site_ids);
                }
                // strTbls.Append("left join site g on f.site_id=g.site_id ");
                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='userlevel') h on f.user_level=h.parametercode ");
                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='price_status') i on f.price_status=i.parametercode ");
                //  strCondition.Append(" and f.site_id is not null and f.price_status=1");
                strCondition.Append("  and f.price_status=1");

                strCols.Append(",f.parametername as product_freight_set,g.parametername as product_mode,a.tax_type,a.product_sort,a.product_createdate,a.product_start,a.product_end ");

                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') f on a.product_freight_set=f.parametercode ");
                strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') g on a.product_mode=g.parametercode ");

                if (!string.IsNullOrEmpty(query.name_number))//變動二
                {
                    strCondition.AppendFormat(" and  a.product_id  in({0}) ", query.name_number);
                }
                if (!string.IsNullOrEmpty(query.product_name))
                {
                    strCondition.AppendFormat(" and  a.product_name like '%{0}%'", query.product_name);
                }

                string strCount = "select count(a.product_id) as totalCount " + strTbls.ToString() + strCondition.ToString();
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }

                strCondition.Append(" order by a.product_id desc ");
                sqlEx = strCols.ToString() + strTbls.ToString() + strCondition.ToString();
                return _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsMaintainDao-->QueryByProSite-->" + ex.Message + sqlEx, ex);
            }
        }
        #endregion
        #region  拼接日期區間 +void CheckCondition(Model.Query.QueryVerifyCondition qcCon, string table, StringBuilder stb)
        /// <summary>
        /// 拼接日期區間
        /// </summary>
        /// <param name="qcCon">QueryVerifyCondition類型參數</param>
        /// <param name="table">表名</param>
        /// <param name="stb">要拼接的字串</param>
        public void CheckCondition(Model.Query.QueryVerifyCondition qcCon, string table, StringBuilder stb)
        {
            if (qcCon.time_end != "")
            {
                stb.AppendFormat(" and {0}.{1}<='{2}'", table, qcCon.date_type, qcCon.time_end);
            }
            if (qcCon.time_start != "")
            {
                stb.AppendFormat(" and {0}.{1}>='{2}'", table, qcCon.date_type, qcCon.time_start);
            }
        }
        #endregion
        /// <summary>
        /// 獲取活動中的商品列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Model.Custom.QueryandVerifyCustom> GetEventList(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlw = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("SELECT p.product_id,p.product_name,p.brand_id,vb.brand_name,p.product_price_list,f.price,f.cost,pf.parametername as product_freight_set");
                sqlw.Append(" FROM  ( SELECT product_id,category_id,brand_id FROM  product_category_set WHERE product_id>=10000");
                if (!string.IsNullOrEmpty(query.cate_id))
                {//活動類別查詢
                    sqlw.AppendFormat(" and category_id='{0}'", query.cate_id);
                }
                if (!string.IsNullOrEmpty(query.product_name))//編號批次查詢
                {
                    sqlw.AppendFormat(" and product_id in ({0})", query.product_name);
                }
                sqlw.Append(" ) pcs");
                sqlw.Append(" LEFT JOIN product p on p.product_id=pcs.product_id");
                sqlw.Append(" LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id");
                sqlw.Append(" LEFT JOIN (select parametercode,parametername from t_parametersrc where parametertype='product_freight') pf on p.product_freight_set=pf.parametercode ");

                sqlw.Append(" LEFT JOIN price_master f on p.product_id=f.product_id and(f.product_id=f.child_id or f.child_id=0) ");
                sqlw.Append(" WHERE p.product_status=5 and p.shortage=0 and f.price_status=1");//搜索上架商品,並且補貨中不停止販售的,價格狀態為上架
                if (!string.IsNullOrEmpty(query.site_ids))//編號批次查詢
                {
                    sqlw.AppendFormat(" and f.site_id in ({0})", query.site_ids);//支持多站台查詢
                }
                sqlw.Append(" order by p.product_id desc");
                totalCount = 0;
                if (query.IsPage)//分頁
                {
                    DataTable _dt = _dbAccess.getDataTable(" select count(p.product_id) as totalCount " + sqlw.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlw.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlw.ToString());
                return _dbAccess.getDataTableForObj<BLL.gigade.Model.Custom.QueryandVerifyCustom>(sql.ToString());


            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsMaintainDao-->GetEventList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 獲取所有待選擇的商品列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Model.Custom.QueryandVerifyCustom> GetProList(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlw = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("SELECT DISTINCT p.product_id,p.product_name,p.brand_id,vb.brand_name,p.product_price_list,f.price,f.cost,pf.parametername as product_freight_set");
                sqlw.Append(" FROM  ( SELECT product_id,product_name,brand_id,product_price_list,product_freight_set FROM  product WHERE product_id>=10000 and product_status=5 and combination=1  and shortage=0 ");//新商品並且上架的單一商品
                if (!string.IsNullOrEmpty(query.product_name))
                {
                    sqlw.AppendFormat(" and product_id in ({0})", query.product_name);//商品編號批次查詢
                }
                if (query.brand_id != 0)
                {//活動類別查詢
                    sqlw.AppendFormat(" and brand_id='{0}'", query.brand_id);
                }
                else if (!string.IsNullOrEmpty(query.brand_ids))//編號批次查詢
                {
                    sqlw.AppendFormat(" and brand_id in ({0})", query.brand_ids);
                }
                sqlw.Append(" ) p");
                sqlw.Append(" LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id");
                sqlw.Append(" LEFT JOIN (select parametercode,parametername from t_parametersrc where parametertype='product_freight') pf on p.product_freight_set=pf.parametercode ");
                sqlw.Append(" LEFT JOIN price_master f on p.product_id=f.product_id and(f.product_id=f.child_id or f.child_id=0) ");
                sqlw.Append(" WHERE  f.price_status=1");//搜索價格狀態上架商品
                if (!string.IsNullOrEmpty(query.site_ids))//編號批次查詢
                {
                    sqlw.AppendFormat(" and f.site_id in ({0})", query.site_ids);//支持多站台查詢
                }

                if (query.category_id != 0)
                {
                    sqlw.AppendFormat(" and pcs.category_id = '{0}'", query.category_id);//支持商品類別搜索 
                }
                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    sqlw.AppendFormat(" and p.product_id not in ( select product_id from product_category_set where category_id='{0}')", query.cate_id);//排除已經添加進該活動中的商品
                }
                sqlw.Append(" order by p.product_id desc");
                totalCount = 0;
                if (query.IsPage)//分頁
                {
                    DataTable _dt = _dbAccess.getDataTable(" select count(p.product_id) as totalCount " + sqlw.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlw.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlw.ToString());
                return _dbAccess.getDataTableForObj<BLL.gigade.Model.Custom.QueryandVerifyCustom>(sql.ToString());


            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsMaintainDao-->GetEventList-->" + ex.Message + sql.ToString(), ex);
            }
        }


    }

}
