using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface ICourseTicketImplDao
    {
        /// <summary>
        /// 查詢售賣票據信息
        /// </summary>
        /// <param name="courseId">courseId</param>
        /// <returns>List</returns>
        List<CourseTicketCustom> Query(int course_detail_id);

        List<CourseTicket> Query(CourseTicket query);

        /// <summary>
        /// 保存單個課程細項信息
        /// </summary>
        /// <param name="c">一個CourseDetail對象</param>
        /// <returns>受影響的行數</returns>
        string Save(CourseTicket ct);


        /// <summary>
        /// 票券核銷列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetTicketCode(CourseTicket query, out int totalCount);
        /// <summary>
        /// 執行核銷動作
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user_type">變更人類型 1  吉甲地管理員 2供應商</param>
        /// <returns></returns>
        bool TicketVerification(CourseTicket store, int user_type);
    }
}
