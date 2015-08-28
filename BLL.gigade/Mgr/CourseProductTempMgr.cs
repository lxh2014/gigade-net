using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class CourseProductTempMgr:ICourseProductTempImplMgr
    {
        private Dao.Impl.ICourseProductTempImplDao _iptDao;
        private string conStr = "";
        public CourseProductTempMgr(string connectionString)
        {
            _iptDao = new Dao.CourseProductTempDao(connectionString);
            conStr = connectionString;
        }

        public CourseProductTemp Query(CourseProductTemp courProd)
        {
           return _iptDao.Query(courProd);
        }

        public bool Save(CourseProductTemp courProd)
        {
            return _iptDao.Save(courProd) > 0;
        }
        public bool Update(CourseProductTemp courProd)
        {
            return _iptDao.Update(courProd) > 0;
        }

        public  string MoveCourseProduct(CourseProductTemp courProd)
        {
            return _iptDao.MoveCourseProduct(courProd);
        }
        public string DeleteSql(CourseProductTemp courProd)
        {
            return _iptDao.DeleteSql(courProd);
        }
    }
}
