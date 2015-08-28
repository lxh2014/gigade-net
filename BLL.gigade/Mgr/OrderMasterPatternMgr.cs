/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderMasterPatternMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/02/26
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    class OrderMasterPatternMgr : IOrderMasterPatternImplMgr
    {
        private IOrderMasterPatternImplDao _orderMasterPatternDao;
        public OrderMasterPatternMgr(string connectionStr)
        {
            _orderMasterPatternDao = new OrderMasterPatternDao(connectionStr);
        }

        #region 公關單與報廢單功能  + Save(OrderMasterPattern op)
        /// <summary>
        /// 保存 公關單與報廢單 中 新增加 的數據   add by zhuoqin0830w  2015/02/26
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public string Save(OrderMasterPattern op)
        {
            try
            {
                return _orderMasterPatternDao.Save(op);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterPatternMgr-->Save-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
