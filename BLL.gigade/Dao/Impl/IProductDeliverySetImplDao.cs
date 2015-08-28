using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    interface IProductDeliverySetImplDao
    {
        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="pds">一個ProductDeliverySet對象</param>
        /// <returns>受影響的行數</returns>
        int Save(List<ProductDeliverySet> pds,int delProductId);

        /// <summary>
        /// 查詢符合條件的ProductDeliverySet集合
        /// </summary>
        /// <param name="pds">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        List<ProductDeliverySet> QueryByProductId(int productId);

        int Delete(ProductDeliverySet pds,uint[] productIds);

        /// <summary>
        /// 根據一組商品id查詢物流配送模式
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        List<ProductDeliverySet> Query(uint[] productIds,ProductDeliverySet deliverySet);

        //add by wwei0216w 2015/1/12
        /// <summary>
        /// 根據條件獲得相關product物流設定的信息
        /// </summary>
        /// <param name="deliverySet">Condition</param>
        /// <returns>List<ProductDeliverySetCustom></returns>
        List<ProductDeliverySetCustom> QueryProductDeliveryByCondition(ProductDeliverySet deliverySet);
    }
}
