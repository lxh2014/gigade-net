using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
namespace BLL.gigade.Dao
{
    public class ProductCategorySetTempDao : IProductCategorySetTempImplDao
    {
        private IDBAccess _access;
        public ProductCategorySetTempDao(string conStr)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, conStr);
        }

        public List<ProductCategorySetTemp> Query(ProductCategorySetTemp queryTemp)
        {
            StringBuilder strSql = new StringBuilder("select writer_id,category_id from product_category_set_temp ");
            strSql.AppendFormat("where writer_id = {0} and combo_type = {1} ", queryTemp.Writer_Id, queryTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}'", queryTemp.Product_Id);
            return _access.getDataTableForObj<ProductCategorySetTemp>(strSql.ToString());
        }

        public int Save(ProductCategorySetTemp saveTemp)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set_temp (`writer_id`,`product_id`,`category_id`,`brand_id`,`combo_type`) ");
            stb.AppendFormat(" values ({0},{1},{2},{3},{4})", saveTemp.Writer_Id, saveTemp.Product_Id, saveTemp.Category_Id, saveTemp.Brand_Id, saveTemp.Combo_Type);
            return _access.execCommand(stb.ToString());
        }

        public int Delete(ProductCategorySetTemp delTemp,string deStr="0")
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0; delete from product_category_set_temp ");
            strSql.AppendFormat("where writer_id = {0} and combo_type = {1}", delTemp.Writer_Id, delTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}'", delTemp.Product_Id );
            if (deStr != "0")
            {
                strSql.AppendFormat(" AND category_id in ({0})", deStr);
            }
            strSql.Append("; set sql_safe_updates = 1;");
            return _access.execCommand(strSql.ToString());
        }

        public string TempMoveCategory(ProductCategorySetTemp proCategorySetTemp)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set(product_id,category_id,brand_id) select {0} as product_id,");
            stb.AppendFormat("category_id,brand_id from product_category_set_temp where writer_id = {0} and combo_type = {1}", proCategorySetTemp.Writer_Id, proCategorySetTemp.Combo_Type);
            stb.AppendFormat(" and product_id='{0}'", proCategorySetTemp.Product_Id);
            return stb.ToString();
        }

        public string TempDelete(ProductCategorySetTemp proCategorySetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0; delete from product_category_set_temp ");
            strSql.AppendFormat("where writer_id = {0} and combo_type = {1}", proCategorySetTemp.Writer_Id, proCategorySetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proCategorySetTemp.Product_Id);
            return strSql.ToString();
        }

        public string SaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_category_set_temp select ");
            strSql.AppendFormat("{0} as writer_id,product_id,category_id,brand_id,{1} as combo_type from product_category_set ", proCategorySetTemp.Writer_Id, proCategorySetTemp.Combo_Type);
            strSql.AppendFormat("where product_id='{0}'", proCategorySetTemp.Product_Id);
            return strSql.ToString();
        }


        #region 供應商商品處理
        #region 刪除臨時表數據 返回sql語句 +string TempDeleteByVendor(ProductCategorySetTemp proCategorySetTemp)
        public string TempDeleteByVendor(ProductCategorySetTemp proCategorySetTemp)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0; delete from product_category_set_temp ");

                strSql.AppendFormat(" where  writer_id={0} ", proCategorySetTemp.Writer_Id);

                strSql.AppendFormat(" and combo_type={0}", proCategorySetTemp.Combo_Type);

                if (!string.IsNullOrEmpty(proCategorySetTemp.Product_Id) && proCategorySetTemp.Product_Id != "0")
                {
                    strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proCategorySetTemp.Product_Id);
                }
                else
                {
                    strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proCategorySetTemp.Product_Id);
                }
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempDao-->TempDeleteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        #endregion


        #region  刪除臨時表數據 返回執行結果 +string DeleteByVendor(ProductCategorySetTemp delTemp)
        public int DeleteByVendor(ProductCategorySetTemp delTemp)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0; delete from product_category_set_temp ");
                strSql.AppendFormat("where writer_id = {0} and combo_type = {1}", delTemp.Writer_Id, delTemp.Combo_Type);
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", delTemp.Product_Id);
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempDao-->DeleteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 執行保存操作 + int SaveByVendor(ProductCategorySetTemp saveTemp)

        public int SaveByVendor(ProductCategorySetTemp saveTemp)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("insert into product_category_set_temp (`writer_id`,`product_id`,`category_id`,`brand_id`,`combo_type`) ");
                strSql.AppendFormat(" values ({0},'{1}',{2},{3},{4})", saveTemp.Writer_Id, saveTemp.Product_Id, saveTemp.Category_Id, saveTemp.Brand_Id, saveTemp.Combo_Type);
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempDao-->SaveByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 查詢該商品下面的前台類別 +List<ProductCategorySetTemp> QueryByVendor(ProductCategorySetTemp queryTemp)
        public List<ProductCategorySetTemp> QueryByVendor(ProductCategorySetTemp queryTemp)
        {   //edit jialei 20140912 用於商品管理供應商申請審核
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("select writer_id,category_id from product_category_set_temp where 1=1 ");
                if (queryTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id = {0} ", queryTemp.Writer_Id);
                }
                if (!string.IsNullOrEmpty(queryTemp.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}'", queryTemp.Product_Id);
                }
                if (queryTemp.Combo_Type!=0)
                {
                    strSql.AppendFormat(" and combo_type = {0}", queryTemp.Combo_Type);
                }
                strSql.AppendFormat(";set sql_safe_updates = 1; ", queryTemp.Combo_Type);
                return _access.getDataTableForObj<ProductCategorySetTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempDao-->QueryByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion
        public string VendorSaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp)
        {//20140905 供應商複製
            StringBuilder strSql = new StringBuilder("insert into product_category_set_temp select ");
            strSql.AppendFormat("{0} as writer_id,product_id,category_id,brand_id,{1} as combo_type from product_category_set_temp ", proCategorySetTemp.Writer_Id, proCategorySetTemp.Combo_Type);
            strSql.AppendFormat("where product_id='{0}'", proCategorySetTemp.Product_Id);
            return strSql.ToString();
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品類別信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proCategorySetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_TempMoveCategory(ProductCategorySetTemp proCategorySetTemp)
        {
            StringBuilder stb = new StringBuilder("insert into product_category_set(product_id,category_id,brand_id) ");
            stb.Append("select {0} as product_id,category_id,brand_id from product_category_set_temp where 1=1");
            if (proCategorySetTemp.Writer_Id != 0)
            {
                stb.AppendFormat(" and writer_id = {0} ", proCategorySetTemp.Writer_Id);
            }
            stb.AppendFormat(" and product_id='{0}' and combo_type = {1}", proCategorySetTemp.Product_Id, proCategorySetTemp.Combo_Type);
            return stb.ToString();
        }
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品類別信息由臨時表移除
        /// </summary>
        /// <param name="proCategorySetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_TempDelete(ProductCategorySetTemp proCategorySetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0; delete from product_category_set_temp where 1=1");
            if (proCategorySetTemp.Writer_Id!=0)
            {
                strSql.AppendFormat(" and writer_id = {0} ", proCategorySetTemp.Writer_Id);
            }
            strSql.AppendFormat(" and combo_type = {0}",proCategorySetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proCategorySetTemp.Product_Id);
            return strSql.ToString();
        }
        #endregion

        public string VendorSaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp, string old_product_Id)
        {
            StringBuilder strSql = new StringBuilder("insert into product_category_set_temp select ");
            strSql.AppendFormat("{0} as writer_id,'{1}' as product_id,category_id,brand_id,{2} as combo_type ", proCategorySetTemp.Writer_Id, proCategorySetTemp.Product_Id, proCategorySetTemp.Combo_Type);
              uint productid = 0;
              if (uint.TryParse(old_product_Id, out productid))
              {
                  strSql.AppendFormat(" from product_category_set  where product_id={0}", productid);
              }
              else {
                  strSql.AppendFormat(" from product_category_set_temp  where product_id='{0}'", old_product_Id);
              }
           
            return strSql.ToString();
        }
    }
}
