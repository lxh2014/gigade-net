using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CousePictureDao : ICoursePictureImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CousePictureDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public string Save(CoursePicture cp)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO `course_picture`(`course_id`,`picture_name`,`picture_type`,`picture_status`,`picture_sort`) VALUES({0},'{1}','{2}',{3},{4});", cp.course_id, cp.picture_name, cp.picture_type, cp.picture_status, cp.picture_sort);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Save" + ex.Message, ex);
            }
        }

        public List<CoursePicture> Query(CoursePicture cp)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT `id`,`course_id`,`picture_name`,`picture_type`,`picture_status`,`picture_sort` FROM course_picture WHERE course_id = {0}",cp.course_id);
                return _dbAccess.getDataTableForObj<CoursePicture>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Query" + ex.Message, ex);
            }
        }

        public string Delete(CoursePicture cp)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SET sql_safe_updates = 0; DELETE FROM course_picture WHERE course_id = {0};SET sql_safe_updates = 1;", cp.course_id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Delete" + ex.Message, ex);
            }
        }
    }
}
