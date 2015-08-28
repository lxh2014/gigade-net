/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagSetTempMgr 
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
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{

    public class VendorProductTagSetTempMgr : IVendorProductTagSetTempImplMgr
    {
        private IVendorProductTagSetTempImplDao _vendorProductTagSetTempDao;
        public VendorProductTagSetTempMgr(string connectStr)
        {
            _vendorProductTagSetTempDao = new VendorProductTagSetTempDao(connectStr);
        }

        public List<ProductTagSetTemp> Query(ProductTagSetTemp vendorProductTagSetTemp)
        {
            try
            {
                return _vendorProductTagSetTempDao.Query(vendorProductTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorProductTagSetTempMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(ProductTagSetTemp vendorProductTagSetTemp)
        {
            throw new NotImplementedException();
        }

        public string DeleteVendor(ProductTagSetTemp vendorProductTagSetTemp)
        {
            throw new NotImplementedException();
        }

        public string Save(ProductTagSetTemp vendorProductTagSetTemp)
        {
            throw new NotImplementedException();
        }

        public string MoveTag(ProductTagSetTemp vendorProductTagSetTemp)
        {
            throw new NotImplementedException();
        }

        public string SaveFromTag(ProductTagSetTemp vendorProductTagSetTemp)
        {
            throw new NotImplementedException();
        }
    }
}
