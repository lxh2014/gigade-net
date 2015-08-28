/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：PriceUpdateApplyMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/6 13:24:18 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class PriceUpdateApplyMgr : IPriceUpdateApplyImplMgr
    {
        private IPriceUpdateApplyImplDao _priceUpdateApplyDao;
        public PriceUpdateApplyMgr(string connectionStr)
        {
            _priceUpdateApplyDao = new PriceUpdateApplyDao(connectionStr);
        }

        public int Save(Model.PriceUpdateApply priceUpdateApply)
        {
            try
            {
                return _priceUpdateApplyDao.Save(priceUpdateApply);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceUpdateApplyMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
            
        }
    }
}
