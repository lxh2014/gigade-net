/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：DeliveryFreightSetMappingDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:57:13 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class DeliveryFreightSetMappingDao : IDeliveryFreightSetMappingImplDao
    {
        private IDBAccess _dbAccess;
        public DeliveryFreightSetMappingDao(string connectionStr)
        {
            _dbAccess=DBFactory.getDBAccess(DBType.MySql,connectionStr);
        }

        public DeliveryFreightSetMapping GetDeliveryFreightSetMapping(DeliveryFreightSetMapping query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select product_freight_set,delivery_freight_set from delivery_freight_set_mapping where 1=1");
            if (query.Product_Freight_Set != 0)
            {
                strSql.AppendFormat(" and product_freight_set={0}", query.Product_Freight_Set);
            }
            return _dbAccess.getSinggleObj<DeliveryFreightSetMapping>(strSql.ToString());
        }
    }
}
