using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    class ProductDeliverySetTempDao : IProductDeliverySetTempImplDao
    {
          private IDBAccess _dbAccess;
        private string strConn;
        public ProductDeliverySetTempDao(string connectStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectStr);
            this.strConn = connectStr;
        }
        
        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="sc">一個ProductDeliverySet對象</param>
        /// <returns>受影響的行數</returns>
        public int Save(List<ProductDeliverySetTemp> proDeliSets,int delProdId,int comboType,int writerId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("set sql_safe_updates=0;delete from product_delivery_set_temp where product_id={0} and combo_type={1} and writer_id={2};set sql_safe_updates=1;",
                delProdId, comboType, writerId);
            sb.Append("INSERT INTO product_delivery_set_temp(`writer_id`,`product_id`,`freight_big_area`,`freight_type`,`combo_type`) values ");
            foreach (var item in proDeliSets)
            {
                sb.AppendFormat("({0},{1},{2},{3},{4}),",item.Writer_Id, item.Product_id, item.Freight_big_area, item.Freight_type,item.Combo_Type);
            }
            return _dbAccess.execCommand(sb.Remove(sb.Length-1, 1).ToString());
        }



        /// <summary>
        /// 查詢符合條件的ProductDeliverySet集合
        /// </summary>
        /// <param name="pds">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        public List<ProductDeliverySetTemp> QueryByProductId(ProductDeliverySetTemp query) 
        {
            string sqlStr = string.Format("SELECT product_id,freight_big_area,freight_type FROM product_delivery_set_temp where product_id ={0} and combo_type={1} and writer_id={2}",
                query.Product_id,query.Combo_Type, query.Writer_Id);
          return _dbAccess.getDataTableForObj<ProductDeliverySetTemp>(sqlStr);
        }

        /// <summary>
        /// 將正式表數據導入臨時表
        /// </summary>
        /// <param name="proDelSetTemp">ProductDeliverySetTemp實體</param>
        /// <returns>sql字符串</returns>
        public string MoveProductDeliverySet(ProductDeliverySetTemp proDelSetTemp)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into product_delivery_set(product_id,freight_big_area,freight_type)"); 
            strSql.Append(" select {0} as product_id,freight_big_area,freight_type from product_delivery_set_temp ");
            strSql.AppendFormat(" where product_id = {0} and combo_type={1} and writer_id ={2}",
            proDelSetTemp.Product_id, proDelSetTemp.Combo_Type, proDelSetTemp.Writer_Id);
            return strSql.ToString();
        }

        public string SaveFromProDeliverySet(ProductDeliverySetTemp proDelSetTemp)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into product_delivery_set_temp(writer_id,product_id,freight_big_area,freight_type,combo_type) ");
            strSql.AppendFormat(" select {0} as writer_id,product_id,freight_big_area,freight_type,{1} as combo_type from product_delivery_set ",proDelSetTemp.Writer_Id,proDelSetTemp.Combo_Type);
            strSql.AppendFormat(" where product_id = {0}", proDelSetTemp.Product_id);
            return strSql.ToString();
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="proDelSetTemp"></param>
        /// <returns>sql字符串</returns>
        public string Delete(ProductDeliverySetTemp proDelSetTemp)
        {
            string strSql= string.Format(@"set sql_safe_updates=0;delete from product_delivery_set_temp
                where writer_id={0} and combo_type={1} and product_id={2};set sql_safe_updates=1;",
                proDelSetTemp.Writer_Id,proDelSetTemp.Combo_Type,proDelSetTemp.Product_id);
            return strSql;
        }
    }
}
