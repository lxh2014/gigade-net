using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICourseProductTempImplMgr
    {
        CourseProductTemp Query(CourseProductTemp courProd);
        bool Save(CourseProductTemp courProd);
        bool Update(CourseProductTemp courProd);
        string MoveCourseProduct(CourseProductTemp courProd);
        string DeleteSql(CourseProductTemp courProd);
    }
}
