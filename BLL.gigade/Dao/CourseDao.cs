using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CourseDao : ICourseImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        /// <summary>
        /// 保存課程
        /// </summary>
        /// <param name="c">一個Course對象</param>
        /// <returns>受影響的行數</returns>
        public int Save(Course c)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("INSERT INTO course(`course_name`,`tel`,`send_msg`,`msg`,`send_mail`,`mail_content`,`start_date`,`end_date`,`create_time`,`source`,`ticket_type`)");
                sb.AppendFormat("VALUES('{0}','{1}',{2},'{3}',{4},'{5}',",c.Course_Name,c.Tel,c.Send_Msg,c.Msg,c.Send_Mail,c.Mail_Content);
                sb.AppendFormat("'{0}','{1}','{2}',{3},{4});SELECT @@identity", c.Start_Date.ToString("yyyy-MM-dd HH:mm:ss"), c.End_Date.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), c.Source, c.Ticket_Type);
                return Convert.ToInt32(_dbAccess.getDataTable(sb.ToString()).Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Save" + ex.Message,ex);
            }
        }

        public List<Course> Query(Course c,out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();//add by wwei0216w 2015/4/7 添加查詢總數的sql語句
            try
            {
                sb.Append("SELECT course_id,course_name,tel,send_msg,msg,send_mail,mail_content,start_date,end_date,create_time,source,ticket_type FROM course WHERE 1=1 ");
                sb2 = sb2.Append("SELECT COUNT(course_id) AS totalCount FROM course WHERE 1=1 ");
                if(c.Course_Id !=0)
                {
                    sb.AppendFormat(" AND course_id = {0}", c.Course_Id);
                    sb2.AppendFormat(" AND course_id = {0}", c.Course_Id);
                }
                if(!string.IsNullOrEmpty(c.Course_Name))
                {
                    sb.AppendFormat(" AND course_name like '%{0}%'",c.Course_Name);
                    sb2.AppendFormat(" AND course_name like '%{0}%'", c.Course_Name);
                }

                sb.AppendFormat(" ORDER BY create_time DESC limit {0},{1}", c.Start, c.Limit);
                sb2.Append("  ORDER BY create_time DESC "); ///edit by wwei0216w 2015/7/31 條換order by 的位置 修改mysql語法報錯的問題

                System.Data.DataTable _dt = _dbAccess.getDataTable(sb2.ToString());
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }

                return _dbAccess.getDataTableForObj<Course>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Query" + ex.Message, ex);
            }
        }

        public List<Course> Query(Course c)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT course_id,course_name,tel,send_msg,msg,send_mail,mail_content,start_date,end_date,create_time,source,ticket_type FROM course WHERE 1=1");
                if (c.Course_Id != 0)
                {
                    sb.AppendFormat(" AND course_id = {0}", c.Course_Id);
                }
                if (!string.IsNullOrEmpty(c.Course_Name))
                {
                    sb.AppendFormat(" AND course_name like '%{0}%'", c.Course_Name);
                }

                return _dbAccess.getDataTableForObj<Course>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Query" + ex.Message, ex);
            }
        }

        public int Update(Course c)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SET sql_safe_updates = 0;UPDATE course c  SET ");

                sb.AppendFormat(" c.msg = '{0}',", c.Msg);

                sb.AppendFormat(" c.mail_content = '{0}',", c.Mail_Content);

                if (!string.IsNullOrEmpty(c.Course_Name))
                {
                    sb.AppendFormat(" c.course_name = '{0}',", c.Course_Name);
                }
                if (!string.IsNullOrEmpty(c.Tel))
                {
                    sb.AppendFormat(" c.tel = '{0}',", c.Tel);
                }
                if(c.Start_Date != DateTime.MinValue)
                {
                    sb.AppendFormat(" c.start_date = '{0}',", c.Start_Date.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if(c.End_Date != DateTime.MinValue)
                {
                    sb.AppendFormat(" c.end_date = '{0}',", c.End_Date.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (c.Send_Msg != -1)
                {
                    sb.AppendFormat(" c.send_msg = {0},", c.Send_Msg);
                }

                if(c.Send_Mail!=-1)
                {
                    sb.AppendFormat(" c.send_mail = {0},", c.Send_Mail);
                }

                if(c.Source!=-1)
                {
                    sb.AppendFormat(" c.source = {0},", c.Source);
                }
                if(c.Ticket_Type != -1)
                {
                    sb.AppendFormat(" c.ticket_type = {0},", c.Ticket_Type);
                }

                sb.Append(" c.course_id = c.course_id,");

                string strSql = sb.ToString().Remove(sb.ToString().Length-1,1);

                string strWhere = string.Format(" WHERE c.course_id = {0}", c.Course_Id);
                strSql += strWhere;
                strSql += "; SET sql_safe_updates = 1;";

                return _dbAccess.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Update" + ex.Message, ex);
            }    
        }

        public bool SaveAll(ArrayList list)
        {
            MySqlDao excuteDao = new MySqlDao(connStr);
            return excuteDao.ExcuteSqls(list);
        }
    }
}
