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
            DataTable dt = _access.getDataTable(sb.ToString());
            try
            {
                return dt.Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->GetMsgByProductId-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        public int Save(RecommendedProductAttribute rPA)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" insert into recommended_product_attribute(product_id,time_start,time_end,expend_day)value('{0}','{1}','{2}','{3}') ", rPA.product_id, rPA.time_start, rPA.time_end, rPA.expend_day);
                results = _access.execCommand(sb.ToString());
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int Update(RecommendedProductAttribute rPA)
        {
            int results = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" update recommended_product_attribute set time_start='{0}',time_end='{1}',expend_day='{2}' where  product_id='{3}' ", rPA.time_start, rPA.time_end, rPA.expend_day,rPA.product_id);
                results = _access.execCommand(sb.ToString());
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }

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
    }
}
