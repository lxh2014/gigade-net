using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface ICourseImplDao
    {
         /// <summary>
        /// 保存課程
        /// </summary>
        /// <param name="c">一個Course對象</param>
        /// <returns>受影響的行數</returns>
        int Save(Course c);

        /// <summary>
        /// 查詢課程信息
        /// </summary>
        /// <param name="c">一個Course對象,裏面是查詢的條件</param>
        /// <returns>符合條件的List集合</returns>
        List<Course> Query(Course c, out int totalCount);

        /// <summary>
        /// 更新課程表信息
        /// </summary>
        /// <param name="c">一個Course對象,裏面包含更改信息</param>
        /// <returns>受影響的行數</returns>
        int Update(Course c);

        /// <summary>
        /// 保存所有語句
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool SaveAll(ArrayList list);

        List<Course> Query(Course c);
    }
}
