using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface ICourseProductTempImplDao
    {
        CourseProductTemp Query(CourseProductTemp courProd);
        int Save(CourseProductTemp courProd);
        int Update(CourseProductTemp courProd);
        string MoveCourseProduct(CourseProductTemp courProd);
        string DeleteSql(CourseProductTemp courProd);
    }
}
