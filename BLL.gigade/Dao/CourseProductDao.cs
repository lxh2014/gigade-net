using BLL.gigade.Dao.Impl;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CourseProductDao : ICourseProductImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseProductDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        public string Delete(uint productId)
        {
            return string.Format("set sql_safe_updates = 0;delete from course_product where product_id = {0};set sql_safe_updates = 1;", productId);
        }
    }
}
