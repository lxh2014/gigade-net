/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：VendorProductTagSetMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：shiwei0620j
 * 完成日期：
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
    public class VendorProductTagSetMgr : IVendorProductTagSetImplMgr
    {
        private IVendorProductTagSetImplDao _vendorProductTagSetDao;
        public VendorProductTagSetMgr(string connectionStr)
        {
            _vendorProductTagSetDao = new VendorProductTagSetDao(connectionStr);
        }
        public List<Model.VendorProductTagSet> Query(Model.VendorProductTagSet vendorProductTagSet)
        {
            try
            {
                return _vendorProductTagSetDao.Query(vendorProductTagSet);
            }
            catch (Exception ex)
            {
                throw new Exception("vendorProductTagSetMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(Model.VendorProductTagSet vendorProductTagSet)
        {
            try
            {
                return _vendorProductTagSetDao.Delete(vendorProductTagSet);
            }
            catch (Exception ex)
            {
                throw new Exception("vendorProductTagSetMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string Save(Model.VendorProductTagSet vendorProductTagSet)
        {
            throw new NotImplementedException();
        }

        public string SaveFromOtherPro(Model.VendorProductTagSet vendorProductTagSet)
        {
            throw new NotImplementedException();
        }
    }
}
