using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CourseProductTempDao : ICourseProductTempImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseProductTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public CourseProductTemp Query(CourseProductTemp courProd)
        {
            return _dbAccess.getSinggleObj<CourseProductTemp>(string.Format("select course_product_id,writer_id,course_id,product_id from course_product_temp where writer_id={0} and product_id={1};", courProd.Writer_Id, courProd.Product_Id));
        }

        public int Save(CourseProductTemp courProd)
        {
            return _dbAccess.execCommand(string.Format("insert into course_product_temp(writer_id,course_id,product_id) values({0},{1},{2});",
                courProd.Writer_Id, courProd.Course_Id, courProd.Product_Id));
        }

        public int Update(CourseProductTemp courProd)
        {
            return _dbAccess.execCommand(string.Format("set sql_safe_updates=0;update course_product_temp set course_id={0} where writer_id={1} and product_id={2};set sql_safe_updates=1;",
                courProd.Course_Id, courProd.Writer_Id, courProd.Product_Id));
        }

        //將臨時表數據導入正式表
        public string MoveCourseProduct(CourseProductTemp courProd)
        {
            StringBuilder strSql = new StringBuilder("insert into course_product(course_id,product_id) select course_id,{0} as product_id from course_product_temp ");
            strSql.AppendFormat(" where writer_id={0} and product_id={1};", courProd.Writer_Id, courProd.Product_Id);
            return strSql.ToString();
        }

        public string DeleteSql(CourseProductTemp courProd)
        {
            return string.Format("set sql_safe_updates=0;delete from course_product_temp where writer_id={0} and product_id={1};set sql_safe_updates=1;", courProd.Writer_Id, courProd.Product_Id);
        }
    }
}
