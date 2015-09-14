using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
  public  class OrderDetailManagerDao
    {

      private IDBAccess _access;
      public OrderDetailManagerDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
      /// <summary>
      /// 列表頁
      /// </summary>
      /// <param name="query"></param>
      /// <param name="totalCount"></param>
      /// <returns></returns>
      public List<OrderDetailManagerQuery> GetODMList(OrderDetailManagerQuery query,out int totalCount)
      {
          StringBuilder sql = new StringBuilder();
          StringBuilder sqlCount = new StringBuilder();
          StringBuilder sqlFrom = new StringBuilder();
          StringBuilder sqlWhere = new StringBuilder();
          totalCount = 0;
          try
          {
              sqlCount.Append("select count(odm.odm_id) as 'totalCount'  ");
              sql.Append("select odm.odm_id,odm.odm_user_id,odm.odm_user_name,odm.odm_status,odm.odm_createdate,odm_createuser,mu.user_username");
              sqlFrom.Append(" from order_detail_manager odm   ");
              sqlFrom.Append(" LEFT JOIN manage_user mu on mu.user_id=odm.odm_createuser  ");
              sqlWhere.Append("  where 1=1  ");
              if (query.odm_status != -1)
              {
                  sqlWhere.AppendFormat(" and odm.odm_status='{0}'  ", query.odm_status);
              }
              DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
              if (_dt != null & _dt.Rows.Count > 0)
              {
                  totalCount = Convert.ToInt32(_dt.Rows[0][0]);
              }
              sqlWhere.AppendFormat(" order by odm.odm_createdate desc  limit {0},{1}; ",query.Start,query.Limit);
              return _access.getDataTableForObj<OrderDetailManagerQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("OrderDetailManagerDao-->GetODMList-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString()+ex.Message,ex);
          }
      }
      /// <summary>
      /// 新增
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      public int InsertODM(OrderDetailManagerQuery query)
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              sql.AppendFormat("insert into order_detail_manager(odm_user_id,odm_user_name,odm_status,odm_createdate,odm_createuser)values('{0}','{1}','{2}','{3}','{4}');",query.odm_user_id,query.odm_user_name,query.odm_status,CommonFunction.DateTimeToString(DateTime.Now),query.odm_createuser);
              return _access.execCommand(sql.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("OrderDetailManagerDao-->InsertODM-->" + sql.ToString()+ ex.Message, ex);
          }
      }
      /// <summary>
      ///新增時查重
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      public DataTable IsExist(OrderDetailManagerQuery query)
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              sql.AppendFormat("select  odm_user_id  from order_detail_manager where odm_user_id='{0}';",query.odm_user_id);
              return _access.getDataTable(sql.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("OrderDetailManagerDao-->IsExist-->" + sql.ToString() + ex.Message, ex);
          }
      }
      /// <summary>
      /// 更改狀態
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      public int UpODMStatus(OrderDetailManagerQuery query)
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              sql.AppendFormat("update order_detail_manager set odm_status='{0}' where odm_id='{1}';",query.odm_status,query.odm_id);
              return _access.execCommand(sql.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("OrderDetailManagerDao-->UpODMStatus-->" + sql.ToString() + ex.Message, ex);
          }
      }
      /// <summary>
      /// ManageUserStore
      /// </summary>
      /// <returns></returns>
      public List<ManageUserQuery> ManageUserStore()
      {
          StringBuilder sql = new StringBuilder();
          try
          {
              sql.AppendFormat(" select user_id,user_username from manage_user where user_status=1;");
              return _access.getDataTableForObj<ManageUserQuery>(sql.ToString());
          }
          catch (Exception ex)
          {
              throw new Exception("OrderDetailManagerDao-->ManageUserStore-->" + sql.ToString() + ex.Message, ex);
          }
      }
    }
}
