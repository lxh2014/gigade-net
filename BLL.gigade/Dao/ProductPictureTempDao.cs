
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    class ProductPictureTempDao : IProductPictureTempImplDao
    {
        private IDBAccess _dbAccess;
        public ProductPictureTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public string Save(ProductPictureTemp Pic, int type)
        {
            Pic.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into ");
            stb.Append(type == 1 ? "product_picture_temp" : "product_picture_app_temp");//當type為1時,操作product_picture_temp表,為2時操作product_picture_app_temp表
            stb.Append("(`writer_id`,`product_id`,`image_filename`,`image_sort`,`image_state`,`image_createdate`,`combo_type`)");
            if (!string.IsNullOrEmpty(Pic.product_id))
            {
                stb.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", Pic.writer_Id, Pic.product_id, Pic.image_filename, Pic.image_sort, Pic.image_state, Pic.image_createdate, Pic.combo_type);
            }
            else
            {
                stb.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", Pic.writer_Id, Pic.product_id, Pic.image_filename, Pic.image_sort, Pic.image_state, Pic.image_createdate, Pic.combo_type);
            }
            return stb.ToString();
        }

        public string Delete(ProductPictureTemp proPictureTemp, int type)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0; delete from ");
            strSql.Append(type == 1 ? " product_picture_temp" : "product_picture_app_temp"); //當type為1時,操作product_picture_temp表,為2時操作product_picture_app_temp表
            strSql.AppendFormat(" where writer_id={0} and combo_type={1}", proPictureTemp.writer_Id, proPictureTemp.combo_type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proPictureTemp.product_id);
            return strSql.ToString();
        }

        public List<ProductPictureTemp> Query(ProductPictureTemp proPictureTemp, int type)
        {
            StringBuilder stb = new StringBuilder();
            //stb.Append(type == 1 ? "product_picture_temp" : "product_picture_app_temp");//當類型為1時查詢商品說明表,為 2時查詢APP圖檔表
            //stb.AppendFormat(" where writer_id={0} and combo_type={1}", proPictureTemp.writer_Id, proPictureTemp.combo_type);
            //stb.AppendFormat(" and product_id='{0}'", proPictureTemp.product_id); //edit by xiangwang0413w 2014/10/08 不等於0的商品不再是本次新複製的商品
            stb.AppendFormat(@"SELECT image_filename,image_sort,image_state, 1 AS pic_type FROM product_picture_temp

    WHERE writer_id={0} and combo_type={1} AND product_id = {2}

    UNION ALL

    SELECT image_filename,image_sort,image_state, 2 AS pic_type FROM product_picture_app_temp

    WHERE writer_id={0} and combo_type={1} AND product_id = {2}", proPictureTemp.writer_Id, proPictureTemp.combo_type, proPictureTemp.product_id);

            return _dbAccess.getDataTableForObj<ProductPictureTemp>(stb.ToString());
        }

        public string MoveToProductPicture(ProductPictureTemp proPictureTemp, int type)
        {
            StringBuilder sql =  new StringBuilder("insert into ");
            sql.Append(type == 1 ? "product_picture" : "product_picture_app");
            sql.Append("(product_id,image_filename,image_sort,image_state,image_createdate)");
            sql.Append(" select {0} as product_id,image_filename,image_sort,image_state,image_createdate from ");
            sql.Append(type == 1 ? "product_picture_temp" : "product_picture_app_temp");//當type為1時,操作product_picture_temp表,為2時操作product_picture_app_temp表
            sql.AppendFormat(" where writer_id={0} and combo_type={1}", proPictureTemp.writer_Id, proPictureTemp.combo_type);
            sql.AppendFormat(" and product_id='{0}'", proPictureTemp.product_id);



            return sql.ToString();
        }

        public string SaveFromProPicture(ProductPictureTemp proPictureTemp, int type)
        {
            StringBuilder strSql = new StringBuilder("insert into ");

            strSql.Append(type == 1 ? "product_picture_temp " : "product_picture_app_temp ");//當type為1時,操作product_picture_temp表,為2時操作product_picture_app_temp表
            strSql.AppendFormat("(writer_id,product_id,image_filename,image_sort,image_state,image_createdate,combo_type) ");
            strSql.AppendFormat("select {0} as writer_id,product_id,image_filename,image_sort,image_state,image_createdate,{1} as combo_type from ", 
                proPictureTemp.writer_Id, proPictureTemp.combo_type);
            strSql.Append(type == 1 ? "product_picture" : "product_picture_app");//當type為1時,操作product_picture表,為2時操作product_picture_app表
            strSql.AppendFormat(" where product_id='{0}'", proPictureTemp.product_id);
            return strSql.ToString();
        }

        #region 刪除數據
        public string DeleteByVendor(ProductPictureTemp proPictureTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0; delete from product_picture_temp ");
            strSql.AppendFormat("where writer_id={0} and combo_type={1}", proPictureTemp.writer_Id, proPictureTemp.combo_type);
            strSql.AppendFormat("  and product_id='{0}';set sql_safe_updates = 1;", proPictureTemp.product_id);//edit by xiangwang0413w 2014/10/08 不等於0的商品不再是本次新複製的商品

            return strSql.ToString();
        }
        #endregion


        public List<ProductPictureTemp> VendorQuery(ProductPictureTemp proPictureTemp)
        {

            StringBuilder stb = new StringBuilder();
            try
            {
                stb.AppendFormat("select image_filename,image_sort,image_state from product_picture_temp  where 1=1 ");
                if (proPictureTemp.writer_Id != 0)
                {
                    stb.AppendFormat(" and writer_id={0} ", proPictureTemp.writer_Id);
                }
                if (proPictureTemp.combo_type != 0)
                {
                    stb.AppendFormat(" and combo_type={0} ", proPictureTemp.combo_type);
                }
                if (!string.IsNullOrEmpty(proPictureTemp.product_id))
                {
                    stb.AppendFormat(" and product_id='{0}' ", proPictureTemp.product_id);
                }


                return _dbAccess.getDataTableForObj<ProductPictureTemp>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempDao-->VendorQuery-->" + ex.Message + stb.ToString(), ex);
            }
        }

        public string VendorSaveFromProPicture(ProductPictureTemp proPictureTemp, string old_product_Id)
        {
            StringBuilder strSql = new StringBuilder("insert into product_picture_temp (writer_id,product_id,image_filename,image_sort,image_state,image_createdate,combo_type) ");
            strSql.AppendFormat("select {0} as writer_id, '{1}' as product_id,image_filename,image_sort,image_state,image_createdate,{2} as combo_type", proPictureTemp.writer_Id, proPictureTemp.product_id, proPictureTemp.combo_type);
            uint productid = 0;
            if (uint.TryParse(old_product_Id, out productid))
            {
                strSql.AppendFormat(" from product_picture where product_id={0};", productid);
            }
            else
            {
                strSql.AppendFormat(" from product_picture_temp where product_id='{0}';", old_product_Id);
            }
            return strSql.ToString();
        }

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品圖檔信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proPictureTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveToProductPicture(ProductPictureTemp proPictureTemp)
        {
            StringBuilder sql = new StringBuilder("insert into product_picture(product_id,image_filename,image_sort,image_state,image_createdate)");
            sql.Append(" select {0} as product_id,image_filename,image_sort,image_state,image_createdate from product_picture_temp where 1=1");
            if (proPictureTemp.writer_Id != 0)
            {
                sql.AppendFormat(" and writer_id={0}", proPictureTemp.writer_Id);
            }
            sql.AppendFormat(" and product_id='{0}' and combo_type={1};", proPictureTemp.product_id, proPictureTemp.combo_type);
            return sql.ToString();
        }
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品圖檔信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proPictureTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(ProductPictureTemp proPictureTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0; delete from product_picture_temp where ");
            //strSql.AppendFormat(" writer_id={0} and combo_type={1}", proPictureTemp.writer_Id, proPictureTemp.combo_type);
            strSql.AppendFormat(" product_id='{0}';set sql_safe_updates = 1;", proPictureTemp.product_id);
            return strSql.ToString();
        }
        #endregion
    }
}
