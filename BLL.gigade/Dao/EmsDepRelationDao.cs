using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model;
using DBAccess;
using System.Collections;

namespace BLL.gigade.Dao
{
 public   class EmsDepRelationDao
    {
     private IDBAccess _access;
     private string connStr;
     public EmsDepRelationDao(string connectionString)
     {
         _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
         this.connStr = connectionString;
     }

     public List<EmsDepRelation> EmsDepRelationList(EmsDepRelation query,out int totalCount)
     {
         StringBuilder sql = new StringBuilder();
         string sqlCount = string.Empty;
         StringBuilder sqlFrom = new StringBuilder();
         StringBuilder sqlWhere = new StringBuilder();
         totalCount = 0;
         try
         {
             query.Replace4MySQL();
             sql.Append(" select  edr.relation_id,edr.relation_type,edr.relation_order_count,edr.relation_order_cost,edr.relation_dep,edr.update_time,");
             sql.Append(" edr.create_time,edr.relation_create_type, edr.create_user,edr.update_user,edr.relation_year,edr.relation_month,edr.relation_day,para.parameterName as 'dep_name',mu.user_username ");
             sqlFrom.Append(" from ems_dep_relation edr ");
             sqlFrom.Append(" LEFT JOIN manage_user  mu on mu.user_id= edr.create_user LEFT JOIN (select parameterType,parameterCode,parameterName,remark from t_parametersrc  where parameterType='dep'  ) para on edr.relation_dep=para.parameterCode ");
             sqlWhere.Append(" where 1=1 ");
             if (query.relation_dep != 0)
             {
                 sqlWhere.AppendFormat(" and edr.relation_dep='{0}' ", query.relation_dep);
             }
             if (query.re_type != 0)
             {
                 sqlWhere.AppendFormat(" and edr.relation_type='{0}' ", query.re_type);
             }
                        if (query.datatype != 0)
             {
                 sqlWhere.AppendFormat(" and edr.relation_create_type='{0}' ", query.datatype);
             }
                 sqlWhere.AppendFormat(" and edr.relation_year='{0}' and  edr.relation_month='{1}' and edr.relation_day='{2}' ", query.date.Year,query.date.Month,query.date.Day);
                 if (query.IsPage)
             {
                 sqlCount = "select count(edr.relation_id) as totalCount "+sqlFrom.ToString()+sqlWhere.ToString();
                 DataTable _dt = _access.getDataTable(sqlCount.ToString());
                 if (_dt.Rows.Count > 0)
                 {
                     totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                 }
             }
                 sqlWhere.AppendFormat(" order by edr.relation_year desc,edr.relation_month desc,edr.relation_day desc limit {0},{1}; ", query.Start, query.Limit);
             return _access.getDataTableForObj<EmsDepRelation>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDao-->EmsDepRelationList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);
         }
     }
     

     public int IsPRSingleExist(EmsDepRelation query)
     {
         StringBuilder sql = new StringBuilder();
         try
         {
             query.Replace4MySQL();
             sql.AppendFormat("select edr.relation_id from ems_dep_relation edr where  edr.relation_year='{0}'  and edr.relation_month='{1}' and edr.relation_day='{2}' and edr.relation_create_type='{3}' and edr.relation_dep='{4}' and edr.relation_type='{5}';", query.predate.Year, query.predate.Month, query.insert_day, query.relation_create_type, query.dep_code_insert, query.relation_type_insert);
             return _access.getDataTable(sql.ToString()).Rows.Count;
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDepRelationDao-->IsPRSingleExist-->" + sql.ToString() + ex.Message, ex);
         }
     }

     public string insertSql(EmsDepRelation query)
     {
         StringBuilder sqlInsert = new StringBuilder();
         query.Replace4MySQL();
         sqlInsert.Append("insert into ems_dep_relation (`relation_type`,`relation_order_count`,`relation_order_cost`,`relation_dep`,`update_time`,`create_time`,`relation_create_type`,`create_user`,`update_user`,`relation_year`,`relation_month`,`relation_day`)");
         sqlInsert.AppendFormat("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');", query.relation_type_insert, query.relation_order_count, query.relation_order_cost, query.dep_code_insert, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.relation_create_type, query.create_user, query.update_user, query.predate.Year, query.predate.Month, query.insert_day);
         return sqlInsert.ToString();
     }

     public bool execSql(ArrayList array)
     {
         try
         {
             MySqlDao myDao = new MySqlDao(connStr);
             return myDao.ExcuteSqls(array);
         }
         catch (Exception ex)
         {
             throw new Exception(" EmsDao-->execSql--> " + array + ex.Message, ex);
         }
     }

     public List<EmsDepRelation> GetDepStore()
     {
         StringBuilder sql = new StringBuilder();
         try
         {
             sql.Append("select  parameterCode as 'dep_code',parameterName as 'dep_name' from t_parametersrc where parameterType='dep' ;");
             return _access.getDataTableForObj<EmsDepRelation>(sql.ToString());
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDao-->GetDepartmentStore-->" + sql.ToString() + ex.Message, ex);
         }
     }
   

     public int RelationOrderCount(EmsDepRelation query)
     {
         StringBuilder sql = new StringBuilder();
         try
         {
             query.Replace4MySQL();
             sql.AppendFormat(" update  ems_dep_relation set relation_order_count='{0}',update_user='{1}',update_time='{2}' where relation_id='{3}';", query.value, query.update_user, CommonFunction.DateTimeToString(DateTime.Now), query.relation_id);

             return _access.execCommand(sql.ToString());
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDepRelationDao-->RelationOrderCount-->" + sql.ToString() + ex.Message, ex);
         }
     }

     public int RelationOrderCost(EmsDepRelation query)
     {
         StringBuilder sql = new StringBuilder();
         try
         {
             query.Replace4MySQL();
             sql.AppendFormat(" update  ems_dep_relation set relation_order_cost='{0}',update_user='{1}',update_time='{2}' where relation_id='{3}';", query.value, query.update_user, CommonFunction.DateTimeToString(DateTime.Now), query.relation_id);

             return _access.execCommand(sql.ToString());
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDepRelationDao-->RelationOrderCount-->" + sql.ToString() + ex.Message, ex);
         }
     }


     public int VerifyData(EmsDepRelation query)
     {
         StringBuilder sql = new StringBuilder();
         try
         {
             query.Replace4MySQL();
             sql.AppendFormat("select count(edr.relation_id) as totalCount from ems_dep_relation edr where  edr.relation_year='{0}'  and edr.relation_month='{1}' and edr.relation_day='{2}' and edr.relation_create_type='{3}' and edr.relation_dep='{4}' and edr.relation_type='{5}';", query.relation_year, query.relation_month, query.relation_day, query.relation_create_type, query.relation_dep, query.relation_type);
             DataTable _dt = _access.getDataTable(sql.ToString());
             return Convert.ToInt32(_dt.Rows[0]["totalCount"]);
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDepRelationDao-->VerifyData-->" + sql.ToString() + ex.Message, ex);
         }
     }

     public int SaveEmsDepRe(EmsDepRelation query)
     {
         StringBuilder sql = new StringBuilder();
         try
         {
             query.Replace4MySQL();
             sql.Append("insert into ems_dep_relation (`relation_type`,`relation_order_count`,`relation_order_cost`,`relation_dep`,`update_time`,`create_time`,`relation_create_type`,`create_user`,`update_user`,`relation_year`,`relation_month`,`relation_day`)");
             sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');", query.relation_type, query.relation_order_count, query.relation_order_cost, query.relation_dep, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.relation_create_type, query.create_user, query.update_user, query.relation_year, query.relation_month, query.relation_day);
             return _access.execCommand(sql.ToString());
         }
         catch (Exception ex)
         {
             throw new Exception("EmsDepRelationDao-->SaveEmsDepRe-->"+sql.ToString()+ex.Message,ex);
         }
     }

    #region wasted  code
        //改
        //public int insertPreDate(EmsDepRelation query)
        //{

        //    string sql = string.Empty;//公關單
        //    string sql2= string.Empty;//報廢單
        //    StringBuilder sqlInsert = new StringBuilder();
        //    DateTime preDate = DateTime.Now.AddDays(-1);
        //    try
        //    {
        //        List<EmsDepRelation> store = GetDepStore();
        //        for (int i =1; i <= preDate.Day; i++)
        //        {
        //            for (int j = 0; j < store.Count; j++)
        //            {
        //                sql = string.Format("select edr.relation_id from ems_dep_relation edr where  edr.relation_year='{0}'  and edr.relation_month='{1}' and edr.relation_day='{2}' and edr.relation_create_type='{3}' and edr.relation_dep='{4}' and edr.relation_type=1;", preDate.Year, preDate.Month, i, query.relation_create_type, store[j].dep_code);
        //                sql2 = string.Format("select edr.relation_id from ems_dep_relation edr where  edr.relation_year='{0}'  and edr.relation_month='{1}' and edr.relation_day='{2}' and edr.relation_create_type='{3}' and edr.relation_dep='{4}' and edr.relation_type=2;", preDate.Year, preDate.Month, i, query.relation_create_type, store[j].dep_code);
        //                DataTable _dt = _access.getDataTable(sql.ToString());
        //                DataTable _dt2 = _access.getDataTable(sql2.ToString());
        //                if (_dt.Rows.Count == 0)
        //                {
        //                    sqlInsert.Append("insert into ems_dep_relation (`relation_type`,`relation_order_count`,`relation_order_cost`,`relation_dep`,`update_time`,`create_time`,`relation_create_type`,`create_user`,`update_user`,`relation_year`,`relation_month`,`relation_day`)");
        //                    sqlInsert.AppendFormat("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');", 1, query.relation_order_count, query.relation_order_cost, store[j].dep_code, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.relation_create_type, query.create_user, query.update_user, preDate.Year, preDate.Month, i);
        //                }
        //                if (_dt2.Rows.Count == 0)
        //                {
        //                    sqlInsert.Append("insert into ems_dep_relation (`relation_type`,`relation_order_count`,`relation_order_cost`,`relation_dep`,`update_time`,`create_time`,`relation_create_type`,`create_user`,`update_user`,`relation_year`,`relation_month`,`relation_day`)");
        //                    sqlInsert.AppendFormat("values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');", 2, query.relation_order_count, query.relation_order_cost, store[j].dep_code, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.relation_create_type, query.create_user, query.update_user, preDate.Year, preDate.Month, i);
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
        //        throw new Exception(" EmsDepRelationDao-->insertPreDate--> " + sql.ToString() + sqlInsert.ToString() + ex.Message, ex);
        //    }
        //}
  //改
     //public int EditEmsDepR(EmsDepRelation query)
     //{
     //    StringBuilder sql = new StringBuilder();
     //    try
     //    {
     //        if (query.emsdep == "relation_order_count")//訂單筆數
     //        {
     //            sql.AppendFormat(" update  ems_dep_relation set relation_order_count='{0}',update_user='{1}',update_time='{2}' where relation_id='{3}';", query.value, query.update_user, CommonFunction.DateTimeToString(DateTime.Now), query.relation_id);
     //        }
     //        else if (query.emsdep == "relation_order_cost")//訂單成本
     //        {
     //            sql.AppendFormat(" update  ems_dep_relation set relation_order_cost='{0}',update_user='{1}',update_time='{2}' where relation_id='{3}';", query.value, query.update_user, CommonFunction.DateTimeToString(DateTime.Now), query.relation_id);
     //        }
     //        return _access.execCommand(sql.ToString());
     //    }
     //    catch (Exception ex)
     //    {
     //        throw new Exception("EmsDepRelationDao-->EditEmsDepR-->" + sql.ToString() + ex.Message, ex);
     //    }
     //}
        #endregion
    }
}
