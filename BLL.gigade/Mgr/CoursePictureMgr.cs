using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class CoursePictureMgr : ICoursePictureImplMgr
    {
        ICoursePictureImplDao _ICorsePictureDao;
        private string conStr = "";
        public CoursePictureMgr(string connectionStr)
        {
            _ICorsePictureDao = new CousePictureDao(connectionStr);
            conStr = connectionStr;
        }

        public string Save(CoursePicture c)
        {
            try
            {
                return _ICorsePictureDao.Save(c);
            }
            catch (Exception ex)
            {
                throw new Exception("CoursePictureMgr-->Save" + ex.Message, ex);
            }
        }

        public List<CoursePicture> Query(CoursePicture cp)
        {
            try
            {
                return _ICorsePictureDao.Query(cp);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Query" + ex.Message, ex);
            }
        }

        public string Delete(CoursePicture cp)
        {
            try
            {
                return _ICorsePictureDao.Delete(cp);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDao-->Query" + ex.Message, ex);
            }    
        }
    }
}
