using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class CourseDetailItemTempMgr : ICourseDetailItemTempImplMgr
    {
        private Dao.Impl.ICourseDetailItemTempImplDao _courseItemTempDao;
        private string conStr = "";
        public CourseDetailItemTempMgr(string connectionString)
        {
            _courseItemTempDao = new Dao.CourseDetailItemTempDao(connectionString);
            conStr = connectionString;
        }

        public List<CourseDetailItemTemp> Query(int writerId, int productId)
        {
            return _courseItemTempDao.Query(writerId,productId);
        }

        public bool Save(List<CourseDetailItemTemp> list,List<CourseDetailItemTemp> delList)
        {
            bool result = true;
            int[] deleteIds = delList.Select(c => c.Course_Detail_Item_Id).ToArray();
            var updateList = list.FindAll(c => c.Course_Detail_Item_Id > 0);
            var addList = list.FindAll(c => c.Course_Detail_Item_Id == 0);

            if (deleteIds.Length > 0)
            {
                result = _courseItemTempDao.Delete(deleteIds) > -1;
            }

            if (updateList.Count > 0)
            {
                result = _courseItemTempDao.Update(updateList)>-1;
            }

            if (addList.Count > 0)
            {
                result = _courseItemTempDao.Add(addList)>0;
            }
            return result;
        }

        public bool Delete(int writerId)
        {
            return _courseItemTempDao.Delete(writerId) > -1;
        }

        public string MoveCourseDetailItem(int writerId)
        {
            return _courseItemTempDao.MoveCourseDetailItem(writerId);
        }
        public string DeleteSql(int writerId)
        {
            return _courseItemTempDao.DeleteSql(writerId);
        }
    }
}
