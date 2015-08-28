using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICourseDetailItemImplMgr
    {
        /// <summary>
        /// 根據courseId查詢Item_id
        /// </summary>
        /// <param name="courseId">課程id</param>
        /// <returns>符合條件的集合</returns>
        List<CourseDetailItem> Query(int proudctId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>受影響行數</returns>
        bool Save(List<CourseDetailItem> list, List<CourseDetailItem> deleteList);
        string Delete(uint productId);
        bool Delte(int[] delIds);

        /// <summary>
        /// 獲得規格名,詳細課程名
        /// </summary>
        /// <param name="proudctId"></param>
        /// <returns></returns>
        List<CourseDetailItemCustom> QueryName(int proudctId);
    }
}
