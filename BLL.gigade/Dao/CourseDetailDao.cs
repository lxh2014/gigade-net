using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CourseDetailDao : ICourseDetailImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseDetailDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        /// <summary>
        /// 保存單個課程細項信息
        /// </summary>
        /// <param name="c">一個CourseDetail對象</param>
        /// <returns>受影響的行數</returns>
        public string Save(CourseDetail cd)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO course_detail(`course_id`,`course_detail_name`,`address`,`start_date`,`end_date`,`P_Number`)");
                sb.AppendFormat("VALUES({0},'{1}','{2}','{3}','{4}',{5})", cd.Course_Id, cd.Course_Detail_Name, cd.Address, cd.Start_Date.ToString("yyyy-MM-dd HH:mm:ss"), cd.End_Date.ToString("yyyy-MM-dd HH:mm:ss"), cd.P_Number);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailDao-->Save" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢單個課程細項
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public List<CourseDetailCustom> Query(CourseDetail cd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //                sb.Append(@"SELECT cd.course_detail_id,cd.course_detail_name,cd.course_id,cd.address,cd.start_date,cd.end_date,cd.P_Number,n FROM course_detail cd
                //LEFT JOIN (SELECT course_detail_id ,COUNT(ticket_id) AS n FROM course_ticket GROUP BY course_detail_id) ct ON ct.course_detail_id = cd.course_detail_id WHERE 1=1");
//                sb.AppendFormat(" AND course_id = {0}", cd.Course_Id);

                sb.AppendFormat(@"SELECT cd.course_detail_id,cd.course_detail_name,cd.course_id,cd.address,cd.start_date,cd.end_date,cd.P_Number,ct.P_NumberReality 
	                                  FROM course_detail cd 
		                          LEFT JOIN (SELECT cdi.course_detail_id, COUNT(cct.ticket_id) AS P_NumberReality  
				                                 FROM course_detail_item cdi
				                                    INNER JOIN course_ticket cct ON cct.ticket_detail_id = cdi.course_detail_id GROUP BY cdi.course_detail_id) ct
	                                                ON ct.course_detail_id = cd.course_detail_id
                                  WHERE 1=1 AND course_id = {0}", cd.Course_Id);
                
                return _dbAccess.getDataTableForObj<CourseDetailCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailDao-->Query" + ex.Message, ex);
            }
        }

        public List<CourseDetail> QueryModel(CourseDetail cd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {

                sb.Append(@"SELECT cd.course_detail_id,cd.course_detail_name,cd.course_id,cd.address,cd.start_date,cd.end_date
                                  FROM course_detail cd WHERE 1=1");
                if (cd.Course_Id != 0)
                {
                    sb.AppendFormat(" and course_id = {0}", cd.Course_Id);
                }

                return _dbAccess.getDataTableForObj<CourseDetail>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailDao-->Query" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新課程信息
        /// </summary>
        /// <param name="c">更新CourseDetail</param>
        /// <returns>受影響的行數</returns>
        public string Update(CourseDetail cd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0;UPDATE course_detail  SET ");
                if (!string.IsNullOrEmpty(cd.Address))
                {
                    sb.AppendFormat(" address = '{0}',", cd.Address);
                }
                if (cd.Start_Date != DateTime.MinValue)
                {
                    sb.AppendFormat(" start_date = '{0}',", cd.Start_Date.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (cd.End_Date != DateTime.MinValue)
                {
                    sb.AppendFormat(" end_date = '{0}',", cd.End_Date.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (cd.P_Number != 0)
                {
                    sb.AppendFormat(" P_Number = {0},", cd.P_Number);
                }
                if (!string.IsNullOrEmpty(cd.Course_Detail_Name))
                {
                    sb.AppendFormat(" course_detail_name = '{0}',", cd.Course_Detail_Name);
                }
                sb.Append("course_detail_id  =course_detail_id WHERE 1=1");
                if (cd.Course_Id != 0)
                {
                    sb.AppendFormat(" AND course_id = {0}", cd.Course_Id);
                }
                if (cd.Course_Detail_Id != 0)
                {
                    sb.AppendFormat(" AND course_detail_id = {0};SET sql_safe_updates = 1", cd.Course_Detail_Id);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailDao-->Update" + ex.Message, ex);
            }  
        }

        public int Delete(CourseDetail cd, string ids)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("DELETE FROM course_detail WHERE course_detail_id NOT IN({0}) AND course_id = {1} ", ids, cd.Course_Id);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailDao-->Delete" + ex.Message, ex);
            }
        }
    }
}
