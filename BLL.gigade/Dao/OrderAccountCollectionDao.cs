using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class OrderAccountCollectionDao
    {
        private IDBAccess _access;
        public OrderAccountCollectionDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable GetOrderAccountCollectionList(Model.OrderAccountCollection query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.Append("select count(oac.row_id)  as totalCount ");
                sql.Append(" select oac.row_id,oac.order_id,oac.account_collection_time,oac.account_collection_money,oac.poundage,oac.return_collection_time,oac.return_collection_money,oac.return_poundage,oac.remark ");
                sqlFrom.Append(" from order_account_collection oac ");

                sqlWhere.Append(" where 1=1  ");
                //if (query.searchdate == 1)
                //{
                //    sqlWhere.AppendFormat(" and eg.year={0}  and  eg.month={1}  ", query.date.Year, query.date.Month);
                //}
                if (query.order_id != 0)
                {
                    sqlWhere.AppendFormat(" AND oac.order_id='{0}'", query.order_id);
                }
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" ORDER BY oac.row_id DESC limit {0},{1} ;", query.Start, query.Limit);
                }
                return _access.getDataTable(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionDao-->GetOrderAccountCollectionList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);

            }
        }


        public int SaveOrEdit(OrderAccountCollection model)
        {
            StringBuilder str = new StringBuilder();
            model.Replace4MySQL();
            try
            {
                if (model.row_id == 0)
                {
                    str.Append(@"insert into order_account_collection(order_id,remark,account_collection_time,account_collection_money,poundage,return_collection_time,return_collection_money,return_poundage) ");
                    str.AppendFormat(" values('{0}','{1}'", model.order_id, model.remark);
                    if (model.account_collection_time != null && model.account_collection_time != DateTime.MinValue)
                    {
                        str.AppendFormat(" ,'{0}','{1}','{2}'", Common.CommonFunction.DateTimeToString(model.account_collection_time), model.account_collection_money, model.poundage);
                    }
                    else
                    {
                        str.AppendFormat(" ,NULL ,NULL  ,NULL  ");
                    }
                    if (model.return_collection_time != null && model.return_collection_time != DateTime.MinValue)
                    {
                        str.AppendFormat(" ,'{0}','{1}','{2}' ", Common.CommonFunction.DateTimeToString(model.return_collection_time), model.return_collection_money, model.return_poundage);
                    }
                    else
                    {
                        str.AppendFormat(" , NULL  ,NULL  ,NULL ");
                    }
                    str.AppendFormat(" );");
                }
                else
                {
                    str.AppendFormat(@"update order_account_collection set order_id='{0}',", model.order_id);

                    if (model.account_collection_time != null && model.account_collection_time != DateTime.MinValue)
                    {
                        str.AppendFormat("account_collection_time='{0}', account_collection_money='{1}',poundage='{2}',", Common.CommonFunction.DateTimeToString(model.account_collection_time), model.account_collection_money, model.poundage);
                    }
                    else
                    {
                        str.AppendFormat("account_collection_time={0}, account_collection_money={1},poundage={2},", "NULL", "NULL", "NULL");
                    }

                    if (model.return_collection_time != null && model.return_collection_time != DateTime.MinValue)
                    {
                        str.AppendFormat("  return_collection_time='{0}',return_collection_money='{1}',return_poundage='{2}', ", Common.CommonFunction.DateTimeToString(model.return_collection_time), model.return_collection_money, model.return_poundage);
                    }
                    else
                    {
                        str.AppendFormat("  return_collection_time={0},return_collection_money={1},return_poundage={2}, ", "NULL", "NULL", "NULL");
                    }
                    str.AppendFormat(@" remark='{0}' ", model.remark);
                    str.AppendFormat(" where row_id='{0}' ", model.row_id);
                }
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionDao-->SaveOrEdit-->" + ex.Message, ex);
            }
        }

        public int Delete(string str_row_id)
        {

            StringBuilder str = new StringBuilder();
            try
            {

                str.AppendFormat(@"delete from order_account_collection where row_id in({0})", str_row_id);

                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionDao-->Delete-->" + ex.Message, ex);
            }
        }

        public int YesOrNoOrderId(string order_id)
        {
            StringBuilder str = new StringBuilder();
            try
            {

                str.AppendFormat(@"select order_id from order_master where order_id='{0}'", order_id);
                DataTable _dt = _access.getDataTable(str.ToString());
                if (_dt != null)
                {
                    return Convert.ToInt32(_dt.Rows[0]["order_id"]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionDao-->YesOrNoOrderId-->" + ex.Message, ex);
            }
        }

        public int IncludeOrderId(string order_id)
        {
            StringBuilder str = new StringBuilder();
            try
            {

                str.AppendFormat(@"select order_id from order_account_collection where order_id= '{0}'", order_id);
                DataTable _dt = _access.getDataTable(str.ToString());
                return _dt.Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionDao-->IncludeOrderId-->" + ex.Message, ex);
            }
        }

    }
}
