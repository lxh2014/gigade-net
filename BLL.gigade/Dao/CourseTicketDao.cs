using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;


namespace BLL.gigade.Dao
{
    public class CourseTicketDao : ICourseTicketImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public CourseTicketDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
         
        /// <summary>
        /// 保存單個課程細項信息
        /// </summary>
        /// <param name="c">一個CourseDetail對象</param>
        /// <returns>受影響的行數</returns>
        public string Save(CourseTicket ct)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //sb.Append(@"INSERT INTO course_ticket(`course_detail_id`,`ticket_Code`,`user_id`,`create_date`,`create_user`,`order_id`,`flag`)");
                //sb.AppendFormat("VALUES({0},'{1}',{2},{3},{4},{5},{6})", ct.ticket_detail_id, ct.ticket_code, ct.User_Id, ct.Create_Date.ToString("yyyy-MM-dd HH:mm:ss"), ct.User_Id, ct.Order_Id, ct.Flag);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketDao-->Save" + ex.Message, ex);
            }
        }

        public List<CourseTicketCustom> Query(int course_detail_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT ct.ticket_id,ct.ticket_Code,ct.user_id,u.user_name AS User_Name,ct.flag FROM course_ticket ct
     INNER JOIN course_detail_item cdi ON cdi.course_detail_item_id = ct.course_detail_item_id
     LEFT  JOIN users u ON u.user_id = ct.user_id
     WHERE cdi.course_detail_id={0} ", course_detail_id);
                return _dbAccess.getDataTableForObj<CourseTicketCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketDao-->Query" + ex.Message, ex);
            }
        }

        public List<CourseTicket> Query(CourseTicket query)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                query.Replace4MySQL();
                sb.Append(@"select ticket_id, ticket_detail_id ,flag,ticket_code from course_ticket where 1=1 ");
                if (query.ticket_id != 0)
                {
                    sb.AppendFormat(" and ticket_id='{0}'", query.ticket_id);
                }
                if (query.ticket_detail_id != 0)
                {
                    sb.AppendFormat(" and ticket_detail_id='{0}'", query.ticket_detail_id);
                }
                if (!string.IsNullOrEmpty(query.ticket_code))
                {
                    sb.AppendFormat(" and ticket_code='{0}'", query.ticket_code);
                }
                return _dbAccess.getDataTableForObj<CourseTicket>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketDao-->Query" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據item_id統計課程的已銷總數和售賣總數
        /// </summary>
        /// <param name="item_id"></param>
        /// <returns></returns>
        public DataTable GetCount(int item_id, int flag = -1)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"SELECT cdi.item_id,td.ticket_detail_id, count(ct.ticket_id) as number
                            FROM  course_detail_item cdi
                            LEFT JOIN ticket_detail td ON td.cd_item_id=cdi.course_detail_item_id
                            LEFT JOIN course_ticket ct ON ct.ticket_detail_id=td.ticket_detail_id WHERE 1=1");

                if (item_id != 0)
                {
                    sql.AppendFormat(" and cdi.item_id='{0}'", item_id);
                }
                if (flag != -1)
                {
                    sql.AppendFormat(" and ct.flag='{0}'", flag);
                }
                sql.Append("  GROUP BY cdi.item_id");
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketDao-->GetCount" + ex.Message, ex);
            }
            return _dbAccess.getDataTable(sql.ToString());
        }

        /// <summary>
        /// 序號核銷列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetTicketCode(CourseTicket query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            //  StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlTemp = new StringBuilder();
            totalCount = 0;
            try
            {
                query.Replace4MySQL();
                #region 條件查詢
                if (query.flag != -1)
                {
                    sqlTemp.AppendFormat(" and flag='{0}'", query.flag);
                }
                if (!string.IsNullOrEmpty(query.ticket_code))
                {
                    sqlTemp.AppendFormat(" and ticket_code='{0}'", query.ticket_code);
                }
                if (query.ticket_detail_id != 0)
                {
                    sqlTemp.AppendFormat(" and ticket_detail_id='{0}'", query.ticket_detail_id);
                }
                #endregion

                sql.Append(@" SELECT cp.course_id, pi.item_id,pi.spec_id_1,pi.spec_id_2,ct.ticket_code ,ct.flag,ct.ticket_detail_id,ct.ticket_id");
                if (sqlTemp.Length > 0)
                {
                    sqlFrom.AppendFormat(" FROM (select ticket_id, ticket_detail_id ,flag,ticket_code from course_ticket where {0} ) as ct", sqlTemp.ToString().TrimStart().Remove(0, 3));
                }
                else
                {
                    sqlFrom.Append(" FROM course_ticket ct");
                }
                sqlFrom.Append(@"   LEFT JOIN ticket_detail td ON td.ticket_detail_id=ct.ticket_detail_id 
                                    LEFT JOIN course_detail_item cdi ON cdi.course_detail_item_id=td.cd_item_id
                                    LEFT JOIN product_item pi ON pi.item_id=cdi.item_id
                                    LEFT JOIN course_product cp ON cp.product_id=pi.product_id");
                if (query.IsPage)
                {
                    DataTable _dtCount = _dbAccess.getDataTable("select count(ct.ticket_code) as totalCount " + sqlFrom.ToString());
                    if (_dtCount.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dtCount.Rows[0]["totalCount"]);
                    }
                    sqlFrom.AppendFormat(" LIMIT {0},{1} ;", query.Start, query.Limit);
                }
                sql.Append(sqlFrom.ToString());
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDetailDao-->GetTicketCode-->" + sql.ToString() + ex.Message, ex);
            }

        }

        /// <summary>
        /// 執行核銷動作
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user_type">變更人類型 1  吉甲地管理員 2供應商</param>
        /// <returns></returns>
        public bool TicketVerification(CourseTicket store, int user_type)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                MySqlDao _sqlDao = new MySqlDao(connStr);
                ArrayList list = new ArrayList();
                list.Add(UpdateFlag(store));
                list.Add(string.Format(@"insert into ticket_accept(ticket_id,ticket_detail_id,ta_create_user_type,ta_create_user,ta_create_date)
                           values('{0}','{1}','{2}','{3}','{4}');", store.ticket_id, store.ticket_detail_id, user_type, store.modify_user,
                                                                  Common.CommonFunction.DateTimeToString(store.modify_date)));
                return _sqlDao.ExcuteSqls(list);
            }
            catch (Exception ex)
            {
                throw new Exception("CourseTicketDao-->TicketVerification" + ex.Message, ex);
            }
        }

        public string UpdateFlag(CourseTicket store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates = 0;update course_ticket set flag=1,modify_user='{1}',modify_date='{2}'  where ticket_id='{0}'; set sql_safe_updates = 1;", store.ticket_id, store.modify_user, Common.CommonFunction.DateTimeToString(store.modify_date));

            return sql.ToString();
        }
    }
}
