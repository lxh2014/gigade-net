using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class EmsDao : IEmsImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public EmsDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public List<Model.Query.EmsGoalQuery> GetEmsGoalList(Model.Query.EmsGoalQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try 
            {
                query.Replace4MySQL();
                sqlCount.Append("select count(eg.row_id)  as totalCount ");
                sql.Append(" select eg.row_id,eg.department_code,para.parameterName  as department_name ,eg.year,eg.month,eg.goal_amount,eg.status,eg.create_time,mu.user_username  ");
                sqlFrom.Append(" from ems_goal eg ");
                sqlFrom.Append("  left JOIN manage_user mu on mu.user_id=eg.create_user  ");
                sqlFrom.Append(" LEFT JOIN (select parameterType,parameterCode,parameterName,remark from t_parametersrc  where parameterType='emsdepartment'  ) para on eg.department_code=para.parameterCode   ");

                sqlWhere.Append(" where 1=1  ");
                if (query.department_code != "")
                {
                    sqlWhere.AppendFormat("  and eg.department_code='{0}'  ", query.department_code);
                }
                //if (query.searchdate == 1)
                //{
                    sqlWhere.AppendFormat(" and eg.year={0}  and  eg.month={1}  ", query.date.Year, query.date.Month);
                //}

                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" ORDER BY eg.`year` DESC ,eg.`month` DESC  limit {0},{1} ;", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<EmsGoalQuery>(sql.ToString()+sqlFrom.ToString()+sqlWhere.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("EmsDao-->GetEmsGoalList-->" + ex.Message + sql.ToString()+sqlWhere.ToString(), ex);

            }
        }

        public List<EmsGoalQuery> GetDepartmentStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select parameterType,parameterCode as department_code,parameterName as department_name,remark from t_parametersrc where parameterType='emsdepartment' ;");
                return _access.getDataTableForObj<EmsGoalQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->GetDepartmentStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int SaveEmsGoal(EmsGoalQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("insert into ems_goal (`department_code`,`year`,`month`,`goal_amount`,`status`,`create_time`,`create_user`) ");
                sql.AppendFormat(" VALUES('{0}',{1} ,{2} ,{3} ,{4},'{5}',{6}); ",query.department_code,query.year,query.month,query.goal_amount,query.status,CommonFunction.DateTimeToString(query.create_time),query.user_userid);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->SaveEmsGoal-->"+sql.ToString()+ex.Message,ex);
            }
         
        }

        public List<EmsActualQuery> GetEmsActualList(EmsActualQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                query.Replace4MySQL();
                sqlCount.Append("select count(ea.row_id)  as totalCount ");
                sql.Append("select ea.row_id,ea.department_code,paral.parameterName  as department_name ,ea.year,ea.month,ea.`day`,ea.type,ea.cost_sum,ea.order_count,ea.amount_sum,ea.status ,ea.create_time,mu.user_username     ");
                sqlFrom.Append(" from ems_actual ea ");
                sqlFrom.Append(" LEFT JOIN (select parameterType,parameterCode,parameterName,remark from t_parametersrc  where parameterType='emsdepartment'  ) paral on ea.department_code=paral.parameterCode   ");
                sqlFrom.Append(" LEFT JOIN manage_user mu on ea.create_user=mu.user_id   ");
                sqlWhere.Append(" where 1=1 and ea.status=1 ");
                if (query.department_code != "")
                {
                    sqlWhere.AppendFormat("  and ea.department_code='{0}'  ", query.department_code);
                }
               sqlWhere.AppendFormat(" and ea.year={0}  and  ea.month={1} and ea.`day`={2} ", query.date.Year, query.date.Month, query.date.Day);
                if (query.type != 0)
                {
                    sqlWhere.AppendFormat("  and ea.type={0}  ",query.type);
                }
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString()+sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat("   ORDER BY ea.`year` DESC,ea.`month` desc,ea.`day` desc   limit {0},{1};", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<EmsActualQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->GetEmsActualList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);

            }
        }

        public int VerifyData(EmsGoalQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select count(eg.row_id) as totalCount from ems_goal eg where eg.`year`={0} and eg.`month`={1} and  eg.department_code='{2}'; ", query.year, query.month,query.department_code);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["totalCount"]);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->VerifyData-->"+sql.ToString()+ex.Message,ex);
            }
        }

        public int EditEmsGoal(EmsGoalQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("update ems_goal set goal_amount={0} where row_id={1};",query.goal_amount,query.row_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->EditEmsGoal-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int CostSumEmsActual(EmsActualQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("update ems_actual set cost_sum={0}  where row_id={1};", query.EmsValue, query.row_id);
                return _access.execCommand(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->CostSumEmsActual-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int OrderCountEmsActual(EmsActualQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("update ems_actual set order_count={0}  where row_id={1};", query.EmsValue, query.row_id);
                return _access.execCommand(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->CostSumEmsActual-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int AmountSumEmsActual(EmsActualQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("update ems_actual set amount_sum={0}  where row_id={1};", query.EmsValue, query.row_id);
                return _access.execCommand(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->CostSumEmsActual-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int SaveEmsActual(EmsActualQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append(" insert into ems_actual (`department_code`,`year`,`month`,`day`,`type`,`cost_sum`,`order_count`,`amount_sum`,`status`,`create_time`,`create_user`)");
                sql.AppendFormat(" VALUES('{0}',{1} ,{2} ,{3} ,{4},{5},{6},{7},{8},'{9}',{10}); ", query.department_code, query.year, query.month, query.day, query.type, query.cost_sum, query.order_count, query.amount_sum, query.status,CommonFunction.DateTimeToString(query.create_time),query.user_userid);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->SaveEmsActual-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int VerifyActualData(EmsActualQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select count(ea.row_id) as totalCount from ems_actual ea where ea.`year`={0} and ea.`month`={1}  and ea.`day`={2}  and  ea.department_code='{3}' and ea.type={4}; ", query.year, query.month, query.day, query.department_code, query.insertType);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["totalCount"]);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsDao-->VerifyActualData-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int IsExist(EmsActualQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat(" select ea.row_id   from ems_actual ea where ea.year='{0}' and ea.month='{1}' and ea.`day`='{2}' and department_code='{3}' and type={4} ;", query.predate.Year, query.predate.Month, query.day,  query.department_code_insert, query.insertType);

               return _access.getDataTable(sql.ToString()).Rows.Count;
            }
            catch(Exception ex)
            {
                throw new Exception(" EmsDao-->IsExist--> " + sql.ToString()+ ex.Message, ex);
            }
        }

        public string insertSql(EmsActualQuery query)
        {
               query.Replace4MySQL();
                StringBuilder sql = new StringBuilder();
                sql.Append(" insert into ems_actual (`department_code`,`year`,`month`,`day`,`type`,`cost_sum`,`order_count`,`amount_sum`,`status`,`create_time`,`create_user`)");
                sql.AppendFormat(" VALUES('{0}',{1} ,{2} ,{3} ,{4},{5},{6},{7},{8},'{9}',{10}); ", query.department_code_insert, query.predate.Year, query.predate.Month, query.day, query.insertType, query.cost_sum, query.order_count, query.amount_sum, query.status, CommonFunction.DateTimeToString(DateTime.Now), query.user_userid);
                return sql.ToString();
        }

        public bool execInsertSql(ArrayList arrayList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqls(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception(" EmsDao-->execInsertSql--> " + arrayList + ex.Message, ex);
            }
        }


        #region  wasted  code
        //改
        //public int insertPreDate(EmsActualQuery query)
        //{
        //    string  sql = string.Empty;
        //    StringBuilder sqlInsert = new StringBuilder();
        //    List<EmsGoalQuery> store = new List<EmsGoalQuery>();
        //    try
        //    {
        //       store=GetDepartmentStore();
        //     //   string[] departCode = { "event_chinatrust", "event_payeasy", "food_dep", "cdp_dep", "event_taishin", "stuff_dep" };

        //        for (int i = 1; i <= query.predate.Day; i++)
        //        {
        //            for (int j = 0; j < store.Count; j++)
        //            {
        //                sql = string.Format(" select ea.row_id   from ems_actual ea where ea.year='{0}' and ea.month='{1}' and ea.`day`='{2}' and department_code='{3}' and type={4} ;", query.predate.Year, query.predate.Month, i, store[j].department_code.ToString(),query.insertType);
        //                DataTable _dt = _access.getDataTable(sql.ToString());
        //                if (_dt.Rows.Count == 0)
        //                {
        //                    sqlInsert.Append(" insert into ems_actual (`department_code`,`year`,`month`,`day`,`type`,`cost_sum`,`order_count`,`amount_sum`,`status`,`create_time`,`create_user`)");
        //                    sqlInsert.AppendFormat(" VALUES('{0}',{1} ,{2} ,{3} ,{4},{5},{6},{7},{8},'{9}',{10}); ", store[j].department_code.ToString(), query.predate.Year, query.predate.Month, i, query.insertType, query.cost_sum, query.order_count, query.amount_sum, query.status, CommonFunction.DateTimeToString(DateTime.Now), query.user_userid);
        //                }

        //            }
        //        }
        //        if (sqlInsert.ToString() != "")
        //        {
        //            return _access.execCommand(sqlInsert.ToString());
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(" EmsDao-->insertPreDate--> "+sql.ToString()+sqlInsert.ToString()+ex.Message,ex);
        //    }
        //}
        //改
        //public int EditEmsActual(EmsActualQuery query)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        if (query.EmsActual == "cost_sum")//成本
        //        {
        //            sql.AppendFormat("update ems_actual set cost_sum={0}  where row_id={1};", query.EmsValue,query.row_id);
        //        }
        //        else if (query.EmsActual == "order_count")//訂單總數
        //        {
        //            sql.AppendFormat("update ems_actual set order_count={0}  where row_id={1};", query.EmsValue, query.row_id);
        //        }
        //        else//累計實績
        //        {
        //            sql.AppendFormat("update ems_actual set amount_sum={0}  where row_id={1};", query.EmsValue, query.row_id);
        //        }
        //        return _access.execCommand(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("EmsDao-->EditEmsActual-->" + sql.ToString() + ex.Message, ex);
        //    }
        //}
        #endregion
    }
}
