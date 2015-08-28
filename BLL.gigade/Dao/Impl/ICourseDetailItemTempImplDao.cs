using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface ICourseDetailItemTempImplDao
    {
        List<CourseDetailItemTemp> Query(int writerId, int productId);
        int Add(List<CourseDetailItemTemp> list);
        int Update(List<CourseDetailItemTemp> list);
        int Delete(int writerId);
        int Delete(int[] ids);
        string MoveCourseDetailItem(int writerId);
        string DeleteSql(int writerId);
    }
}
