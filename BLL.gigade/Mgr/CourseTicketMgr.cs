using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class CourseTicketMgr : ICourseTicketImplMgr
    {
        private Dao.Impl.ICourseTicketImplDao _courseTicketDao;
        private string _conn;
        public CourseTicketMgr(string connectionString)
        {
            _courseTicketDao = new CourseTicketDao(connectionString);
            _conn = connectionString;
        }

        public List<CourseTicketCustom> Query(int course_detail_id, string xmlPath)
        {
            try
            {
                return _courseTicketDao.Query(course_detail_id);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketMgr-->Query" + ex.Message, ex);
            }
        }
        public List<CourseTicket> Query(CourseTicket store)
        {
            try
            {
                return _courseTicketDao.Query(store);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketMgr-->Query" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 保存單個課程細項信息
        /// </summary>
        /// <param name="c">一個CourseDetail對象</param>
        /// <returns>受影響的行數</returns>
        public string Save(CourseTicket ct)
        {
            return _courseTicketDao.Save(ct);
        }


     
        public DataTable GetTicketCode(CourseTicket query, out int totalCount)
        {
            try
            {
                DataTable _dt = _courseTicketDao.GetTicketCode(query, out totalCount);
                _dt.Columns.Add("course_name");
                _dt.Columns.Add("spec_name_1");
                _dt.Columns.Add("spec_name_2");
                _dt.Columns.Add("start_date");
                _dt.Columns.Add("end_date");
                if (_dt != null)
                {
                    CourseDao _courseDao = new CourseDao(_conn);
                    ProductSpecDao _psDao = new ProductSpecDao(_conn);
                    CourseDetailDao _cdDao = new CourseDetailDao(_conn);
                    foreach (DataRow item in _dt.Rows)
                    {
                        item["course_name"] = _courseDao.Query(new Course { Course_Id = Convert.ToInt32(item["course_id"]) }).FirstOrDefault().Course_Name;
                        item["spec_name_1"] = _psDao.query(Convert.ToInt32(item["spec_id_1"])).spec_name;
                        item["spec_name_2"] = _psDao.query(Convert.ToInt32(item["spec_id_2"])).spec_name;
                        CourseDetail store = _cdDao.QueryModel(new CourseDetail { Course_Id = Convert.ToInt32(item["course_id"]) }).FirstOrDefault();
                        item["start_date"] = store.Start_Date.ToString();
                        item["end_date"] = store.End_Date.ToString();

                    }
                }
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketMgr-->GetTicketCode" + ex.Message, ex);
            }
        }
        public bool TicketVerification(CourseTicket store, int user_type)
        {
            try
            {
                return _courseTicketDao.TicketVerification(store, user_type);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketMgr-->TicketVerification" + ex.Message, ex);
            }
        }
    }
}
