using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface ICourseDetailImplDao
    {
        /// <summary>
        /// 查詢和課程相關的各類信息
        /// </summary>
        /// <param name="cd">CourseDetail對象</param>
        /// <returns>符合條件的集合</returns>
        List<CourseDetailCustom> Query(CourseDetail cd);

        /// <summary>
        /// 保存單個課程細項信息
        /// </summary>
        /// <param name="c">一個CourseDetail對象</param>
        /// <returns>受影響的行數</returns>
        string Save(CourseDetail cd);

        /// <summary>
        /// 更新課程信息
        /// </summary>
        /// <param name="c">更新CourseDetail</param>
        /// <returns>受影響的行數</returns>
        string Update(CourseDetail cd);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="cd">CourseDetail對象</param>
        /// <returns>受影響行數</returns>
        int Delete(CourseDetail cd,string ids);
    }
}
