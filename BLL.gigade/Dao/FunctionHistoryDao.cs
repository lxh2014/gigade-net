using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class FunctionHistoryDao : IFunctionHistoryImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public FunctionHistoryDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public int Save(FunctionHistory fh)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO function_history (`function_id`,`user_id`,`operate_time`)VALUES({0},{1},'{2}');",fh.Function_Id,fh.User_Id,fh.Operate_Time.ToString("yyyy-MM-dd HH:mm:ss"));
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionHistoryDao-->Save" + ex.Message,ex);
            }
        }

        public List<FunctionHistoryCustom> Query(int function_id, int start, int limit, string condition, DateTime timeStart, DateTime timeEnd, out int total)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder strCount = new StringBuilder();
            total = 0;
            try
            {
                sb.AppendFormat(@"SELECT fh.row_id,fh.function_id,fh.user_id,fh.operate_time,f.functionName,u.user_username AS user_name FROM function_history fh    
    LEFT JOIN t_function f ON f.rowid =fh.function_id
    LEFT JOIN manage_user u ON u.user_id =fh.user_id WHERE fh.function_id = {0} ",function_id);

                strCount.AppendFormat(@"SELECT COUNT(fh.row_id) AS Count FROM function_history fh  
                                            INNER JOIN manage_user u ON u.user_id =fh.user_id
                                        WHERE fh.function_id = {0}", function_id);

                if(condition!="" && condition !=null)
                {
                    sb.AppendFormat(" AND u.user_username like '%{0}%' AND operate_time BETWEEN '{1}' AND '{2}' ",condition,timeStart,timeEnd);
                    strCount.AppendFormat(" AND u.user_username like '%{0}%' AND operate_time BETWEEN '{1}' AND '{2}' ", condition, timeStart, timeEnd);
                }
                sb.AppendFormat("ORDER BY fh.operate_time DESC LIMIT {0},{1}", start, limit);
                
                
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    total = Convert.ToInt32(_dt.Rows[0]["Count"]);
                }
                return _dbAccess.getDataTableForObj<FunctionHistoryCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionHistoryDao-->Query" + ex.Message, ex);
            }            
        }
    }
}
