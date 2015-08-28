using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class CourseDetailItemMgr : ICourseDetailItemImplMgr
    {
        private Dao.Impl.ICourseDetailItemImplDao _courseItemMgr;
        private string conStr = "";
        public CourseDetailItemMgr(string connectionString)
        {
            _courseItemMgr = new Dao.CourseDetailItemDao(connectionString);
            conStr = connectionString;
        }

        public List<CourseDetailItem> Query(int proudctId)
        {
            return _courseItemMgr.Query(proudctId);
        }

        public bool Save(List<CourseDetailItem> list, List<CourseDetailItem> deleteList)
        {
            bool result = true;
            var delIds = deleteList.Select(c => c.Course_Detail_Item_Id).ToArray();
            var updateList = list.FindAll(c => c.Course_Detail_Item_Id != 0);//修改
            var addList = list.FindAll(c => c.Course_Detail_Item_Id == 0);//新增

            if (delIds.Length > 0)
            {
                result = _courseItemMgr.Delete(delIds) > -1;
            }

            if (updateList.Count > 0)
            {
                result = _courseItemMgr.Update(updateList) > -1;
            }
            if (addList.Count > 0)
            {
                result = _courseItemMgr.Add(addList) > 0;
            }
            return result;
        }

        public string Delete(uint productId)
        {
            return _courseItemMgr.Delete(productId);
        }

        public bool Delte(int[] delIds)
        {
            return _courseItemMgr.Delete(delIds) > -1;
        }

        /// <summary>
        /// 獲得規格名,詳細課程名
        /// </summary>
        /// <param name="proudctId"></param>
        /// <returns></returns>
        public List<CourseDetailItemCustom> QueryName(int proudctId)
        {
            try
            {
                return _courseItemMgr.QueryName(proudctId);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemMgr--QueryName" +ex.Message,ex);
            }
        }
    }
}
