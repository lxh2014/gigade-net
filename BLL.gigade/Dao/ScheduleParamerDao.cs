using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ScheduleParamerDao
    {
        private IDBAccess _dbAccess;
        public ScheduleParamerDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public DataTable GetScheduleParamerList(string code)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.AppendFormat("select para_id,para_value,para_name,para_status from schedule_paramer where schedule_code='{0}'", code);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerDao-->GetScheduleParamerList" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 獲取ScheduleParamer的list
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public List<ScheduleParamer> GetScheduleParameterList(ScheduleParamer query, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbcount = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sbcount.Append("SELECT count(para_id) ");
                sb.AppendFormat(@"SELECT para_id,para_value,para_name,para_status,schedule_code");
                sbwhere.Append(" FROM schedule_paramer WHERE 1=1 ");
                sbwhere.AppendFormat("ORDER BY para_id DESC ");
                DataTable _dt = new DataTable();
                _dt = _dbAccess.getDataTable(sbcount.ToString() + sbwhere.ToString());
                if (_dt != null)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sbwhere.AppendFormat(" LIMIT {0},{1} ", query.Start, query.Limit);
                return _dbAccess.getDataTableForObj<ScheduleParamer>(sb.ToString() + sbwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerDao-->GetScheduleParameterList" + ex.Message + sb.ToString() + sbwhere.ToString(), ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表新增
        /// </summary>
        /// <param name="query">新增數據</param>
        /// <returns></returns>
        public int InsertScheduleParamer(ScheduleParamer query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO schedule_paramer (para_value,para_name,para_status,schedule_code) VALUES (");
                sb.AppendFormat(" '{0}','{1}','{2}','{3}')", query.para_value, query.para_name, query.para_status, query.schedule_code);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerDao-->InsertScheduleParamer" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表編輯
        /// </summary>
        /// <param name="query">編輯信息</param>
        /// <returns></returns>
        public int UpdateScheduleParamer(ScheduleParamer query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE schedule_paramer SET para_value='{0}',para_name='{1}',schedule_code='{2}'", query.para_value, query.para_name, query.schedule_code);
                sb.AppendFormat("  WHERE para_id='{0}'", query.para_id);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerDao-->UpdateScheduleParamer" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表更改狀態
        /// </summary>
        /// <param name="query">更改信息</param>
        /// <returns></returns>
        public int UpdateActive(ScheduleParamer query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE schedule_paramer SET para_status='{0}' WHERE 1=1 ", query.para_status);
                if (query.para_id != 0)
                {
                    sb.AppendFormat("  and para_id='{0}'", query.para_id);
                }
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerDao-->UpdateActive" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表根據para_id刪除數據
        /// </summary>
        /// <param name="query">刪除數據信息</param>
        /// <returns></returns>
        public int DelScheduleParamer(ScheduleParamer query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("DELETE FROM schedule_paramer WHERE para_id='{0}'", query.para_id);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerDao-->DelScheduleParamer" + ex.Message + sb.ToString(), ex);
            }
        }
    }
}
