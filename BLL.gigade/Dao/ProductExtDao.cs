using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Collections;


namespace BLL.gigade.Dao
{
    public class ProductExtDao : IProductExtImplDao
    {
        private IDBAccess _dbAccess;
        //private MySqlDao _mayDao;
        public ProductExtDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        /// <summary>
        /// 查詢product_ext
        /// </summary>
        /// <param name="pe">查詢條件</param>
        /// <returns>符合條件的集合</returns>
        public List<ProductExtCustom> Query(int[] ids, ProductExtCustom.Condition condition)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //edit by zhuoqin0830w  2015/10/21  判斷規格是否為空  CONCAT(IF(ps1.spec_name IS NULL,'',ps1.spec_name),'  ',IF(ps2.spec_name IS NULL,'',ps2.spec_name)) AS spec_name
                sb.Append(@"SELECT p.product_id,p.brand_id,p.product_name,p.prod_sz,pit.item_id,pe.pend_del,pe.cde_dt_shp,pe.pwy_dte_ctl,pe.cde_dt_incr,
                           pe.cde_dt_var,pe.hzd_ind,pe.cse_wid,pe.cse_wgt,pe.cse_unit,pe.cse_len,pe.cse_hgt,pe.unit_ship_cse,pe.inner_pack_wid,pe.inner_pack_wgt,
                           pe.inner_pack_unit,pe.inner_pack_len,pe.inner_pack_hgt,CONCAT(IF(ps1.spec_name IS NULL,'',ps1.spec_name),'  ',IF(ps2.spec_name IS NULL,'',ps2.spec_name)) AS spec_name FROM product p 
                           INNER JOIN product_item pit ON p.product_id =pit.product_id 
                           LEFT JOIN product_spec ps1 ON ps1.spec_id = pit.spec_id_1 
                           LEFT JOIN product_spec ps2 ON ps2.spec_id = pit.spec_id_2 
                           LEFT JOIN product_ext pe ON pe.item_id=pit.item_id WHERE 1=1"); //add by wwei0216w 添加規格1和規格2查詢  
                switch (condition)
                {
                    case ProductExtCustom.Condition.ProductId:
                        sb.AppendFormat("  AND p.product_id in({0}) AND p.product_id > 10000;", string.Join(",", ids));
                        break;
                    case ProductExtCustom.Condition.ItemId:
                        sb.AppendFormat("  AND pit.item_id in({0}) AND p.product_id > 10000;", string.Join(",", ids));
                        break;
                    case ProductExtCustom.Condition.BrandId:
                        sb.AppendFormat("  AND p.brand_id ={0} AND p.product_id > 10000;", string.Join(",", ids));//add by wwei0216w 2015/6/24 只查詢大於10000的商品
                        break;
                }

                return _dbAccess.getDataTableForObj<ProductExtCustom>(sb.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductExtDao-->Query" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 操作商品細項
        /// </summary>
        /// <param name="pe">需要操作的商品條件</param>
        /// <returns>操作是否成功</returns>
        public string UpdateProductExt(ProductExtCustom pe)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                ///該段代碼由於數據庫表結構的原因,只能delete,insert操作,請勿更改 edit by wwei0216w 2015/6/30 
                sb.AppendFormat("DELETE FROM product_ext WHERE item_id = {0};", pe.Item_id);
                sb.Append("INSERT INTO product_ext(`item_id`,`pend_del`,`cde_dt_shp`,`pwy_dte_ctl`,`cde_dt_incr`,`cde_dt_var`,`hzd_ind`,`cse_wid`,`cse_wgt`,`cse_unit`,`cse_len`,`cse_hgt`,`unit_ship_cse`,`inner_pack_wid`,`inner_pack_wgt`,`inner_pack_unit`,`inner_pack_len`,`inner_pack_hgt`) ");
                sb.AppendFormat("VALUES({0},'{1}',{2},'{3}',{4},{5},'{6}',{7},{8},'{9}',", pe.Item_id, pe.Pend_del, pe.Cde_dt_shp, pe.Pwy_dte_ctl, pe.Cde_dt_incr, pe.Cde_dt_var, pe.Hzd_ind, pe.Cse_wid, pe.Cse_wgt, pe.Cse_unit);
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}) ", pe.Cse_len, pe.Cse_hgt, pe.Unit_ship_cse, pe.Inner_pack_wid, pe.Inner_pack_wgt, pe.Inner_pack_unit, pe.Inner_pack_len, pe.Inner_pack_wgt);
                //array.Add(sb.ToString());
                return sb.ToString();
                //sb.AppendFormat(@"UPDATE product_ext SET pend_del = '{0}',cde_dt_shp = {1},pwy_dte_ctl = '{2}',cde_dt_incr = {3},",pe.Pend_del,pe.Cde_dt_shp,pe.Pwy_dte_ctl,pe.Cde_dt_incr);
                //sb.AppendFormat(@"cde_dt_var = {0},hzd_ind = '{1}',cse_wid={2},cse_wgt={3},",pe.Cde_dt_var,pe.Hzd_ind,pe.Cse_wid,pe.Cse_wgt);
                //sb.AppendFormat(@"cse_unit = {0},cse_len = {1},cse_hgt={2},unit_ship_cse={3},inner_pack_wid = {4},", pe.Cse_unit, pe.Cse_len, pe.Cse_hgt, pe.Unit_ship_cse,pe.Inner_pack_wid);
                //sb.AppendFormat(@"inner_pack_wgt = {0},inner_pack_unit = {1},inner_pack_len = {2},inner_pack_hgt = {3} ",pe.Inner_pack_wgt,pe.Inner_pack_unit,pe.Inner_pack_len,pe.Inner_pack_hgt);
                //sb.AppendFormat(@"WHERE item_id = {0}",pe.Item_id);
                //return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtDao-->UpdateProductExt" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除結果
        /// </summary>
        /// <param name="pe">刪除條件</param>
        /// <returns>受影響的行數</returns>
        public int DeleteProductExtByCondition(ProductExtCustom pe)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"DELETE FROM product_ext WHERE item_id = {0}", pe.Item_id);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtDao.DeleteProductExtByCondition-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新“是否等待刪除”
        /// </summary>
        /// <param name="pe">更新條件</param>
        /// <returns>受影響的行數</returns>
        public int UpdatePendDel(uint proudctId, bool penDel)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"set sql_safe_updates=0;update product_ext set pend_del = '{0}' where item_id in (select item_id from product_item where product_id={1});set sql_safe_updates=1;", penDel ? "Y" : "N", proudctId);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtDao.UpdatePendDel-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據有效期限修改數據  add by zhuoqin0830w 2015/06/09
        /// </summary>
        /// <param name="particularsSrc"></param>
        /// <returns></returns>
        public bool UpdateExtByCdedtincr(List<ParticularsSrc> particularsSrc)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SET sql_safe_updates = 0;");
                foreach (var item in particularsSrc)
                {
                    if (item.particularsName != "null")
                    {
                        sb.AppendFormat("UPDATE product_ext SET Cde_dt_shp = {0} WHERE Cde_dt_incr= {1} AND Cde_dt_shp= {2};", item.particularsCome, item.particularsValid, item.oldCome);
                        sb.AppendFormat("UPDATE product_ext SET Cde_dt_var= {0} WHERE Cde_dt_incr= {1} AND Cde_dt_var = {2};", item.particularsCollect, item.particularsValid, item.oldCollect);
                    }
                }
                sb.Append("SET sql_safe_updates = 1;");
                return _dbAccess.execCommand(sb.ToString()) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtDao.UpdateExtByCdedtincr-->" + ex.Message, ex);
            }
        }

        public List<ProductExtCustom> QueryHistoryInfo(DateTime Update_start, DateTime Update_end, int Brand_id = 0, string Item_ids = null, string Product_ids = null)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT th.batchno,vb.brand_name,p.product_id,pi.item_id,p.product_name,u.user_mail,
                                FROM_UNIXTIME(p.product_createdate) AS product_createdate,tb.kuser,tb.kdate,
                                MAX(IF(thi.col_name = 'cde_dt_shp',thi.col_value,0)) AS 'shp_value',
	                            MAX(IF(thi.col_name = 'cde_dt_shp',thi.old_value,0)) AS 'shp_old',
	                            MAX(IF(thi.col_name = 'cde_dt_var',thi.col_value,0)) AS 'var_value',
	                            MAX(IF(thi.col_name = 'cde_dt_var',thi.old_value,0)) AS 'var_old',
	                            MAX(IF(thi.col_name = 'cde_dt_incr',thi.col_value,0)) AS 'incr_value',
	                            MAX(IF(thi.col_name = 'cde_dt_incr',thi.old_value,0)) AS 'incr_old'
                            FROM product p");
                sb.Append(" INNER JOIN product_item pi ON pi.product_id = p.product_id ");
                if (Item_ids != "")
                {
                    sb.AppendFormat(" AND pi.item_id IN ('{0}') ", Item_ids);
                }
                sb.Append(" INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id ");
                if (Brand_id != 0)
                {
                    sb.AppendFormat(" AND vb.brand_id = {0} ", Brand_id);
                }
                sb.Append(" INNER JOIN t_table_history th ON th.PK_value = pi.item_id AND th.PK_name = 'item_id' ");
                sb.Append(" INNER JOIN t_table_historyitem thi ON thi.tableHistoryId = th.rowid AND col_name IN ('cde_dt_shp','cde_dt_var','cde_dt_incr') ");
                sb.AppendFormat(" INNER JOIN t_history_batch tb ON tb.batchno = th.batchno AND tb.kdate BETWEEN '{0}' AND '{1}' ", Update_start.ToString("yyyy-MM-dd HH:MM:ss"), Update_end.ToString("yyyy-MM-dd HH:MM:ss"));
                sb.Append(" INNER JOIN mail_user u ON u.row_id = p.user_id ");
                if (Product_ids != "")
                {
                    sb.AppendFormat(" WHERE p.product_id IN ('{0}')  ", Product_ids);
                }
                sb.Append(" GROUP BY th.batchno");
                return _dbAccess.getDataTableForObj<ProductExtCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtDao-->QueryHistoryInfo" + ex.Message, ex);
            }
        }
    }
}