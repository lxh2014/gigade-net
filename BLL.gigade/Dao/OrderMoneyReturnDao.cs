using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class OrderMoneyReturnDao : IOrderMoneyReturnImplDao
    {
        private IDBAccess _accessMySql;
        private string connString;
        public OrderMoneyReturnDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }


        public string InsertSql(Model.Query.OrderReturnMasterQuery query, OrderMoneyReturn om)
        {

            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat(@"insert into order_money_return (money_id,order_id,money_type,money_total,");
                sql.AppendFormat(@"money_status,money_note,bank_note,bank_name,");
                sql.AppendFormat(@"bank_branch,bank_account,account_name,money_source,");
                sql.AppendFormat(@"money_createdate,money_updatedate,money_ipfrom)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}',", om.money_id, query.order_id, om.money_type, om.money_total);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", 0, "", om.bank_note, om.bank_name);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}',",  om.bank_branch,om.bank_account,om.account_name, om.money_source);
                sql.AppendFormat(@"'{0}','{1}','{2}');", CommonFunction.GetPHPTime(), CommonFunction.GetPHPTime(), query.return_ipfrom);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnDao.InsertSql-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<OrderMoneyReturnQuery> OrderMoneyReturnList(OrderMoneyReturnQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder() ;
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
             totalCount = 0;
             query.Replace4MySQL();
             try
             {
                 sql.Append("select omr.money_id,omr.order_id,omr.money_type,om.order_payment,t.parameterName ,omr.money_total,omr.money_status,omr.money_note,omr.money_source,omr.bank_name,omr.bank_branch, omr.bank_account, omr.bank_note, omr.account_name, omr.money_createdate, omr.money_updatedate, omr.money_ipfrom ");
                 sqlFrom.Append(" from order_money_return omr LEFT JOIN order_master om on om.order_id=omr.order_id ");
                 sqlFrom.Append(" LEFT JOIN (select parameterName,parameterCode,parameterType  from t_parametersrc where parameterType='payment' ) t on t.parameterCode = om.order_payment ");
                 sqlWhere.Append(" where 1=1 ");
                 if (query.SearchStore == "1")
                 {
                     sqlWhere.AppendFormat(" and omr.order_id='{0}' ", query.searchContents);
                 }
                 else  if (query.SearchStore == "2")
                 {
                     sqlWhere.AppendFormat(" and omr.money_id='{0}' ", query.searchContents);
                 }
                 if (query.date == "1")
                 {
                     sqlWhere.AppendFormat(" and omr.money_createdate>='{0}' and  omr.money_createdate<='{1}' ",CommonFunction.GetPHPTime(CommonFunction.DateTimeToString( query.start_date)), CommonFunction.GetPHPTime( CommonFunction.DateTimeToString(query.end_date)));
                 }
                 if(!string.IsNullOrEmpty(query.states))
                 {
                     if ((Convert.ToInt32(query.states) != -1))
                     {
                         sqlWhere.AppendFormat(" and  money_status={0} ", Convert.ToInt32(query.states));
                     }
                 }
                 if (query.IsPage)
                 {
                     DataTable _dt = _accessMySql.getDataTable(("select count(omr.money_id) as totalCount ") + sqlFrom.ToString()+sqlWhere.ToString());
                     if (_dt.Rows.Count > 0)
                     {
                         totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                     }
                 }
                 sqlWhere.AppendFormat("ORDER BY omr.money_id DESC  limit {0},{1};", query.Start, query.Limit);
                 sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
                 return _accessMySql.getDataTableForObj<OrderMoneyReturnQuery>(sql.ToString());
             }
             catch (Exception ex)
             {
                 throw new Exception("OrderMoneyReturnDao-->OrderMoneyReturnList-->"+sql.ToString()+ex.Message,ex);
             }
        }

        //public int


        public DataTable ExportATM(OrderMoneyReturnQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append("select omr.order_id,omr.money_id,omr.money_total,omr.bank_name,omr.bank_branch,omr.bank_account,omr.account_name,omr.money_type  as ATM  from order_money_return as omr INNER JOIN order_master using(order_id) where money_type=2 ");
                if (query.SearchStore == "1")
                {
                    sqlWhere.AppendFormat(" and omr.order_id='{0}' ", query.searchContents);
                }
                else if (query.SearchStore == "2")
                {
                    sqlWhere.AppendFormat(" and omr.money_id='{0}' ", query.searchContents);
                }
                if (query.date == "1")
                {
                    sqlWhere.AppendFormat(" and omr.money_createdate>='{0}' and  omr.money_createdate<='{1}' ", CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.start_date)), CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.end_date)));
                }
                if (!string.IsNullOrEmpty(query.states))
                {
                    if ((Convert.ToInt32(query.states) != -1))
                    {
                        sqlWhere.AppendFormat(" and  money_status={0} ", Convert.ToInt32(query.states));
                    }
                }
                sql.Append(sqlWhere.ToString());
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnDao-->ExportATM-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public DataTable ExportCARD(OrderMoneyReturnQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat("select mr.order_id,mr.money_id,mr.money_total,m.order_amount,m.order_name,mr.money_type,mr.money_status from order_money_return mr join order_master m on mr.order_id=m.order_id where money_type=1 ");
                if (query.SearchStore == "1")
                {
                    sqlWhere.AppendFormat(" and omr.order_id='{0}' ", query.searchContents);
                }
                else if (query.SearchStore == "2")
                {
                    sqlWhere.AppendFormat(" and omr.money_id='{0}' ", query.searchContents);
                }
                if (query.date == "1")
                {
                    sqlWhere.AppendFormat(" and omr.money_createdate>='{0}' and  omr.money_createdate<='{1}' ", CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.start_date)), CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.end_date)));
                }
                if (!string.IsNullOrEmpty(query.states))
                {
                    if ((Convert.ToInt32(query.states) != -1))
                    {
                        sqlWhere.AppendFormat(" and  money_status={0} ", Convert.ToInt32(query.states));
                    }
                }
                sql.Append(sqlWhere.ToString());
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnDao-->ExportATM-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int SaveOMReturn(OrderMoneyReturnQuery query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat("update order_money_return set money_status={0},money_note='{1}',bank_note='{2}',bank_name='{3}', bank_branch='{4}',bank_account='{5}',account_name='{6}', money_updatedate={7}  where money_id={8};", query.money_status, query.money_note, query.bank_note, query.bank_name, query.bank_branch, query.bank_account, query.account_name, CommonFunction.GetPHPTime(), query.money_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnDao-->SaveOMReturn-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public int SaveCSNote(OrderMoneyReturnQuery query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat("update  order_money_return set cs_note='{0}'  where money_id='{1}';",query.cs_note,query.money_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnDao-->SaveCSNote-->" + sql.ToString() + ex.Message, ex);
            }
            throw new NotImplementedException();
        }
    }
}
