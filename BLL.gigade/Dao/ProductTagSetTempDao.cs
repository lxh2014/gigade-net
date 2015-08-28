/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagSetTempDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:42:43 
 * 修改歷史：
 *     v1.1 2014/08/28 by shuangshuang0420j
 *     內容：新增獲取供應商商品標籤+ List<Model.ProductTagSetTemp> QueryVendorTagSet(Model.ProductTagSetTemp productTagSetTemp)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ProductTagSetTempDao : IProductTagSetTempImplDao
    {
        private IDBAccess _dbAccess;
        public ProductTagSetTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region IProductTagSetTempImplDao 成员

        public List<Model.ProductTagSetTemp> Query(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("select writer_id,tag_id from product_tag_set_temp where 1=1");
            if (productTagSetTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", productTagSetTemp.Writer_Id);
            }
            if (productTagSetTemp.Combo_Type != 0)
            {
                strSql.AppendFormat(" and combo_type={0}", productTagSetTemp.Combo_Type);
            }
            strSql.AppendFormat(" and product_id='{0}'", productTagSetTemp.product_id );
            return _dbAccess.getDataTableForObj<Model.ProductTagSetTemp>(strSql.ToString());
        }

        public string DeleteVendor(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_tag_set_temp where 1=1");
            strSql.AppendFormat(" and writer_id={0} and combo_type = {1}", productTagSetTemp.Writer_Id, productTagSetTemp.Combo_Type);
            if (!string.IsNullOrEmpty(productTagSetTemp.product_id))
            {
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", productTagSetTemp.product_id);

            }

            return strSql.ToString();
        }
        public string Delete(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_tag_set_temp where ");
            strSql.AppendFormat(" writer_id={0} and combo_type = {1}", productTagSetTemp.Writer_Id, productTagSetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", productTagSetTemp.product_id );
            return strSql.ToString();
        }

        public string Save(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set_temp(`writer_id`,`product_id`,`tag_id`,`combo_type`)");
            strSql.AppendFormat("values({0},{1},{2},{3});", productTagSetTemp.Writer_Id, productTagSetTemp.product_id, productTagSetTemp.tag_id, productTagSetTemp.Combo_Type);
            return strSql.ToString();
        }

        public string MoveTag(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set(`product_id`,`tag_id`) select {0} as product_id,");
            strSql.AppendFormat("tag_id from product_tag_set_temp where writer_id={0} and combo_type={1}", productTagSetTemp.Writer_Id, productTagSetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}'", productTagSetTemp.product_id );
            return strSql.ToString();
        }

        public string SaveFromTag(Model.ProductTagSetTemp proTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set_temp(writer_id,product_id,tag_id,combo_type) select ");
            strSql.AppendFormat("{0} as writer_id,product_id,tag_id,{1} as combo_type", proTagSetTemp.Writer_Id, proTagSetTemp.Combo_Type);
            strSql.AppendFormat(" from product_tag_set where product_id='{0}'", proTagSetTemp.product_id);
            return strSql.ToString();
        }


        #region 獲取供應商商品標籤+ List<Model.ProductTagSetTemp> QueryVendorTagSet(Model.ProductTagSetTemp productTagSetTemp)

        public List<Model.ProductTagSetTemp> QueryVendorTagSet(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append(" select writer_id,tag_id from product_tag_set_temp where 1=1");
                if (productTagSetTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id={0}", productTagSetTemp.Writer_Id);
                }
                if (productTagSetTemp.Combo_Type != 0)
                {
                    strSql.AppendFormat(" and combo_type={0}", productTagSetTemp.Combo_Type);
                }
                if (!string.IsNullOrEmpty(productTagSetTemp.product_id))
                {
                    strSql.AppendFormat(" and product_id='{0}';", productTagSetTemp.product_id);
                }
                return _dbAccess.getDataTableForObj<Model.ProductTagSetTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempDao-->QueryVendorTagSet-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        public string VendorTagSetTempSave(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set_temp(`writer_id`,`product_id`,`tag_id`,`combo_type`)");
            strSql.AppendFormat("values({0},'{1}',{2},{3});", productTagSetTemp.Writer_Id, productTagSetTemp.product_id, productTagSetTemp.tag_id, productTagSetTemp.Combo_Type);
            return strSql.ToString();
        }
        public string VendorSaveFromTag(Model.ProductTagSetTemp proTagSetTemp, string old_product_id)
        {//20140905 供應商複製
            StringBuilder strSql = new StringBuilder("insert into product_tag_set_temp(writer_id,product_id,tag_id,combo_type) select ");
            strSql.AppendFormat("{0} as writer_id,'{1}' as product_id,tag_id,{2} as combo_type", proTagSetTemp.Writer_Id, proTagSetTemp.product_id, proTagSetTemp.Combo_Type);
            uint productid = 0;
            if (uint.TryParse(old_product_id, out productid))
            {
                strSql.AppendFormat(" from product_tag_set where product_id={0}; ", productid);
            }
            else
            {
                strSql.AppendFormat(" from product_tag_set_temp where product_id='{0}';", old_product_id);
            }
            return strSql.ToString();
        }
        #endregion


        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品信息由臨時表移動到正式表
        /// </summary>
        /// <param name="productTagSetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveTag(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set(`product_id`,`tag_id`) select {0} as product_id,tag_id from product_tag_set_temp where 1=1");
            if (productTagSetTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", productTagSetTemp.Writer_Id);
            }
            strSql.AppendFormat(" and combo_type={0} and product_id='{1}';", productTagSetTemp.Combo_Type, productTagSetTemp.product_id);
            return strSql.ToString();
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品標籤信息由臨時表移動除
        /// </summary>
        /// <param name="productTagSetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(Model.ProductTagSetTemp productTagSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;");
            //strSql.AppendFormat(" writer_id={0} and combo_type = {1}", productTagSetTemp.Writer_Id, productTagSetTemp.Combo_Type);
            strSql.AppendFormat("delete from product_tag_set_temp where product_id='{0}';", productTagSetTemp.product_id);
            strSql.Append("set sql_safe_updates=1;");
            return strSql.ToString();
        }
        #endregion
    }
}
