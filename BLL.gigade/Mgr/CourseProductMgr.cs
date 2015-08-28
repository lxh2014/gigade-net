using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class CourseProductMgr:ICourseProductImplMgr
    {
        private Dao.Impl.ICourseProductImplDao _ipDao;
        private string conStr = "";
        public CourseProductMgr(string connectionString)
        {
            _ipDao = new Dao.CourseProductDao(connectionString);
            conStr = connectionString;
        }

        public string Delete(uint productId)
        {
            return _ipDao.Delete(productId);
        }
    }
}
