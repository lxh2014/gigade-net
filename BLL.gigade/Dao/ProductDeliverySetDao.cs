//add by wangwei0216w 2014/11/03
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ProductDeliverySetDao:IProductDeliverySetImplDao
    {
        private IDBAccess _dbAccess;
        private string strConn;
        public ProductDeliverySetDao(string connectStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectStr);
            this.strConn = connectStr;
        }
        
        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="sc">一個ProductDeliverySet對象</param>
        /// <param name="delProdId">需先删除的商品id</param>
        /// <returns>受影響的行數</returns>
        public int Save(List<ProductDeliverySet> proDeliSets,int delProdId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("set sql_safe_updates=0;delete from product_delivery_set where product_id={0};set sql_safe_updates=1;", delProdId);
            sb.Append("INSERT INTO product_delivery_set(`product_id`,`freight_big_area`,`freight_type`) values ");
            foreach (var item in proDeliSets)
            {
                sb.AppendFormat("({0},{1},{2}),", item.Product_id, item.Freight_big_area, item.Freight_type);
            }
            return _dbAccess.execCommand(sb.Remove(sb.Length-1, 1).ToString());
        }


        /// <summary>
        /// 查詢符合條件的ProductDeliverySet集合
        /// </summary>
        /// <param name="pds">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        public List<ProductDeliverySet> QueryByProductId(int productId)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb = sb.AppendFormat(" SELECT product_id,freight_big_area,freight_type FROM product_delivery_set where product_id ={0}", productId);
                return _dbAccess.getDataTableForObj<ProductDeliverySet>(sb.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("ProductDao.QueryByProductId-->" + ex.Message, ex);
            }
        }

        public int Delete(ProductDeliverySet pds,uint[] productIds)
        {
            try
            {
                if (productIds.Length == 0) return 0;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"set sql_safe_updates=0; DELETE FROM product_delivery_set WHERE product_id in ({0}) and freight_big_area={1} AND  freight_type = {2};set sql_safe_updates=1;"
                    ,string.Join(",",productIds), pds.Freight_big_area, pds.Freight_type);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.DeleteProduct_Delivery-->" + ex.Message, ex);
            }
        }

        public List<ProductDeliverySet> Query(uint[] productIds,ProductDeliverySet deliverySet)
        {
            try
            {
                if (productIds.Length == 0) return new List<ProductDeliverySet>();
                string prodIds = "";
                foreach (uint prodId in productIds)
                {
                    prodIds += prodId + ",";
                }
                string strSql = string.Format("select distinct product_id,freight_big_area,freight_type from product_delivery_set where freight_big_area={0} and freight_type={1} and product_id in({2});",deliverySet.Freight_big_area,deliverySet.Freight_type,prodIds.Remove(prodIds.Length - 1));
                return _dbAccess.getDataTableForObj<ProductDeliverySet>(strSql);

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Query-->" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/1/12
        /// <summary>
        /// 根據條件獲得相關product物流設定的信息
        /// </summary>
        /// <param name="deliverySet">Condition</param>
        /// <returns>List<ProductDeliverySetCustom></returns>
        public List<ProductDeliverySetCustom> QueryProductDeliveryByCondition(ProductDeliverySet deliverySet)
        {
            List<ProductDeliverySetCustom> list = new List<ProductDeliverySetCustom>();
            try
            {
                StringBuilder sb = new StringBuilder();
                //添加 品牌  Brand_name   add by zhuoqin0830w  2015/04/24
                //LEFT JOIN (SELECT parameterCode,parameterName,remark,topValue,kdate,kuser FROM t_parametersrc WHERE parameterType='freight_type') t ON t.parameterCode = pd.freight_type 
                sb.Append(@"SELECT pd.product_id,p.product_name,vb.brand_name AS Brand_name,pd.freight_type,pd.freight_type AS Freight_type_name FROM product_delivery_set pd INNER JOIN product p ON pd.product_id = p.product_id");
                sb.Append(@" LEFT JOIN vendor_brand vb ON p.brand_id = vb.brand_id WHERE 1=1 ");
                if (deliverySet.Freight_big_area != 0 && deliverySet.Freight_type != 0)
                {
                    sb.AppendFormat(" AND pd.freight_big_area={0} AND pd.freight_type = {1}", deliverySet.Freight_big_area, deliverySet.Freight_type);
                    //edit by zhuoqin0830w  2015/05/18
                    IParametersrcImplDao _parameterDao = new ParametersrcDao(strConn);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("freight_type");
                    list = _dbAccess.getDataTableForObj<ProductDeliverySetCustom>(sb.ToString());
                    foreach (ProductDeliverySetCustom q in list)
                    {
                        var alist = parameterList.Find(m => m.ParameterType == "freight_type" && m.ParameterCode == q.Freight_type.ToString());
                        if (alist != null)
                        {
                            q.Freight_type_name = alist.parameterName;
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.QueryProductDeliveryByCondition-->" + ex.Message, ex);
            }
        }
    }
    
}
