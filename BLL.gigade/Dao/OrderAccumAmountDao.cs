using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class OrderAccumAmountDao : IOrderAccumAmountImplDao
    {
         private IDBAccess _access;
         public OrderAccumAmountDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
         public DataTable GetOrderAccumAmountTable(OrderAccumAmountQuery query, out int totalCount)
         {
             StringBuilder sql = new StringBuilder();
             StringBuilder sqlCondition = new StringBuilder();
             StringBuilder sqlcount = new StringBuilder();
             try
             {
                 sql.Append(@"select oaa.event_id,oaa.accum_amount,oaa.event_start_time,oaa.event_end_time,oaa.event_desc,oaa.event_name,oaa.event_desc_start,oaa.event_desc_end,oaa.event_status ");
                 sql.Append(",oaa.event_create_user,oaa.event_create_time,event_update_user,event_update_time,mu1.user_username as create_user,mu2.user_username as update_user ");
                 sqlCondition.Append(" from order_accum_amount oaa  ");
                 sqlCondition.Append(" left join manage_user mu1 on mu1.user_id=oaa.event_create_user ");
                 sqlCondition.Append(" left join manage_user mu2 on mu2.user_id=oaa.event_update_user ");
                 sqlcount.Append("select count(oaa.event_id) as totalCount ");
                 sqlCondition.Append(" where 1=1 ");
                 if (query.event_id != 0)
                 {
                     sqlCondition.AppendFormat(" and   oaa.event_id='{0}' ", query.event_id);
                 }
                 if (query.event_status != -1)
                 {
                     sqlCondition.AppendFormat(" and   oaa.event_status='{0}' ", query.event_status);
                 }
                 if (!string.IsNullOrEmpty(query.event_name))
                 {
                     sqlCondition.AppendFormat(" and   oaa.event_name like N'%{0}%' ", query.event_name);
                 }
                 if (query.dateCondition != 0)
                 {
                     if (query.event_start_time != DateTime.MinValue)
                     {
                         if (query.event_start_time != DateTime.MinValue)
                         {
                             switch (query.dateCondition)
                             { 
                                 case 1:
                                     sqlCondition.AppendFormat(" and  oaa.event_start_time between '{0}' and '{1}' ", query.event_start_time.ToString("yyyy-MM-dd 00:00:00"), query.event_end_time.ToString("yyyy-MM-dd 23:59:59"));
                                     break;
                                 case 2:
                                     sqlCondition.AppendFormat(" and  oaa.event_end_time between '{0}' and '{1}' ", query.event_start_time.ToString("yyyy-MM-dd 00:00:00"), query.event_end_time.ToString("yyyy-MM-dd 23:59:59"));
                                     break;
                                 case 3:
                                     sqlCondition.AppendFormat(" and  oaa.event_desc_start between '{0}' and '{1}' ", query.event_start_time.ToString("yyyy-MM-dd 00:00:00"), query.event_end_time.ToString("yyyy-MM-dd 23:59:59"));
                                     break;
                                 case 4:
                                     sqlCondition.AppendFormat(" and  oaa.event_desc_end between '{0}' and '{1}' ", query.event_start_time.ToString("yyyy-MM-dd 00:00:00"), query.event_end_time.ToString("yyyy-MM-dd 23:59:59"));
                                     break;
                             }                            
                         }
                     }                                   
                 }
                 sqlCondition.Append(" order by oaa.event_id desc ");

                 totalCount = 0;
                 if (query.IsPage)
                 {
                     sqlcount.Append(sqlCondition.ToString());
                     DataTable _dt = _access.getDataTable(sqlcount.ToString());
                     if (_dt.Rows.Count > 0)
                     {
                         totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                     }

                     sqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                 }

                 sql.Append(sqlCondition.ToString());

                 return _access.getDataTable(sql.ToString());
             }
             catch (Exception ex)
             {

                 throw new Exception("OrderAccumAmountDao.GetOrderAccumAmountTable-->" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
             }
         }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
         public string AddOrderAccumAmount(OrderAccumAmountQuery query)
         {
             StringBuilder sb = new StringBuilder();
             try
             {
                 sb.Append(" insert into order_accum_amount(accum_amount,event_start_time,event_end_time,event_desc,event_name,event_desc_start ");
                 sb.Append(" ,event_desc_end,event_status,event_create_user,event_create_time,event_update_user,event_update_time)values( ");
                 sb.AppendFormat("'{0}','{1}','{2}'", query.accum_amount, CommonFunction.DateTimeToString(query.event_start_time), CommonFunction.DateTimeToString(query.event_end_time));
                 sb.AppendFormat(",'{0}','{1}','{2}'", query.event_desc, query.event_name, CommonFunction.DateTimeToString(query.event_desc_start));
                 sb.AppendFormat(",'{0}','{1}','{2}'", CommonFunction.DateTimeToString(query.event_desc_end), query.event_status, query.event_create_user);
                 sb.AppendFormat(",'{0}','{1}','{2}');", CommonFunction.DateTimeToString(query.event_create_time), query.event_update_user, CommonFunction.DateTimeToString(query.event_update_time));
               
                 return sb.ToString();
             }
             catch (Exception ex)
             {
                 throw new Exception("OrderAccumAmountDao.AddOrderAccumAmount-->" + ex.Message + sb.ToString(), ex);
             }
         }
        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
         public string UPOrderAccumAmount(OrderAccumAmountQuery query)
         {
             StringBuilder sb = new StringBuilder();
             try
             {
                 sb.AppendFormat(" set sql_safe_updates=0;update order_accum_amount set accum_amount='{0}',event_start_time='{1}'", query.accum_amount, CommonFunction.DateTimeToString(query.event_start_time));
                 sb.AppendFormat(",event_end_time='{0}',event_desc='{1}',event_name='{2}' ", CommonFunction.DateTimeToString(query.event_end_time), query.event_desc, query.event_name);
                 sb.AppendFormat(",event_desc_start='{0}',event_desc_end='{1}' ", CommonFunction.DateTimeToString(query.event_desc_start),CommonFunction.DateTimeToString(query.event_desc_end));
                 sb.AppendFormat(",event_status='{0}',event_update_user='{1}'", query.event_status, query.event_update_user);
                 sb.AppendFormat(",event_update_time='{0}' where event_id='{1}';set sql_safe_updates=1;", CommonFunction.DateTimeToString(query.event_update_time), query.event_id);
                 return sb.ToString();
             }
             catch (Exception ex)
             {
                 throw new Exception("OrderAccumAmountDao.UPOrderAccumAmount-->" + ex.Message + sb.ToString(), ex);
             }
         
         }
        /// <summary>
        /// 更改狀態
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
         public string UpdateActive(OrderAccumAmountQuery query)
         {
             StringBuilder sql = new StringBuilder();
             try
             {
                 sql.AppendFormat("set sql_safe_updates=0;update order_accum_amount set event_status='{0}',event_update_user='{1}'", query.event_status, query.event_update_user);
                 sql.AppendFormat(",event_update_time='{0}' where event_id='{1}';set sql_safe_updates=1;", CommonFunction.DateTimeToString(query.event_update_time), query.event_id);
                 return sql.ToString();
             }
             catch (Exception ex)
             {
                 throw new Exception("OrderAccumAmountDao.UpdateActive-->" + ex.Message + sql.ToString(), ex);
             }
         }
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
         public string DelOrderAccumAmount(OrderAccumAmountQuery query)
         {
             StringBuilder sql = new StringBuilder();
             try
             {
                 sql.AppendFormat("delete from order_accum_amount where event_id in({0});", query.event_id_in);
                 return sql.ToString();
             }
             catch (Exception ex)
             {
                 throw new Exception("OrderAccumAmountDao.DelOrderAccumAmount-->" + ex.Message + sql.ToString(), ex);
             }
         }


    }
}
