using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class RecommendedProductAttributeDao
    {
        IDBAccess _access;
        public RecommendedProductAttributeDao(string connectionStr)
        {
            _access = DBAccess.DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region 根據product_id判斷是否設定推薦商品
        public int GetMsgByProductId(int productId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" select product_id from recommended_product_attribute where product_id='{0}' ", productId);
            try
            {
                DataTable dt = _access.getDataTable(sb.ToString());
                return dt.Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->GetMsgByProductId-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 保存商品推薦屬性的信息
        public int Save(RecommendedProductAttribute rPA)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" insert into recommended_product_attribute(product_id,time_start,time_end,expend_day,months)value('{0}','{1}','{2}','{3}','{4}') ", rPA.product_id, rPA.time_start, rPA.time_end, rPA.expend_day,rPA.months);
                results = _access.execCommand(sb.ToString());
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 編輯商品推薦屬性表中的信息
        public int Update(RecommendedProductAttribute rPA)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" update recommended_product_attribute set time_start='{0}',time_end='{1}',expend_day='{2}',months='{3}' where  product_id='{4}' ", rPA.time_start, rPA.time_end, rPA.expend_day,rPA.months,rPA.product_id);
                results = _access.execCommand(sb.ToString());
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 刪除商品推薦屬性表的信息
        public int Delete(int productId)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" delete from recommended_product_attribute  where  product_id='{0}' ",productId);
                results = _access.execCommand(sb.ToString());
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Delete-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據userid和productid判斷商品推薦臨時表中是否存在數據
        public int ExsitInTemp(int userId, int productId, int comboType)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" select product_id from recommended_product_attribute_temp  where write_id='{0}'and product_id='{1}' and combo_type='{2}'; ", userId,productId,comboType);
                results = _access.getDataTable(sb.ToString()).Rows.Count;
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->ExsitInTemp-->" + ex.Message + sb.ToString(), ex);
            }
        }
       #endregion

        #region 根據userid和productid獲取到商品推薦臨時表中的數據 
        public DataTable GetTempList(int userId,int productId,int comboType)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" select expend_day,months from recommended_product_attribute_temp  where write_id='{0}' and product_id='{1}'and combo_type='{2}'; ", userId, productId, comboType);
                DataTable _dt = _access.getDataTable(sb.ToString());
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->GetTempList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據userid和productid來刪除商品推薦屬性表中的數據
        public int DeleteTemp(int userId, int productId,int comboType)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"set sql_safe_updates=0; delete from recommended_product_attribute_temp  where write_id='{0}'and product_id='{1}' and combo_type='{2}';set sql_safe_updates=1; ", userId,productId,comboType);
                results = _access.execCommand(sb.ToString());
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->DeleteTemp-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據product_id和userid刪除臨時表中的數據  臨時表中product_id用來判斷是臨時保存數據,還是複製時的數據 用來拼接字符串用
        public string TempDelete(int userId,int productId,int comboType)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" set sql_safe_updates=0;delete from recommended_product_attribute_temp  where write_id='{0}' and product_id='{1}' and combo_type='{2}';set sql_safe_updates=1; ", userId, productId,comboType);
               
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->TempDelete-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 商品複製  推薦商品屬性臨時表中插入複製的數據
        public string SaveRecommendedProductAttributSet(RecommendedRroductAttributeTemp recommendBute)
        {
            StringBuilder strSql = new StringBuilder("insert into recommended_product_attribute_temp select ");
            strSql.AppendFormat("product_id,{0} as write_id,{1} as time_start,{2} as  time_end,expend_day,months,{3} as combo_type from recommended_product_attribute ", recommendBute.write_id, 0, 0,recommendBute.combo_type);
            strSql.AppendFormat("where product_id='{0}';", recommendBute.product_id,recommendBute.write_id);
            return strSql.ToString();
        }
        #endregion
    }
}
