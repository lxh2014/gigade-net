using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface ICourseDetailItemImplDao
    {
        /// <summary>
        /// 查詢課程對應的item_id
        /// </summary>
        /// <param name="proudctId">商品id</param>
        /// <returns>list集合</returns>
        List<CourseDetailItem> Query(int proudctId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="list">List<CourseDetailItem></param>
        /// <returns>受影響的行數</returns>
        int Add(List<CourseDetailItem> list);
        string Delete(uint productId);
        int Delete(int[] delIds);
        int Update(List<CourseDetailItem> list);

        /// <summary>
        /// 獲得規格名,詳細課程名
        /// </summary>
        /// <param name="proudctId"></param>
        /// <returns></returns>
        List<CourseDetailItemCustom> QueryName(int proudctId);
    }
}
