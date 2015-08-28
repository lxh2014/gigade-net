using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class CourseMgr:ICourseImplMgr
    {
        private Dao.Impl.ICourseImplDao _courseImpl;
        private string conStr = "";
        public CourseMgr(string connectionString)
        {
            _courseImpl = new Dao.CourseDao(connectionString);
            conStr = connectionString;
        }

        public int Save(Course c)
        {
            return _courseImpl.Save(c);
        }

        public List<Course> Query(Course c, out int totalCount)
        {
            return _courseImpl.Query(c,out totalCount);
        }

        public List<Course> Query(Course c)//add by wwei0216w 2015/04/10  添加一個無需分頁的查詢
        {
            return _courseImpl.Query(c);
        }

        public int Update(Course c)
        {
            return _courseImpl.Update(c);
        }

        

        public bool ExecuteAll(Course c, List<CourseDetail> cdList,List<CoursePicture> plist)
        {
            ArrayList list = new ArrayList();
            try
            {
                int course_id = GetCourseId(c);
                //獲得操作Course表的sql語句
                List<string> listStr = GetSqlByCourseDetail(cdList, course_id);
                //獲取操作圖片的sql語句
                List<string> plistStr = GetSqlByPicture(plist, course_id);
                if (listStr.Count != 0)
                {
                    list.AddRange(listStr);
                }
                if (plistStr.Count != 0)
                {
                    list.AddRange(plistStr);
                }
                if (list.Count != 0)
                {
                    return _courseImpl.SaveAll(list);
                }
                else
                {
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("CourseMgr-->SaveAll" + ex.Message,ex);
            }
        }


        public int GetCourseId(Course c)
        {
            if (c==null)
                return 0;
            if (c.Course_Id == 0)
            {
                return Save(c);
            }
            else
            {
                Update(c);
                return c.Course_Id; //得到course_id的值,用於後面課程細項的新增和更新
            }
        }

        public List<string> GetSqlByCourseDetail(List<CourseDetail> cdList, int courseId)
        {
            ICourseDetailImplMgr _detailMgr = new CourseDetailMgr(conStr);
            List<string> list = new List<string>();
            if (cdList == null)
                return null;
            int[] cdids = (from c in cdList where c.Course_Detail_Id != 0 select c.Course_Detail_Id).ToArray();
            string ids = string.Join(",", cdids);
            if (_detailMgr.Delete(cdList[0], ids))
            {
                foreach (CourseDetail cd in cdList)
                {
                    if (cd.Course_Id == 0)
                    {
                        cd.Course_Id = courseId;
                    }
                    if (cd.Course_Detail_Id == 0)
                    {
                        list.Add(_detailMgr.Save(cd));
                    }
                    else
                    {
                        list.Add(_detailMgr.Update(cd));
                    }
                }
            }
            return list;
        }

        public List<string> GetSqlByPicture(List<CoursePicture> plist,int course_id)
        {
            List<string> list = new List<string>();
            ICoursePictureImplMgr _p = new CoursePictureMgr(conStr);
            CoursePicture c = new CoursePicture();
            c.course_id = course_id;
            list.Add(_p.Delete(c));
            try
            {
                foreach (CoursePicture item in plist)
                {
                    item.course_id = course_id;
                    list.Add(_p.Save(item));
                }
                
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("CourseMgr-->GetSqlByPicture" + ex, ex);
            }
        }
    }
}
