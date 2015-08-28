using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class VendorCateSetMgr : IVendorCateSetImplMgr
    {
        private IVendorCateSetImplDao _VendorCateSetDao;
        public VendorCateSetMgr(string connectionStr)
        {
            _VendorCateSetDao = new VendorCateSetDao(connectionStr);
        }

        public string GetMaxCodeSerial(VendorCateSet vcs)
        {
            try
            {

                return _VendorCateSetDao.GetMaxCodeSerial(vcs);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorCateSetMgr-->GetMaxCodeSerial-->" + ex.Message, ex);

            }
        }
    }
}

