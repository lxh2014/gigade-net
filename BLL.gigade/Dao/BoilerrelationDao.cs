using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System.Web.Configuration.Common;

namespace BLL.gigade.Dao
{
    public class BoilerrelationDao : IBoilerrelationImplDao
    {
        private IDBAccess _access;
        private string connStr;
        private static string types=string.Empty;
        private static string describes=string.Empty;
        private static string inner_boiler_numbers=string.Empty;
        private static int state = 0;//0表示獲取對應安康內鍋型號和詳細信息為空時 1表示獲取對應安康內鍋型號和詳細信息

        public BoilerrelationDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 匯入安康內鍋型號對照表+GetintoBoilerrelation(DataRow[] dr)
        public int GetintoBoilerrelation(DataRow[] dr, out int total)
        {
            int result = 0;
            total = 0;
            boilerrelationQuery billation = new boilerrelationQuery();
            billation.add_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            billation.add_time=DateTime.Now;
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sb.Append("delete from boiler_relation where 1>0 ; ");//在執行插入之前先進行刪除操作
                for (int i = 1; i < dr.Length; i++)
                {
                   string strstring = string.Empty;
                    try
                    {
                        strstring = dr[i][4].ToString();
                        if (!string.IsNullOrEmpty(strstring))
                        {
                            string[] str = strstring.Replace("\n", "@").Split('@');
                            billation.boiler_type = str[0];//這個是對應安康內鍋型號
                            billation.boiler_describe = str[1];//對應安康內鍋型號詳細信息
                            types = billation.boiler_type;
                            describes = billation.boiler_describe;
                            state = 1;//獲取到了值
                        }
                        else
                        {
                            billation.boiler_type = types;//如果為空則保存原來的值
                            billation.boiler_describe = describes;
                            state = 0;//未獲取到值
                        }
                       
                        billation.inner_boiler_number = dr[i][3].ToString();//安康內鍋型號和描述 可能會出現為空的情況
                        if (!string.IsNullOrEmpty(billation.inner_boiler_number.Trim()))
                        {
                            inner_boiler_numbers = billation.inner_boiler_number.Trim();
                            string[] strinnerbn = billation.inner_boiler_number.Trim('\n').Replace("\n", "@").Replace("／", "@").Replace("/", "@").Split('@');
                            for (int j = 0; j < strinnerbn.Length; j++)
                            {
                                billation.inner_boiler_number = strinnerbn[j];//獲取內鍋編號 可能會有多個
                                billation.out_boiler_number = dr[i][2].ToString();//安康外鍋型號和描述
                                billation.boiler_remark = dr[i][5].ToString();
                                if (!string.IsNullOrEmpty(billation.out_boiler_number.Trim()))//如果內鍋型號不為null,或讀取為""
                                {
                                    string[] stroutbn = billation.out_boiler_number.Trim('\n').Replace("\n", "@").Replace("／", "@").Replace("/", "@").Split('@');
                                    for (int x = 0; x < stroutbn.Length; x++)
                                    {
                                        billation.out_boiler_number = stroutbn[x];//獲取外鍋編號 可能會有多個
                                        sb.AppendFormat("INSERT INTO boiler_relation(boiler_type,boiler_describe,inner_boiler_number,out_boiler_number,add_user,add_time,boiler_remark)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", billation.boiler_type, billation.boiler_describe, billation.inner_boiler_number, billation.out_boiler_number, billation.add_user, Common.CommonFunction.DateTimeToString(billation.add_time),billation.boiler_remark);
                                        // 计数器
                                        total++;
                                    }
                                }
                            }
                           
                        }
                        else//如果內鍋編號為空
                        {
                            if (state == 0)//表示未獲取對應安康內鍋型號和詳細信息
                            {
                                billation.inner_boiler_number = inner_boiler_numbers;
                                string[] strinnerbn = billation.inner_boiler_number.Trim('\n').Replace("\n", "@").Replace("／", "@").Replace("/", "@").Split('@');
                                for (int j = 0; j < strinnerbn.Length; j++)
                                {
                                    billation.inner_boiler_number = strinnerbn[j];//獲取內鍋編號 可能會有多個
                                    billation.out_boiler_number = dr[i][2].ToString();//安康外鍋型號和描述
                                    billation.boiler_remark = dr[i][5].ToString();//外鍋描述
                                    if (!string.IsNullOrEmpty(billation.out_boiler_number.Trim()))//如果內鍋型號不為null,或讀取為""
                                    {
                                        string[] stroutbn = billation.out_boiler_number.Trim('\n').Replace("\n", "@").Replace("／", "@").Replace("/", "@").Split('@');
                                        for (int x = 0; x < stroutbn.Length; x++)
                                        {
                                            billation.out_boiler_number = stroutbn[x];//獲取外鍋編號 可能會有多個
                                            sb.AppendFormat("INSERT INTO boiler_relation(boiler_type,boiler_describe,inner_boiler_number,out_boiler_number,add_user,add_time,boiler_remark)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", billation.boiler_type, billation.boiler_describe, billation.inner_boiler_number, billation.out_boiler_number, billation.add_user, Common.CommonFunction.DateTimeToString(billation.add_time), billation.boiler_remark);
                                            // 计数器
                                            total++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                billation.out_boiler_number = dr[i][2].ToString();//安康外鍋型號和描述
                                billation.boiler_remark = dr[i][5].ToString();//外鍋描述
                                if (!string.IsNullOrEmpty(billation.out_boiler_number.Trim()))//如果內鍋型號不為null,或讀取為""
                                {
                                    //解決最後兩行的問題
                                    string obn=billation.out_boiler_number.Trim();

                                    if (obn[0] > 127)//如果大於127表示此為漢字
                                    {
                                        sb.AppendFormat(" ");
                                    }
                                    else
                                    {
                                        string[] stroutbn = billation.out_boiler_number.Trim('\n').Replace("\n", "@").Replace("／", "@").Replace("/", "@").Split('@');
                                        for (int x = 0; x < stroutbn.Length; x++)
                                        {
                                            billation.out_boiler_number = stroutbn[x];//獲取外鍋編號 可能會有多個
                                            sb.AppendFormat("INSERT INTO boiler_relation(boiler_type,boiler_describe,inner_boiler_number,out_boiler_number,add_user,add_time,boiler_remark)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", billation.boiler_type, billation.boiler_describe, billation.inner_boiler_number, billation.out_boiler_number, billation.add_user, Common.CommonFunction.DateTimeToString(billation.add_time), billation.boiler_remark);
                                            // 计数器
                                            total++;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                }

                mySqlCmd.CommandText = sb.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("BoilerrelationDao.GetintoBoilerrelation-->" + ex.Message + sb.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return result;
        }
        #endregion


        public List<Model.Query.boilerrelationQuery> QueryBoilerRelationAll(Model.Query.boilerrelationQuery boilQuery, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            if (!string.IsNullOrEmpty(boilQuery.out_boiler_number))
            {
                sqlstr.AppendFormat(" and out_boiler_number like '%{0}%'",boilQuery.out_boiler_number);
            }
            if (!string.IsNullOrEmpty(boilQuery.inner_boiler_number))
            {
                sqlstr.AppendFormat(" and inner_boiler_number like '%{0}%'", boilQuery.inner_boiler_number);
            }

            if (!string.IsNullOrEmpty(boilQuery.Boiler_type_describe))
            {
                sqlstr.AppendFormat(" and (boiler_type like '%{0}%' or boiler_describe like '%{0}%')", boilQuery.Boiler_type_describe);
            }
            totalCount = 0;
            try
            {
                sql.Append(@"select br.*,mu.user_username from boiler_relation br LEFT JOIN manage_user mu on br.add_user=mu.user_id where 1=1 ");
                sql.Append(sqlstr.ToString());
                totalCount = 0;
                if (boilQuery.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1};", boilQuery.Start, boilQuery.Limit);
                }
                return _access.getDataTableForObj<boilerrelationQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BoilerrelationDao-->QueryBoilerRelationAll-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
