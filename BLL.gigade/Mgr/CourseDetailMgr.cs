using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class CourseDetailMgr : ICourseDetailImplMgr
    {
        private Dao.Impl.ICourseDetailImplDao _courseDetail;
        private string connstr = "";

        public CourseDetailMgr(string connectionString)
        {
            _courseDetail = new Dao.CourseDetailDao(connectionString);
            connstr = connectionString;
        }

        public List<CourseDetailCustom> Query(CourseDetail cd)
        {
            return _courseDetail.Query(cd);
        }

        public string Save(CourseDetail cd)
        {
            return _courseDetail.Save(cd);
        }

        /// <summary>
        /// 更新課程信息
        /// </summary>
        /// <param name="c">更新CourseDetail</param>
        /// <returns>受影響的行數</returns>
        public string Update(CourseDetail cd)
        {
            return _courseDetail.Update(cd);
        }

        public bool Delete(CourseDetail cd,string ids)
        {
            CourseTicketMgr ct = new CourseTicketMgr(connstr);
            StringBuilder sb = new StringBuilder();
            try
            {
                if (ids!="")
                {
                     return _courseDetail.Delete(cd, ids) >= 0;
                }
                else
                {
                    return true;
                }   
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailMgr-->Delete" + ex.Message,ex);
            }
        }
    }
}
