using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICourseDetailItemTempImplMgr
    {
        /// <summary>
        /// 查詢Course_Detail_Item_Temp臨時表
        /// </summary>
        /// <param name="productId">商品Id</param>
        /// <returns>List</returns>
        List<CourseDetailItemTemp> Query(int writerId,int productId);

        /// <summary>
        /// 保存數據到Course_Detail_Item_Temp臨時表
        /// </summary>
        /// <param name="list">需求保存的數據集合</param>
        /// <returns>true or false</returns>
        bool Save(List<CourseDetailItemTemp> list,List<CourseDetailItemTemp> deleteList);

        bool Delete(int writerId);

        string MoveCourseDetailItem(int writerId);

        string DeleteSql(int writerId);
    }
}
