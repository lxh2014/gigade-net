using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICourseImplMgr
    {
        /// <summary>
        /// 保存課程信息
        /// </summary>
        /// <param name="c">Course對象</param>
        /// <returns>被插入數據庫的數據條數</returns>
        int Save(Course c);

        /// <summary>
        /// 查詢課程信息
        /// </summary>
        /// <param name="c">Course對象</param>
        /// <returns>list</returns>
        List<Course> Query(Course c, out int totalCount);

        /// <summary>
        /// 保存所有
        /// </summary>
        /// <param name="list">需要執行的sql語句集合</param>
        /// <returns>true Or false</returns>
        bool ExecuteAll(Course c, List<CourseDetail> cdList, List<CoursePicture> plist);

        List<Course> Query(Course c);//add by wwei0216w 添加不需要分頁的查詢

        List<string> GetSqlByPicture(List<CoursePicture> plist, int course_id);
    }
}
