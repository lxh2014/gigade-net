/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeSetTempDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:42:20 
 * 修改歷史：v1.1 2014/08/28 by shuangshuang0420j
 *         內容：新增查詢供應商商品公告+ List<Model.ProductNoticeSetTemp> QueryVendorProdNotice(Model.ProductNoticeSetTemp productNoticeSetTemp)
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ProductNoticeSetTempDao : IProductNoticeSetTempImplDao
    {
        private IDBAccess _dbAccess;
        public ProductNoticeSetTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region IProductNoticeSetTempImplDao 成员

        public List<Model.ProductNoticeSetTemp> Query(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("select writer_id,product_id,notice_id from product_notice_set_temp where 1=1");
            if (productNoticeSetTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", productNoticeSetTemp.Writer_Id);
            }
            if (productNoticeSetTemp.Combo_Type != 0)
            {
                strSql.AppendFormat(" and combo_type={0}", productNoticeSetTemp.Combo_Type);
            }
            strSql.AppendFormat(" and product_id='{0}'", productNoticeSetTemp.product_id );
            return _dbAccess.getDataTableForObj<Model.ProductNoticeSetTemp>(strSql.ToString());
        }

        public string Delete(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_notice_set_temp where ");
            strSql.AppendFormat(" writer_id={0} and combo_type = {1}", productNoticeSetTemp.Writer_Id, productNoticeSetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", productNoticeSetTemp.product_id );
            return strSql.ToString();
        }
        public string DeleteVendor(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_notice_set_temp where ");
            strSql.AppendFormat(" writer_id={0} and combo_type = {1}", productNoticeSetTemp.Writer_Id, productNoticeSetTemp.Combo_Type);
            if (!string.IsNullOrEmpty(productNoticeSetTemp.product_id))
            {
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", productNoticeSetTemp.product_id);

            }

            return strSql.ToString();
        }
        public string Save(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set_temp(`writer_id`,`product_id`,`notice_id`,`combo_type`)");
            strSql.AppendFormat("values({0},{1},{2},{3});", productNoticeSetTemp.Writer_Id, productNoticeSetTemp.product_id, productNoticeSetTemp.notice_id, productNoticeSetTemp.Combo_Type);
            return strSql.ToString();
        }

        public string MoveNotice(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set(product_id,notice_id) select {0} as product_id,");
            strSql.AppendFormat("notice_id from product_notice_set_temp where writer_id={0} and combo_type = {1}", productNoticeSetTemp.Writer_Id, productNoticeSetTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}'", productNoticeSetTemp.product_id );
            return strSql.ToString();
        }

        public string SaveFromProNotice(Model.ProductNoticeSetTemp proNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set_temp (writer_id,product_id,notice_id,combo_type) ");
            strSql.AppendFormat("select {0} as writer_id,product_id,notice_id,{1} as combo_type", proNoticeSetTemp.Writer_Id, proNoticeSetTemp.Combo_Type);
            strSql.AppendFormat(" from product_notice_set where product_id='{0}'", proNoticeSetTemp.product_id);
            return strSql.ToString();
        }


        #region 查詢供應商商品公告+ List<Model.ProductNoticeSetTemp> QueryVendorProdNotice(Model.ProductNoticeSetTemp productNoticeSetTemp)

        public List<Model.ProductNoticeSetTemp> QueryVendorProdNotice(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append(" select writer_id,product_id ,notice_id from product_notice_set_temp where 1=1");
                if (productNoticeSetTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id={0}", productNoticeSetTemp.Writer_Id);
                }
                if (productNoticeSetTemp.Combo_Type != 0)
                {
                    strSql.AppendFormat(" and combo_type={0}", productNoticeSetTemp.Combo_Type);
                }
                if (!string.IsNullOrEmpty(productNoticeSetTemp.product_id))
                {
                    strSql.AppendFormat(" and product_id='{0}';", productNoticeSetTemp.product_id);
                }
                return _dbAccess.getDataTableForObj<Model.ProductNoticeSetTemp>(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempDao-->QueryVendorProdNotice-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 供應商商品公告新增+string Save_Vendor(Model.ProductNoticeSetTemp productNoticeSetTemp)
        public string Save_Vendor(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set_temp(`writer_id`,`product_id`,`notice_id`,`combo_type`)");
            strSql.AppendFormat("values({0},'{1}',{2},{3});", productNoticeSetTemp.Writer_Id, productNoticeSetTemp.product_id, productNoticeSetTemp.notice_id, productNoticeSetTemp.Combo_Type);
            return strSql.ToString();
        }
        #endregion

        #region 供應商複製用
        public string VendorSaveFromProNotice(Model.ProductNoticeSetTemp proNoticeSetTemp, string old_product_id)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set_temp (writer_id,product_id,notice_id,combo_type) ");
            strSql.AppendFormat("select {0} as writer_id,'{1}' as product_id,notice_id,{2} as combo_type", proNoticeSetTemp.Writer_Id, proNoticeSetTemp.product_id, proNoticeSetTemp.Combo_Type);

            uint productid = 0;
            if (uint.TryParse(old_product_id, out productid))
            {
                strSql.AppendFormat(" from product_notice_set  where product_id={0}", productid);
            }
            else
            {
                strSql.AppendFormat(" from product_notice_set_temp where product_id='{0}'", old_product_id);

            }



            return strSql.ToString();
        }
        #endregion

        #endregion

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品公告信息由臨時表移動到正式表
        /// </summary>
        /// <param name="productNoticeSetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveNotice(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set(product_id,notice_id) select {0} as product_id,notice_id from product_notice_set_temp where 1=1 ");
            if (productNoticeSetTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0} ", productNoticeSetTemp.Writer_Id);
            }
            strSql.AppendFormat(" and combo_type = {0} and product_id='{1}';", productNoticeSetTemp.Combo_Type, productNoticeSetTemp.product_id);
            return strSql.ToString();
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品公告信息由臨時表移除
        /// </summary>
        /// <param name="productNoticeSetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;");
            strSql.AppendFormat("delete from product_notice_set_temp where product_id='{0}';", productNoticeSetTemp.product_id);
            strSql.Append("set sql_safe_updates=1;");
            return strSql.ToString();
        }
        #endregion
    }
}
