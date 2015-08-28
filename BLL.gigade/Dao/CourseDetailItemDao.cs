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
    public class CourseDetailItemDao : ICourseDetailItemImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseDetailItemDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public List<CourseDetailItem> Query(int proudctId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT cdi.course_detail_item_id,cdi.course_detail_id,cdi.item_id,cdi.people_count 
                                  FROM product_item pi 
                                      INNER JOIN course_detail_item cdi on pi.item_id=cdi.item_id
                                  WHERE pi.product_id={0} group by cdi.course_detail_item_id;", proudctId);
                return _dbAccess.getDataTableForObj<CourseDetailItem>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemDao-->Query" + ex.Message,ex);
            }
        }

        public int Add(List<CourseDetailItem> list)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("INSERT INTO course_detail_item(`course_detail_id`,`item_id`,`people_count`)VALUES");
                foreach (CourseDetailItem cdi in list)
                {
                    sb.AppendFormat("({0},{1},{2}),",cdi.Course_Detail_Id,cdi.Item_Id,cdi.People_Count);
                }
                string strSql = sb.ToString().Remove(sb.ToString().Length - 1, 1);
                return _dbAccess.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemDao-->Save" + ex.Message,ex);
            }
        }

        public int Update(List<CourseDetailItem> list)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                foreach (var item in list)
                {
                    sqlStr.AppendFormat("update course_detail_item set course_detail_id={0},item_id={1},people_count={2} where course_detail_item_id={3};", item.Course_Detail_Id, item.Item_Id,item.People_Count, item.Course_Detail_Item_Id);
                }
                return _dbAccess.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemDao-->Update" + ex.Message, ex);
            }
        }

        public string Delete(uint productId)
        {
            return string.Format("set sql_safe_updates=0;delete from course_detail_item where item_id in (select item_id from product_item where product_id={0});set sql_safe_updates=1;", productId);
        }

        public int Delete(int[] delIds)
        {
            string strSql = string.Format("delete from course_detail_item where course_detail_item_id in ({0});",
                string.Join(",", delIds));
            return _dbAccess.execCommand(strSql);
        }

        /// <summary>
        /// 獲得規格名,詳細課程名
        /// </summary>
        /// <param name="proudctId"></param>
        /// <returns></returns>
        public List<CourseDetailItemCustom> QueryName(int proudctId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT cdi.course_detail_item_id,cdi.course_detail_id,cdi.item_id,cd.course_detail_name,
                                         ps1.spec_name AS spec_name1,ps2.spec_name AS spec_name2 
                                  FROM course_detail_item cdi 
                                      INNER JOIN course_detail cd ON cd.course_detail_id = cdi.course_detail_id
                                      INNER JOIN product_item pitem ON pitem.item_id = cdi.item_id
                                      LEFT JOIN product_spec ps1 ON ps1.spec_id =pitem.spec_id_1
                                      LEFT JOIN product_spec ps2 ON ps2.spec_id =pitem.spec_id_2
                                  WHERE pitem.product_id = {0}", proudctId);
                return _dbAccess.getDataTableForObj<CourseDetailItemCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemDao-->QueryName" +ex.Message,ex);
            }
        }
    }
}
