using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Common;
using System.Collections;
 
namespace BLL.gigade.Dao
{
    public class TicketMasterDao : ITicketMasterImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public TicketMasterDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            connStr = connectionstring;
        }
        public DataTable  GetTicketMasterList(TicketMasterQuery tm, out int totalCount)
        {
            totalCount = 0;
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sqlfield.AppendLine(@"select tm.ticket_master_id,td.ticket_detail_id,v.vendor_id,cd.course_detail_name,cd.start_date,cd.end_date,tm.order_name,");
            sqlfield.AppendLine(@"tm.order_payment,para.parameterName as 'order_payment_string',tm.order_amount,tm.order_createdate,tm.invoice_status,tm.master_status, para2.remark as 'master_status_string' ,tm.billing_checked,tm.note_admin ");
            sqlfield.AppendLine(@",tm.delivery_name,tm.delivery_mobile,tm.delivery_phone,tm.delivery_zip,tm.delivery_address ");
            sqlfield.AppendLine(@" ,tm.order_mobile,tm.order_phone,tm.order_zip,tm.order_address");
            sqlfrom.AppendLine(@" FROM ticket_master tm");
            sqlfrom.Append(" LEFT JOIN ticket_detail td on tm.ticket_master_id=td.ticket_master_id LEFT JOIN vendor v on v.vendor_id=td.vendor_id ");
            sqlfrom.Append(" LEFT JOIN course_detail_item cdi on cdi.course_detail_item_id=td.cd_item_id LEFT JOIN course_detail cd on cd.course_detail_id=cdi.course_detail_id ");
            sqlfrom.Append(" LEFT JOIN course c on c.course_id=cd.course_id  ");
            sqlfrom.Append(" LEFT JOIN (select parameterType,parameterCode,parameterName from t_parametersrc where parameterType='payment') para ON para.parameterCode=tm.order_payment ");
            sqlfrom.AppendLine(@" LEFT JOIN  (select parameterType,parameterCode,remark from t_parametersrc where parameterType='order_status') para2 on para2.parameterCode=tm.master_status ");
            sqlwhere.AppendLine(@"WHERE 1=1 ");
            if (tm.ticket_master_id != 0)
            {
                sqlwhere.AppendFormat(@" AND tm.ticket_master_id='{0}' ", tm.ticket_master_id);
            }
            if (!string.IsNullOrEmpty(tm.order_name))
            {
                sqlwhere.AppendFormat(@" AND tm.order_name LIKE N'%{0}%' ", tm.order_name);
            }
            if (tm.master_status != -1)
            {
                sqlwhere.AppendFormat(@" AND tm.master_status='{0}' ", tm.master_status);
            }
            if (tm.order_payment != -1)
            {
                sqlwhere.AppendFormat(@" AND tm.order_payment='{0}' ", tm.order_payment);
            }
            if (tm.order_start != 0 && tm.order_end != 0)
            {
                sqlwhere.AppendFormat(@" AND tm.order_createdate>={0} AND tm.order_createdate<={1} ", tm.order_start, tm.order_end);
            }
            if (tm.course_id != 0)
            {
                sqlwhere.AppendFormat(@" AND c.course_id='{0}' ", tm.course_id);
            }
            if (!string.IsNullOrEmpty(tm.course_name))
            {
                sqlwhere.AppendFormat(@" AND cd.course_detail_name LIKE N'%{0}%' ", tm.course_name);
            }

            if (tm.start_date != DateTime.MinValue && tm.end_date != DateTime.MinValue)
            {
                sqlwhere.AppendFormat(@" AND cd.start_date>='{0}' AND cd.end_date<='{1}' ", tm.start_date.ToString("yyyy-MM-dd HH:mm:ss"), tm.end_date.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            sqlwhere.AppendFormat(@" AND tm.billing_checked='{0}' ", tm.billing_checked);
            sql.Append(sqlfield.ToString());
            sql.Append(sqlfrom.ToString());
            sql.Append(sqlwhere.ToString());
            try
            {
                if (tm.IsPage)
                {
                    sql.AppendFormat(@" ORDER BY tm.ticket_master_id DESC  LIMIT {0},{1}; ", tm.Start, tm.Limit);
                    DataTable dt = _access.getDataTable(" select tm.ticket_master_id " + sqlfrom + sqlwhere);
                    totalCount = dt.Rows.Count;
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterDao-->GetTicketMasterList" + ex.Message + sql.ToString(), ex);
            }
        }

        public int Update(TicketMaster tm)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE ticket_master SET delivery_name='{0}',delivery_mobile='{1}',delivery_phone='{2}',", tm.delivery_name, tm.delivery_mobile, tm.delivery_phone);
            sql.AppendFormat(@" delivery_zip='{0}',delivery_address='{1}' ", tm.delivery_zip, tm.delivery_address);
            sql.AppendFormat(@" ,order_name='{0}',order_mobile='{1}',order_phone='{2}',", tm.order_name, tm.order_mobile, tm.order_phone);
            sql.AppendFormat(@"order_zip='{0}',order_address='{1}' ", tm.order_zip, tm.order_address);
            sql.AppendFormat(@" WHERE ticket_master_id='{0}'; ", tm.ticket_master_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterDao-->Update" + ex.Message + sql.ToString(), ex);
            }

        }
        public DataTable GetCourseCountList(CourseQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                query.Replace4MySQL();
                sql.Append(@"SELECT c.course_id,c.course_name ,pi.item_id,p.brand_id,pi.spec_id_1,pi.spec_id_2,cd.start_date,cd.end_date ");
                sqlFrom.Append(@" FROM course c LEFT JOIN course_detail cd ON cd.course_id=c.course_id
                                    LEFT JOIN course_product cp ON cp.course_id=c.course_id
                                    LEFT JOIN product p ON p.product_id=cp.product_id
                                    LEFT JOIN product_item pi ON pi.product_id=p.product_id ");


                //                sqlFrom.Append(@" FROM product_item pi
                //                                  LEFT JOIN product p ON p.product_id=pi.product_id
                //                                  LEFT JOIN course_product cp ON cp.product_id=p.product_id
                //                                  LEFT JOIN course c ON c.course_id=cp.course_id
                //                                  LEFT JOIN course_detail cd ON cd.course_id=c.course_id");

                if (!string.IsNullOrEmpty(query.Vendor_Name_Simple))
                {
                    sqlWhere.AppendFormat("and v.vendor_name_simple  like '%{0}%'  ", query.Vendor_Name_Simple);
                }
                if (query.Course_Id != 0)
                {
                    sqlWhere.AppendFormat("and c.course_id='{0}'  ", query.Course_Id);
                }
                if (!string.IsNullOrEmpty(query.Course_Name))
                {
                    sqlWhere.AppendFormat("and c.course_name like '%{0}%' ", query.Course_Name);
                }
                if (query.Start_Date != DateTime.MinValue)
                {
                    sqlWhere.AppendFormat("and cd.start_date >='{0}' ", query.Start_Date);
                }
                if (query.End_Date != DateTime.MinValue)
                {
                    sqlWhere.AppendFormat("and cd.end_date <='{0}' ", query.End_Date);
                }
                if (sqlWhere.Length != 0)
                {
                    sqlFrom.Append(" WHERE " + sqlWhere.ToString().TrimStart().Remove(0, 3));
                }
                if (query.IsPage)
                {
                    DataTable _dtCount = _access.getDataTable("select count(c.course_id) as totalCount " + sqlFrom.ToString());
                    if (_dtCount.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dtCount.Rows[0]["totalCount"]);
                    }
                    sqlFrom.AppendFormat("LIMIT {0},{1} ;", query.Start, query.Limit);
                }
                sql.Append(sqlFrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterDao-->GetCourseCountList-->" + sql.ToString() + ex.Message, ex);
            }

        }

        public string CancelOrderTM(TicketMasterQuery query)//TicketMaster
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" update ticket_master set master_status='{0}' where ticket_master_id='{1}';", query.master_status,query.ticket_master_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterDao-->CancelOrderTM" + ex.Message + sql.ToString(), ex);
            }
        }
        public string CancelOrderTD(TicketMasterQuery query)//TicketDetail
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" SET SQL_SAFE_UPDATES = 0;update ticket_detail set detail_status='{0}' where ticket_master_id='{1}';SET SQL_SAFE_UPDATES = 1;", query.master_status, query.ticket_master_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterDao-->CancelOrderTM" + ex.Message + sql.ToString(), ex);
            }
        }

        public bool ExecSql(ArrayList arrList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqls(arrList);
            }
             catch (Exception ex)
            {
                throw new Exception(" EmsDao-->execInsertSql--> " + arrList + ex.Message, ex);
            }
        }


    }
}
