/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：DeliveryFreightSetMappingMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:57:56 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class DeliveryFreightSetMappingMgr : IDeliveryFreightSetMappingImplMgr
    {
        private IDeliveryFreightSetMappingImplDao _deliveryFreightSetMappingDap;
        public DeliveryFreightSetMappingMgr(string connectionStr)
        {
            _deliveryFreightSetMappingDap = new DeliveryFreightSetMappingDao(connectionStr);
        }

        public DeliveryFreightSetMapping GetDeliveryFreightSetMapping(DeliveryFreightSetMapping query)
        {
            try
            {
                return _deliveryFreightSetMappingDap.GetDeliveryFreightSetMapping(query);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryFreightSetMappingMgr-->GetDeliveryFreightSetMapping-->" + ex.Message, ex);
            }
            
        }
    }
}
