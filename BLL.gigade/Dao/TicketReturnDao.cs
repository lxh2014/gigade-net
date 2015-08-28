using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
  public  class TicketReturnDao
    {
      private IDBAccess _access;
      private string connStr;
      public TicketReturnDao(string connectionString )
      {
          _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
          this.connStr = connectionString;
      }

      public List<TicketReturnQuery> GetTicketReturnList(TicketReturnQuery query,out int totalCount )
      {
          StringBuilder sql = new StringBuilder();
          StringBuilder sqlFrom = new StringBuilder();
          StringBuilder sqlWhere = new StringBuilder();
           totalCount = 0;
          try
          {
              sql.Append(" select  tr.tr_id,tr.ticket_master_id,tr.tr_note,tr.tr_bank_note,tr.tr_update_user,tr.tr_create_user, tr.tr_create_date,tr.tr_update_date,tr.tr_ipfrom,tr.tr_money,tr.tr_status,para.parameterName as 'tr_reason_type'  ");
              sqlFrom.Append(" from ticket_return tr    ");
              sqlFrom.Append(" LEFT JOIN (select parameterType,parameterCode,parameterName from t_parametersrc where parameterType='tr_reason_type') para on para.parameterCode=tr.tr_reason_type  ");
              sqlWhere.Append(" where 1=1 ");
              if (query.start_date != DateTime.MinValue && query.end_date != DateTime.MinValue)
              {
                  sqlWhere.AppendFormat("  and tr_create_date>='{0}'  and   tr_create_date<='{1}'  ",  CommonFunction.DateTimeToString(query.start_date),CommonFunction.DateTimeToString(query.end_date));
              }
              DataTable _dt = _access.getDataTable("select count(tr.tr_id) as 'totalCount' "+sqlFrom.ToString()+sqlWhere.ToString());
              if (_dt.Rows.Count > 0)
              {
                  totalCount = Convert.ToInt32(_dt.Rows[0][0]);
              }
              return _access.getDataTableForObj<TicketReturnQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("TicketReturnDao-->GetTicketReturnList-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString() + ex.Message, ex);
          }
      }

      public string SaveTicketReturn(TicketReturnQuery query)
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              query.Replace4MySQL();
              sql.AppendFormat("set sql_safe_updates=0;update ticket_return set tr_note ='{0}',tr_bank_note='{1}',tr_update_user='{2}'", query.tr_note, query.tr_bank_note, query.tr_update_user);
              sql.AppendFormat(",tr_update_date='{0}',tr_money='{1}', tr_status='{2}',tr_reason_type='{3}'",CommonFunction.DateTimeToString(DateTime.Now),query.tr_money,query.tr_status,query.tr_reason_type);
              sql.AppendFormat(" where tr_id='{0}';set sql_safe_updates=1; ", query.tr_id);
              return sql.ToString();
          }
          catch (Exception ex)
          {
              throw new Exception("TicketReturnDao-->SaveTicketReturn-->"+sql.ToString()+ex.Message,ex);
          }
      }


      public string SaveTicketReturnLog(TicketReturnQuery query)
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              query.Replace4MySQL();
              sql.AppendFormat("insert into ticket_return_change_log (tr_id,trcl_last_status,trcl_new_status,trcl_new_money,trcl_last_money, ");
              sql.AppendFormat("trcl_create_user,trcl_create_date,trcl_note)   ");
              sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}',", query.tr_id, query.trcl_last_status, query.tr_status, query.tr_money, query.trcl_last_money);
              sql.AppendFormat("'{0}','{1}','{2}');", query.trcl_create_user, CommonFunction.GetPHPTime() , query.tr_note);
              return sql.ToString();
          }
          catch (Exception ex)
          {
              throw new Exception("TicketReturnDao-->SaveTicketReturnLog-->" + sql.ToString() + ex.Message, ex);
          }
      }

      public bool ExecTicketReturnSql(ArrayList arrList)
      {
          try
          {
              MySqlDao myDao = new MySqlDao(connStr);
              return myDao.ExcuteSqls(arrList);
          }
          catch (Exception ex)
          {
              throw new Exception("TicketReturnDao-->ExecTicketReturnSql-->"+ ex.Message, ex);
          }
      }

      public List<TicketReturnQuery> GetReasonTypeStore()
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              sql.Append("select parameterCode,parameterName from t_parametersrc where parameterType='tr_reason_type'  ");
              return _access.getDataTableForObj<TicketReturnQuery>(sql.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("TicketReturnDao-->GetReasonTypeStore-->" +sql.ToString()+ ex.Message, ex);
          }
      }

    }
}
