using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICourseDetailImplMgr
    {
        /// <summary>
        /// 獲得課程的各類詳細信息
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
        /// 刪除課程信息
        /// </summary>
        /// <param name="cd">CourseDetail對象</param>
        /// <returns>成功:true 失敗 false</returns>
        bool Delete(CourseDetail cd, string ids);
    }
}
