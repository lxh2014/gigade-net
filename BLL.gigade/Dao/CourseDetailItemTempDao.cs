using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CourseDetailItemTempDao : ICourseDetailItemTempImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseDetailItemTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public List<CourseDetailItemTemp> Query(int writerId, int productId)//add by wwei0216w 此處需要添加 people_count
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT cdi.course_detail_item_id,cdi.course_detail_id,cdi.item_id,cdi.people_count FROM product_item_temp pi 
                INNER JOIN course_detail_item_temp cdi ON pi.item_id=cdi.item_id
                WHERE pi.writer_id={0} AND pi.product_id='{1}';", writerId,productId);
                return _dbAccess.getDataTableForObj<CourseDetailItemTemp>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemTempDao-->Query" + ex.Message,ex);
            }
        }

        public int Add(List<CourseDetailItemTemp> list)//add by wwei0216w 此處需要添加 people_count
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("INSERT INTO course_detail_item_temp(`course_detail_id`,`item_id`,`writer_id`,`people_count`)VALUES");
                foreach (CourseDetailItemTemp cdi in list)
                {
                    sb.AppendFormat("({0},{1},{2},{3}),",cdi.Course_Detail_Id,cdi.Item_Id,cdi.Writer_Id,cdi.People_Count);
                }
                string strSql = sb.ToString().Remove(sb.ToString().Length - 1, 1);
                return _dbAccess.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemTempDao-->Save" + ex.Message,ex);
            }
        }



        public int Update(List<CourseDetailItemTemp> list)
        {
            StringBuilder strSql = new StringBuilder();
            foreach (var item in list)
            {
                strSql.AppendFormat("update course_detail_item_temp set course_detail_id={0},item_id={1},people_count = {2} where course_detail_item_id={3};",
                    item.Course_Detail_Id, item.Item_Id,item.People_Count, item.Course_Detail_Item_Id);
            }
            return _dbAccess.execCommand(strSql.ToString());
        }

        public int Delete(int writerId)
        {
            try
            {
                //string sqlStr = string.Format("set sql_safe_updates=0;delete from course_detail_item_temp where writer_id ={0};set sql_safe_updates=1;", writerId);
                return _dbAccess.execCommand(DeleteSql(writerId));
            }
            catch (Exception ex)
            {
                throw new Exception("CourseDetailItemTempDao-->Delete" + ex.Message, ex);
            }
        }

        public int Delete(int[] ids)
        {
            return _dbAccess.execCommand(string.Format("delete from course_detail_item_temp where Course_Detail_Item_id in {0};",
                string.Join(",", ids)));
        }


          //將臨時表數據導入正式表
        public string MoveCourseDetailItem(int writerId) //add by wwei0216w 此處需要添加 people_count 
        {
            StringBuilder strSql = new StringBuilder("insert into course_detail_item(course_detail_id,item_id,people_count) ");
            strSql.Append("select course_detail_id,{0} as item_id,people_count from course_detail_item_temp where item_id={1} ");
            strSql.AppendFormat(" and writer_id={0};", writerId);
            return strSql.ToString();
        }

        public string DeleteSql(int writerId)
        {
            return string.Format("set sql_safe_updates=0;delete from course_detail_item_temp where writer_id={0};set sql_safe_updates=1;", writerId);
        }

    }
}
