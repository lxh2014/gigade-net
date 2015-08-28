using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    class ProductPictureDao : IProductPictureImplDao
    {
        private IDBAccess _dbAccess;
        public ProductPictureDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public string Save(ProductPicture Pic,int type)
        {
            Pic.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into ");
            stb.Append(type == 1 ? "product_picture" : "product_picture_app");//當type為1時,操作product_picture表,為2時操作product_picture_app表
            stb.AppendFormat("(`product_id`,`image_filename`,`image_sort`,`image_state`,`image_createdate`)");
            stb.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}')", Pic.product_id, Pic.image_filename, Pic.image_sort, Pic.image_state, Pic.image_createdate);
            return stb.ToString();
        }
        public string Delete(int product_id ,int type=1)
        {
            StringBuilder sql = new StringBuilder(@"set sql_safe_updates=0; delete from ");
            sql.Append(type == 1 ? "product_picture" : "product_picture_app");//當type為1時,操作product_picture表,為2時操作product_picture_app表
            sql.AppendFormat(" where product_id={0};set sql_safe_updates=1; ", product_id);
            return sql.ToString();
        }

        public List<ProductPicture> Query(int product_id, int type)
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat(@"SELECT image_filename,image_sort,image_state,1 AS pic_type  FROM product_picture WHERE product_id={0} 

    UNION ALL

    SELECT image_filename,image_sort,image_state, 2 AS pic_type  FROM product_picture_app WHERE product_id={0}",product_id);
            return _dbAccess.getDataTableForObj<ProductPicture>(stb.ToString());
        }

        public string SaveFromOtherPro(ProductPicture productPicture)
        {
            StringBuilder strSql = new StringBuilder("insert into product_picture(`product_id`,`image_filename`,`image_sort`,`image_state`,`image_createdate`) select {0} as product_id");
            strSql.AppendFormat(",image_filename,image_sort,image_state,image_createdate from product_picture where product_id={0}", productPicture.product_id);
            return strSql.ToString();

        }
    }
}
